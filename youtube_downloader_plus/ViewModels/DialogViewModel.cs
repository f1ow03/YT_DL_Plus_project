using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youtube_downloader_plus.ViewModels
{
    public partial class DialogViewModel : ViewModelBase
    {
        private readonly Action _close;

        [ObservableProperty]
        private string _dialogText;

        public DialogViewModel(string? text = null, Action close = null)
        {

            DialogText = text ?? string.Empty;
            _close = close;
        }

        [RelayCommand]
        public async void CloseDialogWindow()
        {
            _close();
        }
    }
}
