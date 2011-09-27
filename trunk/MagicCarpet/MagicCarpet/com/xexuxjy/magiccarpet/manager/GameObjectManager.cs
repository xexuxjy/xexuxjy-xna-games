using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.gameobjects;

namespace com.xexuxjy.magiccarpet.manager
{
    public class GameObjectManager : GameComponent
    {
        public GameObjectManager(Game game)
            : base(game)
        {
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        
        public GameObject CreateAndInitialiseGameObject(GameObjectType gameObjectType, Vector3 startPosition)
        {
            GameObject gameObject = null;
            switch (gameObjectType)
            {
                case GameObjectType.MANABALL:
                    {
                        gameObject = new ManaBall(startPosition, Game);
                        break;
                    }
                case GameObjectType.BALLOON:
                    {
                        gameObject = new Balloon(startPosition, Game);
                        break;
                    }
                case GameObjectType.CASTLE:
                    {
                        gameObject = new Castle(startPosition, Game);
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
                m_gameObjectList.Remove(gameObject);
            }
            m_gameObjectListRemove.Clear();
    
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void FindObjects(GameObjectType typeMask, List<GameObject> results)
        {
            foreach (GameObject gameObject in m_gameObjectList)
            {
                if( typeMask == GameObjectType.NONE || typeMask == gameObject.GameObjectType)
                {
                    results.Add(gameObject);
                }
            }
        }


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

            // ToDo - order the return list base on min to max distance.


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

        public GameObject GetObject(String id)
        {
            return null;
        }
        
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        
        private Dictionary<String, GameObject> m_gameObjectMap = new Dictionary<string, GameObject>();
        private IList<GameObject> m_gameObjectListAdd = new List<GameObject>();
        private IList<GameObject> m_gameObjectList = new List<GameObject>();
        private IList<GameObject> m_gameObjectListRemove = new List<GameObject>();

    }
}
