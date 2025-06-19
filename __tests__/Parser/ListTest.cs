using System.ComponentModel;
using edllx.dotnet.mdParser;

namespace __tests__;


internal partial class TestInput
{
  public const string BaseULList = "- A\n  - Sub list of A\n   - Element of sub list of A\n- B\n  - Sub list of B\n- Simple LI";
  public const string BaseOLList = "1. A\n  - Sub list of A\n   - Element of sub list of A\n- B\n  - Sub list of B\n- Simple LI";
  public const string MixedOLUL = "1. first ol element.\n2. second ol element.\n  - nested ul.\n  3. first nested ol element.\n  1. second nested ol element.\n  2. other nested ol element.";
  public const string ULSeparatePhrase = "- This is a list item.\n- This list is unordered.\n\nThis should be a separate phrase";
  public const string ULSeparateParagraph = "- This is a list item.\n- This list is unordered.\n\nThis should be a separate paragraph\n";
  public const string ULWithParagraphe = "- This is a list item.\n- This list is unordered.\n This should be added to the phrase of the last LI";
  public const string MultipleListOneLine = "- First list.\n\n- Second list.";
  public const string AlternatingList1 = "1. ligne 1\n- ligne 2\n2. ligne 3\n- ligne 4";
  public const string AlternatingList2 = "1. ligne 1\n  - ligne 2\n    2. ligne 3\n- ligne 4";
  public const string AlternatingList3 = "1. ligne 1\n  - ligne 2\n  2. ligne 3\n- ligne 4";
  public const string TaskList1 = "- A\n  - [ ] Sub task of A\n    - [X] Checked task\n- B\n  - [ ] Sub task of B\n- [ ] Tasks C\n  - [ ] Sub task of C\n  - [] not a task\n- [x]not a task";
  public const string TaskListInOl = "1. A\n- [ ] Should be a task\n  - Sub list";
  public const string TaskListInOl2 = "1. A\n- [ ] Should be a task\n [ ] Should not be a task\n  - Sub list";
}

public class ListTest
{
  [Fact]
  [Description("Simple UL")]
  public void BaseUL()
  {
    // Arrange 
    string text = TestInput.BaseULList;

    Root expected = Token.Root([
        Token.UL([
          Token.LI([
            Token.Phrase("A",3)
          ],2),
          Token.UL([
            Token.LI([
              Token.Phrase("Sub list of A",4)
            ],3),
            Token.UL([
              Token.LI([
                Token.Phrase("Element of sub list of A",5)
              ],4),
            ],3)
          ],2),
          Token.LI([
            Token.Phrase("B",3)
          ],2),
          Token.UL([
            Token.LI([
              Token.Phrase("Sub list of B",4)
            ],3),
          ],2),
          Token.LI([
              Token.Phrase("Simple LI",3)
          ],2),
          ],1),
          ]);

          // Act 
          Root actual = Parser.Parse(text);


          // Assert
          Assert.Equal<Token>(expected, actual);
  }

  [Fact]
  [Description("Simple OL")]
  public void BaseOL()
  {
    // Arrange 
    string text = TestInput.BaseOLList;

    Root expected = Token.Root([
        Token.OL([
          Token.LI([
            Token.Phrase("A",3),
          ],2),
          Token.UL([
            Token.LI([
              Token.Phrase("Sub list of A",4)
            ],3),
            Token.UL([
              Token.LI([
                Token.Phrase("Element of sub list of A",5)
              ],4),
            ],3),
          ],2),
          Token.UL([
            Token.LI([
              Token.Phrase("B",4)
            ],3),
            Token.UL([
              Token.LI([
                Token.Phrase("Sub list of B",5)
              ],4),
            ],3),
          ],2),
          Token.UL([
              Token.LI([
                Token.Phrase("Simple LI",4)
              ],3),
          ],2),
          ],1),
          ]);

          // Act 
          Root actual = Parser.Parse(text);


          // Assert
          Assert.Equal<Token>(expected, actual);
  }



