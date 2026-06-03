using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public static class AnimationUtils
{
    public const float FrameRate = 30;
    public const float FrameTime = 1f / FrameRate;

    
    public const float ScalarItem = 1.0f / 32767.0f;

    public static Vector3 PosScalar = new Vector3(ScalarItem, ScalarItem, ScalarItem);


    public static void BuildSkeleton(Transform t, BoneNode parent,ref byte boneId,List<BoneNode> boneNodes)
    {
        if (t != null)
        {
            BoneNode boneNode = new BoneNode();
            boneNodes.Add(boneNode);
            
            boneNode.name = t.name;
            boneNode.Index = boneId;
            boneNode.offset = t.localPosition;
            boneNode.rotation = t.localRotation;
            
            boneId++;

            if (boneId > 250)
            {
                Debug.LogError($"Skeleton potentially has too many bones");
            }
            
            if (parent != null)
            {
                boneNode.ParentIndex = parent.Index;
            }

            foreach (Transform child in t)
            {
                BuildSkeleton(child,boneNode,ref boneId,boneNodes);
            }
        }
        
    }
    
    
    public static void WriteDataAsPAN(BinaryWriter writer, Transform rootBone,List<Transform> transformList, List<float> timeData,Dictionary<Transform, List<(float,Vector3)>> positionData,Dictionary<Transform, List<(float,Quaternion)>> rotationData)
    {
        // build a skeleton from rootBone.
        
        List<BoneNode> boneNodes = new List<BoneNode>();
        byte boneId = 0;
        BuildSkeleton(rootBone,null,ref boneId,boneNodes);
        
        List<string> boneNames = new List<string>();
        foreach (BoneNode boneNode in boneNodes)
        {
            string originalName = boneNode.name;
            if (boneNode.name.Contains("--"))
            {
                originalName = boneNode.name.Substring(0, boneNode.name.IndexOf("--"));
            }

            boneNames.Add(originalName);
        }
        
        List<ushort> timeDataUShort = new List<ushort>();
        

        GladiusFileWriter.WriteVERS(writer);
        //GladiusFileWriter.PadIfNeeded(writer);
        GladiusFileWriter.WriteCPRT(writer);
        //GladiusFileWriter.PadIfNeeded(writer);
        WriteHEDR(writer,timeData.Last());
        //GladiusFileWriter.PadIfNeeded(writer);
        WriteNAME(writer, boneNames);

        List<string> eventsString = new List<string>();
        WriteBLNM(writer, eventsString);
        //GladiusFileWriter.PadIfNeeded(writer);
        WriteMASK(writer, boneNodes);
        //GladiusFileWriter.PadIfNeeded(writer);
        WriteBLTK(writer);
        //GladiusFileWriter.PadIfNeeded(writer);
        WriteBKTM(writer,timeData);
        //GladiusFileWriter.PadIfNeeded(writer);
        WriteBOOL(writer,timeData);
        //GladiusFileWriter.PadIfNeeded(writer);

        // these both work with the vectors
        WriteOPTR(writer, transformList,timeData,positionData);
        //GladiusFileWriter.PadIfNeeded(writer);
        WriteOVEC(writer,transformList,timeData,positionData);
        //GladiusFileWriter.PadIfNeeded(writer);

        // these all work with the quaternions
        WriteORTR(writer, transformList,timeData,rotationData);
        //GladiusFileWriter.PadIfNeeded(writer);
        WriteARKT(writer, transformList,timeData,rotationData);
        //GladiusFileWriter.PadIfNeeded(writer);
        WriteOQUA(writer, transformList,timeData,rotationData);
        //GladiusFileWriter.PadIfNeeded(writer);
        GladiusFileWriter.WriteEND(writer);
    }

    // public static void WriteData(BinaryWriter writer, GladiusSimpleAnim simpleAnim)
    // {
    //     AnimationData animationData = simpleAnim.m_allAnimations[0];
    //     int numEvents = animationData.boolTrack.mNumKeys;
    //
    //     List<List<Vector3>> positionData = new List<List<Vector3>>();
    //     List<List<ushort>> positionTimeData = new List<List<ushort>>();
    //
    //     for (int i = 0; i < animationData.optPosTrack.m_tracks.Count; ++i)
    //     {
    //         List<Vector3> data = new List<Vector3>();
    //         List<ushort> timeData = new List<ushort>();
    //         
    //         positionData.Add(data);
    //         positionTimeData.Add(timeData);
    //         
    //         foreach (var optVec in animationData.optPosTrack.m_tracks[i].mOptVecs)
    //         {
    //             Vector3 dst = Vector3.zero; 
    //
    //             optVec.Get(ref dst, ref PosScalar);
    //             timeData.Add(optVec.time);
    //             data.Add(dst);
    //         }
    //     }
    //
    //     
    //     List<List<Quaternion>> rotationData = new List<List<Quaternion>>();
    //     List<List<ushort>> rotationTimeData = new List<List<ushort>>();
    //     for (int i = 0; i < animationData.optRotTrack.m_tracks.Count; ++i)
    //     {
    //         List<Quaternion> data = new List<Quaternion>();
    //         List<ushort> timeData = new List<ushort>();
    //
    //         rotationData.Add(data);
    //         rotationTimeData.Add(timeData);
    //         
    //         int count = 0;
    //         foreach (var optQuat in animationData.optRotTrack.m_tracks[i].mOptQuats)
    //         {
    //             Quaternion q = Quaternion.identity;
    //             optQuat.Get(ref q);
    //             data.Add(q);
    //             timeData.Add(animationData.optRotTrack.m_tracks[i].mKeyTimes[count++]);
    //
    //         }
    //     }
    //
    //
    //     
    //
    //     GladiusFileWriter.WriteVERS(writer);
    //     GladiusFileWriter.WriteCPRT(writer);
    //     WriteHEDR(writer);
    //     WriteNAME(writer,animationData.boneList);
    //     
    //     WriteBLNM(writer, null);
    //     WriteMASK(writer, null);
    //     WriteBLTK(writer);
    //
    //     WriteBKTM(writer,numEvents);
    //     WriteBOOL(writer,numEvents);
    //
    //     // these both work with the vectors
    //     WriteOPTR(writer, positionData);
    //     WriteOVEC(writer,positionData,positionTimeData);
    //
    //     // these all work with the quaternions
    //     WriteORTR(writer, rotationData);
    //     WriteARKT(writer, rotationTimeData);
    //     WriteOQUA(writer, rotationData);
    //     GladiusFileWriter.WriteEND(writer);
    // }
    
    
    
    /*
     *       if (fileChunk.ChunkName.SequenceEqual(AnimationLoader.hedrTag))
        {
            Int16 jointCount = reader.ReadInt16();
            Int16 temp = reader.ReadInt16();
            animationData.mTranslation = ((float)temp * (1.0f / 1024.0f));
            animationData.mLength = reader.ReadSingle();
            handled = true;
        }
     */
    public static void WriteHEDR(BinaryWriter writer,float animationLength)
    {
        int total = GladiusFileWriter.HeaderSize+8;
        GladiusFileWriter.WriteASCIIString(writer, "HEDR");
        writer.Write(total);
        writer.Write(1); // size 2 
        writer.Write(0);
        writer.Write(0); // joint count
        writer.Write(animationLength); // Animation length
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

    // for each bone, we need to write what sort of animation track is associated with it (for pos and rot)
    // for now everything standard pos and rot
    public static void WriteMASK(BinaryWriter writer, List<BoneNode> boneList)
    {
        int numItems = boneList.Count;        
        GladiusFileWriter.WriteASCIIString(writer, "MASK");
        writer.Write(GladiusFileWriter.HeaderSize+numItems);
        writer.Write(1); 
        writer.Write(numItems);
        byte mask = AnimationData.pan_FilecPos | AnimationData.pan_FilecRot;
        for (int i = 0; i < numItems; ++i)
        {
            writer.Write(mask);
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
    public static void WriteBKTM(BinaryWriter writer, List<float> timeData)
    {
        int total = GladiusFileWriter.HeaderSize + (timeData.Count * 4);

        GladiusFileWriter.WriteASCIIString(writer, "BKTM");
        writer.Write(total);
        writer.Write(0);
        writer.Write(timeData.Count);

        foreach (float t in timeData)
        {
            writer.Write(t);
        }
    }

    public static void WriteBOOL(BinaryWriter writer, List<float> timeData)
    {
        int total = GladiusFileWriter.HeaderSize + (timeData.Count * 4);

        GladiusFileWriter.WriteASCIIString(writer, "BOOL");
        writer.Write(total);
        writer.Write(0);
        writer.Write(timeData.Count);

        for (int i = 0; i < timeData.Count; ++i)
        {
            writer.Write(0);
        }
    }

    public static void WritePOSI(BinaryWriter writer, List<IndexedVector3> points)
    {
        int total = GladiusFileWriter.HeaderSize;
        total += (points.Count * 12);
        int paddedTotal = GladiusFileWriter.GetPadValue(total);
        int numPadBytes = paddedTotal - total; 


        GladiusFileWriter.WriteASCIIString(writer, "POSI");
        writer.Write(paddedTotal); // block size
        writer.Write(1);
        writer.Write(points.Count); // number of elements.


        foreach (IndexedVector3 v in points)
        {
            Common.WriteVector3BE(writer, v);
        }

        GladiusFileWriter.WriteNull(writer, numPadBytes);
    }

    public static void WriteOPTR(BinaryWriter writer,List<Transform> transformList, List<float> timeData,Dictionary<Transform, List<(float,Vector3)>> positionData)
    {
        long startPos = writer.BaseStream.Position;
        int total = GladiusFileWriter.HeaderSize;
        total += transformList.Count * anim_OptPosTrack.Size;

        int paddedTotal = GladiusFileWriter.GetPadValue(total);
        int numPadBytes = paddedTotal - total; 

        GladiusFileWriter.WriteASCIIString(writer, "OPTR");
        writer.Write(paddedTotal); // block size
        writer.Write(1);
        writer.Write(positionData.Count); // number of elements.

        foreach (Transform t in transformList)
        {
            anim_OptPosTrack.ToStream(writer, positionData[t].Count, PosScalar, 0);
        }
        long endPos = writer.BaseStream.Position;
        long diff =  endPos - startPos;
        GladiusFileWriter.WriteNull(writer,numPadBytes);
    }

    public static void WriteORTR(BinaryWriter writer, List<Transform> transformList, List<float> timeData,Dictionary<Transform, List<(float,Quaternion)>> rotationData)
    {
        int total = GladiusFileWriter.HeaderSize;
        total += transformList.Count * anim_OptRotTrack.Size;
        
        int paddedTotal = GladiusFileWriter.GetPadValue(total);
        int numPadBytes = paddedTotal - total; 


        GladiusFileWriter.WriteASCIIString(writer, "ORTR");
        writer.Write(paddedTotal); // block size
        writer.Write(1);
        writer.Write(rotationData.Count); // number of elements.

        foreach (Transform t in transformList)

        {
            writer.Write(rotationData[t].Count);
            writer.Write(0);
            writer.Write(0);

            // mNumKeys = binReader.ReadUInt32();
            // uint dummyPointer = binReader.ReadUInt32();
            // uint keyTimesPointer = binReader.ReadUInt32();
        }
        GladiusFileWriter.WriteNull(writer,numPadBytes);
        
    }

    public static ushort FloatTimeToUShort(float time)
    {
        //return (ushort)(time / FrameRate);
        return (ushort)(time / FrameTime);
        
    }
    
    
    public static void WriteOVEC(BinaryWriter writer, List<Transform> transformList, List<float> timeData,Dictionary<Transform, List<(float,Vector3)>> positionData)
    {
        int total = GladiusFileWriter.HeaderSize;

        int totalElements = 0;
        foreach (var track in positionData.Values)
        {
            totalElements += track.Count;
        }

        total += (totalElements * optVec.Size);
        
        int paddedTotal = GladiusFileWriter.GetPadValue(total);
        int numPadBytes = paddedTotal - total; 


        writer.Write(AnimationLoader.ovecTag);
        writer.Write(paddedTotal); // block size
        writer.Write(0);
        writer.Write(totalElements); // number of elements.

        for (int i = 0; i < transformList.Count; ++i)
        {
            List<(float,Vector3)> positionList = positionData[transformList[i]];
            for (int j = 0; j < positionList.Count; ++j)
            {
                optVec.ToStream(writer, optVec.Put(positionList[j].Item2, FloatTimeToUShort(positionList[j].Item1),PosScalar));                
            }
        }
        GladiusFileWriter.WriteNull(writer, numPadBytes);
    }


    public static void WriteOQUA(BinaryWriter writer, List<Transform> transformList, List<float> timeData,Dictionary<Transform, List<(float,Quaternion)>> rotationData)
    {
        int total = GladiusFileWriter.HeaderSize;
        int totalElements = 0;
        
        foreach (var track in rotationData.Values)
        {
            totalElements += track.Count;
        }

        total += (totalElements * optQuat.Size);
        
        int paddedTotal = GladiusFileWriter.GetPadValue(total);
        int numPadBytes = paddedTotal - total; 

        
        writer.Write(AnimationLoader.oquaTag);
        writer.Write(paddedTotal); // block size
        writer.Write(1);
        writer.Write(totalElements);

        for (int i = 0; i < transformList.Count; ++i)
        {
            List<(float,Quaternion)> rotationList = rotationData[transformList[i]];
            for (int j = 0; j < rotationList.Count; ++j)
            {
                // time will be set in arkt
                optQuat.ToStream(writer, optQuat.Put(rotationList[j].Item2, rotationList[j].Item1));
            }
        }
        GladiusFileWriter.WriteNull(writer, numPadBytes);
    }

    /*
     *             foreach (anim_OptRotTrack track in animationData.optRotTrack.m_tracks)
            {
                for (int i = 0; i < track.mNumKeys; ++i)
                {
                    ushort key = reader.ReadUInt16();
                    // optQuat oq = track.mOptQuats[i];
                    // oq.time = key;
                    // track.mOptQuats[i] = oq;
                    track.mKeyTimes.Add(key);
                }
            }

     */
    public static void WriteARKT(BinaryWriter writer, List<Transform> transformList, List<float> timeData,Dictionary<Transform, List<(float,Quaternion)>> rotationData)
    {
        int total = GladiusFileWriter.HeaderSize;
        int totalElements = 0;
        foreach (var track in rotationData.Values)
        {
            totalElements += track.Count;
        }

        total += (totalElements * 2);
        
        int paddedTotal = GladiusFileWriter.GetPadValue(total);
        int numPadBytes = paddedTotal - total; 
        

        GladiusFileWriter.WriteASCIIString(writer, "ARKT");
        writer.Write(paddedTotal); // block size
        writer.Write(0);
        writer.Write(totalElements);
        
        for (int i = 0; i < transformList.Count; ++i)
        {
            List<(float,Quaternion)> rotationList = rotationData[transformList[i]];
            for (int j = 0; j < rotationList.Count; ++j)
            {
                writer.Write(FloatTimeToUShort(rotationList[j].Item1));
            }
        }

        GladiusFileWriter.WriteNull(writer,numPadBytes);

    }
}

