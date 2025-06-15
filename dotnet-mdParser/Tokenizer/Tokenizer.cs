namespace edllx.dotnet.mdParser;

public class TokenizerOptions
{
  public int Depth {get;set;} = 1;
  public Pattern[] Patterns = Pattern.GeneralPatterns; 

}

public class Tokenizer 
{
  private Root _output {get;init;}
  private string _source {get;init;}
  private TokenizerOptions _options {get;init;}
  private int _offset = 0;
 

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

      for(int i =0;i<_options.Patterns.Length ;i++){
        Pattern p  = _options.Patterns[i];

        bool success = p.Generate(_output,source.Slice(_offset),_options.Depth); 
        if(!success){continue;}
        _offset += p.Len;
        matched = true;

        break;
      }

      if(!matched){
        _offset++;

      }
    }

    return _output;
  }
}
