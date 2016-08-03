using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using UnityEngine;
using UnityEditor;

public class MovementGrid : MonoBehaviour
{
    public bool DrawDebugGrid = true;
    private bool m_drawDebugGrid = true;
    private Arena m_arena;
    public CombatEngine CombatEngine;
    public PathMesh m_pathMesh;
    public MovementGridManager MovementGridManager;

    public Arena Arena
    {
        get
        {
            return m_arena;
        }
        set
        {
            m_arena = value;
        }

    }

    public void FocusCamera(CameraManager cameraManager)
    {
        if (cameraManager != null)
        {
            cameraManager.ReparentTarget(MovementGridCursor.transform);
        }
    }

    public void FocusCamera(CameraManager cameraManager,Vector3 forward)
    {
        if (cameraManager != null)
        {
            cameraManager.ReparentTarget(MovementGridCursor.transform,forward);
        }
    }

    public void ResetGrid()
    {
        m_pathMesh.Reset();
    }

    public int SquareIndex(int x, int y)
    {
        return (x * Arena.ArenaSize) + y;
    }

    public void Start()
    {
        //BuildDictionary();
        //BuildMesh();
        RebuildMesh = true;
        Vector3 pos = gameObject.transform.position;
        pos.y += 0.2f;
        gameObject.transform.position = pos;
        MovementGridCursor = new GameObject();
        MovementGridCursor.name = "MovementGridCursor";
        MovementGridCursor.transform.parent = gameObject.transform;
        m_pathMesh = gameObject.AddComponent<PathMesh>();
        m_pathMesh.Setup(this);
    }

    public bool RebuildMesh
    {
        get;
        set;
    }

    public void BuildMaskForGrid(Point centerPoint, AttackSkill skill)
    {
        int distance = GladiusGlobals.PathDistance(BaseActor.ArenaPoint, centerPoint);
        DrawSkill(skill.SkillRangeName, skill.SkillRange, BaseActor.ArenaPoint, centerPoint, MovementGridManager.CursorDefault);
        DrawSkill(skill.SkillExcludeRangeName, skill.SkillExcludeRange, BaseActor.ArenaPoint, centerPoint, MovementGridManager.CursorDefault);
    }

    public void DrawMovementRange()
    {
        if (BaseActor.CurrentAttackSkill.IsMoveToAttack)
        {
            int range = BaseActor.CurrentAttackSkill.SkillMoveRange;
            BuildMinMaxCircle(BaseActor.ArenaPoint, 0, range, MovementGridManager.CursorDefault);
        }
    }



    public void DrawSkill(String name, int range, Point start, Point end, string texture)
    {
        int distance = GladiusGlobals.PathDistance(start, end);
        if (distance > range)
        {
            return;
        }

        switch (name)
        {
            case "Self":
                SetSkillActivePoint(start, texture);
                break;

            case "Square":
            case "Square2x2":
                break;

            case "Plus":
                BuildCross(end, 1, texture);
                break;
            case "Plus2x2":
                BuildCross(end, 2, texture);
                break;
            case "Plus3x3":
                BuildCross(end, 3, texture);
                break;
            case "Linear":
                break;
            case "Star":
                BuildStar(start, range, texture);
                break;
            case "Diamond":
                BuildDiamond(start, range, texture);
                break;
            case "Cone":
                BuildCone(start, end, range, texture);
                break;
        }
    }

    public void BuildCross(Point start, int armLength, string texture)
    {
        for (int i = 1; i <= armLength; ++i)
        {
            SetSkillActivePoint(Point.Add(start, new Point(i, 0)), texture);
            SetSkillActivePoint(GladiusGlobals.Add(start, new Point(-i, 0)), texture);
            SetSkillActivePoint(GladiusGlobals.Add(start, new Point(0, i)), texture);
            SetSkillActivePoint(GladiusGlobals.Add(start, new Point(0, -i)), texture);
        }
    }

    private int[] m_coneSlots = new int[] { 1, 3, 3, 5, 5, 5, 7, 7, 7, 7 };

    public void BuildCone(Point startPoint, Point endPoint, int length, string texture)
    {
        Point offset1 = GladiusGlobals.CardinalNormalize(GladiusGlobals.Subtract(endPoint, startPoint));
        Point offset2 = GladiusGlobals.Cross(offset1);

        for (int i = 0; i < length; ++i)
        {
            int slotCount = m_coneSlots[i];
            Point rowPoint = GladiusGlobals.Add(startPoint, GladiusGlobals.Mult(offset1, i));
            int midPoint = slotCount / 2;
            for (int j = 0; j < slotCount; ++j)
            {
                int diff = j - midPoint;
                Point offPoint = GladiusGlobals.Mult(offset2, diff);
                Point p = GladiusGlobals.Add(rowPoint, offPoint);
                SetSkillActivePoint(p, texture);
            }
        }
    }

