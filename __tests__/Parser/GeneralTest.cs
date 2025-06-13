using System.ComponentModel;
using edllx.dotnet.mdParser;
namespace __tests__; 


public class GeneralTest
{

  [Fact]
  [Description("Simple phrase test")]
  public void Phase()
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

  [Fact]
  [Description("Simple bold word test")]
  public void Bold()
  {
    // Arrange 
    string text ="Simple bold word"; 
    string bold = $"**{text}**";

    Root expected = Token.Root([
        Token.Bold([
          Token.Phrase(text,2)
        ],1)
    ]);

    // Act 

    Root actual = Parser.Parse(bold);

    // Assert
    Assert.Equal<Token>(expected,actual);
  }
}
