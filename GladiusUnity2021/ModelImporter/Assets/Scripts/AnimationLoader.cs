using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class AnimationLoader
{
    public static char[] pak1Tag = new char[] { 'P', 'A', 'K', '1' };
    public static char[] hedrTag = new char[] { 'H', 'E', 'D', 'R' };   // start of anim
    public static char[] versTag = new char[] { 'V', 'E', 'R', 'S' };   // version label
    public static char[] cprtTag = new char[] { 'C', 'P', 'R', 'T' };   // copyright label.
    public static char[] blnmTag = new char[] { 'B', 'L', 'N', 'M' }; // Animation Event Names. Same values are used for every animation in a given model. Similar(same) event names across similar models. null separated. no other info
    public static char[] maskTag = new char[] { 'M', 'A', 'S', 'K' };   // different number of values to other blocks. single byte. 0xc0,0x80,0x40 0x01,0x02,0x0,. non zero value in nearly all cases.
    public static char[] bltkTag = new char[] { 'B', 'L', 'T', 'K' };   // minimal data - all 28 bytes, mainly header info, with 1 (2) bytes changing value at 0x14
    public static char[] bktmTag = new char[] { 'B', 'K', 'T', 'M' };   // Timestamp events. 33ms gaps. number of events matches BKTM
    public static char[] boolTag = new char[] { 'B', 'O', 'O', 'L' };   // num entries match timestamp events. each entry is 4 bytes. mainly zero. some anims have zero in all values.
    public static char[] dcrtTag = new char[] { 'D', 'C', 'R', 'T' }; // blocks are at 8 bytes (75 * 8 = 600) , not really much info in them.
    public static char[] dcrdTag = new char[] { 'D', 'C', 'R', 'D' };
    public static char[] dcptTag = new char[] { 'D', 'C', 'P', 'T' };// 8 byte blocks. 3 bytes , then a common char (0x80,0xba) then 1 byte, and 3 zero bytes?
    public static char[] dcpdTag = new char[] { 'D', 'C', 'P', 'D' };
    public static char[] optrTag = new char[] { 'O', 'P', 'T', 'R' };
    public static char[] ortrTag = new char[] { 'O', 'R', 'T', 'R' };
    public static char[] ovecTag = new char[] { 'O', 'V', 'E', 'C' };
    public static char[] oquaTag = new char[] { 'O', 'Q', 'U', 'A' };
    public static char[] arktTag = new char[] { 'A', 'R', 'K', 'T' };

    public static char[] endTag = new char[] { 'E', 'N', 'D','.' }; // end of anim

    public static char[] nameTag = new char[] { 'N', 'A', 'M', 'E' }; // names of the various bones, null separated.

    //public static void ReadPak1File(BinaryReader binReader, GladiusCharacterAnim gladiusAnim)
    //{
    //    Common.FindCharsInStream(binReader, pak1Tag);
    //    int numAnimations = binReader.ReadInt32();
    //    int animNameStart = binReader.ReadInt32();
    //    int dataStart = binReader.ReadInt32();

    //    binReader.BaseStream.Position = animNameStart;
    //    List<String> animNames = new List<string>();
    //    Common.ReadNullSeparatedNamesWithCount(binReader, numAnimations, animNames);

    //    binReader.BaseStream.Position = dataStart;

    //    for (int i = 0; i < numAnimations; ++i)
    //    {
    //        AnimationData animationData;
    //        if (AnimationLoader.FromStream(binReader, gladiusAnim, out animationData))
    //        {
    //            animationData.name = StandardiseAnimName(animNames[i]);

    //            // need moverun for overland?
    //            if (animationData.name.Contains("idle") || animationData.name.Contains("moverun"))
    //            {
    //                animationData.mFlags |= AnimationData.ANIM_LOOP;
    //            }
    //            //animations.Add(animationData);
    //            gladiusAnim.AddAnimationData(animationData);
    //        }
    //        //break;
    //    }
    //}

    public static string StandardiseAnimName(string input)
    {
        String result = input;
        if (input != null)
        {
            if (input.Contains("_") && input.Contains(".pan"))
            {

                int startIndex = input.IndexOf('_') + 1;
                int length = input.Length - startIndex - (".pan".Length);
                result = input.Substring(startIndex, length);
            }
            result = result.ToLower();
        }
        return result;
    }

    public static AnimationData ReadSingleAnimationFile(String fileName, GladiusSimpleAnim gladiusAnim, BinaryReader binReader)
    {
        AnimationData animationData;

        if (AnimationLoader.FromStream(binReader, gladiusAnim, out animationData))
        {
            animationData.name = StandardiseAnimName(fileName.ToLower());
        }
        return animationData;
    }




    public static bool HandleChunk(FileChunk fileChunk, AnimationData animationData, BinaryReader binReader)
    {
        bool handled = false;
        if (fileChunk.ChunkName.SequenceEqual(AnimationLoader.hedrTag))
        {
            Int16 jointCount = binReader.ReadInt16();
            Int16 temp = binReader.ReadInt16();
            animationData.mTranslation = ((float)temp * (1.0f / 1024.0f));
            animationData.mLength = binReader.ReadSingle();
            handled = true;
        }
        else if (fileChunk.ChunkName.SequenceEqual(AnimationLoader.nameTag))
        {
            Common.ReadNullSeparatedNamesInSectionLength(binReader, animationData.boneList,fileChunk.ChunkSize-FileChunk.ChunkHeaderSize);
            handled = true;
        }
        else if (fileChunk.ChunkName.SequenceEqual(AnimationLoader.blnmTag))
        {
            Common.ReadNullSeparatedNamesInSectionLength(binReader, animationData.eventList,fileChunk.ChunkSize - FileChunk.ChunkHeaderSize);
            handled = true;
        }
        else if (fileChunk.ChunkName.SequenceEqual(AnimationLoader.maskTag))
        {
            animationData.maskTrack = new MaskTrack(animationData);
            animationData.maskTrack.mNumKeys = fileChunk.ChunkElements;
            animationData.maskTrack.Process(binReader);
            handled = true;
        }
        else if (fileChunk.ChunkName.SequenceEqual(AnimationLoader.bktmTag))
        {
            for (int i = 0; i < fileChunk.ChunkElements; ++i)
            {
                animationData.timeStepList.Add(binReader.ReadSingle());
            }
            handled = true;
        }
        else if (fileChunk.ChunkName.SequenceEqual(AnimationLoader.boolTag))
        {
            animationData.boolTrack = new BoolTrack(animationData);
            animationData.boolTrack.Process(binReader,fileChunk.ChunkElements);
            handled = true;
        }
        else if (fileChunk.ChunkName.SequenceEqual(AnimationLoader.dcptTag))
        {
            animationData.posXTrack = new PosXTrack(animationData);
            animationData.posXTrack.Process(binReader,fileChunk.ChunkElements);
            handled = true;
        }
        else if (fileChunk.ChunkName.SequenceEqual(AnimationLoader.dcrtTag))
        {
            animationData.rotXTrack = new RotXTrack(animationData);
            animationData.rotXTrack.Process(binReader, fileChunk.ChunkElements);
            handled = true;
        }
        else if (fileChunk.ChunkName.SequenceEqual(AnimationLoader.optrTag))
        {
            animationData.optPosTrack = new OptPosTrack(animationData);
            animationData.optPosTrack.Process(binReader, fileChunk.ChunkElements);
            handled = true;
        }
        else if (fileChunk.ChunkName.SequenceEqual(AnimationLoader.ovecTag))
        {
            foreach (anim_OptPosTrack track in animationData.optPosTrack.m_tracks)
            {
                for (int i = 0; i < track.mNumKeys; ++i)
                {
                    optVec ov = optVec.FromStream(binReader);
                    track.mOptVecs.Add(ov);
                }
            }
            handled = true;
        }
        else if (fileChunk.ChunkName.SequenceEqual(AnimationLoader.ortrTag))
        {
            animationData.optRotTrack = new OptRotTrack(animationData);
            animationData.optRotTrack.Process(binReader,fileChunk.ChunkElements);
            handled = true;
        }
        else if (fileChunk.ChunkName.SequenceEqual(AnimationLoader.arktTag))
        {
            foreach (anim_OptRotTrack track in animationData.optRotTrack.m_tracks)
            {
                for (int i = 0; i < track.mNumKeys; ++i)
                {
                    ushort key = binReader.ReadUInt16();
                    track.mKeyTimes.Add(key);
                }
            }
            handled = true;
        }
        else if (fileChunk.ChunkName.SequenceEqual(AnimationLoader.oquaTag))
        {
            foreach (anim_OptRotTrack track in animationData.optRotTrack.m_tracks)
            {
                for (int i = 0; i < track.mNumKeys; ++i)
                {
                    optQuat oq = optQuat.FromStream(binReader);
                    track.mOptQuats.Add(oq);
                }
            }
            int ibreak = 0;
        }

        return handled;
    }



    public static bool FromStream(BinaryReader binReader, GladiusSimpleAnim gladiusAnim, out AnimationData animationData)
    {
        long startStreamPos = binReader.BaseStream.Position;

        animationData = new AnimationData();
        animationData.BuildPrecomputeGrid();
        animationData.gladiusAnim = gladiusAnim;

        FileChunk fileChunk = FileChunk.FromStream(binReader);
        int escape = 20;
        int escapeCount = 0;
        while(fileChunk != null  && (!fileChunk.ChunkName.SequenceEqual(AnimationLoader.endTag)))
        {
            
            //Debug.LogError("FileChunk : " + new string(fileChunk.ChunkName));
            String chunkName = new String(fileChunk.ChunkName);
            int currentPosition = (int)binReader.BaseStream.Position;

            bool handled = HandleChunk(fileChunk, animationData, binReader);
            binReader.BaseStream.Position = currentPosition + fileChunk.ChunkSize - FileChunk.ChunkHeaderSize;
            fileChunk = FileChunk.FromStream(binReader);
        }
        int ibreak = 0;

        return true;    
    }



}


/*----------------------------------------------------------------------------------------------------------*/

public class OptRotTrack : AnimTrack
{
    public OptRotTrack(AnimationData animationData) : base(animationData)
    {

    }

    public void Process(BinaryReader binReader,int numTracks)
    {
        for (int i = 0; i < numTracks; ++i)
        {
            anim_OptRotTrack rotTrack = new anim_OptRotTrack();
            rotTrack.Process(binReader);
            m_tracks.Add(rotTrack);
        }
    }

    public List<anim_OptRotTrack> m_tracks = new List<anim_OptRotTrack>();
}

public class anim_OptRotTrack
{
    public void Process(BinaryReader binReader)
    {
        mNumKeys = binReader.ReadUInt32();
        uint dummyPointer = binReader.ReadUInt32();
        uint keyTimesPointer = binReader.ReadUInt32();
    }

    public int boneId;
    public BoneAnimData boneAnimData;
    public String trackBoneName;
    public UInt32 mNumKeys;
    public List<optQuat> mOptQuats = new List<optQuat>();
    public List<ushort> mKeyTimes = new List<ushort>();

}



public class OptPosTrack : AnimTrack
{
    public OptPosTrack(AnimationData animationData) : base(animationData)
    {

    }

    public void Process(BinaryReader binReader,int numTracks)
    {
        for (int i = 0; i < numTracks; ++i)
        {
            anim_OptPosTrack posTrack = new anim_OptPosTrack();
            posTrack.Process(binReader);
            m_tracks.Add(posTrack);
        }
    }

