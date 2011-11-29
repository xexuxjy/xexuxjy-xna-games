using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;

namespace com.xexuxjy.magiccarpet.actions
{
    public class ActionIdle : BaseAction
    {
        public ActionIdle(GameObject owner)
            : base(owner, null, ActionState.Idle)
        {
            Duration = s_idleDuration;
        }

        const float s_idleDuration = 2f;
    }
}
