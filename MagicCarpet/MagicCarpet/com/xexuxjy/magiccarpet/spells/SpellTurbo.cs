using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.gameobjects;

namespace com.xexuxjy.magiccarpet.spells
{
    public class SpellTurbo : Spell
    {
        public SpellTurbo(GameObject owner)
            : base(owner)
        {

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Initialize(SpellTemplate spellTemplate)
        {
            base.Initialize(spellTemplate);
            m_modifier = new GameObjectAttributeModifier();
            m_modifier.Initialize(Owner.GetAttribute(GameObjectAttributeType.Speed),
                GameObjectAttributeModifierEffectDuration.InstantTemporary, 
                GameObjectAttributeModifierStyle.Multiply,m_multiplier, m_duration);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Update(GameTime gameTime)
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
        const float m_multiplier = 1.5f;
        const float m_duration = 10.0f;

    }
}