    public List<anim_OptPosTrack> m_tracks = new List<anim_OptPosTrack>();
}

public class anim_OptPosTrack
{
    public void Process(BinaryReader binReader)
    {
        mNumKeys = binReader.ReadUInt32();
        //mPosScalar.x = binReader.ReadSingle();
        //mPosScalar.y = binReader.ReadSingle();
        //mPosScalar.z = binReader.ReadSingle();
        mPosScalar = Common.FromStreamVector3(binReader);
        GladiusGlobals.GladiusToUnity(ref mPosScalar);
        uint dummyPointer = binReader.ReadUInt32();
    }

    public int boneId;
    public BoneAnimData boneAnimData;
    public String trackBoneName;
    public UInt32 mNumKeys;
    public Vector3 mPosScalar = Vector3.one;
    public List<optVec> mOptVecs = new List<optVec>();
}


public struct optVec
{
    //	char		x, y;//, z, pad;
    public Int16 x, y, z;
    //	uint32		compact;
    public UInt16 time;

    public void Get(ref Vector3 dst, ref Vector3 _scalar)
    {
        dst.x = ((float)x) * (_scalar.x);
        dst.y = ((float)y) * (_scalar.y);
        dst.z = ((float)z) * (_scalar.z);

    }

    public static optVec Put(Vector3 v, Vector3 s)
    {

        return new optVec();
    }

    public static optVec FromStream(BinaryReader binReader)
    {
        optVec ov = new optVec();
        ov.x = binReader.ReadInt16();
        ov.y = binReader.ReadInt16();
        ov.z = binReader.ReadInt16();
        ov.time = binReader.ReadUInt16();
        return ov;
    }

    public static void ToStream(BinaryWriter binWriter, optVec ov)
    {
        binWriter.Write(ov.x);
        binWriter.Write(ov.y);
        binWriter.Write(ov.z);

    }
    
}



public struct optQuat
{
    // Touch these
    public const int QXBITS = 10;
    public const int QYBITS = 10;
    public const int QZBITS = 10;
    public const int QWBITS = 2;

    // Don't touch these
    public const int QXSHIFT = (QWBITS + QZBITS + QYBITS);
    public const int QYSHIFT = (QWBITS + QZBITS);
    public const int QZSHIFT = (QWBITS);
    public const int QWSHIFT = 0;

    public const int QXMASK = (((1 << QXBITS) - 1) << QXSHIFT);
    public const int QYMASK = (((1 << QYBITS) - 1) << QYSHIFT);
    public const int QZMASK = (((1 << QZBITS) - 1) << QZSHIFT);
    public const int QWMASK = (((1 << QWBITS) - 1) << QWSHIFT);

    public const int QXSCALE = ((1 << (QXBITS - 1)) - 1); // -1 for sign bit
    public const int QYSCALE = ((1 << (QYBITS - 1)) - 1);
    public const int QZSCALE = ((1 << (QZBITS - 1)) - 1);
    public const int QWSCALE = ((1 << (QWBITS - 1)) - 1);

    const float SCALAR_SIGNED8 = 127.0f;         // 8bits (7 + sign)
    const float SCALAR_SIGNED9 = 255.0f;         // 9bits (8 + sign)
    const float SCALAR_SIGNED10 = 511.0f;
    const float SCALAR_SIGNED11 = 1023.0f;
    const float SCALAR_SIGNED12 = 2047.0f;
    const float SCALAR_SIGNED16 = 32767.0f;


    public uint compact;
    public short x;
    public short y;
    public short z;
    public short w;
    public ushort time;
    public Quaternion quat;

    public void Get(ref Quaternion q)
    {
        q = quat;
    }

    public void Get2(ref Quaternion q)
    {
        // sign extend
        int comp = (int)compact;
        float f0 = (float)(comp >> QXSHIFT) * (1.0f / SCALAR_SIGNED10);
        float f1 = (float)((comp << QXBITS) >> (QYSHIFT + QXBITS)) * (1.0f / SCALAR_SIGNED10);
        float f2 = (float)((comp << (QXBITS + QYBITS)) >> (QZSHIFT + QXBITS + QYBITS)) * (1.0f / SCALAR_SIGNED10);

        float d0 = f0 * f0 + f1 * f1 + f2 * f2;
        float sq = (float)Math.Sqrt(Math.Max(0.0f, 1.0f - d0));

        int choice = comp & QWMASK;

        switch (comp & QWMASK)
        {
            case 0: q = new Quaternion(sq, f0, f1, f2); break;
            case 1: q = new Quaternion(f2, sq, f0, f1); break;
            case 2: q = new Quaternion(f1, f2, sq, f0); break;
            case 3: q = new Quaternion(f0, f1, f2, sq); break;
        }
    }

    public static optQuat FromStream(BinaryReader binReader)
    {
        optQuat ov = new optQuat();
        ov.compact = binReader.ReadUInt32();
        ov.Get2(ref ov.quat);
        //ov.x = binReader.ReadInt16();
        //ov.y = binReader.ReadInt16();
        //ov.z = binReader.ReadInt16();
        //ov.w = binReader.ReadInt16();
        //ov.time = binReader.ReadUInt16();
        return ov;
    }
    
    public static optQuat Put(Vector3 v, Quaternion q)
    {

        return new optQuat();
    }

    
    public static void ToStream(BinaryWriter binWriter, optQuat oq)
    {
        binWriter.Write(oq.x);
        binWriter.Write(oq.y);
        binWriter.Write(oq.z);
        binWriter.Write(oq.w);
    }

    
    
}




/*----------------------------------------------------------------------------------------------------------*/

public class MaskTrack : AnimTrack
{
    public MaskTrack(AnimationData ad)
        : base(ad)
    {
    }

    public void Process(BinaryReader binReader)
    {
        byte[] maskTable = binReader.ReadBytes(mNumKeys);
        for (int i = 0; i < mNumKeys; ++i)
        {
            mAnimationData.mNumRot += ((maskTable[i] & AnimationData.pan_FilecRot) != 0) ? 1 : 0;
            mAnimationData.mNumRawRot += ((maskTable[i] & AnimationData.pan_FilecRawRot) != 0) ? 1 : 0;
            mAnimationData.mNumPos += ((maskTable[i] & AnimationData.pan_FilecPos) != 0) ? 1 : 0;
            mAnimationData.mNumFloat += ((maskTable[i] & AnimationData.pan_FilecFloat) != 0) ? 1 : 0;
            mAnimationData.mNumInt += ((maskTable[i] & AnimationData.pan_FilecInt) != 0) ? 1 : 0;
            mAnimationData.mNumXPos += ((maskTable[i] & AnimationData.pan_FilecXPos) != 0) ? 1 : 0;
            mAnimationData.mNumXRot += ((maskTable[i] & AnimationData.pan_FilecXRot) != 0) ? 1 : 0;

            if (mAnimationData.mNumPos != 0 && mAnimationData.mNumXPos != 0)
            {
                Debug.Assert(false, "multiple pos tracks");
            }
            if ((mAnimationData.mNumRot != 0 && mAnimationData.mNumXRot != 0) || (mAnimationData.mNumRot != 0 && mAnimationData.mNumRawRot != 0) || ((mAnimationData.mNumRawRot != 0 && mAnimationData.mNumXRot != 0)))
            {
                //Debug.Assert(false, "multiple rot tracks");
            }
        }

        for (int i = 0; i < mAnimationData.boneList.Count; ++i)
        {
            string boneName = mAnimationData.boneList[i];
            if ((maskTable[i] & AnimationData.pan_FilecXPos) != 0)
            {
                mAnimationData.mXPosNameTable.Add(boneName);
            }
            if ((maskTable[i] & AnimationData.pan_FilecXRot) != 0)
            {
                mAnimationData.mXRotNameTable.Add(boneName);
            }
            if ((maskTable[i] & AnimationData.pan_FilecPos) != 0)
            {
                mAnimationData.mPosNameTable.Add(boneName);
            }
            if ((maskTable[i] & AnimationData.pan_FilecRot) != 0)
            {
                mAnimationData.mRotNameTable.Add(boneName);
            }

        }
        int ibreak = 0;

    }

}

/*----------------------------------------------------------------------------------------------------------*/

public class PosXTrack : AnimTrack
{
    public PosXTrack(AnimationData ad)
        : base(ad)
    {
    }

    public void Process(BinaryReader binReader,int numTracks)
    {
        // read track info...
        for (int i = 0; i < numTracks; ++i)
        {
            m_tracks.Add(anim_DCTTrack.FromStream(binReader));
        }


        if (Common.FindCharsInStream(binReader, AnimationLoader.dcpdTag))
        {
            int sectionLength = binReader.ReadInt32();
            int pad1 = binReader.ReadInt32();
            int numBones = binReader.ReadInt32();

            uint adjust = 0;

            for (int j = 0; j < mAnimationData.mNumXPos; ++j)
            {
                int len = (int)(adjust + m_tracks[j].mNumBytes);
                int alignedLen = len + (len % 4);
                m_tracks[j].mpData = new byte[len];
                m_tracks[j].alignmentAdjust = adjust;
                for (int z = 0; z < m_tracks[j].mNumBytes; ++z)
                {
                    m_tracks[j].mpData[adjust + z] = binReader.ReadByte();
                }

                //sm_tracks[j].mpData = binReader.ReadBytes(m_tracks[j].mNumBytes);
                adjust = (uint)m_tracks[j].mNumBytes % 4;

                if (!CanDealWithAnimLength(mAnimationData.mLength, m_tracks[j]))
                {
                    int ibreak = 0;
                }
                if (!CanDealWithByteSize(m_tracks[j].mNumBytes))
                {
                    int ibreak = 0;
                }
            }
        }

    }

    public List<anim_DCTTrack> m_tracks = new List<anim_DCTTrack>();
}

/*----------------------------------------------------------------------------------------------------------*/

public class RotXTrack : AnimTrack
{
    public RotXTrack(AnimationData ad)
        : base(ad)
    {
    }

    public void Process(BinaryReader binReader,int numTracks)
    {
        // read track info...
        for (int i = 0; i < numTracks; ++i)
        {
            m_tracks.Add(anim_DCTTrack.FromStream(binReader));
        }

        if (Common.FindCharsInStream(binReader, AnimationLoader.dcrdTag))
        {
            int sectionLength = binReader.ReadInt32();
            int pad1 = binReader.ReadInt32();
            int numBones = binReader.ReadInt32();

            uint adjust = 0;

            for (int j = 0; j < mAnimationData.mNumXRot; ++j)
            {
                int len = (int)(adjust + m_tracks[j].mNumBytes);
                int alignedLen = len + (len % 4);
                m_tracks[j].mpData = new byte[len];
                m_tracks[j].alignmentAdjust = adjust;
                for (int z = 0; z < m_tracks[j].mNumBytes; ++z)
                {
                    m_tracks[j].mpData[adjust + z] = binReader.ReadByte();
                }

                //sm_tracks[j].mpData = binReader.ReadBytes(m_tracks[j].mNumBytes);
                adjust = (uint)m_tracks[j].mNumBytes % 4;
                if (!CanDealWithAnimLength(mAnimationData.mLength, m_tracks[j]))
                {
                    int ibreak = 0;
                }
                if (!CanDealWithByteSize(m_tracks[j].mNumBytes))
                {
                    int ibreak = 0;
                }
            }
        }

    }

