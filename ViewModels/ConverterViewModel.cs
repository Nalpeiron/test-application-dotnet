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
using Zentitle.Licensing.Client.Api;
using ZentitleOnPremDemo.Models;
using ZentitleOnPremDemo.Zentitle;

namespace ZentitleOnPremDemo.ViewModels
{
    public class ConverterViewModel : ViewModelBase
    {
        public bool ProcessingEnabled
        {
            get => _processingEnabled;
            set => this.RaiseAndSetIfChanged(ref _processingEnabled, value);
        }

        public bool Online
        {
            get => _online;
            set => this.RaiseAndSetIfChanged(ref _online, value);
        }

        public bool Offline
        {
            get => _offline;
            set => this.RaiseAndSetIfChanged(ref _offline, value);
        }

        public bool Activated
        {
            get => _activated;
            set => this.RaiseAndSetIfChanged(ref _activated, value);
        }

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
            set
            {
                ProcessingEnabled = value > 0;
                this.RaiseAndSetIfChanged(ref _consumptionTokens, value);
            }
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
                if (value.Value > _threadsAvailable && Activated)
                {
                    async void Action()
                    {
                        await ShowThreadsInfo();
                        var maxThreadsAvailable = ThreadsList
                            .OrderBy(x => x.Value).Last(x => x.Value <= _threadsAvailable);
                        this.RaiseAndSetIfChanged(ref _threadSelection, maxThreadsAvailable);
                    }
                    Dispatcher.UIThread.Post(Action);
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
        private long _consumptionTokens;
        private long? _threadsAvailable = 0;
        private long _threadsNumber;
        private long _sampleNumber = 1;
        private int _quality = 5;
        private bool _activated;
        private bool _offline;
        private bool _online;
        private readonly Random _random = new();
        private ListItem<long> _threadSelection = null!;
        private bool _processingEnabled;

        public ConverterViewModel()
        {
            ShowDialog = new Interaction<MessageBoxViewModel, BoolResult>();
            SourcesSelectedIndex = -1;
            Sources = new ObservableCollection<FileItem>();

            ReloadLicenseData();

            FileExistsActionList.CurrentItem = FileExistsActionList.Single(x => x.Value == FileExistsAction.Overwrite);
            BitsPerSampleList.CurrentItem = BitsPerSampleList.Last();
            BitrateList.CurrentItem = BitrateList.First();
            SampleRateList.CurrentItem = SampleRateList.First();
            ThreadSelection = ThreadsList.First();

            StartEncoding = ReactiveCommand.CreateFromTask(async () =>
            {
                if (Sources.Count == 0 && ProcessingFiles.Count == 0) return;
                IsProcessing = true;
                IsStopped = false;
                try
                {
                    if (!ProcessingFiles.Any(x => x.ProgressPercent < 1))
                    {
                        if (_online)
                        {
                            await ReturnElementPool();
                        }

                        ProcessingFiles.Clear();

                        if (Sources.Count > 0)
                        {
                            if (_offline)
                            {
                                OfflineProcessing();
                            }
                            else
                            {
                                if (!await OnlineProcessing()) return;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    await HandleProcessingError(ex.Message);
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

            StopEncoding = ReactiveCommand.Create(() => { _cts?.Cancel(); });

            AddFile = ReactiveCommand.Create(() => { Sources.Add(new FileItem($"Sample{_sampleNumber++}.mp3")); });

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

                FormatsList.Single(x => x.Value == encodeFormat).Selected = true;
                this.RaisePropertyChanged(nameof(FormatsList));
            });
        }

        private void OfflineProcessing()
        {
            _threadsNumber = ThreadSelection.Value;
            AddFilesForProcessing();
        }

        private async Task<bool> OnlineProcessing()
        {
            var result = await Instance.CheckoutUsageCountFeature(Sources.Count);
            if (!result) return false;

            result = await CheckoutElementPool(Sources.Count);
            if (!result) return false;

            AddFilesForProcessing();
            UpdateFilesRemaining();

            return true;
        }

        private void AddFilesForProcessing()
        {
            ProcessingFiles.AddRange(Sources.Select(x => new ProcessingItem(x.Name)).ToList());
            ProcessingFiles.Take((int)_threadsNumber).ToList()
                .ForEach(x => x.Status = EncodeStatus.Processing);
            Sources.Clear();
        }

        private void LoadLicenseStatus()
        {
            Offline = Instance.Activation.Info.Mode == ActivationMode.Offline;
            Online = Instance.Activation.Info.Mode == ActivationMode.Online;
            Activated = Offline || Online;
        }

        public void ReloadLicenseData()
        {
            LoadLicenseStatus();
            LoadFormatList();
            if (_offline)
            {
                LoadOfflineParams();
            }
            else
            {
                UpdateFilesRemaining();
                UpdateThreads();
            }
        }

        private void LoadOfflineParams()
        {
            _threadsAvailable = 5;
            ProcessingEnabled = true;
        }

        private async Task ShowThreadsInfo()
        {
            if (_threadsAvailable.HasValue)
            {
                var thread = _threadsAvailable > 1 ? "threads" : "thread";
                var message =
                    $"Your license allows you to use up to {_threadsAvailable} {thread}. For maximum conversion speed upgrade your license at elevate.com";
                var store = new MessageBoxViewModel(message);
                await ShowDialog.Handle(store);
            }
        }

        private async Task Processing()
        {
            while (ProcessingFiles.Any(x => x.ProgressPercent < 1))
            {
                Task.Delay(_quality * 30).Wait();
                _cts!.Token.ThrowIfCancellationRequested();
                var items = ProcessingFiles.Where(x => x is { ProgressPercent: < 1, Status: EncodeStatus.Processing })
                    .OrderBy(x => x.ProgressPercent);

                foreach (var item in items)
                {
                    item.ProgressPercent += _random.Next(1, 40) * (decimal)0.001;

                    if (item.ProgressPercent < 1) continue;
                    item.Status = EncodeStatus.Completed;
                    var processingItem = ProcessingFiles
                        .FirstOrDefault(x => x.Status == EncodeStatus.Waiting);
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

            await StopProcessing();
        }


        private async Task HandleProcessingError(string? message)
        {
            if (message != null)
            {
                var store = new MessageBoxViewModel(message);
                await ShowDialog.Handle(store);
            }

            await StopProcessing();
        }

        private async Task StopProcessing()
        {
            await ReturnElementPool();
            IsProcessing = false;
            IsStopped = false;
        }

        private void UpdateFilesRemaining()
        {
            var ct = Instance.GetUsageCountFeature();
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
            if (ep.Active is > 0)
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
                _threadsNumber = number;
            }

            return result;
        }

        private void LoadFormatList()
        {
            var activeFeatures = Instance.Activation.Features
                .Where(x => x.Active >= 1).Select(x => x.Key).ToList();

            foreach (var format in FormatsList)
            {
                format.Enabled = _offline || HasFeature(format.Text);
                format.Selected = false;
            }

            var first = FormatsList.FirstOrDefault();
            if (first != null)
            {
                first.Selected = true;
            }

            this.RaisePropertyChanged(nameof(FormatsList));
            return;

            bool HasFeature(string? featureKey)
            {
                return !string.IsNullOrEmpty(featureKey) && activeFeatures.Any(x => x == featureKey);
            }
        }
    }
}