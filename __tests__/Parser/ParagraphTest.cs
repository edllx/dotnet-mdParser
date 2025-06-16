
using System.ComponentModel;
using edllx.dotnet.mdParser;

namespace __tests__;

public class ParagraphTest 
{
  [Fact]
  [Description("Multiple line paragrath")]
  public void BaseH1()
  {
    // Arrange 
    string text ="first line of p1.\nsecond line of p1\n\nfirst line of p2.\nsecond line of p2\n"; 

    Root expected = Token.Root([
        Token.Paragraph([
          Token.Phrase("first line of p1.second line of p1",2)
        ],1),
        Token.NewLine(1),
        Token.Paragraph([
          Token.Phrase("first line of p2.second line of p2",2)
        ],1),
    ]);

    // Act 
    Root actual = Parser.Parse(text);


    // Assert
    Assert.Equal<Token>(expected,actual);
  }
}
