using UnityEngine;
using System.Collections;
using Gladius;

public class WorldInit : MonoBehaviour
{
    public GameObject PlayerParty;

    // Use this for initialization
    void Start()
    {
        Application.targetFrameRate = 30;

        if (GladiusGlobals.LocalisationData == null)
        {
            GladiusGlobals.LocalisationData = new LocalisationData();
            GladiusGlobals.LocalisationData.Load(null);
        }

        if (GladiusGlobals.CameraManager == null)
        {
            GladiusGlobals.CameraManager = new CameraManager();
        }
        GladiusGlobals.CameraManager.CameraTarget = PlayerParty;
        GladiusGlobals.CameraManager.CurrentCameraMode = CameraMode.Overland;

        if (GladiusGlobals.UserControl == null)
        {
            GladiusGlobals.UserControl = new UserControl();
        }


        GameObject[] towns = GameObject.FindGameObjectsWithTag("Town");
        foreach (GameObject go in towns)
        {
            TownData td = go.AddComponent<TownData>();
            td.TownName = go.name;
            td.Popularity = 10;
        }






    }

    // Update is called once per frame
    void Update()
    {

    }
}
