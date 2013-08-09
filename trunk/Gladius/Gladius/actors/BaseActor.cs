using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xexuxjy.Gladius.util;
using Microsoft.Xna.Framework;
using Gladius.combat;
using Gladius.actions;

namespace Gladius.actors
{
    public class BaseActor
    {
        public String Name
        {
            get;
            set;
        }

        public String DebugName
        {
            get;
            set;
        }

        public float GetAttributeValue(GameObjectAttributeType attributeType)
        {
            return m_attributeDictionary[attributeType].CurrentValue;
        }

        public void SetAttributeValue(GameObjectAttributeType attributeType,float val)
        {
            m_attributeDictionary[attributeType].CurrentValue = val;
        }


        public Point CurrentPoint
        {
            get;
            set;
        }


        public Vector3 Position
        {
            get;
            set;
        }

        public Matrix World
        {
            get;
            set;
        }

        public BoundingBox BoundingBox
        {
            get;
            set;
        }

        public String Team
        {
            get;
            set;
        }

        public ActorClass ActorClass
        {
            get;
            set;
        }

        public void TakeDamage(AttackResult attackResult)
        {
            if(attackResult.resultType != AttackResultType.Miss)
            {
                m_attributeDictionary[GameObjectAttributeType.Health].CurrentValue -= attackResult.damageDone;
            }
        }


        public virtual void Update(GameTime gameTime)
        {


        }

        public virtual void CheckState()
        {
            if(m_attributeDictionary[GameObjectAttributeType.Health].CurrentValue <= 0f)
            {
                StartAction(ActionTypes.Death);
            }
        }

        public virtual void StartAction(ActionTypes actionType)
        {
            Globals.EventLogger.LogEvent(EventTypes.Action, String.Format("[{0}] [{1}] started.", DebugName, actionType));
        }

        public virtual void StopAction(ActionTypes actionType)
        {
            Globals.EventLogger.LogEvent(EventTypes.Action, String.Format("[{0}] [{1}] stopped.", DebugName, actionType));
        }


        public List<AttackSkill> m_knownAttacks;
        public Dictionary<GameObjectAttributeType,BoundedAttribute> m_attributeDictionary;
    }

    public enum ActorClass
    {
        Light,
        Medium,
        Heavy
    }

}
