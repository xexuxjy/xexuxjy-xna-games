using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OpenTK;
using System.Diagnostics;

namespace ModelNamer
{



    public class GCModel
    {
        public GCModel(String name)
        {
            m_name = name;
        }

        public void LoadModelTags(BinaryReader binReader)
        {
            foreach (char[] tag in Common.allTags)
            {
                // reset for each so we don't worry about order
                binReader.BaseStream.Position = 0;
                if (Common.FindCharsInStream(binReader, tag, true))
                {
                    TagSizeAndData tsad = TagSizeAndData.Create(binReader);

                    m_tagSizes[tag] = tsad;
                }
                else
                {
                    m_tagSizes[tag] = new TagSizeAndData(-1);
                }
            }
        }

        public void DumpSections(String fileOutputDir)
        {
            fileOutputDir += "-tag-output";
            foreach (char[] tag in m_tagSizes.Keys)
            {
                try
                {
                    TagSizeAndData tsad = m_tagSizes[tag];
                    if (tsad.length > 0)
                    {

                        String tagOutputDirname = fileOutputDir + "/" + m_name + "/";
                        try
                        {
                            Directory.CreateDirectory(tagOutputDirname);
                        }
                        catch (Exception e) { }

                        String tagOutputFilename = tagOutputDirname + "/" + new String(tag);
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

            foreach (DisplayListHeader headerBlock in m_displayListHeaders)
            {

                String tagOutputFilename = tagOutputDirname + "/" + "DSLS-" + counter;
                using (System.IO.StreamWriter outStream = new StreamWriter(File.Open(tagOutputFilename, FileMode.Create)))
                {
                    outStream.WriteLine(String.Format("DSLC[{0:X2}]", headerBlock.dslcEntry));
                    
                    outStream.WriteLine(String.Format("DSLI[{0}][{1}][{2:0.0}][{3}]", headerBlock.dsliInfo.startPos, headerBlock.dsliInfo.length,headerBlock.averageSize,headerBlock.adjustedSizeInt));
                    outStream.WriteLine(String.Format("P[{0}]N[{1}]U[{2}] MP[{3}]MN[{4}]MU[{5}]", headerBlock.maxVertex,headerBlock.maxNormal,headerBlock.maxUV,m_points.Count,m_normals.Count,m_uvs.Count));

                    int headerSize = 6;
                    outStream.WriteLine(Common.ByteArrayToStringSub(headerBlock.blockData,0,headerSize));


                    
                    if(headerBlock.adjustedSizeInt == 4)
                    {
                        for(int i=0;i<headerBlock.indexCount;++i)
                        {
                            int index = headerSize+(i*4);
                            outStream.WriteLine(String.Format("{0:X2}{1:X2} {2:X2}{3:X2}",headerBlock.blockData[index+0],headerBlock.blockData[index+1],headerBlock.blockData[index+2],headerBlock.blockData[index+3]));
                        }
                    }
                    else if(headerBlock.adjustedSizeInt == 5)
                    {
                        for(int i=0;i<headerBlock.indexCount;++i)
                        {
                            int index = headerSize + (i * 5);
                            outStream.WriteLine(String.Format("{0:X2}{1:X2} {2:X2}{3:X2} {4:X2}",headerBlock.blockData[index+0],headerBlock.blockData[index+1],headerBlock.blockData[index+2],headerBlock.blockData[index+3],headerBlock.blockData[index+4]));
                        }
                    }
                    else if(headerBlock.adjustedSizeInt == 6)
                    {
                        for(int i=0;i<headerBlock.indexCount;++i)
                        {
                            int index = headerSize + (i * 6);
                            outStream.WriteLine(String.Format("{0:X2}{1:X2} {2:X2}{3:X2} {4:X2}{5:X2}",headerBlock.blockData[index+0],headerBlock.blockData[index+1],headerBlock.blockData[index+2],headerBlock.blockData[index+3],headerBlock.blockData[index+4],headerBlock.blockData[index+5]));
                        }
                    }
                    else if(headerBlock.adjustedSizeInt == 7)
                    {
                        for(int i=0;i<headerBlock.indexCount;++i)
                        {
                            int index = headerSize + (i * 7);
                            outStream.WriteLine(String.Format("{0:X2}{1:X2} {2:X2}{3:X2} {4:X2} {5:X2}{6:X2}",headerBlock.blockData[index+0],headerBlock.blockData[index+1],headerBlock.blockData[index+2],headerBlock.blockData[index+3],headerBlock.blockData[index+4],headerBlock.blockData[index+5],headerBlock.blockData[index+6]));
                        }
                    }
                    else if(headerBlock.adjustedSizeInt == 8)
                    {
                        for(int i=0;i<headerBlock.indexCount;++i)
                        {
                            int index = headerSize + (i * 8);
                            outStream.WriteLine(String.Format("{0:X2}{1:X2} {2:X2}{3:X2} {4:X2}{5:X2} {6:X2}{7:X2}",headerBlock.blockData[index+0],headerBlock.blockData[index+1],headerBlock.blockData[index+2],headerBlock.blockData[index+3],headerBlock.blockData[index+4],headerBlock.blockData[index+5],headerBlock.blockData[index+6],headerBlock.blockData[index+7]));
                        }
                    }
                    else
                    {
                        int ibreak =0;
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
                    counter++;
                }
            }
        }

        public void BuildBB(List<Vector3> points, DisplayListHeader header)
        {
            Vector3 min = new Vector3(float.MaxValue);
            Vector3 max = new Vector3(float.MinValue);

            int count = 0;
            foreach(DisplayListEntry entry in header.entries)
            {

                count++;
                if (entry.PosIndex < points.Count)
                {
                    if (points[entry.PosIndex].X < min.X) min.X = points[entry.PosIndex].X;
                    if (points[entry.PosIndex].Y < min.Y) min.Y = points[entry.PosIndex].Y;
                    if (points[entry.PosIndex].Z < min.Z) min.Z = points[entry.PosIndex].Z;
                    if (points[entry.PosIndex].X > max.X) max.X = points[entry.PosIndex].X;
                    if (points[entry.PosIndex].Y > max.Y) max.Y = points[entry.PosIndex].Y;
                    if (points[entry.PosIndex].Z > max.Z) max.Z = points[entry.PosIndex].Z;
                }
                else
                {
                    int ibreak = 0;
                }
            }
            header.MinBB = min;
            header.MaxBB = max;
        }

        public static void BuildBB(List<Vector3> points, out Vector3 omin, out Vector3 omax)
        {
            Vector3 min = new Vector3(float.MaxValue);
            Vector3 max = new Vector3(float.MinValue);

            for (int i = 0; i < points.Count; ++i)
            {
                if (points[i].X < min.X) min.X = points[i].X;
                if (points[i].Y < min.Y) min.Y = points[i].Y;
                if (points[i].Z < min.Z) min.Z = points[i].Z;
                if (points[i].X > max.X) max.X = points[i].X;
                if (points[i].Y > max.Y) max.Y = points[i].Y;
                if (points[i].Z > max.Z) max.Z = points[i].Z;

            }
            omin = min;
            omax = max;

        }



        public void BuildBB()
        {
            if (!m_builtBB)
            {
                // build bb for main model, and also for each sub-mesh
                Vector3 min = new Vector3(float.MaxValue);
                Vector3 max = new Vector3(float.MinValue);

                if (m_skinned)
                {
                    int count = 0;
                    foreach (DisplayListHeader header in m_displayListHeaders)
                    {
                        if (m_skinned)
                        {
                            if(count< m_skinBlocks.Count)
                            {
                                header.skinBlock = m_skinBlocks[count];
                            }
                            else
                            {
                                int ibreak = 0;
                            }
                        }
                        try
                        {
                            BuildBB(header.skinBlock.m_points, header);
                            min = Vector3.ComponentMin(header.MinBB, min);
                            max = Vector3.ComponentMax(header.MaxBB, max);
                        }
                        catch (System.Exception ex)
                        {
                            int ibreak = 0;
                        }
                        count++;

                    }


                }
                else
                {
                    BuildBB(m_points, out min, out max);
                }
                

                MinBB = min;
                MaxBB = max;

                m_builtBB = true;
            }
        }

        public void ConstructSkeleton()
        {
            Dictionary<int, BoneNode> dictionary = new Dictionary<int, BoneNode>();
            foreach (BoneNode node in m_bones)
            {
                dictionary[node.id] = node;
            }

            foreach (BoneNode node in m_bones)
            {
                if (node.id != node.parentId)
                {
                    BoneNode parent = dictionary[node.parentId];
                    parent.children.Add(node);
                    node.parent = parent;
                }
            }
         
            
            if (m_bones.Count > 0 && false)
            {

                m_rootBone = m_bones[0];
                CalcBindFinalMatrix(m_rootBone, Matrix4.Identity);

                if (!m_builtBB)
                {
                    Vector3 min = new Vector3(float.MaxValue);
                    Vector3 max = new Vector3(float.MinValue);


                    foreach (BoneNode node in m_bones)
                    {
                        // build tranform from parent chain?
                        Vector3 offset = node.finalMatrix.ExtractTranslation();

                        if (offset.X < min.X) min.X = offset.X;
                        if (offset.Y < min.Y) min.Y = offset.Y;
                        if (offset.Z < min.Z) min.Z = offset.Z;
                        if (offset.X > max.X) max.X = offset.X;
                        if (offset.Y > max.Y) max.Y = offset.Y;
                        if (offset.Z > max.Z) max.Z = offset.Z;

                    }

                    MinBB = min;
                    MaxBB = max;
                    m_builtBB = true;
                }


            }

        }

        void CalcBindFinalMatrix(BoneNode bone, Matrix4 parentMatrix)
        {
            bone.combinedMatrix = bone.localMatrix * parentMatrix;
            //bone.finalMatrix = bone.offsetMatrix * bone.combinedMatrix;
            bone.finalMatrix = bone.combinedMatrix;

            foreach (BoneNode child in bone.children)
            {
                CalcBindFinalMatrix(child, bone.combinedMatrix);
            }
        }


        public void ReadDSLISection(BinaryReader binReader)
        {
            if (Common.FindCharsInStream(binReader, Common.dsliTag, true))
            {
                int blockSize = binReader.ReadInt32();
                int pad1 = binReader.ReadInt32();
                int pad2 = binReader.ReadInt32();
                int numSections = (blockSize - 8 - 4 - 4) / 8;

                for (int i = 0; i < numSections; ++i)
                {
                    DSLIInfo info = DSLIInfo.ReadStream(binReader);
                    if (m_skinned)
                    {
                        //info.length /= 2;
                        info.startPos *= 2;
                    }

                    if (info.length > 0)
                    {
                        m_dsliInfos.Add(info);

                        DisplayListHeader header = new DisplayListHeader();
                        header.dsliInfo = info;
                        m_displayListHeaders.Add(header);
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
                Debug.Assert(numSections == m_displayListHeaders.Count());
                for (int i = 0; i < numSections; ++i)
                {
                    DisplayListHeader header = m_displayListHeaders[i];
                    header.meshUnk1 = binReader.ReadInt32();
                    header.meshUnk2 = binReader.ReadInt32();
                    header.meshId = binReader.ReadInt32();
                    header.meshUnk3 = binReader.ReadInt32();
                    header.meshUnk4 = binReader.ReadInt32();
                    header.lodlevel = binReader.ReadInt32();
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

                for (int i = 0; i < numBones; ++i)
                {
                    BoneNode node = BoneNode.FromStream(binReader);
                    node.name = m_names[i];
                    m_bones.Add(node);
                }
                ConstructSkeleton();
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
                    DisplayListHeader header = m_displayListHeaders[i];
                    header.dslcEntry = binReader.ReadByte();
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
                    if (i == 8)
                    {
                        int ibreak = 0;
                    }
                    binReader.BaseStream.Position = startPos + m_dsliInfos[i].startPos;
                    DisplayListHeader.FromStream(m_displayListHeaders[i],binReader, m_dsliInfos[i]);

                }
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
                for (int i = 0; i < numElements; ++i)
                {
                    m_skinBlocks.Add(SkinBlock.ReadBlock(binReader));
                }
            }
        }


        public void Validate()
        {
            foreach (DisplayListHeader header in m_displayListHeaders)
            {
                if (header.primitiveFlags == 0x90)
                {

                    for (int i = 0; i < header.entries.Count; ++i)
                    {
                        if (header.entries[i].PosIndex > m_maxVertex)
                        {
                            m_maxVertex = header.entries[i].PosIndex;
                        }
                        if (header.entries[i].NormIndex > m_maxNormal)
                        {
                            m_maxNormal = header.entries[i].NormIndex;
                        }

                        if (header.entries[i].UVIndex > m_maxUv)
                        {
                            m_maxUv = header.entries[i].UVIndex;
                        }

                        if (header.entries[i].PosIndex < 0 || header.entries[i].PosIndex >= m_points.Count)
                        {
                            header.Valid = false;
                            //break;
                        }
                        if (header.entries[i].NormIndex < 0 || header.entries[i].NormIndex >= m_normals.Count)
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

        public void WriteOBJ(StreamWriter writer, StreamWriter materialWriter)
        {
            // write material?
            foreach (String name in m_textures)
            {
                String textureName = name + ".png";
                materialWriter.WriteLine("newmtl " + textureName);
                materialWriter.WriteLine("Ka 1.000 1.000 1.000");
                materialWriter.WriteLine("Kd 1.000 1.000 1.000");
                materialWriter.WriteLine("Ks 0.000 0.000 0.000");
                materialWriter.WriteLine("d 1.0");
                materialWriter.WriteLine("illum 2");
                materialWriter.WriteLine("map_Ka ../Textures/" + textureName);
                materialWriter.WriteLine("map_Kd ../Textures/" + textureName);
            }

            writer.WriteLine("mtllib " + m_name + ".mtl");
            // and now points, uv's and normals.
            foreach (Vector3 v in m_points)
            {
                writer.WriteLine(String.Format("v {0:0.00000} {1:0.00000} {2:0.00000}", v.X, v.Y, v.Z));
            }
            foreach (Vector2 v in m_uvs)
            {
                writer.WriteLine(String.Format("vt {0:0.00000} {1:0.00000}", v.X, 1.0f - v.Y));
            }
            foreach (Vector3 v in m_points)
            {
                writer.WriteLine(String.Format("vn {0:0.00000} {1:0.00000} {2:0.00000}", v.X, v.Y, v.Z));
            }

            writer.WriteLine("usemtl " + m_textures[m_textures.Count - 1] + ".png");

            foreach (DisplayListHeader dlh in m_displayListHeaders)
            {
                int counter = 0;
                int offset = 1;
                for (int i = 0; i < dlh.entries.Count; )
                {
                    writer.WriteLine(String.Format("f {0}/{1}/{2} {3}/{4}/{5} {6}/{7}/{8}", dlh.entries[i].PosIndex + offset, dlh.entries[i].UVIndex + offset, dlh.entries[i].NormIndex + offset,
                        dlh.entries[i + 1].PosIndex + offset, dlh.entries[i + 1].UVIndex + offset, dlh.entries[i + 1].NormIndex + offset,
                        dlh.entries[i + 2].PosIndex + offset, dlh.entries[i + 2].UVIndex + offset, dlh.entries[i + 2].NormIndex + offset));
                    i += 3;
                }
            }
        }

        public bool Valid
        {
            get
            {
                if (m_displayListHeaders.Count == 0)
                {
                    return false;
                }
                foreach (DisplayListHeader dlh in m_displayListHeaders)
                {
                    if (dlh.Valid == false)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public Dictionary<char[], TagSizeAndData> m_tagSizes = new Dictionary<char[], TagSizeAndData>();
        public String m_name;
        public List<Vector3> m_points = new List<Vector3>();
        public List<Vector3> m_normals = new List<Vector3>();
        public List<Vector2> m_uvs = new List<Vector2>();
        public List<String> m_textures = new List<String>();
        public List<String> m_names = new List<String>();
        public List<DSLIInfo> m_dsliInfos = new List<DSLIInfo>();
        public List<Vector3> m_centers = new List<Vector3>();
        public List<String> m_selsInfo = new List<string>();
        public List<DisplayListHeader> m_displayListHeaders = new List<DisplayListHeader>();
        public List<SkinBlock> m_skinBlocks = new List<SkinBlock>();
        public List<ShaderData> m_shaderData = new List<ShaderData>();
        public Vector3 MinBB;
        public Vector3 MaxBB;
        public Vector3 Center;
        public List<Matrix4> m_matrices = new List<Matrix4>();
        public List<BoneNode> m_bones = new List<BoneNode>();
        private bool m_builtBB = false;
        public bool m_skinned = false;
        public bool m_hasSkeleton = false;
        public int m_maxVertex;
        public int m_maxNormal;
        public int m_maxUv;

        public BoneNode m_rootBone;

        //public bool Valid =true;
    }

    public class GCModelReader
    {
        public List<GCModel> m_models = new List<GCModel>();

        public void LoadModels()
        {
            LoadModels(@"c:\tmp\unpacking\gc-models\", @"c:\tmp\unpacking\gc-models\results.txt");
        }

        public void LoadModelsTags()
        {
            LoadModels(@"c:\tmp\unpacking\gc-models\", @"c:\tmp\unpacking\gc-models\results.txt");
        }


        public GCModel LoadSingleModel(String modelPath, bool readDisplayLists = true)
        {
            FileInfo sourceFile = new FileInfo(modelPath);

            using (BinaryReader binReader = new BinaryReader(new FileStream(sourceFile.FullName, FileMode.Open)))
            {
                GCModel model = new GCModel(sourceFile.Name);

                model.LoadModelTags(binReader);
                binReader.BaseStream.Position = 0;

                Common.ReadTextureNames(binReader, Common.txtrTag, model.m_textures);
                
                // Look for skeleton and skin if they exist. need to load names first.
                binReader.BaseStream.Position = 0;
                Common.ReadNullSeparatedNames(binReader, Common.nameTag, model.m_names);
                binReader.BaseStream.Position = 0;
                model.ReadSKELSection(binReader);
                binReader.BaseStream.Position = 0;
                model.ReadSkinSection(binReader);
                binReader.BaseStream.Position = 0;
                model.ReadSHDRSection(binReader);
                binReader.BaseStream.Position = 0;
                model.ReadDSLISection(binReader);
                binReader.BaseStream.Position = 0;
                model.ReadDSLCSection(binReader);
                binReader.BaseStream.Position = 0;
                model.ReadDSLSSection(binReader);
                binReader.BaseStream.Position = 0;
                model.ReadMESHSection(binReader);

                binReader.BaseStream.Position = 0;

                if (Common.FindCharsInStream(binReader, Common.cntrTag, true))
                {
                    //int blockSize = binReader.ReadInt32();
                    //int unk2 = binReader.ReadInt32();
                    //int unk3 = binReader.ReadInt32();
                    //for (int i = 0; i < model.m_dsliInfos.Count; ++i)
                    //{
                    //    model.m_matrices.Add(Common.FromStreamMatrix4BE(binReader));
                    //}
                    //int ibreak = 0;
                }
                // not skinned so look for fixed version.l
                if (model.m_skinned == false)
                {
                    if (Common.FindCharsInStream(binReader, Common.posiTag))
                    {
                        int posSectionLength = binReader.ReadInt32();

                        int uk2 = binReader.ReadInt32();
                        int numPoints = binReader.ReadInt32();
                        for (int i = 0; i < numPoints; ++i)
                        {
                            model.m_points.Add(Common.FromStreamVector3BE(binReader));
                        }
                    }

                    if (Common.FindCharsInStream(binReader, Common.normTag))
                    {
                        int normSectionLength = binReader.ReadInt32();
                        int uk4 = binReader.ReadInt32();
                        int numNormals = binReader.ReadInt32();

                        for (int i = 0; i < numNormals; ++i)
                        {
                            model.m_normals.Add(Common.FromStreamVector3BE(binReader));
                        }


                    }
                }

                binReader.BaseStream.Position = 0;

                if (Common.FindCharsInStream(binReader, Common.uv0Tag))
                {
                    int normSectionLength = binReader.ReadInt32();
                    int uk4 = binReader.ReadInt32();
                    int numUVs = binReader.ReadInt32();

                    // normal model has uv's as 8 bytes (2 floats) per block.
                    // skinned model has ub's as 4 bytes (2???) per block...

                    if (model.m_skinned)
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


                            //float a = (((float)binReader.ReadByte()) / 255.0f);
                            //binReader.ReadByte();
                            //float b = (((float)binReader.ReadByte()) / 255.0f);
                            //binReader.ReadByte();
                            model.m_uvs.Add(new Vector2(a,b));
                            
                        }
                    }
                    else
                    {
                        for (int i = 0; i < numUVs; ++i)
                        {
                            model.m_uvs.Add(Common.FromStreamVector2BE(binReader));
                        }
                    }





                }
                model.BuildBB();
                //model.Validate();
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
                        GCModel model = LoadSingleModel(file);
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
            using (System.IO.StreamWriter infoStream = new System.IO.StreamWriter(infoFile))
            {
                foreach (GCModel model in m_models)
                {
                    infoStream.WriteLine(String.Format("File : {0} : {1} : {2}", model.m_name, model.m_points.Count, model.m_normals.Count));
                    infoStream.WriteLine("Verts : ");
                    foreach (Vector3 sv in model.m_points)
                    {
                        Common.WriteInt(infoStream, sv);
                    }
                    infoStream.WriteLine("Normals : ");
                    foreach (Vector3 sv in model.m_normals)
                    {
                        Common.WriteInt(infoStream, sv);
                    }
                    infoStream.WriteLine();
                    infoStream.WriteLine();
                }
            }

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

                        if (sourceFile.Name != "File 005496")
                        {
                            //continue;
                        }


                        using (BinaryReader binReader = new BinaryReader(new FileStream(sourceFile.FullName, FileMode.Open)))
                        {
                            GCModel model = new GCModel(sourceFile.Name);
                            m_models.Add(model);
                            infoStream.WriteLine("File : " + model.m_name);


                            model.LoadModelTags(binReader);

                            binReader.BaseStream.Position = 0;
                            model.ReadDSLISection(binReader);

                            binReader.BaseStream.Position = 0;
                            model.ReadDSLSSection(binReader);

                            binReader.BaseStream.Position = 0;

                            Common.ReadNullSeparatedNames(binReader, Common.selsTag, model.m_selsInfo);
                            Common.ReadNullSeparatedNames(binReader, Common.nameTag, model.m_names);
                            Common.ReadTextureNames(binReader, Common.txtrTag, model.m_textures);

                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("DSLI : ");
                            foreach (DSLIInfo dsliInfo in model.m_dsliInfos)
                            {
                                sb.AppendLine(String.Format("\t {0} {1}", dsliInfo.startPos, dsliInfo.length));
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
                            foreach (string textureName in model.m_textures)
                            {
                                sb.AppendLine("\t" + textureName);
                            }

                            infoStream.WriteLine(sb.ToString());



                        }
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
            String modelPath = @"C:\tmp\unpacking\probable-skinned-models";
            String infoFile = @"c:\tmp\unpacking\gc-models\results.txt";
            //String sectionInfoFile = @"C:\tmp\unpacking\gc-probable-models-renamed\sectionInfo.txt";
            String objOutputPath = @"D:\gladius-extracted-archive\gc-compressed\test-models\obj-models\";
            //String tagOutputPath = @"C:\tmp\unpacking\xbox-ModelFiles\tag-output";

            modelPath = @"D:\gladius-extracted-archive\gc-compressed\probable-skinned-models";
            infoFile = @"D:\gladius-extracted-archive\gc-compressed\test-models\probable-models-renamed-info.txt";

            String tagOutputPath = @"D:\gladius-extracted-archive\gc-compressed\probable-skinned-models\tag-output";

            //sectionInfoFile = @"D:\gladius-extracted-archive\gc-compressed\probable-models-renamed-sectionInfo.txt";

            GCModelReader reader = new GCModelReader();
            //GCModel model = reader.LoadSingleModel(@"D:\gladius-extracted-archive\gc-compressed\test-models\bow.mdl");
            //GCModel model = reader.LoadSingleModel(@"D:\gladius-extracted-archive\gc-compressed\test-models\File 015227");
            int ibreak = 0;
            //reader.LoadModels(modelPath, infoFile,400);
            //foreach (GCModel model in reader.m_models)
            //{
            //    model.DumpSections(tagOutputPath);
            //    //using(StreamWriter objSw = new StreamWriter(objOutputPath+model.m_name+".obj"))
            //    //{
            //    //    using(StreamWriter matSw = new StreamWriter(objOutputPath+model.m_name+".mtl"))
            //    //    {
            //    //        model.WriteOBJ(objSw,matSw);
            //    //    }
            //    //}

            //}
            //reader.LoadModels(modelPath,infoFile);
            //reader.DumpPoints(infoFile);
            //reader.DumpSectionLengths(modelPath, sectionInfoFile);

            modelPath = @"D:\gladius-extracted-archive\gc-compressed\AllModelsRenamed";
            //String outputPath = @"D:\gladius-extracted-archive\gc-compressed\AllModelsRenamed-dsli-output";
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


            //reader.LoadModels(modelPath, infoFile,5);
            reader.m_models.Add(reader.LoadSingleModel(@"D:\gladius-extracted-archive\gc-compressed\AllModelsRenamed\arcane_earth_crown.mdl"));
            foreach (GCModel model in reader.m_models)
            {
                model.DumpSkinBlocks(outputPath);
                model.DumpSections(outputPath);
                model.DumpDisplayBlocks(outputPath);

            }
        }


    }

    public class SkinBlock
    {
        public int blockSize;
        public int numElements;
        public int numBones;
        public byte[] fullBlock;
        public byte[] headerBlock;
        public byte[] remainderBlock;
        public List<SkinElement> elements = new List<SkinElement>();
        public List<Vector3> m_points = new List<Vector3>();
        public List<Vector3> m_normals = new List<Vector3>();
        public int lodLevel;


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

            block.remainderBlock = reader.ReadBytes(block.blockSize - (4 + 4 + 4 + headerBlockSize) - (block.numElements*SkinElement.Size));
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
                    sb.AppendFormat("{0:X2}", remainderBlock[i]);
                }
            }
            sb.AppendLine();
            for (int i = (numElements * elementSize); i < remainderBlock.Length; ++i)
            {
                sb.AppendFormat("{0:X2}", remainderBlock[i]);
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
            element.normal.X = Common.ByteToFloat(reader.ReadByte());
            element.normal.Y = Common.ByteToFloat(reader.ReadByte());
            element.normal.Z = Common.ByteToFloat(reader.ReadByte());

            element.normal.Normalize();
            return element;
        }

    }


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
    public class DisplayListHeader
    {
        public byte primitiveFlags;
        public short indexCount;
        public bool Valid = true;
        public List<DisplayListEntry> entries = new List<DisplayListEntry>();
        public Vector3 MinBB = new Vector3();
        public Vector3 MaxBB = new Vector3();
        public byte dslcEntry;
        public DSLIInfo dsliInfo;
        public byte[] blockData;
        public float averageSize;
        public int averageSizeInt;
        public int adjustedSizeInt;
        public int maxVertex;
        public int maxNormal;
        public int maxUV;
        public SkinBlock skinBlock;

        // these come from the mesh tag element...
        public int meshId;
        public int lodlevel;
        public int meshUnk1;
        public int meshUnk2;
        public int meshUnk3;
        public int meshUnk4;


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
                header.averageSize = header.indexCount != 0 ? (((float)header.blockData.Length-headerSize) / (float)header.indexCount) : 0.0f;
                header.averageSizeInt = (int)header.averageSize;

                reader.BaseStream.Position += (dsliInfo.length - headerSize);

                int foundVal = 0;

                for (int i = 4; i < 9; ++i)
                {
                    int toCheck = headerSize + (header.indexCount * i);
                    if (toCheck< header.blockData.Length)
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

                    success = true;
                    for (int i = 0; i < header.indexCount; ++i)
                    {
                        DisplayListEntry e = DisplayListEntry.FromStream(reader, header.adjustedSizeInt);
                        header.entries.Add(e);
                                                header.maxVertex = Math.Max(header.maxVertex, e.PosIndex);
                        header.maxNormal = Math.Max(header.maxNormal, e.NormIndex);
                        header.maxUV = Math.Max(header.maxUV, e.UVIndex);
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


    public class BoneNode
    {
        public String name;
        public Vector3 offset;
        public Quaternion rotation;
        public byte pad1;
        public byte id;
        public byte id2;
        public byte parentId;

        public Matrix4 rotationMatrix;
        public Matrix4 combinedMatrix;
        public Matrix4 finalMatrix;
        public Matrix4 localMatrix;

        public BoneNode parent;
        public List<BoneNode> children = new List<BoneNode>();

        public static BoneNode FromStream(BinaryReader binReader)
        {
            BoneNode node = new BoneNode();
            node.offset = Common.FromStreamVector3(binReader);
            node.rotation = Common.FromStreamQuaternion(binReader);
            Debug.Assert(Common.FuzzyEquals(node.rotation.Length, 1.0f, 0.0001f));

            node.pad1 = binReader.ReadByte();
            node.id = binReader.ReadByte();
            node.id2 = binReader.ReadByte();
            node.parentId = binReader.ReadByte();

            node.rotationMatrix = Matrix4.CreateRotationX(node.rotation.X) * Matrix4.CreateRotationY(node.rotation.Y) * Matrix4.CreateRotationZ(node.rotation.Z);
            node.localMatrix = Matrix4.CreateTranslation(node.offset) * node.rotationMatrix;

            return node;
        }
    }

    public class ShaderData
    {
        public String shaderName;
        public String textureName;
        public int emptyTag1;
        public int unk1;
        public int unk2;
        public int unk3;
        public byte textureId1;
        public byte textureId2;
        public byte[] unkba1;
        public int unk4;
        public int unk5;
        public int unk6;
        public int unk7;



        public static ShaderData FromStream(BinaryReader binReader)
        {
            ShaderData shader = new ShaderData();
            shader.emptyTag1 = binReader.ReadInt32();

            byte[] name = binReader.ReadBytes(124);
            StringBuilder sb = new StringBuilder();
            char b;
            for(int i=0;i<name.Length;++i)
            {
                b = (char)name[i];
                if (b == 0x00)
                {
                    break;
                }
                sb.Append(b);
            }
            shader.shaderName = sb.ToString();

            shader.unk1 = binReader.ReadInt32();
            shader.unk2 = binReader.ReadInt32();
            shader.unk3 = binReader.ReadInt32();
            shader.textureId1 = binReader.ReadByte();
            shader.textureId2 = binReader.ReadByte();
            shader.unkba1 = binReader.ReadBytes(6);
            shader.unk4 = binReader.ReadInt32();
            shader.unk5 = binReader.ReadInt32();
            shader.unk6 = binReader.ReadInt32();
            shader.unk7 = binReader.ReadInt32();
            return shader;
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
            return "P:" + PosIndex + " N:" + NormIndex + " U:" + UVIndex + " U2:"+UVIndex2+" OB: "+oddByte;
        }

        public static DisplayListEntry FromStream(BinaryReader reader,int sectionSize)
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

}
