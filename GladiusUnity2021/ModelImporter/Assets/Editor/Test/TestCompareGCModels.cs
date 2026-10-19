using System.IO;
using System.Linq;
using System.Text;
using Gladius.util.Test;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TestCompareGCModelsStub))]
public class TestCompareGCModels : Editor
{

    public override void OnInspectorGUI()
    {
        TestCompareGCModelsStub stub = target as TestCompareGCModelsStub;

        base.OnInspectorGUI();

        if (GUILayout.Button("Compare Models"))
        {

            string chunkComparePath = "d:/tmp/chunk-compare/flynny";

            string chunkComparePathLHS = chunkComparePath + "/lhs";
            Directory.CreateDirectory(chunkComparePathLHS);

            string chunkComparePathRHS = chunkComparePath + "/rhs";
            Directory.CreateDirectory(chunkComparePathRHS);


            StringBuilder chunkAlignmentLHS = new StringBuilder();
            StringBuilder chunkAlignmentRHS = new StringBuilder();

            byte[] originalModelReadBuffer = null;
            byte[] originalModelWriteBuffer = null;

            GCModel lhsGCModel = null;
            GCModel rhsGCModel = null;
            
            // Load the skin data into a model.
            using (BinaryReader binReader = new BinaryReader(new MemoryStream(stub.LHS.bytes)))
            {
                binReader.BaseStream.Position = 0;
                StringBuilder debugInfo = new StringBuilder();
                lhsGCModel = GCModel.ReadData(binReader, "", debugInfo);

                foreach (BaseChunk chunk in lhsGCModel.m_chunkList)
                {
                    File.WriteAllBytes($"{chunkComparePathLHS}/{new string(chunk.Signature)}.bin", chunk.RawData);
                    chunkAlignmentLHS.Append($" {new string(chunk.Signature)} - {chunk.ReadStart} ");
                    foreach (int val in chunk.ReadAlignments.Keys.OrderByDescending(x => x))
                    {
                        chunkAlignmentLHS.Append($"{val}:{chunk.ReadAlignments[val]} ,");
                    }

                    chunkAlignmentLHS.AppendLine();
                }
            }


            using (BinaryReader binReader = new BinaryReader(new MemoryStream(stub.RHS.bytes)))
            {
                binReader.BaseStream.Position = 0;
                StringBuilder debugInfo = new StringBuilder();
                rhsGCModel = GCModel.ReadData(binReader, "", debugInfo);

                foreach (BaseChunk chunk in rhsGCModel.m_chunkList)
                {
                    File.WriteAllBytes($"{chunkComparePathRHS}/{new string(chunk.Signature)}.bin", chunk.RawData);
                    chunkAlignmentLHS.Append($" {new string(chunk.Signature)} - {chunk.ReadStart} ");
                    foreach (int val in chunk.ReadAlignments.Keys.OrderByDescending(x => x))
                    {
                        chunkAlignmentLHS.Append($"{val}:{chunk.ReadAlignments[val]} ,");
                    }

                    chunkAlignmentLHS.AppendLine();
                }
            }

            int ibreak = 0;

        }
    }
}