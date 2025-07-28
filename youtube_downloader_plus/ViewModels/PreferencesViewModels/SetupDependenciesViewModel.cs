using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youtube_downloader_plus.ViewModels.PreferencesViewModels
{
    internal class SetupDependenciesViewModel : ViewModelBase
    {
        private Config _config;

        public SetupDependenciesViewModel(Config config)
        {
            Debug.WriteLine("SetupDependenciesViewModel was created!");
            _config = config;
        }
    }
}
