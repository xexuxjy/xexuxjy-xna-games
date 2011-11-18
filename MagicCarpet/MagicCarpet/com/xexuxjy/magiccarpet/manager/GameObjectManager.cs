using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.gameobjects;

using GameStateManagement;
using com.xexuxjy.magiccarpet.terrain;

namespace com.xexuxjy.magiccarpet.manager
{
    public class GameObjectManager : DrawableGameComponent
    {
        public GameObjectManager(GameplayScreen gameplayScreen)
            : base(Globals.Game)
        {
            m_gameplayScreen = gameplayScreen;
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////
        
        public GameObject CreateAndInitialiseGameObject(GameObjectType gameObjectType, Vector3 startPosition)
        {
            GameObject gameObject = null;
            switch (gameObjectType)
            {
                case GameObjectType.manaball:
                    {
                        gameObject = new ManaBall(startPosition);
                        break;
                    }
                case GameObjectType.balloon:
                    {
                        gameObject = new Balloon(startPosition);
                        break;
                    }
                case GameObjectType.castle:
                    {
                        gameObject = new Castle(startPosition);
                        break;
                    }
                case GameObjectType.magician:
                    {
                        gameObject = new Magician(startPosition);
                        break;
                    }
                case GameObjectType.terrain:
                    {
                        gameObject = new Terrain(startPosition);
                        break;
                    }

            }

            if (gameObject != null)
            {
                gameObject.Initialize();
                m_gameObjectListAdd.Add(gameObject);
            }
            return gameObject;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void RemoveGameObject(GameObject gameObject)
        {
            m_gameObjectListRemove.Add(gameObject);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Update(GameTime gameTime)
        {
            foreach (GameObject gameObject in m_gameObjectListAdd)
            {
                m_gameObjectList.Add(gameObject);
            }
            m_gameObjectListAdd.Clear();

            foreach (GameObject gameObject in m_gameObjectList)
            {
                gameObject.Update(gameTime);
            }

            foreach (GameObject gameObject in m_gameObjectListRemove)
            {
                // cleanup may already have been called.
                gameObject.Cleanup();
                Globals.CollisionManager.RemoveFromWorld(gameObject.CollisionObject);
                m_gameObjectList.Remove(gameObject);
            }
            m_gameObjectListRemove.Clear();
    
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void FindObjects(GameObjectType typeMask, List<GameObject> results)
        {
            foreach (GameObject gameObject in m_gameObjectList)
            {
                if( typeMask == GameObjectType.none || typeMask == gameObject.GameObjectType)
                {
                    results.Add(gameObject);
                }
            }

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////


        public void FindObjects(GameObjectType typeMask, Vector3 position, float radius, List<GameObject> results)
        {
            FindObjects(typeMask, position, radius, null, results);
        }

        public void FindObjects(GameObjectType typeMask, Vector3 position, float radius, GameObject owner, List<GameObject> results)
        {
            float radiusSq = radius * radius;
            foreach (GameObject gameObject in m_gameObjectList)
            {
                // if it's the sort of object we're interested in
                GameObjectType gameObjectType = gameObject.GameObjectType;
                if ((gameObjectType & typeMask) > 0)
                {
                    // check and see if it's the owning entity if appropriate
                    if (owner == null || (gameObject.Owner == owner))
                    {
                        if ((gameObject.Position - position).LengthSquared() <= radiusSq)
                        {
                            results.Add(gameObject);
                        }
                    }
                }
            }
            results.Sort(new DistanceSorter(position));
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void FindObjects(GameObjectType typeMask, BoundingBox boundingBox, GameObject owner, List<GameObject> results)
        {
            foreach (GameObject gameObject in m_gameObjectList)
            {
                // if it's the sort of object we're interested in
                GameObjectType gameObjectType = gameObject.GameObjectType;
                if ((gameObjectType & typeMask) > 0)
                {
                    // check and see if it's the owning entity if appropriate
                    if (owner == null || (gameObject.Owner == owner))
                    {
                        // if we contain part of it then include.
                        if(boundingBox.Contains(owner.BoundingBox) != ContainmentType.Disjoint)
                        {
                            results.Add(gameObject);
                        }
                    }
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool ObjectAvailable(GameObject target)
        {
            return true;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        
        public IList<GameObject> DebugObjectList
        {
            get { return m_gameObjectList; }
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public GameObject GetObject(String id)
        {
            return null;
        }
        
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        
        private Dictionary<String, GameObject> m_gameObjectMap = new Dictionary<string, GameObject>();
        private IList<GameObject> m_gameObjectListAdd = new List<GameObject>();
        private IList<GameObject> m_gameObjectList = new List<GameObject>();
        private IList<GameObject> m_gameObjectListRemove = new List<GameObject>();
        private GameplayScreen m_gameplayScreen;
    }

    public class DistanceSorter : IComparer<GameObject>
    {
        public DistanceSorter(Vector3 position)
        {
            m_position = position;
        }

        public int  Compare(GameObject x, GameObject y)
        {
            float xlen = (x.Position - m_position).LengthSquared();
            float ylen = (y.Position - m_position).LengthSquared();

            return (int)(ylen - xlen);
        }

        private Vector3 m_position;
    

}


}
