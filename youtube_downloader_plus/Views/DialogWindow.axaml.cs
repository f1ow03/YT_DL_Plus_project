using Avalonia.Controls;
using youtube_downloader_plus.ViewModels;

namespace youtube_downloader_plus.Views
{
    public partial class DialogWindow : Window
    {
        public DialogWindow() { }

        public DialogWindow(string text)
        {
            var vm = new DialogViewModel(text, () => Close());
            DataContext = vm;
            InitializeComponent();
        }

    }
}
