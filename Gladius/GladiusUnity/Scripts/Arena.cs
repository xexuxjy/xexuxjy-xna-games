using UnityEngine;
using System.Collections;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Gladius;

using System.IO;
using System.Xml;



public class Arena : MonoBehaviour
{
    public const int ArenaSize = 32;
    public const float ArenaMeshScalar = 2f;
    //public const float ArenaMeshScalar = 1f;
    public CombatEngine CombatEngine;

    public int m_propLayer = 0;
    public int m_floorLayer = 0;


    public void SetupData()
    {
        m_pathFinder = new ArenaPathFinder();
        m_pathFinder.Initialize(this);
        m_propLayer = LayerMask.NameToLayer("Prop");
        m_floorLayer = LayerMask.NameToLayer("Floor");
        //BuildDefaultArena();
    }

    public const float heightDiff = 1.0f;//0.5f;
    public const float epsilon = 0.1f;//0.01f;
    //static float heightDiff = 1.0f;
    //static float epsilon = 0.10f;

    public Vector3 GetArenaPoint(Point point)
    {
        Vector3 topLeft = new Vector3(Arena.ArenaSize, 0, Arena.ArenaSize) * Arena.ArenaMeshScalar;
        topLeft *= -0.5f;
        Vector3 v3Point = new Vector3(point.X, 0, point.Y);
        topLeft += (v3Point * Arena.ArenaMeshScalar);
        // center
        topLeft.x += (Arena.ArenaMeshScalar / 2.0f);
        topLeft.z += (Arena.ArenaMeshScalar / 2.0f);

        topLeft.y = GetHeightAtLocation(point);
        return topLeft;
    }


    public static bool AllowableHeightDifference(float a, float b)
    {
        float absDiff = Math.Abs(a - b);
        return absDiff >= 0f - epsilon && absDiff <= heightDiff + absDiff;
    }

    public static bool IsHeightChange(float a, float b)
    {
        float absDiff = Math.Abs(a - b);
        return (absDiff >= (heightDiff - epsilon)) && (absDiff <= (heightDiff + epsilon));
    }

    public static int NumHeightSteps(float a, float b)
    {
        float absDiff = Math.Abs(a - b);
        return (int)(absDiff / (heightDiff-(heightDiff/10.0f)));
    }

    //public void InitialisePathFinding()
    //{
    //    m_pathFinder = new ArenaPathFinder();
    //    m_pathFinder.Initialize(this);
    //}


    public bool CheckValidPath(List<Point> points)
    {
        foreach (Point p in points)
        {
            if (IsPointOccupied(p))
            {
                return false;
            }
            if (!InLevelBounds(p))
            {
                return false;
            }
        }
        return true;
    }

    //public SquareType AtPoint(Point p)
    //{
    //    return GridFile.ArenaData[p.X, p.Y];
    //}

    public bool IsPointOccupied(Point p)
    {
        if (!InLevelBounds(p))
        {
            return true;
        }

        BaseActor ba = null;
        if (m_baseActorMap.TryGetValue(p, out ba))
        {
            return true;
        }

        return GridFile.IsBlocked(p.X, p.Y);
        //SquareType squareType = AtPoint(p);
        //return squareType != SquareType.Empty;
    }

    public float GetLevel(int x, int y)
    {
        float level = 0.0f;
        m_levelMap.TryGetValue(new Point(x, y), out level);
        return level;
    }

    public bool IsBlocked(int x,int y)
    {
        return GridFile.IsBlocked(x,y);
    }

    public bool ValidPointInLevel(Point p)
    {
        return (InLevelBounds(p) && !GridFile.IsBlocked(p.X, p.Y));
    }

    public bool InLevelBounds(Point p)
    {
        return p.X >= 0 && p.X < Width && p.Y >= 0 && p.Y < Breadth;
    }


    public bool InLevel(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Breadth;
    }

    public int Width
    {
        get
        {
            return ArenaSize;
        }
    }
    public int Breadth
    {
        get
        {
            return ArenaSize;
        }
    }

    public String ModelName
    {
        get;
        set;
    }

