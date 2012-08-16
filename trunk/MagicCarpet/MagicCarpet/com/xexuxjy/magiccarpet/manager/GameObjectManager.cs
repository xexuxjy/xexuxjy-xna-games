using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.gameobjects;

using GameStateManagement;
using com.xexuxjy.magiccarpet.terrain;
using BulletXNA.LinearMath;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;
using Dhpoware;
using BulletXNADemos.Demos;
using com.xexuxjy.magiccarpet.collision;
using com.xexuxjy.magiccarpet.util.console;
using com.xexuxjy.magiccarpet.util.debug;

namespace com.xexuxjy.magiccarpet.manager
{
    public class GameObjectManager : DrawableGameComponent
    {
        public GameObjectManager(GameplayScreen gameplayScreen)
            : base(Globals.Game)
        {
            m_gameplayScreen = gameplayScreen;
            m_physicsFrameStart = new AutoResetEvent(false);
            m_physicsFrameEnd = new AutoResetEvent(false);


        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void CreateInitialComponents()
        {
            Globals.Camera = new CameraComponent();
            Globals.GraphicsDevice = Globals.Game.GraphicsDevice;



            Globals.DebugDraw = new XNA_ShapeDrawer(Globals.Game);

            m_debugDrawMode = DebugDrawModes.DBG_DrawAabb;// | DebugDrawModes.DBG_DrawWireframe;
            //m_debugDrawMode = DebugDrawModes.ALL;
            //m_debugDrawMode = DebugDrawModes.DBG_DrawAabb;

            Globals.DebugDraw.SetDebugMode(m_debugDrawMode);
            if (Globals.DebugDraw != null)
            {
                Globals.DebugDraw.LoadContent();
            }

            Globals.CollisionManager = new CollisionManager(m_physicsFrameStart,m_physicsFrameEnd,Globals.worldMinPos, Globals.worldMaxPos);
            Globals.GameObjectManager.AddAndInitializeObject(Globals.CollisionManager, true);


            Globals.SimpleConsole = new SimpleConsole(Globals.DebugDraw);
            Globals.SimpleConsole.Enabled = false;
            Globals.GameObjectManager.AddAndInitializeObject(Globals.SimpleConsole, true);


            Globals.MCContentManager = new MCContentManager();
            Globals.MCContentManager.Initialize();

            Globals.DebugObjectManager = new DebugObjectManager(Globals.DebugDraw);
            Globals.DebugObjectManager.Enabled = true;
            Globals.GameObjectManager.AddAndInitializeObject(Globals.DebugObjectManager);



            Globals.ActionPool = new ActionPool();

            Globals.SpellPool = new SpellPool();

            Globals.Terrain = (Terrain)Globals.GameObjectManager.CreateAndInitialiseGameObject("Terrain", Vector3.Zero);

            Globals.FlagManager = new FlagManager();
            Globals.GameObjectManager.AddAndInitializeObject(Globals.FlagManager, true);


            Globals.TreeManager = new TreeManager();
            Globals.GameObjectManager.AddAndInitializeObject(Globals.TreeManager, true);


            Globals.GameObjectManager.AddAndInitializeObject(Globals.Camera);
            Globals.s_currentCameraFrustrum = new BoundingFrustum(Globals.Camera.ProjectionMatrix * Globals.Camera.ViewMatrix);


            Globals.GameObjectManager.AddAndInitializeObject(new SkyDome(), true);



        }

        
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

        public GameObject CreateAndInitialiseGameObject(String gameObjectType, Vector3 startPosition)
        {
            return CreateAndInitialiseGameObject(gameObjectType, startPosition, null);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
 
        public GameObject CreateAndInitialiseGameObject(String gameObjectType, Vector3 startPosition,Dictionary<String,String> properties)
        {
            GameObject gameObject = null;
            switch (gameObjectType.ToLower())
            {
                case "manaball":
                    {
                        gameObject = new ManaBall(startPosition);
                        break;
                    }
                case "balloon":
                    {
                        gameObject = new Balloon(startPosition);
                        break;
                    }
                case "castle":
                    {
                        gameObject = new Castle(startPosition);
                        break;
                    }
                case "magician":
                    {
                        gameObject = new Magician(startPosition);
                        break;
                    }
                case "terrain":
                    {
                        gameObject = new Terrain(startPosition);
                        break;
                    }
                case "monster":
                    {
                        gameObject = new Monster(startPosition);
                        break;
                    }
                case "dragon":
                    {
                        gameObject = new Dragon(startPosition);
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
            // All new objects are added and removed here

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

            // tell the phyics we're ready to go.
            m_physicsFrameStart.Set();
            Thread.MemoryBarrier();

            foreach (GameObject gameObject in m_gameObjectList)
            {
                gameObject.Update(gameTime);
            }
            // wait for physics before we continue?
            m_physicsFrameEnd.WaitOne();


            // and again all old objects are removed in a single threaded form.


            foreach (GameObject removedGameObject in m_gameObjectListRemove)
            {
                RemoveObjectInternal(removedGameObject);
            }

            m_gameObjectListRemove.Clear();

            // do this at the end
            Globals.CollisionManager.ProcessCollisions();

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
                m_drawableList.Remove(removedGameObject);

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
                castle.GrowToLevel(level);


            }

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Draw(GameTime gameTime)
        {
            foreach (IDrawable drawable in m_drawableList)
            {
                if (drawable as TreeManager == null)
                {
                    //continue;
                }
                drawable.Draw(gameTime);
            }

            // now temp effects.
            DrawEffects(Globals.GraphicsDevice, Globals.Camera.ViewMatrix, Matrix.Identity, Globals.Camera.ProjectionMatrix);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        static Comparison<IDrawable> drawComparator = new Comparison<IDrawable>(DrawableSortPredicate);
        private static int DrawableSortPredicate(IDrawable lhs, IDrawable rhs)
        {
            int result = lhs.DrawOrder - rhs.DrawOrder;
            return result;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public TempGraphicHolder AddTempGraphicHolder(GameObject ownerObject, Model model, Texture2D texture, Texture2D normalTexture, Matrix transform)
        {

            TempGraphicHolder tempHolder = new TempGraphicHolder(ownerObject, model, texture, normalTexture, transform);
            m_tempGraphicHolders.Add(tempHolder);
            return tempHolder;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ReleaseTempGraphicHolder(TempGraphicHolder tempHolder)
        {
            m_tempGraphicHolders.Remove(tempHolder);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        static Matrix[] s_boneTransforms = new Matrix[10];

        public void DrawEffects(GraphicsDevice graphicsDevice, Matrix view, Matrix world, Matrix projection)
        {
            Globals.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            foreach (TempGraphicHolder holder in m_tempGraphicHolders)
            {
                if (holder.m_active && holder.m_model != null)
                {
                    //if (Globals.s_currentCameraFrustrum.Contains(holder.m_boundingBox) != ContainmentType.Disjoint)
                    {
                        foreach (ModelMesh mesh in holder.m_model.Meshes)
                        {
                            holder.m_model.CopyAbsoluteBoneTransformsTo(s_boneTransforms);
                            foreach (Effect effect in mesh.Effects)
                            {
                                effect.CurrentTechnique = effect.Techniques["SimpleTechnique"];
                                Globals.MCContentManager.ApplyCommonEffectParameters(effect);

                                Matrix owner = holder.m_owner != null ? holder.m_owner.WorldTransform : Matrix.Identity;
                                Matrix result = owner * holder.m_transform * world;

                                effect.Parameters["WorldMatrix"].SetValue(s_boneTransforms[mesh.ParentBone.Index] * result);
                                Texture2D texture = holder.m_texture;
                                if (texture != null)
                                {
                                    effect.Parameters["Texture"].SetValue(texture);
                                }

                                Texture2D normalTexture = holder.m_normalTexture;
                                if (texture != null)
                                {
                                    effect.Parameters["NormalTexture"].SetValue(normalTexture);
                                }
                            }
                            mesh.Draw();
                        }
                    }
                }
            }
            Globals.GraphicsDevice.BlendState = BlendState.Opaque;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public ObjectArray<TempGraphicHolder> m_tempGraphicHolders = new ObjectArray<TempGraphicHolder>();

        private List<GameObject> m_gameObjectListAdd = new List<GameObject>();
        private List<GameObject> m_gameObjectList = new List<GameObject>();
        private List<GameObject> m_gameObjectListRemove = new List<GameObject>();
        private List<IDrawable> m_drawableList = new List<IDrawable>();

        private GameplayScreen m_gameplayScreen;
        public const GameObjectType m_allActiveObjectTypes = GameObjectType.spell | GameObjectType.manaball | GameObjectType.balloon | GameObjectType.castle | GameObjectType.magician | GameObjectType.monster;


        private AutoResetEvent m_physicsFrameStart;
        private AutoResetEvent m_physicsFrameEnd;

        private DebugDrawModes m_debugDrawMode;


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

    public class TempGraphicHolder
    {
        public TempGraphicHolder() { } // list
        public TempGraphicHolder(GameObject owner, Model model, Texture2D texture, Texture2D normalTexture, Matrix transform)
        {
            m_model = model;
            m_texture = texture;
            m_normalTexture = normalTexture;
            m_transform = transform;
            m_owner = owner;
            m_active = true;
        }

        public int m_effectHandle;
        public bool m_active;
        public Model m_model;
        public Texture2D m_texture;
        public Texture2D m_normalTexture;
        public Matrix m_transform;
        public BoundingBox m_boundingBox;
        public GameObject m_owner;
    }


}
