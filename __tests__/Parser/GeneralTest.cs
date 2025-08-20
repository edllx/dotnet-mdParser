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

  [Fact]
  [Description("Simple highlighted phrase test")]
  public void Highlight()
  {
    // Arrange 
    string text ="Simple striked phrase"; 
    string highlight = $"=={text}==";

    Root expected = Token.Root([
        Token.Highlight([
          Token.Phrase(text,2)
        ],1)
    ]);

    // Act 
    Root actual = Parser.Parse(highlight);

    // Assert
    Assert.Equal<Token>(expected,actual);
  }

  [Fact]
  [Description("Simple Paragraph test")]
  public void Paragraph()
  {
    // Arrange 
    string phrase = "This is a paragraph."; 
    string text =$"{phrase}\n\n"; 

    Root expected = Token.Root([
        Token.Paragraph([
          Token.Phrase(phrase,2)
        ],1),
        Token.NewLine(1),
    ]);

    // Act 
    Root actual = Parser.Parse(text);

    // Assert
    Assert.Equal<Token>(expected,actual);
  }

  [Fact]
  [Description("Special characters test")]
  public void SpacialCharacter()
  {
    // Arrange 
    string text = "&é\"'()*\\/_{}[]§è!çà-^¨$ù%´`µ£=+~"; 

    Root expected = Token.Root([
        Token.Phrase(text,1),
    ]);

    // Act 
    Root actual = Parser.Parse(text);

    // Assert
    Assert.Equal<Token>(expected,actual);
  }

  [Fact]
  [Description("Escape test")]
  public void EscapeBold()
  {
    // Arrange 
    string text = @"\*\*This line will not be bold\*\*"; 

    Root expected = Token.Root([
        Token.Phrase(text,1),
    ]);

    // Act 
    Root actual = Parser.Parse(text);

    // Assert
    Assert.Equal<Token>(expected,actual);
  }


  [Fact]
  [Description("InlineCode")]
  public void InlineCode()
  {
    // Arrange 
    string code = "inline code";
    string text =$"`{code}`"; 

    Root expected = Token.Root([
        Token.InlineCode([
          Token.Phrase(code,2),
        ],1), 
    ]);

    // Act 
    Root actual = Parser.Parse(text);

    // Assert
    Assert.Equal<Token>(expected,actual);
  }
}
