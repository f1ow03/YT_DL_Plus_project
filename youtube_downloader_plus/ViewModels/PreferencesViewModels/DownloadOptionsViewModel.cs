using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using youtube_downloader_plus.Common;
using YTDLPWrapper;

namespace youtube_downloader_plus.ViewModels.PreferencesViewModels
{
    public partial class DownloadOptionsViewModel : ViewModelBase
    {

        private static readonly List<DropdownItem<YTDLPDownloadParams>> _dropdownContents = new ()
        {
            new DropdownItem<YTDLPDownloadParams> {
                Name = "only best audio",
                Value = YTDLPDownloadParams.BestAudio
            },
            new DropdownItem<YTDLPDownloadParams> {
                Name = "only best video",
                Value = YTDLPDownloadParams.BestVideo
            },
            new DropdownItem<YTDLPDownloadParams> {
                Name = "bestvideo + bestaudio",
                Value = YTDLPDownloadParams.BestVideo | YTDLPDownloadParams.BestAudio
            }
        };

        private Config _config;

        [ObservableProperty]
        private List<string> _dropdownItems;

        [ObservableProperty]
        private string _filePath = string.Empty;

        [ObservableProperty]
        private int _selectedOptionIndex;

        public DownloadOptionsViewModel(Config config)
        {
            Debug.WriteLine("DownloadOptionsViewModel was created!");
            DropdownItems = _dropdownContents.Select(x => x.Name).ToList();
            LoadedDefaultDownloadOption(config.DefaultDownloadValue);
            _config = config;

            var a = new DropdownItem<YTDLPDownloadParams>();
            var b = new DropdownItem<string>();
        }

        void LoadedDefaultDownloadOption(YTDLPDownloadParams option)
        {
            var index = _dropdownContents.FindIndex(x => x.Value == option);
            if (index >= 0)
            {
                SelectedOptionIndex = index;
                return;
            }
            SelectedOptionIndex = 0;
        }
        public void SaveTempParam(int newIndex)
        {
            Debug.WriteLine($"INDEX CHANGED: {newIndex}");
            if (newIndex >= 0 && newIndex < _dropdownContents.Count()) 
            {
                _config.DefaultDownloadValue = _dropdownContents[newIndex].Value;
            }
        }

    }
}