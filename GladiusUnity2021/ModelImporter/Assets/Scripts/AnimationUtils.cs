using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

public static class AnimationUtils
{
    public const float FrameRate = 30;
    public const float FrameTime = 1f / FrameRate;

    
    public const float ScalarItem = 1.0f / 32767.0f;

    public static Vector3 PosScalar = new Vector3(ScalarItem, ScalarItem, ScalarItem);

    public static void CreatePANFromAnimClip(AnimationClip ac)
    {
        using (BinaryWriter bw = new BinaryWriter(new MemoryStream()))
        {
        }
    }

    
    
    

    public static void WriteData(BinaryWriter writer, GladiusAnimationClip gac)
    {
        int numEvents = gac.m_numEvents;
        List<optQuat> optQuatList = new List<optQuat>();

        GladiusFileWriter.WriteVERS(writer);
        GladiusFileWriter.PadIfNeeded(writer);
        GladiusFileWriter.WriteCPRT(writer);
        GladiusFileWriter.PadIfNeeded(writer);
        WriteHEDR(writer);
        GladiusFileWriter.PadIfNeeded(writer);
        WriteNAME(writer, null);
        
        WriteBLNM(writer, null);
        GladiusFileWriter.PadIfNeeded(writer);
        WriteMASK(writer, null);
        GladiusFileWriter.PadIfNeeded(writer);
        WriteBLTK(writer);
        GladiusFileWriter.PadIfNeeded(writer);
        WriteBKTM(writer,numEvents);
        GladiusFileWriter.PadIfNeeded(writer);
        WriteBOOL(writer,numEvents);
        GladiusFileWriter.PadIfNeeded(writer);


        // these both work with the vectors
        WriteOPTR(writer, gac.PositionData,null);
        //GladiusFileWriter.PadIfNeeded(writer);
        WriteOVEC(writer,gac.PositionData,null,null);
        GladiusFileWriter.PadIfNeeded(writer);

        // these all work with the quaternions
        WriteORTR(writer, gac.RotationData);
        GladiusFileWriter.PadIfNeeded(writer);
        WriteARKT(writer, null);
        GladiusFileWriter.PadIfNeeded(writer);
        WriteOQUA(writer, null);
        GladiusFileWriter.PadIfNeeded(writer);
        GladiusFileWriter.WriteEND(writer);
    }

    public static void WriteData(BinaryWriter writer, GladiusSimpleAnim simpleAnim)
    {
        AnimationData animationData = simpleAnim.m_allAnimations[0];
        int numEvents = animationData.boolTrack.mNumKeys;

        List<List<Vector3>> positionData = new List<List<Vector3>>();
        List<List<ushort>> positionTimeData = new List<List<ushort>>();

        List<Vector3> trackPosScalarData = new List<Vector3>();


        for (int i = 0; i < animationData.optPosTrack.m_tracks.Count; ++i)
        {
            List<Vector3> data = new List<Vector3>();
            List<ushort> timeData = new List<ushort>();
            
            positionData.Add(data);
            positionTimeData.Add(timeData);

            trackPosScalarData.Add(animationData.optPosTrack.m_tracks[i].mPosScalar);
            
            foreach (var optVec in animationData.optPosTrack.m_tracks[i].mOptVecs)
            {
                Vector3 dst = Vector3.zero;
                Vector3 scalar = animationData.optPosTrack.m_tracks[i].mPosScalar;
                
                optVec.Get(ref dst, ref scalar);
                timeData.Add(optVec.time);
                data.Add(dst);
            }
        }

        
        List<List<Quaternion>> rotationData = new List<List<Quaternion>>();
        List<List<ushort>> rotationTimeData = new List<List<ushort>>();
        for (int i = 0; i < animationData.optRotTrack.m_tracks.Count; ++i)
        {
            List<Quaternion> data = new List<Quaternion>();
            List<ushort> timeData = new List<ushort>();

            rotationData.Add(data);
            rotationTimeData.Add(timeData);
            
            int count = 0;
            foreach (var optQuat in animationData.optRotTrack.m_tracks[i].mOptQuats)
            {
                Quaternion q = Quaternion.identity;
                optQuat.Get(ref q);
                data.Add(q);
                timeData.Add(animationData.optRotTrack.m_tracks[i].mKeyTimes[count++]);

            }
        }


        

        GladiusFileWriter.WriteVERS(writer);
        GladiusFileWriter.WriteCPRT(writer);
        WriteHEDR(writer);
        WriteNAME(writer,animationData.boneList);
        
        WriteBLNM(writer, null);
        WriteMASK(writer, null);
        WriteBLTK(writer);

        WriteBKTM(writer,numEvents);
        WriteBOOL(writer,numEvents);

        // these both work with the vectors
        WriteOPTR(writer, positionData,trackPosScalarData);
        WriteOVEC(writer,positionData,trackPosScalarData,positionTimeData);

        // these all work with the quaternions
        WriteORTR(writer, rotationData);
        WriteARKT(writer, rotationTimeData);
        WriteOQUA(writer, rotationData);
        GladiusFileWriter.WriteEND(writer);
    }
    
    
    
