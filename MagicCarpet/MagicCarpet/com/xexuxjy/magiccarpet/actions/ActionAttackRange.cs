using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.spells;
using com.xexuxjy.magiccarpet.util;

namespace com.xexuxjy.magiccarpet.actions
{
    public class ActionAttackRange : BaseAction
    {
        public ActionAttackRange(GameObject owner, GameObject target,float attackRange,float damage, SpellType spellType)
            : base(owner, target, ActionState.AttackingRange)
        {
            Duration = 1.0f;
            m_attackRange = attackRange;
            m_damage = damage;
            m_spellType = spellType;
        }

        public override void Start()
        {
            base.Start();
            if (GameUtil.InRange(Owner, Target, m_attackRange))
            {
                Matrix m = Matrix.CreateLookAt(Owner.SpellCastPosition, Target.Position, Vector3.Up);
                // just want the rotation?
                m.Translation = Vector3.Zero;
                Owner.CastSpell(m_spellType, Owner.SpellCastPosition, m);
            }

        }

        float m_attackRange;
        float m_damage;
        SpellType m_spellType;
    }
}
