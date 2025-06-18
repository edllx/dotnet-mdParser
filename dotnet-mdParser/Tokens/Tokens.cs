namespace edllx.dotnet.mdParser;

//TODO
/** 
  UL,
  LI,
  OL,
  CHECK_BOX,
  CHECK_BOX_UL,
  CODE_BLOCK,
*/


public partial class Token 
{
  public List<Token> Children {get;private set;} =[];
  public string Body {get; internal set;} = "";
  public int Depth {get;private set;}
  public const int IndentLength = 2;

  public Token(List<Token> children, string body,int depth)
  {
    Children = children;
    Body = body;
    Depth = depth;
  }

  public void SetDepth(int depth)
  {
    Depth = depth;
    foreach(Token t in Children)
    {
      t.SetDepth(depth +1);
    }
  }

  public override bool Equals(object? obj)
  {
    bool isNull =obj == null; 
    if(obj is null) {return false;}
    if (GetType() != obj.GetType()){return false;}

    Token other = (Token)obj;
    if(other.Children.Count != Children.Count){
      return false;

    }
    if(other.Body != Body){
      return false;
    }
    if(other.Depth != Depth){
      return false;
    }

    for(int i =0;i<Children.Count;i++)
    {
      if(!Children[i].Equals(other.Children[i])) {return false;}
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

    foreach(Token t in Children)
    {
      output += $"\n{t.ToString()}";
    }

    return output;
  }
}

// Generators
public partial class Token
{

  public virtual void AddChild(Token t)
  {
    t.Depth = Depth;
    Children.Add(t);
  }

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

  public static Phrase Phrase(List<Token> childrens,int depth)
  {
    return new(childrens,depth);
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

  // Italic 
  public static Italic Italic(List<Token> children,int depth)
  {
    return new(children,depth);
  }

  // Strikethrough 
  public static Strikethrough Strikethrough(List<Token> children,int depth)
  {
    return new(children,depth);
  }

  public static Highlight Highlight(List<Token> children,int depth)
  {
    return new(children,depth);
  }

  public static Paragraph Paragraph(List<Token> children,int depth)
  {
    return new(children,depth);
  }

  public static InlineCode InlineCode(List<Token> children,int depth)
  {
    return new(children,depth);
  }

  public static Link Link(string name, string source, int depth)
  {
    return new(depth){
      Name = name,
           Source = source
    };
  }

  public static ImageLink ImageLink(string name, string source, int depth)
  {
    return new(depth){
      Name = name,
           Source = source
    };
  }

  public static UL UL(List<Token> children, int depth)
  {
    return new(children,depth);
  }


  public static OL OL(List<Token> children, int depth)
  {
    return new(children,depth);
  }


  public static LI LI(List<Token> children, int depth)
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
  // !! A Phrase with a body can't have children
  // A Phrase with children can't have a body
  internal Phrase(string body, int depth) : base([], body, depth)
  {
  }

  internal Phrase(List<Token> children, int depth) : base(children, "", depth)
  {
  }

  public override void AddChild(Token t)
  {
    Token last = this;
    if(Children.Count>0)
    {
      last = Children.Last();
    }


    if(t.GetType() == typeof(Phrase) && last.GetType() == typeof(Phrase) )
    {
      if(!string.IsNullOrEmpty(last.Body) && !string.IsNullOrEmpty(t.Body))
      {
        last.Body += t.Body;
        return;
      }

      foreach(Token token in t.Children)
      {
        AddChild(token);
      }

      return;
    }
    base.AddChild(t);
  }

}

public class NewLine : Token
{
  internal NewLine( int depth) : base([], "\n", depth)
  {
  }
}

public class Paragraph : Token
{
  internal Paragraph (List<Token> childrens, int depth) : base(childrens,"",depth)
  {
  }
}

