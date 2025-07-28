using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YTDLPWrapper
{
    public class WrapperParams
    {
        public string Url { get; set; }
        public YTDLPDownloadParams? MediaParams { get; set; } = null;
        public string? AudioID { get; set; }
        public string? VideoID { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public bool AudioDisabled { get; set; }
        public bool VideoDisabled { get; set; }
    }
}
