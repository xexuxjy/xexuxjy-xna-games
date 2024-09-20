using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GCTextureTools;
using UnityEngine;

public class TestExamineTextureInfo : MonoBehaviour
{
    public void Awake()
    {
        // string[] allTextures = Directory.GetFiles(@"D:\GladiusGCPacking\BEC\unpack\data\mesh\weapons\weaponcs", "*.ptx",
        //     SearchOption.AllDirectories);

        string[] allTextures = Directory.GetFiles(@"D:\GladiusGCPacking\BEC\unpack\data\mesh\", "*.ptx",
            SearchOption.AllDirectories);

        StringBuilder finalResult = new StringBuilder();
        finalResult.AppendLine("Filename,ImageName,HeaderSize,DXT,MipCount,BitDepth,VStart,UStart,Width,Height,Size,Offset,NameOffset");
        
        foreach (String fileName in allTextures)
        {
            try
            {
                FileInfo file = new FileInfo(fileName);
                using (FileStream fs = new FileStream(file.FullName, FileMode.Open))
                {
                    using (BinaryReader binReader = new BinaryReader(fs))
                    {
                        List<GCGladiusImage> imageList = GCImageExtractor.LoadDataChunk(binReader, null);
                        foreach (GCGladiusImage gi in imageList)
                        {
                            finalResult.AppendLine(fileName + "," + gi.ImageName + "," + gi.DebugInfoCSV);
                        }
                    }
                }

                //debugInfo.AppendLine();

            }
            catch (Exception e)
            {
                //debugInfo.AppendLine(e.ToString());
            }

            //break;
        }

        File.WriteAllText("c:/tmp/gc-texture-info.txt", finalResult.ToString());
        
    }
}