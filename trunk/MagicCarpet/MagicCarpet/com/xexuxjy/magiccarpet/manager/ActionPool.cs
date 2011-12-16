using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;
using com.xexuxjy.magiccarpet.actions;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.spells;

namespace com.xexuxjy.magiccarpet.manager
{
    public class ActionPool
    {
        public ActionAttackMelee GetActionAttackMelee(GameObject owner, GameObject target, float attackRange, float meleeDamage)
        {
            return new ActionAttackMelee(owner, target, attackRange, meleeDamage);
        }

        public ActionAttackRange GetActionAttackRange(GameObject owner, GameObject target,float attackRange,float damage, SpellType spellType)
        {
            return new ActionAttackRange(owner, target, attackRange, damage,spellType);
        }

        public ActionDie GetActionDie(GameObject owner)
        {
            return new ActionDie(owner);
        }

        public ActionFindCastle GetActionFindCastle(GameObject owner, GameObject target, float searchRadius)
        {
            return new ActionFindCastle(owner, target, searchRadius);
        }

        public ActionFindEnemy GetActionFindEnemy(GameObject owner, float searchRadius)
        {
            return new ActionFindEnemy(owner, searchRadius);

        }

        public ActionFindLocation GetActionFindLocation(GameObject owner, float searchRadius)
        {
            return new ActionFindLocation(owner, searchRadius);

        }

        public ActionFindManaball GetActionFindManaball(GameObject owner, GameObject target, float searchRadius)
        {
            return new ActionFindManaball(owner, target,searchRadius);
        }

        public ActionFlee GetActionFlee(GameObject owner, Vector3 direction, float speed)
        {
            return new ActionFlee(owner, direction, speed);
        }

        public ActionIdle GetActionIdle(GameObject owner)
        {
            return new ActionIdle(owner);
        }

        public ActionLoad GetActionLoad(GameObject owner, GameObject target)
        {
            return new ActionLoad(owner, target);
        }

        public ActionUnload GetActionUnload(GameObject owner, GameObject target)
        {
            return new ActionUnload(owner, target);
        }

        public ActionTravel GetActionTravel(GameObject owner, GameObject target, Vector3 position, float speed)
        {
            return new ActionTravel(owner, target, position, speed);
        }

        public ActionCastSpell GetActionCastSpell(GameObject owner, GameObject target, Vector3 position, SpellType spellType)
        {
            return new ActionCastSpell(owner, target, position, spellType);
        }

    }
}
