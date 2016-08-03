using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MaterialUpdater : MonoBehaviour {

    public List<MaterialUpdateInfo> MaterialsToUpdate = new List<MaterialUpdateInfo>();
    
    
    // Use this for initialization
	void Start () 
    {
        foreach (MaterialUpdateInfo mui in MaterialsToUpdate)
        {
            mui.Start();
        }
	
	}
	
	// Update is called once per frame
	void LateUpdate () 
    {
        //if (GetComponent<Renderer>().enabled)
        {

            foreach (MaterialUpdateInfo mui in MaterialsToUpdate)
            {
                if (mui != null)
                {
                    mui.Update();
                }
            }
        }
	}
    [System.Serializable]
    public class MaterialUpdateInfo : MonoBehaviour
    {
        public Material Material;
        public float StartX;
        public float EndX = 1.0f;
        public float StartY;
        public float EndY = 1.0f;
        public bool ReverseX;
        public bool ReverseY;
        
        public Vector2 UVOffset = new Vector2();
        public Vector2 Alternate = new Vector2(1, 1);
        public float UVAnimationRate = 0.01f;

        public void Start()
        {
            UVOffset = new Vector2(StartX, StartY);
        }

        public void Update()
        {
            float timeDelta = UVAnimationRate * Time.deltaTime;
            Vector2 delta = new Vector2(EndX > 0f ? timeDelta : 0.0f,EndY > 0f ? timeDelta:0.0f);
            delta.x  *= Alternate.x;
            delta.y *= Alternate.y;

            UVOffset += delta;

            if (UVOffset.x > EndX && ReverseX)
            {
                Alternate.x = -1;
            }
            if (UVOffset.y > EndY && ReverseY)
            {
                Alternate.y = -1;
            }
            if (UVOffset.x < StartX && ReverseX)
            {
                Alternate.x = 1;
            }
            if (UVOffset.y < StartY && ReverseY)
            {
                Alternate.y = 1;
            }

            Material.SetTextureOffset("_MainTex", UVOffset);

        }

    }


}
