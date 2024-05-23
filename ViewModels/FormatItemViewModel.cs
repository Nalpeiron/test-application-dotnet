using ReactiveUI;
using ZentitleOnPremDemo.Models;

namespace ZentitleOnPremDemo.ViewModels;

public class FormatItemViewModel : ViewModelBase
{
    public string? Text
    {
        get => _text;
        set => this.RaiseAndSetIfChanged(ref _text, value);
    }

    public bool Enabled
    {
        get => _enabled;
        set => this.RaiseAndSetIfChanged(ref _enabled, value);
    }

    public bool Selected
    {
        get => _selected;
        set => this.RaiseAndSetIfChanged(ref _selected, value);
    }

    public EncodeFormat Value
    {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value, value);
    }

    public FormatItemViewModel(EncodeFormat value, string text)
    {
        Text = text;
        Value = value;
    }

    private string? _text;
    private bool _enabled;
    private bool _selected;
    private EncodeFormat _value;
}

