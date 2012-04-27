using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.gameobjects;

namespace com.xexuxjy.magiccarpet.spells
{
    public class SpellComponent : IUpdateable
    {
        public SpellComponent(GameObject owner)
        {

            m_owner = owner;
            // manaCost,castTime,cooldownTime,duration
            InitializeTemplate(SpellType.Convert, 0, 0.1f, 0f, 10f);
            InitializeTemplate(SpellType.Lower, 5, 0.5f, 1f, 10f);
            InitializeTemplate(SpellType.Raise, 5, 0.5f, 1f, 10f);
            InitializeTemplate(SpellType.Castle, 5, 0.5f, 1f, 10f);
            InitializeTemplate(SpellType.Fireball, 5, 0.5f, 1f, 10f);
            InitializeTemplate(SpellType.Heal, 5, 0.5f, 1f, 10f);
            InitializeTemplate(SpellType.Turbo, 5, 0.5f, 1f, 10f);
            InitializeTemplate(SpellType.RubberBand, 5, 0.5f, 1f, 10f);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public float CastTime(SpellType spellType)
        {
            return m_spellTemplates[spellType].CastTime;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private void InitializeTemplate(SpellType spellType, int manaCost, float castTime, float cooldownTime, float duration)
        {
            SpellTemplate template = new SpellTemplate(spellType, manaCost, castTime, cooldownTime, duration);
            m_spellTemplates[spellType] = template;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////



        public void Initialize()
        {
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void Update(GameTime gameTime)
        {
            foreach (SpellTemplate spellTemplate in m_spellTemplates.Values)
            {
                spellTemplate.Update(gameTime);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void CastSpell(SpellType spellType, Vector3 startPosition, Vector3 direction)
        {
            if (CanCastSpell(spellType))
            {
                SpellTemplate template;
                m_spellTemplates.TryGetValue(spellType, out template);
                GameObjectAttribute mana = m_owner.GetAttribute(GameObjectAttributeType.Mana);
                mana.CurrentValue -= template.ManaCost;

                Spell spell = Globals.SpellPool.CreateAndInitializeSpell(spellType, m_owner);

                Globals.GameObjectManager.AddAndInitializeObject(spell,false);

                //spell.Initialize(template);
                
                // why is this needed now??
                template.Cast(spell);

                if (spell is MovingSpell)
                {
                    ((MovingSpell)spell).SetInitialPositionAndDirection(startPosition, direction);
                }

                spell.SpellComplete += new Spell.SpellCompleteHandler(spell_SpellComplete);
                m_activeSpells.Add(spell);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public SpellTemplate GetSpellTemplate(SpellType spellType)
        {
            SpellTemplate template;
            m_spellTemplates.TryGetValue(spellType, out template);
            return template;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        void spell_SpellComplete(Spell spell)
        {
            m_activeSpells.Remove(spell);
            SpellTemplate spellTemplate = spell.SpellTemplate;
            Globals.SpellPool.ReleaseSpell(spell);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool CanCastSpell(SpellType spellType)
        {
            bool result = false;
            SpellTemplate template;
            m_spellTemplates.TryGetValue(spellType, out template);
            if (template != null && template.Available)
            {
                GameObjectAttribute mana = m_owner.GetAttribute(GameObjectAttributeType.Mana);
                if (mana.CurrentValue > template.ManaCost)
                {
                    result = true;
                }
            }
            return result;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        // allow gui or whatever to draw active spells. don't store any references to the spells though.
        public List<Spell> GetActiveSpells()
        {
            return m_activeSpells;
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private GameObject m_owner;
        private List<Spell> m_activeSpells = new List<Spell>();
        private Dictionary<SpellType, SpellTemplate> m_spellTemplates = new Dictionary<SpellType, SpellTemplate>();

        public delegate void SpellCastHandler(GameObject gameObject, Spell spell);
        public event SpellCastHandler SpellCast;

        private const float InfiniteMana = -99f;


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
}
