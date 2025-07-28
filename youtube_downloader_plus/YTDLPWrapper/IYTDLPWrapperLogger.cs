using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YTDLPWrapper
{
    public interface IYTDLPWrapperLogger
    {
        public void WriteLog(string line);
    }
}
