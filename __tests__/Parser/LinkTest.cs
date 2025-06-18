
using System.ComponentModel;
using edllx.dotnet.mdParser;

namespace __tests__;


public class LinkTest 
{
  [Fact]
  [Description("Simple Link")]
  public void BaseLink()
  {
    // Arrange 
    string name = "Crafty";
    string link ="https://github.com/liolle/Crafty"; 
    string text =$"[{name}]({link})"; 

    Root expected = Token.Root([
        Token.Link(name,link,1)
    ]);

    // Act 
    Root actual = Parser.Parse(text);


    // Assert
    Assert.Equal<Token>(expected,actual);
  }

  [Fact]
  [Description("Test link with no name")]
  public void NoNameLink()
  {
    // Arrange 
    string name = "";
    string link ="https://github.com/liolle/Crafty"; 
    string text =$"[]({link})"; 

    Root expected = Token.Root([
        Token.Link(name,link,1)
    ]);

    // Act 
    Root actual = Parser.Parse(text);


    // Assert
    Assert.Equal<Token>(expected,actual);
  }

  [Fact]
  [Description("Test link with no source")]
  public void NoSourceLink()
  {
    // Arrange 
    string name = "Crafty";
    string link =""; 
    string text =$"[{name}]({link})"; 

    Root expected = Token.Root([
        Token.Link(name,link,1)
    ]);

    // Act 
    Root actual = Parser.Parse(text);

    // Assert
    Assert.Equal<Token>(expected,actual);
  }


  [Fact]
  [Description("Test link with no name and source")]
  public void NoNameAndSourceLink()
  {
    // Arrange 
    string name = "Crafty";
    string link =""; 
    string text =$"[{name}]({link})"; 

    Root expected = Token.Root([
        Token.Link(name,link,1)
    ]);

    // Act 
    Root actual = Parser.Parse(text);

    // Assert
    Assert.Equal<Token>(expected,actual);
  }


  [Fact]
  [Description("Test multiple links in one line")]
  public void MultipleLinks()
  {
    // Arrange 
    string name1 = "Crafty";
    string name2 = "Deckster";
    string link1 ="https://github.com/liolle/Crafty"; 
    string link2 ="https://github.com/liolle/Deckster"; 
    string text =$"[{name1}]({link1})[{name2}]({link2})"; 

    Root expected = Token.Root([
        Token.Link(name1,link1,1),
        Token.Link(name2,link2,1)
    ]);

    // Act 
    Root actual = Parser.Parse(text);

    // Assert
    Assert.Equal<Token>(expected,actual);
  }

  [Fact]
  [Description("Test multiple links in one line separated by a phrase")]
  public void MultipleLinksSeparated()
  {
    // Arrange 
    string name1 = "Crafty";
    string name2 = "Deckster";
    string link1 ="https://github.com/liolle/Crafty"; 
    string link2 ="https://github.com/liolle/Deckster"; 
    string separation = "GAP";
    string text =$"[{name1}]({link1}){separation}[{name2}]({link2})"; 

    Root expected = Token.Root([
        Token.Link(name1,link1,1),
        Token.Phrase(separation,1),
        Token.Link(name2,link2,1)
    ]);

    // Act 
    Root actual = Parser.Parse(text);

    // Assert
    Assert.Equal<Token>(expected,actual);
  }

  [Fact]
  [Description("Simple ImageLink")]
  public void BaseImageLink()
  {
    // Arrange 
    string name = "Profile Picture";
    string link ="https://avatars.githubusercontent.com/u/44002141?v=4"; 
    string text =$"![{name}]({link})"; 

    Root expected = Token.Root([
        Token.ImageLink(name,link,1)
    ]);

    // Act 
    Root actual = Parser.Parse(text);

    // Assert
    Assert.Equal<Token>(expected,actual);
  }


  [Fact]
  [Description("Simple ImageLink")]
  public void LinkInList()
  {
    // Arrange 
    string name = "Markdown";
    string link ="https://daringfireball.net/projects/markdown/"; 
    string text =$"- Familiarity with [{name}]({link})"; 

    Root expected = Token.Root([
        Token.UL([
          Token.LI([
            Token.Phrase("Familiarity with ",3),
            Token.Link(name,link,3),
          ],2)

        ],1)
    ]);

    // Act 
    Root actual = Parser.Parse(text);

    // Assert
    Assert.Equal<Token>(expected,actual);
  }


}
