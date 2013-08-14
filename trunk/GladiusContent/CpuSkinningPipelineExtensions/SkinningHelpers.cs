#region File Description
//-----------------------------------------------------------------------------
// SkinningHelper.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using CpuSkinningDataTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System.IO;

namespace CpuSkinningPipelineExtensions
{
    /// <summary>
    /// A static class that generates SkinningData from a NodeContent object, for getting skinning data out of a model.
    /// 
    /// This code was refactored out of the SkinnedModelProcessor from the Skinned Model sample:
    /// http://creators.xna.com/en-US/sample/skinnedmodel
    /// </summary>
    public static class SkinningHelpers
    {
        public static SkinningData GetSkinningData(NodeContent input, ContentProcessorContext context, int maxBones)
        {
            ValidateMesh(input, context, null);

            // Find the skeleton.
            BoneContent skeleton = MeshHelper.FindSkeleton(input);
            
            if (skeleton == null)
                throw new InvalidContentException("Input skeleton not found.");

            // We don't want to have to worry about different parts of the model being
            // in different local coordinate systems, so let's just bake everything.
            FlattenTransforms(input, skeleton);

            // Read the bind pose and skeleton hierarchy data.
            IList<BoneContent> bones = MeshHelper.FlattenSkeleton(skeleton);

            if (bones.Count > maxBones)
            {
                throw new InvalidContentException(string.Format("Skeleton has {0} bones, but the maximum supported is {1}.", bones.Count, maxBones));
            }

            List<Matrix> bindPose = new List<Matrix>();
            List<Matrix> inverseBindPose = new List<Matrix>();
            List<int> skeletonHierarchy = new List<int>();

            foreach (BoneContent bone in bones)
            {
                bindPose.Add(bone.Transform);
                inverseBindPose.Add(Matrix.Invert(bone.AbsoluteTransform));
                skeletonHierarchy.Add(bones.IndexOf(bone.Parent as BoneContent));
            }

            // Convert animation data to our runtime format.
            Dictionary<string, AnimationClip> animationClips;
            animationClips = ProcessAnimations(skeleton.Animations, bones,context,input.Identity);

            return new SkinningData(animationClips, bindPose, inverseBindPose, skeletonHierarchy);
        }

        /// <summary>
        /// Converts an intermediate format content pipeline AnimationContentDictionary
        /// object to our runtime AnimationClip format.
        /// </summary>
        static Dictionary<string, AnimationClip> ProcessAnimations(
                    AnimationContentDictionary animations, IList<BoneContent> bones,
                    ContentProcessorContext context, ContentIdentity sourceIdentity)
        {
            // Build up a table mapping bone names to indices.
            Dictionary<string, int> boneMap = new Dictionary<string, int>();

            for (int i = 0; i < bones.Count; i++)
            {
                string boneName = bones[i].Name;

                if (!string.IsNullOrEmpty(boneName))
                    boneMap.Add(boneName, i);
            }

            // Convert each animation in turn.
            Dictionary<string, AnimationClip> animationClips;
            animationClips = new Dictionary<string, AnimationClip>();

            // We process the original animation first, so we can use their keyframes
            foreach (KeyValuePair<string, AnimationContent> animation in animations)
            {
                AnimationClip processed = ProcessAnimation(animation.Value, boneMap, animation.Key);

                animationClips.Add(animation.Key, processed);
            }

            // Check to see if there's an animation clip definition
            // Here, we're checking for a file with the _Anims suffix.
            // So, if your model is named dude.fbx, we'd add dude_Anims.xml in the same folder
            // and the pipeline will see the file and use it to override the animations in the
            // original model file.
            string SourceModelFile = sourceIdentity.SourceFilename;
            string SourcePath = Path.GetDirectoryName(SourceModelFile);
            string AnimFilename = Path.GetFileNameWithoutExtension(SourceModelFile);
            AnimFilename += "_Anims.xml";
            string AnimPath = Path.Combine(SourcePath, AnimFilename);
            if (File.Exists(AnimPath))
            {
                // Add the filename as a dependency, so if it changes, the model is rebuilt
                context.AddDependency(AnimPath);

                // Load the animation definition from the XML file
                AnimationDefinition AnimDef = context.BuildAndLoadAsset<XmlImporter, AnimationDefinition>(new ExternalReference<XmlImporter>(AnimPath), null);

                // Break up the original animation clips into our new clips
                // First, we check if the clips contains our clip to break up
                if (animationClips.ContainsKey(AnimDef.OriginalClipName))
                {
                    // Grab the main clip that we are using
                    AnimationClip MainClip = animationClips[AnimDef.OriginalClipName];

                    // Now remove the original clip from our animations
                    animationClips.Remove(AnimDef.OriginalClipName);

                    // Process each of our new animation clip parts
                    foreach (AnimationDefinition.ClipPart Part in AnimDef.ClipParts)
                    {
                        // Calculate the frame times
                        TimeSpan StartTime = GetTimeSpanForFrame(Part.StartFrame, AnimDef.OriginalFrameCount, MainClip.Duration.Ticks);
                        TimeSpan EndTime = GetTimeSpanForFrame(Part.EndFrame, AnimDef.OriginalFrameCount, MainClip.Duration.Ticks);

                        // Get all the keyframes for the animation clip
                        // that fall within the start and end time
                        List<Keyframe> Keyframes = new List<Keyframe>();
                        foreach (Keyframe AnimFrame in MainClip.Keyframes)
                        {
                            if ((AnimFrame.Time >= StartTime) && (AnimFrame.Time <= EndTime))
                            {
                                Keyframe NewFrame = new Keyframe(AnimFrame.Bone, AnimFrame.Time - StartTime, AnimFrame.Transform);
                                Keyframes.Add(NewFrame);
                            }
                        }

                        // Process the events
                        List<AnimationEvent> Events = new List<AnimationEvent>();
                        if (Part.Events != null)
                        {
                            // Process each event
                            foreach (AnimationDefinition.ClipPart.Event Event in Part.Events)
                            {
                                // Get the event time within the animation
                                TimeSpan EventTime = GetTimeSpanForFrame(Event.Keyframe, AnimDef.OriginalFrameCount, MainClip.Duration.Ticks);

                                // Offset the event time so it is relative to the start of the animation
                                EventTime -= StartTime;

                                // Create the event
                                AnimationEvent NewEvent = new AnimationEvent();
                                NewEvent.EventTime = EventTime;
                                NewEvent.EventName = Event.Name;
                                Events.Add(NewEvent);
                            }
                        }

                        // Create the clip
                        AnimationClip NewClip = new AnimationClip(EndTime - StartTime, Keyframes, Events, Part.ClipName);
                        animationClips[Part.ClipName] = NewClip;
                    }
                }
            }

            if (animationClips.Count == 0)
            {
                throw new InvalidContentException(
                            "Input file does not contain any animations.");
            }

            return animationClips;
        }

