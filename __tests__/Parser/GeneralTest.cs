using System.ComponentModel;
using edllx.dotnet.mdParser;
namespace __tests__; 


public class GeneralTest
{

  [Fact]
  [Description("Simple word test")]
  public void BaseH6()
  {
    // Arrange 
    string text ="Simple word"; 

    Root expected = Token.Root([
        Token.Phrase(text,1)
    ]);

    // Act 

    Root actual = Parser.Parse(text);

    // Assert
    Assert.Equal<Token>(expected,actual);
  }

}
