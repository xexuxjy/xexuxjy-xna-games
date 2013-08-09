using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gladius.actors;
using Gladius.combat;

namespace Gladius.actions
{
    public class AttackAction : BaseAction
    {
        public AttackAction(Arena arena, BaseActor baseActor,AttackSkill attackData)
            : base(arena, baseActor)
        {

        }

        public override void Start()
        {
            
            foreach(BaseActor target in Targets)
            {
                AttackResult attackResult = Globals.CombatEngine.ResolveAttack(m_baseActor,target,m_attackData);
                target.TakeDamage(attackResult);
            }
        }

        public override void Stop()
        {

        }

        public override string Name
        {
            get { return "Attack"; }
        }

        public List<BaseActor> Targets = new List<BaseActor>();
        public AttackSkill m_attackData;
    }
}
