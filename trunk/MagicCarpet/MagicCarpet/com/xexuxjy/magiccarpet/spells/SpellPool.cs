using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.gameobjects;

namespace com.xexuxjy.magiccarpet.spells
{
    public class SpellPool : IUpdateable
    {
        public SpellPool(GameObject owner)
        {
            m_owner = owner;

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Initialize()
        {
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void Update(GameTime gameTime)
        {
            foreach (Spell spell in m_activeSpells)
            {
                spell.Update(gameTime);
            }


        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void CastSpell(SpellType spellType, Vector3 startPosition, Vector3 targetPosition)
        {
            if (CanCastSpell(spellType))
            {
                SpellTemplate template;
                m_spellTemplates.TryGetValue(spellType, out template);
                GameObjectAttribute mana = m_owner.GetAttribute(GameObjectAttributeType.Mana);
                mana.CurrentValue -= template.ManaCost;
                Spell spell = null;
                switch (spellType)
                {
                    case (SpellType.Castle):
                        {
                            spell = new SpellCastle(m_owner);
                            break;
                        }

                    case (SpellType.Convert):
                        {
                            spell = new SpellConvert(m_owner);
                            break;
                        }
                    case (SpellType.Fireball):
                        {
                            spell = new SpellFireball(m_owner);
                            break;
                        }
                    case (SpellType.Heal):
                        {
                            spell = new SpellHeal(m_owner);
                            break;
                        }
                    case (SpellType.LowerRaise):
                        {
                            spell = new SpellAlterTerrain(m_owner);
                            break;
                        }
                    case (SpellType.RubberBand):
                        {
                            spell = new SpellRubberband(m_owner);
                            break;
                        }
                    case (SpellType.SwarmOfBees):
                        {
                            spell = new SpellSwarmOfBees(m_owner);
                            break;
                        }
                    case (SpellType.Turbo):
                        {
                            spell = new SpellTurbo(m_owner);
                            break;
                        }
                }

                spell.Initialize(template);
                if (spell is MovingSpell)
                {
                    Vector3 direction = targetPosition - startPosition;
                    direction.Normalize();
                    ((MovingSpell)spell).SetInitialPositionAndDirection(startPosition, direction);
                }


                m_activeSpells.Add(spell);
                spell.SpellComplete += new Spell.SpellCompleteHandler(spell_SpellComplete);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        void spell_SpellComplete(Spell spell)
        {
            SpellTemplate spellTemplate = spell.SpellTemplate;

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



        private GameObject m_owner;
        private List<Spell> m_activeSpells = new List<Spell>();
        private Dictionary<SpellType, SpellTemplate> m_spellTemplates = new Dictionary<SpellType, SpellTemplate>();

        public delegate void SpellCastHandler(GameObject gameObject, Spell spell);
        public event SpellCastHandler SpellCast;




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
