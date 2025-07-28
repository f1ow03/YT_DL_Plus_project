//to disable non-awaited async warning
#pragma warning disable CS4014

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using youtube_downloader_plus.Views;
using YTDLPWrapper;

namespace youtube_downloader_plus.ViewModels;

public partial class MainViewModel : ViewModelBase, IYTDLPWrapperLogger
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoadAvancedOptionsCommand))]
    private YoutubeSourceInfo _sourceInfo = new YoutubeSourceInfo();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DownloadFileCommand))]
    [NotifyCanExecuteChangedFor(nameof(LoadAvancedOptionsCommand))]
    private object? _selectedVideo;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DownloadFileCommand))]
    [NotifyCanExecuteChangedFor(nameof(LoadAvancedOptionsCommand))]
    private object? _selectedAudio;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DownloadFileCommand))]
    [NotifyCanExecuteChangedFor(nameof(LoadAvancedOptionsCommand))]
    private string _url = string.Empty;

    [ObservableProperty]
    private int _selectedAudioIndex;

    [ObservableProperty]
    private int _selectedVideoIndex;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DownloadFileCommand))]
    private string _filePath = string.Empty;

    [ObservableProperty]
    private string _fileName = "%(title)s.%(ext)s";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DownloadFileCommand))]
    [NotifyCanExecuteChangedFor(nameof(LoadAvancedOptionsCommand))]
    private bool _advancedOptionsEnabled = false;

    [ObservableProperty]
    private bool _advancedOptionsChecked = false;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DownloadFileCommand))]
    private bool _excludeVideoDownloadEnabled = false;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DownloadFileCommand))]
    private bool _excludeAudioDownloadEnabled = false;

    [ObservableProperty]
    private string _statusLine = string.Empty;

    [ObservableProperty]
    private int _statusCaretIndex = 0;

    private YoutubeDLPWrapper _wrapper;

    private bool CanFetch() => !Fetching;

    private Config _activeConfig;

    public MainViewModel()
    {
        LoadConfigVariables();
        _wrapper = new YoutubeDLPWrapper(this);
    }

    private bool CanStartDownload()
    {
        Debug.WriteLine("Check called!");
        if (AdvancedOptionsEnabled)
        {
            if (ExcludeVideoDownloadEnabled)
            {
                return SelectedAudio != null;
            }
            if (ExcludeAudioDownloadEnabled)
            {
                return SelectedVideo != null;
            }
            return SelectedVideo != null && SelectedAudio != null;
        }
        else
        {
            return !string.IsNullOrEmpty(Url) && !string.IsNullOrEmpty(FilePath);
        }

    }

    private bool CanLoadAdvancedOptions()
    {
        if (string.IsNullOrEmpty(Url))
        {
            return false;
        }
        if (!string.IsNullOrEmpty(Url) && !string.IsNullOrEmpty(SourceInfo.Url) && Url == SourceInfo.Url)
        {
            return true;
        }
        return true;

    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoadAvancedOptionsCommand))]
    private bool _fetching;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DownloadFileCommand))]
    private bool _downloading;

    private async Task<IReadOnlyList<IStorageFolder>> DoSaveFilePickerAsync()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.StorageProvider is not { } provider)
            throw new NullReferenceException("Missing StorageProvider instance.");
        var storageInfo = await provider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
        {
            Title = "Pick save location"
        });
        return storageInfo;
    }
    [RelayCommand(CanExecute = nameof(CanStartDownload))]
    public async void DownloadFile()
    {
        _activeConfig = Config.GetActive();
        var defaultDownloadParam = typeof(YTDLPDownloadParams)
            .GetField(_activeConfig.DefaultDownloadValue.ToString())?
            .GetCustomAttribute<YTDLPConsoleParamName>(false)?.Value;
        Downloading = true;

        var a = _activeConfig.DefaultDownloadValue.ToString();
        var wrapperParams = new WrapperParams();
        if (AdvancedOptionsChecked == true)
        {
            wrapperParams.AudioID = SourceInfo.AvailableFormats.AudioCodecs[SelectedAudioIndex].ID;
            wrapperParams.VideoID = SourceInfo.AvailableFormats.VideoCodecs[SelectedVideoIndex].ID;
            wrapperParams.FileName = YoutubeDLPWrapper.DefaultFileNameMask;
            wrapperParams.AudioDisabled = ExcludeAudioDownloadEnabled;
            wrapperParams.VideoDisabled = ExcludeVideoDownloadEnabled;
        }
        else
        {
            wrapperParams.VideoDisabled = true;
            wrapperParams.AudioDisabled = true;
            wrapperParams.MediaParams = _activeConfig.DefaultDownloadValue;
            if ((_activeConfig.DefaultDownloadValue & YTDLPDownloadParams.BestAudio) != 0)
            {
                wrapperParams.AudioID = defaultDownloadParam;
                wrapperParams.AudioDisabled = false;
                wrapperParams.FileName = FileName;
            } 
            if ((_activeConfig.DefaultDownloadValue & YTDLPDownloadParams.BestVideo) != 0)
            {
                wrapperParams.VideoID = defaultDownloadParam;
                wrapperParams.VideoDisabled = false;
                wrapperParams.FileName = FileName;
            }

        }
        wrapperParams.Url = Url;
        wrapperParams.FilePath = FilePath;
        Debug.WriteLine($"{wrapperParams.AudioID}, {wrapperParams.VideoID}, {wrapperParams.FileName}");
        var x = () => OpenDialog("Download completed");
        Task.Run(async () =>
        {
            await _wrapper.Download(wrapperParams);
            Downloading = false;
            WriteLog($"Download completed. File saved at {FilePath}");
            OpenDialog("Download completed");
        });
    }

    [RelayCommand]
    public async void SaveFile()
    {
        var storageInfo = await DoSaveFilePickerAsync();
        if (storageInfo != null && storageInfo.Any())
        {
            FilePath = storageInfo[0].Path?.AbsolutePath?.ToString() ?? "";
        }
    }
    [RelayCommand(CanExecute = nameof(CanLoadAdvancedOptions))]
    public async void LoadAvancedOptions()
    {
        if (string.IsNullOrEmpty(SourceInfo.Url) || Url != SourceInfo.Url)
        {
            Fetching = true;
            Task.Run(async () =>
            {
                var getResult = await Dispatcher.UIThread.InvokeAsync(() => _wrapper.FetchCodecInfo(Url), DispatcherPriority.Background);
                SourceInfo = getResult;
                Fetching = false;
                AdvancedOptionsEnabled = true;
                Debug.WriteLine($"GOT THE CODECS!");
                Debug.WriteLine($"SrInfo = {SourceInfo.Url}, Url = {Url}");
                WriteLog("Advanced options loaded. Check Video format and Audio format fields.");
            });
            return;
        }
        if (Url == SourceInfo.Url)
        {
            Debug.WriteLine($"CURRENT URL MATCHES SOURCEINFO URL NO NEED TO RE-FETCH!");
        }
    }

    [RelayCommand]
    public void ToggleAdvancedOptions()
    {
        var mainWindow = GetMainWindow();
        mainWindow.Height = 100;
    }

    [RelayCommand]
    public async void OpenAboutDialog()
    {
        var window = new AboutProgramWindow();
        var mainWindow = GetMainWindow();
        var result = await window.ShowDialog<bool>(mainWindow);
    }

    [RelayCommand]
    public async void OpenPreferences()
    {
        var window = new PreferencesWindow();
        var mainWindow = GetMainWindow();
        var result = await window.ShowDialog<bool>(mainWindow);
    }

    public void ResetAdvancedOptions()
    {
        AdvancedOptionsChecked = false;
        AdvancedOptionsEnabled = false;
        SourceInfo = new YoutubeSourceInfo();
        SelectedAudioIndex = 0;
        SelectedVideoIndex = 0;
    }

    public void SavePathVariableToConfig(string param)
    {
        Config.GetActive().DefaultSaveFolder = param;
    }

    public void LoadConfigVariables()
    {
        FilePath = Config.GetActive().DefaultSaveFolder;
    }

    public void WriteLog(string line)
    {
        if (string.IsNullOrEmpty(StatusLine))
        {
            StatusLine = line;
        }
        StatusLine += $"\n{line}";
        StatusCaretIndex = StatusLine.Length;
    }

    public async void OpenDialog(string text)
    {
        var window = new DialogWindow(text);
        var mainWindow = GetMainWindow();
        var result = await window.ShowDialog<bool>(mainWindow);
    }

    private Window GetMainWindow()
    {
        var lifeTime = (IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime;
        var mainWindow = lifeTime.MainWindow;
        return mainWindow;
    }
}
