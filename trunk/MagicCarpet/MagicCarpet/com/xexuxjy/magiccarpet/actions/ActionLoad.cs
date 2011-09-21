using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;

namespace com.xexuxjy.magiccarpet.actions
{
    public class ActionLoad : BaseAction
    {
        public ActionLoad(GameObject owner,GameObject target) : base(owner,target,ActionState.Loading)
        {
            Duration = Globals.BalloonLoadTime;
        }
    }
}
