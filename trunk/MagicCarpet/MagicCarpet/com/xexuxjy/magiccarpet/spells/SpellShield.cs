using System;
using com.xexuxjy.magiccarpet.gameobjects;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.manager;
using Microsoft.Xna.Framework.Graphics;

namespace com.xexuxjy.magiccarpet.spells
{
    public class SpellShield : Spell
    {
        public SpellShield(GameObject owner)
            : base(owner)
        {
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Initialize(SpellTemplate template)
        {
            base.Initialize(template);
            //Set up to heal the caster for m_multipler over duration
            m_modifier = new GameObjectAttributeModifier();
            m_modifier.Initialize(Owner.GetAttribute(GameObjectAttributeType.DamageReduction),
                GameObjectAttributeModifierEffectDuration.OverTimeTemporary,
                GameObjectAttributeModifierStyle.Add, m_multiplier, m_duration);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Start()
        {
            base.Start();
            Model model = Globals.MCContentManager.GetModelForName("SpellShield");
            Texture2D texture = Globals.MCContentManager.GetTexture("SpellShield");
            m_tempGraphicsHolder = Globals.GameObjectManager.AddTempGraphicHolder(Owner, model, texture, null, Matrix.Identity);
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

        public override void Cleanup()
        {
            Globals.GameObjectManager.ReleaseTempGraphicHolder(m_tempGraphicsHolder);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        TempGraphicHolder m_tempGraphicsHolder;
        GameObjectAttributeModifier m_modifier;
        const float m_multiplier = 0.99f;
        const float m_duration = 10.0f;

    }
}
