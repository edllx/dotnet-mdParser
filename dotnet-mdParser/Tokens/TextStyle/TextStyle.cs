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
