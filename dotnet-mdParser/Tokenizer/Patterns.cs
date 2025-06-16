using System.Text.RegularExpressions;

namespace edllx.dotnet.mdParser;

public interface IPattern
{
  bool Generate(Root root, ReadOnlySpan<char> source, int depth);
}

public class NestedTokenRange
{
  public Pattern? Pattern { get; set; }
  public int Left { get; set; }
  public int Right { get; set; }

  public override string ToString()
  {
    return $"{Pattern?.GetType().Name.Split(".").Last()}: [{Left} - {Right}]";
  }

  public static List<NestedTokenRange> MergeRanges(List<NestedTokenRange> ranges)
  {
    ranges.Sort((a, b) => a.Left - b.Left);
    Stack<NestedTokenRange> stack = [];

    int maxIndex = 0;

    foreach(NestedTokenRange n in ranges)
    {
      maxIndex = Math.Max(maxIndex,n.Right);
    }

    for (int i = 0; i < ranges.Count; i++)
    {
      NestedTokenRange current = ranges[i];
      try
      {
        NestedTokenRange top = stack.Pop();

        if (top.Pattern!.GetType() == typeof(PhrasePattern))
        {
          top.Right = current.Left - 1;
          stack.Push(top);
          stack.Push(current);

          continue;
        }

        // 2+ distinct zones  
        // push top => inner range => current
        if (top.Right < current.Left)
        {
          stack.Push(top);

          if (top.Right + 1 < current.Left - 1)
          {
            // push inner range
            stack.Push(new()
            {
              Pattern = new PhrasePattern(),
              Left = top.Right + 1,
              Right = current.Left - 1
            });
          }

          stack.Push(current);
        }
        else
        {
          stack.Push(top);

          int r = Math.Max(top.Right, current.Right);

          if (r >= top.Right + 1)
          {

            stack.Push(new()
            {
              Pattern = current.Pattern,
              Left = top.Right,
              Right = current.Right
            });
          }
        }

      }
      catch (InvalidOperationException)
      {
        stack.Push(current);
        continue;
      }
    }

    // Add last phrase range
    if(stack.Count>0)
    {
      NestedTokenRange top = stack.Peek();

      if (top.Right<maxIndex)
      {
        stack.Push(new()
            {
            Pattern = new PhrasePattern(),
            Left = top.Right +1,
            Right = maxIndex
            });
      }
    }

    return stack.ToList();
  }
}


public abstract class Pattern : IPattern
{
  protected Regex Expression { get; init; }
  protected Regex? ExpressionGroup { get; init; }
  public int Len { get; protected set; }
  protected int DelimiterSize { get; set; } = 1;
  protected Pattern[] NestedPaterns { get; set; } = [];

  public static Pattern[] GeneralPatterns
  {
    get
    {
      return [
        new NewLinePattern(),
            new HeadingPattern(),
            new ParagraphPattern(),
            new BoldPattern(),
            new ItalicPattern(),
            new StrikethroughPattern(),
            new HighlightPattern(),
            new InlineCodePattern(),
            new PhrasePattern(),

      ];
    }
  }

  public static Pattern[] PhrasePatterns
  {
    get
    {
      return [
        new BoldPattern(),
            new ItalicPattern(),
            new StrikethroughPattern(),
            new HighlightPattern(),
            new InlineCodePattern(),
            new PhrasePattern(),
      ];
    }
  }

  public static Pattern[] HeaderPatterns
  {
    get
    {
      return [
        new BoldPattern(),
            new ItalicPattern(),
            new StrikethroughPattern(),
            new HighlightPattern(),
            new InlineCodePattern(),
            new PhrasePattern(),
      ];
    }
  }

  public static Pattern[] BoldPatterns
  {
    get
    {
      return [
        new ItalicPattern(),
            new StrikethroughPattern(),
            new HighlightPattern(),
            new PhrasePattern(),
      ];
    }
  }

  public static Pattern[] ItalicPatterns
  {
    get
    {
      return [
        new BoldPattern(),
            new StrikethroughPattern(),
            new HighlightPattern(),
            new PhrasePattern(),
      ];
    }
  }

  public static Pattern[] StrikethroughPatterns
  {
    get
    {
      return [
        new BoldPattern(),
            new ItalicPattern(),
            new HighlightPattern(),
            new PhrasePattern(),
      ];
    }
  }


  public static Pattern[] HighlightPatterns
  {
    get
    {
      return [
        new BoldPattern(),
            new ItalicPattern(),
            new StrikethroughPattern(),
            new PhrasePattern(),
      ];
    }
  }

  public static Pattern[] InlineCodePatterns
  {
    get
    {
      return [
        new PhrasePattern(),
      ];
    }
  }



