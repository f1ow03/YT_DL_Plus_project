using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using youtube_downloader_plus.ViewModels;

namespace youtube_downloader_plus.Views
{
    public partial class PreferencesView : UserControl
    {
        public PreferencesView()
        {
            InitializeComponent();
        }

        private void PropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property != ListBox.SelectedIndexProperty) return;

            var vm = (PreferencesViewModel)DataContext;
            vm.ActivateView((int)e.NewValue);
        }
       
    }
}
