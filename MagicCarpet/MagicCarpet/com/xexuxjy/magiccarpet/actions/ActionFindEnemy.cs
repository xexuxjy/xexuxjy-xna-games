using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;

namespace com.xexuxjy.magiccarpet.actions
{
    public class ActionFindEnemy : BaseAction
    {
        public ActionFindEnemy(GameObject owner, float searchRadius)
            : base(owner, null, ActionState.Searching)
        {
            Duration = 1.0f;
            m_searchRadius = searchRadius;
        }

        protected override void InternalComplete()
        {
            List<GameObject> searchResults = new List<GameObject>();
            GameObjectType searchMask = 0;

            if (Owner is Monster)
            {
                searchMask = GameObjectType.magician | GameObjectType.balloon | GameObjectType.castle;
            }
            else if (Owner is Magician)
            {
                searchMask = GameObjectType.magician | GameObjectType.balloon | GameObjectType.castle | GameObjectType.monster;
            }
          
            Globals.GameObjectManager.FindObjects(searchMask, Owner.Position, m_searchRadius, Owner, searchResults);
            if (searchResults.Count > 0)
            {
                Target = searchResults[0];
            }
        
        }
        private float m_searchRadius;


    }
}
