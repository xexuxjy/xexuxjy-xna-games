using System.Collections.Generic;
using UnityEngine;

public class TestCreateSkinData : MonoBehaviour
{
    public GameObject Target;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Target != null)
        {
            SkinnedMeshRenderer[] skinnedRenderers = Target.GetComponentsInChildren<SkinnedMeshRenderer>();
            List<SkinData> skinDataList = new List<SkinData>();
            foreach (SkinnedMeshRenderer renderer in skinnedRenderers)
            {
                SkinData skinData = GCModel.CreateSkinData(renderer.sharedMesh);
                skinDataList.Add(skinData);       
            }

            int ibreak = 0;

        }
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
