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
        public static char[] selsTag = new char[] { 'S', 'E', 'L', 'S' }; // External link information? referes to textures, other models, entities and so on? 
        public static char[] cntrTag = new char[] { 'C', 'N', 'T', 'R' };
        public static char[] shdrTag = new char[] { 'S', 'H', 'D', 'R' };
        public static char[] txtrTag = new char[] { 'T', 'X', 'T', 'R' };
        //static char[] paddTag = new char[] { 'P', 'A', 'D', 'D' };
        public static char[] dslsTag = new char[] { 'D', 'S', 'L', 'S' };  // DisplayList information
        public static char[] dsliTag = new char[] { 'D', 'S', 'L', 'I' };
        public static char[] dslcTag = new char[] { 'D', 'S', 'L', 'C' };
        public static char[] posiTag = new char[] { 'P', 'O', 'S', 'I' };
        public static char[] normTag = new char[] { 'N', 'O', 'R', 'M' };
        public static char[] uv0Tag = new char[] { 'U', 'V', '0', ' ' };
        public static char[] vflaTag = new char[] { 'V', 'F', 'L', 'A' };
        public static char[] ramTag = new char[] { 'R', 'A', 'M', ' ' };
        public static char[] msarTag = new char[] { 'M', 'S', 'A', 'R' };
        public static char[] nlvlTag = new char[] { 'N', 'L', 'V', 'L' };
        public static char[] meshTag = new char[] { 'M', 'E', 'S', 'H' };
        public static char[] elemTag = new char[] { 'E', 'L', 'E', 'M' };
        public static char[] skelTag = new char[] { 'S', 'K', 'E', 'L' };
        public static char[] skinTag = new char[] { 'S', 'K', 'I', 'N' };
        public static char[] nameTag = new char[] { 'N', 'A', 'M', 'E' };
        public static char[] vflgTag = new char[] { 'V', 'F', 'L', 'G' };
        public static char[] stypTag = new char[] { 'S', 'T', 'Y', 'P' };


        public static char[][] allTags = { versTag, cprtTag, selsTag, cntrTag, shdrTag, txtrTag, 
                                      dslsTag, dsliTag, dslcTag, posiTag, normTag, uv0Tag, vflaTag, 
                                      ramTag, msarTag, nlvlTag, meshTag, elemTag, skelTag, skinTag,
                                      vflgTag,stypTag,nameTag };



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

        public List<string> animNames = new List<string>();
        public List<AnimationData> animations = new List<AnimationData>();



        static void Main(string[] args)
        {
            String filename = "c:/tmp/unpacking/PAK1/PAK1/File 004853";
            AnimationReader animationReader = new AnimationReader();
            using (BinaryReader binReader = new BinaryReader(new FileStream(filename, FileMode.Open)))
            {
                animationReader.Read(binReader);
            }
            int ibreak = 0;
        }

    }


    public class AnimationData
    {
        String animationName;
        List<string> boneList = new List<string>();


        public static bool FromStream(BinaryReader reader, out AnimationData animationData)
        {
            animationData = new AnimationData();
            Debug.Assert(Common.FindCharsInStream(reader, AnimationReader.versTag));
            Debug.Assert(Common.FindCharsInStream(reader, AnimationReader.cprtTag));
            Debug.Assert(Common.FindCharsInStream(reader, AnimationReader.hedrTag));

            Common.ReadNullSeparatedNames(reader, AnimationReader.nameTag, animationData.boneList);



            return true;
        }

    }

}
