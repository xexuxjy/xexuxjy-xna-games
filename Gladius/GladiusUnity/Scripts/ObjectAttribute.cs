//using UnityEngine;
//namespace Gladius
//{
//    public class GameObjectAttributeModifier
//    {

//        public GameObjectAttributeModifier()
//        {

//        }

//        //////////////////////////////////////////////////////////////////////////////////////////////////////

//        public GameObjectAttributeModifier Copy()
//        {
//            return new GameObjectAttributeModifier(this.AttributeType, this.DurationType, this.ModiferType, this.m_modifier, this.m_duration);
//        }

//        public GameObjectAttributeModifier(GameObjectAttributeType attributeType, int modifier)
//        {
//            m_attributeType = attributeType;
//            ModiferType = GameObjectAttributeModifierType.Add;
//            DurationType = GameObjectAttributeModifierDurationType.InstantTemporary;
//            m_duration = 1;
//            m_currentTime = 0;
//            m_modifier = modifier;
//        }


//        public GameObjectAttributeModifier(GameObjectAttributeType attributeType,
//            GameObjectAttributeModifierDurationType durationType,
//            GameObjectAttributeModifierType modifierType, int modifier, int duration)
//        {
//            m_attributeType = attributeType;
//            ModiferType = modifierType;
//            DurationType = durationType;
//            m_duration = duration;
//            m_currentTime = 0;
//            m_modifier = modifier;
//        }

//        //////////////////////////////////////////////////////////////////////////////////////////////////////

//        public void Initialize(GameObjectAttributeType attributeType, BaseActor appliedTo,
//            GameObjectAttributeModifierDurationType durationType,
//            GameObjectAttributeModifierType modifierType, int modifier, int duration)
//        {
//            m_appliedTo = appliedTo;
//            m_attributeType = attributeType;
//            ModiferType = modifierType;
//            DurationType = durationType;
//            m_duration = duration;
//            m_currentTime = 0;
//            m_modifier = modifier;
//            ApplyTo(m_appliedTo);
//        }

//        //////////////////////////////////////////////////////////////////////////////////////////////////////

//        public void ApplyTo(BaseActor appliedTo)
//        {
//            if (appliedTo != null)
//            {
//                m_appliedTo = appliedTo;
//                m_originalValue = m_appliedTo.GetAttributeValue(m_attributeType);
//            }


//            if (DurationType == GameObjectAttributeModifierDurationType.InstantTemporary ||
//                DurationType == GameObjectAttributeModifierDurationType.OverTimeTemporary)
//            {

//                int newValue = m_originalValue;
//                if (ModiferType == GameObjectAttributeModifierType.Add)
//                {
//                    newValue += m_modifier;
//                }
//                else
//                {
//                    newValue *= m_modifier;
//                }
//                if (m_appliedTo != null)
//                {
//                    m_appliedTo.SetAttributeValue(m_attributeType, newValue);
//                }
//            }

//        }

//        //////////////////////////////////////////////////////////////////////////////////////////////////////

//        public void Cleanup()
//        {
//            // If the effect is temporary then restore the original version.
//            if (m_durationType == GameObjectAttributeModifierDurationType.InstantTemporary ||
//                m_durationType == GameObjectAttributeModifierDurationType.OverTimeTemporary)
//            {
//                if (m_appliedTo != null)
//                {
//                    m_appliedTo.SetAttributeValue(m_attributeType, m_originalValue);
//                }
//            }
//            else
//            {
//            }
//        }

//        //////////////////////////////////////////////////////////////////////////////////////////////////////

//        public void Update()
//        {
//            //float elapsedSeconds = Time.deltaTime;
//            //m_currentTime+= elapsedSeconds;
//            // Time is done on a per turn basis now , not 'realtime'
//            m_currentTime++;
//            if (!Complete)
//            {
//                int newValue = m_originalValue;
//                if (m_modifierType == GameObjectAttributeModifierType.Add)
//                {
//                    newValue += m_modifier;
//                }
//                else
//                {
//                    newValue *= m_modifier;
//                }
//                if (m_appliedTo != null)
//                {
//                    m_appliedTo.SetAttributeValue(m_attributeType, newValue);
//                }
//            }
//            else
//            {
//                Cleanup();
//            }

//        }

//        //////////////////////////////////////////////////////////////////////////////////////////////////////

//        public bool Complete
//        {
//            get
//            {
//                if (m_duration == INFINITE_DURATION)
//                {
//                    return false;
//                }
//                return (m_currentTime >= m_duration);
//            }
//        }

//        //////////////////////////////////////////////////////////////////////////////////////////////////////

//        public GameObjectAttributeType AttributeType
//        {
//            get { return m_attributeType; }
//            set { m_attributeType = value; }
//        }

//        //////////////////////////////////////////////////////////////////////////////////////////////////////

//        public GameObjectAttributeModifierDurationType DurationType
//        {
//            get { return m_durationType; }
//            set { m_durationType = value; }
//        }

//        //////////////////////////////////////////////////////////////////////////////////////////////////////

//        public GameObjectAttributeModifierType ModiferType
//        {
//            get { return m_modifierType; }
//            set { m_modifierType = value; }
//        }

//        //////////////////////////////////////////////////////////////////////////////////////////////////////

//        public int Duration
//        {
//            get { return m_duration; }
//            set { m_duration = value; }
//        }

//        //////////////////////////////////////////////////////////////////////////////////////////////////////


//        private GameObjectAttributeType m_attributeType;
//        private GameObjectAttributeModifierDurationType m_durationType;
//        private GameObjectAttributeModifierType m_modifierType;
//        private BaseActor m_appliedTo;
//        private int m_duration;
//        private int m_currentTime;
//        private int m_modifier;
//        private int m_originalValue;
//        public const int INFINITE_DURATION = -1;
//    }


//    public class BoundedAttribute
//    {
//        // object array ctor.
//        public BoundedAttribute() { }

//        public BoundedAttribute(int startValue)
//            : this(startValue, false)
//        { }


//        public BoundedAttribute(int startValue, bool limitless)
//        {
//            MinValue = 0;
//            MaxValue = startValue;
//            BaseValue = startValue;
//            CurrentValue = startValue;
//            m_limitless = limitless;
//        }

//        public BoundedAttribute(int startValue, int minValue, int maxValue)
//        {
//            MinValue = minValue;
//            MaxValue = maxValue;
//            BaseValue = maxValue;
//            CurrentValue = startValue;
//        }
//        private int m_baseValue;

//        public int BaseValue
//        {
//            get { return m_baseValue; }
//            set { m_baseValue = value; }
//        }
//        private int m_currentValue;

//        public int CurrentValue
//        {
//            get { return m_currentValue; }
//            set
//            {
//                if (!Limitless)
//                {
//                    m_currentValue = GladiusGlobals.Clamp(value, MinValue, MaxValue);
//                }
//            }
//        }
//        private int m_maxValue;

//        public int MaxValue
//        {
//            get { return m_maxValue; }
//            set { m_maxValue = value; }
//        }
//        private int m_minValue;

//        public int MinValue
//        {
//            get { return m_minValue; }
//            set { m_minValue = value; }
//        }

//        // this never changes after initial values set (handy for basic ammo)
//        private bool m_limitless;
//        public bool Limitless
//        {
//            get { return m_limitless; }
//        }
//    }
//}