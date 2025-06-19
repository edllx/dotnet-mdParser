namespace edllx.dotnet.mdParser;

public class Parser 
{
  public static Root Parse(string input)
  {
    Tokenizer tokenizer = new(input);
    return tokenizer.Generate();
  }
}
