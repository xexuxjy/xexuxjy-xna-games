using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.gameobjects;

using GameStateManagement;
using com.xexuxjy.magiccarpet.terrain;
using BulletXNA.LinearMath;

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

        public static bool IsAttackable(GameObject gameObject)
        {
            bool alive = gameObject.IsAlive();
            if(alive)
            {
                GameObjectType gameObjectType = gameObject.GameObjectType;
                if((gameObjectType & Globals.s_attackableObjects) != 0)
                {
                    return true;
                }
            }
            return false;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public GameObject CreateAndInitialiseGameObject(GameObjectType gameObjectType, IndexedVector3 startPosition)
        {
            return CreateAndInitialiseGameObject(gameObjectType, startPosition, null);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
 
        public GameObject CreateAndInitialiseGameObject(GameObjectType gameObjectType, IndexedVector3 startPosition,Dictionary<String,String> properties)
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
                case GameObjectType.monster:
                    {
                        gameObject = new Monster(startPosition);
                        break;
                    }

            }


            if (gameObject != null)
            {
                AddGameObject(gameObject);
                gameObject.Initialize();
            }

            if (properties != null)
            {
                SetObjectProperties(gameObject, properties);
            }


            return gameObject;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void AddGameObject(GameObject gameObject)
        {
            m_gameObjectListAdd.Add(gameObject);


#if LOG_EVENT
            Globals.EventLogger.LogEvent(String.Format("AddObject[{0}][{1}].", gameObject.Id, gameObject.GameObjectType));
#endif
 
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void RemoveGameObject(GameObject gameObject)
        {
            m_gameObjectListRemove.Add(gameObject);
#if LOG_EVENT
            Globals.EventLogger.LogEvent(String.Format("RemoveObject[{0}][{1}].", gameObject.Id, gameObject.GameObjectType));
#endif
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Update(GameTime gameTime)
        {
            foreach (GameObject gameObject in m_gameObjectListAdd)
            {
                m_gameObjectList.Add(gameObject);
            }

            foreach (GameObject newGameObject in m_gameObjectListAdd)
            {
                foreach (GameObject gameObject in m_gameObjectList)
                {
                    gameObject.WorldObjectAdded(newGameObject);
                }
            }


            m_gameObjectListAdd.Clear();

            foreach (GameObject gameObject in m_gameObjectList)
            {
                gameObject.Update(gameTime);
            }

            foreach (GameObject removedGameObject in m_gameObjectListRemove)
            {
                // cleanup may already have been called.
                if (removedGameObject.Owner != null)
                {
                    removedGameObject.Owner.NotifyOwnershipLost(removedGameObject);
                }
                removedGameObject.Cleanup();
                Globals.CollisionManager.RemoveFromWorld(removedGameObject.CollisionObject);
                m_gameObjectList.Remove(removedGameObject);
                foreach (GameObject gameObject in m_gameObjectList)
                {
                    gameObject.WorldObjectRemoved(removedGameObject);
                }
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


        public void FindObjects(GameObjectType typeMask, IndexedVector3 position, float radius, List<GameObject> results)
        {
            FindObjectsExcludeOwner(typeMask, position, radius, null, results);
        }

        public void FindObjectsForOwner(GameObjectType typeMask, IndexedVector3 position, float radius, GameObject owner, List<GameObject> results)
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

        public void FindObjectsExcludeOwner(GameObjectType typeMask, IndexedVector3 position, float radius, GameObject owner, List<GameObject> results)
        {
            float radiusSq = radius * radius;
            foreach (GameObject gameObject in m_gameObjectList)
            {
                // if it's the sort of object we're interested in
                GameObjectType gameObjectType = gameObject.GameObjectType;
                if ((gameObjectType & typeMask) > 0)
                {
                    // check and see if it's the owning entity if appropriate
                    if (owner == null || (gameObject.Owner != owner && gameObject != owner))
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
            GameObject result = null;
            // brute force search for now.
            foreach(GameObject gameObject in m_gameObjectList)
            {
                if (gameObject.Id == id)
                {
                    result = gameObject;
                    break;
                }
            }

            // check the add list
            if (result == null)
            {

                foreach (GameObject gameObject in m_gameObjectListAdd)
                {
                    if (gameObject.Id == id)
                    {
                        result = gameObject;
                        break;
                    }
                }
            }
            return result;
        }
        
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SetObjectProperties(GameObject gameObject, Dictionary<String,String> properties)
        {
            String value = null;

            if (properties.TryGetValue("id", out value))
            {
                gameObject.Id = value;

                if (value == "Player1")
                {
                    Globals.Player = (Magician)gameObject;
                }

            }

            if(properties.TryGetValue("ownerid",out value))
            {
                // find owner.
                GameObject owner = GetObject(value);
                gameObject.Owner = owner;
            }

            if(properties.TryGetValue("castlelevel",out value))
            {
                Castle castle =  gameObject as Castle;
                int level = int.Parse(value);
                castle.GrowToSize(level);

            }

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private ObjectArray<GameObject> m_gameObjectListAdd = new ObjectArray<GameObject>();
        private ObjectArray<GameObject> m_gameObjectList = new ObjectArray<GameObject>();
        private ObjectArray<GameObject> m_gameObjectListRemove = new ObjectArray<GameObject>();
        private GameplayScreen m_gameplayScreen;
    }

    public class DistanceSorter : IComparer<GameObject>
    {
        public DistanceSorter(IndexedVector3 position)
        {
            m_position = position;
        }

        public int  Compare(GameObject x, GameObject y)
        {
            float xlen = (x.Position - m_position).LengthSquared();
            float ylen = (y.Position - m_position).LengthSquared();

            return (int)(ylen - xlen);
        }

        private IndexedVector3 m_position;
    

}


}
