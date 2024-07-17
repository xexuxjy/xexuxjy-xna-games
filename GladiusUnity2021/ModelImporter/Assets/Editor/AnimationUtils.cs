using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Editor
{
    public class AnimationUtils
    {
        public const float FrameRate = 30;
        public const float FrameTime = 1f / FrameRate;

        
        
        public static void CreatePANFromAnimClip(AnimationClip ac)
        {
            using (BinaryWriter bw = new BinaryWriter(new MemoryStream()))
            {
                

                
                
            }
        }


        public void WriteData(BinaryWriter writer,GladiusAnimationClip gac)
        {
            GladiusFileWriter.WriteVERS(writer);
            GladiusFileWriter.PadIfNeeded(writer);
            GladiusFileWriter.WriteCPRT(writer);
            GladiusFileWriter.PadIfNeeded(writer);
            WriteHEDR(writer);
            GladiusFileWriter.PadIfNeeded(writer);
            WriteBLNM(writer, null);
            GladiusFileWriter.PadIfNeeded(writer);
            WriteMASK(writer, null);
            GladiusFileWriter.PadIfNeeded(writer);
            WriteBLTK(writer);
            GladiusFileWriter.PadIfNeeded(writer);
            WriteBKTM(writer, null);
            GladiusFileWriter.PadIfNeeded(writer);
            WriteBOOL(writer, null);
            GladiusFileWriter.PadIfNeeded(writer);


            // these both work with the vectors
            WriteOPTR(writer, null);
            GladiusFileWriter.PadIfNeeded(writer);
            WriteOVEC(writer, null);
            GladiusFileWriter.PadIfNeeded(writer);

            // these all work with the quaternions
            WriteORTR(writer, null);
            GladiusFileWriter.PadIfNeeded(writer);
            WriteARKT(writer, null);
            GladiusFileWriter.PadIfNeeded(writer);
            WriteOQUA(writer, null);
            GladiusFileWriter.PadIfNeeded(writer);


        }
        
        public void WriteHEDR(BinaryWriter writer)
        {
            int total = GladiusFileWriter.HeaderSize;
            GladiusFileWriter.WriteASCIIString(writer, "HEDR");
            writer.Write(0);
            writer.Write(1); // num materials, 1 for now
            writer.Write(1);
        }

        public void WriteNAME(BinaryWriter writer,List<string> boneNames)
        {
            int total = GladiusFileWriter.HeaderSize;
            GladiusFileWriter.WriteASCIIString(writer, "NAME");
            writer.Write(0);
            writer.Write(1); // num materials, 1 for now
            writer.Write(boneNames.Count);
            //GladiusFileWriter.WriteStringList(writer, m_selsInfo, (paddedTotal - GladiusFileWriter.HeaderSize));
            GladiusFileWriter.WriteStringList(writer,boneNames,0);
        }

        // this is a list of animation event names.e.g. : detachShield detachWeapon1 detachWeapon2 footStepL footStepR hideShield hideWeapon1 hideWeapon2 hit react show
        public void WriteBLNM(BinaryWriter writer, List<string> boneNames)
        {

        }

        public void WriteMASK(BinaryWriter writer, List<string> boneNames)
        {

        }

        
        public void WriteBLTK(BinaryWriter writer)
        {
        }

        
        // write timestamps.
        public void WriteBKTM(BinaryWriter writer, GladiusAnimationClip gac)
        {
            int numEvents = gac.m_numEvents;
            int total = GladiusFileWriter.HeaderSize + (numEvents * 4);

            GladiusFileWriter.WriteASCIIString(writer, "BLTK");
            writer.Write(total);
            writer.Write(0); 
            writer.Write(numEvents);

            for (int i = 0; i < numEvents; ++i)
            {
                writer.Write((float)i * FrameTime);
            }
        }

        public static void WriteBOOL(BinaryWriter writer, GladiusAnimationClip gac)
        {
            int numEvents = gac.m_numEvents;
            int total = GladiusFileWriter.HeaderSize + (numEvents * 4);

            GladiusFileWriter.WriteASCIIString(writer, "BOOL");
            writer.Write(total);
            writer.Write(0); 
            writer.Write(numEvents);

            for (int i = 0; i < numEvents; ++i)
            {
                writer.Write(0);
            }
        }
        
        public static void WritePOSI(BinaryWriter writer,List<IndexedVector3> points)
        {
            int total = GladiusFileWriter.HeaderSize;
            total += (points.Count * 12);
            int paddedTotal = GladiusFileWriter.GetPadValue(total);

            GladiusFileWriter.WriteASCIIString(writer, "POSI");
            writer.Write(paddedTotal); // block size
            writer.Write(1);
            writer.Write(points.Count); // number of elements.

        
            foreach (IndexedVector3 v in points)
            {
                Common.WriteVector3BE(writer, v);
            }
        
            GladiusFileWriter.WriteNull(writer, (paddedTotal - total));
        
        }

        public static void WriteOPTR(BinaryWriter writer,GladiusAnimationClip gladiusAnimationClip)
        {
            int total = GladiusFileWriter.HeaderSize;
            int paddedTotal = GladiusFileWriter.GetPadValue(total);

            GladiusFileWriter.WriteASCIIString(writer, "OPTR");
            writer.Write(paddedTotal); // block size
            writer.Write(1);
            writer.Write(1); // number of elements.
            
            
            
            
            
        }

        public static void WriteORTR(BinaryWriter writer,GladiusAnimationClip gladiusAnimationClip)
        {
            int total = GladiusFileWriter.HeaderSize;
            int paddedTotal = GladiusFileWriter.GetPadValue(total);

            GladiusFileWriter.WriteASCIIString(writer, "ORTR");
            writer.Write(paddedTotal); // block size
            writer.Write(1);
            writer.Write(1); // number of elements.
            
            
        }

        public static void WriteOVEC(BinaryWriter writer,GladiusAnimationClip gladiusAnimationClip)
        {
            int total = GladiusFileWriter.HeaderSize;
            int paddedTotal = GladiusFileWriter.GetPadValue(total);

            GladiusFileWriter.WriteASCIIString(writer, "OVEC");
            writer.Write(paddedTotal); // block size
            writer.Write(1);
            writer.Write(1); // number of elements.


            int totalElements = 0;
            foreach (var track in gladiusAnimationClip.PositionData)
            {
                totalElements += track.Count;
            }

            foreach (var track in gladiusAnimationClip.PositionData)
            {
                foreach (Vector3 v in track)
                {
                    optVec.ToStream(writer,optVec.Put(v, Vector3.one));
                }
            }
        }

        
        public static void WriteOQUA(BinaryWriter writer,GladiusAnimationClip gladiusAnimationClip)
        {
            int total = GladiusFileWriter.HeaderSize;
            int paddedTotal = GladiusFileWriter.GetPadValue(total);

            GladiusFileWriter.WriteASCIIString(writer, "OQUA");
            writer.Write(paddedTotal); // block size
            writer.Write(1);
            writer.Write(1); // number of elements.

            int totalElements = 0;
            foreach (var track in gladiusAnimationClip.RotationData)
            {
                totalElements += track.Count;
            }

            foreach (var track in gladiusAnimationClip.RotationData)
            {
                foreach (Quaternion q in track)
                {
                    optQuat.ToStream(writer,optQuat.Put(Vector3.one,q));
                }
            }
        }

        public static void  WriteARKT(BinaryWriter writer,GladiusAnimationClip gladiusAnimationClip)
        {
            int total = GladiusFileWriter.HeaderSize;
            int paddedTotal = GladiusFileWriter.GetPadValue(total);

            GladiusFileWriter.WriteASCIIString(writer, "ARKT");
            writer.Write(paddedTotal); // block size
            writer.Write(1);
            writer.Write(1); // number of elements.


            int totalElements = 0;
            foreach (var track in gladiusAnimationClip.RotationData)
            {
                totalElements += track.Count;
            }

            foreach (var track in gladiusAnimationClip.RotationData)
            {
                foreach (Quaternion q in track)
                {
                    ushort key = 0;
                    writer.Write(key);
                }
            }
        }
        
    }

    public class GladiusAnimationClip
    {
        public AnimationClip m_animationClip;
        public int m_numEvents = 0;

        public List<string> TrackBoneNames = new List<string>();
        public List<List<Vector3>> PositionData = new List<List<Vector3>>();
        public List<List<Quaternion>> RotationData = new List<List<Quaternion>>();

        public GladiusAnimationClip(AnimationClip animationClip)
        {
            m_animationClip = animationClip;
            m_numEvents = (int)(m_animationClip.length / AnimationUtils.FrameTime);
        }
    
        
        
    }
    
    
    
}