        /// <summary>
        /// Gets a TimeSpan value for a frame index in an animation
        /// </summary>
        private static TimeSpan GetTimeSpanForFrame(int FrameIndex, int TotalFrameCount, long TotalTicks)
        {
            float MaxFrameIndex = (float)TotalFrameCount - 1;
            float AmountOfAnimation = (float)FrameIndex / MaxFrameIndex;
            float NumTicks = AmountOfAnimation * (float)TotalTicks;
            return new TimeSpan((long)NumTicks);
        }

        /// <summary>
        /// Converts an intermediate format content pipeline AnimationContent
        /// object to our runtime AnimationClip format.
        /// </summary>
        static AnimationClip ProcessAnimation(AnimationContent animation, Dictionary<string, int> boneMap,string clipName)
        {
            List<Keyframe> keyframes = new List<Keyframe>();

            // For each input animation channel.
            foreach (KeyValuePair<string, AnimationChannel> channel in
                animation.Channels)
            {
                // Look up what bone this channel is controlling.
                int boneIndex;

                if (!boneMap.TryGetValue(channel.Key, out boneIndex))
                {
                    throw new InvalidContentException(string.Format(
                        "Found animation for bone '{0}', " +
                        "which is not part of the skeleton.", channel.Key));
                }

                // Convert the keyframe data.
                foreach (AnimationKeyframe keyframe in channel.Value)
                {
                    keyframes.Add(new Keyframe(boneIndex, keyframe.Time,
                                               keyframe.Transform));
                }
            }

            // Sort the merged keyframes by time.
            keyframes.Sort(CompareKeyframeTimes);

            if (keyframes.Count == 0)
                throw new InvalidContentException("Animation has no keyframes.");

            if (animation.Duration <= TimeSpan.Zero)
                throw new InvalidContentException("Animation has a zero duration.");

            return new AnimationClip(animation.Duration, keyframes, new List<AnimationEvent>(), clipName);
        }

        /// <summary>
        /// Comparison function for sorting keyframes into ascending time order.
        /// </summary>
        static int CompareKeyframeTimes(Keyframe a, Keyframe b)
        {
            return a.Time.CompareTo(b.Time);
        }

        /// <summary>
        /// Makes sure this mesh contains the kind of data we know how to animate.
        /// </summary>
        static void ValidateMesh(NodeContent node, ContentProcessorContext context, string parentBoneName)
        {
            MeshContent mesh = node as MeshContent;

            if (mesh != null)
            {
                // Validate the mesh.
                if (parentBoneName != null)
                {
                    context.Logger.LogWarning(null, null,
                        "Mesh {0} is a child of bone {1}. SkinnedModelProcessor " +
                        "does not correctly handle meshes that are children of bones.",
                        mesh.Name, parentBoneName);
                }

                if (!MeshHasSkinning(mesh))
                {
                    context.Logger.LogWarning(null, null,
                        "Mesh {0} has no skinning information, so it has been deleted.",
                        mesh.Name);

                    mesh.Parent.Children.Remove(mesh);
                    return;
                }
            }
            else if (node is BoneContent)
            {
                // If this is a bone, remember that we are now looking inside it.
                parentBoneName = node.Name;
            }

            // Recurse (iterating over a copy of the child collection,
            // because validating children may delete some of them).
            foreach (NodeContent child in new List<NodeContent>(node.Children))
                ValidateMesh(child, context, parentBoneName);
        }

        /// <summary>
        /// Checks whether a mesh contains skininng information.
        /// </summary>
        static bool MeshHasSkinning(MeshContent mesh)
        {
            foreach (GeometryContent geometry in mesh.Geometry)
            {
                if (!geometry.Vertices.Channels.Contains(VertexChannelNames.Weights()))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Bakes unwanted transforms into the model geometry,
        /// so everything ends up in the same coordinate system.
        /// </summary>
        static void FlattenTransforms(NodeContent node, BoneContent skeleton)
        {
            foreach (NodeContent child in node.Children)
            {
                // Don't process the skeleton, because that is special.
                if (child == skeleton)
                    continue;

                // Bake the local transform into the actual geometry.
                MeshHelper.TransformScene(child, child.Transform);

                // Having baked it, we can now set the local
                // coordinate system back to identity.
                child.Transform = Matrix.Identity;

                // Recurse.
                FlattenTransforms(child, skeleton);
            }
        }
    }
}
