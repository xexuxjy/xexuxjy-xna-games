using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BulletXNA.BulletCollision;
using com.xexuxjy.magiccarpet.gameobjects;
using System.Diagnostics;

namespace com.xexuxjy.magiccarpet.spells
{

    public enum SpellType
    {
        None,
        Castle,
        Convert,
        Fireball,
        Lower,
        Raise,
        Heal,
        RubberBand,
        SwarmOfBees,
        Turbo,
        NumSpellTypes
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////

    public class SpellTemplate 
    {

        public SpellTemplate(SpellType spellType,int manaCost, float castTime, float cooldownTime, float duration)
        {
            m_spellType = spellType;
            m_manaCost = manaCost;
            m_castTime = castTime;
            m_cooldownTime = cooldownTime;
            m_duration = duration;
            State = SpellTemplateState.Available;
        }


        public void Cast(Spell spell)
        {
            Debug.Assert(Spell == null);
            Debug.Assert(State == SpellTemplateState.Available);

            Spell = spell;
            State  = SpellTemplateState.Casting;
        }

        public void Update(GameTime gameTime)
        {

            if (State == SpellTemplateState.Casting || State == SpellTemplateState.Cooldown)
            {
                m_currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            
            if (State  == SpellTemplateState.Casting)
            {
                // spell cast time complete , start spell and allow cooldown.
                if (m_currentTime > m_castTime)
                {
                    Spell.Start();
                    Spell = null;
                    if (m_cooldownTime > 0f)
                    {
                        State = SpellTemplateState.Cooldown;
                    }
                    else
                    {
                        State = SpellTemplateState.Available;
                    }
                }
            }
            else if (State == SpellTemplateState.Cooldown)
            {
                if (m_currentTime > m_cooldownTime)
                {
                    State = SpellTemplateState.Available;
                }
            }
        }

        public float CoolDownPercentage()
        {
            float result = 0f;;
            if (State == SpellTemplateState.Cooldown)
            {
                if(m_cooldownTime > 0)
                {
                    result = MathHelper.Clamp((m_currentTime / m_cooldownTime),0,1f);
                }
            }
            return result;
        }


        public bool Available
        {
            get
            {
                return State == SpellTemplateState.Available;
            }
        }

        private int m_manaCost;

        public int ManaCost
        {
            get { return m_manaCost; }
            set { m_manaCost = value; }
        }
        private float m_cooldownTime;

        public float CooldownTime
        {
            get { return m_cooldownTime; }
            set { m_cooldownTime = value; }
        }
        private float m_castTime;

        public float CastTime
        {
            get { return m_castTime; }
            set { m_castTime = value; }
        }

        private float m_duration;
        public float Duration
        {
            get { return m_duration; }
            set { m_duration = value; }
        }

        private SpellType m_spellType;

        public SpellType SpellType
        {
          get { return m_spellType; }
          set { m_spellType = value; }
        }

        public enum SpellTemplateState
        {
            Available,
            Casting,
            Cooldown
        }

        public Spell Spell
        {
            get { return m_spellToCast; }
            set { m_spellToCast = value; }
        }


        public SpellTemplateState State
        {
            get { return m_spellTemplateState; }
            set
            {
                m_spellTemplateState = value;
                // reset time on state change.
                m_currentTime = 0f;
            }
        }

        private Spell m_spellToCast;
        private SpellTemplateState m_spellTemplateState;
        private float m_currentTime;

    }
    //////////////////////////////////////////////////////////////////////////////////////////////////////

    public abstract class Spell : GameObject
    {
        public Spell(GameObject owner)
            : base(GameObjectType.spell)
        {
            Owner = owner;
        }

        public virtual void Initialize(SpellTemplate spellTemplate)
        {
            base.Initialize();
            m_spellTemplate = spellTemplate;
            m_scaleTransform = Matrix.CreateScale(s_objectSize);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        // called by the spell template when the cast time is complete.
        public void Start()
        {


        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public GameObject Owner
        {
            get { return m_owner; }
            set { m_owner = value; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void  Update(GameTime gameTime)
        {
            base.Update(gameTime);
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            m_currentTime += elapsedSeconds;
            if (Complete())
            {
                Cleanup();
            }

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool Complete()
        {
            return m_currentTime >= m_spellTemplate.Duration;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Cleanup()
        {
            if (!m_awaitingRemoval)
            {
#if LOG_EVENT
                Globals.EventLogger.LogEvent(String.Format("SpellComplete[{0}][{1}][{2}].", Id, m_owner.Id, SpellType));
#endif

                SpellComplete(this);
                base.Cleanup();
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public SpellType SpellType
        {
            get { return m_spellTemplate.SpellType; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public SpellTemplate SpellTemplate
        {
            get { return m_spellTemplate; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public const float s_objectSize = 0.2f;

        public delegate void SpellCompleteHandler(Spell spell);
        public event SpellCompleteHandler SpellComplete;

        protected SpellTemplate m_spellTemplate;
        protected float m_currentTime;
    }
}
