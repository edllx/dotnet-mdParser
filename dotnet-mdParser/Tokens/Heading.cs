
namespace edllx.dotnet.mdParser;

public class Heading : Token
{
  public int Size {get;init;}
  internal Heading(List<Token> childrens, string body, int depth,int size) : base(childrens, body, depth)
  {
    Size = size;
  }

  public override string GetName()
  {
    return $"H{Size}";
  }

  public override bool Equals(object? obj)
  {
    if (! base.Equals(obj)){return false;}

    Heading other = (Heading)obj;
    return Size == other.Size ;
  }

  public override int GetHashCode()
  {
    throw new NotImplementedException();
  }
}
