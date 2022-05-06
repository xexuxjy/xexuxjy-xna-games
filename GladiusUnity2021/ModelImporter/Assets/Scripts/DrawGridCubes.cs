using System.Collections.Generic;
using UnityEngine;

public class DrawGridCubes : MonoBehaviour
{

    public TextAsset CurrentGridFile;
    
    public Arena Arena;

    private GameObject GridFileDebugParent;
    public Point CurrentPoint;
    
    public bool DrawTeamStarts = false;
    public bool DrawBlockedPoints = true;

    // Use this for initialization
    void Start()
    {
        GridFileDebugParent = new GameObject("GridFileDebug");
        DrawDebugGridCubes();
    }


    public void DrawDebugGridCubes()
    {
        GridFile gridFile = GridFileManager.ReadFile("TestFile", CurrentGridFile);
        if (gridFile != null)
        {

            GladiusGlobals.DestroyChildren(GridFileDebugParent.transform);
            if (DrawBlockedPoints)
            {
                for (int x = 0; x < Arena.ArenaSize; ++x)
                {
                    for (int y = 0; y < Arena.ArenaSize; ++y)
                    {

                        if (gridFile.TestPointKey(x, y, GridFile.noMoveKey) ||
                            gridFile.TestPointKey(x, y, GridFile.noMoveNoCursorKey) ||
                            gridFile.TestPointKey(x, y, GridFile.mapCenterKey)
                            )
                        {

                            Color c = new Color();
                            if (gridFile.TestPointKey(x, y, GridFile.noMoveKey))
                            {
                                c.r = 1;
                            }
                            if (gridFile.TestPointKey(x, y, GridFile.noMoveNoCursorKey))
                            {
                                c.g = 1;
                            }
                            if (gridFile.TestPointKey(x, y, GridFile.mapCenterKey))
                            {
                                c.b = 1;
                            }
                            if (CurrentPoint.X == x && CurrentPoint.Y == y)
                            {
                                c = Color.white;
                            }

                            c.a = 0.5f;
                            DrawCube(new Point(x, y), c, Arena);

                        }
                    }
                }
            }

            if (DrawTeamStarts)
            {
                int teamKey = 0;
                foreach (string key in GridFile.teamStartKeys)
                {

                    List<Point> pointList = null;
                    //if (gridFile.TeamStartPoints.TryGetValue(key, out pointList))
                    //{
                    //    Color c = GladiatorSchool.SchoolColours[teamKey];
                    //    foreach (Point p in pointList)
                    //    {
                    //        DrawCube(p, c);
                    //    }
                    //}
                    teamKey++;
                }
            }
            DrawCube(CurrentPoint, Color.white, Arena);
        }
    }

    private void DrawCube(Point p, Color c,Arena arena)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = string.Format("x-{0}-y-{1}", p.X, p.Y);
        go.transform.SetParent(GridFileDebugParent.transform);

        //Vector3 midPoint = new Vector3(-Arena.ArenaSize * scaleMultipler / 2.0f, 0, -Arena.ArenaSize * scaleMultipler / 2.0f);

        Vector3 midPoint = arena.GridPointToWorld(p);

        go.transform.localPosition = midPoint;
        go.transform.localScale = new Vector3(arena.BlockSize, arena.BlockSize, arena.BlockSize);
        go.GetComponent<Renderer>().material.SetFloat("_Mode", 3.0f);
        go.GetComponent<Renderer>().material.color = c;

    }

}
