using UnityEngine;
using System.Collections;

public class RandomAnimation : MonoBehaviour {

    //public Random m_random = new Random();

    Material m_material;

	// Use this for initialization
	void Start () 
    {
        Object[] clips = Resources.LoadAll("warriors", typeof(AnimationClip));
        Debug.Log("Start called. " + GetComponent<Animation>().GetClipCount()+"  ,  "+clips.Length);
        foreach (var o in clips)
        {
            AnimationClip c = o as AnimationClip;
            GetComponent<Animation>().AddClip(c, c.name.Contains("@") ? c.name.Substring(c.name.LastIndexOf("@") + 1) : c.name);
        }

        m_material = Resources.Load("Health", typeof(Material)) as Material;

    }

    //void Awake()
    //{
    //    Object[] clips = Resources.LoadAll("warriors/" + name, typeof(AnimationClip));
    //    foreach (var o in clips)
    //    {
    //        AnimationClip c = o as AnimationClip;
    //        animation.AddClip(c, c.name.Contains("@") ? c.name.Substring(c.name.LastIndexOf("@") + 1) : c.name);
    //    }

    //    //foreach (var a in animation.Cast<AnimationState>())
    //    //{
    //    //    a.enabled = true;
    //    //}
    //}

	// Update is called once per frame
	void Update () 
    {
        if (!GetComponent<Animation>().isPlaying)
        {
            float f = Random.value;
            if (f < 0.5f)
            {
                GetComponent<Animation>().Play("w_bow_action");
            }
            else
            {
                GetComponent<Animation>().Play("w_death-1");
            }
        }
        else
        {
        }

        GL.Color(Color.white);

        m_material.SetPass(0);


        float markerHeight = 20;
        float markerWidth = 20;

        GL.PushMatrix();

        {

            GL.LoadOrtho();

            GL.LoadIdentity();



            //pos.x *= ar;

            //GL.MultMatrix(Matrix4x4.Scale(new Vector3(1.0f , 1.0f, 1.0f)) * Matrix4x4.TRS(pos, q, Vector3.one));
            GL.MultMatrix(gameObject.transform.localToWorldMatrix);
            //Matrix4x4.

            GL.Begin(GL.QUADS);

            {
                GL.TexCoord2(0, 0);
                GL.Vertex3(-markerWidth, 0f,-markerHeight);
                GL.TexCoord2(1, 0);
                GL.Vertex3(-markerWidth, 0f,markerHeight);
                GL.TexCoord2(1, 1);
                GL.Vertex3(markerWidth, 0f,markerHeight);
                GL.TexCoord2(0, 1);
                GL.Vertex3(markerWidth, 0f,-markerHeight);

            }

            GL.End();

        }

        GL.PopMatrix();

	}

    
}
