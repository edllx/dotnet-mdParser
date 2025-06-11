
namespace edllx.dotnet.mdParser;

public class Heading : Token
{
  public int Size {get;init;}
  protected Heading(List<Token> childrens, string body, int depth,int size) : base(childrens, body, depth)
  {
    Size = size;
  }

  public override string GetName()
  {
    return $"H{Size}";
  }

  public static Heading H1(List<Token> childrens, string body)
  {
    return new(childrens,body,1,1);
  }


  public static Heading H2(List<Token> childrens, string body)
  {
    return new(childrens,body,1,2);
  }

  public static Heading H3(List<Token> childrens, string body)
  {
    return new(childrens,body,1,3);
  }

  public static Heading H4(List<Token> childrens, string body)
  {
    return new(childrens,body,1,4);
  }

  public static Heading H5(List<Token> childrens, string body)
  {
    return new(childrens,body,1,5);
  }

  public static Heading H6(List<Token> childrens, string body)
  {
    return new(childrens,body,1,6);
  }


  // Body only 
  public static Heading H1(string body)
  {
    return new([],body,1,1);
  }

  public static Heading H2(string body)
  {
    return new([],body,1,2);
  }

  public static Heading H3(string body)
  {
    return new([],body,1,3);
  }
  public static Heading H4(string body)
  {
    return new([],body,1,4);
  }

  public static Heading H5(string body)
  {
    return new([],body,1,5);
  }
  public static Heading H6(string body)
  {
    return new([],body,1,6);
  }

}