  protected Pattern(Regex expression, Regex? expressionGroup)
  {
    Expression = expression;
    ExpressionGroup = expressionGroup;
  }

  public abstract bool Generate(Root root, ReadOnlySpan<char> source, int depth);

  protected virtual Match? IsMatch(ReadOnlySpan<char> source)
  {
    bool isMatch = false;

    foreach (ValueMatch match in Expression.EnumerateMatches(source))
    {
      isMatch = true;
    }

    if (!isMatch) { return null; }

    if (ExpressionGroup is null)
    {
      return Expression.Match(source.ToString());
    }

    return ExpressionGroup.Match(source.ToString());
  }

  protected List<NestedTokenRange> EnumarateMatchRange(ReadOnlySpan<char> source, Pattern[] patterns)
  {

    List<NestedTokenRange> arr = [];
    foreach (Pattern p in patterns)
    {
      foreach (ValueMatch match in p.Expression.EnumerateMatches(source))
      {
        ReadOnlySpan<char> element = source.Slice(match.Index, match.Length);
        arr.Add(new()
            {
            Pattern = p,
            Left = match.Index,
            Right = match.Index + match.Length - 1
            });
      }
    }

    arr = NestedTokenRange.MergeRanges(arr);
    arr.Sort((a, b) => a.Left - b.Left);
    return arr;
  }

  protected bool GenerateTextStyle(Root root, ReadOnlySpan<char> source, int depth)
  {
    Match? match = IsMatch(source);
    if (match is null || match.Index != 0) { return false; }

    Len = match.Length;

    List<Token> children = [];
    ReadOnlySpan<char> text = match.Value.AsSpan().Slice(DelimiterSize, match.Length - DelimiterSize * 2);
    List<NestedTokenRange> ranges = EnumarateMatchRange(text, NestedPaterns);

    foreach (NestedTokenRange r in ranges)
    {
      string body = text.Slice(r.Left, r.Right - r.Left + 1).ToString();

      Tokenizer tokenizer = new(body, new()
          {
          Patterns = Pattern.HeaderPatterns,
          Depth = depth + 1
          });

      Root rr = tokenizer.Generate();

      foreach (Token token in rr.Childrens)
      {
        children.Add(token);
      }
    }

    CreateTextStyleToken(root, children, depth);
    return true;
  }


  protected virtual void CreateTextStyleToken(Root root, List<Token> children, int depth)
  {
  }
}

public class HeadingPattern : Pattern
{
  public HeadingPattern() : base(new("^#{1,6} [^\n]+\n?"), new("^(#{1,6}) ([^\n]+)\n?"))
  {
  }

  public override bool Generate(Root root, ReadOnlySpan<char> source, int depth)
  {
    Match? match = IsMatch(source);
    if (match is null) { return false; }

    Len = match.Length;

    List<Token> children = [];
    List<NestedTokenRange> ranges = EnumarateMatchRange(match.Groups[2].Value.AsSpan(), Pattern.HeaderPatterns);

    foreach (NestedTokenRange r in ranges)
    {
      string body = match.Groups[2].Value.AsSpan().Slice(r.Left, r.Right - r.Left +1).ToString();

      Tokenizer tokenizer = new(body, new()
          {
          Patterns = Pattern.HeaderPatterns,
          Depth = depth + 1
          });

      Root rr = tokenizer.Generate();

      foreach (Token token in rr.Childrens)
      {
        children.Add(token);
      }
    }

    switch (match.Groups[1].Value.Length)
    {
      case 1:
        root.Childrens.Add(Token.H1(children));
        break;
      case 2:
        root.Childrens.Add(Token.H2(children));
        break;
      case 3:
        root.Childrens.Add(Token.H3(children));
        break;
      case 4:
        root.Childrens.Add(Token.H4(children));
        break;
      case 5:
        root.Childrens.Add(Token.H5(children));
        break;
      case 6:
        root.Childrens.Add(Token.H6(children));
        break;
      default:
        return false;
    }

    return true;
  }
}


public class ParagraphPattern : Pattern
{

  public ParagraphPattern() : base(new("([^\n]+\n)+"), null)
  {
  }


  public override bool Generate(Root root, ReadOnlySpan<char> source, int depth)
  {
    Match? match = IsMatch(source);
    if (match is null || match.Index != 0) { return false; }

    Len = match.Length;

    string body = match.Value.ReplaceLineEndings("");

    Tokenizer tokenizer = new(body, new()
        {
        Depth = depth + 1
        });

    Root rr = tokenizer.Generate();


    root.Childrens.Add(Token.Paragraph(rr.Childrens,depth));
    return true;
  }
}

public class PhrasePattern : Pattern
{
  public PhrasePattern() : base(new("[^\n]+"), null)
  {
  }

