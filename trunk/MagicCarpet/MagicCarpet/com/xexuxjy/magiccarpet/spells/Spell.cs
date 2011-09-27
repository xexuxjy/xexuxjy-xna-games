using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BulletXNA.BulletCollision;
using com.xexuxjy.magiccarpet.gameobjects;

namespace com.xexuxjy.magiccarpet.spells
{

    public enum SpellType
    {
        Castle,
        Convert,
        Fireball,
        Heal,
        LowerRaise,
        RubberBand,
        SwarmOfBees,
        Turbo
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////

    public class SpellTemplate
    {
        private int m_manaCost;

        public void Update(GameTime gameTime)
        {



        }

        public bool Available
        {
            get
            {
                return m_available;
            }
        }

        private bool m_available;


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
        private float m_lastCastTime;

        public float LastCastTime
        {
            get { return m_lastCastTime; }
            set { m_lastCastTime = value; }
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

    }
    //////////////////////////////////////////////////////////////////////////////////////////////////////

    public abstract class Spell : GameObject
    {
        public Spell(Game game)
            : base(game,GameObjectType.SPELL)
        {

        }

        public virtual void Initialize(SpellTemplate spellTemplate,GameObject owner)
        {
            m_spellTemplate = spellTemplate;
            Owner = owner;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public GameObject Owner
        {
            get { return m_owner; }
            set { m_owner = value; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Cleanup()
        {
            SpellComplete(this);
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

        public delegate void SpellCompleteHandler(Spell spell);
        public event SpellCompleteHandler SpellComplete;

        protected SpellTemplate m_spellTemplate;
        protected float m_currentTime;
    }
}
