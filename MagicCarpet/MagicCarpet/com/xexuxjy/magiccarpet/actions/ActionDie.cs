using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;

namespace com.xexuxjy.magiccarpet.actions
{
    public class ActionDie : BaseAction
    {

        public ActionDie(GameObject owner)
            : base(owner, null, ActionState.Dieing)
        {
            int ibreak = 0;
        }

    }
}
