namespace edllx.dotnet.mdParser;

public class Bold : Token
{
    internal Bold(List<Token> childrens, int depth) : base(childrens, "", depth)
    {
    }
}

public class Italic : Token
{
    internal Italic(List<Token> childrens, int depth) : base(childrens, "", depth)
    {
    }
}

public class Strikethrough : Token
{
    internal Strikethrough(List<Token> childrens, int depth) : base(childrens, "", depth)
    {
    }
}


public class Highlight : Token
{
    internal Highlight(List<Token> childrens, int depth) : base(childrens, "", depth)
    {
    }
}


public class InlineCode : Token
{
    internal InlineCode(List<Token> childrens, int depth) : base(childrens, "", depth)
    {
    }
}

public class CodeBlock : Token
{
  public string Language {get;} = "";
  private int MaxDisplayLen = 20;
  internal CodeBlock(string body, int depth,string language) : base([], body, depth)
  {
    Language = language;
  }

  public override string ToString()
  {
    string indentation = string.Concat(Enumerable.Repeat(" ",Depth*Token.IndentLength));

    string output = $"{indentation}{GetName()}: {string.Join("",Body.AsSpan().Slice(0,Math.Min(MaxDisplayLen,Body.Length)).ToString().Split("\n"))} {(Body.Length > MaxDisplayLen ? "..." : "")}";

    return output;
  }
}
