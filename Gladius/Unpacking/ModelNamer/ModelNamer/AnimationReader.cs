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

        public static void DumpAllSectionData(String path, String outputPath)
        {
            String[] files = Directory.GetFiles(path, "*");

            foreach (String file in files)
            {
                try
                {
                    FileInfo sourceFile = new FileInfo(file);
                    AnimationReader animationReader = new AnimationReader();
                    animationReader.DumpSectionData(sourceFile, outputPath);
                }
                catch { }
             //   break;
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
                    infoStream.WriteLine(String.Format("\t {0} : {1}", new String(tag), animData.m_tagSizes[tag].length));
                }
            }
        }

        public void DumpSectionData(FileInfo fileInfo, String outputPath)
        {

            using (BinaryReader binReader = new BinaryReader(new FileStream(fileInfo.FullName, FileMode.Open)))
            {
                Common.FindCharsInStream(binReader, pak1Tag);
                int numAnimations = binReader.ReadInt32();
                int animNameStart = binReader.ReadInt32();
                binReader.BaseStream.Position = animNameStart;
                Common.ReadNullSeparatedNames(binReader, animNameStart, numAnimations, animNames);
                if (numAnimations > 100)
                {
                    //infoStream.WriteLine(filename + " has too many anmims : " + numAnimations);
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


            //infoStream.WriteLine("Filename : " + filename);

            if (animations.Count > 0)
            {
                String animationTargetName = animations[0].animationName.Substring(0, animations[0].animationName.IndexOf("_"));
                if (String.IsNullOrEmpty(animationTargetName))
                {
                    animationTargetName = fileInfo.Name;
                }


                String fileOutputDir = outputPath + "/" + animationTargetName;
                Directory.CreateDirectory(fileOutputDir);
                //using (System.IO.StreamWriter infoStream = new System.IO.StreamWriter(infoFile))


                foreach (AnimationData animData in animations)
                {

                    foreach (char[] tag in animData.m_tagSizes.Keys)
                    {
                        try
                        {
                            TagSizeAndData tsad = animData.m_tagSizes[tag];
                            if (tsad.length > 0)
                            {
                                String shortAnimName = animData.animationName.Substring(animData.animationName.IndexOf("_") + 1);

                                
                                String tagOutputDirname = fileOutputDir + "/" +  new String(tag);
                                try
                                {
                                    Directory.CreateDirectory(tagOutputDirname);
                                }
                                catch (Exception e) { }

                                String tagOutputFilename = tagOutputDirname + "/" + (shortAnimName);
                                using (System.IO.BinaryWriter outStream = new BinaryWriter(File.Open(tagOutputFilename, FileMode.Create)))
                                {
                                    outStream.Write(tsad.data);
                                }
                            }
                        }
                        catch (Exception e)
                        { }
                    }
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
            //AnimationReader.DumpAllSectionLengths("c:/tmp/unpacking/PAK1/PAK1", "c:/tmp/unpacking/PAK1/header-results.txt");

            AnimationReader.DumpAllSectionData("c:/tmp/unpacking/PAK1/PAK1", "c:/tmp/unpacking/PAK1/SplitOutput");
        }

    }

    public class TagSizeAndData
    {
        public static TagSizeAndData Create(BinaryReader reader)
        {
            int length = reader.ReadInt32();
            TagSizeAndData t = new TagSizeAndData(length);
            reader.BaseStream.Position -= 8;
            t.data = reader.ReadBytes(t.length);
            return t;

        }

        public TagSizeAndData(int len)
        {
            length = len;
        }

        public int length;
        public byte[] data;
    }


    public class AnimationData
    {
        public String animationName;
        List<string> boneList = new List<string>();
        List<float> timeStepList = new List<float>();
        List<int> boolList = new List<int>();
        public Dictionary<char[], TagSizeAndData> m_tagSizes = new Dictionary<char[], TagSizeAndData>();

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
                    TagSizeAndData tsad = TagSizeAndData.Create(binReader);
                    
                    m_tagSizes[tag] = tsad;
                    //if(tag == AnimationReader.bktmTag)
                    //{
                    //    int pad = binReader.ReadInt32();
                    //    int numTime = binReader.ReadInt32();
                    //    // numt values seems to correspond so (4*numt) +16 = bktmLength
                    //    m_tagSizes[new char[]{'n','u','m','t'}] = numTime;
                    //}
                    //if (tag == AnimationReader.dcrtTag)
                    //{
                    //    int pad = binReader.ReadInt32();
                    //    int numBones= binReader.ReadInt32();
                    //    m_tagSizes[new char[] { 'b', 'o', 'n', 'e' }] = numBones;
                    //}


                    //infoStream.WriteLine(String.Format("\t {0} : {1}", new String(tag), tsad.length));
                }
                else
                {
                    m_tagSizes[tag] = new TagSizeAndData(-1);
                }
            }

        }
    }
}
