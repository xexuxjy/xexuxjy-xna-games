using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;
using Microsoft.Xna.Framework;

namespace com.xexuxjy.magiccarpet.spells
{
    public class Heal : Spell
    {

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Initialize(GameObject owner)
        {
            base.Initialize(owner);
            //Set up to heal the caster for m_multipler over duration
            m_modifier = new GameObjectAttributeModifier();
            m_modifier.Initialize(Owner.GetAttribute(GameObjectAttributeType.Health),
                GameObjectAttributeModifierEffectDuration.OverTimePermanent,
                GameObjectAttributeModifierStyle.Add, m_multiplier, m_duration);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void Update(GameTime gameTime)
        {
            // delegate it on to the modifier
            m_modifier.Update(gameTime);
            if (m_modifier.Complete())
            {
                // and if thats complete then do that as well.
                Cleanup();
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////


        GameObjectAttributeModifier m_modifier;
        const float m_multiplier = 100f;
        const float m_duration = 10.0f;

    }
}
