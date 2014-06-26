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
            String sourceDirectory = @"c:\gladius-extracted\ps2-decompressed\";
            String infoFile = @"c:\gladius-extracted\ps2-decompressed\nbTesterResult.txt";

            using (System.IO.StreamWriter infoStream = new System.IO.StreamWriter(infoFile))
            {
                using (BinaryReader binReader = new BinaryReader(new FileStream(sourceDirectory+"data-test1.bin", FileMode.Open)))
                {
                    try
                    {
                        while (true)
                        {
                            byte b = binReader.ReadByte();
                            float nvb = (b / (127.5f)) + 1.0f;
                            infoStream.WriteLine(String.Format("Converting [{0:X}] to [{1:0.00000}]", b, nvb));
                        }
                    }
                    catch (System.Exception ex)
                    {
                    }
                }
            }
        }

        public void RunTest2()
        {
            String sourceDirectory = @"C:\tmp\unpacking\VERSModelFilesRenamed\";
            String infoFile = @"C:\tmp\unpacking\nbResults2.txt";

            //String filename = "move_select.mdl";
            //String filename = "nordagh_tree.mdl";
            //String filename = "imperia_wall.mdl";
            //String filename = "jewel_coral.mdl";
            //String filename = "propivy4.mdl";
            //String filename = "staff_bo.mdl";
            //String filename = "rune_algiz.mdl";
            String filename = "carafe_flask.mdl";
            byte[] scratch = new byte[4];
            using (System.IO.StreamWriter infoStream = new System.IO.StreamWriter(infoFile))
            {
                using (BinaryReader binReader = new BinaryReader(new FileStream(sourceDirectory + filename, FileMode.Open)))
                {
                    try
                    {
                        Common.FindCharsInStream(binReader, R2V2.tag);
                        binReader.BaseStream.Position -= R2V2.tag.Length;
                        R2V2 r2v2 = R2V2.FromStream(binReader);

                        //binReader.BaseStream.Position = 0x344;
                        binReader.BaseStream.Position -= 1;

                        //binReader.BaseStream.Position = 0x349;

                        int uvcounter = 0;
                        while (binReader.Read(scratch, 0, scratch.Length)==scratch.Length)
                        {
                            //float f = Common.ReadSingleBigEndian(scratch, 0);
                            float f = System.BitConverter.ToSingle(scratch, 0);
                            infoStream.WriteLine(String.Format("Writing  [{0}]", f));

                            //int i16x = binReader.ReadInt16();
                            ////float nvbx = (i16x / 65536f);
                            ////float nvbx = (i16x / (float)short.MaxValue);
                            //float nvbx = (i16x / (float)16384);
                            ////float nvbx = (i16x / (float)4096f);
                            ////float nvbx = (i16x / (float)8192f);
                            //if (nvbx >= -1f && nvbx <= 1f)
                            //{
                            //    uvcounter++;
                            //}
                            //else
                            //{
                            //    //if (uvcounter/2 == r2v2.numVertices)
                            //    if(uvcounter > 3)
                            //    {
                            //        infoStream.WriteLine(String.Format("Got a possible run of [{0}] UV's [{1}] , starting at [{2}]",uvcounter,r2v2.numVertices,binReader.BaseStream.Position-((uvcounter+1)*2)));
                            //    }
                            //    uvcounter = 0;

                            //}
                                     

                            //int i16y = binReader.ReadInt16();
                            //float nvby = (i16y / 65536f);
                            //int i16z = binReader.ReadInt16();
                            //float nvbz = (i16z / 65536f);

                            //sVector3 sv3 = new sVector3(nvbx, nvby, nvbz);
                            //if (Common.FuzzyEquals(sv3.Len2, 1f))
                            //{
                            //    infoStream.WriteLine(String.Format("Possible normal : [{0:0.0000000}][{1:0.0000000}][{2:0.0000000}]", nvbx, nvby, nvbz));
                            //}
                                

                            //infoStream.WriteLine(String.Format("Converting  [{0:x} [{1}] to [{2:0.0000000}]", binReader.BaseStream.Position-2,i16x, nvbx));
                        }
                    }
                    catch (System.Exception ex)
                    {
                    }
                }
            }
        }


        static void Main(string[] args)
        {
            new NormalisedByteTester().RunTest2();

        }

    }
}