    public List<anim_DCTTrack> m_tracks = new List<anim_DCTTrack>();
}

/*----------------------------------------------------------------------------------------------------------*/

public class BitPointer
{
    private uint[] mpBuf;
    public int currentBufferLength;
    //const FetchType*	mpBuf;
    public uint mBitIdx;
    public uint bufferIndex = 0;
    public uint fixedOffset = 0;
    public uint mAlignmentAdjust;

    public BitPointer()
    {
    }

    public void Init(byte[] pByteBuffer, uint bitIdx, uint alignmentAdjust)
    {
        bufferIndex = 0;
        fixedOffset = 0;
        // not sure i have to worry about alignment here really...
        //uint align = (uint)(pByteBuffer.Length & (AnimationData.kBitsPerFetch / 8 - 1));
        //uint t1 = (AnimationData.kBitsPerFetch / 8);
        //uint align = (uint)(pByteBuffer.Length % t1);
        uint align = alignmentAdjust;
        mAlignmentAdjust = align;
        bitIdx += align << 3;
        mBitIdx = (bitIdx & (AnimationData.kBitsPerFetch - 1));

        currentBufferLength = pByteBuffer.Length / 4;
        if (mpBuf == null || mpBuf.Length < currentBufferLength)
        {
            mpBuf = new uint[currentBufferLength];
        }
        for (int i = 0; i < currentBufferLength; ++i)
        {
            mpBuf[i] = BitConverter.ToUInt32(pByteBuffer, (int)Index + (i * 4));
        }
        fixedOffset = (bitIdx / AnimationData.kBitsPerFetch);


    }

    private uint Index { get { return fixedOffset + bufferIndex; } }

    public uint ItemAt(int index)
    {
        uint item = 0;
        //uint item = BitConverter.ToUInt32(mpBuf, (int)Index+(index*4));
        if (Index + index < currentBufferLength)
        {
            item = mpBuf[Index + index];
        }

        return item;
    }

    public void Increment()
    {
        bufferIndex++;
    }

    public uint GetBitIdx(byte[] pByteBuffer, int index)
    {
        //return (index << 3) + mBitIdx;
        uint val1 = ((bufferIndex * 4) << 3) + mBitIdx;
        uint val2 = ((bufferIndex) << 3) + mBitIdx;
        uint val3 = (((bufferIndex + mAlignmentAdjust) * 4) << 3) + mBitIdx;
        uint val4 = (((bufferIndex - mAlignmentAdjust) * 4) << 3) + mBitIdx;
        uint val5 = (((bufferIndex * 4) - mAlignmentAdjust) << 3) + mBitIdx;
        uint val6 = (((Index * 4) - mAlignmentAdjust) << 3) + mBitIdx;
        return val6;
        //return (((byte*)mpBuf - pByteBuffer) << 3) + mBitIdx;
    }
}

/*----------------------------------------------------------------------------------------------------------*/

public class PrecomputedValues
{
    //public int m_numValues;
    public byte[] m_numBits = new byte[AnimationData.kNumCodes];
    public int[] m_values = new int[AnimationData.kNumCodes];
}

/*----------------------------------------------------------------------------------------------------------*/

public class AnimTrack
{
    public const int kBitOffsetBits = 18;
    public const int kKeyTimeBits = 13;

    public AnimTrack(AnimationData ad)
    {
        mAnimationData = ad;
    }

    //public virtual void Process(BinaryReader binReader)
    //{
    //    int sectionLength = binReader.ReadInt32();
    //    int pad1 = binReader.ReadInt32();
    //    mNumKeys = binReader.ReadInt32();
    //}

    public AnimationData mAnimationData;
    public int mNumKeys;
    public List<float> mKeyTimes
    {
        get { return mAnimationData.timeStepList; }
    }

    public virtual bool CanDealWithByteSize(int byteSize)
    {
        return (8 * byteSize) < (1 << kBitOffsetBits);

    }
    public virtual bool CanDealWithAnimLength(float length, anim_DCTTrack track)
    {
        return length * ((256 * 30) >> track.mRate) < 256 * ((1 << kKeyTimeBits) - AnimationData.kKeyTime_Offset);
    }


}

/*----------------------------------------------------------------------------------------------------------*/

public class BoolTrack : AnimTrack
{
    public BoolTrack(AnimationData ad)
        : base(ad)
    {
    }

    public void Process(BinaryReader binReader,int numKeys)
    {
        mNumKeys = numKeys;
        for (int i = 0; i < numKeys; ++i)
        {
            mBools.Add(binReader.ReadUInt32());
        }
    }

    public List<uint> mBools = new List<uint>();
}


public struct AnimPlayInfo
{
    public AnimPlayInfo(string name, float timeStart, float speed,float rate, bool loop)
    {
        Name = name;
        TimeStart = timeStart;
        Speed = speed;
        Rate = rate;
        Loop = loop;
        PlayRate = 1.0f;
    }

    public void CalcPlayRate(AnimationData animData)
    {
        if (TimeStart > 0)
        {
            PlayRate = 1.0f + (1.0f - TimeStart);
        }
        else if (Rate > 0)
        {
            PlayRate = Rate;
        }
        else
        {
            PlayRate = 1.0f;
        }
    }

    public float PlayRate
    {
        get; set;
    }

    public String Name;
    public float Speed;
    public float TimeStart;
    public float Rate;
    public bool Loop;
}

/*----------------------------------------------------------------------------------------------------------*/

public class anim_DCTTrack
{
    public const int kPosIntScale = 1 << 10; // all values are assumed to be stored in 1/1024th of a meter
    public const int kQuatIntScale = 1 << 12; // quat components are stored in 12 bits precision
    public const int kNominalQuality = 128;

    public uint mBitOffset;
    public uint mKeyTime0;
    public bool mbHasOneMoreKey;    // if this buffer has an additional key which was rotated in from the previous buffer
    public uint alignmentAdjust;

    public string trackBoneName;
    public BoneAnimData boneAnimData;
    public GladiusCharacterAnim gladiusAnim;
    public int boneId = -1;


    public Vector4[] mKeysArray = new Vector4[AnimationData.kBufferSize];
    //Vec3		mKeys[kBufferSize];
    public Vector3 mLastDCCoeff = new Vector3();



    public enum ePostRot
    {
        kPostQuat_Identity = 0,
        kPostQuat_X180,
        kPostQuat_Y180,
        kPostQuat_Z180,

        kPostQuat_XP90,
        kPostQuat_XM90,
        kPostQuat_YP90,
        kPostQuat_YM90,
        kPostQuat_ZP90,
        kPostQuat_ZM90,

        kPostQuat_PerBlock, // the first four kPostQuat_ are encoded for each block
    };

    public byte mRate;
    public byte mSqrtQuality;
    public byte mPreRoll;
    public ePostRot mPostRot;
    public int mNumBytes;
    public byte[] mpData;
    //union
    //{
    //    byte*		mpData;		// the pointer eventually replaces the byte count
    //    uint32		mNumBytes;
    //};

    public static void KeyInterpolate(ref Vector4 outV, ref Vector4 key0, ref Vector4 key1, float t, anim_DCTTrack track)
    {
        LinearCombine(ref outV, key0, 1.0f - t, key1, t);
    }

    public static void LinearCombine(ref Vector4 a, Vector4 b, float t, Vector4 c, float t1)
    {
        a.x = (t * b.x) + (t1 * c.x);
        a.y = (t * b.y) + (t1 * c.y);
        a.z = (t * b.z) + (t1 * c.z);
        a.w = (t * b.w) + (t1 * c.w);
    }


    public static void KeyFetch(ref Vector4 outV, ref Vector4 key, anim_DCTTrack track)
    {
        outV = new Vector4(key.x, key.y, key.z, 0.0f);
    }

    public static Vector4 QuatWQ(Vector3 ioQ)
    {
        //float xyzSqr = RavenMath::Vec3MagSqr(ioQ);
        Vector4 q = new Vector4(ioQ.x, ioQ.y, ioQ.z, 0.0f);
        float xyzSqr = ioQ.sqrMagnitude;
        if (xyzSqr < 1.0f)
        {
            //ioQ.w = RavenMath::Sqrt (1.0f - xyzSqr);
            q.w = (float)Math.Sqrt(1.0f - xyzSqr);
        }
        return q;
    }

    public static void UnrotateQuatQ(ref Quaternion outQ, ref Quaternion inQ, ePostRot postRot)
    {
        float x = inQ.x;
        float y = inQ.y;
        float z = inQ.z;
        float w = inQ.w;
        if (postRot <= anim_DCTTrack.ePostRot.kPostQuat_Z180)
        {
            switch (postRot)
            {
                case anim_DCTTrack.ePostRot.kPostQuat_Identity:
                    {
                        outQ = new Quaternion(x, y, z, w);
                        break;
                    }
                case anim_DCTTrack.ePostRot.kPostQuat_X180:
                    {
                        outQ = new Quaternion(w, z, -y, -x);
                        break;
                    }
                case anim_DCTTrack.ePostRot.kPostQuat_Y180:
                    {
                        outQ = new Quaternion(-z, w, x, -y);
                        break;
                    }
                case anim_DCTTrack.ePostRot.kPostQuat_Z180:
                    {
                        outQ = new Quaternion(y, -x, w, -z);
                        break;
                    }
            }
        }
        else
        {
            switch (postRot)
            {
                case anim_DCTTrack.ePostRot.kPostQuat_XP90:
                    {
                        inQ = new Quaternion(x + w, y + z, z - y, w - x);
                        break;
                    }
                case anim_DCTTrack.ePostRot.kPostQuat_XM90:
                    {
                        inQ = new Quaternion(x - w, y - z, z + y, w + x);
                        break;
                    }
                case anim_DCTTrack.ePostRot.kPostQuat_YP90:
                    {
                        inQ = new Quaternion(x - z, y + w, z + x, w - y);
                        break;
                    }
                case anim_DCTTrack.ePostRot.kPostQuat_YM90:
                    {
                        inQ = new Quaternion(x + z, y - w, z - x, w + y);
                        break;
                    }
                case anim_DCTTrack.ePostRot.kPostQuat_ZP90:
                    {
                        inQ = new Quaternion(x + y, y - x, z + w, w - z);
                        break;
                    }
                case anim_DCTTrack.ePostRot.kPostQuat_ZM90:
                    {
                        inQ = new Quaternion(x - y, y + x, z - w, w + z);
                        break;
                    }
            }

            float sin45 = (float)Math.Sin(Math.PI / 4);
            //sin45 = 1.0f;
            outQ = new Quaternion(inQ.x * sin45, inQ.y * sin45, inQ.z * sin45, inQ.w * sin45);
        }
    }

