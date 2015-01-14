using System;
using System.Text;
using UnityEngine;

public class TownStartup: MonoBehaviour
{
    public String TownName = "Trikata";
    public TownData TownData;

        // Use this for initialization
    void Start()
    {
        if (GladiusGlobals.GameStateManager == null)
        {
            GladiusGlobals.GameStateManager = new GameStateManager();
            GladiusGlobals.GameStateManager.StartGame();
        }

        TownData = GladiusGlobals.GameStateManager.TownManager.Find(TownName);
        GetComponent<TownGUIController>().SetData(TownData);
        //ActorGenerator.Initialise();
        //AttackSkillDictionary.

    }
}
