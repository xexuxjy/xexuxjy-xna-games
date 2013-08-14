#region File Description
//-----------------------------------------------------------------------------
// AnimationClip.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace CpuSkinningDataTypes
{
    /// <summary>
    /// An animation clip is the runtime equivalent of the
    /// Microsoft.Xna.Framework.Content.Pipeline.Graphics.AnimationContent type.
    /// It holds all the keyframes needed to describe a single animation.
    /// 
    /// This class was taken from the original Skinned Model Sample:
    /// http://creators.xna.com/en-US/sample/skinnedmodel 
    /// </summary>
    public class AnimationClip
    {
        /// <summary>
        /// Gets the total length of the animation.
        /// </summary>
        [ContentSerializer]
        public TimeSpan Duration { get; private set; }

        /// <summary>
        /// Gets a combined list containing all the keyframes for all bones,
        /// sorted by time.
        /// </summary>
        [ContentSerializer]
        public List<Keyframe> Keyframes { get; private set; }

        /// <summary>
        /// Constructs a new animation clip object.
        /// </summary>
        public AnimationClip(TimeSpan duration, List<Keyframe> keyframes, List<AnimationEvent> events, string name)
        {
            Duration = duration;
            Keyframes = keyframes;
            Events = events;
            Name = name;
        }

        /// <summary>
        /// Callback events for the animation clips
        /// </summary>
        [ContentSerializer]
        public List<AnimationEvent> Events { get; private set; }

        /// <summary>
        /// The name of the clip
        /// </summary>
        [ContentSerializer]
        public string Name { get; private set; }
        /// <summary>
        /// Private constructor for use by the XNB deserializer.
        /// </summary>
        private AnimationClip()
        {
        }
    }
}