    public String SkyBoxMaterialName
    {
        get;
        set;

    }

    //public void SetActorAtPosition(int x, int y, BaseActor actor)
    //{
    //    m_arenaGrid[x, y] = SquareType.Mobile;

    //}

    public void PlaceProp(Point p , Bounds b,GameObject gameObject)
    {
        gameObject.transform.localPosition = PropGridPointToWorld(p, b);
        m_levelMap[p] = b.size.z;
        UnityEngine.Debug.LogFormat("Placing prop at [{0}] height [{1}].", p, b.size.z);
    }

    public void RemoveActor(BaseActor actor)
    {
        Point p = actor.ArenaPoint;
        m_baseActorMap.Remove(p);
        //m_arenaGrid[p.X, p.Y] = SquareType.Empty;
    }


    public bool CanMoveActor(BaseActor baseActor, Point newPoint)
    {
        // deal with moving to same position
        if (GetActorAtPosition(newPoint) == baseActor)
        {
            return true;
        }
        //return m_arenaGrid[newPoint.X, newPoint.Y] == SquareType.Empty;
        return !IsPointOccupied(newPoint);
    }

    // move actor - assumes any validity checks have already taken place.
    public void MoveActor(BaseActor baseActor, Point newPoint)
    {
        System.Diagnostics.Debug.Assert(CanMoveActor(baseActor, newPoint));
        if (CanMoveActor(baseActor, newPoint))
        {
            // set current actor square to empty
            //baseActor
            //m_arenaGrid[baseActor.CurrentPosition.X, baseActor.CurrentPosition.Y] = SquareType.Empty;
            if (m_baseActorMap.ContainsKey(baseActor.ArenaPoint))
            {
                m_baseActorMap.Remove(baseActor.ArenaPoint);
            }
            //m_arenaGrid[newPoint.X, newPoint.Y] = SquareType.Mobile;
            baseActor.ArenaPoint = newPoint;
            m_baseActorMap[baseActor.ArenaPoint] = baseActor;
        }
    }

    public void AssertBounds(Point p)
    {
        if (p.X < 0 || p.X >= Width || p.Y < 0 || p.Y >= Breadth)
        {
            System.Diagnostics.Debug.Assert(false);
        }
    }

    public BaseActor GetActorAtPosition(Point p)
    {
        BaseActor result = null;
        if (InLevelBounds(p))
        {
            m_baseActorMap.TryGetValue(p, out result);
        }
        return result;
    }

    public float GetMeshHeight(Vector3 worldPos)
    {
        Ray ray = new Ray(worldPos + Vector3.up, Vector3.down);
        RaycastHit hitResult;
        Physics.Raycast(ray, out hitResult, 0);
        if (hitResult.collider != null)
        {
            return hitResult.point.y;
        }
        return 0f;
    }

    public const float NoHeightMatch = -1000.0f;
    public float GetHeightAtLocation(Point point)
    {
        //float result = NoHeightMatch;
        //RaycastHit raycastHit;
        //int layerMask = m_propLayer | m_floorLayer;
        //Vector3 worldpos = GridPointToWorld(point, false);
        //float rayLength = 10;
        //Vector3 rayStart = new Vector3(worldpos.x, rayLength, worldpos.z);
        //Vector3 rayDirection = Vector3.down;

        ////if (Physics.Raycast(rayStart, rayDirection, out raycastHit, rayLength, layerMask))
        //if (Physics.Raycast(rayStart, rayDirection, out raycastHit))
        //{
        //    result = raycastHit.point.y;
        //}

        //return result;
        // check and see if we have a platform prop.
        float result = 0.0f;
        if (m_levelMap.ContainsKey(point))
        {
            result = m_levelMap[point];
        }
        else
        {
            result = GridFile.GetHeightAtPoint(point.X, point.Y);
        }
        return result;
    }

    public Dictionary<Point, BaseActor> PointActorMap
    {
        get
        {
            return m_baseActorMap;
        }
    }


    public void InitialArenaSetup()
    {
    }