    public static void KeyInterpolate(ref Quaternion outQ, ref Vector4 key0, ref Vector4 key1, float t, anim_DCTTrack track)
    {

        // check if we wrapped around:
        float kJointTwistDotLimit = -0.5f;
        //float xyzDot = RavenMath::Vec3Dot (tempVecs[1], tempVecs[2]);
        float xyzDot = Vector3.Dot(new Vector3(key0.x, key0.y, key0.z), new Vector3(key1.x, key1.y, key1.z));


        if (xyzDot >= kJointTwistDotLimit)
        {
            // since data is sampled at 30fps, we really don't need the Slerp:
            Vector4 temp = new Vector4();
            LinearCombine(ref temp, key0, 1.0f - t, key1, t);

            Vector4 temp2 = QuatWQ(new Vector3(temp.x, temp.y, temp.z));
            Quaternion tq = new Quaternion(temp2.x, temp2.y, temp2.z, temp2.w);
            UnrotateQuatQ(ref outQ, ref tq, track.mPostRot);
        }
        else
        { // special interpolation when wrapping:
            Vector4 t1 = QuatWQ(new Vector3(key0.x, key0.y, key0.z));
            Vector4 t2 = QuatWQ(new Vector3(key1.x, key1.y, key1.z));

            Vector4 lerpResultiv4 = new Vector4();

            LinearCombine(ref lerpResultiv4, t1, 1.0f - t, t2, -t);

            Quaternion lerpResult = new Quaternion(lerpResultiv4.x, lerpResultiv4.y, lerpResultiv4.z, lerpResultiv4.w);

            UnrotateQuatQ(ref outQ, ref lerpResult, track.mPostRot);
        }
    }

    public static void KeyFetch(ref Quaternion outQ, ref Vector4 key, anim_DCTTrack track)
    {
        //Vector4 temp = tempVecs[0];
        Vector4 temp = QuatWQ(new Vector3(key.x, key.y, key.z));
        Quaternion tq = new Quaternion(temp.x, temp.y, temp.z, temp.w);
        UnrotateQuatQ(ref outQ, ref tq, track.mPostRot);
    }

    public static anim_DCTTrack FromStream(BinaryReader binReader)
    {
        anim_DCTTrack track = new anim_DCTTrack();
        track.mRate = binReader.ReadByte();
        track.mSqrtQuality = binReader.ReadByte();
        track.mPreRoll = binReader.ReadByte();
        track.mPostRot = (ePostRot)binReader.ReadByte();
        track.mNumBytes = binReader.ReadInt32();
        return track;
    }
}

/*----------------------------------------------------------------------------------------------------------*/

public static class BitPointerPool
{
    public static BitPointer Get(byte[] pByteBuffer, uint bitIdx, uint alignmentAdjust)
    {
        if (m_bitPointers.Count == 0)
        {
            m_bitPointers.Push(new BitPointer());
        }
        BitPointer bp = m_bitPointers.Pop();
        bp.Init(pByteBuffer, bitIdx, alignmentAdjust);
        return bp;
    }

    public static void Return(BitPointer bp)
    {
        Debug.Assert(!m_bitPointers.Contains(bp));
        m_bitPointers.Push(bp);
    }

    private static Stack<BitPointer> m_bitPointers = new Stack<BitPointer>();
}



public class AnimationData
{
    public String name;

    public GladiusSimpleAnim gladiusAnim;

    // how far this animation moves the character forward?
    public float mTranslation;
    public List<string> boneList = new List<string>();
    public List<string> eventList = new List<string>();
    public List<float> timeStepList = new List<float>();

    public AnimationClip animClip = new AnimationClip();

    public bool IsPlaying;
    public BoolTrack boolTrack;
    public MaskTrack maskTrack;
    public PosXTrack posXTrack;
    public RotXTrack rotXTrack;
    public OptPosTrack optPosTrack;
    public OptRotTrack optRotTrack;

    public float mLength;

    public int mCurXPos;
    public int mCurXRot;
    public uint m_intervalsInBuffer;
    public int m_lastBaseKey, m_lastEndKey;

    public uint mEvent;
    public uint mStickyEvent;

    public List<string> mXPosNameTable = new List<string>();
    public List<Transform> mXPosTransformTable = new List<Transform>();

    public List<string> mXRotNameTable = new List<string>();
    public List<Transform> mXRotTransformTable = new List<Transform>();

    public List<string> mPosNameTable = new List<string>();
    public List<Transform> mPosTransformTable = new List<Transform>();

    public List<string> mRotNameTable = new List<string>();
    public List<Transform> mRotTransformTable = new List<Transform>();


    //public List<int> mPosTrackIndex = new List<int>();
    public int[] mPosTrackIndex = null;
    public int[] mRotTrackIndex = null;

    public int mNumRot;
    public int mNumRawRot;
    public int mNumPos;
    public int mNumFloat;
    public int mNumInt;
    public int mNumXPos;
    public int mNumXRot;


    public const uint ANIM_RESERVED = 0x0001;
    public const uint ANIM_LOOP = 0x0002;
    public const uint ANIM_ONESHOT = 0x0008;      // remove it when it hits its last keyframe
    public const uint ANIM_FADE_DELETE = 0x0010;      // remove it when it is fully faded out (also releases the TrackSet)
    public const uint ANIM_DIDLOOP = 0x0020;      // in this update frame, the anim looped
    public const uint ANIM_DONTINTERPOLATE = 0x0040;      // don't interpolate, you sucka
    public const uint ANIM_ISCAMERATRACK = 0x0080;        // it's a camera track - bool track will say when to cut
    public const uint ANIM_FORCE_DWORD = 0x80000000;



    public const uint ANIM_EVENT_START = 0x01;            // anim started playing
    public const uint ANIM_EVENT_STOP = 0x02;         // anim is being deleted
    public const uint ANIM_EVENT_FINISHED = 0x04;         // anim has finished (eg, it's not a looping anim, and it's sitting on it's last frame)
    public const uint ANIM_EVENT_FADEIN = 0x08;           // anim has finished fading in
    public const uint ANIM_EVENT_STARTFADEOUT = 0x10;         // anim has started to fade out
    public const uint ANIM_EVENT_FADEOUT = 0x20;          // anim has faded out to 0
    public const uint ANIM_EVENT_LOOP = 0x40;         // anim is looping back
    public const uint ANIM_EVENT_PLAYING = 0x80;          // anim is playing - this will get called every frame
    public const uint ANIM_EVENT_USER = 0x100;            // user events start with this one


    public static float[] kQuantizeDividers = new float[] { 1.6f, 1.1f, 1.0f, 1.6f, 2.4f, 4.0f, 5.1f, 6.1f };


    public const int kSamplesPerBlock = 8;
    public const int kLookAheadBits = 8;
    public const int kNumCodes = 1 << kLookAheadBits;
    public const int kBitsPerFetch = 32;
    public const int kNominalQuality = 128;

    public const int pan_FilecRot = 1 << 0;
    public const int pan_FilecPos = 1 << 1;
    public const int pan_FilecFloat = 1 << 2;
    public const int pan_FilecInt = 1 << 3;
    public const int pan_FilecRawRot = 1 << 4;
    public const int pan_FilecRawPos = 1 << 5;
    public const int pan_FilecXPos = 1 << 6;
    public const int pan_FilecXRot = 1 << 7;

    public const int kBufferSize = kSamplesPerBlock + 1;
    public const int kKeyTime_Offset = kBufferSize;
    public const int kKeyTime_Bad = -kKeyTime_Offset;

    public static PrecomputedValues precomputed = new PrecomputedValues();

    static Matrix4x4 kIDCTMatrixEven0 = new Matrix4x4();
    static Matrix4x4 kIDCTMatrixOdd0 = new Matrix4x4();

    public int mBoolCounter = 0;

    public uint mFlags;

    public uint mHeld;
    public uint mPressed;
    public uint mReleased;



    float mAnimTime;

    public float AnimTime
    { get { return mAnimTime; } }

    float mAnimOverrun;  /* If we ran off the end of an animation, this is how much we overran by */
    float mAnimRate = 1.0f;
    float mLaunchTime;

    float mCurWeight;   // current weight (always heads for 0 or 1)
    float mFadeRate;        // weight velocity

    float mFadeOutTime; // fadeit out over this much time when it ends (if it's not looping)


    public void Start(float startTime,float speed, float playRate)
    {
        //mAnimTime = frame * rate * playRate * speed;
        if(speed < 0 || startTime < 0)
        {
            int ibreak = 0;
        }

        if(startTime > 0)
        {
            int ibreak = 0;
        }

        mAnimTime = startTime;


        mAnimRate = speed * playRate;
        IsPlaying = true;

    }

    public bool Complete
    {
        get 
        { 
            if(mAnimRate >= 0.0f)
            {
                float diff = mAnimTime - TrueLength;
                return diff >= 0.0f;
            }
            else
            {
                return mAnimTime <= 0.0f;
            }
        }
    }

    public void RandomAnimTIme()
    {
        mAnimTime = (float)(TrueLength * GladiusGlobals.Random.NextDouble());
        
    }

    public float FindFirstEvent(uint mask)
    {
        for (int i = 0; i < boolTrack.mNumKeys; ++i)
        {
            if ((mask & boolTrack.mBools[i]) != 0)
            {
                return boolTrack.mKeyTimes[i];
            }
        }
        return -1;
    }




    // get a mask for a bool by name (slowly - linear search)
    public uint GetEventMask(String name)
    {
        for (int i = 0; i < eventList.Count; ++i)
        {
            if (String.Equals(eventList[i], name))
            {
                return ANIM_EVENT_USER << i;    // userbool tracks start at 0x100 (24 available)
            }
        }
        return 0;
    }


    public bool hasHideEvents()
    {
        bool hideNoShow = false;
        bool hideSet = false;
        bool detachSet = false;
        bool detachNoAttach = false;
        bool showSet = false;
        bool attachSet = false;
        foreach (String eventName in eventList)
        {
            if (eventName.Contains("hide"))
            {
                hideSet = true;
            }
            if (eventName.Contains("detach"))
            {
                detachSet = true;
            }
            if (eventName.Contains("show"))
            {
                showSet = true;
            }
            if (eventName.Contains("attach"))
            {
                attachSet = true;
            }
        }
        if (hideSet && !showSet)
        {
            return true;
        }
        if (detachSet && !attachSet)
        {
            return true;
        }
        return false;
    }

    public bool IsLooping
    {
        get
        {
            return (mFlags & AnimationData.ANIM_LOOP) == AnimationData.ANIM_LOOP;
        }
    }

    public bool DidLoop
    {
        get { return (mFlags & AnimationData.ANIM_DIDLOOP) == AnimationData.ANIM_DIDLOOP; }
    }

    public object CommonModelImporter { get; private set; }

    public void Reset()
    {
        IsPlaying = false;
        mAnimTime = 0.0f;
        mAnimOverrun = 0.0f;
        mEvent = 0;
        mStickyEvent = 0;
        mBoolCounter = 0;
        mHeld = 0;
        mPressed = 0;
        mReleased = 0;
        if (mPosTrackIndex != null)
        {
            Array.Clear(mPosTrackIndex, 0, mPosTrackIndex.Length);
        }
        if (mRotTrackIndex != null)
        {
            Array.Clear(mRotTrackIndex, 0, mRotTrackIndex.Length);
        }

    }


    public void AnimateTracks(float animationSpeed)
    {
        float AnimLength = TrueLength;
        if(AnimLength <= 0f)
        {
            return;
        }


        mFlags &= ~ANIM_DIDLOOP; // clear did loop.
        if (mAnimTime >= AnimLength)
        {
            if ((mFlags & ANIM_LOOP) != 0)
            {
                mFlags |= ANIM_DIDLOOP;
                while (mAnimTime >= AnimLength)
                {
                    mAnimTime -= AnimLength;
                }
            }
        }

        bool interpolate = true;
        if (boolTrack != null)
        {
            AnimateBoolTrack();
        }
        if (posXTrack != null)
        {
            AnimateXPosTrack(interpolate);
        }
        if (rotXTrack != null)
        {
            AnimateXRotTrack(interpolate);
        }
        if (optPosTrack != null)
        {
            AnimateOptPosTrack(interpolate);
        }
        if (optRotTrack != null)
        {
            AnimateOptRotTrack(interpolate);
        }

        int ibreak = 0;

        float timeDelta = Time.deltaTime;
        mAnimTime += (timeDelta * animationSpeed);



        //Blend();
        //PostBlend();
        //Build();
    }

