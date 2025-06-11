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
     
      Root expected = new([
          Heading.H1(text)
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

      Root expected = new([
          Heading.H6(text)
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

        Root expected = new([
            Heading.H1("H1"),
            Heading.H2("H2"),
            Heading.H3("H3"),
            Heading.H4("H4"),
            Heading.H5("H5"),
            Heading.H6("H6")
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
}

