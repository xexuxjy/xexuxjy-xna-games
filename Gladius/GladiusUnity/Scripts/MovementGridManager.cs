using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MovementGridManager : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        m_gridAtlasTexture = Resources.Load<Texture2D>("GladiusUI/Arena/ArenaUIAtlas");
        m_gridAtlas = new TPTextureAtlas();
        m_gridAtlas.BuildTPSheet("GladiusUI/Arena/ArenaUIAtlas_data", m_gridAtlasTexture);
        gameObject.transform.position = Vector3.zero;
        for (int i = 0; i < 30; ++i)
        {
            CreateQuad();
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ReleaseSquare(GameObject go)
    {
        Debug.Assert(m_usedQuads.Contains(go));
        go.GetComponent<SquareInfo>().Reset();
        m_usedQuads.Remove(go);
        m_availableQuads.Add(go);
    }

    public GameObject AddSquareVertical(Arena arena, Point point, Vector3 direction, float heightOffset, bool up, BaseActor actor, bool disabled)
    {
        direction.y = 0f;
        direction.Normalize();

        Vector3 topLeft = arena.GetArenaPoint(point);

        //topLeft += direction * (Arena.ArenaMeshScalar/2.0f);

        Vector3 pos = topLeft;

        pos.y += heightOffset;
        //pos.y += (quarterDims) * (up ? 1.0f : -1.0f);
        pos.y += (halfDims) * (up ? 1.0f : -1.0f);

        if (!up)
        {
            pos += direction * Arena.ArenaMeshScalar;
        }

        pos += (up ? -direction : direction) * s_hoverHeight;

        string region = CursorStraight;
        Quaternion rot = Quaternion.identity;
        Vector3[] vertices = null;
        //Quaternion rot = Quaternion.identity;
        if (direction == Vector3.forward)
        {
            vertices = up?riserPointsForward:riserPointsBackward;
            //rot = Quaternion.AngleAxis(270, Vector3.right);
            // in demo scene 0,0,0.5 ,0,180,90 , scale = 0.25,1,1

        }
        else if (direction == Vector3.back)
        {
            vertices = up?riserPointsBackward:riserPointsForward;

            // in demo scene 0,0,-0.5 0,0,90 , scale = 0.25,1,1
            //rot = Quaternion.AngleAxis(90, Vector3.right);
        }
        else if (direction == Vector3.right)
        {
            vertices = up?riserPointsRight:riserPointsLeft;
            if (!up)
            {
                int ibreak = 0;
            }
            // in demo scene 0.5,0,0 0,270,90 , scale = 0.25,1,1
            //rot = Quaternion.Euler(90, 270, 0);
            //rot = Quaternion.AngleAxis(90,Vector3.back);
        }
        else if (direction == Vector3.left)
        {
            vertices = up?riserPointsLeft:riserPointsRight;

            // in demo scene -0.5,0,0 0,90,90 , scale = 0.25,1,1
            //rot = Quaternion.Euler(90, 90, 0);
            //rot = Quaternion.AngleAxis(270,Vector3.forward);
        }

        return AddSquare(region, pos, rot, vertices, actor.TeamColour, disabled);
    }


    const float s_hoverHeight = 0.05f;

    public GameObject AddSquare(Arena arena, Point prevPoint, Point point, Point nextPoint, BaseActor actor, bool prevExists, bool nextExists, bool disabled)
    {
        GameObject result = null;
        Vector3 topLeft = arena.GetArenaPoint(point);
        Vector3 pos = topLeft;
        Vector3 scale = new Vector3(Arena.ArenaMeshScalar, 1, Arena.ArenaMeshScalar);

        Vector2 v2 = new Vector2(point.X, point.Y);
        Vector2 v2p = new Vector2(prevPoint.X, prevPoint.Y);
        Vector2 v2n = new Vector2(nextPoint.X, nextPoint.Y);

        //Matrix rot = Matrix.Identity;

        Color squareColor = actor.TeamColour;

        int steps = 0;

        Vector2 diffPrevious = v2 - v2p;
        Vector3 diffNext = v2n - v2;

        Quaternion rot = Quaternion.identity;
        string region = "";

        pos.y += s_hoverHeight;

        if (prevExists)
        {
            Side enterSide = Side.Left;
            Side exitSide = Side.Right;
            if (diffPrevious.x == 1)
            {
                enterSide = Side.Left;
            }
            else if (diffPrevious.x == -1)
            {
                enterSide = Side.Right;
            }
            else if (diffPrevious.y == -1)
            {
                enterSide = Side.Bottom;
            }
            else
            {
                enterSide = Side.Top;
            }

            if (diffNext.x == 1)
            {
                exitSide = Side.Right;
            }
            else if (diffNext.x == -1)
            {
                exitSide = Side.Left;
            }
            else if (diffNext.y == -1)
            {
                exitSide = Side.Top;
            }
            else
            {
                exitSide = Side.Bottom;
            }

            if (nextExists)
            {
                if (MovementGrid.CompareSide(enterSide, exitSide, Side.Left, Side.Right))
                {
                    region = CursorStraight;
                    rot = Quaternion.AngleAxis(90, Vector3.up);
                }
                else if (MovementGrid.CompareSide(enterSide, exitSide, Side.Top, Side.Bottom))
                {
                    region = CursorStraight;
                }
                else if (MovementGrid.CompareSide(enterSide, exitSide, Side.Left, Side.Top))
                {
                    // correct
                    region = CursorLeftTurn;
                    squareColor = Color.cyan;
                    //Debug.Log("LeftTop");
                }
                else if (MovementGrid.CompareSide(enterSide, exitSide, Side.Left, Side.Bottom))
                {
                    region = CursorLeftTurn;
                    squareColor = Color.yellow;
                    rot = Quaternion.AngleAxis(90, Vector3.up);
                    //Debug.Log("LeftBottom");

                    //rot = Quaternion.AngleAxis(270, Vector3.up);
                }
                else if (MovementGrid.CompareSide(enterSide, exitSide, Side.Right, Side.Top))
                {
                    //correct.
                    region = CursorRightTurn;
                    squareColor = Color.blue;

                    //Debug.Log("RightTop");
                }
                else if (MovementGrid.CompareSide(enterSide, exitSide, Side.Right, Side.Bottom))
                {
                    region = CursorRightTurn;
                    squareColor = Color.magenta;
                    rot = Quaternion.AngleAxis(270, Vector3.up);

                    //Debug.Log("RightBottom");
                    //rot = Quaternion.AngleAxis(270, Vector3.up);
                }
            }
            else
            {
                //region = CursorEnd;
                region = CursorEnd;
                switch (enterSide)
                {
                    case (Side.Left):
                        rot = Quaternion.AngleAxis(90, Vector3.up);
                        break;
                    case (Side.Right):
                        rot = Quaternion.AngleAxis(270, Vector3.up);
                        break;
                    case (Side.Top):
                        rot = Quaternion.AngleAxis(0, Vector3.up);

                        break;
                    case (Side.Bottom):
                        rot = Quaternion.AngleAxis(180, Vector3.up);
                        break;
                }
            }
        }
        else
        {
            if (nextExists)
            {
                region = CursorStart;
            }
            else if(actor.UnitActive)
            {
                region = CursorMapGrid;
            }
        }
        if (!String.IsNullOrEmpty(region))
        {
            result = AddSquare(region, pos, rot, squarePoints, squareColor, disabled);
        }
        return result;
    }


    public GameObject AddSquare(Arena arena, string textureName, Point point, Color teamColor)
    {
        GameObject go = GetQuad();
        Vector3 topLeft = arena.GetArenaPoint(point);
        Vector3 pos = topLeft;
        pos.y += s_hoverHeight;
        Quaternion rot = Quaternion.AngleAxis(0, Vector3.up);

        go.GetComponent<SquareInfo>().Init(m_gridAtlas, m_gridAtlasTexture, textureName, pos, rot, squarePoints, teamColor, false);
        return go;
    }




    public GameObject AddSquare(string textureName, Vector3 pos, Quaternion rot, Vector3[] vertices, Color teamColor, bool disabled)
    {
        GameObject go = GetQuad();
        go.GetComponent<SquareInfo>().Init(m_gridAtlas, m_gridAtlasTexture, textureName, pos, rot, vertices, teamColor, disabled);
        return go;
    }


    public GameObject GetQuad()
    {
        if (m_availableQuads.Count == 0)
        {
            CreateQuad();
        }

        GameObject go = m_availableQuads[0];
        m_availableQuads.RemoveAt(0);
        Debug.Assert(!m_usedQuads.Contains(go));
        m_usedQuads.Add(go);
        return go;
    }

    private int m_quadCounter;
    const float dims = 0.5f * Arena.ArenaMeshScalar;
    const float halfDims = dims / 2.0f;
    //const float quarterDims = dims / 4.0f;
    //static Vector3[] squarePoints = new Vector3[] { new Vector3(-dims, 0, -dims), new Vector3(dims, 0, -dims), new Vector3(dims, 0, dims), new Vector3(-dims, 0, dims) };
    //static Vector3[] riserPointsForward = new Vector3[] { new Vector3(-dims, -quarterDims, dims), new Vector3(dims, -quarterDims, dims), new Vector3(dims, quarterDims, dims), new Vector3(-dims, quarterDims, dims) };
    //static Vector3[] riserPointsBackward = new Vector3[] { new Vector3(-dims, quarterDims, -dims), new Vector3(dims, quarterDims, -dims), new Vector3(dims, -quarterDims, -dims), new Vector3(-dims, -quarterDims, -dims) };
    //static Vector3[] riserPointsRight = new Vector3[] {new Vector3(dims, quarterDims, -dims),new Vector3(dims, quarterDims, dims),new Vector3(dims, -quarterDims, dims),new Vector3(dims, -quarterDims, -dims)};
    //static Vector3[] riserPointsLeft = new Vector3[] { new Vector3(-dims, -quarterDims, -dims), new Vector3(-dims, -quarterDims, dims), new Vector3(-dims, quarterDims, dims), new Vector3(-dims, quarterDims, -dims) };

    static Vector3[] squarePoints = new Vector3[] { new Vector3(-dims, 0, -dims), new Vector3(dims, 0, -dims), new Vector3(dims, 0, dims), new Vector3(-dims, 0, dims) };
    static Vector3[] riserPointsForward = new Vector3[] { new Vector3(-dims, -halfDims, dims), new Vector3(dims, -halfDims, dims), new Vector3(dims, halfDims, dims), new Vector3(-dims, halfDims, dims) };
    static Vector3[] riserPointsBackward = new Vector3[] { new Vector3(-dims, halfDims, -dims), new Vector3(dims, halfDims, -dims), new Vector3(dims, -halfDims, -dims), new Vector3(-dims, -halfDims, -dims) };
    static Vector3[] riserPointsRight = new Vector3[] { new Vector3(dims, halfDims, -dims), new Vector3(dims, halfDims, dims), new Vector3(dims, -halfDims, dims), new Vector3(dims, -halfDims, -dims) };
    static Vector3[] riserPointsLeft = new Vector3[] { new Vector3(-dims, -halfDims, -dims), new Vector3(-dims, -halfDims, dims), new Vector3(-dims, halfDims, dims), new Vector3(-dims, halfDims, -dims) };


    static int[] triangles = new int[] { 2, 1, 0, 0, 3, 2 };
    private void CreateQuad()
    {
        //GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
        GameObject go = new GameObject();
        go.name = "GridQuad-" + (m_quadCounter++);
        go.transform.parent = transform;
        Renderer renderer = go.AddComponent<MeshRenderer>();
        //go.GetComponent<Renderer>().material = m_material;
        renderer.material = Instantiate(m_material);
        MeshFilter meshFilter = go.AddComponent<MeshFilter>();
        SquareInfo squareInfo = go.AddComponent<SquareInfo>();

        squareInfo.m_mesh = meshFilter.mesh;
        squareInfo.m_mesh.vertices = squarePoints;

        squareInfo.m_mesh.triangles = triangles;

        m_availableQuads.Add(go);
    }

    public List<GameObject> m_availableQuads = new List<GameObject>();
    public List<GameObject> m_usedQuads = new List<GameObject>();

    public const string CursorLeftTurn = "CursorTurnLeft";
    public const string CursorRightTurn = "CursorTurnRight";
    public const string CursorStraight = "CursorStraight";
    public const string CursorStart = "CursorStart";
    public const string CursorEnd = "CursorEnd";
    public const string CursorMapGrid = "mapgrid_cursor";
    public const string CursorTarget = "targetable";
    public const string CursorBlank = "arena_cursor";
    public const string CursorDefault = "arena_default";


    private TPTextureAtlas m_gridAtlas;
    private Texture2D m_gridAtlasTexture;
    public Material m_material;
}

