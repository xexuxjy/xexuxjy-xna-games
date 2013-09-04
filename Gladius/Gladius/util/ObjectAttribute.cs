using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.ComponentModel;
using Gladius.actors;

namespace xexuxjy.Gladius.util
{
    public enum GameObjectAttributeType
    {
        Health,
        Mana,
        Movement,
        Damage,
        Agility,
        Defense,
        CriticalChance,
        EarthAffinity,
        AirAffinity,
        FireAffinity,
        WaterAffinity,
        LightAffinity,
        DarkAffinity,
        ActionPoints,
        NumTypes
    }

    public enum GameObjectAttributeModifierDurationType
    {
        InstantPermanent,
        InstantTemporary,
        OverTimePermanent,
        OverTimeTemporary
    }

    public enum GameObjectAttributeModifierType
    {
        Add,
        Multiply
    }

    public class GameObjectAttributeModifier
    {

        public GameObjectAttributeModifier()
        {

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public GameObjectAttributeModifier(GameObjectAttributeType attributeType, 
            GameObjectAttributeModifierDurationType durationType,
            GameObjectAttributeModifierType modifierType, float modifier, float duration)
        {
            m_attributeType = attributeType;
            ModiferType = modifierType;
            DurationType = durationType;
            m_duration = duration;
            m_currentTime = 0f;
            m_modifier = modifier;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Initialize(GameObjectAttributeType attributeType, BaseActor appliedTo,
            GameObjectAttributeModifierDurationType durationType,
            GameObjectAttributeModifierType modifierType, float modifier, float duration) 
        {
            m_appliedTo = appliedTo;
            m_attributeType = attributeType;
            ModiferType = modifierType;
            DurationType = durationType;
            m_duration = duration;
            m_currentTime = 0f;
            m_modifier = modifier;
            ApplyTo(m_appliedTo);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ApplyTo(BaseActor appliedTo)
        {
            if (appliedTo != null)
            {
                m_appliedTo = appliedTo;
                m_originalValue = m_appliedTo.GetAttributeValue(m_attributeType);
            }


            if (DurationType == GameObjectAttributeModifierDurationType.InstantTemporary ||
                DurationType == GameObjectAttributeModifierDurationType.OverTimeTemporary)
            {

                float newValue = m_originalValue;
                if (ModiferType == GameObjectAttributeModifierType.Add)
                {
                    newValue += m_modifier;
                }
                else
                {
                    newValue *= m_modifier;
                }
                if (m_appliedTo != null)
                {
                    m_appliedTo.SetAttributeValue(m_attributeType, newValue);
                }
            }

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Cleanup()
        {
            // If the effect is temporary then restore the original version.
            if (m_durationType == GameObjectAttributeModifierDurationType.InstantTemporary ||
                m_durationType == GameObjectAttributeModifierDurationType.OverTimeTemporary)
            {
                if (m_appliedTo != null)
                {
                    m_appliedTo.SetAttributeValue(m_attributeType, m_originalValue);
                }
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
            if (!Complete)
            {
                float newValue = m_originalValue;
                if (m_modifierType == GameObjectAttributeModifierType.Add)
                {
                    newValue += m_modifier;
                }
                else
                {
                    newValue *= m_modifier;
                }
                if (m_appliedTo != null)
                {
                    m_appliedTo.SetAttributeValue(m_attributeType, newValue);
                }
            }
            else
            {
                Cleanup();
            }

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        [ReadOnlyAttribute(true)] 
        public bool Complete
        {
            get
            {
                if (m_duration == INFINITE_DURATION)
                {
                    return false;
                }
                return (m_currentTime >= m_duration);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public GameObjectAttributeType AttributeType
        {
            get { return m_attributeType; }
            set { m_attributeType = value; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public GameObjectAttributeModifierDurationType DurationType
        {
            get { return m_durationType; }
            set { m_durationType = value; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public GameObjectAttributeModifierType ModiferType
        {
            get { return m_modifierType; }
            set { m_modifierType = value; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public float Duration
        {
            get { return m_duration; }
            set { m_duration  = value; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////


        private GameObjectAttributeType m_attributeType;
        private GameObjectAttributeModifierDurationType m_durationType;
        private GameObjectAttributeModifierType m_modifierType;
        private BaseActor m_appliedTo;
        private float m_duration;
        private float m_currentTime;
        private float m_modifier;
        private float m_originalValue;
        public const float INFINITE_DURATION = -1f;
    }


    public class BoundedAttribute
    {
        // object array ctor.
        public BoundedAttribute() { }

        public BoundedAttribute(float startValue) : this(startValue,false)
        {}
            

        public BoundedAttribute(float startValue,bool limitless)
        {
            MinValue = float.MinValue;
            MaxValue = float.MaxValue;
            BaseValue = startValue;
            CurrentValue = startValue;
            m_limitless = limitless;
        }

        public BoundedAttribute(float startValue,float minValue,float maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            BaseValue = maxValue;
            CurrentValue = startValue;
        }
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
            set 
            { 
                if(!Limitless)
                {
                    m_currentValue = MathHelper.Clamp(value,MinValue,MaxValue); 
                }
            }
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

        // this never changes after initial values set (handy for basic ammo)
        private bool m_limitless;
        public bool Limitless
        {
            get { return m_limitless; }
        }
    }

}
