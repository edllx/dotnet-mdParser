namespace edllx.dotnet.mdParser;

public class TokenizerOptions
{
  public int Depth {get;set;} = 1;
}

public class Tokenizer 
{
  private Root _output {get;init;}
  private string _source {get;init;}
  private TokenizerOptions _options {get;init;}
  private int _offset = 0;
  private Pattern[] patterns = [
    new HeadingPattern(), 
        new PhrasePattern(),
        new NewLinePattern()
  ];

  public Tokenizer(string source)
  {
    _options = new TokenizerOptions();
    _output = new([]);
    _source = source;
  }

  public Tokenizer(string source, TokenizerOptions options)
  {
    _options = options;
    _output = new([]);
    _source = source;
  }

  public Root Generate()
  {
    ReadOnlySpan<char> source = _source.AsSpan();

    while(_offset < _source.Length)
    {
      bool matched = false;

      for(int i =0;i<patterns.Length ;i++){
        Pattern p  = patterns[i];

        Token? token = p.Generate(source.Slice(_offset),_options.Depth); 
        if(token is null){continue;}
        _output.Childrens.Add(token);
        _offset += p.Len;
        matched = true;

        break;
      }

      if(!matched){_offset++;}
    }

    return _output;
  }
}
