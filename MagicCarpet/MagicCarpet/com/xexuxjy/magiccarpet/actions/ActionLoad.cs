using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;

namespace com.xexuxjy.magiccarpet.actions
{
    public class ActionLoad : BaseAction
    {
        public ActionLoad( GameObject owner, GameObject target)
            : base(owner, target, ActionState.Loading)
        {
            Duration = Globals.s_balloonLoadTime;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Start()
        {
            base.Start();
            ((ManaBall)Target).BeingLoaded = true;
        }

        protected override void InternalComplete()
        {
            // if we didn't complete normally then allow the manaball to go free.
            if (!Complete)
            {
#if LOG_EVENT
                Globals.EventLogger.LogEvent(String.Format("ActionLoad not complete, dropping manaball [{0}]",Owner.Id ));
#endif

                ((ManaBall)Target).BeingLoaded = false;
            }

        }
    }
}
