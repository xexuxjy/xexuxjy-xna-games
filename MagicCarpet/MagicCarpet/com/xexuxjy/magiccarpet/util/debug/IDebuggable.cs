using System;
using System.Collections.Generic;
using System.Text;

namespace com.xexuxjy.magiccarpet.util.debug
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
