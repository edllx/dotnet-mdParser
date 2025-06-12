using System.Text.RegularExpressions;

namespace edllx.dotnet.mdParser;

public interface IPattern 
{
  Token? Generate(ReadOnlySpan<char> source,int depth);
}

public abstract class Pattern : IPattern
{
  protected Regex Expression {get;init;}
  protected Regex? ExpressionGroup  {get;init;}
  public int Len {get;protected set;}

  protected Pattern(Regex expression, Regex? expressionGroup)
  {
    Expression = expression;
    ExpressionGroup = expressionGroup;
  }

  public abstract Token? Generate(ReadOnlySpan<char> source,int depth);

  protected virtual Match? IsMatch(ReadOnlySpan<char> source)
  {
    bool isMatch = false;

    foreach (ValueMatch match in Expression.EnumerateMatches(source))
    {
      isMatch = true;
    }

    if(!isMatch){return null;}

    if(ExpressionGroup is null)
    {
      return Expression.Match(source.ToString());
    }

    return ExpressionGroup.Match(source.ToString());
  }
}

public class HeadingPattern : Pattern
{
  public HeadingPattern() : base(new("^#{1,6} [^\n]+\n?"),new("^(#{1,6}) ([^\n]+)\n?"))
  {
  }

  public override Token? Generate(ReadOnlySpan<char> source,int depth)
  {
    Match? match = IsMatch(source);
    if(match is null){return null;}

    Len = match.Length;

    Tokenizer tokenizer = new(match.Groups[2].Value,new(){
        Depth = depth+1,
        });

    Root r = tokenizer.Generate();
    List<Token> childs = r.Childrens;

    switch (match.Groups[1].Value.Length)
    {
      case 1 :
        return Token.H1(childs); 
      case 2 :
        return Token.H2(childs); 
      case 3 :
        return Token.H3(childs); 
      case 4 :
        return Token.H4(childs); 
      case 5 :
        return Token.H5(childs); 
      case 6 :
        return Token.H6(childs); 
      default:
        return null;
    }
  }
}

public class PhrasePattern : Pattern
{
  private readonly Regex _expression = new("[^\n]+");

  public PhrasePattern(): base(new("[^\n]+"),null)
  {
  }

  public override Token? Generate(ReadOnlySpan<char> source,int depth)
  {
    Match? match = IsMatch(source);
    if(match is null){return null;}

    Len = match.Length;

    return Token.Phrase(match.Value,depth);
  }
}

public class NewLinePattern : Pattern
{
  private readonly Regex _expression = new("\n");

  public NewLinePattern() : base(new("\n"),null)
  {
  }

  public override Token? Generate(ReadOnlySpan<char> source,int depth)
  {
    Match? match = IsMatch(source);
    if(match is null){return null;}

    Len = match.Length;

    return Token.NewLine(1);
  }
}
