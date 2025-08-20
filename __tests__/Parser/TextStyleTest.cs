
using System.ComponentModel;
using edllx.dotnet.mdParser;
namespace __tests__;

public partial class TestInput
{

  public const string Code1 = @"
print(arg){
  console.log(arg)
}
";

  public const string Code2 = $@"
let message = {"\"hello world\""};
alert(message);
";

  public const string BaseCodeBlock = $"``` js\n{TestInput.Code1}```";
  public const string BaseCodeBlock2 = $"``` \n{TestInput.Code2}```";
}

public class TextStyleTest
{

  [Fact]
  [Description("Mixed text style in phrase")]
  public void MixedTextStyle()
  {
    // Arrange 
    string italic = "italic";
    string bold = "bold";
    string strikethrough = "strikethrough";
    string text = $"This is _{italic}_, this is **{bold}**, and this is ~~{strikethrough}~~";

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
    Assert.Equal<Token>(expected, actual);
  }

  [Fact]
  [Description("Multiple bold element in a phrase")]
  public void MultipleBold()
  {
    // Arrange 
    string italic = "italic";
    string firstBold = "first bold";
    string secondBold = "second bold";
    string text = $"This is _{italic}_, this is **{firstBold}**, and this is **{secondBold}**";

    Root expected = Token.Root([
        Token.Phrase("This is ",1),
        Token.Italic([
          Token.Phrase(italic,2)
        ],1),
        Token.Phrase(", this is ",1),
        Token.Bold([
          Token.Phrase(firstBold,2)
        ],1),
        Token.Phrase(", and this is ",1),
        Token.Bold([
          Token.Phrase(secondBold,2)
        ],1),
    ]);

    // Act 

    Root actual = Parser.Parse(text);

    // Assert
    Assert.Equal<Token>(expected, actual);
  }

  [Fact]
  [Description("Nested italic in bold")]
  public void NestedItalicInBold()
  {
    // Arrange 
    string italic = "nested italic";
    string text = $"**Bold text and _{italic}_ text**";

    Root expected = Token.Root([
        Token.Bold([
          Token.Phrase("Bold text and ",2),
          Token.Italic([
            Token.Phrase(italic,3)
          ],2),
          Token.Phrase(" text",2)
        ],1),
    ]);

    // Act 

    Root actual = Parser.Parse(text);

    // Assert
    Assert.Equal<Token>(expected, actual);
  }


  [Fact]
  [Description("Nested bold in italic")]
  public void NestedBoldInItalic()
  {
    // Arrange 
    string bold = "nested bold";
    string text = $"_Italic text and **{bold}** text_";

    Root expected = Token.Root([
        Token.Italic([
          Token.Phrase("Italic text and ",2),
          Token.Bold([
            Token.Phrase(bold,3)
          ],2),
          Token.Phrase(" text",2)
        ],1),
    ]);

    // Act 

    Root actual = Parser.Parse(text);

    // Assert
    Assert.Equal<Token>(expected, actual);
  }

  [Fact]
  [Description("Test Base code block")]
  public void BaseCodeBlock()
  {
    // Arrange 
    string text = TestInput.BaseCodeBlock;

    Root expected = Token.Root([
        Token.CodeBlock(TestInput.Code1,1,"js")
    ]);

    // Act 

    Root actual = Parser.Parse(text);

    // Assert
    Assert.Equal<Token>(expected, actual);
  }

  [Fact]
  [Description("Test Base code block")]
  public void BaseCodeBlock2()
  {
    // Arrange 
    string text = TestInput.BaseCodeBlock2;

    Root expected = Token.Root([
        Token.CodeBlock(TestInput.Code2,1,"")
    ]);

    // Act 

    Root actual = Parser.Parse(text);

    // Assert
    Assert.Equal<Token>(expected, actual);
  }
}