    // props (maybe platforms?) are 2x2 or larger blocks that need placing 
    // so they span the squares rather than sitting in the middle
    public Vector3 PropGridPointToWorld(Point p, Bounds b,bool includeHeight = true)
    {
        float groundHeight = includeHeight ? GetHeightAtLocation(p) : 0.0f; ;
        groundHeight = 0f;
        if (groundHeight > 0)
        {
            int ibreak = 0;
        }

        Vector3 topLeft = new Vector3(-Width / 2f, 0, -Breadth / 2f) * ArenaMeshScalar;
        Vector3 propPosition = new Vector3(p.X, 0, p.Y) * ArenaMeshScalar;

        Vector3 result = topLeft + propPosition;

        result.y = groundHeight;

        // place it in the middle of the square.
        //result -= new Vector3(b.size.x , 0, b.size.z );
        result += new Vector3(1, 0, 1);
        //result += new Vector3(ArenaMeshScalar, 0, ArenaMeshScalar);

        // adjust it to where the areana is placed?
        result += transform.position;

        return result;
    }

    public Vector3 GridPointToWorld(Point p, bool includeHeight = true)
    {
        float groundHeight = includeHeight ? GetHeightAtLocation(p) : 0.0f; ;
        if (groundHeight > 0)
        {
            int ibreak = 0;
        }

        Vector3 topLeft = new Vector3(-Width / 2f, 0, -Breadth / 2f);
        Vector3 result = topLeft + new Vector3(p.X, 0, p.Y);

        result *= ArenaMeshScalar;
        result.y = groundHeight;

        // place it in the middle of the square.
        result += new Vector3(ArenaMeshScalar / 2.0f, 0, ArenaMeshScalar / 2.0f);
        //result += new Vector3(ArenaMeshScalar, 0, ArenaMeshScalar);

        // adjust it to where the areana is placed?
        result += transform.position;

        return result;
    }

    public bool GetRandomEmptySquare(out Point result)
    {
        int numAttempts = 5;
        for (int i = 0; i < numAttempts; ++i)
        {
            int x = m_rng.Next(Width);
            int y = m_rng.Next(Breadth);
            Point p = new Point(x, y);
            BaseActor ba;
            if (!m_baseActorMap.TryGetValue(p, out ba))
            {
                result = p;
                return true;
            }
        }
        result = Point.Zero;
        return false;
    }


    //public Vector3 Position
    //{
    //    get;
    //    set;
    //}

    private List<Point> m_tempList = new List<Point>();
    public bool DoesPathExist(Point start, Point end)
    {
        m_tempList.Clear();
        return FindPath(start, end, m_tempList);
    }

    public bool FindPath(Point start, Point end, List<Point> result)
    {
        return m_pathFinder.FindPath(start, end, result);
    }


    private static void Swap<T>(ref T lhs, ref T rhs)
    {
        T temp;
        temp = lhs;
        lhs = rhs;
        rhs = temp;
    }

    public bool HasLineOfSight(Point start, Point end)
    {
        return HasLineOfSight(start.X, start.Y, end.X, end.Y);
    }

    public bool HasLineOfSight(int x0, int y0, int x1, int y1)
    {
        bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
        if (steep)
        {
            Swap<int>(ref x0, ref y0);
            Swap<int>(ref x1, ref y1);
        }
        if (x0 > x1)
        {
            Swap<int>(ref x0, ref x1);
            Swap<int>(ref y0, ref y1);
        }

        var dX = (x1 - x0);
        var dY = (y1 - y0);
        var err = (dX / 2);
        var ystep = (y0 < y1 ? 1 : -1);
        var y = y0;

        for (var x = x0; x <= x1; ++x)
        {
            // it's blocked so no LOS....
            if (IsPointOccupied(new Point(x, y)))
            {
                return false;
            }
            //if (!(steep ? plot(y, x) : plot(x, y)))
            //    return;

            err = err - dY;
            if (err < 0)
            {
                y += ystep;
                err += dX;
            }

        }
        // got here so we should have LOS
        return true;
    }


