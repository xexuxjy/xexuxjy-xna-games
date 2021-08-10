using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTextureTools
{

    public static class Common
    {
        public static char[] versTag = new char[] { 'V', 'E', 'R', 'S' };
        public static char[] cprtTag = new char[] { 'C', 'P', 'R', 'T' };
        public static char[] selsTag = new char[] { 'S', 'E', 'L', 'S' }; // External link information? referes to textures, other models, entities and so on? 
        public static char[] cntrTag = new char[] { 'C', 'N', 'T', 'R' };
        public static char[] shdrTag = new char[] { 'S', 'H', 'D', 'R' };
        public static char[] txtrTag = new char[] { 'T', 'X', 'T', 'R' };
        //static char[] paddTag = new char[] { 'P', 'A', 'D', 'D' };
        public static char[] dslsTag = new char[] { 'D', 'S', 'L', 'S' };   // DisplayList information - int16 pairs of position,normal,uv0 - possible that some uv's also used for weights?
        public static char[] dsliTag = new char[] { 'D', 'S', 'L', 'I' };   // display list -offsets and lengths
        public static char[] dslcTag = new char[] { 'D', 'S', 'L', 'C' };   // seems to contain the number of display lists and then bytes at 01 to say used?
        public static char[] posiTag = new char[] { 'P', 'O', 'S', 'I' };
        public static char[] normTag = new char[] { 'N', 'O', 'R', 'M' };
        public static char[] uv0Tag = new char[] { 'U', 'V', '0', ' ' };    // for non-skinned models, this seems to be 2 bigendian float32's
        public static char[] uv1Tag = new char[] { 'U', 'V', '1', ' ' };    // for non-skinned models, this seems to be 2 bigendian float32's

        public static char[] vflaTag = new char[] { 'V', 'F', 'L', 'A' };
        public static char[] ramTag = new char[] { 'R', 'A', 'M', ' ' };
        public static char[] msarTag = new char[] { 'M', 'S', 'A', 'R' };
        public static char[] nlvlTag = new char[] { 'N', 'L', 'V', 'L' };
        public static char[] meshTag = new char[] { 'M', 'E', 'S', 'H' };   // how many mesh segments exist. each block is 24 bytes
        public static char[] elemTag = new char[] { 'E', 'L', 'E', 'M' };   // how many elements , matches mesh segments, each block is 8 bytes,
        public static char[] skelTag = new char[] { 'S', 'K', 'E', 'L' };
        public static char[] skinTag = new char[] { 'S', 'K', 'I', 'N' };
        public static char[] nameTag = new char[] { 'N', 'A', 'M', 'E' };
        public static char[] vflgTag = new char[] { 'V', 'F', 'L', 'G' };
        public static char[] stypTag = new char[] { 'S', 'T', 'Y', 'P' };
        public static char[] pak1Tag = new char[] { 'P', 'A', 'K', '1' };
        public static char[] jlodTag = new char[] { 'J', 'L', 'O', 'D' };
        public static char[] nmptTag = new char[] { 'N', 'M', 'P', 'T' };
        public static char[] pttpTag = new char[] { 'P', 'T', 'T', 'P' };
        public static char[] r2d2Tag = new char[] { 'R', '2', 'D', '2', 'p', 's', 'x', '2' };
        public static char[] pfhdTag = new char[] { 'P', 'F', 'H', 'D' };
        public static char[] ptdtTag = new char[] { 'P', 'T', 'D', 'T' };
        public static char[] tmapTag = new char[] { 't', 'm', 'a', 'p' };

        public static char[] xrndTag = new char[] { 'X', 'R', 'N', 'D' };
        public static char[] doegTag = new char[] { 'd', 'o', 'e', 'g' };
        public static char[] endTag = new char[] { 'E', 'N', 'D', (char)0x2E };
        public static char[] obbtTag = new char[] { 'O', 'B', 'B', 'T' };
        //public static char[] endTag = new char[] { (char)0x3F,'E', 'N', 'D'};


        public static char[][] allTags = { versTag, cprtTag, selsTag, cntrTag, shdrTag, txtrTag,
                                      dslsTag, dsliTag, dslcTag, posiTag, normTag, uv0Tag, vflaTag,
                                      ramTag, msarTag, nlvlTag, meshTag, elemTag, skelTag, skinTag,
                                      vflgTag,stypTag,nameTag };


        public static bool FindCharsInStream(BinaryReader binReader, char[] charsToFind, bool resetPositionIfNotFound = true)
        {
            bool found = false;
            byte b = (byte)' ';
            int lastFoundIndex = 0;
            long currentPosition = binReader.BaseStream.Position;
            try
            {
                while (true)
                {
                    b = binReader.ReadByte();
                    if (b == charsToFind[lastFoundIndex])
                    {
                        lastFoundIndex++;
                        if (lastFoundIndex == charsToFind.Length)
                        {
                            found = true;
                            break;
                        }
                    }
                    else
                    {
                        lastFoundIndex = 0;
                    }
                }
            }
            catch (Exception e)
            {
            }
            if (!found && resetPositionIfNotFound)
            {
                binReader.BaseStream.Position = currentPosition;
            }


            return found;

        }

        public static bool FindCharsInStream(BinaryReader binReader, char[] charsToFind, long endRange, bool resetPositionIfNotFound = true)
        {
            bool found = false;
            byte b = (byte)' ';
            int lastFoundIndex = 0;
            long currentPosition = binReader.BaseStream.Position;
            try
            {
                while (true)
                {
                    if (binReader.BaseStream.Position >= endRange)
                    {
                        found = false;
                        break;
                    }
                    b = binReader.ReadByte();
                    if (b == charsToFind[lastFoundIndex])
                    {
                        lastFoundIndex++;
                        if (lastFoundIndex == charsToFind.Length)
                        {
                            found = true;
                            break;
                        }
                    }
                    else
                    {
                        lastFoundIndex = 0;
                    }
                }
            }
            catch (Exception e)
            {
            }
            if (!found && resetPositionIfNotFound)
            {
                binReader.BaseStream.Position = currentPosition;
            }


            return found;

        }



        public static bool FindCharsInStream(BinaryReader binReader, byte[] charsToFind, bool resetPositionIfNotFound = true)
        {
            bool found = false;
            byte b = (byte)' ';
            int lastFoundIndex = 0;
            long currentPosition = binReader.BaseStream.Position;
            try
            {
                while (true)
                {
                    b = binReader.ReadByte();
                    if (b == charsToFind[lastFoundIndex])
                    {
                        lastFoundIndex++;
                        if (lastFoundIndex > 2)
                        {
                            int ibreak = 0;
                        }
                        if (lastFoundIndex == charsToFind.Length)
                        {
                            found = true;
                            break;
                        }
                    }
                    else
                    {
                        binReader.BaseStream.Position -= lastFoundIndex;
                        lastFoundIndex = 0;
                    }
                }
            }
            catch (Exception e)
            {
            }
            if (!found && resetPositionIfNotFound)
            {
                binReader.BaseStream.Position = currentPosition;
            }


            return found;

        }





        public static void ReadNullSeparatedNames(BinaryReader binReader, char[] tagName, List<String> selsNames)
        {
            if (Common.FindCharsInStream(binReader, tagName))
            {
                int selsSectionLength = binReader.ReadInt32();

                int pad = binReader.ReadInt32();
                int pad2 = binReader.ReadInt32();

                selsSectionLength -= 16;

                StringBuilder sb = new StringBuilder();

                char b;
                int count = 0;
                while (count < selsSectionLength)
                {
                    while ((b = (char)binReader.ReadByte()) != 0x00)
                    {
                        count++;
                        sb.Append(b);
                    }
                    count++;
                    if (sb.Length > 0)
                    {
                        selsNames.Add(sb.ToString());
                    }
                    sb = new StringBuilder();
                }
            }
        }

        public static void ReadNullSeparatedNames(BinaryReader binReader, long position, int numAnims, List<String> selsNames)
        {
            binReader.BaseStream.Position = position;
            StringBuilder sb = new StringBuilder();
            char b;
            int count = 0;
            while (selsNames.Count < numAnims)
            {
                while ((b = (char)binReader.ReadByte()) != 0x00)
                {
                    count++;
                    sb.Append(b);
                }
                count++;
                if (sb.Length > 0)
                {
                    selsNames.Add(sb.ToString());
                }
                sb = new StringBuilder();
            }
        }



    }

}
