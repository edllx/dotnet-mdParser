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

    foreach (NestedTokenRange n in ranges)
    {
      maxIndex = Math.Max(maxIndex, n.Right);
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
              Left = top.Right + 1,
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
    if (stack.Count > 0)
    {
      NestedTokenRange top = stack.Peek();

      if (top.Right < maxIndex)
      {
        stack.Push(new()
        {
          Pattern = new PhrasePattern(),
          Left = top.Right + 1,
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
          new CodeBlockPattern(),
            new ULPattern(),
            new OLPattern(),
            new LinkPattern(),
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
        new LinkPattern(),
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
        new LinkPattern(),
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

  public static Pattern[] ULPatterns
  {
    get
    {
      return [
        new ULPattern(),
            new OLPattern(),
            new LIPattern(),
      ];
    }
  }


  public static Pattern[] OLPatterns
  {
    get
    {
      return [
        new ULPattern(),
            new OLPattern(),
            new LIPattern(),

      ];
    }
  }

  public static Pattern[] LIPatterns
  {
    get
    {
      return [
        new CheckBoxPattern(),
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

      foreach (Token token in rr.Children)
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
      string body = match.Groups[2].Value.AsSpan().Slice(r.Left, r.Right - r.Left + 1).ToString();

      Tokenizer tokenizer = new(body, new()
          {
          Patterns = Pattern.HeaderPatterns,
          Depth = depth + 1
          });

      Root rr = tokenizer.Generate();

      foreach (Token token in rr.Children)
      {
        children.Add(token);
      }
    }

    switch (match.Groups[1].Value.Length)
    {
      case 1:
        root.Children.Add(Token.H1(children));
        break;
      case 2:
        root.Children.Add(Token.H2(children));
        break;
      case 3:
        root.Children.Add(Token.H3(children));
        break;
      case 4:
        root.Children.Add(Token.H4(children));
        break;
      case 5:
        root.Children.Add(Token.H5(children));
        break;
      case 6:
        root.Children.Add(Token.H6(children));
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


    root.Children.Add(Token.Paragraph(rr.Children, depth));
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
        root.Children.Add(Token.Phrase(body, depth));
        continue;
      }

      Tokenizer tokenizer = new(body, new()
          {
          Patterns = Pattern.PhrasePatterns,
          Depth = depth 
          });

      Root rr = tokenizer.Generate();
      
      

      foreach (Token token in rr.Children)
      {
        root.Children.Add(token);
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

    root.Children.Add(Token.NewLine(1));
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
    root.Children.Add(Token.Bold(children, depth));
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
    root.Children.Add(Token.Italic(children, depth));
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
    root.Children.Add(Token.Strikethrough(children, depth));
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
    root.Children.Add(Token.Highlight(children, depth));
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
    root.Children.Add(Token.InlineCode(children, depth));
  }
}

public class LinkPattern : Pattern
{
  public LinkPattern() : base(new(@"!?\[([^\n\]\[]|!\[[^\n]+\]\([^\n]+\))*?\]\([^\n]*?\)\n?"), new(@"!?\[(([^\n\]\[]|!\[[^\n]+\]\([^\n]+\))*?)\]\(([^\n]*?)\)\n?"))
  {
  }

  public override bool Generate(Root root, ReadOnlySpan<char> source, int depth)
  {
    Match? match = IsMatch(source);
    if (match is null || match.Index != 0) { return false; }

    Len = match.Length;

    string name = match.Groups[1].Value;
    string link = match.Groups[3].Value;

    if (source[0] == '!')
    {
      root.Children.Add(Token.ImageLink(name, link, depth));
    }
    else {
      root.Children.Add(Token.Link(name, link, depth));
    }

    return true;
  }

  protected override void CreateTextStyleToken(Root root, List<Token> children, int depth)
  {
    root.Children.Add(Token.InlineCode(children, depth));
  }
}

public class ULPattern : Pattern
{

  public ULPattern() : base(new Regex(@"([ \t]*- [^\n]+)\n?(([ \t]*(([0-9]+\.)|-) ([^\n]+\n)|([^\n]+\n?))*)(?:\n?)"),null)
  {}

  public override bool Generate(Root root, ReadOnlySpan<char> source, int depth)
  {
    Match? match = IsMatch(source);
    if (match is null || match.Index != 0) { return false; }

    Len = match.Length;

    UL ul =Token.UL([], 1); 
    ListBuilder builder = new(ul);

    foreach(string s in match.Value.Split("\n"))
    {
      if(s == ""){continue;}

      Tokenizer tokenizer = new(s, new()
          {
          Patterns = [
          new LIPattern(),
          new PhrasePattern()
          ],
          Depth = depth + 1
          });

      Root rr = tokenizer.Generate();

      if(rr.Children.Count==0){
        continue;
      }

      builder.Push(rr.Children[0]);
      
    }
    root.Children.Add(ul);

    return true;
  }
}

public class OLPattern : Pattern
{

  public OLPattern() : base(new Regex(@"([ \t]*[0-9]+\. [^\n]+)\n?(([ \t]*(([0-9]+\.)|-) ([^\n]+\n)|([^\n]+\n?))*)(?:\n?)"),null)
  {}

  public override bool Generate(Root root, ReadOnlySpan<char> source, int depth)
  {
    Match? match = IsMatch(source);
    if (match is null || match.Index != 0) { return false; }

    Len = match.Length;

    OL ol =Token.OL([], 1); 
    ListBuilder builder = new(ol);

    foreach(string s in match.Value.Split("\n"))
    {
      if(s == ""){continue;}

      Tokenizer tokenizer = new(s, new()
          {
          Patterns = [
          new LIPattern(),
          new PhrasePattern()
          ],
          Depth = depth + 1
          });

      Root rr = tokenizer.Generate();

      if(rr.Children.Count==0){
        continue;
      }

      builder.Push(rr.Children[0]);
      
    }
    root.Children.Add(ol);

    return true;
  }
}

public class LIPattern : Pattern
{

  public LIPattern() : base(new Regex(@"([ \t]*(([0-9]+\.)|-) [^\n]+)\n?"),new Regex(@"(([ \t]*)(([0-9]+\.)|-) ([^\n]+))\n?"))
  {}

  public override bool Generate(Root root, ReadOnlySpan<char> source, int depth)
  {
    Match? match = IsMatch(source);
    if (match is null || match.Index != 0) { return false; }

    Len = match.Length;

    Tokenizer tokenizer = new(match.Groups[5].Value, new()
        {
        Patterns = Pattern.LIPatterns,
        Depth = depth + 1
        });

    Root rr = tokenizer.Generate();
    Type parentType = typeof(UL);
    int offset = match.Groups[2].Length;
    LI? li = null; 

    if(match.Groups[3].Value == "-")
    {
      parentType = typeof(UL);
    }
    else{
      parentType = typeof(OL);
    }
    
    if(rr.Children[0] is CheckBox)
    {
      li = (CheckBox) rr.Children[0];
    }
    else{
      li = Token.LI(rr.Children, depth);
    }

    li.Offset = offset;
    li.ParentType = parentType;

    root.Children.Add(li);
    return true;
  }
}

public class CheckBoxPattern : Pattern
{

  public CheckBoxPattern() : base(new Regex(@"(\[([xX ])\] ([^\n]+))\n?"), null)
  {
  }

  public override bool Generate(Root root, ReadOnlySpan<char> source, int depth)
  {
    Match? match = IsMatch(source);
    if (match is null || match.Index != 0) { return false; }

    Len = match.Length;

    bool Done = new Regex("^[xX]$").Match(match.Groups[2].Value).Success;

    Tokenizer tokenizer = new(match.Groups[3].Value, new()
        {
        Patterns = Pattern.LIPatterns,
        Depth = depth + 1
        });

    Root rr = tokenizer.Generate();


    CheckBox checkbox = Token.CheckBox(rr.Children,depth,Done); 


    root.Children.Add(checkbox);
    return true;
  }
}

public class CodeBlockPattern : Pattern
{

  public CodeBlockPattern() : base(new Regex(@"((?<=[^`]|^)`{3}(?=[^`]|$))(.|\n)*?((?<=[^`]|^)`{3}(?=[^`]|$))"),null)
  {
  }

  public override bool Generate(Root root, ReadOnlySpan<char> source, int depth)
  {
    Match? match = IsMatch(source);
    if (match is null || match.Index != 0) { return false; }
    Len = match.Length;

    List<String> parts = match.Value.AsSpan().Slice(3,match.Value.Length-6).ToString().Split("\n").ToList();
    string language = "";

    Match lgMatch = new Regex(" *([^\n]*)").Match(parts[0]);
    parts.RemoveAt(0);

    language = lgMatch.Groups[1].Value;

    CodeBlock code = Token.CodeBlock(string.Join("\n",parts),depth,language);
    root.Children.Add(code);
    return true;
  }
}
