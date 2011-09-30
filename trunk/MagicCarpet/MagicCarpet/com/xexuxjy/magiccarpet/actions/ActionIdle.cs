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
        }

        public override void Initialize()
        {
            Duration = 1f;
            base.Initialize();
        }
    }
}
