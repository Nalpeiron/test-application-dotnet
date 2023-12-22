using ReactiveUI;

namespace ZentitleOnPremDemo.Models;

public class ProcessingItem : FileItem
{
    public ProcessingItem(string name) : base(name)
    { }

    public EncodeStatus Status
    {
        get => _status;
        set
        {
            this.RaiseAndSetIfChanged(ref _status, value);
        }
    }

    public decimal ProgressPercent
    {
        get => _progressPercent;
        set
        {
            this.RaiseAndSetIfChanged(ref _progressPercent, value);
        }
    }

    private EncodeStatus _status = EncodeStatus.Waiting;
    private decimal _progressPercent = 0;
}