    public void BuildPath(Point start, Point end, List<Point> points, List<bool> occupied)
    {
        points.Clear();
        occupied.Clear();
        int x0 = start.X;
        int y0 = start.Y;
        int x1 = end.X;
        int y1 = end.Y;

        bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
        if (steep)
        {
            Swap<int>(ref x0, ref y0);
            Swap<int>(ref x1, ref y1);
        }
        if (x0 > x1)
        {
            Swap<int>(ref x0, ref x1);
            Swap<int>(ref y0, ref y1);
        }

        var dX = (x1 - x0);
        var dY = (y1 - y0);
        var err = (dX / 2);
        var ystep = (y0 < y1 ? 1 : -1);
        var y = y0;

        for (var x = x0; x <= x1; ++x)
        {
            Point p = new Point(x, y);
            points.Add(p);
            // it's blocked so no LOS....
            occupied.Add(IsPointOccupied(p));
            //if (!(steep ? plot(y, x) : plot(x, y)))
            //    return;

            err = err - dY;
            if (err < 0)
            {
                y += ystep;
                err += dX;
            }

        }


    }



    // dumb version for now. doesn't check path
    public BaseActor FindNearestEnemy(BaseActor searcher)
    {
        float closest = float.MaxValue;
        BaseActor closestActor = null;
        foreach (BaseActor enemy in m_baseActorMap.Values)
        {
            if (CombatEngine != null && CombatEngine.IsValidTarget(searcher, enemy, searcher.CurrentAttackSkill))
            {
                float dist = (enemy.Position - searcher.Position).sqrMagnitude;
                if (dist < closest)
                {
                    closestActor = enemy;
                }
            }

        }
        return closestActor;
    }

    public BaseActor PointNextToEnemy(BaseActor source, Point point, bool orthogonal = true)
    {
        foreach (Point p2 in orthogonal ? GladiusGlobals.CardinalPoints : GladiusGlobals.SurroundingPoints)
        {
            Point adjusted = Point.Add(point, p2);
            if (InLevelBounds(adjusted))
            {
                BaseActor ba = null;
                if (m_baseActorMap.TryGetValue(adjusted, out ba))
                {
                    if (ba.TeamName != source.TeamName)
                    {
                        return ba;
                    }
                }
            }
        }
        return null;
    }

    public BaseActor NextToEnemy(BaseActor source, bool orthogonal = true)
    {
        foreach (Point p2 in orthogonal ? GladiusGlobals.CardinalPoints : GladiusGlobals.SurroundingPoints)
        {
            Point adjusted = Point.Add(source.ArenaPoint, p2);
            if (InLevelBounds(adjusted))
            {
                BaseActor ba = null;
                if (m_baseActorMap.TryGetValue(adjusted, out ba))
                {
                    if (ba.TeamName != source.TeamName)
                    {
                        return ba;
                    }
                }
            }
        }
        return null;
    }


    public BaseActor NextToEnemy(BaseActor source, BaseActor target,bool orthogonal = true)
    {
        foreach (Point p2 in orthogonal ? GladiusGlobals.CardinalPoints : GladiusGlobals.SurroundingPoints)
        {
            Point adjusted = Point.Add(source.ArenaPoint, p2);
            if (InLevelBounds(adjusted))
            {
                BaseActor ba = null;
                if (m_baseActorMap.TryGetValue(adjusted, out ba))
                {
                    if (ba == target)
                    {
                        return ba;
                    }
                }
            }
        }
        return null;
    }



    public Point PointNearestLocation(Point startLocation, Point location, bool includeSquare = true)
    {
        if (includeSquare)
        {
            if (!IsPointOccupied(location))
            {
                return location;
            }
        }
        int closest = int.MaxValue;
        Point closestPoint = Point.Zero;
        foreach (Point p in GladiusGlobals.CardinalPoints)
        {
            Point adjusted = Point.Add(location, p);
            if (InLevelBounds(adjusted) && !IsPointOccupied(adjusted))
            {
                int dist = GladiusGlobals.PointDist2(adjusted, startLocation);
                if (dist < closest)
                {
                    closest = dist;
                    closestPoint = adjusted;
                }
            }
        }
        return closestPoint;
    }

