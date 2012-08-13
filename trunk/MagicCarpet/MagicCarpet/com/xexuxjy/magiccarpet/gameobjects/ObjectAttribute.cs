using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace com.xexuxjy.magiccarpet.gameobjects
{
    public enum GameObjectAttributeType
    {
        Health,
        Mana,
        Speed,
        DamageReduction,
        NumTypes
    }

    public enum GameObjectAttributeModifierEffectDuration
    {
        InstantPermanent,
        InstantTemporary,
        OverTimePermanent,
        OverTimeTemporary
    }

    public enum GameObjectAttributeModifierStyle
    {
        Add,
        Multiply
    }

    public class GameObjectAttributeModifier : IUpdateable
    {

        public GameObjectAttributeModifier()
        {

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Initialize(GameObjectAttribute attribute, GameObjectAttributeModifierEffectDuration durationStyle,
            GameObjectAttributeModifierStyle modifierStyle, float modifier, float duration)
        {
            m_attribute = attribute;
            m_modifierStyle = modifierStyle;
            m_durationStyle = durationStyle;
            m_duration = duration;
            m_currentTime = 0f;
            m_modifier = modifier;
            m_originalValue = attribute.CurrentValue;


            if (durationStyle == GameObjectAttributeModifierEffectDuration.InstantTemporary ||
                durationStyle == GameObjectAttributeModifierEffectDuration.OverTimeTemporary)
            {

                float newValue = m_originalValue;
                if (modifierStyle == GameObjectAttributeModifierStyle.Add)
                {
                    newValue += modifier;
                }
                else
                {
                    newValue *= modifier;
                }
                m_attribute.CurrentValue = newValue;
            }

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Cleanup()
        {
            // If the effect is temporary then restore the original version.
            if (m_durationStyle == GameObjectAttributeModifierEffectDuration.InstantTemporary ||
                m_durationStyle == GameObjectAttributeModifierEffectDuration.OverTimeTemporary)
            {
                m_attribute.CurrentValue = m_attribute.BaseValue;
            }
            else
            {
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Update(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            m_currentTime += elapsedSeconds;
            if (!Complete())
            {
                float newValue = m_originalValue;
                if (m_modifierStyle == GameObjectAttributeModifierStyle.Add)
                {
                    newValue += m_modifier;
                }
                else
                {
                    newValue *= m_modifier;
                }
                m_attribute.CurrentValue = newValue;
            }
            else
            {
                Cleanup();
            }

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool Complete()
        {
            return (m_currentTime >= m_duration);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////


        private GameObjectAttribute m_attribute;
        private GameObjectAttributeModifierEffectDuration m_durationStyle;
        private GameObjectAttributeModifierStyle m_modifierStyle;
        private float m_duration;
        private float m_currentTime;
        private float m_modifier;
        private float m_originalValue;

        public bool Enabled
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler<EventArgs> EnabledChanged;

        public int UpdateOrder
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler<EventArgs> UpdateOrderChanged;
    }


    public class GameObjectAttribute
    {
        // object array ctor.
        public GameObjectAttribute() { }
        public GameObjectAttribute(GameObjectAttributeType attributeType, float startValue)
        {
            AttributeType = attributeType;
            BaseValue = startValue;
            CurrentValue = startValue;
        }

        public GameObjectAttributeType AttributeType
        {
            get { return m_attributeType; }
            set { m_attributeType = value; }
        }



        private GameObjectAttributeType m_attributeType;
        private float m_baseValue;

        public float BaseValue
        {
            get { return m_baseValue; }
            set { m_baseValue = value; }
        }
        private float m_currentValue;

        public float CurrentValue
        {
            get { return m_currentValue; }
            set { m_currentValue = value; }
        }
        private float m_maxValue;

        public float MaxValue
        {
            get { return m_maxValue; }
            set { m_maxValue = value; }
        }
        private float m_minValue;

        public float MinValue
        {
            get { return m_minValue; }
            set { m_minValue = value; }
        }
    }
}
