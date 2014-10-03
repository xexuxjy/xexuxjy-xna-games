using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ModelNamer
{
    public class NormalisedByteTester
    {
        public void RunTest()
        {
            String sourceFile = @"D:\gladius-extracted-archive\gc-compressed\test-models\tag-ouput\Bow.mdl\UV0 ";
            String infoFile = @"D:\gladius-extracted-archive\gc-compressed\test-models\tag-ouput\Bow.mdl\info.txt";

            using (System.IO.StreamWriter infoStream = new System.IO.StreamWriter(infoFile))
            {
                using (BinaryReader binReader = new BinaryReader(new FileStream(sourceFile, FileMode.Open)))
                {
                    try
                    {
                        binReader.BaseStream.Position = 0x10;
                        ushort[] holder = new ushort[2];
                        float[] fholder = new float[2];
                        //int stride = 9 - 3;
                        int stride = 0;
                        while (true)
                        {
                            for (int i = 0; i < holder.Length; ++i)
                            {
                                fholder[i] = Common.ToFloatUInt16(binReader);
                                //holder[i] = Common.ToInt16BigEndian(binReader);
                                //holder[i] = binReader.ReadUInt16();
                                //fholder[i] = (holder[i] / (127.5f)) + 1.0f;
                                //fholder[i] = (holder[i] / (float)(UInt16.MaxValue));
                            }

                            infoStream.WriteLine(String.Format("[{0},{1}]]",fholder[0],fholder[1]));
                            binReader.BaseStream.Position += stride;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        infoStream.Flush();
                    }
                }
            }
        }

        public void RunTest2()
        {
            String sourceDirectory = @"D:\gladius-extracted\ps2-decompressed\PAK1Animations\";
            String infoFile = @"d:\nbResults2.txt";

            //String filename = "move_select.mdl";
            //String filename = "nordagh_tree.mdl";
            //String filename = "imperia_wall.mdl";
            //String filename = "jewel_coral.mdl";
            //String filename = "propivy4.mdl";
            //String filename = "staff_bo.mdl";
            //String filename = "rune_algiz.mdl";
            String filename = "File_005101";
            byte[] scratch = new byte[4];
            using (System.IO.StreamWriter infoStream = new System.IO.StreamWriter(infoFile))
            {
                using (BinaryReader binReader = new BinaryReader(new FileStream(sourceDirectory + filename, FileMode.Open)))
                {
                    try
                    {
                        char[] tag = new char[] { 'D', 'C', 'R', 'D' };
                        
                        Common.FindCharsInStream(binReader, tag);
                        int headerLength = binReader.ReadInt32();
                        int pad = binReader.ReadInt32();
                        int foo = binReader.ReadInt32();

                        while (true)
                        {
                            int val1 = Common.ToInt16BigEndian(binReader);
                            float v = ((float)val1)/32767;
                            int a = 0;
                        }


                        //int uvcounter = 0;
                        //while (binReader.Read(scratch, 0, scratch.Length)==scratch.Length)
                        //{
                        //    //float f = Common.ReadSingleBigEndian(scratch, 0);
                        //    float f = System.BitConverter.ToSingle(scratch, 0);
                        //    infoStream.WriteLine(String.Format("Writing  [{0}]", f));

                        //    //int i16x = binReader.ReadInt16();
                        //    ////float nvbx = (i16x / 65536f);
                        //    ////float nvbx = (i16x / (float)short.MaxValue);
                        //    //float nvbx = (i16x / (float)16384);
                        //    ////float nvbx = (i16x / (float)4096f);
                        //    ////float nvbx = (i16x / (float)8192f);
                        //    //if (nvbx >= -1f && nvbx <= 1f)
                        //    //{
                        //    //    uvcounter++;
                        //    //}
                        //    //else
                        //    //{
                        //    //    //if (uvcounter/2 == r2v2.numVertices)
                        //    //    if(uvcounter > 3)
                        //    //    {
                        //    //        infoStream.WriteLine(String.Format("Got a possible run of [{0}] UV's [{1}] , starting at [{2}]",uvcounter,r2v2.numVertices,binReader.BaseStream.Position-((uvcounter+1)*2)));
                        //    //    }
                        //    //    uvcounter = 0;

                        //    //}
                                     

                        //    //int i16y = binReader.ReadInt16();
                        //    //float nvby = (i16y / 65536f);
                        //    //int i16z = binReader.ReadInt16();
                        //    //float nvbz = (i16z / 65536f);

                        //    //sVector3 sv3 = new sVector3(nvbx, nvby, nvbz);
                        //    //if (Common.FuzzyEquals(sv3.Len2, 1f))
                        //    //{
                        //    //    infoStream.WriteLine(String.Format("Possible normal : [{0:0.0000000}][{1:0.0000000}][{2:0.0000000}]", nvbx, nvby, nvbz));
                        //    //}
                                

                        //    //infoStream.WriteLine(String.Format("Converting  [{0:x} [{1}] to [{2:0.0000000}]", binReader.BaseStream.Position-2,i16x, nvbx));
                        //}
                    }
                    catch (System.Exception ex)
                    {
                    }
                }
            }
        }


        static void Main(string[] args)
        {
            new NormalisedByteTester().RunTest();

        }

    }
}
