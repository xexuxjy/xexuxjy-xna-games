using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Assets.Editor;
using TMPro;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(TestCreateSkinnedModelStub))]
public class TestCreateSkinnedModel : Editor
{
    public override void OnInspectorGUI()
    {
        TestCreateSkinnedModelStub stub = target as TestCreateSkinnedModelStub;

        base.OnInspectorGUI();

        if (GUILayout.Button("Create GC Model"))
        {
            GCModel gcModel = GCModel.CreateFromGameObject(stub.OriginalModel,(short)stub.AnimShift,stub.LodLevel,stub.BoneRoot);
            byte[] buffer = null;

            using (MemoryStream writeMemoryStream = new MemoryStream())
            {
                using (BinaryWriter binWriter = new BinaryWriter(writeMemoryStream))
                {
                    gcModel.WriteData(binWriter);
                }
                writeMemoryStream.Flush();
                buffer = writeMemoryStream.ToArray();
            }

            File.WriteAllBytes(stub.OutputPath,buffer);

            using (MemoryStream readMemoryStream = new MemoryStream(buffer))
            {
                using (BinaryReader binReader = new BinaryReader(readMemoryStream))
                {
                    StringBuilder debugInfo = new StringBuilder();
                    try
                    {
                        GCModel rebuiltModelGC = GCModel.ReadData(binReader, "", debugInfo);
                        CommonModelData rebuiltCommonModel = rebuiltModelGC.ToCommon();
                        if (rebuiltCommonModel != null)
                        {
                            GameObject rebuiltModel = CommonModelProcessor.CommonModelToGameObject("", stub.LodLevel,
                                rebuiltCommonModel, out Dictionary<BoneNode, GameObject> boneObjectMapRebuilt);
                            rebuiltModel.name = stub.OriginalModel.name + "-Rebuilt";
                            rebuiltModel.transform.position = new Vector3(0, 3, 0);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(debugInfo.ToString());
                        throw;
                    }
                }
            }
        }

    }
}