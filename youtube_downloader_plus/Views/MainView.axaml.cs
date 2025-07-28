using Avalonia.Controls;
using YTDLPWrapper;
using System.Threading;
using System.Diagnostics;
using Avalonia.Interactivity;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using System;
using Avalonia.Controls.Shapes;
using youtube_downloader_plus.ViewModels;
using Avalonia;
using System.Reflection;
using System.ComponentModel;
namespace youtube_downloader_plus.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        //string EXE = Assembly.GetExecutingAssembly().Location.Replace(".dll", ".ini").Replace(".exe", ".ini");
        //Debug.WriteLine($"MY PROGRAM EXE PATH IS : {EXE}");
    }

    private void UrlChanged(object sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property != TextBox.TextProperty) return;

        var vm = (MainViewModel)DataContext;
        vm.ResetAdvancedOptions();
    }

    private void SavePathChanged (object sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property != TextBox.TextProperty) return;
        var vm = (MainViewModel)DataContext;
        vm.SavePathVariableToConfig(e.NewValue?.ToString() ?? "");
    }

   
}

