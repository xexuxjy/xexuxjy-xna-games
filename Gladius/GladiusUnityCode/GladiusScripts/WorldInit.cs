using UnityEngine;
using System.Collections;
using Gladius;

public class WorldInit : MonoBehaviour
{
    public GameObject PlayerParty;
    public string SchoolName = "Orins-School";
    // Use this for initialization
    void Start()
    {
        Application.targetFrameRate = 30;

        //ActorGenerator.Initialise();

        GladiusGlobals.GladiatorSchool = new GladiatorSchool();
        GladiusGlobals.GladiatorSchool.Load(SchoolName);

        if (GladiusGlobals.CameraManager == null)
        {
            GladiusGlobals.CameraManager = new CameraManager();
        }
        GladiusGlobals.CameraManager.CameraTarget = PlayerParty;
        GladiusGlobals.CameraManager.CurrentCameraMode = CameraMode.Overland;


        GameObject[] towns = GameObject.FindGameObjectsWithTag("Town");
        foreach (GameObject go in towns)
        {
            TownData td = go.AddComponent<TownData>();
            td.TownName = go.name;
        }






    }

    // Update is called once per frame
    void Update()
    {

    }
}
