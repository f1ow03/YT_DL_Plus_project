using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using youtube_downloader_plus.ViewModels;
using youtube_downloader_plus.ViewModels.PreferencesViewModels;

namespace youtube_downloader_plus.Views

{
    public partial class DownloadOptionsView : UserControl
    {
        public DownloadOptionsView()
        {
            InitializeComponent();
        }

        private void ParamChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property != ComboBox.SelectedIndexProperty) return;

            var vm = (DownloadOptionsViewModel)DataContext;
            vm?.SaveTempParam((int)e.NewValue);
        }
    }
}