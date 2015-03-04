using UnityEngine;
using System.Collections;
using Gladius;

public class OverlandStartup : MonoBehaviour
{
    public GameObject PlayerParty;
    public string SchoolName = "Orins-School";
    // Use this for initialization
    void Start()
    {
        Application.targetFrameRate = 30;

        if (GladiusGlobals.GameStateManager == null)
        {
            GladiusGlobals.GameStateManager = new GameStateManager();
        }

        OverlandStateCommon state = new OverlandStateCommon();
        state.CameraManager = GameObject.Find("Main Camera").GetComponent<CameraManager>();
        state.GladiatorSchool = new GladiatorSchool();
        GladiusGlobals.GameStateManager.SetStateData(state);

        state.GladiatorSchool.Load(SchoolName);

        state.CameraManager.CameraTarget = PlayerParty;
        state.CameraManager.CurrentCameraMode = CameraMode.Overland;
            

        GameObject[] towns = GameObject.FindGameObjectsWithTag("Town");
        //foreach (GameObject go in towns)
        //{
        //    TownData td = go.AddComponent<TownData>();
        //    td.TownName = go.name;
        //}






    }

    // Update is called once per frame
    void Update()
    {

    }
}
