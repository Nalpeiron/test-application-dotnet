using Avalonia.Threading;
using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using ZentitleOnPremDemo.Models;
using ZentitleOnPremDemo.Zentitle;

namespace ZentitleOnPremDemo.ViewModels
{
    public class ConverterViewModel : ViewModelBase
    {
        public bool IsProcessing
        {
            get => _isProcessing;
            set => this.RaiseAndSetIfChanged(ref _isProcessing, value);
        }

        public bool IsStopped
        {
            get => _isStopped;
            set => this.RaiseAndSetIfChanged(ref _isStopped, value);
        }

        public long ConsumptionTokens
        {
            get => _consumptionTokens;
            set => this.RaiseAndSetIfChanged(ref _consumptionTokens, value);
        }

        public int Quality
        {
            get => _quality;
            set => this.RaiseAndSetIfChanged(ref _quality, value);
        }

        public ListItem<long> ThreadSelection
        {
            get => _threadSelection;
            set
            {
                if (value.Value > _threadsAvailable)
                {
                    Dispatcher.UIThread.Post(async () =>
                    {
                        await ShowThreadsInfo(value.Value);
                        var maxThreadsAvailable = ThreadsList.OrderBy(x => x.Value).Where(x => x.Value <= _threadsAvailable).Last();
                        this.RaiseAndSetIfChanged(ref _threadSelection, maxThreadsAvailable);
                    });
                }

                this.RaiseAndSetIfChanged(ref _threadSelection, value);
            }
        }

        public ICommand AddFile { get; }
        public ICommand RemoveFile { get; }
        public ICommand StartEncoding { get; }
        public ICommand StopEncoding { get; }
        public ReactiveCommand<EncodeFormat, Unit> SelectFormat { get; }

        public int SourcesSelectedIndex { get; set; }
        public string Destination { get; set; } = "C:/";
        public ObservableCollection<FileItem> Sources { get; }
        public ObservableCollection<ProcessingItem> ProcessingFiles { get; } = new();
        public Interaction<MessageBoxViewModel, BoolResult> ShowDialog { get; }

        public List<ListItem<long>> ThreadsList { get; } = new()
        {
            new ListItem<long>(1, "1 thread"),
            new ListItem<long>(2, "2 threads"),
            new ListItem<long>(3, "3 threads"),
            new ListItem<long>(4, "4 threads"),
            new ListItem<long>(5, "5 threads"),
        };

        public ListItemCollectionView<int> SampleRateList { get; } = new()
        {
             new ListItem<int>(0, "Source"),
             new ListItem<int>(8000, "8000 Hz"),
             new ListItem<int>(11025, "11,025 Hz"),
             new ListItem<int>(16000, "16,000 Hz"),
             new ListItem<int>(22050, "22,050 Hz"),
             new ListItem<int>(44100, "44,100 Hz"),
             new ListItem<int>(48000, "48,000 Hz"),
             new ListItem<int>(88200, "88,200 Hz"),
             new ListItem<int>(96000, "96,000 Hz"),
             new ListItem<int>(176400, "176,400 Hz"),
             new ListItem<int>(192000, "192,000 Hz"),
             new ListItem<int>(352800, "352,800 Hz"),
             new ListItem<int>(384000, "384,000 Hz")
        };

        public ListItemCollectionView<int> BitrateList { get; } = new()
        {
            new ListItem<int>(0, "Source"),
            new ListItem<int>(96, "96 kbps"),
            new ListItem<int>(128, "128 kbps"),
            new ListItem<int>(192, "192 kbps"),
            new ListItem<int>(256, "256 kbps"),
            new ListItem<int>(320, "320 kbps")
        };

        public ListItemCollectionView<int> BitsPerSampleList { get; } = new()
        {
            new ListItem<int>(8, "8-bits"),
            new ListItem<int>(16, "16-bits"),
            new ListItem<int>(24, "24-bits"),
            new ListItem<int>(32, "32-bits")
        };

        public ListItemCollectionView<FileExistsAction> FileExistsActionList { get; } = new()
        {
            new ListItem<FileExistsAction>(FileExistsAction.Ask, "Ask"),
            new ListItem<FileExistsAction>(FileExistsAction.Skip, "Skip"),
            new ListItem<FileExistsAction>(FileExistsAction.Overwrite, "Overwrite"),
            new ListItem<FileExistsAction>(FileExistsAction.Rename, "Rename"),
            new ListItem<FileExistsAction>(FileExistsAction.Cancel, "Cancel")
        };

        public ObservableCollection<FormatItemViewModel> FormatsList { get; } = new()
        {
            new FormatItemViewModel(EncodeFormat.Mp3, "MP3"),
            new FormatItemViewModel(EncodeFormat.Aac, "AAC"),
            new FormatItemViewModel(EncodeFormat.Wav, "WAV"),
            new FormatItemViewModel(EncodeFormat.Flac, "FLAC"),
            new FormatItemViewModel(EncodeFormat.Ogg, "OGG"),
            new FormatItemViewModel(EncodeFormat.Opus, "OPUS")
        };

        private CancellationTokenSource? _cts;
        private bool _isProcessing;
        private bool _isStopped;
        private long _consumptionTokens = 0;
        private long? _threadsAvailable = 0;
        private long _threadsNumber = 0;
        private long _sampleNumber = 1;
        private int _quality = 5;
        private readonly Random _random = new();
        private ListItem<long> _threadSelection;


