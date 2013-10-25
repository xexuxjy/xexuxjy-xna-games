using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Gladius.actors;
using Gladius.gamestatemanagement.screens;

namespace Gladius.modes.arena
{
    public class Arena
    {

        public Arena(ArenaScreen arenaScreen,int width, int breadth)
        {
            m_arenaScreen = arenaScreen;
            m_arenaGrid = new SquareType[width, breadth];
            m_width = width;
            m_breadth = breadth;
            m_pathFinder = new ArenaPathFinder();
            m_pathFinder.Initialize(this);
            BuildDefaultArena();
        }

        public bool InLevel(Point p)
        {
            return p.X >= 0 && p.X < Width && p.Y >= 0 && p.Y < Breadth;
        }


        public bool InLevel(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Breadth;
        }

        public void BuildDefaultArena()
        {
            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j < Breadth; ++j)
                {
                    if (i == 0 || j == 0 || i == Width - 1 || j == Breadth - 1)
                    {
                        //m_arenaGrid[i, j] = SquareType.Wall;
                    }
                    else
                    {
                        m_arenaGrid[i, j] = SquareType.Empty;
                    }
                }
            }
        }


        public int Width
        {
            get
            {
                return m_width;
            }
        }
        public int Breadth
        {
            get
            {
                return m_breadth;
            }
        }

        public void SetActorAtPosition(int x, int y, BaseActor actor)
        {
            m_arenaGrid[x, y] = SquareType.Mobile;

        }

        public bool CanMoveActor(BaseActor baseActor, Point newPoint)
        {
            // deal with moving to same position
            if (GetActorAtPosition(newPoint) == baseActor)
            {
                return true;
            }
            return m_arenaGrid[newPoint.X, newPoint.Y] == SquareType.Empty;

        }

        // move actor - assumes any validity checks have already taken place.
        public void MoveActor(BaseActor baseActor, Point newPoint)
        {
            Debug.Assert(CanMoveActor(baseActor,newPoint));
            if (m_arenaGrid[newPoint.X, newPoint.Y] == SquareType.Empty)
            {
                // set current actor square to empty
                //baseActor
                m_arenaGrid[baseActor.CurrentPosition.X, baseActor.CurrentPosition.Y] = SquareType.Empty;
                if (m_baseActorMap.ContainsKey(baseActor.CurrentPosition))
                {
                    m_baseActorMap.Remove(baseActor.CurrentPosition);
                }
                m_arenaGrid[newPoint.X, newPoint.Y] = SquareType.Mobile;
                baseActor.CurrentPosition = newPoint;
                m_baseActorMap[baseActor.CurrentPosition] = baseActor;
            }
        }

        public void AssertBounds(Point p)
        {
            if (p.X < 0 || p.X >= Width || p.Y < 0 || p.Y >= Breadth)
            {
                Debug.Assert(false);
            }
        }

        public BaseActor GetActorAtPosition(Point p)
        {
            BaseActor result = null;
            if (InLevel(p))
            {
                m_baseActorMap.TryGetValue(p, out result);
            }
            return result;
        }


        public float GetHeightAtLocation(Point point)
        {
            SquareType st = GetSquareTypeAtLocation(point);
            float result = 0;
            switch (st)
            {
                case (SquareType.Level1):
                    {
                        result = 0.5f;
                        break;
                    }
                case (SquareType.Level2):
                    {
                        result = 1.0f;
                        break;
                    }
                case (SquareType.Level3):
                    {
                        result = 1.5f;
                        break;
                    }
                default:
                    result = 0.0f;
                    break;
            }
            return result;
        }

        public SquareType GetSquareTypeAtLocation(Point p)
        {
            AssertBounds(p);
            return m_arenaGrid[p.X, p.Y];
        }


        public SquareType GetSquareTypeAtLocation(int x, int y)
        {
            return m_arenaGrid[x, y];
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


        public Vector3 ArenaToWorld(Point p, bool includeHeight = true)
        {
            float groundHeight = includeHeight ? GetHeightAtLocation(p) : 0.0f; ;
            Vector3 topLeft = new Vector3(-Width / 2f, 0, -Breadth / 2f);
            topLeft += Position;

            Vector3 result = topLeft + new Vector3(p.X, groundHeight, p.Y);
            return result;
            //    //DrawBox(Vector3.One, texture2d, translation);
            //    DrawBaseActor3(m_baseActorScale, m_baseActorTexture, translation);

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


        public Vector3 Position
        {
            get;
            set;
        }

        public bool FindPath(Point start, Point end, List<Point> result)
        {
            return m_pathFinder.FindPath(start, end, result);
        }

        // dumb version for now. doesn't check path
        public BaseActor FindNearestEnemy(BaseActor searcher)
        {
            float closest = float.MaxValue;
            BaseActor closestActor = null;
            foreach (BaseActor enemy in m_baseActorMap.Values)
            {
                if (m_arenaScreen.CombatEngine.IsValidTarget(searcher,enemy,searcher.CurrentAttackSkill))
                {
                    float dist = (enemy.Position - searcher.Position).LengthSquared();
                    if (dist < closest)
                    {
                        closestActor = enemy;
                    }
                }

            }
            return closestActor;
        }

        public BaseActor NextToEnemy(BaseActor source, bool orthogonal = true)
        {
            foreach (Point p2 in orthogonal ? m_orthognalPoints : m_surroundingPoints)
            {
                Point adjusted = source.CurrentPosition + p2;
                if (InLevel(adjusted))
                {
                    if (GetSquareTypeAtLocation(adjusted) == SquareType.Mobile)
                    {
                        BaseActor ba = m_baseActorMap[adjusted];
                        if (ba.Team != source.Team)
                        {
                            return ba;
                        }
                    }
                }
            }
            return null;
        }


        public Point PointNearestLocation(Point startLocation,Point location, bool includeSquare = true)
        {
            if (includeSquare)
            {
                if (GetSquareTypeAtLocation(location) == SquareType.Empty)
                {
                    return location;
                }
            }
            int closest = int.MaxValue;
            Point closestPoint = Point.Zero;
            foreach (Point p in m_orthognalPoints)
            {
                Point adjusted = location + p;
                if (InLevel(adjusted) && GetSquareTypeAtLocation(adjusted) == SquareType.Empty)
                {
                    if (Globals.PointDist2(adjusted, startLocation) < closest)
                    {
                        closestPoint = adjusted;
                    }
                }
            }
            return closestPoint;
        }


        private ArenaScreen m_arenaScreen;
        private SquareType[,] m_arenaGrid;
        private int m_width;
        private int m_breadth;

        private Dictionary<Point, BaseActor> m_baseActorMap = new Dictionary<Point, BaseActor>();
        private ArenaPathFinder m_pathFinder;
        private Random m_rng = new Random();
        Point[] m_orthognalPoints = new Point[] { new Point(-1, 0), new Point(1, 0), new Point(0, -1), new Point(0, 1) };
        Point[] m_surroundingPoints = new Point[] { new Point(-1, -1), new Point(0, -1), new Point(1, -1), new Point(0, -1), 
            new Point(0,1),new Point(-1,1),new Point(0,1),new Point(1,1)};
    }

    public enum SquareType
    {
        Empty,
        Level1,
        Level2,
        Level3,
        Unaccesible,
        Wall,
        Crowd,
        Mobile
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
        private SearchMethod searchMethod = SearchMethod.BestFirst;

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
        public void Update(GameTime gameTime)
        {
            if (searchStatus == SearchStatus.Searching)
            {
                timeSinceLastSearchStep += (float)gameTime.ElapsedGameTime.TotalSeconds;
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

            if (m_levelMap.InLevel(x, y) && m_levelMap.GetSquareTypeAtLocation(x, y) == SquareType.Empty)
            {
                results.Add(new Point(x, y));
            }

            y = (int)center.Y - 1;
            if (m_levelMap.InLevel(x, y) && m_levelMap.GetSquareTypeAtLocation(x, y) == SquareType.Empty)
            {
                results.Add(new Point(x, y));
            }

            y = (int)center.Y;
            x = (int)center.X + 1;

            if (m_levelMap.InLevel(x, y) && m_levelMap.GetSquareTypeAtLocation(x, y) == SquareType.Empty)
            {
                results.Add(new Point(x, y));
            }
            x = (int)center.X - 1;

            if (m_levelMap.InLevel(x, y) && m_levelMap.GetSquareTypeAtLocation(x, y) == SquareType.Empty)
            {
                results.Add(new Point(x, y));
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

}