    public void BuildDiamond(Point startPoint, int armLength, string texture)
    {
        int rowCount = armLength;
        Point offset2 = new Point(1, 0);

        for (int i = 0; i < armLength; i++)
        {
            int midPoint = rowCount / 2;
            for (int j = 0; j < rowCount; ++j)
            {
                int diff = j - midPoint;
                Point offPoint = GladiusGlobals.Mult(offset2, diff);
                offPoint.Y = i;
                Point offPoint2 = new Point(offPoint.X, -offPoint.Y);
                Point p = GladiusGlobals.Add(startPoint, offPoint);
                Point p2 = GladiusGlobals.Add(startPoint, offPoint2);
                SetSkillActivePoint(p, texture);
                SetSkillActivePoint(p2, texture);

            }
            rowCount -= 2;
        }
    }

    public void BuildStar(Point startPoint, int armLength, string texture)
    {
        Point tl = new Point(-1, 1);
        Point t = new Point(0, 1);
        Point tr = new Point(1, 1);
        Point l = new Point(-1, 0);
        Point r = new Point(1, 0);
        Point bl = new Point(-1, -1);
        Point b = new Point(0, -1);
        Point br = new Point(1, -1);

        for (int i = 0; i < armLength; ++i)
        {
            SetSkillActivePoint(GladiusGlobals.Add(startPoint, GladiusGlobals.Mult(tl, i)), texture);
            SetSkillActivePoint(GladiusGlobals.Add(startPoint, GladiusGlobals.Mult(t, i)), texture);
            SetSkillActivePoint(GladiusGlobals.Add(startPoint, GladiusGlobals.Mult(tr, i)), texture);
            SetSkillActivePoint(GladiusGlobals.Add(startPoint, GladiusGlobals.Mult(l, i)), texture);
            SetSkillActivePoint(GladiusGlobals.Add(startPoint, GladiusGlobals.Mult(r, i)), texture);
            SetSkillActivePoint(GladiusGlobals.Add(startPoint, GladiusGlobals.Mult(bl, i)), texture);
            SetSkillActivePoint(GladiusGlobals.Add(startPoint, GladiusGlobals.Mult(b, i)), texture);
            SetSkillActivePoint(GladiusGlobals.Add(startPoint, GladiusGlobals.Mult(br, i)), texture);
        }

    }

    public void BuildCenteredGrid(Point centerPoint, int size, string texture)
    {
        if (size > 0)
        {
            int width = size;//((size - 1) / 2);

            for (int i = -width; i <= width; ++i)
            {
                for (int j = -width; j <= width; ++j)
                {
                    Point p = new Point(centerPoint.X + i, centerPoint.Y + j);
                    SetSkillActivePoint(p, texture);
                }
            }
        }
    }



    public void BuildMinMaxCircle(Point centerPoint, int min, int max, String texture)
    {
        int width = max;//((max - 1) / 2);
        float min2 = min * min;
        float max2 = max * max;
        for (int i = -width; i <= width; ++i)
        {
            for (int j = -width; j <= width; ++j)
            {
                Point p = new Point(centerPoint.X + i, centerPoint.Y + j);
                float dist2 = GladiusGlobals.PointDist2(p, centerPoint);
                if (dist2 >= min2 && dist2 <= max2)
                {
                    SetSkillActivePoint(p, texture);
                }
            }
        }

    }



    public void SetSkillActivePoint(Point p, String texture)
    {
        if (p.X > 0 && p.X < Arena.ArenaSize && p.Y > 0 && p.Y < Arena.ArenaSize)
        {
            m_pathMesh.AddSquare(p, texture, Color.white);
        }
    }


    private void RebuildForActor()
    {
        if (BaseActor != null)
        {
            ResetGrid();

            if (BaseActor.UnitActive)
            {
                if (!BaseActor.FollowingWayPoints)
                {
                    //m_pathMesh.AddSquare(BaseActor.ArenaPoint, MovementGridManager.CursorMapGrid, BaseActor.TeamColour);
                    m_pathMesh.AddSquare(CurrentCursorPoint, MovementGridManager.CursorMapGrid, BaseActor.TeamColour);
                }

                if (BaseActor.CurrentAttackSkill != null)
                {
                    if (BaseActor.CurrentAttackSkill.IsMoveToAttack)
                    {
                        DrawMovementRange();
                        DrawMovementPath();
                    }
                    else
                    {
                        BuildMaskForGrid(BaseActor.ArenaPoint, BaseActor.CurrentAttackSkill);
                    }
                }
            }
            //RebuildMeshData();
        }
    }