    public void AnimateBoolTrack()
    {
        if (boolTrack == null)
        {
            return;
        }
        // this should be ascertained by the caller:
        //ASSERT(mTrackSet->mBoolTrack.mNumKeys > 1);

        float time = mAnimTime;

        bool looped = false;
        //anim_BoolTrack & boolTrack = mTrackSet->mBoolTrack;

        int boolKey = (int)mBoolCounter;
        uint oldBools = mHeld;

        mPressed = 0;
        mReleased = 0;

        int oldBoolKey = boolKey;

        bool done = false;

        while (!done)
        {
            int nextKey = boolKey + 1;

            // sort out the looping here...

            if (nextKey == boolTrack.mNumKeys - 1)
            {
                if ((mFlags & ANIM_LOOP) != 0)
                {
                    looped = true;
                    /* WOLF[14May02]  Attempt at fixing the last key problem. Don't search back through if we go off the end */
                    if (time >= boolTrack.mKeyTimes[nextKey])
                    {
                        done = true;
                    }
                }
                else
                {
                    done = true;
                }
            }

            if (time >= boolTrack.mKeyTimes[boolKey] && time < boolTrack.mKeyTimes[nextKey])
            {
                done = true;
            }

            mHeld = boolTrack.mBools[boolKey];

            uint delta = oldBools ^ mHeld;

            mPressed |= delta & mHeld;
            mReleased |= delta & ~mHeld;
            oldBools = mHeld;

            //console_Printf("Update: (%d) %x %x %x\n",boolKey,mHeld,mPressed,mReleased);

            if (!done)
            {
                boolKey = incmod(boolKey, (int)boolTrack.mNumKeys - 1);
            }

            if (boolKey == oldBoolKey)
            {
                break;
            }
        }
        mBoolCounter = boolKey;

    }

    public void AnimateXPosTrack(bool interpolate)
    {
        float time = mAnimTime;
        int compIdx = 0;
        int sPosStorageIndex = 0;
        mCurXPos = 0;

        //mCurXPos = anim_Character::sPosStorage;
        // CAUTION KNOWN BUG: cuts that happen a few frames after the begin or before the end of the anim cause problems when looping
        //int bSkipBlock = mReleased & mTrackSet->mCameraCutMask; // cut just happened?
        //PoolNode* pNode = mDCTPosTrackBuffers;

        for (int posJoint = 0; posJoint < mNumXPos; ++posJoint, ++sPosStorageIndex)
        {
            if (mAnimRate >= 0.0f)
            {
                anim_DCTTrack track = posXTrack.m_tracks[posJoint];
                if (track.boneId != -1)
                {
                    Vector4 output = new Vector4();
                    Quaternion iq = Quaternion.identity;
                    Eval(ref output, ref iq, true, track, mLength, time, (mFlags & ANIM_LOOP) != 0, false);

                    GladiusGlobals.AdjustV4(ref output);

                    //output *= -1.0f;

                    if (track.boneAnimData != null)
                    {
                        track.boneAnimData.CurrentPosition = output;
                    }
                }
                else
                {
                    // if we're sharing anims then we may get tracks for bones we don't have (hair?)
                    int ibreak = 0;
                }
            }
        }
    }

    public void AnimateXRotTrack(bool interpolate)
    {
        float time = mAnimTime;
        uint compIdx = 0;

        mCurXRot = 0;
        int sRotStorageIndex = 0;

        for (int rotJoint = 0; rotJoint < mNumXRot; ++rotJoint, ++sRotStorageIndex)
        {
            if (mAnimRate >= 0.0f)
            {
                anim_DCTTrack track = rotXTrack.m_tracks[rotJoint];
                if (track.boneId != -1)
                {
                    Vector4 iv4 = new Vector4();
                    Quaternion output = Quaternion.identity;
                    Eval(ref iv4, ref output, false, track, mLength, time, (mFlags & ANIM_LOOP) != 0, false);

                    GladiusGlobals.AdjustQuaternion(ref output);

                    if (track.boneAnimData != null)
                    {
                        track.boneAnimData.CurrentRotation = output;
                    }
                }
                else
                {
                    // if we're sharing anims then we may get tracks for bones we don't have (hair?)
                    int ibreak = 0;
                }
            }
        }
    }

    public void AnimateOptRotTrack(bool interpolate)
    {
        float time = mAnimTime;
        // animate all the optimized rot tracks
        for (int rotJoint = 0; rotJoint < mNumRot; ++rotJoint)
        {
            if (mAnimRate >= 0.0f)
            {
                anim_OptRotTrack rotTrack = optRotTrack.m_tracks[rotJoint];
                int rotKey = mRotTrackIndex[rotJoint];


                if (rotTrack.boneId != -1)
                {
                    Quaternion resolvedQuaternion = Quaternion.identity;

                    do
                    {
                        float endTime = 0.0f;
                        float startTime = 0.0f;

                        int key = rotKey + 1;

                        // hit the last key?
                        if (key >= (int)rotTrack.mNumKeys)
                        {
                            // yes, looping anim?
                            if ((mFlags & ANIM_LOOP) != 0)
                            {
                                // yes, loop the keycounter
                                endTime = mLength;
                                key = 0;
                            }
                            else
                            {
                                // no looping, stickit on the last key
                                key = rotKey;

                                rotTrack.mOptQuats[key].Get(ref resolvedQuaternion);
                                goto Done;
                            }
                        }
                        else
                        {
                            // snarf the next key's time
                            endTime = rotTrack.mKeyTimes[key] * (1.0f / 30.0f); ;
                        }

                        startTime = rotTrack.mKeyTimes[rotKey] * (1.0f / 30.0f); ;

                        // time is inside this key?

                        if (time >= startTime && time <= endTime)
                        {
                            // yes, slerp it based on how far along this key the time is
                            mRotTrackIndex[rotJoint] = rotKey;
                            float keyEndTime = rotTrack.mKeyTimes[rotKey] * (1.0f / 30.0f);

                            float timeDiff = time - keyEndTime;
                            float timeSlice = endTime - keyEndTime;

                            float interpAmount = GetInterp(interpolate, timeDiff, timeSlice);

                            Quaternion quat0 = Quaternion.identity;
                            Quaternion quat1 = Quaternion.identity;

                            rotTrack.mOptQuats[rotKey].Get(ref quat0);
                            rotTrack.mOptQuats[key].Get(ref quat1);
                            resolvedQuaternion = Quaternion.Slerp(quat0, quat1, interpAmount);
                            // and quit the key search loop
                            break;
                        }

                        rotKey = incmod(rotKey, (int)rotTrack.mNumKeys);

                    } while (rotKey != mRotTrackIndex[rotJoint]); // what to do if total keysearch fails...?

                Done:
                    if (rotTrack.boneAnimData != null)
                    {
                        GladiusGlobals.AdjustQuaternion(ref resolvedQuaternion);
                        rotTrack.boneAnimData.CurrentRotation = resolvedQuaternion;
                    }




                }
            }
        }
    }

    public void AnimateOptPosTrack(bool interpolate)
    {
        float time = mAnimTime;
        int sPosStorageIndex = 0;

        for (int posJoint = 0; posJoint < mNumPos; ++posJoint, ++sPosStorageIndex)
        {
            if (mAnimRate >= 0.0f)
            {
                anim_OptPosTrack posTrack = optPosTrack.m_tracks[posJoint];
                int posKey = mPosTrackIndex[posJoint];


                if (posTrack.boneId != -1)
                {
                    Vector3 resolvedPos = new Vector3();

                    if (time >= mLength)
                    {
                        posTrack.mOptVecs[(int)posTrack.mNumKeys - 1].Get(ref resolvedPos, ref posTrack.mPosScalar);
                    }
                    else if (mAnimRate >= 0.0f)
                    {
                        do
                        {
                            float endTime;
                            float startTime;

                            int key = posKey + 1;

                            if (key == (int)posTrack.mNumKeys)
                            {
                                if ((mFlags & ANIM_LOOP) != 0)
                                {
                                    endTime = mLength;
                                    key = 0;
                                }
                                else
                                {
                                    key = posKey;
                                    posTrack.mOptVecs[key].Get(ref resolvedPos, ref posTrack.mPosScalar);
                                    break;
                                }
                            }
                            else
                            {
                                endTime = posTrack.mOptVecs[key].time * (1.0f / 30.0f);
                            }
                            startTime = posTrack.mOptVecs[posKey].time * (1.0f / 30.0f);

                            if (time >= startTime && time <= endTime)
                            {
                                mPosTrackIndex[posJoint] = posKey;
                                float keyEndTime = posTrack.mOptVecs[posKey].time * (1.0f / 30.0f);
                                float timeDiff = time - keyEndTime;
                                float timeSlice = endTime - keyEndTime;

                                float interpAmount = GetInterp(interpolate, timeDiff, timeSlice);

                                //float interpAmount = 1.0f;// FIXME GetInterp(interpolate, timeDiff, timeSlice);

                                Vector3 v0 = new Vector3();
                                Vector3 v1 = new Vector3();

                                posTrack.mOptVecs[posKey].Get(ref v0, ref posTrack.mPosScalar);
                                posTrack.mOptVecs[key].Get(ref v1, ref posTrack.mPosScalar);

                                resolvedPos = Vector3.Lerp(v0, v1, interpAmount);
                                break;
                            }
                            posKey = incmod(posKey, (int)posTrack.mNumKeys);
                        }
                        while (posKey != mPosTrackIndex[posJoint]);
                    }
                    if (posTrack.boneAnimData != null)
                    {
                        GladiusGlobals.GladiusToUnity(ref resolvedPos);
                        posTrack.boneAnimData.CurrentPosition = resolvedPos;
                    }


                }
            }
        }
    }


    public bool EventPressedOccurred(String name)
    {
        //return (mHeld & GetEventMask(name)) != 0;
        //return (mReleased & GetEventMask(name)) != 0;
        return (mPressed & GetEventMask(name)) != 0;
    }

    public bool EventReleaseddOccurred(String name)
    {
        //return (mHeld & GetEventMask(name)) != 0;
        //return (mReleased & GetEventMask(name)) != 0;
        return (mReleased & GetEventMask(name)) != 0;
    }

    public bool EventHeld(String name)
    {
        //return (mHeld & GetEventMask(name)) != 0;
        //return (mReleased & GetEventMask(name)) != 0;
        return (mHeld & GetEventMask(name)) != 0;
    }



