
using System.Collections.Generic;
namespace com.xexuxjy.magiccarpet.threading
{
    public class ChangeBuffer
    {
        public List<ChangeMessage> Messages { get; set; }

        public ChangeBuffer()
        {
            Messages = new List<ChangeMessage>();
        }
        public void Add(ChangeMessage msg)
        {
            Messages.Add(msg);
        }
        public void Clear()
        {
            Messages.Clear();
        }

    }
}
