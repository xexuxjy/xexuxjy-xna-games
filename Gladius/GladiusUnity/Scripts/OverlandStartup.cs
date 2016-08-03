using UnityEngine;
using System.Collections;
using Gladius;

public class OverlandStartup : CommonStartup
{
    public GameObject PlayerParty;
    public CameraManager CameraManager;
    public GladiatorSchool GladiatorSchool;

    public string SchoolName = "Orins-School";
    // Use this for initialization
    public override void ChildStart()
    {
        CameraManager = GameObject.Find("Main Camera").GetComponent<CameraManager>();
        GladiatorSchool = new GladiatorSchool();

        GladiatorSchool.Load(SchoolName);

        CameraManager.CameraTarget = PlayerParty;
        CameraManager.CurrentCameraMode = GladiusCameraMode.Overland;

        GameObject[] towns = GameObject.FindGameObjectsWithTag("Town");

        if (GladiusGlobals.ZoneTransitionData != null)
        {
            string objectName = GladiusGlobals.ZoneTransitionData.lastZone+"Portal";
            GameObject startPos =  GameObject.Find(objectName);
            int ibreak = 0;
            GladiusGlobals.ZoneTransitionData = null;
            GameObject playerParty = GameObject.Find("PlayerParty");
            playerParty.transform.position = startPos.transform.position;

        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
