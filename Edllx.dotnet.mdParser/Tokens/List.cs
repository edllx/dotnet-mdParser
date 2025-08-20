namespace edllx.dotnet.mdParser;

public class ListToken : Token
{
  public int Offset { get; set; }
  public Type ParentType { get; set; } = typeof(UL);
  public ListToken(List<Token> childrens, string body, int depth) : base(childrens, body, depth)
  {
  }

  public override string ToString()
  {
    string indentation = string.Concat(Enumerable.Repeat(" ", Depth * Token.IndentLength));
    string output = $"{indentation}{GetName()}: {Body} : {Depth} ";

    foreach (Token t in Children)
    {
      output += $"\n{t.ToString()}";
    }

    return output;
  }
}

public class UL : ListToken
{
  internal UL(List<Token> children, int depth) : base(children, "", depth)
  {
  }
}

public class OL : ListToken
{
  internal OL(List<Token> children, int depth) : base(children, "", depth)
  {
  }
}

public class LI : ListToken
{
  internal LI(List<Token> children, int depth) : base(children, "", depth)
  {
  }
}

public class CheckBox : LI
{
  public bool Done {get;set;}
    internal CheckBox(List<Token> children, int depth,bool done) : base(children, depth)
    {
      Done = done;
    }

    public override string ToString()
    {
      string indentation = string.Concat(Enumerable.Repeat(" ", Depth * Token.IndentLength));
      string output = $"{indentation}{GetName()}: {Body} : {Depth} {(Done ? "done":"pending")} ";

      foreach (Token t in Children)
      {
        output += $"\n{t.ToString()}";
      }

      return output;

    }

    public override bool Equals(object? obj)
    {
      if(obj is null) {return false;}
      if(!base.Equals(obj)) {return false;}
      if (!(obj is CheckBox)){return false;}

      CheckBox other = (CheckBox)obj;
      if(other.Done != Done){return false;}
     
      return true;
    }

    public override int GetHashCode()
    {
      throw new NotImplementedException();
    }
}

[Flags]
enum ListBuilderFlags
{
  None = 0,
  IsRoot = 1,
  LastGroupOffsetEQ = 1 << 1,
  LastGroupOffsetGT = 1 << 2,
  LastGroupOffsetST = 1 << 3,
  LastUL = 1 << 4,
  LastOL = 1 << 5,
  CurrentUL = 1 << 6,
  CurrentOL = 1 << 7,
  LastPushedOffsetEQ = 1 << 8,
  LastPushedOffsetGT = 1 << 9,
  LastPushedOffsetST = 1 << 10,
}

internal class ListBuilder
{
  private ListToken _token { get; init; }
  private List<ListToken> _tokens { get; } = [];
  private Token? _lastModified { get; set; }
  private LI? _lastModifiedLi { get; set; }
  private ListToken? _lastPushed { get; set; }

  public ListBuilder(ListToken token)
  {
    _token = token;
    _tokens.Add(_token);
  }

  public void Push(Token token)
  {

    try
    {
      LI li = (LI)token;

      ListToken? tokenGroup = FindGroup(li.Offset, token, li.ParentType);
      if (tokenGroup is null)
      {
        return;
      }

      li.SetDepth(tokenGroup.Depth + 1);

      _lastPushed = li;
      tokenGroup.Children.Add(li);
      _lastModified = tokenGroup;
      _lastModifiedLi = li;

    }
    catch (System.Exception)
    {
      if (_lastModifiedLi is not null)
      {
        if (token is Phrase)
        {
          _lastModifiedLi.Children[0].AddChild(token);
        }
        else
        {
          _lastModifiedLi.AddChild(token);
        }
      }
    }
  }

  private ListToken? FindGroup(int offset, Token token, Type type)
  {
    int state = 0;
    ListToken? Group = null;

    ListToken lastGroup = _tokens.Last();
    int dp = _lastPushed?.Offset ?? 0;

    state |= lastGroup.Offset == 0 || dp == 0 ? ((int)ListBuilderFlags.IsRoot) : 0;
    state |= offset == lastGroup.Offset ? ((int)ListBuilderFlags.LastGroupOffsetEQ) : 0;
    state |= offset > lastGroup.Offset ? ((int)ListBuilderFlags.LastGroupOffsetGT) : 0;
    state |= offset < lastGroup.Offset ? ((int)ListBuilderFlags.LastGroupOffsetST) : 0;

    state |= lastGroup.GetType().IsAssignableFrom(typeof(UL)) ? ((int)ListBuilderFlags.LastUL) : 0;
    state |= lastGroup.GetType().IsAssignableFrom(typeof(OL)) ? ((int)ListBuilderFlags.LastOL) : 0;

    state |= type.IsAssignableFrom(typeof(UL)) ? ((int)ListBuilderFlags.CurrentUL) : 0;
    state |= type.IsAssignableFrom(typeof(OL)) ? ((int)ListBuilderFlags.CurrentOL) : 0;

    state |= offset == dp && dp != 0 ? ((int)ListBuilderFlags.LastPushedOffsetEQ) : 0;
    state |= offset > dp && dp != 0 ? ((int)ListBuilderFlags.LastPushedOffsetGT) : 0;
    state |= offset < dp && dp != 0 ? ((int)ListBuilderFlags.LastPushedOffsetST) : 0;


    Console.WriteLine($"{state}: {token}");

    switch (state)
    {
      case 83:
      case 163:
      case 338:
      case 418:
      case 1106:
      case 1107:
      case 1109:
        Group = lastGroup;
        break;

      case 405:
      case 421:
      case 660:
        Group = Token.OL([], Math.Max(0, _tokens.Last().Depth + 1));
        Group.Offset = offset;
        lastGroup.Children.Add(Group);
        _tokens.Add(Group);
        break;

      case 85:
      case 99:
      case 101:
      case 596:
      case 1123:
        Group = Token.UL([], Math.Max(0, _tokens.Last().Depth + 1));
        Group.Offset = offset;
        lastGroup.Children.Add(Group);
        _tokens.Add(Group);
        break;

      case 400:
      case 1171:
        _tokens.RemoveAt(_tokens.Count - 1);
        Group = _tokens.Last();
        break;

      case 1112:
      case 1128:
      case 1176:
        if(lastGroup.Offset == offset){return lastGroup;}

        while (_tokens.Count > 0 && offset < lastGroup.Offset)
        {
          lastGroup = _tokens.Last();
          _tokens.RemoveAt(_tokens.Count - 1);
        }

        _tokens.Add(lastGroup);
        return FindGroup(offset, token, type);

      case 147:
      case 402:
        while (_tokens.Count > 0 && offset <= lastGroup.Offset)
        {
          lastGroup = _tokens.Last();
          _tokens.RemoveAt(_tokens.Count - 1);
        }

        _tokens.Add(lastGroup);
        return FindGroup(offset, token, type);

      default:
        break;
    }

    return Group;
  }
}
