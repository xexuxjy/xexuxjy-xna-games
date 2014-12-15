using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.IO;
using System.Threading;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace ModelNamer
{
    public class GCModel : BaseModel
    {
        public GCModel(String name) : base(name)
        {
            
        }


        public void LoadData(BinaryReader binReader,long startPosition=0)
        {
            binReader.BaseStream.Position = startPosition;

            //Common.ReadTextureNames(binReader, Common.txtrTag, model.m_textures);

            // check here as we need skinned info on building display lists.
            if (Common.FindCharsInStream(binReader, Common.skinTag))
            {
                m_skinned = true;
            }


            // Look for skeleton and skin if they exist. need to load names first.
            binReader.BaseStream.Position = startPosition;
            Common.ReadNullSeparatedNames(binReader, Common.nameTag, m_names);
            binReader.BaseStream.Position = startPosition;
            ReadSKELSection(binReader);
            binReader.BaseStream.Position = startPosition;
            ReadJLODSection(binReader);
            binReader.BaseStream.Position = startPosition;
            ReadSHDRSection(binReader);
            binReader.BaseStream.Position = startPosition;
            ReadDSLCSection(binReader);
            binReader.BaseStream.Position = startPosition;
            ReadDSLISection(binReader);
            binReader.BaseStream.Position = startPosition;
            ReadDSLSSection(binReader);
            binReader.BaseStream.Position = startPosition;
            ReadMESHSection(binReader);
            binReader.BaseStream.Position = startPosition;
            ReadSkinSection(binReader);
            binReader.BaseStream.Position = startPosition;
            ReadTextureSection(binReader);
            binReader.BaseStream.Position = startPosition;

            if (Common.FindCharsInStream(binReader, Common.cntrTag, true))
            {
                // setting this to all null seemed to make no difference...
                //int ibreak = 0;
            }
            // not skinned so look for fixed version.l
            if (m_skinned == false)
            {
                if (Common.FindCharsInStream(binReader, Common.posiTag))
                {
                    int posSectionLength = binReader.ReadInt32();

                    int uk2 = binReader.ReadInt32();
                    int numPoints = binReader.ReadInt32();
                    for (int i = 0; i < numPoints; ++i)
                    {
                        //model.m_points.Add(Common.FromStreamVector3BE(binReader));
                        m_modelMeshes[0].Vertices.Add(Common.FromStreamVector3BE(binReader));
                    }
                }

                if (Common.FindCharsInStream(binReader, Common.normTag))
                {
                    int normSectionLength = binReader.ReadInt32();
                    int uk4 = binReader.ReadInt32();
                    int numNormals = binReader.ReadInt32();

                    for (int i = 0; i < numNormals; ++i)
                    {
                        //model.m_normals.Add(Common.FromStreamVector3BE(binReader));
                        m_modelMeshes[0].Normals.Add(Common.FromStreamVector3BE(binReader));
                    }


                }
            }

            binReader.BaseStream.Position = startPosition;

            if (Common.FindCharsInStream(binReader, Common.uv0Tag))
            {
                int normSectionLength = binReader.ReadInt32();
                int uk4 = binReader.ReadInt32();
                int numUVs = binReader.ReadInt32();

                // normal model has uv's as 8 bytes (2 floats) per block.
                // skinned model has ub's as 4 bytes (2???) per block...

                if (m_skinned)
                {
                    for (int i = 0; i < numUVs; ++i)
                    {
                        //model.m_uvs.Add(new Vector2(Common.ToFloatUInt16BigEndian(binReader), Common.ToFloatUInt16BigEndian(binReader)));
                        //model.m_uvs.Add(new Vector2(Common.FromStream2ByteToFloat(binReader), Common.FromStream2ByteToFloat(binReader)));

                        ushort ua = Common.ToUInt16BigEndian(binReader);
                        ushort ub = Common.ToUInt16BigEndian(binReader);

                        float a = (float)ua / 4096;
                        float b = (float)ub / 4096;

                        //float a = Common.FromStream2ByteToFloatR(binReader);
                        //float b = Common.FromStream2ByteToFloatR(binReader);

                        // just use fractional part.
                        a -= (int)a;
                        b -= (int)b;


                        //float a = (((float)binReader.ReadByte()) / 255.0f);
                        //binReader.ReadByte();
                        //float b = (((float)binReader.ReadByte()) / 255.0f);
                        //binReader.ReadByte();
                        m_uvs.Add(new Vector2(a, b));

                    }
                }
                else
                {
                    for (int i = 0; i < numUVs; ++i)
                    {
                        m_uvs.Add(Common.FromStreamVector2BE(binReader));
                    }
                }





            }
            BuildBB();
            //model.Validate();

        }





        public void DumpDisplayBlocks(String fileOutputDir)
        {
            fileOutputDir += "-disp-output";
            int counter = 0;
            String tagOutputDirname = fileOutputDir + "/" + m_name + "/";
            try
            {
                Directory.CreateDirectory(tagOutputDirname);
            }
            catch (Exception e)
            {
            }

            foreach (DisplayListHeader headerBlock in m_modelMeshes)
            {

                String tagOutputFilename = tagOutputDirname + "/" + "DSLS-" + counter;
                using (System.IO.StreamWriter outStream = new StreamWriter(File.Open(tagOutputFilename, FileMode.Create)))
                {
                    outStream.WriteLine(String.Format("DSLC[{0:X2}]", headerBlock.dslcEntry));

                    outStream.WriteLine(String.Format("DSLI[{0}][{1}][{2:0.0}][{3}]", headerBlock.dsliInfo.startPos, headerBlock.dsliInfo.length, headerBlock.averageSize, headerBlock.adjustedSizeInt));
                    outStream.WriteLine(String.Format("P[{0}]N[{1}]U[{2}] MP[{3}]MN[{4}]MU[{5}]I[{6}", headerBlock.MaxVertex, headerBlock.MaxNormal, headerBlock.MaxUV, headerBlock.Vertices.Count, headerBlock.Normals.Count, m_uvs.Count,headerBlock.entries.Count()/3));

                    int headerSize = 6;
                    outStream.WriteLine(Common.ByteArrayToStringSub(headerBlock.blockData, 0, headerSize));



                    if (headerBlock.adjustedSizeInt == 4)
                    {
                        for (int i = 0; i < headerBlock.indexCount; ++i)
                        {
                            int index = headerSize + (i * 4);
                            outStream.WriteLine(String.Format("{0:X2}{1:X2} {2:X2}{3:X2}", headerBlock.blockData[index + 0], headerBlock.blockData[index + 1], headerBlock.blockData[index + 2], headerBlock.blockData[index + 3]));
                        }
                    }
                    else if (headerBlock.adjustedSizeInt == 5)
                    {
                        for (int i = 0; i < headerBlock.indexCount; ++i)
                        {
                            int index = headerSize + (i * 5);
                            outStream.WriteLine(String.Format("{0:X2}{1:X2} {2:X2}{3:X2} {4:X2}", headerBlock.blockData[index + 0], headerBlock.blockData[index + 1], headerBlock.blockData[index + 2], headerBlock.blockData[index + 3], headerBlock.blockData[index + 4]));
                        }
                    }
                    else if (headerBlock.adjustedSizeInt == 6)
                    {
                        for (int i = 0; i < headerBlock.indexCount; ++i)
                        {
                            int index = headerSize + (i * 6);
                            outStream.WriteLine(String.Format("{0:X2}{1:X2} {2:X2}{3:X2} {4:X2}{5:X2}", headerBlock.blockData[index + 0], headerBlock.blockData[index + 1], headerBlock.blockData[index + 2], headerBlock.blockData[index + 3], headerBlock.blockData[index + 4], headerBlock.blockData[index + 5]));
                        }
                    }
                    else if (headerBlock.adjustedSizeInt == 7)
                    {
                        for (int i = 0; i < headerBlock.indexCount; ++i)
                        {
                            int index = headerSize + (i * 7);
                            outStream.WriteLine(String.Format("{0:X2}{1:X2} {2:X2}{3:X2} {4:X2} {5:X2}{6:X2}", headerBlock.blockData[index + 0], headerBlock.blockData[index + 1], headerBlock.blockData[index + 2], headerBlock.blockData[index + 3], headerBlock.blockData[index + 4], headerBlock.blockData[index + 5], headerBlock.blockData[index + 6]));
                        }
                    }
                    else if (headerBlock.adjustedSizeInt == 8)
                    {
                        for (int i = 0; i < headerBlock.indexCount; ++i)
                        {
                            int index = headerSize + (i * 8);
                            outStream.WriteLine(String.Format("{0:X2}{1:X2} {2:X2}{3:X2} {4:X2}{5:X2} {6:X2}{7:X2}", headerBlock.blockData[index + 0], headerBlock.blockData[index + 1], headerBlock.blockData[index + 2], headerBlock.blockData[index + 3], headerBlock.blockData[index + 4], headerBlock.blockData[index + 5], headerBlock.blockData[index + 6], headerBlock.blockData[index + 7]));
                        }
                    }
                    else
                    {
                        int ibreak = 0;
                    }

                }
                counter++;
            }
        }



        public void DumpSkinBlocks(String fileOutputDir)
        {
            fileOutputDir += "-skin-output";
            int counter = 0;
            String tagOutputDirname = fileOutputDir + "/" + m_name + "/";
            try
            {
                Directory.CreateDirectory(tagOutputDirname);
            }
            catch (Exception e)
            {
            }

            String tagOutputFilename = tagOutputDirname + "/" + "ALLHDR";
            using (System.IO.BinaryWriter allOutStream = new BinaryWriter(File.Open(tagOutputFilename, FileMode.Create)))
            {

                foreach (SkinBlock skinBlock in m_skinBlocks)
                {
                    tagOutputFilename = tagOutputDirname + "/" + "SKIN-" + counter;
                    using (System.IO.BinaryWriter outStream = new BinaryWriter(File.Open(tagOutputFilename, FileMode.Create)))
                    {
                        outStream.Write(skinBlock.fullBlock);
                    }
                    tagOutputFilename = tagOutputDirname + "/" + "VAL-" + counter;
                    using (System.IO.StreamWriter outStream = new StreamWriter(File.Open(tagOutputFilename, FileMode.Create)))
                    {
                        outStream.Write(skinBlock.PrettyPrint());
                    }


                    tagOutputFilename = tagOutputDirname + "/" + "HDR-" + counter;
                    using (System.IO.BinaryWriter outStream = new BinaryWriter(File.Open(tagOutputFilename, FileMode.Create)))
                    {
                        outStream.Write(skinBlock.headerBlock);
                    }
                    allOutStream.Write(Common.ByteArrayToString(skinBlock.headerBlock, 2));
                    allOutStream.Write('\n');

                    using (System.IO.StreamWriter sw = new StreamWriter(File.Open(tagOutputDirname + "/" + "pos-bone" + counter, FileMode.Create)))
                    {
                        foreach(SkinElement skinElement in skinBlock.elements)
                        {
                            Common.WriteFloat(sw, skinElement.pos);
                            sw.WriteLine(String.Format("[{0}][{1}][{2}]", skinElement.rem[0], skinElement.rem[1], skinElement.rem[2]));
                        }
                    }
                    counter++;
                }
            }
        }

        public void BuildBB(List<Vector3> points, DisplayListHeader header)
        {
            Vector3 min = new Vector3(float.MaxValue);
            Vector3 max = new Vector3(float.MinValue);

            int count = 0;
            foreach (DisplayListEntry entry in header.entries)
            {

                count++;
                if (entry.PosIndex < points.Count)
                {
                    min = Vector3.Min(min, points[entry.PosIndex]);
                    max = Vector3.Max(max, points[entry.PosIndex]);
                }
                else
                {
                    int ibreak = 0;
                }
            }
            header.MinBB = min;
            header.MaxBB = max;
        }

        public void BuildBB()
        {
            if (!m_builtBB)
            {
                // build bb for main model, and also for each sub-mesh
                Vector3 min = new Vector3(float.MaxValue);
                Vector3 max = new Vector3(float.MinValue);

                int count = 0;
                foreach (DisplayListHeader header in m_modelMeshes)
                {
                    BuildBB(header.Vertices, header);
                    min = Vector3.Min(header.MinBB, min);
                    max = Vector3.Max(header.MaxBB, max);
                }



                //if (m_skinned)
                //{
                //    int count = 0;
                //    foreach (DisplayListHeader header in m_modelMeshes)
                //    {
                //        if (m_skinned)
                //        {
                //            if (count < m_skinBlocks.Count)
                //            {
                //                header.skinBlock = m_skinBlocks[count];
                //            }
                //            else
                //            {
                //                int ibreak = 0;
                //            }
                //        }
                //        try
                //        {
                //            BuildBB(header.skinBlock.m_points, header);
                //            min = Vector3.Min(header.MinBB, min);
                //            max = Vector3.Max(header.MaxBB, max);
                //        }
                //        catch (System.Exception ex)
                //        {
                //            int ibreak = 0;
                //        }
                //        count++;

                //    }


                //}
                //else
                //{
                //    BuildBB(m_points, out min, out max);
                //}


                MinBB = min;
                MaxBB = max;

                m_builtBB = true;
            }
        }

        public void ReadDSLISection(BinaryReader binReader)
        {
            if (Common.FindCharsInStream(binReader, Common.dsliTag, true))
            {
                int blockSize = binReader.ReadInt32();
                int pad1 = binReader.ReadInt32();
                int pad2 = binReader.ReadInt32();
                int numSections2 = (blockSize - 8 - 4 - 4) / 8;
                int numSections = m_modelMeshes.Count;

                for (int i = 0; i < numSections; ++i)
                {
                    DisplayListHeader header = m_modelMeshes[i] as DisplayListHeader;
                    DSLIInfo info = DSLIInfo.ReadStream(binReader);
                    if (m_skinned)
                    {
                        //info.length /= 2;
                        info.startPos *= 2;
                    }

                    if (info.length > 0)
                    {
                        m_dsliInfos.Add(info);
                        header.dsliInfo = info;
                    }
                }
            }
        }

        public void ReadMESHSection(BinaryReader binReader)
        {
            if (Common.FindCharsInStream(binReader, Common.meshTag))
            {
                int pad1 = binReader.ReadInt32();
                int pad2 = binReader.ReadInt32();
                int numSections = binReader.ReadInt32();
                //Debug.Assert(numSections == m_modelMeshes.Count())
                for (int i = 0; i < numSections; ++i)
                {
                    DisplayListHeader header = m_modelMeshes[i] as DisplayListHeader;
                    header.meshUnk1 = binReader.ReadInt32();
                    header.meshUnk2 = binReader.ReadInt32();
                    header.MeshId = binReader.ReadInt32();
                    header.meshUnk3 = binReader.ReadInt32();
                    header.meshUnk4 = binReader.ReadInt32();
                    header.LodLevel = binReader.ReadInt32();
                }
            }
        }


        public void ReadSHDRSection(BinaryReader binReader)
        {
            if (Common.FindCharsInStream(binReader, Common.shdrTag))
            {
                int blockSize = binReader.ReadInt32();
                int pad1 = binReader.ReadInt32();
                int numElements = binReader.ReadInt32();
                for (int i = 0; i < numElements; ++i)
                {
                    m_shaderData.Add(ShaderData.FromStream(binReader));

                }
                int ibreak = 0;
            }

        }



        public void ReadSKELSection(BinaryReader binReader)
        {
            if (Common.FindCharsInStream(binReader, Common.skelTag))
            {
                m_hasSkeleton = true;
                int blockSize = binReader.ReadInt32();
                int pad1 = binReader.ReadInt32();
                int pad2 = binReader.ReadInt32();
                int numBones = (blockSize - 16) / 32;
                Debug.Assert(pad2 == numBones);

                for (int i = 0; i < numBones; ++i)
                {
                    BoneNode node = BoneNode.FromStream(binReader);
                    node.name = m_names[i];
                    m_bones.Add(node);
                }
                ConstructSkeleton();
            }

        }

        public void ReadJLODSection(BinaryReader binReader)
        {
            // lod details for bones. - maybe. 
            if (Common.FindCharsInStream(binReader, Common.jlodTag))
            {
                int blockSize = binReader.ReadInt32();
                int pad1 = binReader.ReadInt32();
                int numElements = binReader.ReadInt32();
                Debug.Assert(numElements == m_bones.Count);

                HashSet<int> set = new HashSet<int>();
                for (int i = 0; i < numElements; ++i)
                {
                    int info = binReader.ReadInt32();
                    m_jlodInfo.Add(info);
                    set.Add(info);
                }
                int ibreak = 0;
            }


        }

        public void ReadDSLCSection(BinaryReader binReader)
        {
            binReader.BaseStream.Position = 0;
            if (Common.FindCharsInStream(binReader, Common.dslcTag))
            {
                int dslsSectionLength = binReader.ReadInt32();
                int uk2a = binReader.ReadInt32();
                int numEntries = binReader.ReadInt32();

                for (int i = 0; i < numEntries; ++i)
                {
                    DisplayListHeader header = new DisplayListHeader();
                    header.m_model = this;
                    //header.dsliInfo = info;

                    //DisplayListHeader header = m_modelMeshes[i] as DisplayListHeader;
                    byte val = binReader.ReadByte();
                    header.dslcEntry = val;
                    // add based on number of times?
                    for(byte j=0;j<val;++j)
                    {
                        m_modelMeshes.Add(header);
                    }
                }
            }

        }


        public void ReadDSLSSection(BinaryReader binReader)
        {
            if (Common.FindCharsInStream(binReader, Common.dslsTag))
            {
                long dsllStartsAt = binReader.BaseStream.Position;
                int dslsSectionLength = binReader.ReadInt32();
                int uk2a = binReader.ReadInt32();
                int uk2b = binReader.ReadInt32();

                long startPos = binReader.BaseStream.Position;

                for (int i = 0; i < m_dsliInfos.Count; ++i)
                {
                    binReader.BaseStream.Position = startPos + m_dsliInfos[i].startPos;
                    DisplayListHeader.FromStream(m_modelMeshes[i] as DisplayListHeader, binReader, m_dsliInfos[i]);
                    MaxVertex = Math.Max(MaxVertex, m_modelMeshes[i].MaxVertex);
                }

     /*
      * 19248  0x4b30
        37068  0x90cc
      */
                long nowAt = binReader.BaseStream.Position;

                long diff = (dsllStartsAt + (long)dslsSectionLength) - nowAt;

            }


        }


        // it looks as though there are lodded models in some of the lists
        //fixme - determine lodding flag
        //    determing texture flag
        //look in SkinBlock header info.
        public void ReadSkinSection(BinaryReader binReader)
        {
            if (Common.FindCharsInStream(binReader, Common.skinTag))
            {
                m_skinned = true;
                int blocksize = binReader.ReadInt32();
                int pad1 = binReader.ReadInt32();
                int numElements = binReader.ReadInt32();
                int counter = 0;
                for (int i = 0; i < numElements; ++i)
                {
                    SkinBlock skinBlock = SkinBlock.ReadBlock(binReader);
                    DisplayListHeader h = (m_modelMeshes[counter] as DisplayListHeader);
                    h.skinBlock = skinBlock;
                    m_skinBlocks.Add(skinBlock);
                    counter += h.dslcEntry;
                }
            }
        }

        public override void Validate()
        {
            foreach (DisplayListHeader header in m_modelMeshes)
            {
                if (header.primitiveFlags == 0x90)
                {

                    for (int i = 0; i < header.entries.Count; ++i)
                    {
                        if (header.entries[i].PosIndex > MaxVertex)
                        {
                            MaxVertex = header.entries[i].PosIndex;
                        }
                        if (header.entries[i].NormIndex > m_maxNormal)
                        {
                            m_maxNormal = header.entries[i].NormIndex;
                        }

                        if (header.entries[i].UVIndex > m_maxUv)
                        {
                            m_maxUv = header.entries[i].UVIndex;
                        }

                        if (header.entries[i].PosIndex < 0 || header.entries[i].PosIndex >= header.Vertices.Count)
                        {
                            header.Valid = false;
                            //break;
                        }
                        if (header.entries[i].NormIndex < 0 || header.entries[i].NormIndex >= header.Normals.Count)
                        {
                            header.Valid = false;
                            //break;
                        }
                        if (header.entries[i].UVIndex < 0 || header.entries[i].UVIndex >= m_uvs.Count)
                        {
                            header.Valid = false;
                            //break;
                        }


                    }
                }
            }


        }

        public void WriteOBJ(StreamWriter writer, StreamWriter materialWriter,String texturePath,int desiredLod=-1)
        {
            int vertexCountOffset = 0;
            int normalCountOffset = 0;
            int uvCountOffset = 0;

            //String materialName = null;

            string reflectname = "skygold_R.tga";
            // write material?
            if (m_textures.Count == 2 && (m_textures[0].textureName.Contains(reflectname) || m_textures[1].textureName.Contains(reflectname)))
            {
                int notsgindex = m_textures[0].textureName.Contains(reflectname) ? 1 : 0;

                String textureName = m_textures[notsgindex].textureName + ".jpg";
                materialWriter.WriteLine("newmtl " + textureName);
                materialWriter.WriteLine("Ka 1.000 1.000 1.000");
                materialWriter.WriteLine("Kd 1.000 1.000 1.000");
                materialWriter.WriteLine("Ks 0.000 0.000 0.000");
                materialWriter.WriteLine("d 1.0");
                materialWriter.WriteLine("illum 3");
                materialWriter.WriteLine("map_Ka " + texturePath + textureName);
                materialWriter.WriteLine("map_Kd " + texturePath + textureName);

                materialWriter.WriteLine("refl -type sphere -mm 0 1 " + texturePath + reflectname+".jpg"); 


            }
            else
            {

                foreach (TextureData textureData in m_textures)
                {
                    String textureName = textureData.textureName + ".jpg";
                    materialWriter.WriteLine("newmtl " + textureName);
                    materialWriter.WriteLine("Ka 1.000 1.000 1.000");
                    materialWriter.WriteLine("Kd 1.000 1.000 1.000");
                    materialWriter.WriteLine("Ks 0.000 0.000 0.000");
                    materialWriter.WriteLine("d 1.0");
                    materialWriter.WriteLine("illum 2");
                    materialWriter.WriteLine("map_Ka " + texturePath + textureName);
                    materialWriter.WriteLine("map_Kd " + texturePath + textureName);

                    materialWriter.WriteLine("refl -type sphere -mm 0 1 clouds.mpc");
                }
            }
            writer.WriteLine("mtllib " + m_name + ".mtl");
            int submeshCount = 0;
            foreach (DisplayListHeader headerBlock in m_modelMeshes)
            {
                // just want highest lod.
                if (headerBlock.LodLevel != 0 && ((headerBlock.LodLevel & desiredLod) == 0))
                {
                    continue;
                }

                string groupName = String.Format("{0}-submesh{1}-LOD{2}" ,m_name,submeshCount,headerBlock.LodLevel);

                writer.WriteLine("o " + groupName);
                writer.WriteLine("g " + groupName);
                // and now points, uv's and normals.
                foreach (Vector3 v in headerBlock.Vertices)
                {
                    writer.WriteLine(String.Format("v {0:0.00000} {1:0.00000} {2:0.00000}", v.X, v.Y, v.Z));
                }
                foreach (Vector2 v in headerBlock.UVs)
                {
                    writer.WriteLine(String.Format("vt {0:0.00000} {1:0.00000}", v.X, 1.0f - v.Y));
                }
                foreach (Vector3 v in headerBlock.Normals)
                {
                    writer.WriteLine(String.Format("vn {0:0.00000} {1:0.00000} {2:0.00000}", v.X, v.Y, v.Z));
                }

                
                ShaderData shaderData = m_shaderData[headerBlock.MeshId];

                int adjustedId = MathHelper.Clamp(shaderData.textureId2, 0, m_textures.Count - 1);

                String materialName = m_textures[adjustedId].textureName+ ".jpg";
                writer.WriteLine("usemtl " + materialName);

                int counter = 0;
                int vertexOffset = 1+vertexCountOffset;
                int normalOffset = 1 + normalCountOffset;
                int uvOffset = 1 + uvCountOffset;
                for (int i = 0; i < headerBlock.entries.Count; )
                {
                    writer.WriteLine(String.Format("f {0}/{1}/{2} {3}/{4}/{5} {6}/{7}/{8}", 
                        headerBlock.entries[i].PosIndex + vertexOffset, headerBlock.entries[i].UVIndex + uvOffset, headerBlock.entries[i].NormIndex + normalOffset,
                        headerBlock.entries[i + 1].PosIndex + vertexOffset, headerBlock.entries[i + 1].UVIndex + uvOffset, headerBlock.entries[i + 1].NormIndex + normalOffset,
                        headerBlock.entries[i + 2].PosIndex + vertexOffset, headerBlock.entries[i + 2].UVIndex + uvOffset, headerBlock.entries[i + 2].NormIndex + normalOffset));
                    i += 3;
                }

                vertexCountOffset += headerBlock.Vertices.Count;
                normalCountOffset += headerBlock.Normals.Count;
                uvCountOffset += 0; // shared uvs?
                submeshCount++;
            }
        }

        public bool Valid
        {
            get
            {
                if (m_modelMeshes.Count == 0)
                {
                    return false;
                }
                foreach (DisplayListHeader dlh in m_modelMeshes)
                {
                    if (dlh.Valid == false)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public List<Vector3> m_pos = new List<Vector3>();
        public List<Vector3> m_normal = new List<Vector3>();
        public List<Vector2> m_uvs = new List<Vector2>();
        public List<DSLIInfo> m_dsliInfos = new List<DSLIInfo>();
        //public List<DisplayListHeader> m_modelMeshes = new List<DisplayListHeader>();
        public List<SkinBlock> m_skinBlocks = new List<SkinBlock>();
        public List<int> m_jlodInfo = new List<int>();
    }

    public class GCModelReader : BaseModelReader
    {
        public List<BaseModel> m_models = new List<BaseModel>();

        public void LoadModels()
        {
            LoadModels(@"c:\tmp\unpacking\gc-models\", @"c:\tmp\unpacking\gc-models\results.txt");
        }

        public void LoadModelsTags()
        {
            LoadModels(@"c:\tmp\unpacking\gc-models\", @"c:\tmp\unpacking\gc-models\results.txt");
        }


        public override BaseModel LoadSingleModel(String modelPath, bool readDisplayLists = true)
        {
            FileInfo sourceFile = new FileInfo(modelPath);

            using (BinaryReader binReader = new BinaryReader(new FileStream(sourceFile.FullName, FileMode.Open)))
            {   
                GCModel model = new GCModel(sourceFile.Name);

                model.LoadModelTags(binReader,Common.allTags);
                model.LoadData(binReader);
                return model;
            }

        }


        public void LoadModels(String sourceDirectory, String infoFile, int maxFiles = -1)
        {
            m_models.Clear();
            String[] files = Directory.GetFiles(sourceDirectory, "*");
            int counter = 0;

            using (System.IO.StreamWriter infoStream = new System.IO.StreamWriter(infoFile))
            {
                foreach (String file in files)
                {
                    try
                    {
                        BaseModel model = LoadSingleModel(file);
                        if (model != null)
                        {
                            m_models.Add(model);
                        }

                    }
                    catch (Exception e)
                    {
                        int ibreak = 0;
                    }
                    counter++;
                    if (maxFiles > 0 && counter > maxFiles)
                    {
                        break;
                    }

                }
            }
        }

        public void DumpPoints(String infoFile)
        {
            //using (System.IO.StreamWriter infoStream = new System.IO.StreamWriter(infoFile))
            //{
            //    foreach (GCModel model in m_models)
            //    {
            //        infoStream.WriteLine(String.Format("File : {0} : {1} : {2}", model.m_name, model.m_points.Count, model.m_normals.Count));
            //        infoStream.WriteLine("Verts : ");
            //        foreach (Vector3 sv in model.m_points)
            //        {
            //            Common.WriteInt(infoStream, sv);
            //        }
            //        infoStream.WriteLine("Normals : ");
            //        foreach (Vector3 sv in model.m_normals)
            //        {
            //            Common.WriteInt(infoStream, sv);
            //        }
            //        infoStream.WriteLine();
            //        infoStream.WriteLine();
            //    }
            //}

        }

        public void DumpSectionLengths(String sourceDirectory, String infoFile)
        {
            m_models.Clear();
            String[] files = Directory.GetFiles(sourceDirectory, "*");

            using (System.IO.StreamWriter infoStream = new System.IO.StreamWriter(infoFile))
            {
                foreach (String file in files)
                {
                    try
                    {

                        FileInfo sourceFile = new FileInfo(file);


                        BaseModel model = LoadSingleModel(file);
                        StringBuilder sb = new StringBuilder();
                        infoStream.WriteLine("File : " + model.m_name);
                        foreach (ModelSubMesh modelMesh in model.m_modelMeshes)
                        {
                            sb.AppendLine(String.Format("P[{0}]N[{1}]U[{2}] MP[{3}]MN[{4}]MU[{5}]I[{6}]",
                                modelMesh.MaxVertex, modelMesh.MaxNormal, modelMesh.MaxUV,
                                modelMesh.Vertices.Count, modelMesh.Normals.Count, model.UVs.Count, modelMesh.NumIndices / 3));
                        }

                        sb.AppendLine("SELS : ");
                        foreach (string selName in model.m_selsInfo)
                        {
                            sb.AppendLine("\t" + selName);
                        }

                        sb.AppendLine("NAME : ");
                        foreach (string name in model.m_names)
                        {
                            sb.AppendLine("\t" + name);
                        }

                        sb.AppendLine("Textures : ");
                        foreach (TextureData textureData in model.m_textures)
                        {
                            sb.AppendLine("\t" + textureData.textureName);
                        }

                        infoStream.WriteLine(sb.ToString());
                    }
                    catch (Exception e)
                    {
                        
                    }
                }
            }

        }




        //        for (int i = 0; i < numBones; ++i)
        //        {
        //            BoneNode node = BoneNode.FromStream(binReader);
        //            model.m_bones.Add(node);
        //        }
        //    }
        //    model.ConstructSkeleton();
        //}


        static void Main(string[] args)
        {
            //String modelPath = @"C:\tmp\unpacking\gc-probable-models-renamed\probable-models-renamed";
            String modelPath = @"D:\gladius-extracted-archive\gc-compressed\AllModelsRenamed - Copy";
            String infoFile = @"D:\gladius-extracted-archive\gc-compressed\ModelInfo.txt";
            //String sectionInfoFile = @"C:\tmp\unpacking\gc-probable-models-renamed\sectionInfo.txt";
            String objOutputPath = @"D:\gladius-extracted-archive\gc-compressed\AllModelsRenamed-Obj\";
            //String tagOutputPath = @"C:\tmp\unpacking\xbox-ModelFiles\tag-output";

            String tagOutputPath = @"D:\gladius-extracted-archive\gc-compressed\probable-skinned-models\tag-output";

            string sectionInfoFile = @"D:\gladius-extracted-archive\gc-compressed\ModelInfo.txt";

            GCModelReader reader = new GCModelReader();
            //GCModel model = reader.LoadSingleModel(@"D:\gladius-extracted-archive\gc-compressed\test-models\bow.mdl");
            //GCModel model = reader.LoadSingleModel(@"D:\gladius-extracted-archive\gc-compressed\test-models\File 015227");
            int ibreak = 0;

            String texturePath = @"D:\gladius-extracted-archive\gc-compressed\textures.jpg\";


            List<string> filenames = new List<string>();


            //filenames.AddRange(Directory.GetFiles(@"D:\gladius-extracted-archive\gc-compressed\AllModelsRenamed\weapons", "*"));
            //filenames.AddRange(Directory.GetFiles(@"D:\gladius-extracted-archive\gc-compressed\AllModelsRenamed", "*"));
            //filenames.AddRange(Directory.GetFiles(@"D:\gladius-extracted-archive\gc-compressed\AllModelsRenamed", "*"));

            //filenames.AddRange(Directory.GetFiles(@"D:\gladius-extracted-archive\gc-compressed\AllModelsRenamed\characters", "*"));
            //filenames.Add(@"D:\gladius-extracted-archive\gc-compressed\AllModelsRenamed\characters\prop_practicepost1.mdl");

            //filenames.Add(@"D:\gladius-extracted-archive\gc-compressed\AllModelsRenamed\weapons\swordM_gladius.mdl");
            filenames.Add(@"D:\gladius-extracted-archive\gc-compressed\AllModelsRenamed\scorpion.mdl");
            foreach (string name in filenames)
            {
                reader.m_models.Add(reader.LoadSingleModel(name));
            }
            int ibreak2 = 0;


            HashSet<string> shadernames = new HashSet<string>();
            foreach (GCModel model in reader.m_models)
            {
                try
                {
                    //foreach (ShaderData sd in model.m_shaderData)
                    //{
                    //    shadernames.Add(sd.shaderName);
                    //}
                    ////model.DumpSections(tagOutputPath);
                    using (StreamWriter objSw = new StreamWriter(objOutputPath + model.m_name + ".obj"))
                    {
                        using (StreamWriter matSw = new StreamWriter(objOutputPath + model.m_name + ".mtl"))
                        {
                            model.WriteOBJ(objSw, matSw, texturePath);
                        }
                    }
                }
                catch (System.Exception ex)
                {

                }
            }
            String outputPath = @"D:\gladius-extracted-archive\gc-compressed\AllModelsRenamed";
            infoFile = @"D:\gladius-extracted-archive\gc-compressed\AllModelsRenamed-tag-output-results.txt";
            //using (StreamWriter objSw = new StreamWriter(@"d:\tmp\skin-data.txt"))
            //{
            //    reader.LoadModels(modelPath, infoFile, 400);
            //    foreach (GCModel model in reader.m_models)
            //    {
            //        objSw.WriteLine(String.Format("{0} ", model.m_name));
            //        for(int i=0;i<model.m_skinBlocks.Count;++i)
            //        {
            //            objSw.WriteLine(String.Format("\t Block[{0}]  NE[{1}] B[{2}] BS[{3}] Avg[{4}].",i,model.m_skinBlocks[i].numElements,model.m_skinBlocks[i].numBones,model.m_skinBlocks[i].blockSize,model.m_skinBlocks[i].blockSize/model.m_skinBlocks[i].numElements));
            //        }
            //    }
            //}


            //reader.LoadModels(modelPath, infoFile);
            //foreach (GCModel model in reader.m_models)
            //{
            //    //model.DumpSections(tagOutputPath);
            //    model.DumpSkinBlocks(outputPath);
            //    //model.DumpSections(outputPath);
            //    //model.DumpDisplayBlocks(outputPath);

            //}




        }


    }

    public class SkinBlock
    {
        public int blockSize;
        public int numElements;
        public int numBones;
        public byte[] fullBlock;
        public byte[] headerBlock;
        public byte[] weightBlock;
        public List<SkinElement> elements = new List<SkinElement>();
        public List<Vector3> m_points = new List<Vector3>();
        public List<Vector3> m_normals = new List<Vector3>();
        public int lodLevel;
        public int numPairs = 0;

        public static SkinBlock ReadBlock(BinaryReader reader)
        {
            int headerBlockSize = 116;
            long currentPos = reader.BaseStream.Position;

            SkinBlock block = new SkinBlock();
            block.blockSize = Common.ReadInt32BigEndian(reader);
            block.numElements = Common.ReadInt32BigEndian(reader);
            block.numBones = Common.ReadInt32BigEndian(reader);
            block.headerBlock = reader.ReadBytes(headerBlockSize);
            for (int i = 0; i < block.numElements; ++i)
            {
                SkinElement element = SkinElement.ReadElement(reader);
                block.elements.Add(element);
                block.m_points.Add(element.pos);

            }

            block.weightBlock = reader.ReadBytes(block.blockSize - (4 + 4 + 4 + headerBlockSize) - (block.numElements * SkinElement.Size));

            for (int i = 0; i < block.weightBlock.Length; )
            {
                if (block.weightBlock[i] + block.weightBlock[i + 1] == 256)
                {
                    block.numPairs++;
                }
                else
                {
                    break;
                }
                i += 2;
            }
            //block.remainderBlock = reader.ReadBytes(block.blockSize - (4 + 4 + 4 + headerBlockSize));

            reader.BaseStream.Position = currentPos;
            block.fullBlock = reader.ReadBytes(block.blockSize);
            return block;
        }

        public String PrettyPrint()
        {
            StringBuilder sb = new StringBuilder();
            int elementSize = 9;

            if (elements.Count > 0)
            {
                for (int i = 0; i < elements.Count; ++i)
                {
                    sb.AppendFormat("[{0:0.00000000}][{1:0.00000000}][{2:0.00000000}]\n", elements[i].pos.X, elements[i].pos.Y, elements[i].pos.Z);
                }
            }
            else
            {
                for (int i = 0; i < numElements * elementSize; ++i)
                {
                    if (i > 0 && i % elementSize == 0)
                    {
                        sb.AppendLine();
                    }
                    sb.AppendFormat("{0:X2}", weightBlock[i]);
                }
            }
            sb.AppendLine();
            for (int i = (numElements * elementSize); i < weightBlock.Length; ++i)
            {
                sb.AppendFormat("{0:X2}", weightBlock[i]);
            }
            return sb.ToString();
        }

    }

    public class SkinElement
    {
        public const int Size = 9;
        public Vector3 pos = new Vector3();
        public Vector3 normal = new Vector3();
        public byte[] rem;

        public static SkinElement ReadElement(BinaryReader reader)
        {
            SkinElement element = new SkinElement();
            element.pos.X = Common.FromStream2ByteToFloat(reader);
            element.pos.Y = Common.FromStream2ByteToFloat(reader);
            element.pos.Z = Common.FromStream2ByteToFloat(reader);

            // note sure of this remainder - possibly a 3 byte normal?
            //element.rem = reader.ReadBytes(3);
            byte[] extra = reader.ReadBytes(3);
            element.normal.X = Common.ByteToFloat(extra[0]);
            element.normal.Y = Common.ByteToFloat(extra[1]);
            element.normal.Z = Common.ByteToFloat(extra[2]);
            element.rem = extra;
            element.normal.Normalize();
            return element;
        }

    }


    // 0x0c , 0x0a, 0xc3 , 3 bones.

    public class DSLIInfo
    {
        public int startPos;
        public int length;

        public static DSLIInfo ReadStream(BinaryReader reader)
        {
            DSLIInfo info = new DSLIInfo();

            info.startPos = Common.ReadInt32BigEndian(reader);
            info.length = Common.ReadInt32BigEndian(reader);
            return info;
        }

    }

    // Info taken from : http://smashboards.com/threads/melee-dat-format.292603/
    // much appreciated.

    //http://www.falloutsoftware.com/tutorials/gl/gl3.htm

    //case 0xB8: // (GL_POINTS)
    //case 0xA8: // (GL_LINES)
    //case 0xB0: // (GL_LINE_STRIP)
    //case 0x90: // (GL_TRIANGLES)
    //case 0x98: // (GL_TRIANGLE_STRIP)
    //case 0xA0: // (GL_TRIANGLE_FAN)
    //case 0x80: // (GL_QUADS)
    public class DisplayListHeader : ModelSubMesh
    {
        public byte primitiveFlags;
        public short indexCount;
        public bool Valid = true;
        public List<DisplayListEntry> entries = new List<DisplayListEntry>();
        public byte dslcEntry;
        public DSLIInfo dsliInfo;
        public byte[] blockData;
        public float averageSize;
        public int averageSizeInt;
        public int adjustedSizeInt;
        public SkinBlock skinBlock;

        // these come from the mesh tag element...
        public int meshUnk1;
        public int meshUnk2;
        public int meshUnk3;
        public int meshUnk4;

        public GCModel m_model;

        public override List<Vector3> Vertices
        {
            get 
            { 
                return skinBlock != null ? skinBlock.m_points : m_model.m_pos; 
            }
        }

        public override List<Vector3> Normals
        {
            get { return skinBlock != null ? skinBlock.m_normals : m_model.m_normal; }
        }

        public override List<Vector2> UVs
        {
            get { return m_model != null ? m_model.m_uvs : null; }
        }

        public override List<ushort> Indices
        {
            get 
            {
                int ibreak = 0;
                return null;
            }
        }

        public override int NumIndices
        { get { return (int)indexCount; } }

        public override int NumVertices
        { get { return Vertices.Count(); } }

        public static bool FromStream(DisplayListHeader header, BinaryReader reader, DSLIInfo dsliInfo)
        {
            long currentPosition = reader.BaseStream.Position;
            header.blockData = reader.ReadBytes(dsliInfo.length);
            reader.BaseStream.Position = currentPosition;
            if (true)
            {
                int headerSize = 6; // display type byte, uint16,display type byte, uint16
                byte header1 = reader.ReadByte();
                short pad1 = reader.ReadInt16();
                header.primitiveFlags = reader.ReadByte();
                header.indexCount = Common.ToInt16BigEndian(reader);
                if (header.indexCount == 0)
                {
                    int ibreak = 0;
                }


                header.averageSize = header.indexCount != 0 ? (((float)header.blockData.Length - headerSize) / (float)header.indexCount) : 0.0f;
                header.averageSizeInt = (int)header.averageSize;

                reader.BaseStream.Position += (dsliInfo.length - headerSize);

                int foundVal = 0;

                for (int i = 4; i < 9; ++i)
                {
                    int toCheck = headerSize + (header.indexCount * i);
                    if (toCheck < header.blockData.Length)
                    {

                        bool allzero = true;
                        for (int j = toCheck; j < header.blockData.Length; ++j)
                        {
                            if (header.blockData[j] != 0x00)
                            {
                                allzero = false;
                                foundVal = i;

                                break;
                            }
                        }

                        if (allzero)
                        {
                            break;
                        }
                    }
                }
                foundVal += 1;

                header.adjustedSizeInt = foundVal;
            }

            reader.BaseStream.Position = currentPosition;
            bool success = false;

            if (true)
            {
                byte header1 = reader.ReadByte();
                //Debug.Assert(header1 == 0x98);
                short pad1 = reader.ReadInt16();

                //header = new DisplayListHeader();
                header.primitiveFlags = reader.ReadByte();
                //Debug.Assert(header.primitiveFlags== 0x090);
                if (header.primitiveFlags == 0x90 || header.primitiveFlags == 0x00)
                {
                    header.indexCount = Common.ToInt16BigEndian(reader);
                    if (header.indexCount == 0)
                    {
                        int ibreak = 0;
                    }

                    success = true;
                    for (int i = 0; i < header.indexCount; ++i)
                    {
                        DisplayListEntry e = DisplayListEntry.FromStream(reader, header.adjustedSizeInt);
                        header.entries.Add(e);
                        header.MaxVertex = Math.Max(header.MaxVertex, e.PosIndex);
                        header.MaxNormal = Math.Max(header.MaxNormal, e.NormIndex);
                        header.MaxUV = Math.Max(header.MaxUV, e.UVIndex);
                    }
                }
                else
                {
                    reader.BaseStream.Position = currentPosition;
                }
            }
            return success;
        }
    }



    public struct DisplayListEntry
    {
        public ushort PosIndex;
        public ushort NormIndex;
        public ushort UVIndex;
        public ushort UVIndex2;

        public byte oddByte;



        public String ToString()
        {
            return "P:" + PosIndex + " N:" + NormIndex + " U:" + UVIndex + " U2:" + UVIndex2 + " OB: " + oddByte;
        }

        public static DisplayListEntry FromStream(BinaryReader reader, int sectionSize)
        {
            DisplayListEntry entry = new DisplayListEntry();


            // section size = 4,5,6,7,8,9
            if (sectionSize >= 4)
            {
                entry.PosIndex = Common.ToUInt16BigEndian(reader);
                entry.NormIndex = Common.ToUInt16BigEndian(reader);
            }


            if (sectionSize == 5)
            {
                entry.oddByte = reader.ReadByte();
            }
            else if (sectionSize == 6)
            {
                entry.UVIndex = Common.ToUInt16BigEndian(reader);

            }
            else if (sectionSize == 7)
            {
                entry.oddByte = reader.ReadByte(); // this is probably the texture index...
                entry.UVIndex = Common.ToUInt16BigEndian(reader);

            }
            else if (sectionSize == 8)
            {
                entry.UVIndex = Common.ToUInt16BigEndian(reader);
                entry.UVIndex2 = Common.ToUInt16BigEndian(reader);


            }
            else if (sectionSize == 9)
            {
                entry.UVIndex = Common.ToUInt16BigEndian(reader);
                entry.UVIndex2 = Common.ToUInt16BigEndian(reader);
                entry.oddByte = reader.ReadByte();

            }


            return entry;
        }
    }


    public class TextureData
    {
        public string textureName;
        public int minusOne;
        public int unknown;
        public int width;
        public int height;
        public int three;
        public int zero;


        public static TextureData FromStream(BinaryReader binReader)
        {
            TextureData textureData = new TextureData();
            StringBuilder sb = new StringBuilder();
            int count = 0;
            char b;
            int textureNameLength = 0x80;
            for(int i=0;i<textureNameLength;++i)
            {
                b = (char)binReader.ReadByte();
                if(b != 0x00)
                {
                    sb.Append(b);
                }
            }

            String textureName = sb.ToString();
            textureData.textureName = textureName;
            textureData.minusOne = binReader.ReadInt32();
            textureData.unknown = binReader.ReadInt32();
            textureData.width = binReader.ReadInt32();
            textureData.height = binReader.ReadInt32();
            textureData.three = binReader.ReadInt32();
            textureData.zero = binReader.ReadInt32();

            //Debug.Assert(textureData.three == 3);
            //Debug.Assert(textureData.zero == 0);

            return textureData;
        }


    }

}