  public override bool Generate(Root root, ReadOnlySpan<char> source, int depth)
  {
    Match? match = IsMatch(source);
    if (match is null) { return false; }

    Len = match.Length;

    List<Token> children = [];
    ReadOnlySpan<char> boldBody = match.Value.AsSpan();
    List<NestedTokenRange> ranges = EnumarateMatchRange(boldBody, Pattern.PhrasePatterns);

    foreach (NestedTokenRange r in ranges)
    {
      string body = boldBody.Slice(r.Left, r.Right - r.Left + 1).ToString();

      if (r.Pattern!.GetType() == typeof(PhrasePattern))
      {
        root.Childrens.Add(Token.Phrase(body, depth));
        continue;
      }

      Tokenizer tokenizer = new(body, new()
          {
          Patterns = Pattern.PhrasePatterns,
          Depth = depth
          });

      Root rr = tokenizer.Generate();

      foreach (Token token in rr.Childrens)
      {
        root.Childrens.Add(token);
      }
    }
    return true;
  }
}

public class NewLinePattern : Pattern
{
  public NewLinePattern() : base(new("\n"), null)
  {
  }

  public override bool Generate(Root root, ReadOnlySpan<char> source, int depth)
  {
    Match? match = IsMatch(source);
    if (match is null || match.Index != 0) { return false; }

    Len = 1;

    root.Childrens.Add(Token.NewLine(1));
    return true;
  }
}

public class BoldPattern : Pattern
{
  public BoldPattern() : base(new(@"((?<=[^\*]|^)\*{2}(?=[^\*]|$))[^\n]*?((?<=[^\*]|^)\*{2}(?=[^\*]|$))"), null)
  {
  }

  public override bool Generate(Root root, ReadOnlySpan<char> source, int depth)
  {
    DelimiterSize = 2;
    NestedPaterns = Pattern.BoldPatterns;
    return GenerateTextStyle(root, source, depth);
  }

  protected override void CreateTextStyleToken(Root root, List<Token> children, int depth)
  {
    root.Childrens.Add(Token.Bold(children, depth));
  }
}


public class ItalicPattern : Pattern
{
  public ItalicPattern() : base(new(@"((?<=[^_]|^)_(?=[^_]|$))[^\n]+?((?<=[^_]|^)_(?=[^_]|$))"), null)
  {
  }

  public override bool Generate(Root root, ReadOnlySpan<char> source, int depth)
  {
    DelimiterSize = 1;
    NestedPaterns = Pattern.ItalicPatterns;
    return GenerateTextStyle(root, source, depth);
  }

  protected override void CreateTextStyleToken(Root root, List<Token> children, int depth)
  {
    root.Childrens.Add(Token.Italic(children, depth));
  }
}

public class StrikethroughPattern : Pattern
{
  public StrikethroughPattern() : base(new(@"((?<=[^~~]|^)~~(?=[^~~]|$))[^\n]+?((?<=[^~~]|^)~~(?=[^~~]|$))"), null)
  {
  }

  public override bool Generate(Root root, ReadOnlySpan<char> source, int depth)
  {
    DelimiterSize = 2;
    NestedPaterns = Pattern.StrikethroughPatterns;
    return GenerateTextStyle(root, source, depth);
  }

  protected override void CreateTextStyleToken(Root root, List<Token> children, int depth)
  {
    root.Childrens.Add(Token.Strikethrough(children, depth));
  }
}

public class HighlightPattern : Pattern
{
  public HighlightPattern() : base(new(@"((?<=[^==]|^)==(?=[^==]|$))[^\n]+?((?<=[^==]|^)==(?=[^==]|$))"), null)
  {
  }

  public override bool Generate(Root root, ReadOnlySpan<char> source, int depth)
  {
    DelimiterSize = 2;
    NestedPaterns = Pattern.HeaderPatterns;
    return GenerateTextStyle(root, source, depth);
  }

  protected override void CreateTextStyleToken(Root root, List<Token> children, int depth)
  {
    root.Childrens.Add(Token.Highlight(children, depth));
  }
}


public class InlineCodePattern : Pattern
{
  public InlineCodePattern() : base(new(@"((?<=[^`]|^)`(?=[^`]|$))[^\n]+?((?<=[^`]|^)`(?=[^`]|$))"), null)
  {
  }

  public override bool Generate(Root root, ReadOnlySpan<char> source, int depth)
  {
    DelimiterSize = 1;
    NestedPaterns = Pattern.InlineCodePatterns;
    return GenerateTextStyle(root, source, depth);
  }

  protected override void CreateTextStyleToken(Root root, List<Token> children, int depth)
  {
    root.Childrens.Add(Token.InlineCode(children, depth));
  }
}
