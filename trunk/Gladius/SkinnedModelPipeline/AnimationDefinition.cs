using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;

namespace SkinnedModelPipeline
{
    /// <summary>
    /// A class for storing our animation definitions
    /// </summary>
    public class AnimationDefinition
    {
        /// <summary>
        /// The original clip name that was exported by the modelling package
        /// Usually this will be Take 001
        /// </summary>
        public string OriginalClipName
        {
            get;
            set;
        }

        /// <summary>
        /// The number of frames in the original animation
        /// </summary>
        public int OriginalFrameCount
        {
            get;
            set;
        }
        
        /// <summary>
        /// A class for storing information about individual clips that we want to create
        /// </summary>
        public class ClipPart
        {
            /// <summary>
            /// The name we have given the clip
            /// </summary>
            public string ClipName
            {
                get;
                set;
            }

            /// <summary>
            /// The starting frame of the clip
            /// </summary>
            public int StartFrame
            {
                get;
                set;
            }

            /// <summary>
            /// The ending frame of the clip
            /// </summary>
            public int EndFrame
            {
                get;
                set;
            }

            /// <summary>
            /// A class for defining events in an animation
            /// </summary>
            public class Event
            {
                /// <summary>
                /// The name of the event
                /// </summary>
                public string Name
                {
                    get;
                    set;
                }

                /// <summary>
                /// The frame that the event fires on
                /// </summary>
                public int Keyframe
                {
                    get;
                    set;
                }
            };

            /// <summary>
            /// Our list of events in this animation clip
            /// Animation clips do not require events, so this is marked as optional
            /// </summary>
            [Microsoft.Xna.Framework.Content.ContentSerializer(Optional = true)]
            public List<Event> Events
            {
                get;
                set;
            }
        };

        /// <summary>
        /// The list of clip parts that we are breaking the original clip into
        /// </summary>
        public List<ClipPart> ClipParts
        {
            get;
            set;
        }
    }
}
