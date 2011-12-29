using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;

namespace com.xexuxjy.magiccarpet.actions
{
    public class ActionFind : BaseAction
    {
        public ActionFind(FindData findData)
            : base(findData.m_owner, findData.m_target, ActionState.Searching)
        {
            m_findData = findData;
            Duration = 1.0f;
        }

        // new clever version that lets us find a number of objects and give weights to the search?
        protected override void InternalComplete()
        {
            List<GameObject> searchResults = new List<GameObject>();
            Globals.GameObjectManager.FindObjectsForOwner(m_findData.m_findMask, Owner.Position, m_findData.m_findRadius, m_findData.m_owner, searchResults);
            if (searchResults.Count > 0)
            {
                int index = 0;
                float heighestWeight = 0;
                for (int i = 0; i < searchResults.Count; ++i)
                {
                    float currentWeight = m_findData.GetWeightForType(searchResults[i].GameObjectType);
                    if (currentWeight > heighestWeight)
                    {
                        heighestWeight = currentWeight;
                        index = i;
                    }
                }
                Target = searchResults[index];
            }
        }
        public FindData m_findData;
    }
}
