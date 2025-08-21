namespace __tests__ ;

public partial class TestInput
{

  private Dictionary<string, string> input = new(){
    ["TaskListCheckbox1.md"] = "- [ ] element 1 **line**",
    ["TaskListCheckbox2.md"] = "- [ ] element 1 \n**line**",
    ["SimpleLink.md"] = "Simple Link [link](https://github.com)"
  };

  public TestInput()
  {
    LoadListInputs();
  }

  private void LoadListInputs()
  {
    var files = Directory.GetFiles("Inputs/Files");

    foreach (var filename in files)
    {
      try
      {
        var key = filename.Split('/').Last();
        var content = File.ReadAllText(filename);
        
        Input[key] = content; 
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
      }
    }
  }

  public Dictionary<string, string> Input { get => input;}
}
