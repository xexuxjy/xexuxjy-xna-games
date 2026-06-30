using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TestCreateSkinData : MonoBehaviour
{
    public GameObject Target;

    public TextAsset OriginalModel;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CommonModelData commonModel = null;

        GCModel model = new GCModel("");
        // Load the skin data into a model.
        using (BinaryReader binReader = new BinaryReader(new MemoryStream(OriginalModel.bytes)))
        {
            model.LoadData(binReader, null);
            commonModel = model.ToCommon();
        }

        if (commonModel != null)
        {
            string assetName = "test";
            string outputHierarchy = "";
            uint lodLevel = 0;
            string prefabOutputDirectory = "";
            
            GameObject gameObject  =CommonModelProcessor.CommonModelToGameObject(outputHierarchy, lodLevel,
                    commonModel,out Dictionary<BoneNode,GameObject> boneObjectMap);
            
            
            // back to skin.
        }
        
        
    // take that model and go back to skin data.
        
        
        // if (Target != null)
        // {
        //     SkinnedMeshRenderer[] skinnedRenderers = Target.GetComponentsInChildren<SkinnedMeshRenderer>();
        //     List<SkinData> skinDataList = new List<SkinData>();
        //     foreach (SkinnedMeshRenderer renderer in skinnedRenderers)
        //     {
        //         SkinData skinData = GCModel.CreateSkinData(renderer.sharedMesh);
        //         skinDataList.Add(skinData);       
        //     }
        //
        //     int ibreak = 0;
        //
        // }
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
