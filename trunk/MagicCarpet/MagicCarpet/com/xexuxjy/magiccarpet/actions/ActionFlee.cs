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
        public ActionFlee(GameObject owner, Vector3 direction)
            : base(owner, null, ActionState.Fleeing)
        {
            Owner.Direction = direction;
            Owner.TargetSpeed = s_fleeSpeed;
            Duration = s_fleeDuration;
        }

        protected override void InternalComplete()
        {
            Owner.TargetSpeed = 0f;
        }


        const float s_fleeDuration = 5f;
        const float s_fleeSpeed = 10f;

    }
}
