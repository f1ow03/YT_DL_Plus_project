using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YTDLPWrapper;

namespace youtube_downloader_plus.Common
{
    public struct DropdownItem<T>
    {
        public string Name { get; set; }

        public T Value { get; set; }
    }

    public class Term(string id)
    {
        public string Id { get; set; } = id;

        public override string ToString() { 
            //get culture from config and load translation
            return Id;
        }
    }
}
