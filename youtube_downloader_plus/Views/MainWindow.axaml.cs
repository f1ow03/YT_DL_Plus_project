using Avalonia.Controls;
using System;
using System.ComponentModel;
using System.Diagnostics;
using youtube_downloader_plus.ViewModels;

namespace youtube_downloader_plus.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        Config.Load();
        Closing += HandleClosed;
        Closed += HandleClosed;
        Closed += HandleClosed2ElectricBoogaloo;
        InitializeComponent();
    }


    void HandleClosed(object? sender, EventArgs e)
    {
        Config.Save();
        Debug.WriteLine("HANDLER 1 CLOSED");
    }

    void HandleClosed2ElectricBoogaloo(object? sender, EventArgs e)
    {

        Debug.WriteLine("HANDLER 2 CLOSED");
    }

}
