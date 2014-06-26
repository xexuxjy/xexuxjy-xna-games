using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ModelNamer
{
    public class R2V2Analyzer
    {

        public void Process()
        {
            String sourceDirectory = @"c:\tmp\unpacking\VERSModelFilesRenamed\";
            String infoFile = @"c:\tmp\unpacking\R2V2Result.txt";

          
            char[] r2v2Tag = new char[] { 'r', '2', 'v', '2'};
            String[] files = Directory.GetFiles(sourceDirectory, "*");

            List<R2V2> fileData = new List<R2V2>();

            using (System.IO.StreamWriter infoStream = new System.IO.StreamWriter(infoFile))
            {
                foreach (String file in files)
                {
                    try
                    {
                        FileInfo sourceFile = new FileInfo(file);
                        if (sourceFile.Name != "File_000376")
                        {
                            //continue;
                        }

                        using (BinaryReader binReader = new BinaryReader(new FileStream(sourceFile.FullName, FileMode.Open)))
                        {
                            fileData.Clear();
                            int counter = 0;
                            infoStream.WriteLine(String.Format("{0}", sourceFile.Name));
                            while (Common.FindCharsInStream(binReader, r2v2Tag, true))
                            {
                                binReader.BaseStream.Position -= 4;

                                R2V2 r2v2 = R2V2.FromStream(binReader);
                                r2v2.FileName = sourceFile.Name;
                                r2v2.fileSize = (int)sourceFile.Length;
                                allData.Add(r2v2);
                                fileData.Add(r2v2);
                                //infoStream.WriteLine(r2v2.hexString);
                                counter++;
                            }

                            // r2v2 block is 0x50 long, so add 0x4b
                            //long remainder = (sourceFile.Length-(binReader.BaseStream.Position+0x4b));
                            long remainder = (sourceFile.Length - (binReader.BaseStream.Position));
                            infoStream.WriteLine();
                            R2V2 prev = null;
                            foreach (R2V2 filer2v2 in fileData)
                            {
                                //if (prev != null)
                                //{
                                //    int diff = (filer2v2.val4 - prev.val4);
                                //    if (diff % 16 != 0)
                                //    {
                                //        int ibreak = 0;
                                //    }   

                                //    //infoStream.WriteLine("Diff : " + diff);
                                //}
                                //prev = filer2v2;
                                //infoStream.WriteLine(String.Format("Verts: {0}  Faces: {1} ", filer2v2.numVertices, filer2v2.numFaces));

                            }


                            // check 48 vals and place 4 incrementing / diff values.
                            for (int i = 0; i < fileData.Count; ++i)
                            {
                                if (i > 0)
                                {
                                    int diff = fileData[i].sizecounter48 - fileData[i - 1].sizecounter48;
                                    if (Math.Abs(diff) != 0x48)
                                    {
                                        int ibreak = 0;
                                    }

                                    if (fileData[i].val4 < fileData[i - 1].val4)
                                    {
                                        int ibreak = 0;
                                    }
                                }

                                if (fileData[i].numVertices < 0)
                                {
                                    int ibreak = 0;
                                }

                                if (fileData[i].numFaces < 0)
                                {
                                    int ibreak = 0;
                                }

                                if (fileData[i].val3 < 0)
                                {
                                    int ibreak = 0;
                                }
                                if (fileData[i].val4 < 0)
                                {
                                    int ibreak = 0;
                                }
                                if (fileData[i].val5 < 0)
                                {
                                    int ibreak = 0;
                                }
                                if (fileData[i].val6 < 0)
                                {
                                    int ibreak = 0;
                                }

                                if (fileData[i].numVertices > fileData[i].fileSize)
                                {
                                    int ibreak = 0;
                                }
                                if (fileData[i].numFaces > fileData[i].fileSize)
                                {
                                    int ibreak = 0;
                                }
                                if (fileData[i].val3 > fileData[i].fileSize)
                                {
                                    int ibreak = 0;
                                }


                            }




                            //infoStream.WriteLine(String.Format("{0} R2V2Blocks . Size after last block {1} val1 {2} val2 {3} ", counter, remainder));
                            infoStream.WriteLine();
                            infoStream.WriteLine();
                        }
                    }
                    catch (Exception e)
                    {
                        int ibreak = 0;
                    }
                }
                allData.Sort(delegate(R2V2 t1, R2V2 t2)
                { return (t1.numVertices.CompareTo(t2.numVertices)); });

                foreach (R2V2 r2v2 in allData)
                {
                    infoStream.WriteLine(String.Format("{0} : {1} : {2} : {3}", r2v2.FileName, r2v2.numVertices, r2v2.numFaces, r2v2.hexStringFormatted));
                }

            }
            //R2V2 val = allData.Find(item => item.FileName == "nordagh_tree.mdl");

    
            int ibreak2=  0;
        }
        static void Main(string[] args)
        {
            new R2V2Analyzer().Process();
        }



        List<R2V2> allData = new List<R2V2>();

    }
}
