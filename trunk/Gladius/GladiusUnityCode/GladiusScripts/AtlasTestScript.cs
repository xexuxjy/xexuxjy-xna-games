using UnityEngine;
using System.Collections;
using System;

public class AtlasTestScript : MonoBehaviour
{

    public int Region;
    public int OldRegion;
    public Vector2[] m_uvCopy;

    // Use this for initialization
    void Start()
    {
        m_moveAtlasTexture = Resources.Load<Texture2D>("GladiusArena/grid-atlas");
        m_moveAtlas = new TextureAtlas("GladiusArena/grid-atlas-key");
        m_uvCopy = new Vector2[4];
        MeshFilter viewedModelFilter = (MeshFilter)gameObject.GetComponent("MeshFilter");
        viewedModelFilter.mesh.uv = m_uvCopy;
        renderer.material = new Material(Shader.Find("Transparent/Diffuse"));
        viewedModelFilter.renderer.material.mainTexture = m_moveAtlasTexture;

    }

    // Update is called once per frame
    void Update()
    {
        if (OldRegion != Region )
        {
            OldRegion = Region;


            Rect uvg = new Rect();
            GetUV(ref uvg);
            int vcount = 0;
            m_uvCopy[vcount] = new Vector2(uvg.x, uvg.y);
            m_uvCopy[vcount + 1] = new Vector2(uvg.width, uvg.y);
            m_uvCopy[vcount + 2] = new Vector2(uvg.x, uvg.height);
            m_uvCopy[vcount + 3] = new Vector2(uvg.width, uvg.height);

            MeshFilter viewedModelFilter = (MeshFilter)gameObject.GetComponent("MeshFilter") ;
            viewedModelFilter.mesh.uv = m_uvCopy;

            //m_mesh.uv = m_uvCopy;

        }

    }

    public void GetUV(ref Rect uvs)
    {
        TextureRegion region = null;
        if (Region == 0) region = m_moveAtlas.GetRegion(CTBlank);
        if (Region == 1) region = m_moveAtlas.GetRegion(CTEmpty);
        if (Region == 2) region = m_moveAtlas.GetRegion(CTSelect);
        if (Region == 3) region = m_moveAtlas.GetRegion(CTCross);
        if (Region == 4) region = m_moveAtlas.GetRegion(CTForwardH);
        if (Region == 5) region = m_moveAtlas.GetRegion(CTForwardV);
        if (Region == 6) region = m_moveAtlas.GetRegion(CTTurnBL);
        if (Region == 7) region = m_moveAtlas.GetRegion(CTTurnBR);
        if (Region == 8) region = m_moveAtlas.GetRegion(CTTurnTR);
        if (Region == 9) region = m_moveAtlas.GetRegion(CTTurnTL);

        if (region != null)
        {
            uvs = region.BoundsUV;
        }

    }

    public const String CTBlank = "Zero.png";
    public const String CTEmpty = "Blank_1.png";
    public const String CTSelect = "Select1_1.png";
    public const String CTCross = "Cross_1.png";
    public const String CTForwardH = "ForwardH.png";
    public const String CTForwardV = "ForwardV.png";

    public const String CTTurnBL = "TurnBL.png";
    public const String CTTurnBR = "TurnBR.png";
    public const String CTTurnTL = "TurnTL.png";
    public const String CTTurnTR = "TurnTR.png";

    public const String CTEndMoveB = "EndMoveB.png";
    public const String CTEndMoveT = "EndMoveT.png";
    public const String CTEndMoveL = "EndMoveL.png";
    public const String CTEndMoveR = "EndMoveR.png";

    public const String CTTarget = "Target_1.png";


    Texture2D m_moveAtlasTexture;
    TextureAtlas m_moveAtlas;
}