    public bool IsNextTo(Point p1, Point p2)
    {
        int xdiff = Math.Abs(p1.X - p2.X);
        int ydiff = Math.Abs(p1.Y - p2.Y);
        return (xdiff == 1 && ydiff == 0) || (xdiff == 0 && ydiff == 1);

    }


    public void SetupScenery()
    {
        //m_modelObject = new GameObject();

        //Mesh m = Resources.Load<Mesh>(ModelName);
        //MeshFilter mf = m_modelObject.AddComponent<MeshFilter>();
        //mf.mesh = m;
        //ModelScale = new 
        m_modelObject = (GameObject)Instantiate(Resources.Load<GameObject>(ModelName));
        if (m_modelObject != null)
        {
            m_modelObject.name = "ArenaScenery";
            m_modelObject.layer = LayerMask.NameToLayer("Floor");
            m_modelObject.isStatic = true;
            m_modelObject.transform.position = ModelPosition;
            m_modelObject.transform.localScale = ModelScale;
            Quaternion rotation = Quaternion.Euler(ModelRotation);
            m_modelObject.transform.rotation = rotation;
            //m_modelObject.active = true;

            //m_modelObject.get
            //MeshFilter[] meshes = m_modelObject.GetComponents<MeshFilter>();

            MeshCollider[] colliders = m_modelObject.GetComponentsInChildren<MeshCollider>();
            foreach (MeshCollider collider in colliders)
            {
                collider.tag = "environment";
            }


            foreach (Transform t in m_modelObject.transform)
            {
                t.gameObject.layer = m_modelObject.layer;
            }
        }
        Material m = Resources.Load<Material>(SkyBoxMaterialName);
        RenderSettings.skybox = m;

    }

    public void AddGridFile(GridFile gridFile)
    {
        m_gridFileList.Add(gridFile);
    }

    public void NextGridFile()
    {
        if (m_gridFileList.Count > 1)
        {
            m_currentGridFileIndex = (m_currentGridFileIndex + 1) % m_gridFileList.Count;
            //RebuildDebugGrid();
        }
    }

    public void PrevGridFile()
    {
        if (m_gridFileList.Count > 1)
        {
            m_currentGridFileIndex = (m_currentGridFileIndex - 1);
            if (m_currentGridFileIndex < 0)
            {
                m_currentGridFileIndex += m_gridFileList.Count;
            }
            //RebuildDebugGrid();
        }
    }

    public void RestoreDefaultGridFile()
    {
        m_currentGridFileIndex = 0;
        //CurrentGridFile = Arena.GridFile;
        //RebuildDebugGrid();
    }

    public void LoadAllGridFiles()
    {
        //var info = new DirectoryInfo("GladiusData/ArenaData/GridFiles");
        //var fileInfo = info.GetFiles();
        //for (file in fileInfo) print(file);

        TextAsset[] gridFiles = Resources.LoadAll<TextAsset>("GladiusData/ArenaData/GridFiles");
        foreach (TextAsset gfo in gridFiles)
        {
            GridFile gridFile = GridFileReader.ReadFile("", gfo);
            m_gridFileList.Add(gridFile);
        }
    }

    public bool m_gridXFlipped = false;
    public void FlipX()
    {
        m_gridXFlipped = !m_gridXFlipped;
        //RebuildDebugGrid();
    }

    public bool m_gridYFlipped = false;

    public void FlipY()
    {
        m_gridYFlipped = !m_gridYFlipped;
        //RebuildDebugGrid();
    }


    private List<GridFile> m_gridFileList = new List<GridFile>();
    private int m_currentGridFileIndex = 0;

    private GridFile GridFile
    {
        get
        { return m_gridFileList[m_currentGridFileIndex]; }
    }



    public List<Point> PlayerPointList = new List<Point>();
    public List<Point> Team1PointList = new List<Point>();
    public List<Point> Team2PointList = new List<Point>();
    public List<Point> Team3PointList = new List<Point>();


    private GameObject m_modelObject;
    public Vector3 ModelRotation;
    public Vector3 ModelScale;
    public Vector3 ModelPosition;

