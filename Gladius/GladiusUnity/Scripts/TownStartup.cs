using System;
using System.Text;
using UnityEngine;

public class TownStartup: CommonStartup
{
    public String TownName = "Trikata";
    public String SchoolName = "Legionaires-School";
    public TownData TownData;
    private bool m_townDataReady = false;

    public bool TownDataReady
    {
        get
        {
            return m_townDataReady;
        }

        set
        {
            m_townDataReady = value;
        }
    }


    // Use this for initialization
    public override void ChildStart()
    {
        TownData = GladiusGlobals.GameStateManager.TownManager.Find(TownName);
        GetComponent<TownGUIController>().TownData = TownData;
        GladiatorSchool school = new GladiatorSchool();
        GladiusGlobals.GladiatorSchool = school;
        //m_school.Load("Orins-school");
        school.Load(SchoolName);
        TownDataReady = true;
    
    }
}
