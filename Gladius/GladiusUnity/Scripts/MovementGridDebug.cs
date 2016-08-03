//using UnityEngine;
//using System.Collections;

//public class MovementGridDebug : MonoBehaviour
//{
//    public MovementGrid MovementGrid;
//    public GameObject PlayerModel;
//    // Use this for initialization
//    void Start()
//    {


//    }

//    // Update is called once per frame
//    void Update()
//    {
//        //if (PlayerModel != null && MovementGrid != null && MovementGrid.MovementGridCursor != null && PlayerModel.transform.parent == null)
//        //{
//        //    PlayerModel.transform.parent = MovementGrid.MovementGridCursor.transform;
//        //}

//        if (PlayerModel != null && MovementGrid != null)
//        {
//            PlayerModel.GetComponent<BaseActor>().MovementGridPoint = MovementGrid.CurrentCursorPoint;
//        }

//        float moveRate = 0.3f;
//        if (Input.GetKey(KeyCode.PageDown))
//        {
//            Camera.main.transform.position += new Vector3(0, -moveRate, 0);
//        }
//        if (Input.GetKey(KeyCode.PageUp))
//        {
//            Camera.main.transform.position += new Vector3(0, moveRate, 0);
//        }
//        if (Input.GetKey(KeyCode.LeftArrow))
//        {
//            Camera.main.transform.position += new Vector3(-moveRate, 0, 0);
//        }
//        if (Input.GetKey(KeyCode.RightArrow))
//        {
//            Camera.main.transform.position += new Vector3(moveRate, 0, 0);
//        }
//        if (Input.GetKey(KeyCode.UpArrow))
//        {
//            Camera.main.transform.position += new Vector3(0, 0, moveRate);
//        }
//        if (Input.GetKey(KeyCode.DownArrow))
//        {
//            Camera.main.transform.position += new Vector3(0, 0, -moveRate);
//        }
        

//        if (Input.GetKeyDown(KeyCode.I))
//        {
//            if (MovementGrid.CurrentCursorPoint.Y < Arena.ArenaSize - 1)
//            {
//                MovementGrid.CurrentCursorPoint = GladiusGlobals.Add(MovementGrid.CurrentCursorPoint, new Point(0, 1));
//                MovementGrid.RebuildActorTest();
//                UnityEngine.Debug.LogFormat("Cursor pos {0} Character pos {1} , {2}", MovementGrid.CurrentV3, PlayerModel.transform.position, PlayerModel.GetComponent<BaseActor>().Position);
//            }
//        }
//        if (Input.GetKeyDown(KeyCode.J))
//        {
//            if (MovementGrid.CurrentCursorPoint.X > 0)
//            {
//                MovementGrid.CurrentCursorPoint = GladiusGlobals.Add(MovementGrid.CurrentCursorPoint, new Point(-1, 0));
//                MovementGrid.RebuildActorTest();
//                UnityEngine.Debug.LogFormat("Cursor pos {0} Character pos {1} , {2}", MovementGrid.CurrentV3, PlayerModel.transform.position, PlayerModel.GetComponent<BaseActor>().Position);
//            }
//        }
//        if (Input.GetKeyDown(KeyCode.K))
//        {
//            if (MovementGrid.CurrentCursorPoint.Y > 0)
//            {
//                MovementGrid.CurrentCursorPoint = GladiusGlobals.Add(MovementGrid.CurrentCursorPoint, new Point(0, -1));
//                MovementGrid.RebuildActorTest();
//                UnityEngine.Debug.LogFormat("Cursor pos {0} Character pos {1} , {2}", MovementGrid.CurrentV3, PlayerModel.transform.position, PlayerModel.GetComponent<BaseActor>().Position);
//            }
//        }
//        if (Input.GetKeyDown(KeyCode.L))
//        {
//            if (MovementGrid.CurrentCursorPoint.X < Arena.ArenaSize - 1)
//            {
//                MovementGrid.CurrentCursorPoint = GladiusGlobals.Add(MovementGrid.CurrentCursorPoint, new Point(1, 0));
//                MovementGrid.RebuildActorTest();
//                UnityEngine.Debug.LogFormat("Cursor pos {0} Character pos {1} , {2}", MovementGrid.CurrentV3, PlayerModel.transform.position, PlayerModel.GetComponent<BaseActor>().Position);
//            }
//        }

//        //Vector3 cursorPos = MovementGrid.V3ForSquare(MovementGrid.CurrentCursorPoint);
//        //PlayerModel.transform.position = cursorPos;


//    }
//}
