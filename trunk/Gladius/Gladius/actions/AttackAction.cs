using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gladius.actors;

namespace Gladius.actions
{
    public class AttackAction : BaseAction
    {
        public AttackAction(Arena arena, BaseActor baseActor)
            : base(arena, baseActor)
        {

        }

        public override string Name
        {
            get { return "Attack"; }
        }
    }
}
