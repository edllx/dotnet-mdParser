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
          Token.H1([

            Token.Phrase("H1",2)
          ]),
          Token.H2([

            Token.Phrase("H2",2)
          ]),
          Token.H3([

            Token.Phrase("H3",2)
          ]),
          Token.H4([

            Token.Phrase("H4",2)
          ]),
          Token.H5([

            Token.Phrase("H5",2)
          ]),
          Token.H6([

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
    // Arrange 
    string text ="####### This is a heading 1"; 

    Root expected = Token.Root([
        Token.Phrase(text,1)
    ]);

    // Act 
    Root actual = Parser.Parse(text);

    // Assert
    Assert.Equal<Token>(expected,actual);
  }



  [Fact]
  [Description("Test nested bold text")]
  public void NestedBold()
  {
    // Arrange 
    string h3 ="H3 with nested "; 
    string bold ="bold text";
    string sample = $"### {h3}**{bold}**";

    Root expected = Token.Root([
        Token.H3([
          Token.Phrase(h3,2),
          Token.Bold([
            Token.Phrase(bold,3)
          ],2)
        ])
    ]);

    // Act 
    Root actual = Parser.Parse(sample);

    // Assert
    Assert.Equal<Token>(expected,actual);

  }

  [Fact]
  [Description("Test nested text style in header 1")]
  public void NestedInHeader()
  {
    // Arrange 
    string text = "## ~~**Bold text and _nested italic_ text in heading and strike**~~";

    Root expected = Token.Root([
        Token.H2([
          Token.Strikethrough([
            Token.Bold([
              Token.Phrase("Bold text and ",4),
              Token.Italic([
                Token.Phrase("nested italic",5),
              ],4),
              Token.Phrase(" text in heading and strike",4),
            ],3)
          ],2)
        ])
    ]);

    // Act 
    Root actual = Parser.Parse(text);

    // Assert
    Assert.Equal<Token>(expected,actual);
  }
}
