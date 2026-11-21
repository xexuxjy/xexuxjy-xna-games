using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Editor.Test
{
    [CustomEditor(typeof(TestStripLODLevelStub))]
    public class TestStripLODLevel : UnityEditor.Editor
    {

        private List<(bool, bool, bool,bool)> m_foldOutsCompareList = new List<(bool, bool, bool,bool)>();

        public GCModel m_originalModel;
        public GCModel m_rebuiltModel;

        public override void OnInspectorGUI()
        {
              TestStripLODLevelStub stub = target as TestStripLODLevelStub;

            base.OnInspectorGUI();

            if (GUILayout.Button("Strip To LOD"))
            {
                using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(stub.OriginalTextAsset.bytes)))
                {
                    m_originalModel = GCModel.ReadData(binaryReader, "", null);
                }

                using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(stub.OriginalTextAsset.bytes)))
                {
                    m_rebuiltModel = GCModel.ReadData(binaryReader, "", null);
                }

                
                if (m_originalModel != null)
                {
                    
                    // which elements match out LOD level.
                    List<int> filteredIndicies = new List<int>();
                    
                    MESHChunk meshChunk = m_originalModel.GetChunk<MESHChunk>();
                    for (int i = 0; i < meshChunk.PaxElements.Count; i++)
                    {
                        PaxElement paxElement = meshChunk.PaxElements[i];
                        if ((paxElement.SelectSetMask & stub.LodLevel) == stub.LodLevel)
                        {
                            filteredIndicies.Add(i);    
                        }
                    }

                    int totalMeshes = meshChunk.PaxElements.Count; 

                    
                    DSLCChunk rebuiltDSLCChunk = m_rebuiltModel.GetChunk<DSLCChunk>();
                    List<byte> newDSLCList = new List<byte>();
                    for (int i = 0; i < totalMeshes; i++)
                    {
                        if (filteredIndicies.Contains(i))
                        {
                            newDSLCList.Add(rebuiltDSLCChunk.DisplayListCounts[i]);
                        }
                    }
                    rebuiltDSLCChunk.DisplayListCounts = newDSLCList;
                    // recalc sizes?
                    
                    
                    MESHChunk rebuiltMESHChunk = m_rebuiltModel.GetChunk<MESHChunk>();
                    List<PaxElement> newPaxElementList = new List<PaxElement>();
                    for (int i = 0; i < totalMeshes; i++)
                    {
                        if (filteredIndicies.Contains(i))
                        {
                            newPaxElementList.Add(rebuiltMESHChunk.PaxElements[i]);
                        }
                    }
                    rebuiltMESHChunk.PaxElements = newPaxElementList;

                    // Don't need ELEMChunk as that gets built from display list info
                    
                    
                    DSLIChunk rebuiltDSLIChunk =  m_rebuiltModel.GetChunk<DSLIChunk>();
                    List<DSLIInfo> newDSLIInfoList = new List<DSLIInfo>();
                    int runningTotal = 0;
                    int sizeMultiplier = 1;
                    for (int i = 0; i < totalMeshes; i++)
                    {
                        if (filteredIndicies.Contains(i))
                        {
                            int toAdd = (rebuiltDSLIChunk.DisplayListIndices[i].length / 2) + 16;
                            newDSLIInfoList.Add(new DSLIInfo(){startPos = runningTotal,length = rebuiltDSLIChunk.DisplayListIndices[i].length});
                            //runningTotal += rebuiltDSLIChunk.DisplayListIndices[i].length;                                 
                            runningTotal += toAdd;
                        }
                    }
                    rebuiltDSLIChunk.DisplayListIndices = newDSLIInfoList;
                    
                    
                    
                    DSLSChunk rebuiltDSLSChunk =  m_rebuiltModel.GetChunk<DSLSChunk>();
                    List<DisplayListHeader> newDisplayListHeaderList = new List<DisplayListHeader>();
                    for (int i = 0; i < totalMeshes; i++)
                    {
                        if (filteredIndicies.Contains(i))
                        {
                            newDisplayListHeaderList.Add(rebuiltDSLSChunk.DisplayListHeaders[i]);
                        }
                    }
                    rebuiltDSLSChunk.DisplayListHeaders = newDisplayListHeaderList;

                    
                    // STYPChunk rebuiltSTYPChunk =  m_rebuiltModel.GetChunk<STYPChunk>();
                    // if (rebuiltSTYPChunk != null)
                    // {
                    //     List<SelectSet> newSelectSetList = new List<SelectSet>();
                    //     for (int i = 0; i < totalMeshes; i++)
                    //     {
                    //         if (filteredIndicies.Contains(i))
                    //         {
                    //             newSelectSetList.Add(rebuiltSTYPChunk.SelectSetList[i]);
                    //         }
                    //     }
                    //
                    //     rebuiltSTYPChunk.SelectSetList = newSelectSetList;
                    // }

                    CNTRChunk rebuiltCNTRChunk =  m_rebuiltModel.GetChunk<CNTRChunk>();
                    List<Vector3> newPositions = new List<Vector3>();
                    int posIndex = 0;
                    
                    // first 4 are always there, then dependant on meshes

                    int alwaysIncludedOffset = 4;
                    for (int i = 0; i < alwaysIncludedOffset;i++)
                    {
                        newPositions.Add(rebuiltCNTRChunk.Positions[i]);
                    }
                    
                    for (int i = 0; i < totalMeshes; i++)
                    {
                        if (filteredIndicies.Contains(i))
                        {
                            newPositions.Add(rebuiltCNTRChunk.Positions[i+alwaysIncludedOffset]);
                        }
                    }
                    rebuiltCNTRChunk.Positions = newPositions;

                    if (m_rebuiltModel.IsSkinned())
                    {
                        SKINChunk rebuiltSKINChunk = m_rebuiltModel.GetChunk<SKINChunk>();
                        List<SkinData> newSkinDataList = new List<SkinData>();
                        for (int i = 0; i < totalMeshes; i++)
                        {
                            if (filteredIndicies.Contains(i))
                            {
                                newSkinDataList.Add(rebuiltSKINChunk.SkinDataList[i]);
                            }
                        }

                        rebuiltSKINChunk.SkinDataList = newSkinDataList;
                    }

                    byte[] buffer = null;

                    using (MemoryStream writeMemoryStream = new MemoryStream())
                    {
                        using (BinaryWriter binWriter = new BinaryWriter(writeMemoryStream))
                        {
                            m_rebuiltModel.ToStream(binWriter);
                        }
                        writeMemoryStream.Flush();
                        buffer = writeMemoryStream.ToArray();
                    }

                    File.WriteAllBytes(stub.OutputPath, buffer);
                    //File.WriteAllBytes(@"D:\GladiusModding\GC\unpack\data\characters\galdrf\galdrf.pax",buffer);
   
                    m_foldOutsCompareList.Clear();
                    for (int i = 0; i < totalMeshes; i++)
                    {
                        m_foldOutsCompareList.Add((false, false, false,false));
                    }

                }
            }

            if (m_originalModel != null && m_rebuiltModel != null)
            {
                ChunkComparisonTools.DrawChunkCompare(m_originalModel, m_rebuiltModel, Color.green, Color.yellow);

                ChunkComparisonTools.DrawDSLIListsCompare(m_originalModel.GetChunk<DSLIChunk>().DisplayListIndices,
                    m_rebuiltModel.GetChunk<DSLIChunk>().DisplayListIndices,m_originalModel.GetChunk<DSLSChunk>(),m_rebuiltModel.GetChunk<DSLSChunk>());

                if (m_rebuiltModel.IsSkinned())
                {
                    List<SkinData> oldList = new List<SkinData>();
                    oldList.AddRange(m_originalModel.GetChunk<SKINChunk>().SkinDataList);
                    List<SkinData> newList = new List<SkinData>();
                    newList.AddRange(m_rebuiltModel.GetChunk<SKINChunk>().SkinDataList);
                    ChunkComparisonTools.DrawSkinDataListsCompare(oldList, newList, true, m_foldOutsCompareList);
                }
            }
        }
    }
}