    public void Update()
    {
        if (BaseActor != null)
        {
            //renderer.material.SetColor("Main Color", CurrentActor.TeamColour);
        }

        if (MovementGridCursor != null)
        {
            MovementGridCursor.transform.position = CurrentV3;
        }


        if (RebuildMesh)
        {
            RebuildForActor();
            RebuildMesh = false;
        }
    }

    public void DrawIfValid(Point prevPoint, Point point, Point nextPoint, BaseActor actor, bool prevExists, bool nextExists, bool disabled)
    {
        if (m_pathMesh != null)
        {
            m_pathMesh.AddSquare(prevPoint,point, nextPoint,BaseActor,prevExists,nextExists,disabled);
        }
    }

    public static bool CompareSide(Side side1, Side side2, Side check1, Side check2)
    {
        return (side1 == check1 && side2 == check2) || (side1 == check2 && side2 == check1);
    }


    public void DrawMovementPath()
    {
        // if the last point we're moving to is next to our target , then we should probably move to the target as well?
        // still think the logic is wrong.
        int numPoints = BaseActor.WayPointList.Count;

        if (BaseActor.Target != null && numPoints> 0)
        {
            if (GladiusGlobals.PathDistance(BaseActor.Target.ArenaPoint, BaseActor.WayPointList[numPoints - 1]) == 1)
            {
                //m_pointsCopy.Add(actor.Target.ArenaPoint);
            }
        }

        Point prev = new Point();
        Point curr = new Point();
        Point next = new Point();

        int skillRange = BaseActor.CurrentAttackSkill.TotalSkillRange;

        for (int i = 0; i < numPoints; ++i)
        {
            //GridSquareTextureType squareType = GridSquareTextureType.None;
            bool disabled = !BaseActor.CurrentAttackSkill.InRange(i);
            prev = curr;
            if (i == 0)
            {
                prev = BaseActor.ArenaPoint;
            }


            curr = BaseActor.WayPointList[i];
            if (i < (numPoints - 1))
            {
                next = BaseActor.WayPointList[i + 1];
                //DrawIfValid(prev, curr, next, BaseActor, (i >= 0), true, disabled);
                DrawIfValid(prev, curr, next, BaseActor, (i > 0), true, disabled);
            }
            else
            {
                DrawIfValid(prev, curr, next, BaseActor, true, false, disabled);
            }
        }
        
        // draw the target point if it's 
        if (BaseActor.ActionSelected)
        {
            //m_pathMesh.AddSquare(curr, MovementGridManager.CursorTarget, BaseActor.TeamColour);
        }


        if (BaseActor.WayPointList.Count == 0)
        {
            curr = BaseActor.ArenaPoint;
        }
        //DrawDefaultBlanks(curr);

        BuildMaskForGrid(CurrentCursorPoint, BaseActor.CurrentAttackSkill);

    }


    public void DrawDefaultBlanks(Point curr)
    {
        // draw blanks and targets around last point?
        DrawBlankOrOccupied(GladiusGlobals.Add(curr, new Point(1, 0)));
        DrawBlankOrOccupied(GladiusGlobals.Add(curr, new Point(-1, 0)));
        DrawBlankOrOccupied(GladiusGlobals.Add(curr, new Point(0, 1)));
        DrawBlankOrOccupied(GladiusGlobals.Add(curr, new Point(0, -1)));
    }

    private void DrawBlankOrOccupied(Point p)
    {
        if (Arena.ValidPointInLevel(p))
        {
            if (!BaseActor.WayPointList.Contains(p))
            {
                string cursor = MovementGridManager.CursorBlank;
                Color color = BaseActor.TeamColour;
                BaseActor target = Arena.GetActorAtPosition(p);
                if (target != BaseActor)
                {
                    if (target != null)
                    {
                        cursor = MovementGridManager.CursorTarget;
                        color = Color.red;
                    }
                    m_pathMesh.AddSquare(p, cursor, color);
                }
            }
        }
    }

    public bool CursorOnTarget(BaseActor source)
    {
        if (CombatEngine != null)
        {
            BaseActor ba = Arena.GetActorAtPosition(CurrentCursorPoint);
            return (ba != null && CombatEngine != null && CombatEngine.IsValidTarget(source, ba, source.CurrentAttackSkill));
        }
        return false;
    }


    public Point CurrentCursorPoint
    {
        get
        {
            return m_currentPosition;
        }
        set
        {
            m_currentPosition = value;

            if (!Arena.InLevelBounds(m_currentPosition)
                )
            {
                int ibreak = 0;
            }
        }
    }

