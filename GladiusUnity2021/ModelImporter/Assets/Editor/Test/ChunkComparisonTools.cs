using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.Test
{
    public static class ChunkComparisonTools
    {
        public static int ColumnWidth = 100;
        public static int IndentLevel = 20;

        public static GUIStyle GetSectionHeaderStyle()
        {
            GUIStyle gs = new GUIStyle(GUI.skin.label);
            gs.fontStyle = FontStyle.Bold;
            gs.normal.textColor = Color.white;
            gs.fontSize = 20;
            return gs;
        }

        public static GUIStyle GetTableHeaderStyle()
        {
            GUIStyle gs = new GUIStyle(GUI.skin.label);
            gs.fontStyle = FontStyle.Bold;
            gs.normal.textColor = Color.white;
            return gs;
        }

        public static GUIStyle GetTableStyle()
        {
            GUIStyle gs = new GUIStyle(GUI.skin.label);
            gs.normal.textColor = Color.white;
            return gs;
        }

        public static GUIStyle GetFoldoutSectionHeaderStyle()
        {
            GUIStyle gs = new GUIStyle(GUI.skin.GetStyle("FoldoutHeader"));
            gs.fontStyle = FontStyle.Bold;
            gs.fontSize = 20;
            gs.normal.textColor = Color.white;
            return gs;
        }

        // utility method
        public static void HorizontalLine(Color color)
        {
            var c = GUI.color;
            GUI.color = color;
            GUIStyle horizontalLine = new GUIStyle();
            horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
            horizontalLine.margin = new RectOffset(0, 0, 4, 4);
            horizontalLine.fixedHeight = 1;
            GUILayout.Box(GUIContent.none, horizontalLine);

            GUI.color = c;
        }


        public static void DrawChunkCompare(GCModel model1, GCModel model2, Color matchColor, Color diffColor)
        {
            HorizontalLine(Color.white);
            EditorGUILayout.LabelField($"Chunks", GetSectionHeaderStyle(), GUILayout.Width(ColumnWidth));


            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Chunk", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField($"Size", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField($"Elements", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
            GUILayout.EndHorizontal();
            Color oldColor = GUI.contentColor;

            foreach (BaseChunk model1Chunk in model1.m_chunkList)
            {
                string model1ChunkName = new string(model1Chunk.Signature);
                string model2ChunkName = null;

                BaseChunk model2Chunk = model2.m_chunkList.Find(x => x.GetType() == model1Chunk.GetType());

                if (model2Chunk == null)
                {
                    GUI.contentColor = Color.red;
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"{model1ChunkName}", GetTableHeaderStyle(),
                        GUILayout.Width(ColumnWidth));
                    EditorGUILayout.LabelField($"{model1Chunk.Length}", GetTableHeaderStyle(),
                        GUILayout.Width(ColumnWidth));
                    EditorGUILayout.LabelField($"{model1Chunk.NumElements}", GetTableHeaderStyle(),
                        GUILayout.Width(ColumnWidth));
                    GUILayout.EndHorizontal();
                }
                else
                {
                    model2ChunkName = new string(model2Chunk.Signature);
                    ;

                    GUI.contentColor = matchColor;
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"{model1ChunkName} / {model2ChunkName}", GetTableHeaderStyle(),
                        GUILayout.Width(ColumnWidth));
                    GUI.contentColor = CompareVals((int)model1Chunk.Length, (int)model2Chunk.Length, matchColor,
                        diffColor);
                    EditorGUILayout.LabelField($"{model1Chunk.Length} / {model2Chunk.Length}", GetTableHeaderStyle(),
                        GUILayout.Width(ColumnWidth));
                    GUI.contentColor = CompareVals((int)model1Chunk.CalculatedNumElements(),
                        (int)model2Chunk.CalculatedNumElements(),
                        matchColor,
                        diffColor);
                    EditorGUILayout.LabelField(
                        $"{model1Chunk.CalculatedNumElements()} / {model2Chunk.CalculatedNumElements()}",
                        GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                    GUILayout.EndHorizontal();
                }
            }

            GUI.contentColor = oldColor;
        }


        public static void DrawCSK1(CSK1 csk1, Color color)
        {
            Color oldColor = GUI.contentColor;
            GUI.contentColor = color;
            EditorGUILayout.LabelField(csk1.idxBone.ToString(), GetTableStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField(csk1.count.ToString(), GetTableStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField(csk1.vertSrc.ToString(), GetTableStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField(csk1.vertDst.ToString(), GetTableStyle(), GUILayout.Width(ColumnWidth));
            GUI.contentColor = oldColor;
        }

        public static Color CompareVals(int val1, int val2, Color matchColor, Color diffColor)
        {
            return val1 == val2 ? matchColor : diffColor;
        }

        public static Color CompareVals(uint val1, uint val2, Color matchColor, Color diffColor)
        {
            return val1 == val2 ? matchColor : diffColor;
        }

        public static void DrawCSK1Compare(List<CSK1> oldList, [ItemNotNull] List<CSK1> newList, int index,
            Color matchColor, Color diffColor)
        {
            Color oldColor = GUI.contentColor;
            CSK1 oldCSK = index < oldList.Count ? oldList[index] : null;
            CSK1 newCSK = index < newList.Count ? newList[index] : null;

            int oldVal = oldCSK != null ? oldCSK.idxBone : -1;
            int newVal = newCSK != null ? newCSK.idxBone : -1;

            GUI.contentColor = CompareVals(oldVal, newVal, matchColor, diffColor);
            EditorGUILayout.LabelField($"{oldVal} / {newVal}", GetTableStyle(), GUILayout.Width(ColumnWidth));

            oldVal = oldCSK != null ? oldCSK.count : -1;
            newVal = newCSK != null ? newCSK.count : -1;

            GUI.contentColor = CompareVals(oldVal, newVal, matchColor, diffColor);
            EditorGUILayout.LabelField($"{oldVal} / {newVal}", GetTableStyle(), GUILayout.Width(ColumnWidth));

            uint oldValU = oldCSK != null ? oldCSK.vertSrc : uint.MaxValue;
            uint newValU = newCSK != null ? newCSK.vertSrc : uint.MaxValue;

            GUI.contentColor = CompareVals(oldVal, newVal, matchColor, diffColor);
            EditorGUILayout.LabelField($"{oldVal} / {newVal}", GetTableStyle(), GUILayout.Width(ColumnWidth));

            oldValU = oldCSK != null ? oldCSK.vertDst : uint.MaxValue;
            newValU = newCSK != null ? newCSK.vertDst : uint.MaxValue;

            GUI.contentColor = CompareVals(oldValU, newValU, matchColor, diffColor);
            EditorGUILayout.LabelField($"{oldValU} / {newValU}", GetTableStyle(), GUILayout.Width(ColumnWidth));

            GUI.contentColor = oldColor;
        }


        public static void DrawCSK2(CSK2 csk2, Color color)
        {
            Color oldColor = GUI.contentColor;
            GUI.contentColor = color;
            EditorGUILayout.LabelField(csk2.idxBone[0].ToString(), GetTableStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField(csk2.idxBone[1].ToString(), GetTableStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField(csk2.count.ToString(), GetTableStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField(csk2.vertSrc.ToString(), GetTableStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField(csk2.vertDst.ToString(), GetTableStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField(csk2.weightsSrc.ToString(), GetTableStyle(), GUILayout.Width(ColumnWidth));
            GUI.contentColor = oldColor;
        }

        public static void DrawCSK2Compare(List<CSK2> oldList, List<CSK2> newList, int index, Color matchColor,
            Color diffColor)
        {
            CSK2 oldCSK = index < oldList.Count ? oldList[index] : null;
            CSK2 newCSK = index < newList.Count ? newList[index] : null;


            int oldVal = oldCSK != null ? oldCSK.idxBone[0] : -1;
            int newVal = newCSK != null ? newCSK.idxBone[0] : -1;


            Color oldColor = GUI.contentColor;
            GUI.contentColor = CompareVals(oldVal, newVal, matchColor, diffColor);
            EditorGUILayout.LabelField($"{oldVal} / {newVal}", GetTableStyle(), GUILayout.Width(ColumnWidth));

            oldVal = oldCSK != null ? oldCSK.idxBone[1] : -1;
            newVal = newCSK != null ? newCSK.idxBone[1] : -1;

            GUI.contentColor = CompareVals(oldVal, newVal, matchColor, diffColor);
            EditorGUILayout.LabelField($"{oldVal} / {newVal}", GetTableStyle(), GUILayout.Width(ColumnWidth));

            oldVal = oldCSK != null ? oldCSK.count : -1;
            newVal = newCSK != null ? newCSK.count : -1;

            GUI.contentColor = CompareVals(oldVal, newVal, matchColor, diffColor);
            EditorGUILayout.LabelField($"{oldVal} / {newVal}", GetTableStyle(), GUILayout.Width(ColumnWidth));

            uint oldValU = oldCSK != null ? oldCSK.vertSrc : uint.MaxValue;
            uint newValU = newCSK != null ? newCSK.vertSrc : uint.MaxValue;

            GUI.contentColor = CompareVals(oldValU, newValU, matchColor, diffColor);
            EditorGUILayout.LabelField($"{oldValU} / {newValU}", GetTableStyle(), GUILayout.Width(ColumnWidth));

            oldValU = oldCSK != null ? oldCSK.vertDst : uint.MaxValue;
            newValU = newCSK != null ? newCSK.vertDst : uint.MaxValue;

            GUI.contentColor = CompareVals(oldValU, newValU, matchColor, diffColor);
            EditorGUILayout.LabelField($"{oldValU} / {newValU}", GetTableStyle(), GUILayout.Width(ColumnWidth));

            oldValU = oldCSK != null ? oldCSK.weightsSrc : uint.MaxValue;
            newValU = newCSK != null ? newCSK.weightsSrc : uint.MaxValue;

            GUI.contentColor = CompareVals(oldValU, newValU, matchColor, diffColor);
            EditorGUILayout.LabelField($"{oldValU} / {newValU}", GetTableStyle(), GUILayout.Width(ColumnWidth));

            GUI.contentColor = oldColor;
        }


        public static void DrawCSKA(CSKA cska, Color color)
        {
            Color oldColor = GUI.contentColor;
            GUI.contentColor = color;
            EditorGUILayout.LabelField($"{cska.idxBone}", GetTableStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField($"{cska.count}", GetTableStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField($"{cska.vertSrc}", GetTableStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField($"{cska.weightsSrc}", GetTableStyle(), GUILayout.Width(ColumnWidth));
            GUI.contentColor = oldColor;
        }

        public static void DrawCSKACompare(List<CSKA> oldList, List<CSKA> newList, int index, Color matchColor,
            Color diffColor)
        {
            CSKA oldCSK = index < oldList.Count ? oldList[index] : null;
            CSKA newCSK = index < newList.Count ? newList[index] : null;

            Color oldColor = GUI.contentColor;

            int oldVal = oldCSK != null ? oldCSK.idxBone : -1;
            int newVal = newCSK != null ? newCSK.idxBone : -1;


            GUI.contentColor = CompareVals(oldVal, newVal, matchColor, diffColor);
            EditorGUILayout.LabelField($"{oldVal} / {newVal}", GetTableStyle(), GUILayout.Width(ColumnWidth));

            oldVal = oldCSK != null ? oldCSK.count : -1;
            newVal = newCSK != null ? newCSK.count : -1;

            GUI.contentColor = CompareVals(oldVal, newVal, matchColor, diffColor);
            EditorGUILayout.LabelField($"{oldVal} / {newVal}", GetTableStyle(), GUILayout.Width(ColumnWidth));

            uint oldValU = oldCSK != null ? oldCSK.vertSrc : uint.MaxValue;
            uint newValU = newCSK != null ? newCSK.vertSrc : uint.MaxValue;

            GUI.contentColor = CompareVals(oldValU, newValU, matchColor, diffColor);
            EditorGUILayout.LabelField($"{oldValU} / {newValU}", GetTableStyle(), GUILayout.Width(ColumnWidth));

            oldValU = oldCSK != null ? oldCSK.weightsSrc : uint.MaxValue;
            newValU = newCSK != null ? newCSK.weightsSrc : uint.MaxValue;

            GUI.contentColor = CompareVals(oldValU, newValU, matchColor, diffColor);
            EditorGUILayout.LabelField($"{oldValU} / {newValU}", GetTableStyle(), GUILayout.Width(ColumnWidth));

            GUI.contentColor = oldColor;
        }


        public static void DrawSingleSkinData(int meshIndex, SkinData origSD,
            List<(bool, bool, bool, bool)> foldOutsList)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Mesh {meshIndex}", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Size", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField("AnimShift", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField("NumList1", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField("NumList2", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField("NumListA", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField("NumPackets1", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField("Packets1Start", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField("Packets1Sizes", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField("NumPackets2", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField("Packets2Start", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField("Packets2Sizes", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(origSD.Size.ToString(), GetTableStyle(),
                GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField(origSD.AnimShift.ToString(), GetTableStyle(),
                GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField(origSD.NumList1.ToString(), GetTableStyle(),
                GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField(origSD.NumList2.ToString(), GetTableStyle(),
                GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField(origSD.NumListA.ToString(), GetTableStyle(),
                GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField(origSD.NumPackets1.ToString(), GetTableStyle(),
                GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField(origSD.PacketStart1.ToString(), GetTableStyle(),
                GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField(origSD.PacketSize1.ToString(), GetTableStyle(),
                GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField(origSD.NumPackets2.ToString(), GetTableStyle(),
                GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField(origSD.PacketStart2.ToString(), GetTableStyle(),
                GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField(origSD.PacketSize2.ToString(), GetTableStyle(),
                GUILayout.Width(ColumnWidth));

            GUILayout.EndHorizontal();


            bool csk1FoldOut = foldOutsList[meshIndex].Item1;
            bool csk2FoldOut = foldOutsList[meshIndex].Item2;
            bool cskAFoldOut = foldOutsList[meshIndex].Item3;
            bool packetsFoldout = foldOutsList[meshIndex].Item4;

            packetsFoldout = StartIndentedFoldoutHeader("Packets", packetsFoldout);
            if (packetsFoldout)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Packet1 Start", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("Packet1 Size", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                GUILayout.EndHorizontal();

                for (int j = 0; j < origSD.NumPackets1; j++)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"{origSD.Packet1Starts[j]}", GetTableStyle(),
                        GUILayout.Width(ColumnWidth));
                    EditorGUILayout.LabelField($"{origSD.Packet1Sizes[j]}", GetTableStyle(),
                        GUILayout.Width(ColumnWidth));
                    GUILayout.EndHorizontal();
                }

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Packet2 Start", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("Packet2 Size", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                GUILayout.EndHorizontal();

                for (int j = 0; j < origSD.NumPackets2; j++)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"{origSD.Packet2Starts[j]}", GetTableStyle(),
                        GUILayout.Width(ColumnWidth));
                    EditorGUILayout.LabelField($"{origSD.Packet2Sizes[j]}", GetTableStyle(),
                        GUILayout.Width(ColumnWidth));
                    GUILayout.EndHorizontal();
                }
            }

            EndIndentedFoldoutHeader();

            csk1FoldOut = StartIndentedFoldoutHeader("CSK1", csk1FoldOut);
            if (csk1FoldOut)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("IdxBone", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("Count", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("Src", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("Dst", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                GUILayout.EndHorizontal();

                foreach (CSK1 csk1 in origSD.CSK1List)
                {
                    GUILayout.BeginHorizontal();
                    DrawCSK1(csk1, Color.green);
                    GUILayout.EndHorizontal();
                }
            }

            EndIndentedFoldoutHeader();

            csk2FoldOut = StartIndentedFoldoutHeader("CSK2", csk2FoldOut);
            if (csk2FoldOut)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("IdxBone1", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("IdxBone2", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("Count", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("Src", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("Dst", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                GUILayout.EndHorizontal();

                foreach (CSK2 csk2 in origSD.CSK2List)
                {
                    GUILayout.BeginHorizontal();
                    DrawCSK2(csk2, Color.green);
                    GUILayout.EndHorizontal();
                }
            }

            EndIndentedFoldoutHeader();

            cskAFoldOut = StartIndentedFoldoutHeader("CSKA", cskAFoldOut);
            if (cskAFoldOut)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("IdxBone", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("Count", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("VertSrc", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("WeightSrc", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                GUILayout.EndHorizontal();
                foreach (CSKA cska in origSD.CSKAList)
                {
                    GUILayout.BeginHorizontal();
                    DrawCSKA(cska, Color.green);
                    GUILayout.EndHorizontal();
                }
            }

            EndIndentedFoldoutHeader();

            foldOutsList[meshIndex] = (csk1FoldOut, csk2FoldOut, cskAFoldOut, packetsFoldout);
        }

        public static void DrawDSLIListsCompare(List<DSLIInfo> originalList, List<DSLIInfo> newList,DSLSChunk oldDSLS,DSLSChunk newDSLS)
        {
            if (originalList.Count > 0 && newList.Count > 0)
            {
                
                HorizontalLine(Color.white);
                EditorGUILayout.LabelField($"DSLI Lists", GetSectionHeaderStyle(), GUILayout.Width(ColumnWidth));


                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Position", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("Length", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("Aligns", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("Aligns+16", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("DSLSData", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("Diff", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("Start32A", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("Length32A", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));

                GUILayout.EndHorizontal();


                int numRows = Math.Max(originalList.Count, newList.Count);

                for (int index = 0; index < numRows; index++)
                {
                    Color oldColor = GUI.contentColor;
                    DSLIInfo oldData = index < originalList.Count ? originalList[index] : null;
                    DSLIInfo newData = index < newList.Count ? newList[index] : null;

                    int oldVal = oldData != null ? oldData.startPos : -1;
                    int newVal = newData != null ? newData.startPos : -1;

                    GUILayout.BeginHorizontal();

                    GUI.contentColor = CompareVals(oldVal, newVal, Color.green, Color.yellow);
                    EditorGUILayout.LabelField($"{oldVal} / {newVal}", GetTableStyle(),
                        GUILayout.Width(ColumnWidth));

                    oldVal = oldData != null ? oldData.length : -1;
                    newVal = newData != null ? newData.length : -1;

                    GUI.contentColor = CompareVals(oldVal, newVal, Color.green, Color.yellow);
                    EditorGUILayout.LabelField($"{oldVal} / {newVal}", GetTableStyle(),
                        GUILayout.Width(ColumnWidth));

                    string matches = "Yes";
                    if (index > 0)
                    {
                        int val1 = originalList[index - 1].startPos + originalList[index - 1].length / 2;
                        int val2 = originalList[index].startPos;
                        matches = val1 == val2 ? "Yes" : "No";
                    }

                    GUI.contentColor = matches == "Yes" ? Color.green : Color.red;
                    EditorGUILayout.LabelField($"{matches}", GetTableStyle(), GUILayout.Width(ColumnWidth));

                    matches = "";
                    if (index > 0)
                    {
                        int val1 = (originalList[index - 1].startPos + originalList[index - 1].length / 2) + 16;
                        int val2 = originalList[index].startPos;
                        matches = val1 == val2 ? "Yes" : "No";
                    }

                    int dslsSize = oldDSLS.DisplayListHeaders[index].GetSize();
                    int diff = originalList[index].length - dslsSize;
                    matches = diff == 0 ? "Yes" : "No";
                    
                    GUI.contentColor = matches == "Yes" ? Color.green : Color.red;
                    EditorGUILayout.LabelField($"{matches}", GetTableStyle(), GUILayout.Width(ColumnWidth));

                    EditorGUILayout.LabelField($"{dslsSize}", GetTableStyle(), GUILayout.Width(ColumnWidth));

                    
                    EditorGUILayout.LabelField($"{diff}", GetTableStyle(), GUILayout.Width(ColumnWidth));
                    
                    
                    bool start32A = (originalList[index].startPos % 32) == 0;
                    bool length32A = (originalList[index].length % 32) == 0;
                    
                    GUI.contentColor = start32A ? Color.green : Color.red;
                    EditorGUILayout.LabelField($"{start32A}", GetTableStyle(), GUILayout.Width(ColumnWidth));
                    GUI.contentColor = length32A ? Color.green : Color.red;
                    EditorGUILayout.LabelField($"{length32A}", GetTableStyle(), GUILayout.Width(ColumnWidth));

                    GUILayout.EndHorizontal();
                    
                    GUI.contentColor = oldColor;
                }

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Position", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("Length", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("Aligns", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("Aligns+16", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("DSLSData", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("Diff", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("Start32A", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                EditorGUILayout.LabelField("Length32A", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                GUILayout.EndHorizontal();
                for (int index = 0; index < newList.Count; index++)
                {
                    Color oldColor = GUI.contentColor;
                    GUILayout.BeginHorizontal();
                    GUI.contentColor = Color.green;

                    EditorGUILayout.LabelField($"{newList[index].startPos}", GetTableStyle(),
                        GUILayout.Width(ColumnWidth));
                    EditorGUILayout.LabelField($"{newList[index].length}", GetTableStyle(),
                        GUILayout.Width(ColumnWidth));

                    string matches = "";
                    if (index > 0)
                    {
                        int val1 = newList[index - 1].startPos + newList[index - 1].length / 2;
                        int val2 = newList[index].startPos;
                        matches = val1 == val2 ? "Yes" : "No";
                    }

                    GUI.contentColor = matches == "Yes" ? Color.green : Color.red;
                    EditorGUILayout.LabelField($"{matches}", GetTableStyle(), GUILayout.Width(ColumnWidth));

                    matches = "Yes";
                    if (index > 0)
                    {
                        int val1 = (newList[index - 1].startPos + newList[index - 1].length / 2) + 16;
                        int val2 = newList[index].startPos;
                        matches = val1 == val2 ? "Yes" : "No";
                    }

                    int dslsSize = newDSLS.DisplayListHeaders[index].GetSize();
                    int diff = newList[index].length - dslsSize;
                    matches = diff == 0 ? "Yes" : "No";

                    
                    GUI.contentColor = matches == "Yes" ? Color.green : Color.red;
                    EditorGUILayout.LabelField($"{matches}", GetTableStyle(), GUILayout.Width(ColumnWidth));
                    
                    EditorGUILayout.LabelField($"{dslsSize}", GetTableStyle(), GUILayout.Width(ColumnWidth));
                    EditorGUILayout.LabelField($"{diff}", GetTableStyle(), GUILayout.Width(ColumnWidth));

                    bool start32A = (newList[index].startPos % 32) == 0;
                    bool length32A = (newList[index].length % 32) == 0;
                    
                    GUI.contentColor = start32A ? Color.green : Color.red;
                    EditorGUILayout.LabelField($"{start32A}", GetTableStyle(), GUILayout.Width(ColumnWidth));
                    GUI.contentColor = length32A ? Color.green : Color.red;
                    EditorGUILayout.LabelField($"{length32A}", GetTableStyle(), GUILayout.Width(ColumnWidth));
                    
                    GUILayout.EndHorizontal();

                    GUI.contentColor = oldColor;
                }
            }
        }


        public static void DrawSkinDataListsCompare(List<SkinData> originalList, List<SkinData> newList, bool diffData,
            List<(bool, bool, bool, bool)> foldOutsList)
        {
            if (originalList.Count > 0 && newList.Count > 0)
            {
                HorizontalLine(Color.white);
                EditorGUILayout.LabelField($"Skin Data", GetSectionHeaderStyle(), GUILayout.Width(ColumnWidth));
                int numRows = Math.Min(originalList.Count, newList.Count);

                for (int i = 0; i < numRows; i++)
                {
                    SkinData origSD = originalList[i];
                    SkinData newSD = newList[i];

                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"Mesh {i}", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Size", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                    EditorGUILayout.LabelField("AnimShift", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                    EditorGUILayout.LabelField("NumList1", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                    EditorGUILayout.LabelField("NumList2", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                    EditorGUILayout.LabelField("NumListA", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                    EditorGUILayout.LabelField("NumPackets1", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                    EditorGUILayout.LabelField("PacketsStart1", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                    EditorGUILayout.LabelField("PacketsSizes1", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                    EditorGUILayout.LabelField("NumPackets2", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                    EditorGUILayout.LabelField("PacketsStart2", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                    EditorGUILayout.LabelField("Packets2Sizes2", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();

                    Color oldColor = GUI.contentColor;
                    GUI.contentColor = CompareVals(origSD.Size, newSD.Size, Color.green, Color.yellow);
                    EditorGUILayout.LabelField($"{origSD.Size} / {newSD.Size}", GetTableStyle(),
                        GUILayout.Width(ColumnWidth));
                    GUI.contentColor = CompareVals(origSD.AnimShift, newSD.AnimShift, Color.green, Color.yellow);
                    EditorGUILayout.LabelField($"{origSD.AnimShift} / {newSD.AnimShift}", GetTableStyle(),
                        GUILayout.Width(ColumnWidth));

                    GUI.contentColor = CompareVals(origSD.NumList1, newSD.NumList1, Color.green, Color.yellow);
                    EditorGUILayout.LabelField($"{origSD.NumList1} / {newSD.NumList1}", GetTableStyle(),
                        GUILayout.Width(ColumnWidth));

                    GUI.contentColor = CompareVals(origSD.NumList2, newSD.NumList2, Color.green, Color.yellow);
                    EditorGUILayout.LabelField($"{origSD.NumList2} / {newSD.NumList2}", GetTableStyle(),
                        GUILayout.Width(ColumnWidth));

                    GUI.contentColor = CompareVals(origSD.NumListA, newSD.NumListA, Color.green, Color.yellow);
                    EditorGUILayout.LabelField($"{origSD.NumListA} / {newSD.NumListA}", GetTableStyle(),
                        GUILayout.Width(ColumnWidth));

                    GUI.contentColor = CompareVals(origSD.NumPackets1, newSD.NumPackets1, Color.green, Color.yellow);
                    EditorGUILayout.LabelField($"{origSD.NumPackets1} / {newSD.NumPackets1}", GetTableStyle(),
                        GUILayout.Width(ColumnWidth));

                    GUI.contentColor = CompareVals((int)origSD.PacketStart1, (int)newSD.PacketStart1, Color.green,
                        Color.yellow);
                    EditorGUILayout.LabelField($"{origSD.PacketStart1} / {newSD.PacketStart1}", GetTableStyle(),
                        GUILayout.Width(ColumnWidth));

                    GUI.contentColor = CompareVals((int)origSD.PacketSize1, (int)newSD.PacketSize1, Color.green,
                        Color.yellow);
                    EditorGUILayout.LabelField($"{origSD.PacketSize1} / {newSD.PacketSize1}", GetTableStyle(),
                        GUILayout.Width(ColumnWidth));

                    GUI.contentColor = CompareVals(origSD.NumPackets2, newSD.NumPackets2, Color.green, Color.yellow);
                    EditorGUILayout.LabelField($"{origSD.NumPackets2} / {newSD.NumPackets2}", GetTableStyle(),
                        GUILayout.Width(ColumnWidth));

                    GUI.contentColor = CompareVals((int)origSD.PacketStart2, (int)newSD.PacketStart2, Color.green,
                        Color.yellow);
                    EditorGUILayout.LabelField($"{origSD.PacketStart2} / {newSD.PacketStart2}", GetTableStyle(),
                        GUILayout.Width(ColumnWidth));

                    GUI.contentColor = CompareVals((int)origSD.PacketSize2, (int)newSD.PacketSize2, Color.green,
                        Color.yellow);
                    EditorGUILayout.LabelField($"{origSD.PacketSize2} / {newSD.PacketSize2}", GetTableStyle(),
                        GUILayout.Width(ColumnWidth));

                    GUILayout.EndHorizontal();

                    GUI.contentColor = oldColor;

                    bool csk1FoldOut = foldOutsList[i].Item1;
                    bool csk2FoldOut = foldOutsList[i].Item2;
                    bool cskAFoldOut = foldOutsList[i].Item3;
                    bool packetsFoldout = foldOutsList[i].Item4;

                    packetsFoldout = StartIndentedFoldoutHeader("Packets", packetsFoldout);
                    if (packetsFoldout)
                    {
                        DrawPacketsCompare(origSD, newSD, Color.green, Color.yellow);
                    }

                    EndIndentedFoldoutHeader();


                    csk1FoldOut = StartIndentedFoldoutHeader("CSK1", csk1FoldOut);
                    if (csk1FoldOut)
                    {
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("IdxBone", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                        EditorGUILayout.LabelField("Count", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                        EditorGUILayout.LabelField("Src", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                        EditorGUILayout.LabelField("Dst", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                        GUILayout.EndHorizontal();

                        if (diffData)
                        {
                            int numSteps = Math.Max(origSD.CSK1List.Count, newSD.CSK1List.Count);
                            for (int j = 0; j < numSteps; j++)
                            {
                                GUILayout.BeginHorizontal();
                                DrawCSK1Compare(origSD.CSK1List, newSD.CSK1List, j, Color.green, Color.yellow);
                                GUILayout.EndHorizontal();
                            }
                        }
                        else
                        {
                            foreach (CSK1 csk1 in origSD.CSK1List)
                            {
                                GUILayout.BeginHorizontal();
                                DrawCSK1(csk1, Color.green);
                                GUILayout.EndHorizontal();
                            }

                            foreach (CSK1 csk1 in newSD.CSK1List)
                            {
                                GUILayout.BeginHorizontal();
                                DrawCSK1(csk1, Color.yellow);
                                GUILayout.EndHorizontal();
                            }
                        }
                    }

                    EndIndentedFoldoutHeader();

                    csk2FoldOut = StartIndentedFoldoutHeader("CSK2", csk2FoldOut);
                    if (csk2FoldOut)
                    {
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("IdxBone1", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                        EditorGUILayout.LabelField("IdxBone2", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                        EditorGUILayout.LabelField("Count", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                        EditorGUILayout.LabelField("Src", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                        EditorGUILayout.LabelField("Dst", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                        EditorGUILayout.LabelField("WeightSrc", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                        GUILayout.EndHorizontal();

                        if (diffData)
                        {
                            int numSteps = Math.Max(origSD.CSK2List.Count, newSD.CSK2List.Count);
                            for (int j = 0; j < numSteps; j++)
                            {
                                GUILayout.BeginHorizontal();
                                DrawCSK2Compare(origSD.CSK2List, newSD.CSK2List, j, Color.green, Color.yellow);
                                GUILayout.EndHorizontal();
                            }
                        }
                        else
                        {
                            foreach (CSK2 csk2 in origSD.CSK2List)
                            {
                                GUILayout.BeginHorizontal();
                                DrawCSK2(csk2, Color.green);
                                GUILayout.EndHorizontal();
                            }

                            foreach (CSK2 csk2 in newSD.CSK2List)
                            {
                                GUILayout.BeginHorizontal();
                                DrawCSK2(csk2, Color.yellow);
                                GUILayout.EndHorizontal();
                            }
                        }
                    }

                    EndIndentedFoldoutHeader();

                    cskAFoldOut = StartIndentedFoldoutHeader("CSKA", cskAFoldOut);
                    if (cskAFoldOut)
                    {
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("IdxBone", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                        EditorGUILayout.LabelField("Count", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                        EditorGUILayout.LabelField("VertSrc", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                        EditorGUILayout.LabelField("WeightSrc", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
                        GUILayout.EndHorizontal();
                        if (diffData)
                        {
                            int numSteps = Math.Max(origSD.CSKAList.Count, newSD.CSKAList.Count);
                            for (int j = 0; j < numSteps; j++)
                            {
                                GUILayout.BeginHorizontal();
                                DrawCSKACompare(origSD.CSKAList, newSD.CSKAList, j, Color.green, Color.yellow);
                                GUILayout.EndHorizontal();
                            }
                        }
                        else
                        {
                            foreach (CSKA cska in origSD.CSKAList)
                            {
                                GUILayout.BeginHorizontal();
                                DrawCSKA(cska, Color.green);
                                GUILayout.EndHorizontal();
                            }

                            foreach (CSKA cska in newSD.CSKAList)
                            {
                                GUILayout.BeginHorizontal();
                                DrawCSKA(cska, Color.yellow);
                                GUILayout.EndHorizontal();
                            }
                        }
                    }

                    EndIndentedFoldoutHeader();

                    foldOutsList[i] = (csk1FoldOut, csk2FoldOut, cskAFoldOut, packetsFoldout);
                }
            }
        }


        public static void DrawPacketsCompare(SkinData oldSkinData, SkinData newSkinData, Color matchColor,
            Color diffColor)
        {
            Color oldColor = GUI.contentColor;
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Packet1 Start", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField("Packet1 Size", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
            GUILayout.EndHorizontal();

            int max = Math.Max(oldSkinData.NumPackets1, newSkinData.NumPackets1);

            for (int j = 0; j < max; j++)
            {
                ushort oldVal = ushort.MaxValue;
                ushort newVal = ushort.MaxValue;

                GUILayout.BeginHorizontal();
                GUI.contentColor = CompareListPosition(oldSkinData.Packet1Starts, newSkinData.Packet1Starts, j,
                    ref oldVal, ref newVal, matchColor, diffColor);
                EditorGUILayout.LabelField($"{oldVal} / {newVal}", GetTableStyle(), GUILayout.Width(ColumnWidth));

                GUI.contentColor = CompareListPosition(oldSkinData.Packet1Sizes, newSkinData.Packet1Sizes, j,
                    ref oldVal, ref newVal, matchColor, diffColor);
                EditorGUILayout.LabelField($"{oldVal} / {newVal}", GetTableStyle(), GUILayout.Width(ColumnWidth));
                GUILayout.EndHorizontal();
            }

            GUI.contentColor = oldColor;
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Packet2 Start", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
            EditorGUILayout.LabelField("Packet2 Size", GetTableHeaderStyle(), GUILayout.Width(ColumnWidth));
            GUILayout.EndHorizontal();


            max = Math.Max(oldSkinData.NumPackets2, newSkinData.NumPackets2);
            for (int j = 0; j < max; j++)
            {
                ushort oldVal = ushort.MaxValue;
                ushort newVal = ushort.MaxValue;

                GUILayout.BeginHorizontal();
                GUI.contentColor = CompareListPosition(oldSkinData.Packet2Starts, newSkinData.Packet2Starts, j,
                    ref oldVal, ref newVal, matchColor, diffColor);
                EditorGUILayout.LabelField($"{oldVal} / {newVal}", GetTableStyle(), GUILayout.Width(ColumnWidth));

                GUI.contentColor = CompareListPosition(oldSkinData.Packet2Sizes, newSkinData.Packet2Sizes, j,
                    ref oldVal, ref newVal, matchColor, diffColor);
                EditorGUILayout.LabelField($"{oldVal} / {newVal}", GetTableStyle(), GUILayout.Width(ColumnWidth));


                GUILayout.EndHorizontal();
            }

            GUI.contentColor = oldColor;
        }

        public static Color CompareListPosition(List<ushort> oldList, List<ushort> newList, int index,
            ref ushort oldVal, ref ushort newVal, Color matchColor, Color diffColor)
        {
            oldVal = index < oldList.Count ? oldList[index] : ushort.MaxValue;
            newVal = index < newList.Count ? newList[index] : ushort.MaxValue;

            if (oldVal == ushort.MaxValue || newVal == ushort.MaxValue)
            {
                return Color.red;
            }
            else
            {
                return CompareVals(oldVal, newVal, matchColor, diffColor);
            }
        }

        // public static Color CompareListPosition<T>(List<T> oldList, List<T> newList,int index, ref T oldVal, ref T newVal, Color matchColor, Color diffColor)
        // {
        //     oldVal = index < oldList.Count ? oldList[index] : ushort.MaxValue;
        //     newVal = index < newList.Count ? newList[index] : ushort.MaxValue;
        //
        //     if (oldVal == ushort.MaxValue || newVal == ushort.MaxValue)
        //     {
        //         return Color.red;
        //     }
        //     else
        //     {
        //         return CompareVals(oldVal, newVal, matchColor, diffColor);
        //     }
        // }

        public static bool StartIndentedFoldoutHeader(string name, bool foldout)
        {
            bool returnValue = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, name,
                GetFoldoutSectionHeaderStyle(), null, GUI.skin.GetStyle("IconButton"));
            GUILayout.BeginHorizontal();
            GUILayout.Space(IndentLevel);
            GUILayout.BeginVertical();

            return returnValue;
        }

        public static void EndIndentedFoldoutHeader()
        {
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }
}