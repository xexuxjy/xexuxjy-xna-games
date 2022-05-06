using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugGrid : MonoBehaviour
{
    public int Width = 32;
    public int Height = 32;
    public float SquareSize = 2.0f;


    // Use this for initialization
    void Start()
    {
        Material black = new Material(Shader.Find("Standard"));
        black.color = Color.black;

        Material white = new Material(Shader.Find("Standard"));
        white.color = Color.white;

        Material currentMaterial = black;
        Vector3 topLeft = new Vector3(-Width / 2f, 0, -Height / 2f);
        for (int i = 0; i < Width; ++i)
        {
            for (int j = 0; j < Height; ++j)
            {

                Vector3 offset = new Vector3(i * SquareSize, 0, j * SquareSize);
                GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                
                quad.transform.SetParent(transform);
                quad.GetComponent<MeshRenderer>().material = currentMaterial;

                quad.transform.localScale = new Vector3(SquareSize, SquareSize, SquareSize);
                quad.transform.localPosition = topLeft + offset;
                quad.transform.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);

                GameObject text = new GameObject();
                text.name = "Text";
                text.transform.SetParent(quad.transform);
                text.transform.localPosition = Vector3.zero;
                text.transform.localRotation = Quaternion.identity;

                TextMesh tm = text.AddComponent<TextMesh>();
                tm.text = "" + j + "," + i;
                tm.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                tm.color = (currentMaterial == black) ? Color.white : Color.black;
                tm.fontSize = 20;
                tm.alignment = TextAlignment.Center;

                //TextMeshPro tmp = text.AddComponent<TextMeshPro>();
                //tmp.text = "" + j + "," + i;
                //tmp.alignment = TextAlignmentOptions.Center;
                //tmp.fontSize = 36f;

                //tmp.textContainer.width = 10f;
                //tmp.textContainer.height = 10f;

                //text.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

                //tmp.color = (currentMaterial == black) ? Color.white: Color.black;


                //RectTransform rt = quad.GetComponent<RectTransform>();
                //rt.sizeDelta = new Vector2(1f, 1f);

                currentMaterial = (currentMaterial == black) ? white : black;

            }
            if (Width % 2 == 0)
            {
                currentMaterial = (currentMaterial == black) ? white : black;
            }
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private List<GameObject> gameQuads = new List<GameObject>();
}
