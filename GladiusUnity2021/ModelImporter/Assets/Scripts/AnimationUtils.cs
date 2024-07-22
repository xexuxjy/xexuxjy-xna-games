using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public static class AnimationUtils
{
    public const float FrameRate = 30;
    public const float FrameTime = 1f / FrameRate;


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
        WriteOPTR(writer, gac.PositionData);
        GladiusFileWriter.PadIfNeeded(writer);
        WriteOVEC(writer,gac.PositionData);
        GladiusFileWriter.PadIfNeeded(writer);

        // these all work with the quaternions
        WriteORTR(writer, gac.RotationData);
        GladiusFileWriter.PadIfNeeded(writer);
        WriteARKT(writer, optQuatList);
        GladiusFileWriter.PadIfNeeded(writer);
        WriteOQUA(writer, optQuatList);
        GladiusFileWriter.PadIfNeeded(writer);
        GladiusFileWriter.WriteEND(writer);
    }

    public static void WriteData(BinaryWriter writer, GladiusSimpleAnim simpleAnim)
    {
        AnimationData animationData = simpleAnim.m_allAnimations[0];
        int numEvents = animationData.boolTrack.mNumKeys;

        List<List<Vector3>> positionData = new List<List<Vector3>>();

        for (int i = 0; i < animationData.optPosTrack.m_tracks.Count; ++i)
        {
            List<Vector3> data = new List<Vector3>();
            positionData.Add(data);
            foreach (var optVec in animationData.optPosTrack.m_tracks[i].mOptVecs)
            {
                Vector3 dst = Vector3.zero; 
                Vector3 scalar = Vector3.one;
                optVec.Get(ref dst, ref scalar);
                data.Add(dst);
            }
        }

        
        List<List<Quaternion>> rotationData = new List<List<Quaternion>>();
        for (int i = 0; i < animationData.optRotTrack.m_tracks.Count; ++i)
        {
            List<Quaternion> data = new List<Quaternion>();
            rotationData.Add(data);
            foreach (var optQuat in animationData.optRotTrack.m_tracks[i].mOptQuats)
            {
                Quaternion q = Quaternion.identity;
                optQuat.Get(ref q);
                data.Add(q);
            }
        }

        
        List<optQuat> optQuatList = new List<optQuat>();

        // for (int i = 0; i < animationData.optRotTrack.m_tracks.Count; ++i)
        // {
        //     // List<Quaternion> data = new List<Quaternion>();
        //     // rotationData.Add(data);
        //     //foreach (var optQuat in animationData.optRotTrack.m_tracks[i].mOptQuats)
        //     for(int j=0;j<animationData.optRotTrack.m_tracks[i].mOptQuats.Count;++j)
        //     {
        //         optQuat oq = animationData.optRotTrack.m_tracks[i].mOptQuats[j];
        //         oq.time = animationData.optRotTrack.m_tracks[i].mKeyTimes[j];
        //         
        //         //optQuat.Get(ref q);
        //         //data.Add(q);
        //
        //         optQuatList.Add(oq);
        //
        //     }
        // }

        
        foreach(List<Quaternion> trackRotations in rotationData)
        {
            for (int j = 0; j < trackRotations.Count; ++j)
            {
                optQuat oq = optQuat.Put(trackRotations[j], 0.0f);
                optQuatList.Add(oq);
            }
        }


        GladiusFileWriter.WriteVERS(writer);
        GladiusFileWriter.PadIfNeeded(writer);
        GladiusFileWriter.WriteCPRT(writer);
        GladiusFileWriter.PadIfNeeded(writer);
        WriteHEDR(writer);
            WriteNAME(writer,animationData.boneList);
        GladiusFileWriter.PadIfNeeded(writer);
        
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
        WriteOPTR(writer, positionData);
        GladiusFileWriter.PadIfNeeded(writer);
        WriteOVEC(writer,positionData);
        GladiusFileWriter.PadIfNeeded(writer);

        // these all work with the quaternions
        WriteORTR(writer, rotationData);
        GladiusFileWriter.PadIfNeeded(writer);
        WriteARKT(writer, optQuatList);
        GladiusFileWriter.PadIfNeeded(writer);
        WriteOQUA(writer, optQuatList);
        GladiusFileWriter.PadIfNeeded(writer);
        GladiusFileWriter.WriteEND(writer);
    }
    
    
    
    public static void WriteHEDR(BinaryWriter writer)
    {
        int total = GladiusFileWriter.HeaderSize;
        GladiusFileWriter.WriteASCIIString(writer, "HEDR");
        writer.Write(0);
        writer.Write(1); // num materials, 1 for now
        writer.Write(1);
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

        total += runningTotal;
        
        GladiusFileWriter.WriteASCIIString(writer, "NAME");
        writer.Write(total);
        writer.Write(1); // num materials, 1 for now
        writer.Write(boneNames.Count);
        GladiusFileWriter.WriteStringList(writer, boneNames, runningTotal);
    }

    // this is a list of animation event names.e.g. : detachShield detachWeapon1 detachWeapon2 footStepL footStepR hideShield hideWeapon1 hideWeapon2 hit react show
    public static void WriteBLNM(BinaryWriter writer, List<string> events)
    {
    }

    public static void WriteMASK(BinaryWriter writer, List<string> boneNames)
    {
    }


    public static void WriteBLTK(BinaryWriter writer)
    {
    }


    // write timestamps.
    public static void WriteBKTM(BinaryWriter writer, int numEvents)
    {
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

    public static void WriteOPTR(BinaryWriter writer, List<List<Vector3>> positionData)
    {
        int total = GladiusFileWriter.HeaderSize;
        int paddedTotal = GladiusFileWriter.GetPadValue(total);

        GladiusFileWriter.WriteASCIIString(writer, "OPTR");
        writer.Write(paddedTotal); // block size
        writer.Write(1);
        writer.Write(positionData.Count); // number of elements.

        foreach (List<Vector3> positions in positionData)
        {
            anim_OptPosTrack.ToStream(writer, positions.Count, Vector3.one, 0);
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

    public static void WriteOVEC(BinaryWriter writer, List<List<Vector3>> positionData)
    {
        int total = GladiusFileWriter.HeaderSize;
        int paddedTotal = GladiusFileWriter.GetPadValue(total);

        GladiusFileWriter.WriteASCIIString(writer, "OVEC");
        writer.Write(paddedTotal); // block size
        writer.Write(1);
        writer.Write(1); // number of elements.


        int totalElements = 0;
        foreach (var track in positionData)
        {
            totalElements += track.Count;
        }

        foreach (var track in positionData)
        {
            foreach (Vector3 v in track)
            {
                optVec.ToStream(writer, optVec.Put(v, 0,Vector3.one));
            }
        }
    }


    public static void WriteOQUA(BinaryWriter writer, List<optQuat> optQuatRotationData)
    {
        int total = GladiusFileWriter.HeaderSize;
        int paddedTotal = GladiusFileWriter.GetPadValue(total);
        int totalElements = optQuatRotationData.Count;

        GladiusFileWriter.WriteASCIIString(writer, "OQUA");
        writer.Write(paddedTotal); // block size
        writer.Write(1);
        writer.Write(totalElements); 

        foreach (optQuat oq in optQuatRotationData)
        {
            //optQuat.ToStream(writer, optQuat.Put(Vector3.one, q));
            optQuat.ToStream(writer, oq);
        }
    }

    public static void WriteARKT(BinaryWriter writer, List<optQuat> optQuatRotationData)
    {
        int total = GladiusFileWriter.HeaderSize;
        int paddedTotal = GladiusFileWriter.GetPadValue(total);
        int totalElements = optQuatRotationData.Count;

        GladiusFileWriter.WriteASCIIString(writer, "ARKT");
        writer.Write(paddedTotal); // block size
        writer.Write(1);
        writer.Write(totalElements); 

        foreach (optQuat  oq in optQuatRotationData)
        {
            writer.Write(oq.time);
        }
    }
}

