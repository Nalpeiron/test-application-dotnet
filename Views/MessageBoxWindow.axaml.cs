using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System;
using System.Threading.Tasks;
using ZentitleOnPremDemo.ViewModels;

namespace ZentitleOnPremDemo.Views
{
    public partial class MessageBoxWindow : ReactiveWindow<MessageBoxViewModel>
    {
        public MessageBoxWindow()
        {
            InitializeComponent();
            this.WhenActivated(d => d(ViewModel!.Cancel.Subscribe(Close)));
            this.WhenActivated(d => d(ViewModel!.Ok.Subscribe(Close)));
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
