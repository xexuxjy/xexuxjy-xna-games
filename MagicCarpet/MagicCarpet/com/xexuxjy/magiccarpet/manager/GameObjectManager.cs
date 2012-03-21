﻿using System;
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

        public GameObject CreateAndInitialiseGameObject(GameObjectType gameObjectType, Vector3 startPosition)
        {
            return CreateAndInitialiseGameObject(gameObjectType, startPosition, null);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
 
        public GameObject CreateAndInitialiseGameObject(GameObjectType gameObjectType, Vector3 startPosition,Dictionary<String,String> properties)
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
                AddAndInitializeObject(gameObject,true);
            }

            if (properties != null)
            {
                SetObjectProperties(gameObject, properties);
            }


            return gameObject;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void AddAndInitializeObject(GameObject gameObject)
        {
            AddAndInitializeObject(gameObject, false);
        }

        public void AddAndInitializeObject(GameObject gameObject,bool initialize)
        {
            if (initialize)
            {
                gameObject.Initialize();
            }
            m_gameObjectListAdd.Add(gameObject);

            m_drawableList.Add(gameObject);
            m_drawableList.Sort(drawComparator);

#if LOG_EVENT
                Globals.EventLogger.LogEvent(String.Format("AddObject[{0}][{1}].", gameObject.Id, gameObject.GameObjectType));
#endif

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void RemoveObject(GameObject gameObject)
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
                RemoveObjectInternal(removedGameObject);
            }

            m_gameObjectListRemove.Clear();
    
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private void RemoveObjectInternal(GameObject removedGameObject)
        {
            if (removedGameObject != null)
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
            FindObjects(typeMask, position, radius, null, results,false);
        }

        public void FindObjects(GameObjectType typeMask, Vector3 position, float radius, GameObject owner, List<GameObject> results,bool includeOwner)
        {
            float radiusSq = radius * radius;
            foreach (GameObject gameObject in m_gameObjectList)
            {
                // if it's the sort of object we're interested in
                GameObjectType gameObjectType = gameObject.GameObjectType;
                if ((gameObjectType & typeMask) > 0)
                {
                        bool shouldInclude = true;
                        if (!includeOwner)
                        {
                            if (gameObject == owner)
                            {
                                shouldInclude = false;
                            }
                            if (gameObject.Owner == owner)
                            {
                                shouldInclude = false;
                            }

                        }

                    // check and see if it's the owning entity if appropriate
                    if (owner == null || shouldInclude)
                    {
                        if ((gameObject.Position - position).LengthSquared() <= radiusSq)
                        {
                            results.Add(gameObject);
                        }
                    }
                }
            }
            List<GameObject> copy = new List<GameObject>();
            copy.AddRange(results);
            results.Sort(new DistanceSorter(position));
            int ibreak = 0;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        //public void FindObjectsExcludeOwner(GameObjectType typeMask, Vector3 position, float radius, GameObject owner, List<GameObject> results)
        //{
        //    float radiusSq = radius * radius;
        //    foreach (GameObject gameObject in m_gameObjectList)
        //    {
        //        // if it's the sort of object we're interested in
        //        GameObjectType gameObjectType = gameObject.GameObjectType;
        //        if ((gameObjectType & typeMask) > 0)
        //        {
        //            // check and see if it's the owning entity if appropriate
        //            if (owner == null || 
        //            {
        //                if ((gameObject.Position - position).LengthSquared() <= radiusSq)
        //                {
        //                    results.Add(gameObject);
        //                }
        //            }
        //        }
        //    }
        //    results.Sort(new DistanceSorter(position));
        //}

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

        public override void Draw(GameTime gameTime)
        {
            foreach (IDrawable drawable in m_drawableList)
            {
                drawable.Draw(gameTime);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        static Comparison<IDrawable> drawComparator = new Comparison<IDrawable>(DrawableSortPredicate);
        private static int DrawableSortPredicate(IDrawable lhs, IDrawable rhs)
        {
            int result = lhs.DrawOrder - rhs.DrawOrder;
            return result;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private List<GameObject> m_gameObjectListAdd = new List<GameObject>();
        private List<GameObject> m_gameObjectList = new List<GameObject>();
        private List<GameObject> m_gameObjectListRemove = new List<GameObject>();
        private List<IDrawable> m_drawableList = new List<IDrawable>();

        private GameplayScreen m_gameplayScreen;
        public const GameObjectType m_allActiveObjectTypes = GameObjectType.spell | GameObjectType.manaball | GameObjectType.balloon | GameObjectType.castle | GameObjectType.magician | GameObjectType.monster;

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

            return (int)(xlen - ylen);
        }

        private Vector3 m_position;
    

}


}
