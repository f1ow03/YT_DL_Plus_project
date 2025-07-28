using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Diagnostics;

namespace youtube_downloader_plus.ViewModels
{
    public partial class AboutProgramViewModel : ViewModelBase
    {
        private readonly Action _close;

        public AboutProgramViewModel(Action close = null)
        {
            _close = close;
        }

        [RelayCommand]
        public async void CloseAboutWindow()
        {
            _close();
        }
    }
}
