using System.ComponentModel;
using edllx.dotnet.mdParser;

namespace __tests__;

public class HeadingTest 
{
  [Fact]
  [Description("Base case H1")]
  public void BaseH1()
  {
    // Arrange 
    string text ="This is a heading 1"; 
    string sample = $"# {text}";

    Root expected = Token.Root([
        Token.H1([
          Token.Phrase(text,2)
        ])
    ]);

    // Act 

    Root actual = Parser.Parse(sample);


    // Assert
    Assert.Equal<Token>(expected,actual);

  }

  [Fact]
  [Description("Base case H6")]
  public void BaseH6()
  {
    // Arrange 
    string text ="This is a heading 6"; 
    string sample = $"###### {text}";

    Root expected = Token.Root([
        Token.H6([
          Token.Phrase(text,2)
        ])
    ]);

    // Act 

    Root actual = Parser.Parse(sample);

    // Assert
    Assert.Equal<Token>(expected,actual);
  }

  [Fact]
  [Description("Test all size")]
  public void AllSizes()
  {
    // Arrange 
    string sample = $@"# H1
## H2
### H3
#### H4
##### H5
###### H6";

      Root expected = Token.Root([
          Heading.H1([

            Token.Phrase("H1",2)
          ]),
          Heading.H2([

            Token.Phrase("H2",2)
          ]),
          Heading.H3([

            Token.Phrase("H3",2)
          ]),
          Heading.H4([

            Token.Phrase("H4",2)
          ]),
          Heading.H5([

            Token.Phrase("H5",2)
          ]),
          Heading.H6([

              Token.Phrase("H6",2)
          ])
            ]);

    // Act 

    Root actual = Parser.Parse(sample);

    // Assert
    Assert.Equal<Token>(expected,actual);
  }

  [Fact]
  [Description("Testing that different header size with same text a not equal")]
  public void DifferentSize()
  {
    // Arrange 
    string text ="This is a heading 1"; 
    string h1 = $"# {text}";
    string h2 = $"## {text}";

    // Act 

    Root actualH1 = Parser.Parse(h1);
    Root actualH2 = Parser.Parse(h2);

    // Assert
    Assert.NotEqual<Token>(actualH1,actualH2);

  }

  [Fact]
  [Description("Test more than 6 #")]
  public void MoreThan6()
  {
    // TODO should not be a header (probably need to be a word);
    Assert.True(true);
  }



  [Fact]
  [Description("Test nested element")]
  public void NestedElement()
  {
    // TODO should handle nested element;
    Assert.True(true);
  }

}