  [Fact]
  [Description("Test UL with separate phrase")]
  public void ULWithSeparatePhrase()
  {
    // Arrange 
    string text = TestInput.ULSeparatePhrase;

    Root expected = Token.Root([
        Token.UL([
          Token.LI([Token.Phrase("This is a list item.",3)],2),
          Token.LI([Token.Phrase("This list is unordered.",3)],2)
        ],1),
        Token.Phrase("This should be a separate phrase",1)
    ]);

    // Act 
    Root actual = Parser.Parse(text);


    // Assert
    Assert.Equal<Token>(expected, actual);
  }

  [Fact]
  [Description("Test UL with separate paragraph")]
  public void ULWithSeparateParagraph()
  {
    // Arrange 
    string text = TestInput.ULSeparateParagraph;

    Root expected = Token.Root([
        Token.UL([
          Token.LI([Token.Phrase("This is a list item.",3)],2),
          Token.LI([Token.Phrase("This list is unordered.",3)],2)
        ],1),
        Token.Paragraph([
          Token.Phrase("This should be a separate paragraph",2)
        ],1)
    ]);

    // Act 
    Root actual = Parser.Parse(text);


    // Assert
    Assert.Equal<Token>(expected, actual);
  }

  [Fact]
  [Description("Test UL with added phrase")]
  public void ULWithAddedPhrase()
  {
    // Arrange 
    string text = TestInput.ULWithParagraphe;

    Root expected = Token.Root([
        Token.UL([
          Token.LI([Token.Phrase("This is a list item.",3)],2),
          Token.LI([Token.Phrase("This list is unordered. This should be added to the phrase of the last LI",3)],2)
        ],1),
    ]);

    // Act 
    Root actual = Parser.Parse(text);


    // Assert
    Assert.Equal<Token>(expected, actual);
  }

  [Fact]
  [Description("Multiple simple item list")]
  public void MultipleList1()
  {
    // Arrange 
    string text = TestInput.MultipleListOneLine;

    Root expected = Token.Root([
        Token.UL([
          Token.LI([Token.Phrase("First list.",3)],2),
        ],1),
        Token.UL([
          Token.LI([Token.Phrase("Second list.",3)],2),
        ],1),
    ]);

    // Act 
    Root actual = Parser.Parse(text);


    // Assert
    Assert.Equal<Token>(expected, actual);
  }

  [Fact]
  [Description("Mixed Ol and UL ")]
  public void MixedOLUL()
  {
    // Arrange 
    string text = TestInput.MixedOLUL;

    Root expected = Token.Root([
        Token.OL([
          Token.LI([Token.Phrase("first ol element.",3)],2),
          Token.LI([Token.Phrase("second ol element.",3)],2),
          Token.UL([
            Token.LI([Token.Phrase("nested ul.",4)],3),
          ],2), 
          Token.OL([
            Token.LI([Token.Phrase("first nested ol element.",4)],3),
            Token.LI([Token.Phrase("second nested ol element.",4)],3),
            Token.LI([Token.Phrase("other nested ol element.",4)],3),
          ],2),
        ],1),
    ]);

    // Act 
    Root actual = Parser.Parse(text);


    // Assert
    Assert.Equal<Token>(expected, actual);
  }

  [Fact]
  [Description("Test alternating list ")]
  public void AlternatingList1()
  {
    // Arrange 
    string text = TestInput.AlternatingList1;

    Root expected = Token.Root([
        Token.OL([
          Token.LI([Token.Phrase("ligne 1",3)],2),
          Token.UL([
            Token.LI([Token.Phrase("ligne 2",4)],3),
          ],2), 
          Token.LI([Token.Phrase("ligne 3",3)],2),
          Token.UL([
            Token.LI([Token.Phrase("ligne 4",4)],3),
          ],2), 
        ],1),
    ]);

    // Act 
    Root actual = Parser.Parse(text);

    // Assert
    Assert.Equal<Token>(expected, actual);
  }

