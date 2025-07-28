using Avalonia.Controls;
using youtube_downloader_plus.ViewModels;

namespace youtube_downloader_plus.Views

{
    public partial class PreferencesWindow : Window
    {
        public PreferencesWindow()
        {
            var vm = new PreferencesViewModel(() => Close());
            DataContext = vm;
            InitializeComponent();
        }
    }
}