using System;
using System.Collections.Generic;
using System.Text;

namespace com.xexuxjy.utils.debug
{
    public interface IDebuggable
    {
        String DebugText
        {
            get;
        }
        bool DebugEnabled
        {
            get;
            set;
        }
    }
}
