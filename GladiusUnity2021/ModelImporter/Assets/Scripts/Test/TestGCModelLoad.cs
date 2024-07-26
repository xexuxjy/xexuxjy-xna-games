using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class TestGCModelLoad : MonoBehaviour
{
    public TextAsset GCModelData;

    public void Awake()
    {
        if (GCModelData != null)
        {
            GCModel testModel = new GCModel("Test");
            StringBuilder debugInfo = new StringBuilder();

            
            using (BinaryReader binReader = new BinaryReader(new MemoryStream(GCModelData.bytes)))
            {
                testModel.LoadData(binReader,debugInfo);
                if (debugInfo.Length > 0)
                {
                    Debug.LogWarning(debugInfo.ToString());
                }
            }

            CommonModelData commeModelData = testModel.ToCommon();
            
            
            int ibreak = 0;
        }
    }
}