  [Fact]
  [Description("Test alternating list ")]
  public void AlternatingList2()
  {
    // Arrange 
    string text = TestInput.AlternatingList2;

    Root expected = Token.Root([
        Token.OL([
          Token.LI([Token.Phrase("ligne 1",3)],2),
          Token.UL([
            Token.LI([Token.Phrase("ligne 2",4)],3),
            Token.OL([
              Token.LI([Token.Phrase("ligne 3",5)],4),
            ],3)
          ],2), 
          Token.UL([
            Token.LI([Token.Phrase("ligne 4",4)],3),
          ],2)
        ],1),
    ]);

    // Act 
    Root actual = Parser.Parse(text);

    // Assert
    Assert.Equal<Token>(expected, actual);
  }

  [Fact]
  [Description("Test alternating list ")]
  public void AlternatingList3()
  {
    // Arrange 
    string text = TestInput.AlternatingList3;

    Root expected = Token.Root([
        Token.OL([
          Token.LI([Token.Phrase("ligne 1",3)],2),
          Token.UL([
            Token.LI([Token.Phrase("ligne 2",4)],3),
          ],2), 
          Token.OL([
            Token.LI([Token.Phrase("ligne 3",4)],3),
          ],2),
          Token.UL([
            Token.LI([Token.Phrase("ligne 4",4)],3),
          ],2), 
        ],1),
    ]);

    // Act 
    Root actual = Parser.Parse(text);

    // Assert
    Assert.Equal<Token>(expected, actual);
  }

  [Fact]
  [Description("Test task list")]
  public void BaseTaskList()
  {
    // Arrange 
    string text = TestInput.TaskList1;

    Root expected = Token.Root([
        Token.UL([
          Token.LI([Token.Phrase("A",3)],2),
          Token.UL([
            Token.CheckBox([Token.Phrase("Sub task of A",4)],3,false),
            Token.UL([
              Token.CheckBox([Token.Phrase("Checked task",5)],4,true),
            ],3)
          ],2),
          Token.LI([Token.Phrase("B",3)],2),
          Token.UL([
            Token.CheckBox([Token.Phrase("Sub task of B",4)],3,false),
          ],2),
          Token.CheckBox([Token.Phrase("Tasks C",3)],2,false),
          Token.UL([
            Token.CheckBox([Token.Phrase("Sub task of C",4)],3,false),
            Token.LI([Token.Phrase("[] not a task",4)],3),
          ],2),
          Token.LI([Token.Phrase("[x]not a task",3)],2),
        ],1),
    ]);

    // Act 
    Root actual = Parser.Parse(text);

    // Assert
    Assert.Equal<Token>(expected, actual);
  }

  [Fact]
  [Description("Test task list in OL")]
  public void TaskInOL()
  {
    // Arrange 
    string text = TestInput.TaskListInOl;

    Root expected = Token.Root([
        Token.OL([
          Token.LI([Token.Phrase("A",3)],2),
          Token.UL([
            Token.CheckBox([Token.Phrase("Should be a task",4)],3,false),
            Token.UL([
              Token.LI([Token.Phrase("Sub list",5)],4)
            ],3),
          ],2),
        ],1),
    ]);

    // Act 
    Root actual = Parser.Parse(text);

    // Assert
    Assert.Equal<Token>(expected, actual);
  }

  [Fact]
  [Description("Test task list in OL")]
  public void TaskInOL2()
  {
    // Arrange 
    string text = TestInput.TaskListInOl2;

    Root expected = Token.Root([
        Token.OL([
          Token.LI([Token.Phrase("A",3)],2),
          Token.UL([
            Token.CheckBox([Token.Phrase("Should be a task [ ] Should not be a task",4)],3,false),
            Token.UL([
              Token.LI([Token.Phrase("Sub list",5)],4)
            ],3),
          ],2),
        ],1),
    ]);

    // Act 
    Root actual = Parser.Parse(text);

    // Assert
    Assert.Equal<Token>(expected, actual);
    Assert.True(false);
  }

}
