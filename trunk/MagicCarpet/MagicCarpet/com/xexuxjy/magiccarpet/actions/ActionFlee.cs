using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;
using Microsoft.Xna.Framework;

namespace com.xexuxjy.magiccarpet.actions
{
    public class ActionFlee : BaseAction
    {
        public ActionFlee(GameObject owner, Vector3 direction,float speed)
            : base(owner, null, ActionState.Fleeing)
        {
            Owner.Forward = direction;
            Owner.TargetSpeed = speed;
            Duration = s_fleeDuration;
        }

        protected override void InternalComplete()
        {
            Owner.TargetSpeed = 0f;
        }


        const float s_fleeDuration = 5f;
    }
}