        public ConverterViewModel()
        {
            ShowDialog = new Interaction<MessageBoxViewModel, BoolResult>();
            SourcesSelectedIndex = -1;
            Sources = new ObservableCollection<FileItem>();
            ReloadLicenseData();

            FileExistsActionList.CurrentItem = FileExistsActionList.Where(x => x.Value == FileExistsAction.Overwrite).Single();
            BitsPerSampleList.CurrentItem = BitsPerSampleList.Last();
            BitrateList.CurrentItem = BitrateList.First();
            SampleRateList.CurrentItem = SampleRateList.First();
            ThreadSelection = ThreadsList.First();

            StartEncoding = ReactiveCommand.CreateFromTask(async () =>
            {
                if (Sources.Count == 0 && ProcessingFiles.Count == 0) return;
                IsProcessing = true;
                IsStopped = false;

                if (!ProcessingFiles.Where(x => x.ProgressPercent < 1).Any())
                {
                    await ReturnElementPool();

                    ProcessingFiles.Clear();

                    if (Sources.Count > 0)
                    {
                        var result = await Instance.CheckoutConsumptionToken(Sources.Count());
                        if (!result) return;

                        result = await CheckoutElementPool(Sources.Count);
                        if (!result) return;

                        ProcessingFiles.AddRange(Sources.Select(x => new ProcessingItem(x.Name)).ToList());
                        ProcessingFiles.Take((int)_threadsNumber).ToList().ForEach(x => x.Status = EncodeStatus.Processing);
                        Sources.Clear();
                        UpdateFilesRemaining();
                    }
                }

                try
                {
                    _cts = new CancellationTokenSource();
                    await Task.Run(Processing, cancellationToken: _cts.Token);
                }
                catch (OperationCanceledException)
                {
                    IsStopped = true;
                }
                finally
                {
                    _cts!.Dispose();
                }
            });

            StopEncoding = ReactiveCommand.Create(() =>
            {
                if (_cts != null)
                {
                    _cts.Cancel();
                }
            });

            AddFile = ReactiveCommand.Create(() =>
            {
                Sources.Add(new FileItem($"Sample{_sampleNumber++}.mp3"));
            });

            RemoveFile = ReactiveCommand.Create(() =>
            {
                if (SourcesSelectedIndex >= 0)
                {
                    Sources.RemoveAt(SourcesSelectedIndex);
                }
            });

            SelectFormat = ReactiveCommand.Create<EncodeFormat>((encodeFormat) =>
            {
                var selected = FormatsList.Where(x => x.Selected = true).ToList();
                foreach (var format in selected)
                {
                    format.Selected = false;
                }
                FormatsList.Where(x => x.Value == encodeFormat).Single().Selected = true;
                this.RaisePropertyChanged(nameof(FormatsList));
            });
        }

        public void ReloadLicenseData()
        {
            LoadFormatList();
            UpdateFilesRemaining();
            UpdateThreads();
        }

        public async Task ShowThreadsInfo(long value)
        {
            if (_threadsAvailable.HasValue)
            {
                var thread = _threadsAvailable > 1 ? "threads" : "thread";
                var message = $"Your license allowes you to use up to {_threadsAvailable} {thread}. For maximum conversion speed upgrade your license at elevate.com";
                var store = new MessageBoxViewModel(message);
                await ShowDialog.Handle(store);
            }
        }

        public async Task Processing()
        {
            while (ProcessingFiles.Where(x => x.ProgressPercent < 1).Any())
            {
                Task.Delay(_quality * 30).Wait();
                _cts!.Token.ThrowIfCancellationRequested();
                var items = ProcessingFiles.Where(x => x.ProgressPercent < 1 && x.Status == EncodeStatus.Processing).OrderBy(x => x.ProgressPercent);

                foreach (var item in items)
                {
                    item.ProgressPercent += _random.Next(1, 40) * (decimal)0.001;

                    if (item.ProgressPercent >= 1)
                    {
                        item.Status = EncodeStatus.Completed;
                        var processingItem = ProcessingFiles.Where(x => x.Status == EncodeStatus.Waiting).FirstOrDefault();
                        if (processingItem != null)
                        {
                            processingItem.Status = EncodeStatus.Processing;
                        }
                        else
                        {
                            await Instance.ReturnElementPoolFeature(1);
                        }
                    }
                }
            }
            await ReturnElementPool();
            IsProcessing = false;
            IsStopped = false;
        }

        private void UpdateFilesRemaining()
        {
            var ct = Instance.GetConsumptionToken();
            ConsumptionTokens = ct.Available ?? 0;
        }

        private void UpdateThreads()
        {
            var ep = Instance.GetElementPoolFeature();
            _threadsAvailable = ep.Total;
        }

        private static async Task ReturnElementPool()
        {
            var ep = Instance.GetElementPoolFeature();
            if (ep.Active.HasValue && ep.Active > 0)
            {
                await Instance.ReturnElementPoolFeature(ep.Active.Value);
            }
        }

        private async Task<bool> CheckoutElementPool(long maxNeeded)
        {
            var ep = Instance.GetElementPoolFeature();
            var number = Math.Min(Math.Min(ep.Available ?? 0, maxNeeded), ThreadSelection.Value);
            var result = await Instance.CheckoutElementPoolFeature(number);
            if (result)
            {
                ep = Instance.GetElementPoolFeature();
                _threadsNumber = number;
            }
            return result;
        }

        public void LoadFormatList()
        {
            var activeFeatures = Instance.Activation.Features
                .Where(x => x.Active >= 1).Select(x => x.Key);

            foreach (var format in FormatsList)
            {
                format.Enabled = HasFeature(format.Text);
                format.Selected = false;
            }

            var first = FormatsList.FirstOrDefault();
            if (first != null)
            {
                first.Selected = true;
            }
            this.RaisePropertyChanged(nameof(FormatsList));
            bool HasFeature(string featureKey)
            {
                return activeFeatures.Any(x => x == featureKey);
            }
        }
    }
}
