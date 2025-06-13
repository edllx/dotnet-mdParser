namespace edllx.dotnet.mdParser;

//TODO
/** 
  BOLD,
  ITALIC,
  STRIKETHROUGH,
  HIGHLIGHT,
  NEW_LINE,
  SPACE,
  TILDE,
  WORD,
  ROOT,
  PARAGRAPH,
  ESCAPE,
  EXTERNAL_LINK,
  UL,
  LI,
  OL,
  INPUT,
  CHECK_BOX,
  CHECK_BOX_UL,
  INLINE_CODE,
  CODE_BLOCK,
*/


public partial class Token 
{
  public List<Token> Childrens {get;private set;} =[];
  public string Body {get; private set;} = "";
  public int Depth {get;init;}
  const int IndentLength = 2;

  public Token(List<Token> childrens, string body,int depth)
  {
    Childrens = childrens;
    Body = body;
    Depth = depth;
  }

  public override bool Equals(object? obj)
  {
    bool isNull =obj == null; 
    if(obj is null) {return false;}
    if (GetType() != obj.GetType()){return false;}

    Token other = (Token)obj;
    if(other.Childrens.Count != Childrens.Count){return false;}
    if(other.Body != Body){return false;}
    if(other.Depth != Depth){return false;}

    for(int i =0;i<Childrens.Count;i++)
    {
      if(!Childrens[i].Equals(other.Childrens[i])) {return false;}
    }
    return true;
  }

  public override int GetHashCode()
  {
    throw new NotImplementedException();
  }

  public virtual string GetName()
  {
    return GetType().Name.Split(".").Last(); 
  }

  public override string ToString()
  {
    string indentation = string.Concat(Enumerable.Repeat(" ",Depth*Token.IndentLength));
    string output = $"{indentation}{GetName()}: {Body}";

    foreach(Token t in Childrens)
    {
      output += $"\n{t.ToString()}";
    }

    return output;
  }

}

// Generators
public partial class Token
{

  //Root 
  public static Root Root(List<Token> childrens)
  {
    return new Root(childrens);
  }

  // Heading
  public static Heading H1(List<Token> childrens)
  {
    return new(childrens,"",1,1);
  }

  public static Heading H2(List<Token> childrens)
  {
    return new(childrens,"",1,2);
  }

  public static Heading H3(List<Token> childrens)
  {
    return new(childrens,"",1,3);
  }

  public static Heading H4(List<Token> childrens)
  {
    return new(childrens,"",1,4);
  }

  public static Heading H5(List<Token> childrens)
  {
    return new(childrens,"",1,5);
  }

  public static Heading H6(List<Token> childrens)
  {
    return new(childrens,"",1,6);
  }

  // Phrase
  public static Phrase Phrase(string body,int depth)
  {
    return new(body,depth);
  }

  // NewLine
  public static NewLine NewLine(int depth)
  {
    return new(depth);
  }

  // Bold
  public static Bold Bold(List<Token> children,int depth)
  {
    return new(children,depth);
  }
}

public class Root : Token
{
  internal Root(List<Token> childrens) : base(childrens, "", 0)
  {
  }
}

public class Phrase : Token
{
  internal Phrase(string body, int depth) : base([], body, depth)
  {
  }
}

public class NewLine : Token
{
  internal NewLine( int depth) : base([], "\n", depth)
  {
  }
}
