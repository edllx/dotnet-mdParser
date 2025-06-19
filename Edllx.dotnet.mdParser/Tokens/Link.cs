namespace edllx.dotnet.mdParser;

public class Link : Token 
{
  public String Name {get;init;} = "";
  public String Source {get;init;} ="";

  internal Link(int depth) : base([], "", depth)
  {
  }

  public override string ToString()
  {
    string indentation = string.Concat(Enumerable.Repeat(" ",Depth*Token.IndentLength));
    string output = $"{indentation}{GetName()}: {Name}|{Source}";
    return output;

  }
  public override bool Equals(object? obj)
  {
    bool isNull =obj == null; 
    if(obj is null) {return false;}
    if (GetType() != obj.GetType()){return false;}

    Link other = (Link)obj;
    if(other.Children.Count != Children.Count){return false;}
    if(other.Body != Body){return false;}
    if(other.Depth != Depth){return false;}
    if(other.Name != Name){return false;}
    if(other.Source != Source){return false;}
    
    return true;
  }

  public override int GetHashCode()
  {
    throw new NotImplementedException();
  }
}

public class ImageLink : Link 
{
  internal ImageLink(int depth) : base( depth)
  {
  }
}