    public bool Fade(float timeDelta)
    {
        uint oldsticky = mStickyEvent;
        float AnimLength;

        int r = UpdateWeight(timeDelta);

        // mark new sticky events
        mEvent |= (mStickyEvent ^ oldsticky) & mStickyEvent;

        //HandleCallbacks();

        mAnimOverrun = 0.0f;

        AnimLength = TrueLength;

        mFlags &= ~ANIM_DIDLOOP;
        if (r != 1)
        {
            return true;
        }
        else
        {
            if (AnimLength <= 0)
            {
                mAnimTime = 0;

                if ((mFlags & ANIM_ONESHOT) != 0)
                {
                    FadeOut(mFadeOutTime);
                }
            }
            else if (mAnimRate >= 0.0f)
            {
                if (mAnimTime < 0.0f)
                {
                    mAnimTime = 0.0f;
                }
                else if (mAnimTime >= AnimLength)
                {
                    mAnimOverrun = mAnimTime - AnimLength;

                    if ((mFlags & ANIM_LOOP) != 0)
                    {
                        mFlags |= ANIM_DIDLOOP;
                        while (mAnimTime >= AnimLength)
                        {
                            mAnimTime -= AnimLength;
                        }
                    }
                    else
                    {
                        mAnimTime = AnimLength;
                        if ((mFlags & ANIM_ONESHOT) != 0)
                        {
                            FadeOut(mFadeOutTime);
                        }
                    }
                }
            }
            else
            {
                if (mAnimTime > AnimLength)
                {
                    mAnimTime = AnimLength;
                }
                else if (mAnimTime <= 0.0f)
                {
                    mAnimOverrun = -mAnimTime;

                    if ((mFlags & ANIM_LOOP) != 0)
                    {
                        mFlags |= ANIM_DIDLOOP;
                        while (mAnimTime <= 0.0f)
                        {
                            mAnimTime += AnimLength;
                        }
                    }
                    else
                    {
                        mAnimTime = 0;
                        if ((mFlags & ANIM_ONESHOT) != 0)
                        {
                            FadeOut(mFadeOutTime);
                        }
                    }
                }
            }
            return false;
        }
    }

    public void FadeOut(float timeDelta)
    {
    }

    public int UpdateWeight(float timeDelta)
    {
        // always set the playing flag
        mEvent = ANIM_EVENT_PLAYING;

        // is it fading in or out?
        if (mFadeRate != 0.0f)
        {
            // yes, do the fading in/out
            if (mFadeRate > 9999999)
            {
                mCurWeight = 1.0f;
            }
            else if (mFadeRate < -9999999)
            {
                mCurWeight = 0.0f;
            }
            else
            {
                mCurWeight += mFadeRate * timeDelta;
            }

            // fully faded in?
            if (mFadeRate > 0 && mCurWeight >= 1.0f)
            {
                // clamp curWeight, stop fading
                mCurWeight = 1.0f;
                mFadeRate = 0;

                // notify client that animation has finished fading in
                mEvent |= ANIM_EVENT_FADEIN;
            }
            // fully faded out?
            else if (mFadeRate < 0 && mCurWeight <= 0.0f)
            {
                mCurWeight = 0.0f;
                mFadeRate = 0;

                // fire and forget special?
                if ((mFlags & (ANIM_ONESHOT | ANIM_FADE_DELETE)) != 0)
                {
                    // yes, return 1 to tell caller to delete this
                    mEvent |= ANIM_EVENT_FINISHED;
                    return 1;
                }
                // not a fire and forget
                else
                {
                    // notify client that anim has finished fading out
                    mEvent |= ANIM_EVENT_FADEOUT;
                }
            }
        }
        return 0;
    }

    public float TrueLength
    {
        get
        {
            float AnimLength = 0.0f;
            if ((mFlags & ANIM_LOOP) != 0)
            {
                AnimLength = mLength;
                //AnimLength = mLength - GladiusAnim.AnimFrameRate;
            }
            else
            {
                AnimLength = mLength - GladiusCharacterAnim.FixedAnimFrameRate;
            }
            // If we don't do this by assignment, we have the double/float conversion problem
            return (AnimLength);
        }
    }


    public void BuildPrecomputeGrid()
    {
        // precompute IDCT matrices and factor in quantization table:
        float c0 = .3535533905f; // =sqrt(1/8)
        float ck = 0.5f;
        float pi8 = ((float)(Math.PI)) / 8.0f;
        float pi16 = ((float)(Math.PI)) / 16.0f;
        float pi32 = ((float)(Math.PI)) / 32.0f;
        float pi64 = ((float)(Math.PI)) / 64.0f;

        //PrecomputedValues precomputed = new PrecomputedValues();


        int k, s;
        for (k = 0; k < 4; ++k)
        {
            float ckEven = (k == 0 ? c0 : ck) * kQuantizeDividers[2 * k];
            float ckOdd = ck * kQuantizeDividers[2 * k + 1];
            // 30fps coefficients:

            int rowEvenIndex = 4 * k;
            int rowOddIndex = 4 * k;
            //float* rowEven0 = &kIDCTMatrixEven0[4*k];
            //float* rowOdd0 = &kIDCTMatrixOdd0[4*k];
            // and compute them:
            Vector4 evenV4 = new Vector4();
            Vector4 oddV4 = new Vector4();

            for (s = 0; s < 4; ++s)
            { // using cosf here in the hope to get maximum precision:
              // 30fps matrices:
                evenV4[s] = ckEven * (float)Math.Cos(pi8 * (((1 + 2 * s) * k) & 0xf));
                oddV4[s] = ckOdd * (float)Math.Cos(pi16 * (((1 + 2 * s) * (1 + 2 * k)) & 0x1f));
                //kIDCTMatrixEven0[rowEvenIndex + s] = ckEven * (float)Math.Cos(pi8 * (((1 + 2 * s) * k) & 0xf));
                //kIDCTMatrixOdd0[rowOddIndex + s] = ckOdd * (float)Math.Cos(pi16 * (((1 + 2 * s) * (1 + 2 * k)) & 0x1f));
            }
            Assign(ref kIDCTMatrixEven0, k, ref evenV4);
            Assign(ref kIDCTMatrixOdd0, k, ref oddV4);
        }




        // precompute bit codes:
        int codeBits, valueBits;
        int value;
        for (int code = 0; code < AnimationData.kNumCodes; ++code)
        {
            if ((code & 1) == 0)
            { // code = 0...
                if ((code & 2) == 0)
                { // code = 00...
                    codeBits = 2;
                    valueBits = 0;
                    value = 0;
                }
                else
                { // code = 01...
                    codeBits = 3;
                    if ((code & 4) == 0)
                        valueBits = 1; // code 010 -> 1
                    else
                        valueBits = 2; // code 011 -> 2
                }
            }
            else
            { // code = 1...
                if ((code & 2) == 0)
                { // code = 10...
                    codeBits = 3;
                    if ((code & 4) == 0)
                        valueBits = 3; // code 100 -> 3
                    else
                        valueBits = 4; // code 101 -> 4
                }
                else
                { // code = 11...
                  // code 110 -> 5, 1110 ->6, 11110 -> 7 and so on
                    uint mask = 4;
                    valueBits = 5;
                    if ((code & mask) != 0)
                    {
                        do
                        {
                            ++valueBits;
                            mask <<= 1;
                        } while ((code & mask) != 0);
                    }
                    codeBits = valueBits - 2;
                }
            }
            // extract value:
            // mask out the result:
            {
                int maskOR = -1 << valueBits;
                int maskAND = ~maskOR;
                value = (code >> codeBits) & maskAND;
                // if MSB==0, it's negative:
                if (value <= (maskAND >> 1))
                    value = (value | maskOR) + 1; // make negative and add 1 back on
            }

            // and store result:
            precomputed.m_numBits[code] = (byte)(codeBits + valueBits);
            precomputed.m_values[code] = value;
        }
    }

    static Vector3[] tempVecs = new Vector3[3];
    static float[] DCTCoeffs = new float[3 * kSamplesPerBlock + 4];

    public uint DecodeBlock(Vector4[] outKeys, int startIndex, ref Vector3 ioDCCoeff, byte[] pBuffer, uint bitOffset, float scale, uint alignmentAdjust)
    {
        //IndexedVector3;
        // decode them from bit stream:
        BitPointer bitPtr = BitPointerPool.Get(pBuffer, bitOffset, alignmentAdjust);
        EDecode(DCTCoeffs, 3 * kSamplesPerBlock, bitPtr);

        //int ioDCCoeffIndex = 0;
        int pBufferIndex = 0;

        // restore dc offsets:
        float[] pDCTCoeffs = DCTCoeffs;
        //Vector3 currentV3 = ioDCCoeff[ioDCCoeffIndex];
        ioDCCoeff.x = (pDCTCoeffs[0 * kSamplesPerBlock] += ioDCCoeff.x);
        ioDCCoeff.y = (pDCTCoeffs[1 * kSamplesPerBlock] += ioDCCoeff.y);
        ioDCCoeff.z = (pDCTCoeffs[2 * kSamplesPerBlock] += ioDCCoeff.z);
        //ioDCCoeff[ioDCCoeffIndex] = currentV3;

        Vec4Mul(pDCTCoeffs, 0, pDCTCoeffs, 0, scale);
        Vec4Mul(pDCTCoeffs, 4, pDCTCoeffs, 4, scale);
        Vec4Mul(pDCTCoeffs, 8, pDCTCoeffs, 8, scale);
        Vec4Mul(pDCTCoeffs, 12, pDCTCoeffs, 12, scale);
        Vec4Mul(pDCTCoeffs, 16, pDCTCoeffs, 16, scale);
        Vec4Mul(pDCTCoeffs, 20, pDCTCoeffs, 20, scale);

        // IDCT them:
        //ASSERT (kSamplesPerBlock == 8);
        //register float * const pTemp = tempVecs[0];
        //float[] pTemp = new float[16];
        Vector4 temp = new Vector4();
        Vector4 temp2 = new Vector4();

        for (int i = 0; i < 3; ++i)
        {
            //float *  pCoeffs = &DCTCoeffs[i*kSamplesPerBlock];
            Vector4 pCoeffs = new Vector4();
            Vector4 pCoeffs2 = new Vector4();
            for (int j = 0; j < 4; ++j)
            {
                pCoeffs[j] = DCTCoeffs[(i * kSamplesPerBlock) + j];
                pCoeffs2[j] = DCTCoeffs[(i * kSamplesPerBlock) + j + 4];
            }

            // do 8 point IDCT on shuffled coefficients (includes unquantization factors):
            // this is an optimization that exploits symmetries in the full 8x8 IDCT matrix
            // even and odd coefficients can be transformed separately,
            // then their sum and difference, respectively, provides the first 4 and second 4 samples
            // NOTE ON SHUFFLING: first even, then odd coefficients
            Mat44MulVec4(ref temp, ref kIDCTMatrixEven0, ref pCoeffs);
            Mat44MulVec4(ref temp2, ref kIDCTMatrixOdd0, ref pCoeffs2);
            // derive samples 0..3:
            Vector4 result = new Vector4();
            Vec4Add(ref result, ref temp, ref temp2);

            SetVal(outKeys, startIndex + 0, i, result[0]);
            SetVal(outKeys, startIndex + 1, i, result[1]);
            SetVal(outKeys, startIndex + 2, i, result[2]);
            SetVal(outKeys, startIndex + 3, i, result[3]);

            Vec4Sub(ref result, ref temp, ref temp2);
            SetVal(outKeys, startIndex + 4, i, result[3]);
            SetVal(outKeys, startIndex + 5, i, result[2]);
            SetVal(outKeys, startIndex + 6, i, result[1]);
            SetVal(outKeys, startIndex + 7, i, result[0]);

        }

        uint result2 = (uint)bitPtr.GetBitIdx(pBuffer, pBufferIndex);
        BitPointerPool.Return(bitPtr);

        return result2;
    }

