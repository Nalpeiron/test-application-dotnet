using ReactiveUI;

namespace ZentitleOnPremDemo.Models;

public class FileItem : ReactiveObject
{
    public FileItem(string name)
    {
        Name = name;
    }

    public string Name
    {
        get => _name;
        set
        {
            this.RaiseAndSetIfChanged(ref _name, value);
            this.RaisePropertyChanged(nameof(Text));
        }
    }
    private string _name = string.Empty;
    public string Text => _name;
    public virtual string ToolTip => Name;
}
