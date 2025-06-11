namespace edllx.dotnet.mdParser;

//TODO
/**
  H1,
  H2,
  H3,
  H4,
  H5,
  H6,
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


public class Token 
{
  public List<Token> Childrens {get;private set;} =[];
  public string Body {get; private set;} = "";
  public int Depth {get;init;}

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
    string indentation = string.Concat(Enumerable.Repeat(" ",this.Depth));
    string output = $"{indentation}{GetName()}: {this.Body}";

    foreach(Token t in Childrens)
    {
      output += $"\n{t.ToString()}";
    }

    return output;
  }
}

public class Root : Token
{
  public Root(List<Token> childrens) : base(childrens, "", 0)
  {
  }
}