    public void AssignModelAndSkeleton(GladiusSimpleAnim gladiusAnim, Transform transform)
    {
        // go through and assign joint indicies.
        for (int i = 0; i < mXPosNameTable.Count; ++i)
        {
            String boneName = mXPosNameTable[i];
            BoneAnimData bad = FindJointForName(gladiusAnim.m_orderedBoneList, boneName);
            if (bad != null)
            {
                //posTrack.m_tracks[i].boneAnimData = bad;
                posXTrack.m_tracks[i].boneId = bad.boneId;
                posXTrack.m_tracks[i].boneAnimData = bad;
                posXTrack.m_tracks[i].trackBoneName = mXPosNameTable[i];
            }
            else
            {
                //Debug.LogWarningFormat("Anim {0} Model {1} Bone-mismatch {2} ", name, gladiusAnim.name, boneName);
            }
        }

        for (int i = 0; i < mXRotNameTable.Count; ++i)
        {
            String boneName = mXRotNameTable[i];
            BoneAnimData bad = FindJointForName(gladiusAnim.m_orderedBoneList, boneName);
            if (bad != null)
            {
                //rotTrack.m_tracks[i].boneAnimData = bad;
                rotXTrack.m_tracks[i].boneId = bad.boneId;
                rotXTrack.m_tracks[i].boneAnimData = bad;
                rotXTrack.m_tracks[i].trackBoneName = mXRotNameTable[i];
            }
            else
            {
                //Debug.LogWarningFormat("Anim {0} Model {1} Bone-mismatch {2} ", name, gladiusAnim.name, boneName);
            }
        }

        for (int i = 0; i < mPosNameTable.Count; ++i)
        {
            String boneName = mPosNameTable[i];
            BoneAnimData bad = FindJointForName(gladiusAnim.m_orderedBoneList, boneName);
            if (bad != null)
            {
                //rotTrack.m_tracks[i].boneAnimData = bad;
                optPosTrack.m_tracks[i].boneId = bad.boneId;
                optPosTrack.m_tracks[i].boneAnimData = bad;
                optPosTrack.m_tracks[i].trackBoneName = mPosNameTable[i];
            }
        }

        for (int i = 0; i < mRotNameTable.Count; ++i)
        {
            String boneName = mRotNameTable[i];
            BoneAnimData bad = FindJointForName(gladiusAnim.m_orderedBoneList, boneName);
            if (bad != null)
            {
                //rotTrack.m_tracks[i].boneAnimData = bad;
                optRotTrack.m_tracks[i].boneId = bad.boneId;
                optRotTrack.m_tracks[i].boneAnimData = bad;
                optRotTrack.m_tracks[i].trackBoneName = mRotNameTable[i];
            }
        }


        mPosTrackIndex = new int[mPosNameTable.Count];
        mRotTrackIndex = new int[mRotNameTable.Count];
    }


    public static BoneAnimData FindJointForName(List<BoneAnimData> dataList, string name)
    {
        //return dataList.Find(x => x.boneName.StartsWith(name));
        BoneAnimData result = dataList.Find(x => x.boneName.Equals(name));
        return result;
    }

    public static void Assign(ref Matrix4x4 mtx, int row, ref Vector4 v)
    {
        if (row == 0)
        {
            mtx.m00 = v.x;
            mtx.m01 = v.y;
            mtx.m02 = v.z;
            mtx.m03 = v.w;
        }
        else if (row == 1)
        {
            mtx.m10 = v.x;
            mtx.m11 = v.y;
            mtx.m12 = v.z;
            mtx.m13 = v.w;
        }
        else if (row == 2)
        {
            mtx.m20 = v.x;
            mtx.m21 = v.y;
            mtx.m22 = v.z;
            mtx.m23 = v.w;
        }
        else if (row == 3)
        {
            mtx.m30 = v.x;
            mtx.m31 = v.y;
            mtx.m32 = v.z;
            mtx.m33 = v.w;
        }
    }



    public static void Vec4Mul(float[] dst, int dstIndex, float[] src, int srcIndex, float scale)
    {
        for (int i = 0; i < 4; ++i)
        {
            dst[dstIndex + i] = src[srcIndex + i] * scale;
        }
    }

    //Mat44MulVec4

    public static void Vec4Add(ref Vector4 dst, ref Vector4 src1, ref Vector4 src2)
    {
        for (int i = 0; i < 4; ++i)
        {
            dst[i] = src1[i] + src2[i];
        }
    }

    public static void Vec4Sub(ref Vector4 dst, ref Vector4 src1, ref Vector4 src2)
    {
        for (int i = 0; i < 4; ++i)
        {
            dst[i] = src1[i] - src2[i];
        }
    }

    public static void Mat44MulVec4(ref Vector4 dst, ref Matrix4x4 matrix, ref Vector4 vector)
    {
        //Vector4.Transform(ref src, ref mtx, out dst);
        //dst = mtx * src;
        //dst = src * mtx;
        dst = new Vector4((vector.x * matrix.m00) + (vector.y * matrix.m10) + (vector.z * matrix.m20) + (vector.w * matrix.m30),
                             (vector.x * matrix.m01) + (vector.y * matrix.m11) + (vector.z * matrix.m21) + (vector.w * matrix.m31),
                             (vector.x * matrix.m02) + (vector.y * matrix.m12) + (vector.z * matrix.m22) + (vector.w * matrix.m32),
                             (vector.x * matrix.m03) + (vector.y * matrix.m13) + (vector.z * matrix.m23) + (vector.w * matrix.m33));
    }

    public static void SetVal(Vector4[] lv, int index, int i, float val)
    {
        Vector4 iv3 = lv[index];
        iv3[i] = val;
        lv[index] = iv3;
    }

    public static Quaternion ToQ(Vector3 v)
    {
        return ToQ(v.y, v.x, v.z);
    }

    public static Quaternion ToQ(float yaw, float pitch, float roll)
    {
        yaw *= Mathf.Deg2Rad;
        pitch *= Mathf.Deg2Rad;
        roll *= Mathf.Deg2Rad;
        float rollOver2 = roll * 0.5f;
        float sinRollOver2 = (float)Math.Sin((double)rollOver2);
        float cosRollOver2 = (float)Math.Cos((double)rollOver2);
        float pitchOver2 = pitch * 0.5f;
        float sinPitchOver2 = (float)Math.Sin((double)pitchOver2);
        float cosPitchOver2 = (float)Math.Cos((double)pitchOver2);
        float yawOver2 = yaw * 0.5f;
        float sinYawOver2 = (float)Math.Sin((double)yawOver2);
        float cosYawOver2 = (float)Math.Cos((double)yawOver2);
        Quaternion result;
        result.w = cosYawOver2 * cosPitchOver2 * cosRollOver2 + sinYawOver2 * sinPitchOver2 * sinRollOver2;
        result.x = cosYawOver2 * sinPitchOver2 * cosRollOver2 + sinYawOver2 * cosPitchOver2 * sinRollOver2;
        result.y = sinYawOver2 * cosPitchOver2 * cosRollOver2 - cosYawOver2 * sinPitchOver2 * sinRollOver2;
        result.z = cosYawOver2 * cosPitchOver2 * sinRollOver2 - sinYawOver2 * sinPitchOver2 * cosRollOver2;

        return result;
    }

    public static Vector3 FromQ2(Quaternion q1)
    {
        float sqw = q1.w * q1.w;
        float sqx = q1.x * q1.x;
        float sqy = q1.y * q1.y;
        float sqz = q1.z * q1.z;
        float unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
        float test = q1.x * q1.w - q1.y * q1.z;
        Vector3 v;

        if (test > 0.4995f * unit)
        { // singularity at north pole
            v.y = 2f * Mathf.Atan2(q1.y, q1.x);
            v.x = Mathf.PI / 2;
            v.z = 0;
            return NormalizeAngles(v * Mathf.Rad2Deg);
        }
        if (test < -0.4995f * unit)
        { // singularity at south pole
            v.y = -2f * Mathf.Atan2(q1.y, q1.x);
            v.x = -Mathf.PI / 2;
            v.z = 0;
            return NormalizeAngles(v * Mathf.Rad2Deg);
        }
        Quaternion q = new Quaternion(q1.w, q1.z, q1.x, q1.y);
        v.y = (float)Math.Atan2(2f * q.x * q.w + 2f * q.y * q.z, 1 - 2f * (q.z * q.z + q.w * q.w));     // Yaw
        v.x = (float)Math.Asin(2f * (q.x * q.z - q.w * q.y));                             // Pitch
        v.z = (float)Math.Atan2(2f * q.x * q.y + 2f * q.z * q.w, 1 - 2f * (q.y * q.y + q.z * q.z));      // Roll
        return NormalizeAngles(v * Mathf.Rad2Deg);
    }

    static Vector3 NormalizeAngles(Vector3 angles)
    {
        angles.x = NormalizeAngle(angles.x);
        angles.y = NormalizeAngle(angles.y);
        angles.z = NormalizeAngle(angles.z);
        return angles;
    }

    static float NormalizeAngle(float angle)
    {
        while (angle > 360)
            angle -= 360;
        while (angle < 0)
            angle += 360;
        return angle;
    }



    public static Vector3 ExtractEulerZYX(Quaternion q)
    {
        Matrix4x4 m = Matrix4x4.TRS(Vector3.zero, q, Vector3.one);
        return ExtractEulerZYX(m);
    }

    public static Vector3 ExtractEulerZYX(Matrix4x4 m)
    {
        m = Matrix4x4.Transpose(m);

        Vector3 r = new Vector3();
        if (m.m20 < 1.0f)
        {
            if (m.m20 > -1.0f)
            {
                r.y = (float)Math.Asin(-m.m20);
                r.z = (float)Math.Atan2(m.m10, m.m00);
                r.x = (float)Math.Atan2(m.m21, m.m22);
            }
            else
            {
                // Not a unique solution.
                r.y = (float)Math.PI / 2.0f;
                r.z = -(float)Math.Atan2(-m.m12, m.m11);
                r.x = 0;
            }
        }
        else
        {
            r.y = (float)-Math.PI / 2.0f;
            r.z = -(float)Math.Atan2(-m.m12, m.m11);
            r.x = 0;

        }
        return r;
    }


    public static int incmod(int val, int maxVal)
    {
        return (val + 1) % maxVal;
    }





