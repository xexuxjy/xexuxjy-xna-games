using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gladius.util;
using Gladius;

public static class EventManager
{
    public delegate void BaseActorSelectionChanged(object sender, BaseActorChangedArgs baca);
    public delegate void ActionButtonPressed(object sender, ActionButtonPressedArgs e);

    public static event BaseActorSelectionChanged BaseActorChanged;
    public static event ActionButtonPressed ActionPressed;


    public static void ChangeActor(Object caller, BaseActor originalBa, BaseActor newBa)
    {
        GladiusGlobals.EventLogger.LogEvent(EventTypes.Update, "ChangeActorRaised");
        if (BaseActorChanged != null)
        {
            BaseActorChanged(caller, new BaseActorChangedArgs(originalBa, newBa));
        }
    }

    public static void PerformAction(Object caller, ActionButton button)
    {
        GladiusGlobals.EventLogger.LogEvent(EventTypes.Update, "PerformActionRaised [" + button + "]");
        if (ActionPressed != null)
        {
            ActionPressed(caller, new ActionButtonPressedArgs(button));
        }
    }

}

public class BaseActorChangedArgs : EventArgs
{
    public BaseActorChangedArgs(BaseActor originalActor, BaseActor newActor)
    {
        Original = originalActor;
        New = newActor;
    }

    public BaseActor Original
    {
        get;
        private set;
    }
    public BaseActor New
    {
        get;
        private set;
    }
}



public class ActionButtonPressedArgs : EventArgs
{
    public ActionButtonPressedArgs(ActionButton button)
    {
        ActionButton = button;
    }

    public ActionButton ActionButton
    {
        get;
        private set;
    }
}

