using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;

namespace com.xexuxjy.magiccarpet.actions
{
    public class ActionFindCastle : BaseAction
    {
        public ActionFindCastle(GameObject owner, GameObject target, float searchRadius)
            : base(owner, target, ActionState.Searching)
        {
            Duration = 1.0f;
            m_searchRadius = searchRadius;
        }

        protected override void InternalComplete()
        {
            List<GameObject> searchResults = new List<GameObject>();
            GameObjectType searchMask = GameObjectType.castle;
            Globals.GameObjectManager.FindObjects(searchMask, Owner.Position, m_searchRadius, Owner, searchResults);
            if (searchResults.Count > 0)
            {
                Target = searchResults[0];
            }
        }
        private float m_searchRadius;

    }
}