    private Texture2D m_debugLayoutTexture;
    private GameObject m_debugQuad;

    private Dictionary<Point, BaseActor> m_baseActorMap = new Dictionary<Point, BaseActor>();
    private Dictionary<Point, float> m_levelMap = new Dictionary<Point, float>();

    private ArenaPathFinder m_pathFinder;
    private System.Random m_rng = new System.Random();
}


struct SearchNode
{
    /// <summary>
    /// Location on the map
    /// </summary>
    public Point Position;

    /// <summary>
    /// Distance to goal estimate
    /// </summary>
    public float DistanceToGoal;

    /// <summary>
    /// Distance traveled from the start
    /// </summary>
    public float DistanceTraveled;

    public SearchNode(
        Point mapPosition, float distanceToGoal, float distanceTraveled)
    {
        Position = mapPosition;
        DistanceToGoal = distanceToGoal;
        DistanceTraveled = distanceTraveled;
    }
}


public class ArenaPathFinder
{
    // How much time has passed since the last search step
    private float timeSinceLastSearchStep = 0f;
    // Holds search nodes that are avaliable to search
    private List<SearchNode> openList;
    // Holds the nodes that have already been searched
    private List<SearchNode> closedList;
    // Holds all the paths we've creted so far
    private Dictionary<Point, Point> paths;
    // The map we're searching
    private Arena m_levelMap;
    // Seconds per search step        
    public float timeStep = .5f;

    public Point startPoint = new Point();
    public Point endPoint = new Point();

    public List<Point> stepPoints = new List<Point>(4);


    #region Properties

    // Tells us if the search is stopped, started, finished or failed
    public SearchStatus SearchStatus
    {
        get { return searchStatus; }
    }
    private SearchStatus searchStatus;

    // Tells us which search type we're using right now
    public SearchMethod SearchMethod
    {
        get { return searchMethod; }
    }
    //private SearchMethod searchMethod = SearchMethod.BestFirst;
    private SearchMethod searchMethod = SearchMethod.BreadthFirst;

    // Seconds per search step
    public float TimeStep
    {
        get { return timeStep; }
        set { timeStep = value; }
    }

    /// <summary>
    /// Toggles searching on and off
    /// </summary>
    public bool IsSearching
    {
        get { return searchStatus == SearchStatus.Searching; }
        set
        {
            if (searchStatus == SearchStatus.Searching)
            {
                searchStatus = SearchStatus.Stopped;
            }
            else if (searchStatus == SearchStatus.Stopped)
            {
                searchStatus = SearchStatus.Searching;
            }
        }
    }

    /// <summary>
    /// How many search steps have elapsed on this map
    /// </summary>
    public int TotalSearchSteps
    {
        get { return totalSearchSteps; }
    }
    private int totalSearchSteps = 0;

    #endregion




    #region Initialization

    /// <summary>
    /// Setup search
    /// </summary>
    /// <param name="levelMap">Map to search</param>
    public void Initialize(Arena levelMap)
    {
        searchStatus = SearchStatus.Stopped;
        openList = new List<SearchNode>();
        closedList = new List<SearchNode>();
        paths = new Dictionary<Point, Point>();
        m_levelMap = levelMap;
    }

    /// <summary>
    /// Load the Draw texture
    /// </summary>
    //public void LoadContent(ContentManager content)
    //{
    //    nodeTexture = content.Load<Texture2D>("dot");
    //    nodeTextureCenter =
    //        new Vector2(nodeTexture.Width / 2, nodeTexture.Height / 2);
    //}

    #endregion

    #region Update and Draw

    /// <summary>
    /// Search Update
    /// </summary>
    public void Update()
    {
        if (searchStatus == SearchStatus.Searching)
        {
            timeSinceLastSearchStep += Time.deltaTime;
            if (timeSinceLastSearchStep >= timeStep)
            {
                DoSearchStep();
                timeSinceLastSearchStep = 0f;
            }
        }
    }

