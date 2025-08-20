using System.ComponentModel;
using edllx.dotnet.mdParser;

namespace __tests__;

public class ListTest : IClassFixture<TestInput>
{
  private readonly TestInput _testInput;

  public ListTest(TestInput testInput)
  {
    _testInput = testInput;
  }

  private string Get(string key)
  {
    if(!_testInput.Input.TryGetValue(key, out string? text) || string.IsNullOrEmpty(text))
    {
      throw new Exception($"Missing Input: [{text}]");
    }

    return text;
  }

  [Fact]
  [Description("Simple UL")]
  public void BaseUL()
  {
    return;
    // Arrange 
    string text = Get("BaseULList.md");

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
    return;
    // Arrange 
    string text = Get("BaseOLList.md");

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
  [Description("Test UL with separate paragraph")]
  public void ULWithSeparateParagraph()
  {
    return;
    // Arrange 
    string text = Get("ULSeparateParagraph.md");

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
    return;
    // Arrange 
    string text = Get("ULWithParagraphe.md");

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
    return;
    // Arrange 
    string text = Get("MultipleListOneLine.md");

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
    return;
    // Arrange 
    string text = Get("MixedOLUL.md");

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
    return;
    // Arrange 
    string text = Get("AlternatingList1.md");

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
    return;
    // Arrange 
    string text = Get("AlternatingList2.md");

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
    return;
    // Arrange 
    string text = Get("AlternatingList3.md");

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
    return;
    // Arrange 
    string text = Get("TaskList1.md");

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
    return;
    // Arrange 
    string text = Get("TaskListInOl.md");

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
    return;
    // Arrange 
    string text = Get("TaskListInOl2.md");

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
  }

  [Fact]
  [Description]
  public void TaskInULWithNested()
  {
    return;

    string text = Get("TaskListCheckbox1.md");
    Root expected = Token.Root([
        Token.UL([
          Token.CheckBox([
            Token.Phrase("element 1 ",3),
            Token.Bold([
              Token.Phrase("line",4)
            ],3)
          ],2,false),
        ],1),
    ]);

    // Act 
    Root actual = Parser.Parse(text);

    // Assert
    Assert.Equal<Token>(expected, actual);

  }

  [Fact]
  [Description]
  public void TaskInULWithNestedMulti()
  {
    return;

    string text = Get("TaskListCheckbox2.md");

    Root expected = Token.Root([
        Token.UL([
          Token.CheckBox([
            Token.Phrase("element 1 ",3),
            Token.Bold([
              Token.Phrase("line",4)
            ],3)
          ],2,false),
        ],1),
    ]);


    // Act 
    Root actual = Parser.Parse(text);

    // Assert
    Assert.Equal<Token>(expected, actual);

  }

  [Fact]
  [Description]
  public void ListIndent1()
  {
    return;
    string text = Get("ListIndent1.md");

    Root expected = Token.Root([
        Token.UL([
          Token.LI([Token.Phrase("ligne 1",3)],2),
          Token.UL([
            Token.LI([Token.Phrase("ligne 2",4)],3),
            Token.UL([
              Token.LI([Token.Phrase("ligne 3",5)],4),
            ],3),
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
  [Description]
  public void ListIndent2()
  {
    string text = Get("ListIndent2.md");

    Root expected = Token.Root([
        Token.UL([
          Token.LI([Token.Phrase("element 1",3)],2),
          Token.LI([Token.Phrase("element 1",3)],2),
          Token.UL([
            Token.LI([Token.Phrase("nested 1",4)],3),
            Token.OL([
              Token.LI([Token.Phrase("nested OL",5)],4),
            ],3)
          ],2),
          Token.LI([Token.Phrase("element 3",3)],2),
          Token.UL([
            Token.LI([Token.Phrase("nested 2",4)],3),
          ],2)
        ],1),
        Token.OL([
          Token.LI([Token.Phrase("element 4",3)],2),
        ],1)
    ]);

    // Act 
    Root actual = Parser.Parse(text);

    // Assert
    Assert.Equal<Token>(expected, actual);
  }
}
