#region File Description
//-----------------------------------------------------------------------------
// AnimationClip.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
#endregion

namespace Gladius.renderer.animation
{
    /// <summary>
    /// An animation clip is the runtime equivalent of the
    /// Microsoft.Xna.Framework.Content.Pipeline.Graphics.AnimationContent type.
    /// It holds all the keyframes needed to describe a single animation.
    /// </summary>
    public class AnimationClip
    {
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
        /// Private constructor for use by the XNB deserializer.
        /// </summary>
        private AnimationClip()
        {
        }


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
        /// Callback events for the animation clips
        /// </summary>
        [ContentSerializer]
        public List<AnimationEvent> Events { get; private set; }

        /// <summary>
        /// The name of the clip
        /// </summary>
        [ContentSerializer]
        public string Name { get; private set; }
    }
}
