using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.spells;
using BulletXNA.LinearMath;

namespace com.xexuxjy.magiccarpet.actions
{
    public class ActionCastSpell : BaseAction
    {
        public ActionCastSpell(GameObject owner, GameObject target, IndexedVector3? targetPosition, SpellType spellType) : base(owner,target,ActionState.Casting)
        {

        }

        private SpellType m_spellType;
    }
}
