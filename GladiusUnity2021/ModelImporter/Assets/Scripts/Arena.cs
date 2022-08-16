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
using System.Net.WebSockets;

public class Arena : MonoBehaviour
{
    public const int ArenaSize = 32;
    public float BlockSize = 2f;

    private int m_propLayer = 0;
    private int m_floorLayer = 0;

    //private Vector3 GridOffset = new Vector3(-1, 0, 1);
    private Vector3 GridOffset = new Vector3(-1, 0, -1);

    private int m_groundLayerMask;

    private void Awake()
    {
        GridOffset = new Vector3(-BlockSize / 2f, 0, -BlockSize / 2f);

        m_groundLayerMask = 1 << LayerMask.NameToLayer("GROUND");

        if (m_pathFinder == null)
        {
            m_pathFinder = new ArenaPathFinder();
            m_pathFinder.Initialize(this);
        }

    }


    public ArenaOffice ArenaOffice
    {
        get;set;
    }

    public GridFile GridFile
    {
        get;set;
    }


    public void SetupData(string scenename)
    {
        String arenaOfficeFileName = scenename + "_leagues";
        ArenaOffice = TownData.LoadOffceData(GladiusGlobals.LeaguesPath + arenaOfficeFileName);
    }

    public const float heightDiff = 0.7f;//1.0f;//0.5f;
    public const float epsilon = 0.1f;//0.01f;


    public static bool AllowableHeightDifference(float a, float b)
    {
        float absDiff = Math.Abs(a - b);
        return absDiff >= 0f - epsilon && absDiff <= heightDiff + absDiff;
    }

    public static bool IsHeightChange(float a, float b)
    {
        float absDiff = Math.Abs(a - b);
        //return (absDiff >= (heightDiff - epsilon)) && (absDiff <= (heightDiff + epsilon));
        return absDiff >= heightDiff;
    }

    public static int NumHeightSteps(float a, float b)
    {
        float absDiff = Math.Abs(a - b);
        return (int)(absDiff / (heightDiff - (heightDiff / 10.0f)));
    }

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

    public bool IsPointOccupied(Point p)
    {
        if (!InLevelBounds(p))
        {
            return true;
        }


        return GridFile != null?GridFile.IsBlocked(p.X, p.Y):false;
    }


    public bool IsBlocked(Point p)
    {
        return GridFile.IsBlocked(p.X, p.Y);
    }

    public bool IsBlocked(int x, int y)
    {
        return GridFile.IsBlocked(x, y);
    }

    public bool ValidPointInLevel(Point p)
    {
        return (InLevelBounds(p) && !IsBlocked(p.X, p.Y));
    }