    /// <summary>
    /// Draw the search space
    /// </summary>
    //public void Draw(SpriteBatch spriteBatch)
    //{
    //    if (searchStatus != SearchStatus.PathFound)
    //    {
    //        spriteBatch.Begin();
    //        foreach (SearchNode node in openList)
    //        {
    //            spriteBatch.Draw(nodeTexture, 
    //                map.MapToWorld(node.Position, true), null, openColor, 0f,
    //                nodeTextureCenter, scale, SpriteEffects.None, 0f);
    //        }
    //        foreach (SearchNode node in closedList)
    //        {
    //            spriteBatch.Draw(nodeTexture, 
    //                map.MapToWorld(node.Position, true), null, closedColor, 0f,
    //                nodeTextureCenter, scale, SpriteEffects.None, 0f);
    //        }
    //        spriteBatch.End();
    //    }
    //}

    #endregion

    #region Methods

    /// <summary>
    /// Reset the search
    /// </summary>
    public void Reset()
    {
        searchStatus = SearchStatus.Stopped;
        totalSearchSteps = 0;
        openList.Clear();
        closedList.Clear();
        paths.Clear();
    }

    /// <summary>
    /// Cycle through the search method to the next type
    /// </summary>
    public void NextSearchType()
    {
        searchMethod = (SearchMethod)(((int)searchMethod + 1) %
            (int)SearchMethod.Max);
    }

    /// <summary>
    /// This method find the next path node to visit, puts that node on the 
    /// closed list and adds any nodes adjacent to the visited node to the 
    /// open list.
    /// </summary>
    private void DoSearchStep()
    {
        SearchNode newOpenListNode;

        bool foundNewNode = SelectNodeToVisit(out newOpenListNode);
        if (foundNewNode)
        {
            Point currentPos = newOpenListNode.Position;
            stepPoints.Clear();
            OpenMapTiles(currentPos, stepPoints);
            foreach (Point point in stepPoints)
            {
                SearchNode mapTile = new SearchNode(point,
                    StepDistanceToEnd(point),
                    newOpenListNode.DistanceTraveled + 1);
                if (!InList(openList, point) &&
                    !InList(closedList, point))
                {
                    openList.Add(mapTile);
                    paths[point] = newOpenListNode.Position;
                }
            }
            if (currentPos == endPoint)
            {
                searchStatus = SearchStatus.PathFound;
            }
            openList.Remove(newOpenListNode);
            closedList.Add(newOpenListNode);
        }
        else
        {
            searchStatus = SearchStatus.NoPath;
        }
    }

    /// <summary>
    /// Determines if the given Point is inside the SearchNode list given
    /// </summary>
    private static bool InList(List<SearchNode> list, Point point)
    {
        bool inList = false;
        foreach (SearchNode node in list)
        {
            if (node.Position == point)
            {
                inList = true;
            }
        }
        return inList;
    }

    /// <summary>
    /// This Method looks at everything in the open list and chooses the next 
    /// path to visit based on which search type is currently selected.
    /// </summary>
    /// <param name="result">The node to be visited</param>
    /// <returns>Whether or not SelectNodeToVisit found a node to examine
    /// </returns>
    private bool SelectNodeToVisit(out SearchNode result)
    {
        result = new SearchNode();
        bool success = false;
        float smallestDistance = float.PositiveInfinity;
        float currentDistance = 0f;
        if (openList.Count > 0)
        {
            switch (searchMethod)
            {
                // Breadth first search looks at every possible path in the 
                // order that we see them in.
                case SearchMethod.BreadthFirst:
                    totalSearchSteps++;
                    result = openList[0];
                    success = true;
                    break;
                // Best first search always looks at whatever path is closest to
                // the goal regardless of how long that path is.
                case SearchMethod.BestFirst:
                    totalSearchSteps++;
                    foreach (SearchNode node in openList)
                    {
                        currentDistance = node.DistanceToGoal;
                        if (currentDistance < smallestDistance)
                        {
                            success = true;
                            result = node;
                            smallestDistance = currentDistance;
                        }
                    }
                    break;
                // A* search uses a heuristic, an estimate, to try to find the 
                // best path to take. As long as the heuristic is admissible, 
                // meaning that it never over-estimates, it will always find 
                // the best path.
                case SearchMethod.AStar:
                    totalSearchSteps++;
                    foreach (SearchNode node in openList)
                    {
                        currentDistance = Heuristic(node);
                        // The heuristic value gives us our optimistic estimate 
                        // for the path length, while any path with the same 
                        // heuristic value is equally ‘good’ in this case we’re 
                        // favoring paths that have the same heuristic value 
                        // but are longer.
                        if (currentDistance <= smallestDistance)
                        {
                            if (currentDistance < smallestDistance)
                            {
                                success = true;
                                result = node;
                                smallestDistance = currentDistance;
                            }
                            else if (currentDistance == smallestDistance &&
                                node.DistanceTraveled > result.DistanceTraveled)
                            {
                                success = true;
                                result = node;
                                smallestDistance = currentDistance;
                            }
                        }
                    }
                    break;
            }
        }
        return success;
    }