    public static void WriteHEDR(BinaryWriter writer)
    {
        int total = GladiusFileWriter.HeaderSize+8;
        GladiusFileWriter.WriteASCIIString(writer, "HEDR");
        writer.Write(total);
        writer.Write(1); // num materials, 1 for now
        writer.Write(0);
        writer.Write(0);
        writer.Write(0);
    }

    public static void WriteNAME(BinaryWriter writer, List<string> boneNames)
    {
        int total = GladiusFileWriter.HeaderSize;
        int runningTotal = 0;
        foreach (string s in boneNames)
        {
            runningTotal += s.Length;
            runningTotal += 1;
        }

        runningTotal = GladiusFileWriter.GetPadValue(runningTotal,8); 
        
        total += runningTotal;
        
        GladiusFileWriter.WriteASCIIString(writer, "NAME");
        writer.Write(total);
        writer.Write(0); 
        writer.Write(1);
        GladiusFileWriter.WriteStringList(writer, boneNames, runningTotal);
    }

    // this is a list of animation event names.e.g. : detachShield detachWeapon1 detachWeapon2 footStepL footStepR hideShield hideWeapon1 hideWeapon2 hit react show
    public static void WriteBLNM(BinaryWriter writer, List<string> events)
    {
        List<string> eventNames = new List<string>();
        eventNames.Add("detachShield");
        eventNames.Add("detachWeapon1");
        eventNames.Add("detachWeapon2");
        eventNames.Add("footStepL");
        eventNames.Add("footStepR");
        eventNames.Add("hideShield");
        eventNames.Add("hideWeapon1");
        eventNames.Add("hideWeapon2");
        eventNames.Add("hit");
        eventNames.Add("react");
        eventNames.Add("show");
        
        int total = GladiusFileWriter.HeaderSize;
        int runningTotal = 0;
        foreach (string s in eventNames)
        {
            runningTotal += s.Length;
            runningTotal += 1;
        }

        
        
        total += runningTotal;

        total = GladiusFileWriter.GetPadValue(total);

        
        GladiusFileWriter.WriteASCIIString(writer, "BLNM");
        writer.Write(total);
        writer.Write(0);
        writer.Write(1);
        //writer.Write(eventNames.Count);
        GladiusFileWriter.WriteStringList(writer, eventNames, total-GladiusFileWriter.HeaderSize);
        
    }

    public static void WriteMASK(BinaryWriter writer, List<string> boneNames)
    {
        int numItems = 188;        
        GladiusFileWriter.WriteASCIIString(writer, "MASK");
        writer.Write(GladiusFileWriter.HeaderSize+numItems);
        writer.Write(1); // num materials, 1 for now
        writer.Write(numItems);
        for (int i = 0; i < numItems; ++i)
        {
            writer.Write((byte)0);
        }
    }


    public static void WriteBLTK(BinaryWriter writer)
    {
        int numItems = 1;
        int itemSize = 12;
        
        GladiusFileWriter.WriteASCIIString(writer, "BLTK");
        writer.Write(GladiusFileWriter.HeaderSize+(numItems*itemSize));
        writer.Write(0);
        writer.Write(numItems);
        GladiusFileWriter.WriteNull(writer, (numItems * itemSize));
    }


    // write timestamps.
    public static void WriteBKTM(BinaryWriter writer, int numEvents)
    {
        int total = GladiusFileWriter.HeaderSize + (numEvents * 4);

        GladiusFileWriter.WriteASCIIString(writer, "BKTM");
        writer.Write(total);
        writer.Write(0);
        writer.Write(numEvents);

        for (int i = 0; i < numEvents; ++i)
        {
            writer.Write((float)i * FrameTime);
        }
    }