public class SquareInfo : MonoBehaviour
{
    public void Init(TPTextureAtlas textureAtlas, Texture texture,string textureName, Vector3 pos, Quaternion rot,Vector3[] vertices,Color teamColor, bool disabled)
    {
        m_textureName = textureName;
        m_teamColor = teamColor;
        m_pos = pos;
        m_rot = rot;
        m_disabled = disabled;
        if (disabled)
        {
            int ibreak = 0;
        }
        //gameObject.GetComponent<Renderer>().material.SetColor("Tint", teamColor);
        gameObject.GetComponent<Renderer>().material.SetColor("_ColorTint", teamColor);
        gameObject.GetComponent<Renderer>().material.SetFloat("_Disabled", m_disabled?1.0f:0.0f);
        //gameObject.GetComponent<Renderer>().material.mainTexture = texture;
        TPTextureRegion textureRegion = textureAtlas.GetRegion(textureName);
        Rect uvBounds = textureRegion.BoundsUV;
        m_uvs[0] = new Vector2(uvBounds.x, uvBounds.y);
        m_uvs[1] = new Vector2(uvBounds.width, uvBounds.y);
        m_uvs[2] = new Vector2(uvBounds.width, uvBounds.height);
        m_uvs[3] = new Vector2(uvBounds.x, uvBounds.height);


        int index = 0;
        //m_uvs[index++] = new Vector2(uvBounds.x, uvBounds.height);
        //m_uvs[index++] = new Vector2(uvBounds.x, uvBounds.y);
        //m_uvs[index++] = new Vector2(uvBounds.width, uvBounds.y);
        //m_uvs[index++] = new Vector2(uvBounds.width, uvBounds.height);


        m_mesh.uv = m_uvs;
        if (vertices == null || vertices.Length != 4)
        {
            int ibreak = 0;
        }
        m_mesh.vertices = vertices;

        gameObject.transform.localPosition = m_pos;
        gameObject.transform.localRotation = m_rot;
        //gameObject.transform.localScale = m_scale;

        gameObject.SetActive(true);
    }

    public void Reset()
    {
        gameObject.SetActive(false);
    }

    public string m_textureName;
    public Color m_teamColor;
    public Vector3 m_pos;
    public Quaternion m_rot;
    public Vector3 m_scale;
    public bool m_disabled;
    public Mesh m_mesh;
    public Vector2[] m_uvs = new Vector2[4];
}