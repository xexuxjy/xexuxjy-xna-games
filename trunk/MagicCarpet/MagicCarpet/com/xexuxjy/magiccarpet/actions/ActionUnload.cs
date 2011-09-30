using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;
using System.Diagnostics;

namespace com.xexuxjy.magiccarpet.actions
{
    public class ActionUnload : BaseAction
    {
        public ActionUnload(GameObject owner, GameObject target)
            : base(owner, target, ActionState.Unloading)
        {
            Debug.Assert(owner is Balloon);
            Debug.Assert(target is Castle);
            Duration = Globals.BalloonUnLoadTime;
        }
    }
}