    public Vector3 CurrentV3
    {
        get
        {
            return Arena.GridPointToWorld(CurrentCursorPoint);
        }
    }


    public BaseActor BaseActor
    {
        get;
        set;
    }



    public Vector3 m_cursorMovement = Vector3.zero;
    public int CurrentCursorSize = 5;
    public const float m_hover = 0.01f;

    public GameObject MovementGridCursor;

    public Point m_currentPosition;

}




public enum Side
{
    Left,
    Right,
    Top,
    Bottom
}

public enum Rot
{
    Zero,
    Ninety,
    OneEighty,
    TwoSeventy
}


public class PathMesh : MonoBehaviour
{
    public MovementGrid Owner;
    public void Setup(MovementGrid owner)
    {
        Owner = owner;
    }

    public Arena Arena
    {
        get
        {
            if (Owner != null && Owner.Arena != null)
            {
                return Owner.Arena;
            }
            return null;
        }
    }


    public MovementGridManager MovementGridManager
    {
        get
        {
            return GameObject.Find("MovementGridManager").GetComponent<MovementGridManager>();
        }
    }

    public void AddSquare(Point point, string textureName, Color color)
    {
        GameObject go = MovementGridManager.AddSquare(Arena, textureName, point, color);
        if (go != null)
        {
            m_quadList.Add(go);
        }
    }

    public void AddSquare(Point prevPoint, Point point, Point nextPoint, BaseActor actor, bool prevExists, bool nextExists, bool disabled)
    {

        Vector3 vPrev = new Vector3(prevPoint.X, Arena.GetHeightAtLocation(prevPoint), prevPoint.Y);
        Vector3 vCurrent = new Vector3(point.X, Arena.GetHeightAtLocation(point), point.Y);
        Vector3 vNext = new Vector3(nextPoint.X, Arena.GetHeightAtLocation(nextPoint), nextPoint.Y);


        Vector3 diff = vCurrent - vPrev;

        bool left = diff.x < 0;
        bool right = diff.x > 0;
        bool forward = diff.z > 0;
        bool backward = diff.z < 0;
        bool up = diff.y > 0;
        bool down = diff.y < 0;


        if (prevExists)
        {
            if (Arena.IsHeightChange(vPrev.y, vCurrent.y))
            {
                if (down)
                {
                    int ibreak = 0;
                }
                int numDiffs = Arena.NumHeightSteps(vPrev.y, vCurrent.y);
                float heightOffset2 = 0f;
                for(int i=0;i<numDiffs;++i)
                {
                    AddQuad(MovementGridManager.AddSquareVertical(Arena, prevPoint, diff, heightOffset2, up, actor, disabled));
                    heightOffset2 += up ? Arena.heightDiff : -Arena.heightDiff;
                }
            }
        }
        float debugHeightOffset = 0.0f;
        bool debugUp = true;
        if (false)
        {
            AddQuad(MovementGridManager.AddSquareVertical(Arena, point, Vector3.forward, debugHeightOffset, debugUp, actor, disabled));
            AddQuad(MovementGridManager.AddSquareVertical(Arena, point, Vector3.back, debugHeightOffset, debugUp, actor, disabled));
            AddQuad(MovementGridManager.AddSquareVertical(Arena, point, Vector3.left, debugHeightOffset, debugUp, actor, disabled));
            AddQuad(MovementGridManager.AddSquareVertical(Arena, point, Vector3.right, debugHeightOffset, debugUp, actor, disabled));

            //debugHeightOffset = 0.5f;
            //m_quadList.Add(MovementGridManager.AddSquareVertical(Arena, point, Vector3.forward, debugHeightOffset, true, actor, disabled));
            //m_quadList.Add(MovementGridManager.AddSquareVertical(Arena, point, Vector3.back, debugHeightOffset, true, actor, disabled));
            //m_quadList.Add(MovementGridManager.AddSquareVertical(Arena, point, Vector3.left, debugHeightOffset, true, actor, disabled));
            //m_quadList.Add(MovementGridManager.AddSquareVertical(Arena, point, Vector3.right, debugHeightOffset, true, actor, disabled));

        }

        AddQuad(MovementGridManager.AddSquare(Arena, prevPoint, point, nextPoint, actor, prevExists, nextExists, disabled));
    }

    private void AddQuad(GameObject go)
    {
        if(go != null)
        {
            m_quadList.Add(go);
        }
    }

    public void Reset()
    {
        foreach (GameObject go in m_quadList)
        {
            MovementGridManager.ReleaseSquare(go);
        }
        m_quadList.Clear();
    }
    private List<GameObject> m_quadList = new List<GameObject>();

}


