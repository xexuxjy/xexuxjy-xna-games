using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;

namespace com.xexuxjy.magiccarpet.actions
{
    public class ActionSpawn : BaseAction
    {
         public ActionSpawn(GameObject owner)
            : base(owner, null, ActionState.Spawning)
        {
            int ibreak = 0;
        }
    }
}
