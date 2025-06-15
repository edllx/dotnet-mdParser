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
  [Description("Simple bold phrase test")]
  public void Bold()
  {
    // Arrange 
    string text ="Simple bold phrase"; 
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

  [Fact]
  [Description("Simple Italic phrase test")]
  public void Italic()
  {
    // Arrange 
    string text ="Simple Italic phrase"; 
    string Italic = $"_{text}_";

    Root expected = Token.Root([
        Token.Italic([
          Token.Phrase(text,2)
        ],1)
    ]);

    // Act 

    Root actual = Parser.Parse(Italic);

    // Assert
    Assert.Equal<Token>(expected,actual);
  }


  [Fact]
  [Description("Simple striked phrase test")]
  public void Strikethrough()
  {
    // Arrange 
    string text ="Simple striked phrase"; 
    string Italic = $"~~{text}~~";

    Root expected = Token.Root([
        Token.Strikethrough([
          Token.Phrase(text,2)
        ],1)
    ]);

    // Act 

    Root actual = Parser.Parse(Italic);

    // Assert
    Assert.Equal<Token>(expected,actual);
  }

}