    public static void WriteBOOL(BinaryWriter writer, int numEvents)
    {
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

    public static void WritePOSI(BinaryWriter writer, List<IndexedVector3> points)
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

    public static void WriteOPTR(BinaryWriter writer, List<List<Vector3>> positionData,List<Vector3> trackScalarData)
    {
        int total = GladiusFileWriter.HeaderSize;
        int paddedTotal = GladiusFileWriter.GetPadValue(total);

        GladiusFileWriter.WriteASCIIString(writer, "OPTR");
        writer.Write(paddedTotal); // block size
        writer.Write(1);
        writer.Write(positionData.Count); // number of elements.

        int count = 0;
        foreach (List<Vector3> positions in positionData)
        {
            
            // calc scalar.
            Vector3 scalar = Vector3.one;
            foreach (Vector3 v in positions)
            {
                scalar.x = Math.Min(scalar.x, Math.Abs(v.x));
                scalar.y = Math.Min(scalar.y, Math.Abs(v.y));
                scalar.z = Math.Min(scalar.z, Math.Abs(v.z));
            }

            scalar.x /= PosScalar.x;
            scalar.y /= PosScalar.y;
            scalar.z /= PosScalar.z;
            
            anim_OptPosTrack.ToStream(writer, positions.Count, scalar, 0);
        }
    }

    public static void WriteORTR(BinaryWriter writer, List<List<Quaternion>> rotationData)
    {
        int total = GladiusFileWriter.HeaderSize;
        int paddedTotal = GladiusFileWriter.GetPadValue(total);

        GladiusFileWriter.WriteASCIIString(writer, "ORTR");
        writer.Write(paddedTotal); // block size
        writer.Write(1);
        writer.Write(rotationData.Count); // number of elements.

        foreach (List<Quaternion> rotations in rotationData)
        {
            writer.Write(rotations.Count);
            writer.Write(0);
            writer.Write(0);

            // mNumKeys = binReader.ReadUInt32();
            // uint dummyPointer = binReader.ReadUInt32();
            // uint keyTimesPointer = binReader.ReadUInt32();
        }
    }

    public static void WriteOVEC(BinaryWriter writer, List<List<Vector3>> positionData,List<Vector3> trackScalarData,List<List<ushort>> timeData)
    {
        int total = GladiusFileWriter.HeaderSize;
        int paddedTotal = GladiusFileWriter.GetPadValue(total);

        int totalElements = 0;
        foreach (var track in positionData)
        {
            totalElements += track.Count;
        }

        GladiusFileWriter.WriteASCIIString(writer, "OVEC");
        writer.Write(paddedTotal); // block size
        writer.Write(0);
        writer.Write(total); // number of elements.



        for (int i = 0; i < positionData.Count; ++i)
        {
            for (int j = 0; j < positionData[i].Count; ++j)
            {
                optVec.ToStream(writer, optVec.Put(positionData[i][j], timeData[i][j],trackScalarData[i]));                
            }
        }
    }


    public static void WriteOQUA(BinaryWriter writer, List<List<Quaternion>> rotationData)
    {
        int total = GladiusFileWriter.HeaderSize;
        
        
        int paddedTotal = GladiusFileWriter.GetPadValue(total);

        int runningTotal = 0;
        foreach (var list in rotationData)
        {
            runningTotal += list.Count;
        }

        total += runningTotal;
        
        
        GladiusFileWriter.WriteASCIIString(writer, "OQUA");
        writer.Write(paddedTotal); // block size
        writer.Write(1);
        writer.Write(runningTotal);


        foreach (var list in rotationData)
        {
            foreach (Quaternion q in list)
            {
                // time will be set in arkt
                optQuat.ToStream(writer, optQuat.Put(q, 0f));
            }
        }
    }

    public static void WriteARKT(BinaryWriter writer, List<List<ushort>> rotationTimeData)
    {
        int total = GladiusFileWriter.HeaderSize;

        int runningTotal = 0;
        foreach (var list in rotationTimeData)
        {
            runningTotal += list.Count;
        }
        
        total += (rotationTimeData.Count * 2);
        
        int paddedTotal = total + runningTotal + 2;

        GladiusFileWriter.WriteASCIIString(writer, "ARKT");
        writer.Write(paddedTotal); // block size
        writer.Write(0);
        writer.Write(runningTotal);

        foreach (var list in rotationTimeData)
        {
            foreach (ushort t in list)
            {
                writer.Write(t);
            }
        }

        GladiusFileWriter.WriteNull(writer,2);

    }
}

