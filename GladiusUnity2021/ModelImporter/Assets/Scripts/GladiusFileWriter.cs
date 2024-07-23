using System.Collections.Generic;
using System.IO;
using System.Text;

public static class GladiusFileWriter
{
    public const int HeaderSize = 16;
    public const int AlignmentValue = 16;

    
    public static void WriteNull(BinaryWriter writer, int num)
    {
        for (int i = 0; i < num; ++i)
        {
            writer.Write((byte)0);
        }
    }

    public static void WriteNullPaddedString(BinaryWriter writer, string str, int requiredLength)
    {
        writer.Write(str);
        WriteNull(writer, requiredLength - str.Length);
    }

    public static void WriteStringList(BinaryWriter writer, List<string> list, int totalLength)
    {
        int ongoingTotal = 0;
        for (int i = 0; i < list.Count; ++i)
        {
            ongoingTotal += list[i].Length;
            WriteASCIIString(writer, list[i]);
            if (i < list.Count - 1)
            {
                writer.Write((byte)0x00);
                ongoingTotal += 1;
            }
        }

        while (ongoingTotal < totalLength)
        {
            writer.Write((byte)0x00);
            ongoingTotal++;
        }
    }

    public static int GetStringListSize(List<string> list)
    {
        int total = 0;
        for (int i = 0; i < list.Count; ++i)
        {
            total += list[i].Length;
            if (i < list.Count - 1)
            {
                total += 1;
            }
        }

        return total; //+pad;
    }

    public static void WriteASCIIString(BinaryWriter writer, string s, int padToLength = 0)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(s);
        writer.Write(bytes);
        if (padToLength > 0)
        {
            int pad = padToLength - bytes.Length;
            for (int i = 0; i < pad; ++i)
            {
                writer.Write((byte)0x00);
            }
        }
    }

    public static void WriteVERS(BinaryWriter writer)
    {
        int total = HeaderSize + 8;
        WriteASCIIString(writer, "VERS");
        // header size
        writer.Write(total);
        writer.Write(0);
        writer.Write(1);
        writer.Write(0);
        writer.Write(14);
        // writer.Write(0);
        // writer.Write(0);
    }

    public static void WriteCPRT(BinaryWriter writer)
    {
        int blockSize = 0x90;
        int remain = blockSize;
        WriteASCIIString(writer, "CPRT");
        writer.Write(blockSize);
        writer.Write(0x00);
        writer.Write(0x80);

        remain -= GladiusFileWriter.HeaderSize;
        
        string s = "(C) Mar 21 2003 LucasArts Entertainment LLC";
        remain -= s.Length;

        WriteASCIIString(writer, s);
        WriteNull(writer, remain);
    }


    public static int GetPadValue(int total)
    {
        int pad = total % AlignmentValue;
        if (pad != 0)
        {
            total += (AlignmentValue - pad);
        }

        return total;
    }

    public static int GetPadValue(int total,int alignmentValue)
    {
        int pad = total % alignmentValue;
        if (pad != 0)
        {
            total += (alignmentValue - pad);
        }

        return total;
    }

    
    public static void PadIfNeeded(BinaryWriter writer)
    {
        //return;
        int padValue = 64;
        if (writer.BaseStream.Position % padValue == 0)
        {
            WritePADD(writer);
        }
    }
    
    public static void WritePADD(BinaryWriter writer)
    {
        WriteASCIIString(writer, "PADD");
        writer.Write(HeaderSize);
        writer.Write(0x00);
        writer.Write(0x00);
    }

    public static void WriteEND(BinaryWriter writer)
    {
        GladiusFileWriter.WriteASCIIString(writer, "END.");
        writer.Write(GladiusFileWriter.HeaderSize); // number of elements.
        writer.Write(0);
        writer.Write(0);
    }


    
}