using System;
using com.xexuxjy.magiccarpet.spells;
using com.xexuxjy.magiccarpet.gameobjects;

namespace com.xexuxjy.magiccarpet.manager
{
    public class SpellPool
    {

        public Spell CreateAndInitializeSpell(SpellType spellType, GameObject owner)
        {

                Spell spell = null;
                switch (spellType)
                {
                    case (SpellType.Castle):
                        {
                            spell = new SpellCastle(owner);
                            break;
                        }

                    case (SpellType.Convert):
                        {
                            spell = new SpellConvert(owner);
                            break;
                        }
                    case (SpellType.Fireball):
                        {
                            spell = new SpellFireball(owner);
                            break;
                        }
                    case (SpellType.Heal):
                        {
                            spell = new SpellHeal(owner);
                            break;
                        }
                    case (SpellType.Lower):
                        {
                            spell = new SpellAlterTerrain(owner, false);
                            break;
                        }
                    case (SpellType.Raise):
                        {
                            spell = new SpellAlterTerrain(owner, true);
                            break;
                        }
                    case (SpellType.RubberBand):
                        {
                            spell = new SpellRubberband(owner);
                            break;
                        }
                    case (SpellType.SwarmOfBees):
                        {
                            spell = new SpellSwarmOfBees(owner);
                            break;
                        }
                    case (SpellType.Turbo):
                        {
                            spell = new SpellTurbo(owner);
                            break;
                        }
                    case (SpellType.Shield):
                        {
                            spell = new SpellShield(owner);
                            break;
                        }
                }
                spell.Initialize(owner.SpellComponent.GetSpellTemplate(spellType));
#if LOG_EVENT
                Globals.EventLogger.LogEvent(String.Format("CreateSpell[{0}][{1}][{2}].", spell.Id, owner.Id, spell.SpellType));
#endif


                return spell;
        }


        public void ReleaseSpell(Spell spell)
        {

#if LOG_EVENT
            Globals.EventLogger.LogEvent(String.Format("ReleaseSpell[{0}][{1}][{2}].", spell.Id, spell.Owner.Id, spell.SpellType));
#endif

        }

    }
}
