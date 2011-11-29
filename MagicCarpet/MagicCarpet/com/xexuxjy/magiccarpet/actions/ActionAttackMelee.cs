using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.spells;
using com.xexuxjy.magiccarpet.util;
using com.xexuxjy.magiccarpet.combat;

namespace com.xexuxjy.magiccarpet.actions
{
    public class ActionAttackMelee : BaseAction
    {
        public ActionAttackMelee(GameObject owner, GameObject target,float attackRange,float meleeDamage)
            : base(owner, target, ActionState.AttackingMelee)
        {
            Duration = 1.0f;
            m_attackRange = attackRange;
            m_meleeDamage = meleeDamage;
        }

        public override void Start()
        {
            base.Start();
            if (GameUtil.InRange(Owner, Target, m_attackRange))
            {
                Target.Damaged(new DamageData(Owner,m_meleeDamage));
            }
        }

        float m_meleeDamage = 0f;
        float m_attackRange = 0;
    }
}