    public void EDecode(float[] outValues, int count, BitPointer bitpointer)
    {
        uint bufQ = bitpointer.ItemAt(0);
        uint bufQNext = bitpointer.ItemAt(1);
        //uint bufQNext = bitpointer.NextInt32();
        uint bitIdx = bitpointer.mBitIdx;
        int outValueIndex = 0;

        do
        {
            uint codeQ = bitIdx != 0 ? (bufQ >> (int)bitIdx) | (bufQNext << (int)(kBitsPerFetch - bitIdx)) : bufQ;

            // see if this code is precomputed:
            uint idx = codeQ & (uint)(kNumCodes - 1);
            if (idx != 0)
            { // do regular decoding:
                float result = 0.0f; ;
                uint numBits = precomputed.m_numBits[idx];
                if (numBits <= kLookAheadBits)
                { // yes, quickly get it:
                    result = (float)(precomputed.m_values[idx]);
                    bitIdx += numBits;
                }
                else
                { // no, compute 'manually':
                    uint codeBits, valueBits;
                    // is the bit count still reliable?
                    if (numBits <= 2 * kLookAheadBits + 2)
                    { // yes, deduce code & value bit count:
                        codeBits = (numBits >> 1) - 1;
                        valueBits = numBits - codeBits;
                    }
                    else
                    { // no, decode from code word:
                        int mask = kNumCodes;
                        valueBits = kLookAheadBits + 3;
                        if ((codeQ & mask) != 0)
                        {
                            do
                            {
                                ++valueBits;
                                mask <<= 1;
                            } while ((codeQ & mask) != 0);
                        }
                        codeBits = valueBits - 2;
                    }

                    // now that we know how many bits to get, rotate that word into place and extract it out:
                    bitIdx += codeBits;
                    if (bitIdx >= kBitsPerFetch)
                    {
                        //bitIdx -= kBitsPerFetch;
                        //++bufPtr;
                        //bufQ = bufQNext;
                        //bufQNext = endian_Long(bufPtr[1]);
                        bitIdx -= kBitsPerFetch;
                        bitpointer.Increment();
                        bufQ = bufQNext;
                        bufQNext = bitpointer.ItemAt(1);
                    }
                    codeQ = bitIdx != 0 ? (bufQ >> (int)bitIdx) | (bufQNext << (int)(kBitsPerFetch - bitIdx)) : bufQ;

                    // mask out the result:
                    int maskOR = -1 << (int)valueBits;
                    int maskAND = ~maskOR;
                    int r = (int)(codeQ) & maskAND;
                    // if MSB==0, it's negative:
                    if (r <= (maskAND >> 1))
                        r = (r | maskOR) + 1; // make negative and add 1 back on
                    bitIdx += valueBits;
                    result = (float)(r);
                }

                // store result:
                //*outValues++ = result;
                outValues[outValueIndex++] = result;
                --count;
            }
            else
            { // all zero means four consecutive zeroes:
                outValues[outValueIndex++] = 0.0f;
                outValues[outValueIndex++] = 0.0f;
                outValues[outValueIndex++] = 0.0f;
                outValues[outValueIndex++] = 0.0f;
                bitIdx += 8;
                count -= 4;
                if (count < 0)
                { // if overshot, rewind bit position:
                    bitIdx += (uint)(2 * count);
                }
            }

            // increment bit pointer:
            if (bitIdx >= kBitsPerFetch)
            {
                bitIdx -= kBitsPerFetch;
                bitpointer.Increment();
                bufQ = bufQNext;
                bufQNext = bitpointer.ItemAt(1);
            }
        } while (count > 0);

        //bitPointer.mpBuf = bufPtr;
        bitpointer.mBitIdx = bitIdx;
    }


    public void Eval(ref Vector4 outV, ref Quaternion outQ, bool pos, anim_DCTTrack track, float length, float time, bool bLooping, bool bSkipBlock)
    {
        int kIntScale = pos ? anim_DCTTrack.kPosIntScale : anim_DCTTrack.kQuatIntScale;  // kQuatIntScale;


        // determine sample rate of this track:
        int ratePower = track.mRate;
        float rateFactor = (256 >> ratePower) * (1.0f / 256.0f);
        float rate = 30.0f * rateFactor;

        if (time > length)
        {
            int ibreak = 0;
        }
        //Debug.Assert(time >= 0.0f && time <= length);
        if (time < 0.0f || time > length)
        {
            int ibreak = 0;
        }

        // convert from seconds to frames:
        float keyTime = time * rate;
        float keyLength = length * rate;
        float keyBack = keyLength - rateFactor; // last key that's represented by source data
                                                //int lastBaseKey, lastEndKey;
        if (bLooping)
        { // looping - check if in the wrap area:
            m_lastBaseKey = Math.Max(0, (int)Math.Floor(keyBack + (1.0f / 256.0f))); // add something small to compensate for rounding error
            if (keyTime > m_lastBaseKey)
            { // loop back to start & stretch time so it will fit the gap:
                keyTime = (keyTime - keyLength) / (keyLength - m_lastBaseKey);
            }
        }
        else
        { // oneshot - limit time to last key:
            m_lastEndKey = Math.Max(0, (int)Math.Ceiling(keyBack - (1.0f / 256.0f))); // subtract something small to compensate for rounding error
            keyTime = (float)Math.Min(keyTime, keyBack); //, float(lastEndKey));
        }

        int pNewKeyIndex = 0;
        Vector4 keyToInsert = new Vector4();
        int keyInsertIdx = -1;
        uint preRoll = track.mPreRoll;
        bool bSkipping = false;



        while (true)
        {
            // now see where we are in the buffer:
            int bufKeyTime0 = (int)track.mKeyTime0 - kKeyTime_Offset;
            m_intervalsInBuffer = (uint)((kSamplesPerBlock - 1) + (track.mbHasOneMoreKey ? 1 : 0)); // 7 or 8
            if (bufKeyTime0 != kKeyTime_Bad)
            {
                //int bufKey = Floor(keyTime) - bufKeyTime0;
                int bufKey = (int)(Math.Floor(keyTime) - bufKeyTime0);
                if (bufKey >= 0)
                {
                    if (bufKey < m_intervalsInBuffer)
                    { // in range, now interpolate the value:
                        if (!bSkipBlock)
                        {
                            if (pos)
                            {
                                Finish(keyTime, bufKeyTime0, bufKey, track, ref outV);
                            }
                            else
                            {
                                Finish(keyTime, bufKeyTime0, bufKey, track, ref outQ);
                            }
                            return;
                        }
                        // skipping a block without advancing the time:
                        if (bSkipping)
                        { // extrapolate first key to avoid popping after cut:
                            track.mKeysArray[0] = track.mKeysArray[1];
                            if (pos)
                            {
                                Finish(keyTime, bufKeyTime0, bufKey, track, ref outV);
                            }
                            else
                            {
                                Finish(keyTime, bufKeyTime0, bufKey, track, ref outQ);
                            }
                            return; // yuck
                        }
                        bSkipping = true;
                        keyInsertIdx = -1;
                        track.mKeyTime0 -= kSamplesPerBlock;
                        //goto NextBlock;
                        NextBlock(ref pNewKeyIndex, track);
                    }

                    // deal with the exact last key:
                    if (!bLooping
                        && keyTime > keyBack
                        && bufKeyTime0 >= m_lastEndKey - m_intervalsInBuffer)
                    {
                        if (pos)
                        {
                            anim_DCTTrack.KeyFetch(ref outV, ref track.mKeysArray[(int)m_intervalsInBuffer], track);
                        }
                        else
                        {
                            anim_DCTTrack.KeyFetch(ref outQ, ref track.mKeysArray[(int)m_intervalsInBuffer], track);
                        }
                        return;
                    }
                }
            }

            // out of range, buffer new data:

            keyInsertIdx = -1;
            if (bufKeyTime0 == kKeyTime_Bad)
            {
                ColdStart(preRoll, ref pNewKeyIndex, track);
            }
            else
            {
                if (keyTime < 0.0f)
                { // we looped back to the beginning:
                    track.mBitOffset = 0;
                    track.mLastDCCoeff = Vector3.zero;
                    if (preRoll > 0)
                    { // if we have pre-roll, remember last key and copy over data after it's been decoded
                        track.mKeyTime0 = kKeyTime_Offset - preRoll;
                        int srcIdx = (int)(m_lastBaseKey - bufKeyTime0);
                        if (srcIdx >= 0 && srcIdx <= m_intervalsInBuffer)
                        {
                            keyToInsert = track.mKeysArray[srcIdx]; // remember key to insert
                            keyInsertIdx = (int)(preRoll - 1); // remember where to insert
                        }
                        //pNewKeys = &mKeys[0]; // store new data here
                        pNewKeyIndex = 0;
                        track.mbHasOneMoreKey = false;
                    }
                    else
                    { // if no pre-roll, copy to front right away:
                        track.mKeyTime0 = kKeyTime_Offset - 1;
                        int srcIdx = (int)(m_lastBaseKey - bufKeyTime0);
                        if (srcIdx >= 0 && srcIdx <= m_intervalsInBuffer)
                            track.mKeysArray[0] = track.mKeysArray[srcIdx]; // rotate last key to front
                                                                            //pNewKeys = &mKeys[1]; // store new data here					
                        pNewKeyIndex = 1;
                        track.mbHasOneMoreKey = true;
                    }
                }
                else if (keyTime < bufKeyTime0)
                {
                    //goto ColdStart;
                    ColdStart(preRoll, ref pNewKeyIndex, track);
                }
                else
                {
                    NextBlock(ref pNewKeyIndex, track);
                }
            }

            // and decode into buffer:
            int sqrtQuality = track.mSqrtQuality;
            float scale = (float)(kNominalQuality * kNominalQuality) / (float)(kIntScale * sqrtQuality * sqrtQuality);
            uint newBitOffset = DecodeBlock(track.mKeysArray, pNewKeyIndex, ref track.mLastDCCoeff, track.mpData, track.mBitOffset, scale, track.alignmentAdjust);
            //for (int i = 0; i < pNewKeys.Count; ++i)
            //{
            //    mKeys[i] = pNewKeys[i];
            //}
            Debug.Assert(newBitOffset > track.mBitOffset); // look that it didn't wrap around
            track.mBitOffset = newBitOffset;
            if (keyInsertIdx >= 0)
            {
                track.mKeysArray[keyInsertIdx] = keyToInsert;
            }

        }
    }


    public void Finish(float keyTime, float bufKeyTime0, int bufKey, anim_DCTTrack track, ref Vector4 outV)
    {
        float t = keyTime - (float)(bufKeyTime0 + bufKey);
        anim_DCTTrack.KeyInterpolate(ref outV, ref track.mKeysArray[bufKey], ref track.mKeysArray[bufKey + 1], t, track);

    }

    public void Finish(float keyTime, float bufKeyTime0, int bufKey, anim_DCTTrack track, ref Quaternion outV)
    {
        float t = keyTime - (float)(bufKeyTime0 + bufKey);
        anim_DCTTrack.KeyInterpolate(ref outV, ref track.mKeysArray[bufKey], ref track.mKeysArray[bufKey + 1], t, track);

    }

    public void ColdStart(uint preRoll, ref int pNewKeyIndex, anim_DCTTrack track)
    {
        track.mBitOffset = 0;
        track.mLastDCCoeff = Vector3.zero;
        track.mKeyTime0 = kKeyTime_Offset - preRoll;
        //pNewKeys = &mKeys[0];
        pNewKeyIndex = 0;
        track.mbHasOneMoreKey = false;
    }


    public void NextBlock(ref int pNewKeyIndex, anim_DCTTrack track)
    {
        track.mKeyTime0 += m_intervalsInBuffer;
        Debug.Assert(track.mKeyTime0 >= (uint)m_intervalsInBuffer); // look that it didn't wrap around
        track.mKeysArray[0] = track.mKeysArray[(int)m_intervalsInBuffer]; // rotate last key to front
                                                                          //pNewKeys = &mKeys[1]; // store new data behind that
        pNewKeyIndex = 1;
        track.mbHasOneMoreKey = true;

    }


    public static float GetInterp(bool interpolate, float timeDiff, float timeSlice)
    {

        float r;
        if (timeSlice > 0)
        {
            r = interpolate ? timeDiff / timeSlice : 0;
        }
        else
        {
            r = interpolate ? 0.99f : 0.0f;
        }
        if (r > 1)
        {
            r = 1;
        }
        if (r < 0)
        {
            r = 0;
        }
        return r;
    }
}

