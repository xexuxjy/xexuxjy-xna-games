using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.spells;
using BulletXNA.LinearMath;
using com.xexuxjy.magiccarpet.util;

namespace com.xexuxjy.magiccarpet.actions
{
    public class ActionCastSpell : BaseAction
    {
        public ActionCastSpell(GameObject owner, GameObject target, Vector3? targetPosition, SpellType spellType) : base(owner,target,ActionState.Casting)
        {
            m_spellType = spellType;
            m_targetPosition = targetPosition;
            // Check if we can cast this?
            Duration = owner.SpellComponent.CastTime(spellType);

        }

        public override void Start()
        {
            base.Start();
            Vector3 targetDirection = Vector3.Up;
            if (Target != null)
            {
                targetDirection = GameUtil.DirectionToTarget(Owner, Target);
            }
            else
            {
                targetDirection = GameUtil.DirectionToTarget(Owner, m_targetPosition.Value);
            }
            Owner.CastSpell(m_spellType, Owner.Position, targetDirection);
        }



        private SpellType m_spellType;
        private Vector3? m_targetPosition;

    }
}
