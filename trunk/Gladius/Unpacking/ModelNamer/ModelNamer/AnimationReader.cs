using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ModelNamer
{
    public class AnimationReader
    {

        public static char[] pak1Tag = new char[] { 'P', 'A', 'K', '1' };
        public static char[] hedrTag = new char[] { 'H', 'E', 'D', 'R' };
        public static char[] versTag = new char[] { 'V', 'E', 'R', 'S' };
        public static char[] cprtTag = new char[] { 'C', 'P', 'R', 'T' };
        public static char[] blnmTag = new char[] { 'B', 'L', 'N', 'M' };
        public static char[] maskTag = new char[] { 'M', 'A', 'S', 'K' };
        public static char[] bltkTag = new char[] { 'B', 'L', 'T', 'K' };
        public static char[] bktmTag = new char[] { 'B', 'K', 'T', 'M' };
        public static char[] boolTag = new char[] { 'B', 'O', 'O', 'L' };
        public static char[] dcrtTag = new char[] { 'D', 'C', 'R', 'T' };
        public static char[] dcrdTag = new char[] { 'D', 'C', 'R', 'D' };
        public static char[] dcptTag = new char[] { 'D', 'C', 'P', 'T' };
        public static char[] dcpdTag = new char[] { 'D', 'C', 'P', 'D' };

        public static char[] nameTag = new char[] { 'N', 'A', 'M', 'E' };

        public static char[] bonesTag = new char[] { 'N', 'A', 'M', 'E' };


        public static char[][] allTags = { pak1Tag, hedrTag, versTag, cprtTag, blnmTag, maskTag, bltkTag, bktmTag, boolTag, dcrtTag, dcrdTag,dcptTag, dcpdTag, nameTag };
//        public static char[][] subTags = { versTag, cprtTag, hedrTag, nameTag ,blnmTag, maskTag, bltkTag, bktmTag, boolTag, dcrtTag, dcrdTag, dcptTag, dcpdTag };
        public static char[][] subTags = { versTag, cprtTag, hedrTag, nameTag, blnmTag, maskTag, bltkTag, bktmTag, boolTag, dcrtTag, dcrdTag};



        public void Read(BinaryReader binReader)
        {
            Common.FindCharsInStream(binReader, pak1Tag);
            int numAnimations = binReader.ReadInt32();
            int animNameStart = binReader.ReadInt32();
            binReader.BaseStream.Position = animNameStart;
            Common.ReadNullSeparatedNames(binReader, animNameStart, numAnimations,animNames);

            for (int i = 0; i < numAnimations; ++i)
            {
                AnimationData animationData;
                if (AnimationData.FromStream(binReader, out animationData))
                {
                    animations.Add(animationData);
                }
            }
        }

        public static void DumpAllSectionLengths(String path,String infoFile)
        {
            String[] files = Directory.GetFiles(path, "*");

            using (System.IO.StreamWriter infoStream = new System.IO.StreamWriter(infoFile))
            {
                foreach (String file in files)
                {
                    try
                    {
                        FileInfo sourceFile = new FileInfo(file);
                        AnimationReader animationReader = new AnimationReader();
                        animationReader.DumpSectionLengths(sourceFile.FullName, infoStream);
                    }
                    catch { }
                }
            }
        }

        public void DumpSectionLengths(String filename,StreamWriter infoStream)
        {

            using (BinaryReader binReader = new BinaryReader(new FileStream(filename, FileMode.Open)))
            {
                Common.FindCharsInStream(binReader, pak1Tag);
                int numAnimations = binReader.ReadInt32();
                int animNameStart = binReader.ReadInt32();
                binReader.BaseStream.Position = animNameStart;
                Common.ReadNullSeparatedNames(binReader, animNameStart, numAnimations, animNames);
                if (numAnimations > 100)
                {
                    infoStream.WriteLine(filename + " has too many anmims : " + numAnimations);
                }
                else
                {

                    for (int i = 0; i < numAnimations; ++i)
                    {
                        AnimationData animationData = new AnimationData();
                        animationData.animationName = animNames[i];
                        animations.Add(animationData);
                        animationData.BuildTagInfo(binReader);
                    }
                }
            }

            infoStream.WriteLine("Filename : " + filename);

            foreach (AnimationData animData in animations)
            {
                infoStream.WriteLine("Anim : " + animData.animationName);
                foreach (char[] tag in animData.m_tagSizes.Keys)
                {
                    infoStream.WriteLine(String.Format("\t {0} : {1}", new String(tag), animData.m_tagSizes[tag]));
                }
            }
        }


        public List<string> animNames = new List<string>();
        public List<AnimationData> animations = new List<AnimationData>();







        static void Main(string[] args)
        {
            //String filename = "c:/tmp/unpacking/PAK1/PAK1/File 004853";
            //AnimationReader animationReader = new AnimationReader();
            //using (BinaryReader binReader = new BinaryReader(new FileStream(filename, FileMode.Open)))
            //{
            //    animationReader.Read(binReader);
            //}
            //int ibreak = 0;
            AnimationReader.DumpAllSectionLengths("c:/tmp/unpacking/PAK1/PAK1", "c:/tmp/unpacking/PAK1/header-results.txt");
        }

    }


    public class AnimationData
    {
        public String animationName;
        List<string> boneList = new List<string>();
        List<float> timeStepList = new List<float>();
        List<int> boolList = new List<int>();
        public Dictionary<char[], int> m_tagSizes = new Dictionary<char[], int>();

        public static bool FromStream(BinaryReader reader, out AnimationData animationData)
        {
            animationData = new AnimationData();
            Debug.Assert(Common.FindCharsInStream(reader, AnimationReader.versTag));
            Debug.Assert(Common.FindCharsInStream(reader, AnimationReader.cprtTag));
            Debug.Assert(Common.FindCharsInStream(reader, AnimationReader.hedrTag));

            Common.ReadNullSeparatedNames(reader, AnimationReader.nameTag, animationData.boneList);

            // bktm section
            if (Common.FindCharsInStream(reader, AnimationReader.bktmTag))
            {
                int sectionLength = reader.ReadInt32();
                int pad1 = reader.ReadInt32();
                int numTimeSteps = reader.ReadInt32();
                for (int i = 0; i < numTimeSteps; ++i)
                {
                    animationData.timeStepList.Add(reader.ReadSingle());
                }
            }

            if (Common.FindCharsInStream(reader, AnimationReader.boolTag))
            {
                int sectionLength = reader.ReadInt32();
                int pad1 = reader.ReadInt32();
                int numTimeSteps = reader.ReadInt32();
                // based on that. each entry is 4 bytes. not sure wha tthey are
                // though as mainly 0,1 (though not always)
                for (int i = 0; i < numTimeSteps; ++i)
                {
                    animationData.boolList.Add(reader.ReadInt32());
                }


            }

            if (Common.FindCharsInStream(reader, AnimationReader.dcrtTag))
            {
                int sectionLength = reader.ReadInt32();
                int pad1 = reader.ReadInt32();
                int numBones = reader.ReadInt32();
                // each entry here is 8? bytes


            }

            if (Common.FindCharsInStream(reader, AnimationReader.dcrdTag))
            {
                int sectionLength = reader.ReadInt32();
                int pad1 = reader.ReadInt32();
                int numBones = reader.ReadInt32();
                // each entry here is 16 bytes
            }

            // These 2 seem optional, but always exist together.

            if (Common.FindCharsInStream(reader, AnimationReader.dcptTag))
            {
                int sectionLength = reader.ReadInt32();
                int pad1 = reader.ReadInt32();
                int numBones = reader.ReadInt32();
                // each entry here is 16 bytes
            }

            if (Common.FindCharsInStream(reader, AnimationReader.dcpdTag))
            {
                int sectionLength = reader.ReadInt32();
                int pad1 = reader.ReadInt32();
                int numBones = reader.ReadInt32();
                // each entry here is 16 bytes
                

            }




            return true;
        }

        public void BuildTagInfo(BinaryReader binReader)
        {

            foreach (char[] tag in AnimationReader.subTags)
            {
                // reset for each so we don't worry about order
                
                if (Common.FindCharsInStream(binReader, tag, true))
                {
                    int blockSize = binReader.ReadInt32();
                    m_tagSizes[tag] = blockSize;
                    //infoStream.WriteLine(String.Format("\t {0} : {1}", new String(tag), blockSize));
                }
                else
                {
                    m_tagSizes[tag] = -1;
                }
            }

        }
    }
}
