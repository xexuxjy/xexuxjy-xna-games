using System;
using System.IO;

namespace com.xexuxjy.magiccarpet.util.debug
{
    public class EventLogger
    {
        public EventLogger()
        {
            string filename = @"../../../logs/action-logs.txt";
            FileStream filestream = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.Read);
            m_streamWriter = new StreamWriter(filestream);

        }


        public void LogEvent(String eventDetails)
        {
#if WINDOWS
            m_streamWriter.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff")+" : "+eventDetails);
#endif
        }

#if WINDOWS
        StreamWriter m_streamWriter = null;
#endif

        public void Dispose()
        {
            m_streamWriter.Flush();
            m_streamWriter.Close();
            
        }
    }
}
