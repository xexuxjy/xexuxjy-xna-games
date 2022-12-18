using System.Drawing;
using System.Numerics;


public interface IMapData
{
    public bool CanMove(Vector2 from,Vector2 to);
    public Vector2[] GetDirections();
    public Vector2 GetTargetPosition();

    public float DistanceToTarget(Vector2 v);
}

public class AStar
{
    // How much time has passed since the last search step
    private float timeSinceLastSearchStep = 0f;
    // Holds search nodes that are avaliable to search
    private List<SearchNode> openList;
    // Holds the nodes that have already been searched
    private List<SearchNode> closedList;
    // Holds all the paths we've creted so far
    private Dictionary<Vector2, Vector2> paths;
    // The map we're searching
    // Seconds per search step        
    public float timeStep = .5f;

    public Vector2 startVector2 = new Vector2();
    public Vector2 endVector2 = new Vector2();

    public List<Vector2> stepVector2s = new List<Vector2>(4);

    private IMapData m_levelMap;

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
    public void Initialize(IMapData levelMap)
    {
        searchStatus = SearchStatus.Stopped;
        openList = new List<SearchNode>();
        closedList = new List<SearchNode>();
        paths = new Dictionary<Vector2, Vector2>();
        m_levelMap = levelMap;
    }

    #endregion

    #region Update and Draw


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
            Vector2 currentPos = newOpenListNode.Position;
            stepVector2s.Clear();
            OpenMapTiles(currentPos, stepVector2s);
            foreach (Vector2 v in stepVector2s)
            {
                SearchNode mapTile = new SearchNode(v,m_levelMap.DistanceToTarget(v), newOpenListNode.DistanceTraveled + 1);
                if (!InList(openList, v) && !InList(closedList, v))
                {
                    openList.Add(mapTile);
                    paths[v] = newOpenListNode.Position;
                }
            }
            if (currentPos == endVector2)
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
    /// Determines if the given Vector2 is inside the SearchNode list given
    /// </summary>
    private static bool InList(List<SearchNode> list, Vector2 Vector2)
    {
        bool inList = false;
        foreach (SearchNode node in list)
        {
            if (node.Position == Vector2)
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
                        // heuristic value is equally â€˜goodâ€™ in this case weâ€™re 
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
    public LinkedList<Vector2> FinalPath()
    {
        LinkedList<Vector2> path = new LinkedList<Vector2>();
        if (searchStatus == SearchStatus.PathFound)
        {
            Vector2 curPrev = endVector2;
            path.AddFirst(curPrev);
            while (paths.ContainsKey(curPrev))
            {
                curPrev = paths[curPrev];
                path.AddFirst(curPrev);
            }
        }
        return path;
    }

    //public static float StepDistance(Vector2 Vector2A, Vector2 Vector2B)
    //{
    //    float distanceX = Math.Abs(Vector2A.X - Vector2B.X);
    //    float distanceY = Math.Abs(Vector2A.Y - Vector2B.Y);

    //    return distanceX + distanceY;
    //}

    //public float StepDistanceToEnd(Vector2 Vector2)
    //{
    //    return StepDistance(Vector2, endVector2);
    //}


    public bool FindPath(Vector2 start, Vector2 end, List<Vector2> result)
    {

        if (start == end)
        {
            return true;
        }
        Reset();
        startVector2 = start;
        endVector2 = end;

        openList.Add(new SearchNode(start, m_levelMap.DistanceToTarget(start), 0));
        IsSearching = true;
        while (IsSearching)
        {
            DoSearchStep();
        }

        if (searchStatus == SearchStatus.PathFound)
        {
            Vector2 curPrev = endVector2;
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

    public void OpenMapTiles(Vector2 center, List<Vector2> results)
    {
        foreach (Vector2 p in m_levelMap.GetDirections())
        {
            Vector2 adjusted = Vector2.Add(center, p);
            if(m_levelMap.CanMove(center,adjusted))
            {
                results.Add(adjusted);
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

struct SearchNode
{
    /// <summary>
    /// Location on the map
    /// </summary>
    public Vector2 Position;

    /// <summary>
    /// Distance to goal estimate
    /// </summary>
    public float DistanceToGoal;

    /// <summary>
    /// Distance traveled from the start
    /// </summary>
    public float DistanceTraveled;

    public SearchNode(Vector2 mapPosition, float distanceToGoal, float distanceTraveled)
    {
        Position = mapPosition;
        DistanceToGoal = distanceToGoal;
        DistanceTraveled = distanceTraveled;
    }
}
