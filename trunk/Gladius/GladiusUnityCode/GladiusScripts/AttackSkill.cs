using UnityEngine;
using System.Collections;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace Gladius
{
    public class AttackSkill
    {
        public const int m_defaultMoveToAttackRange = 5;

        public String Name;
        public String Type;
        public String SubType;
        public String UseClass;
        public String DisplayNameId;
        public String DescriptionId;
        public int SkillLevel;
        public float SkillCosts1;
        public float SkillCosts2;
        public int JobPointCost;
        public int CombatMod1;
        public float CombatMod2;
        public int SkillRange;
        public String SkillRangeName;
        public int SkillExcludeRange;
        public String SkillExcludeRangeName;

        public String MeterName;
        public String AnimName;
        public String FXName;
        public String SkillEffectName;
        public float SkillEffectModifier1;
        public float SkillEffectModifier2;
        public int SkillEffectRange;
        public String SkillEffectRangeName;
        public String SkillEffectCondition;

        public String Affinity;
        public int AffinityCost;
        public String TargetCondition;
		public String PreRequisite;

        public AnimationEnum Animation;

        public DamageType DamageType;

        public float AttackBonus = 1.0f;
        public float DefenseBonus = 1.0f;


        private List<String> m_targetConditions = new List<string>();
        private List<String> m_skillAttributes = new List<string>();

        public SkillStatus m_skillStatus1;
        public SkillStatus m_skillStatus2;

        public SkillEffect m_skillEffect = new SkillEffect();


        public bool ValidForTarget(BaseActor user,BaseActor target,Point targetPos)
        {
            foreach(String condition in m_targetConditions)
            {
                bool valid = CheckIndividualSkill(condition,user,target,targetPos);
                if(!valid)
                {
                    return false;
                }
            }
            return true;
        }


        private bool CheckIndividualSkill(String condition, BaseActor user, BaseActor target,Point targetPoint)
        {
            if ("any_square".Equals(condition))
            {
                return true;
            }
            if ("enemy_only".Equals(condition))
            {
                return target.TeamName != user.TeamName;
            }
            if ("self_only".Equals(condition))
            {
                return user == target;
            }
            if ("friend_only_not_self".Equals(condition))
            {
                return user != target && user.TeamName == target.TeamName;
            }
            if ("friend_only_not_self".Equals(condition))
            {
                return user != target && user.TeamName == target.TeamName;
            }
            if ("all_units_not_self".Equals(condition))
            {
                return user != target;
            }

            if ("empty_square".Equals(condition))
            {
            }



            if (condition.StartsWith("unitattr_req"))
            {
            }
            if (condition.StartsWith("unitattr_ign"))
            {
            }

            if (condition.StartsWith("target_has"))
            {
            }


            return false;
        }

        public void AddTargetCondition(String condition)
        {
            m_targetConditions.Add(condition);
        }

        public void AddAttribute(String attribute)
        {
            m_skillAttributes.Add(attribute);
        }

        public bool Available(BaseActor actor)
        {
            if (Name == "None")
            {
                return false;
            }

            // need to make sure actor has enough skillpoints or affinity points to use this.
            if (UseCost <= actor.ArenaSkillPoints)
            {
                return true;
            }
            return false;
        }

        public bool InRange(int val)
        {
            if (IsMoveToAttack && val < m_defaultMoveToAttackRange)
            {
                return true;
            }
            return val <= m_skillEffect.Range;
        }

        public int TotalSkillRange
        {
            get { return IsMoveToAttack ? m_skillEffect.Range : 1; }
        }

        public SkillIcon SkillIcon
        {
            get { return SkillIcon.Attack; }
        }

	
		public void Initialise()
		{
			SetupSkillRow();
            if (m_skillStatus1 != null)
            {
                m_skillStatus1.Initialise();
            }
            if (m_skillStatus2 != null)
            {
                m_skillStatus2.Initialise();
            }
		}
		
		private void SetupSkillRow()
		{
			if (SubType == "Combo")
			{
				m_skillRow = 1;
			}
            else if (m_skillAttributes.Contains("affinity"))
			{
				m_skillRow = 3;
			}
			else if(SubType == "Support")
			{
				m_skillRow = 4;
			}
            else if (Type == "Attack" && SubType == "Standard")
            {
                m_skillRow = 0;
            }
            else
            {
                m_skillRow = -1;
            }
		}
		

		private int m_skillRow;
        public int SkillRow
        {
            get
            {
            	return m_skillRow;
            }
            
        }

		public String GetPreRequisite()
		{
			return PreRequisite;
		}

        public int UseCost
        {
            get { return (int)SkillCosts2/10; }
        }

        public float DamageMultiplier
        {
            get { return 1.0f; }
        }

        public bool IsMoveToAttack
        {
            get { return m_skillAttributes.Contains("movetoattack"); }
        }

        public bool IsMultiHit
        {
            get { return m_skillAttributes.Contains("multihit"); }
        }

        public bool CantMiss
        {
            get { return m_skillAttributes.Contains("cantmiss"); }
        }

        public bool FaceOnAttack
        {
            get{return !m_skillAttributes.Contains("dontface");}
        }

        public bool IsAffinity
        {
            get { return !m_skillAttributes.Contains("affinity"); }
        }

        public bool IsBite
        {
            get { return !m_skillAttributes.Contains("bite"); }
        }

        public bool IsCharge
        {
            get { return !m_skillAttributes.Contains("charge"); }
        }

        public bool IsMelee
        {
            get { return !m_skillAttributes.Contains("melee"); }
        }

        public bool IsOkWithNoTargets
        {
            get { return !m_skillAttributes.Contains("okwithnotargets"); }
        }
        
        public bool IsPiercing
        {
            get { return !m_skillAttributes.Contains("piercing"); }
        }

        public bool IsRanged
        {
            get { return !m_skillAttributes.Contains("ranged"); }
        }
        
        public bool IsShield
        {
            get { return !m_skillAttributes.Contains("shield"); }
        }

        public bool IsSpearAnim
        {
            get { return !m_skillAttributes.Contains("spearanim"); }
        }

        public bool IsSpell
        {
            get { return !m_skillAttributes.Contains("spell"); }
        }

        public bool IsSuicide
        {
            get { return !m_skillAttributes.Contains("suicide"); }
        }

        public bool IsTeleport
        {
            get { return !m_skillAttributes.Contains("teleport"); }
        }

        public bool IsWeapon
        {
            get { return !m_skillAttributes.Contains("weapon"); }
        }

		public bool IsImmuneTo(String immunType)
		{
			bool result = false;
			if(m_skillStatus1 != null)
			{
				result = m_skillStatus1.ImmuneTo(immunType);
			}
			if(result == false && m_skillStatus2 != null)
			{
				result = m_skillStatus2.ImmuneTo(immunType);
			}
			return result;
		}

    }

    public class AttackSkillDictionary
    {
        public void Load(String path)
        {
            TextAsset textAsset = (TextAsset)Resources.Load("ExtractedData/SkillData");
            String data = textAsset.text;
            Parse(data);
            AttackSkill noneSkill = new AttackSkill();
            noneSkill.Name = "None";
            Data[noneSkill.Name] = noneSkill;
        }

        HashSet<String> skillTypeSet = new HashSet<String>();
        HashSet<String> skillTypeSet2 = new HashSet<String>();

        public void Parse(String data)
        {
            AttackSkill currentSkill = null;
            String[] lines = data.Split('\n');
            char[] splitTokens = new char[] { ',', ':' };
            for (int counter = 0; counter < lines.Length; counter++)
            {
                String line = lines[counter];
                if (!line.StartsWith("SKILL"))
                {
                    continue;
                }

                String[] lineTokens = GladiusGlobals.SplitAndTidyString(line,splitTokens);


                if (line.StartsWith("SKILLCREATE:"))
                {
                	if(currentSkill != null)
                	{
                		currentSkill.Initialise();
                	}
                    currentSkill = new AttackSkill();
                    if (lineTokens.Length == 4)
                    {
                        currentSkill.Name = lineTokens[1];
                        currentSkill.Type = lineTokens[2];
                        skillTypeSet.Add(currentSkill.Type);

                        currentSkill.SubType = lineTokens[3];
                        skillTypeSet2.Add(currentSkill.SubType);

                        Data[currentSkill.Name] = currentSkill;
                    }

                }
                else if (line.StartsWith("SKILLUSECLASS:"))
                {
                    currentSkill.UseClass = lineTokens[1];
                }
                else if (line.StartsWith("SKILLDISPLAYNAMEID:"))
                {
                    currentSkill.DisplayNameId = lineTokens[1];
                }
                else if (line.StartsWith("SKILLDESCRIPTIONID:"))
                {
                    currentSkill.DescriptionId = lineTokens[1];
                }
                else if (line.StartsWith("SKILLLEVEL:"))
                {
                    currentSkill.SkillLevel = int.Parse(lineTokens[1]);
                }
                else if (line.StartsWith("SKILLJOBPOINTCOST:"))
                {
                    currentSkill.JobPointCost = int.Parse(lineTokens[1]);
                }
                else if (line.StartsWith("SKILLCOSTS:"))
                {
                    currentSkill.SkillCosts1 = float.Parse(lineTokens[1]);
                    currentSkill.SkillCosts2 = float.Parse(lineTokens[2]);
                }
                else if (line.StartsWith("SKILLATTRIBUTE:"))
                {
                    currentSkill.AddAttribute(lineTokens[1]);
                }
                else if (line.StartsWith("SKILLCOMBATMODS:"))
                {
                    currentSkill.CombatMod1 = int.Parse(lineTokens[1]);
                    currentSkill.CombatMod2 = float.Parse(lineTokens[2]);
                }
                else if (line.StartsWith("SKILLRANGE:"))
                {
                    currentSkill.SkillRange = int.Parse(lineTokens[1]);
                    currentSkill.SkillRangeName = lineTokens[2];
                }
                else if (line.StartsWith("SKILLEXCLUDERANGE:"))
                {
                    currentSkill.SkillExcludeRange = int.Parse(lineTokens[1]);
                    currentSkill.SkillExcludeRangeName = lineTokens[2];

                }
                else if (line.StartsWith("SKILLAFFCOST:"))
                {
                    currentSkill.AffinityCost = int.Parse(lineTokens[1]);
                }
                
                else if (line.StartsWith("SKILLMETER:"))
                {
                    currentSkill.MeterName = lineTokens[1];
                }
                else if (line.StartsWith("SKILLANIMSPEED:"))
                {

                }
                else if (line.StartsWith("SKILLFXSWING:"))
                {

                }
                else if (line.StartsWith("SKILLPREREQ:"))
                {
                	currentSkill.PreRequisite = lineTokens[1];
                }
                else if (line.StartsWith("SKILLEFFECTRANGE:"))
                {
                    currentSkill.m_skillEffect.Range = int.Parse(lineTokens[1]);
                    //currentSkill.m_skillEffect.EffectRangeType = (SkillEffectRangeType)Enum.Parse(typeof(SkillEffectRangeType),lineTokens[2]);
                }
                else if (line.StartsWith("SKILLEFFECT:"))
                {
                    currentSkill.SkillEffectName = lineTokens[1];
                    currentSkill.SkillEffectModifier1 = float.Parse(lineTokens[2]);
                    currentSkill.SkillEffectModifier2 = float.Parse(lineTokens[3]);
                }
                else if (line.StartsWith("SKILLEFFECTCONDITION:"))
                {
                    currentSkill.SkillEffectCondition = lineTokens[1];
                }
                else if (line.Contains("SKILLSTATUS"))
                {
                    HandleSkillStatus(currentSkill, lineTokens[0], lineTokens);
                }
                else if (line.StartsWith("SKILLANIM:"))
                {
                    // can't use these directly.
                }
                else if (line.StartsWith("SKILLFX:"))
                {

                }
                else if (line.StartsWith("SKILLPROJECTILE:"))
                {

                }
                else if (line.StartsWith("SKILLPROJECTILESEQUENCE:"))
                {

                }
                else if (line.StartsWith("SKILLFXPROJECTILE:"))
                {

                }
                else if (line.StartsWith("SKILLAFFINITY:"))
                {
                    currentSkill.Affinity = lineTokens[1];
                }
                else if (line.StartsWith("SKILLTARGETCONDITION:"))
                {
                    currentSkill.AddTargetCondition(lineTokens[1]);

                }

                else
                {

                    //err
                    //Debug.LogError("Unknown token : "+line);
                }
            }
            int ibreak = 0;
        }

        public void HandleSkillStatus(AttackSkill skill,String attrName,String[] lineTokens)
        {
            SkillStatus status=null;
            if(attrName.EndsWith("2:"))
            {
                if(skill.m_skillStatus2 == null)
                {
                    skill.m_skillStatus2 = new SkillStatus();
                }
                status = skill.m_skillStatus2;
            }
            else
            {
                if(skill.m_skillStatus1 == null)
                {
                    skill.m_skillStatus1 = new SkillStatus();
                }
                status = skill.m_skillStatus1;
            }


            if (attrName.Contains("SKILLSTATUSDURATION"))
            {
                status.statusDurationType = lineTokens[1];
                status.statusDuration = float.Parse(lineTokens[2]);
            }
            else if (attrName.Contains("SKILLSTATUSTARGET"))
            {
                status.statusTargetName= lineTokens[1];
                status.statusTargetRange = int.Parse(lineTokens[2]);
                status.statusTargetShape = lineTokens[3];


            }
            else if (attrName.Contains("SKILLSTATUSCONDITION"))
            {
                status.skillStatusCondition.Add(lineTokens[1]);

            }
            else if (attrName.Contains("SKILLSTATUSINTERVAL"))
            {
                status.statusIntervalName = lineTokens[1];
                status.statusIntervalDuration = int.Parse(lineTokens[2]);

            }
            else if (attrName.Contains("SKILLSTATUSCHANCE"))
            {
                status.statusChance = int.Parse(lineTokens[1]);
            }
            else if (attrName.Contains("STATUSSITUATIONUNITCONDITION"))
            {
                status.skillStatusSituationUnitCondition.Add(lineTokens[1]);
            }
            else if (attrName.Contains("STATUSSITUATIONAFFINITYCONDITION"))
            {
                //status.skillStatusSituationUnitCondition.Add(lineTokens[1]);
            }
            else if (attrName.Contains("SKILLSTATUSSITUATIONSKILLCONDITION"))
            {
                //status.skillStatusSituationUnitCondition.Add(lineTokens[1]);
            }
            else if (attrName.Contains("SKILLSTATUSSITUATIONSTATUSCONDITION"))
            {
            }
            else if (attrName.Contains("SKILLSTATUSUSELIMIT"))
            {
            }
            else if (attrName.Contains("SKILLSTATUS"))
            {
                // FIXME - something needs to happen to parse the status names as they contrain rules/modifications
                status.statusName = lineTokens[1];
                status.statusVal1 = float.Parse(lineTokens[2]);
                status.statusVal2 = float.Parse(lineTokens[3]);
            }
            else
            {
                int ibreak = 0;
            }

        }
        
        public Dictionary<String, AttackSkill> Data = new Dictionary<String, AttackSkill>();
    }


    public class SkillEffect
    {
        public String EffectType;
        public float Value1;
        public float Value2;
        public int Range;
        public SkillEffectRangeType EffectRangeType;
    }

    public class SkillStatus
    {
        public String statusName;
        public float statusVal1;
        public float statusVal2;
        public String statusDurationType;
        public float statusDuration;
        public String statusTargetName;
        public int statusTargetRange;
        public String statusTargetShape;
        public List<String> skillStatusCondition = new List<String>();
        public List<String> skillStatusSituationUnitCondition = new List<String>();

        public String statusSituationAffinityCondition;
        public int statusChance;
        public String statusIntervalName;
        public int statusIntervalDuration;
  
    
    	public bool CombatDefend;
    
    
    	public bool IsImmunityStatus()
    	{
    		return "immunity status" == statusName;
    	}
    	
    	public bool ImmuneTo(String immunType)
    	{
    		return IsImmunityStatus() && skillStatusCondition.Contains(immunType);
    	}
    	

		public void Initialise()
		{
		
		}





    
    
    }


	public class Resistance
	{
		String type;
		float value;
	}





}
