using System.Text.RegularExpressions;

namespace edllx.dotnet.mdParser;

public interface IPattern
{
  Token? Generate(ReadOnlySpan<char> source, int depth);
}

public class NestedTokenRange
{
  public Pattern? Pattern {get;set;}
  public int Left {get;set;}
  public int Right {get;set;}

  public override string ToString()
  {
    return $"{Pattern?.GetType().Name.Split(".").Last()}: [{Left} - {Right}]";
  }


  public static List<NestedTokenRange> MergeRanges(List<NestedTokenRange> ranges)
  {
    ranges.Sort((a,b)=>a.Left - b.Left);
    Stack<NestedTokenRange> stack = [];

    for(int i = 0;i<ranges.Count;i++)
    {
      NestedTokenRange current = ranges[i];
      try
      {
        NestedTokenRange top = stack.Pop();

        if(top.Pattern!.GetType() == typeof(PhrasePattern))
        {
          top.Right = current.Left;
          stack.Push(top);
          stack.Push(current);

          continue;
        }

        // 2+ distinct zones  
        // push top => inner range => current
        if (top.Right < current.Left){
          stack.Push(top);

          if(top.Right +1 < current.Left -1)
          {
            // push inner range
            stack.Push(new(){
                Pattern = new PhrasePattern(),
                Left = top.Right + 1,
                Right = current.Left 
                });
          }

          stack.Push(current);
        }else{
          stack.Push(top);
          
          int r = Math.Max(top.Right,current.Right);

          if (r>=top.Right +1)
          {

            stack.Push(new(){
                Pattern = current.Pattern,
                Left = top.Right+1,
                Right = current.Right 
                });
          }
        }

      }
      catch (InvalidOperationException )
      {
        stack.Push(current);
        continue;
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

  public static Pattern[] GeneralPatterns {get{
    return [
      new HeadingPattern(), 
          new BoldPattern(),
          new PhrasePattern(),
          new NewLinePattern(),
    ];
  }}


  public static Pattern[] HeaderPatterns {get{
    return [
      new BoldPattern(),
          new PhrasePattern(),
    ];
  }}

  public static Pattern[] BoldPatterns {get{
    return [
      new PhrasePattern(),
    ];
  }}

  protected Pattern(Regex expression, Regex? expressionGroup)
  {
    Expression = expression;
    ExpressionGroup = expressionGroup;
  }

  public abstract Token? Generate(ReadOnlySpan<char> source, int depth);

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

  protected List<NestedTokenRange> EnumarateMatchRange(ReadOnlySpan<char> source,Pattern[] patterns)
  {

    List<NestedTokenRange> arr = [];
    foreach(Pattern p in patterns)
    {
      foreach (ValueMatch match in p.Expression.EnumerateMatches(source))
      {
        ReadOnlySpan<char> element = source.Slice(match.Index, match.Length); // "World", "Span<T>"
        arr.Add(new(){
            Pattern = p,
            Left = match.Index,
            Right = match.Index + match.Length
            });
      }
    }

    arr = NestedTokenRange.MergeRanges(arr);
    arr.Sort((a,b)=>a.Left-b.Left);
    return arr;
  }
}

public class HeadingPattern : Pattern
{
  public HeadingPattern() : base(new("^#{1,6} [^\n]+\n?"), new("^(#{1,6}) ([^\n]+)\n?"))
  {
  }

  public override Token? Generate(ReadOnlySpan<char> source, int depth)
  {
    Match? match = IsMatch(source);
    if (match is null) { return null; }

    Len = match.Length;

    List<Token> children = [];
    List<NestedTokenRange> ranges = EnumarateMatchRange(match.Groups[2].Value.AsSpan(),Pattern.HeaderPatterns);

    foreach(NestedTokenRange r in ranges)
    {
      string body = match.Groups[2].Value.AsSpan().Slice(r.Left,r.Right - r.Left).ToString();

      Tokenizer tokenizer = new(body,new(){
          Patterns = Pattern.HeaderPatterns, 
          Depth = depth +1
          });

      Root root = tokenizer.Generate();

      foreach (Token token in root.Childrens)
      {
        children.Add(token);
      }
    }

    switch (match.Groups[1].Value.Length)
    {
      case 1:
        return Token.H1(children);
      case 2:
        return Token.H2(children);
      case 3:
        return Token.H3(children);
      case 4:
        return Token.H4(children);
      case 5:
        return Token.H5(children);
      case 6:
        return Token.H6(children);
      default:
        return null;
    }
  }
}

public class PhrasePattern : Pattern
{
  public PhrasePattern() : base(new("[^\n]+"), null)
  {
  }

  public override Token? Generate(ReadOnlySpan<char> source, int depth)
  {
    Match? match = IsMatch(source);
    if (match is null) { return null; }

    Len = match.Length;

    return Token.Phrase(match.Value, depth);
  }
}

public class NewLinePattern : Pattern
{
  public NewLinePattern() : base(new("\n"), null)
  {
  }

  public override Token? Generate(ReadOnlySpan<char> source, int depth)
  {
    Match? match = IsMatch(source);
    if (match is null) { return null; }

    Len = match.Length;

    return Token.NewLine(1);
  }
}


public class BoldPattern : Pattern
{
  public BoldPattern() : base(new(@"((?<=[^\*]|^)\*{2}(?=[^\*]|$))[^\n]*?((?<=[^\*]|^)\*{2}(?=[^\*]|$))"), null)
  {
  }

  public override Token? Generate(ReadOnlySpan<char> source, int depth)
  {
    Match? match = IsMatch(source);
    if (match is null) { return null; }

    Len = match.Length;

    List<Token> children = [];
    ReadOnlySpan<char> boldBody = match.Value.AsSpan().Slice(2,match.Length-4);
    List<NestedTokenRange> ranges = EnumarateMatchRange(boldBody,Pattern.BoldPatterns);

    foreach(NestedTokenRange r in ranges)
    {
      string body = boldBody.Slice(r.Left,r.Right - r.Left ).ToString();

      Tokenizer tokenizer = new(body,new(){
          Patterns = Pattern.HeaderPatterns,
          Depth = depth +1
          });

      Root root = tokenizer.Generate();

      foreach (Token token in root.Childrens)
      {
        children.Add(token);
      }
    }

    return Token.Bold(children,depth);
  }
}
