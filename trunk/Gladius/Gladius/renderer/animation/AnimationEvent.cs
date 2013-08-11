#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace Gladius.renderer.animation
{
    /// <summary>
    /// Information about an event in our animation
    /// </summary>
    public class AnimationEvent
    {
        /// <summary>
        /// The name of the event
        /// </summary>
        public String EventName
        {
            get;
            set;
        }

        /// <summary>
        /// The time of the event
        /// </summary>
        public TimeSpan EventTime
        {
            get;
            set;
        }
    }
}
