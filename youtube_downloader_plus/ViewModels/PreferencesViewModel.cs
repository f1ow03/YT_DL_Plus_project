using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using youtube_downloader_plus.ViewModels.PreferencesViewModels;

namespace youtube_downloader_plus.ViewModels
{
    public enum PreferencesMenu
    {
        [Display(Name = "Download options")]
        DownloadOptions,
        [Display(Name = "Dependencies")]
        Dependencies
    }

    public partial class PreferencesViewModel : ViewModelBase
    {
        private readonly Action _close;
        private readonly Config _config;

        public PreferencesViewModel(Action close = null)
        {
            _close = close;
            _config = Config.GetCopy();
            _CurrentViewModel = new DownloadOptionsViewModel(_config);
        }

        [ObservableProperty]
        private List<PreferencesMenu> _menuOptions = new List<PreferencesMenu>()
        {
            PreferencesMenu.DownloadOptions,
            PreferencesMenu.Dependencies
        };

        [ObservableProperty]
        private ViewModelBase _CurrentViewModel;

        [ObservableProperty]
        private int _selectedIndex;

        public void ActivateView(int selectedMenuIndex)
        {

            CurrentViewModel = selectedMenuIndex switch
            {
                0 => new DownloadOptionsViewModel(_config),
                1 => new SetupDependenciesViewModel(_config),
                _ => throw new ArgumentException($"unknown menu option")
            };
        }
        public void SaveYTDLPWrapperValue(string value)
        {

        }
        [RelayCommand]
        public async void CloseAndSave()
        {
            _config.SetActive();
            _close();
        }

        [RelayCommand]
        public async void CloseWindow()
        {
            _close();
        }
    }

}
