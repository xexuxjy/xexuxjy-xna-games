using UnityEngine;
using System.Collections;
using Vectrosity;
using System.Collections.Generic;
public class TestVectrocity : MonoBehaviour
{
    VectorLine m_vectorLine;
    int rotationAngle = 0;
    int counter = 0;
    // Use this for initialization
    void Start()
    {
        //var line = new VectorLine("3DLine", new Vector3[]{new Vector3(0,-2,-4), new Vector3(0,4,3)}, null, 2.0f);
        //line.Draw3DAuto();
        DrawBox(Vector3.zero, new Vector3(5, 10, 5));
        DrawBox(new Vector3(-10,-2,-5), new Vector3(5, 10, 5));
        DrawBox(new Vector3(10, -2, -5), new Vector3(5, 10, 5));
        DrawBox(new Vector3(-10, -2, 5), new Vector3(5, 10, 5));


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rotationAngle++;

        Quaternion currentRotation = Quaternion.Euler(45, rotationAngle, 0);

        transform.rotation = currentRotation;
        foreach (VectorLine vl in m_boxPool)
        {
            vl.drawTransform = transform;
        }
        //m_vectorLine.drawTransform= transform;
        counter++;
        if (counter > 120)
        {
            m_boxPool[0].active = !m_boxPool[0].active;
            counter = 0;
        }
    }

    void DrawBox(Vector3 position, Vector3 dims)
    {
        Vector3 halfDims = dims / 2f;
        Vector3[] corners = new Vector3[8];
        int count  =0;
        corners[count++] = position + new Vector3(-halfDims.x, -halfDims.y, -halfDims.z);
        corners[count++] = position + new Vector3(halfDims.x, -halfDims.y, -halfDims.z);
        corners[count++] = position + new Vector3(halfDims.x, -halfDims.y, halfDims.z);
        corners[count++] = position + new Vector3(-halfDims.x, -halfDims.y, halfDims.z);
        corners[count++] = position + new Vector3(-halfDims.x, halfDims.y, -halfDims.z);
        corners[count++] = position + new Vector3(halfDims.x, halfDims.y, -halfDims.z);
        corners[count++] = position + new Vector3(halfDims.x, halfDims.y, halfDims.z);
        corners[count++] = position + new Vector3(-halfDims.x, halfDims.y, halfDims.z);

        VectorLine vectorLine = new VectorLine("3DLine", new Vector3[] { corners[0], corners[1], 
                corners[1], corners[2], 
                corners[2], corners[3], 
                corners[3], corners[0],
                corners[4], corners[5], 
                corners[5], corners[6], 
                corners[6], corners[7], 
                corners[7], corners[4],
                corners[0],corners[4],
                corners[1],corners[5],
                corners[2],corners[6],
                corners[3],corners[7]
        
        }, null, 4.0f);
        vectorLine.Draw3DAuto();
        m_boxPool.Add(vectorLine);

    }


    List<VectorLine> m_boxPool = new List<VectorLine>();

}
