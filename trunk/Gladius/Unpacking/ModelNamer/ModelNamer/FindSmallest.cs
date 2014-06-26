using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ModelNamer
{
    class FindSmallest
    {
        public void ProcessFindSmallest()
        {

            String sourceDirectory = @"D:\gladius-extracted\ps2-decompressed\VERSModelFilesRenamed\";
            String infoFile = @"D:\gladius-extracted\ps2-decompressed\findFaceResults.txt";

            char[] modelName = new char[32];
            
            char[] searchString1 = new char[]{'R','2','D','2','p','s','x','2'};
            String[] files = Directory.GetFiles(sourceDirectory, "*");
            long modelNameOffset = 68;

            List<Result> resultList = new List<Result>();

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
                            if (Common.FindCharsInStream(binReader, searchString1) )
                            {
                                if (Common.FindCharsInStream(binReader, R2V2.tag))
                                {
                                    binReader.BaseStream.Position -= (8 + R2V2.tag.Length);
                                    int numVerts = binReader.ReadInt32();
                                    int numFaces = binReader.ReadInt32();
                                    Result r = new Result();
                                    r.ModelName = sourceFile.Name;
                                    r.NumVertices = numVerts;
                                    r.NumFaces = numFaces;
                                    r.FileSize = sourceFile.Length;
                                    resultList.Add(r);

                                    //infoStream.WriteLine(String.Format("{0}  nv={1} nf ={2}", sourceFile.Name, numVerts, numFaces));
                                    //if (numFaces > 0 && numFaces < minFaces)
                                    //{
                                    //    minFaces = numFaces;
                                    //    minFacesFilename = sourceFile.Name;
                                    //}
                                    //if (numVerts > 0 && numVerts< minVerts)
                                    //{
                                    //    minVerts = numVerts;
                                    //    minVertsFilename = sourceFile.Name;
                                    //}

                                }
                            }
                            else
                            {
                                infoStream.WriteLine("Unable to find search string in file : " + file);
                            }

                        }
                    }
                    catch (Exception e)
                    {
                        infoStream.WriteLine("Unable to convert : " + file);
                    }
                }

                resultList.Sort(delegate(Result t1, Result t2)
                { return (t1.NumVertices.CompareTo(t2.NumVertices)); });

                //resultList.Sort(delegate(Result t1, Result t2)
                //{ return (t1.FileSize.CompareTo(t2.FileSize)); });


                foreach (Result r in resultList)
                {
                    infoStream.WriteLine(String.Format("{0}  nv={1} nf={2} fs={3}", r.ModelName, r.NumVertices, r.NumFaces,r.FileSize));
                }
                 
                //infoStream.WriteLine(String.Format("\n\n\n\n\n Faces Min : {0} nf={1} \n Verts : {2} nv={3}", minFacesFilename, minFaces, minFacesFilename, minVerts));


            }
        }

        public class Result
        {
            public string ModelName;
            public int NumVertices;
            public int NumFaces;
            public long FileSize;
        }

        static void Main(string[] args)
        {
            new FindSmallest().ProcessFindSmallest();
        }
    
    }
}