    public static bool InLevelBounds(Point p)
    {
        return p.X >= 0 && p.X < ArenaSize && p.Y >= 0 && p.Y < ArenaSize;
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

    public void PlaceArenaProp(Prop prop)
    {
        prop.GameObject.transform.localPosition = PropGridPointToWorld(prop);
        int extents = (int)prop.Bounds.extents.x;
        Point center = new Point((int)prop.Position.x, (int)prop.Position.y);
        if (extents == 1)
        {
            prop.ArenaPoints = new Point[] { center };
        }
        else
        {
            prop.ArenaPoints = new Point[] { Point.Add(center,new Point(-1,-1)),center,Point.Add(center,new Point(-1,0)),Point.Add(center,new Point(0,-1))};
        }

        foreach(Point p in prop.ArenaPoints)
        {
            m_levelHeightMap[p] = prop.GameObject.transform.localPosition.y + prop.Bounds.size.y;
        }
        //UnityEngine.Debug.LogFormat("Placing prop at [{0}] height [{1}].", prop.ArenaPoint, prop.Bounds.size.z);
    }



    public void AssertBounds(Point p)
    {
        if (p.X < 0 || p.X >= Width || p.Y < 0 || p.Y >= Breadth)
        {
            System.Diagnostics.Debug.Assert(false);
        }
    }



    public const float NoHeightMatch = -1000.0f;
    public float GetHeightAtLocation(Point point)
    {
        float result = NoHeightMatch;
        int poly = -1;
        // check prop heights first.

        //if(resultChunk== null)
        //{
        //    // Check unity collision.
        //    result =  GetMeshHeight()
        //}

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
        //float result = 0.0f;
        //if (m_levelMap.ContainsKey(point))
        //{
        //    result = m_levelMap[point];
        //}
        //else
        //{
        //    result = GridFile != null ? GridFile.GetHeightAtPoint(point.X, point.Y) : 0.0f;
        //}

        return result;
    }



    public void InitialArenaSetup()
    {
    }

    // props (maybe platforms?) are 2x2 or larger blocks that need placing 
    // so they span the squares rather than sitting in the middle
    public Vector3 PropGridPointToWorld(Prop prop, bool includeHeight = true)
    {
        Point p = new Point((int)prop.Position.x, (int)prop.Position.y);
        return GridPointToWorld(p, includeHeight);
    }

    public Vector3 GridPointToWorld(Point p, bool includeHeight = true)
    {
        float groundHeight = includeHeight ? GetHeightAtLocation(p) : 0.0f; ;

        // adjust for centered grid at 0,0
        p = new Point(p.X - 16, p.Y - 16);



        if (groundHeight > 0)
        {
            int ibreak = 0;
        }

        Vector3 result = new Vector3(p.X, 0, p.Y);

        result *= BlockSize;
        result.y = groundHeight;

        // place it in the middle of the square.
        result += new Vector3(BlockSize / 2.0f, 0, BlockSize / 2.0f);

        // adjust it to where the areana is placed?
        result += transform.position;

        result += GridOffset;

        return result;
    }

    public bool RayToGridPoint(Ray ray ,out Point p)
    {
        bool inbounds = false;
        p = new Point();

        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit,1000,m_groundLayerMask))
        {
            Vector3 localPos = hit.point - transform.position;
            localPos -= GridOffset;

            inbounds = true;
            int halfExtent = (int)(ArenaSize * BlockSize / 2);

            float xlerp = Mathf.InverseLerp(-halfExtent, halfExtent, localPos.x);
            float zlerp = Mathf.InverseLerp(-halfExtent, halfExtent, localPos.z);

            float xval = (xlerp * ArenaSize);
            float zval = (zlerp * ArenaSize);

            p = new Point((int)xval, (int)zval);
        }
        return inbounds;
    }



    public bool GetRandomEmptySquare(out Point result,int range = 32)
    {
        int numAttempts = 5;
        for (int i = 0; i < numAttempts; ++i)
        {
            int randomX = GladiusGlobals.Random.Next(0, ArenaSize - 1);
            int randomY = GladiusGlobals.Random.Next(0, ArenaSize - 1);
            // range broken at the moment
            Point p = new Point(randomX, randomY);
            if(!IsPointOccupied(p))
            {
                result = p;
                return true;
            }
        }
        result = GladiusGlobals.InvalidPoint;
        return false;
    }


    private List<Point> m_tempList = new List<Point>();
    public bool DoesPathExist(Point start, Point end)
    {
        m_tempList.Clear();
        if(IsPointOccupied(end))
        {
            return false;
        }
        return FindPath(start, end, m_tempList);
    }

    public bool FindPath(Point start, Point end, List<Point> result)
    {
        if(!(InLevelBounds(start) && InLevelBounds(end)))
        {
            return false;
        } 
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




    //public ArenaActor NextToEnemy(ArenaActor source, bool orthogonal = true)
    //{
    //    foreach (Point p2 in orthogonal ? GladiusGlobals.CardinalPoints : GladiusGlobals.SurroundingPoints)
    //    {
    //        Point adjusted = Point.Add(source.ArenaPoint, p2);
    //        if (InLevelBounds(adjusted))
    //        {
    //            ArenaActor ba = null;
    //            if (m_pointBaseActorMap.TryGetValue(adjusted, out ba))
    //            {
    //                if (ba.TeamName != source.TeamName)
    //                {
    //                    return ba;
    //                }
    //            }
    //        }
    //    }
    //    return null;
    //}



    public bool IsNextTo(Point p1, Point p2)
    {
        int xdiff = Math.Abs(p1.X - p2.X);
        int ydiff = Math.Abs(p1.Y - p2.Y);
        return (xdiff == 1 && ydiff == 0) || (xdiff == 0 && ydiff == 1);

    }

    public void SetupPropData(TextAsset propFile)
    {
        if(propFile != null)
        { 
            List<Prop> props = PropLoader.Load(propFile.text);
            if (props != null)
            {
                foreach (Prop prop in props)
                {
                    string objectPath = GladiusGlobals.ModelsRoot + prop.Model;
                    GameObject propGO = GladiusGlobals.InstantiateModel(objectPath);
                    if (propGO != null)
                    {
                        prop.GameObject = propGO;
                        prop.GameObject.isStatic = true;
                        //prop.GameObject.layer = LayerMask.NameToLayer("PROP");
                        prop.GameObject.name = "Prop-" + prop.Position.x + "-" + prop.Position.y;
                        prop.GameObject.transform.localRotation = GladiusGlobals.CharacterLocalRotation;
                        BoxCollider bc = prop.GameObject.AddComponent<BoxCollider>();
                        bc.center = prop.Bounds.center;
                        bc.size = prop.Bounds.size;

                        PlaceArenaProp(prop);
                    }
                }
            }
        }
    }



    private Dictionary<Point, float> m_levelHeightMap = new Dictionary<Point, float>();

    private ArenaPathFinder m_pathFinder;

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

            if (Arena.InLevelBounds(adjusted) && validOccupied)
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