    /// <summary>
    /// Generates an optimistic estimate of the total path length to the goal 
    /// from the given position.
    /// </summary>
    /// <param name="location">Location to examine</param>
    /// <returns>Path length estimate</returns>
    private static float Heuristic(SearchNode location)
    {
        return location.DistanceTraveled + location.DistanceToGoal;
    }

    /// <summary>
    /// Generates the path from start to end.
    /// </summary>
    /// <returns>The path from start to end</returns>
    public LinkedList<Point> FinalPath()
    {
        LinkedList<Point> path = new LinkedList<Point>();
        if (searchStatus == SearchStatus.PathFound)
        {
            Point curPrev = endPoint;
            path.AddFirst(curPrev);
            while (paths.ContainsKey(curPrev))
            {
                curPrev = paths[curPrev];
                path.AddFirst(curPrev);
            }
        }
        return path;
    }

    public static float StepDistance(Point pointA, Point pointB)
    {
        float distanceX = Math.Abs(pointA.X - pointB.X);
        float distanceY = Math.Abs(pointA.Y - pointB.Y);

        return distanceX + distanceY;
    }

    public float StepDistanceToEnd(Point point)
    {
        return StepDistance(point, endPoint);
    }


    public bool FindPath(Point start, Point end, List<Point> result)
    {
        if (start == end)
        {
            return true;
        }
        Reset();
        startPoint = start;
        endPoint = end;

        openList.Add(new SearchNode(start, StepDistance(start, end), 0));
        IsSearching = true;
        while (IsSearching)
        {
            DoSearchStep();
        }


        if (searchStatus == SearchStatus.PathFound)
        {
            Point curPrev = endPoint;
            result.Add(curPrev);
            while (paths.ContainsKey(curPrev))
            {
                curPrev = paths[curPrev];
                // copied from link list style. hmm.
                result.Insert(0, curPrev);
            }
        }

        return searchStatus == SearchStatus.PathFound;
    }

    public void OpenMapTiles(Point center, List<Point> results)
    {
        int x = (int)center.X;
        int y = (int)center.Y + 1;
        object o = null;

        float currentHeight = m_levelMap.GetHeightAtLocation(center);

        foreach (Point p in GladiusGlobals.CardinalPoints)
        {
            Point adjusted = Point.Add(center, p);

            // if the square is empty, or it's occupied but it's the end point...
            bool validOccupied = !m_levelMap.IsPointOccupied(adjusted) || adjusted == endPoint;

            if (m_levelMap.InLevelBounds(adjusted) && validOccupied)
            {
                // check for height differnces.
                if (Arena.AllowableHeightDifference(m_levelMap.GetHeightAtLocation(adjusted), currentHeight))
                {
                    results.Add(adjusted);
                }
            }
        }
    }


    #endregion

}



#region Search Status Enum
public enum SearchStatus
{
    Stopped,
    Searching,
    NoPath,
    PathFound,
}
#endregion

#region Search Method Enum
public enum SearchMethod
{
    BreadthFirst,
    BestFirst,
    AStar,
    Max,
}
#endregion

//}
