using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class CommonStartup : MonoBehaviour
{
    public void Start()
    {
        Application.targetFrameRate = 30;

        if (GladiusGlobals.GameStateManager == null)
        {
            GladiusGlobals.GameStateManager = new GameStateManager();
            GladiusGlobals.GameStateManager.StartGame();
        }
        ChildStart();
    }

    public abstract void ChildStart();

}
