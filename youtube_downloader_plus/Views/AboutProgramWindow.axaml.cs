using Avalonia.Controls;
using System;
using System.Diagnostics;
using youtube_downloader_plus.ViewModels;

namespace youtube_downloader_plus.Views
{
    public partial class AboutProgramWindow : Window
    {
        public AboutProgramWindow()
        {
            var vm = new AboutProgramViewModel(() => Close());
            DataContext = vm;
            InitializeComponent();
            //Debug.WriteLine("===================");
            //Debug.WriteLine(Content);
            //(AboutProgramView)Content.
        }
    }
}
