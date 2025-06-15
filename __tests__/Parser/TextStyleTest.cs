
using System.ComponentModel;
using edllx.dotnet.mdParser;
namespace __tests__; 


public class TextStyleTest
{

  [Fact]
  [Description("Mixed decoration in phrase")]
  public void Phase()
  {
    // Arrange 
    string italic = "italic";
    string bold = "bold";
    string strikethrough = "strikethrough";
    string text =$"This is _{italic}_, this is **{bold}**, and this is ~~{strikethrough}~~"; 

    Root expected = Token.Root([
        Token.Phrase("This is ",1),
        Token.Italic([
          Token.Phrase(italic,2)
        ],1),
        Token.Phrase(", this is ",1),
        Token.Bold([
          Token.Phrase(bold,2)
        ],1),
        Token.Phrase(", and this is ",1),
        Token.Strikethrough([
          Token.Phrase(strikethrough,2)
        ],1),
    ]);

    // Act 

    Root actual = Parser.Parse(text);

    // Assert
    Assert.Equal<Token>(expected,actual);
  }

}
