namespace edllx.dotnet.mdParser;

public class ListToken : Token
{
    // number of space carater before the LI marker 
    public int Offset { get; set; }
    public Type ParentType { get; set; } = typeof(UL);
    public ListToken(List<Token> childrens, string body, int depth) : base(childrens, body, depth)
    {
    }

    public override string ToString()
    {
        string indentation = string.Concat(Enumerable.Repeat(" ", Depth * Token.IndentLength));
        string output = $"{indentation}{GetName()}: Depth:{Depth} - Offset:{Offset} ";

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
        string output = $"{indentation}{GetName()}: {Depth} {(Done ? "done":"pending")} ";

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

internal class ListGroup
{
    public ListGroup? Parent {get;set;} 
    public ListToken Group {get;init;}

    public ListGroup(ListToken group)
    {
        Group = group;
    }
}

internal class ListBuilder
{
    private readonly List<Token> _result = [];
    private ListGroup? _structure;
    private List<Token> _children { get; set; } = [];
    private Token? _output { get; set; }

    public ListBuilder()
    {
    }

    public ListBuilder(List<Token> children, Token? output, ListGroup? structure)
    {
        _children = children;
        _output =output;
        _structure=structure;
    }

    public List<Token> Build()
    {
        return _result;
    }

    public ListBuilder From()
    {
        return new(_children,_output,_structure);
    }

    public void Push(Token token)
    {
        //Console.WriteLine($"[{token}]");
        if (token is not LI li)
        {
            AppendToken(token);
            return;
        }
        if(_structure is null)
        {
            AddNewRoot(li);
            return;
        }

        if (_structure.Group.Offset == li.Offset)
        {
            HandleSameOffset(li);
            return;
        }
        
        if (_structure.Group.Offset < li.Offset)
        {
            HandleNested(li);
            return;
        }
        
        HandleBacktrack(li);
    }

    private void AppendToken(Token token)
    {
        var lastLi = _children.Last();
        if (token is Phrase phrase)
        {
           lastLi.Children.Last().AddChild(phrase); 
            return;
        }
        lastLi.AddChild(token);
    }

    private void PushGroup(LI li,int depth = 1)
    {
        var prev = _structure;
        if(li.ParentType == typeof(OL))
        {
            var el = Token.OL([], prev?.Group.Depth + 1 ?? depth);
            el.Offset = li.Offset;
            _structure = new(el);
        }else
        {
            var el = Token.UL([], prev?.Group.Depth + 1 ?? depth);
            el.Offset = li.Offset;
            _structure = new(el);
        }

        _output = _structure.Group;
        _structure.Parent = prev;
        _children =_output.Children;
        
        if (prev is not null)
        {
            prev.Group.Children.Add(_output);
        }
        li.SetDepth(_structure?.Group.Depth + 1 ?? 1); 
        _children.Add(li);
    }
    
    private void PopGroup(bool force = false)
    {
        if(_structure is null ){return;}
        if(_structure.Parent is null && !force){return;}
        
        _structure = _structure.Parent;
        if (_structure is not null)
        {
            _output = _structure.Group;
            _children =_output.Children;
        }
        else
        {
            _output = null;
            _children = [];
        }
    }

    private void AddNewRoot(LI li)
    {
        
        PushGroup(li); 
       
        if (_output is null) {return;}
        _result.Add(_output);
    }

    private void HandleNested(LI li)
    {
        PushGroup(li);
    }
    
    private void HandleSameOffset(LI li)
    {
        if(_structure is null){return;}

        if (li.ParentType == _structure.Group.GetType())
        {
            li.SetDepth(_structure?.Group.Depth + 1 ?? 1); 
            _children.Add(li);
            return;
        }
        
        // Edge case list type change at the root. 
        if (_structure is not null && _structure.Parent is null)
        {
            PopGroup(true);
            AddNewRoot(li);
            return;
        }
        PopGroup();
        PushGroup(li);
    }
    
    private void HandleBacktrack(LI li)
    {
        while (_structure is not null && _structure.Group.Offset > li.Offset)
        {
            PopGroup(); 
        }
        Push(li);
    }
}