using Microsoft.Xna.Framework;
using System.ComponentModel;
using com.xexuxjy.magiccarpet.collision;
using BulletXNA.BulletDynamics;
using BulletXNA;
using System;
using System.Collections.Generic;
using BulletXNA.BulletCollision;
using com.xexuxjy.magiccarpet.spells;
using com.xexuxjy.magiccarpet.actions;
using com.xexuxjy.utils.debug;
using Dhpoware;
using Microsoft.Xna.Framework.Graphics;
namespace com.xexuxjy.magiccarpet.gameobjects
{

    public abstract class GameObject : DrawableGameComponent , IDebuggable
	{

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public GameObject(Game game,GameObjectType gameObjectType)
            : base(game)
        {
            m_gameObjectType = gameObjectType;
            m_motionState = new DefaultMotionState();
            game.Components.Add(this);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public GameObject(Vector3 startPosition, Game game,GameObjectType gameObjectType)
            : base(game)
        {
            m_gameObjectType = gameObjectType;
            m_motionState = new DefaultMotionState(Matrix.CreateTranslation(startPosition), Matrix.Identity);
            game.Components.Add(this);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public override void Initialize()
        {
            m_scaleTransform = Matrix.Identity;

            m_model = Globals.MCContentManager.ModelForObjectType(GameObjectType);
            base.Initialize();
        }
        
        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (CurrentAction != null)
            {
                CurrentAction.Update(gameTime);
            }

            foreach (Spell spell in m_activeSpells)
            {
                spell.Update(gameTime);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public virtual void Cleanup()
        {

        }
        ///////////////////////////////////////////////////////////////////////////////////////////////	

        protected virtual void BuildCollisionObject()
        {
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public override void Draw(GameTime gameTime)
        {

            ICamera camera = Globals.Camera;

            // only one of these should be active.

            DrawEffect(Game.GraphicsDevice, camera.ViewMatrix, WorldTransform, camera.ProjectionMatrix);

            //DrawDebugAxes(Game.GraphicsDevice);
            //if (ShouldDrawBoundingBox())
            //{
            //    DrawBoundingBox(Game.GraphicsDevice);
            //}
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        protected virtual void DrawEffect(GraphicsDevice graphicsDevice, Matrix view, Matrix world, Matrix projection)
        {
            if (m_model != null)
            {
                //Matrix scale = Matrix.CreateScale(modelScalingData.scale);
                Matrix[] transforms = new Matrix[m_model.Bones.Count];
                BasicEffect basicEffect = Globals.MCContentManager.BasicEffect;
                foreach (ModelMesh mesh in m_model.Meshes)
                {
                    m_model.CopyAbsoluteBoneTransformsTo(transforms);
                    foreach (Effect effect in mesh.Effects)
                    {
                        basicEffect.View = view;
                        basicEffect.Projection = projection;
                        basicEffect.World = transforms[mesh.ParentBone.Index] * world;
                    }
                    mesh.Draw();
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	
        
        public virtual void DrawDebugAxes(GraphicsDevice graphicsDevice)
        {

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public virtual void DrawBoundingBox(GraphicsDevice graphicsDevice)
        {

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	
        [DescriptionAttribute("Position in the world")]
		virtual public Vector3 Position
		{
            get 
            {
                Matrix m;
                m_motionState.GetWorldTransform(out m);
                return m.Translation;
            }
            set 
            {
                Matrix m;
                m_motionState.GetWorldTransform(out m);
                m.Translation = value;
                m_motionState.SetWorldTransform(ref m);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public Matrix WorldTransform
        {
            get
            {
                Matrix m;
                m_motionState.GetWorldTransform(out m);
                return m;
            }
            set
            {
                Matrix m = value;
                m_motionState.SetWorldTransform(ref m);
            }
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////	

        [DescriptionAttribute("Spawn Position in the world")]
        virtual public Vector3 SpawnPosition
        {
            get{return m_spawnPosition;}
            set{m_spawnPosition = value;}
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	
 
        [DescriptionAttribute("Id")]
		virtual public System.String Id
		{
			get{return m_id;}
            set{m_id = value;}
		}

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public BoundingBox BoundingBox
        {
            get 
            {
                 return m_boundingBox; 
            }
            set 
            { 
                m_boundingBox = value; 
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public CollisionObject CollisionObject
        {
            get { return m_collisionObject; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public GameObject Owner
        {
            get { return m_owner; }
            set 
            {
                if (value != m_owner)
                {
                    OwnerChanged(m_owner, value);
                }
                m_owner = value; 
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void CastSpell(SpellType spellType,Vector3 startPosition,Vector3 targetPosition)
        {
            if (CanCastSpell(spellType))
            {
                SpellTemplate template;
                m_spellTemplates.TryGetValue(spellType, out template);
                GameObjectAttribute mana = GetAttribute(GameObjectAttributeType.Mana);
                mana.CurrentValue -= template.ManaCost;
                // Todo - factory or similar to create object
                Spell spell = new Turbo(Game);
                spell.Initialize(template, this);
                m_activeSpells.Add(spell);
                spell.SpellComplete += new Spell.SpellCompleteHandler(spell_SpellComplete);
            }
        }

        void spell_SpellComplete(Spell spell)
        {
            SpellTemplate spellTemplate = spell.SpellTemplate;

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool CanCastSpell(SpellType spellType)
        {
            bool result = false;
            SpellTemplate template;
            m_spellTemplates.TryGetValue(spellType, out template);
            if (template != null && template.Available)
            {
                GameObjectAttribute mana = GetAttribute(GameObjectAttributeType.Mana);
                if (mana.CurrentValue > template.ManaCost)
                {
                    result = true;
                }
            }
            return result;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public GameObjectAttribute GetAttribute(GameObjectAttributeType type)
        {
            return m_attributes[type];
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Die()
        {
            CurrentAction = new ActionDie(this);

        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////
        
        public BaseAction CurrentAction
        {
            get { return m_currentAction; }
            set 
            {
                if (value != m_currentAction)
                {
                    m_currentAction = value;
                    value.Initialize();
                    value.ActionStarted += new BaseAction.ActionStartedHandler(value_ActionStarted);
                    value.ActionComplete += new BaseAction.ActionCompleteHandler(value_ActionComplete);
                }

            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public Vector3 WorldToLocal(Vector3 worldPoint)
        {
            return ( worldPoint - m_boundingBox.Min);

        }

        public Vector3 LocalToWorld(Vector3 localPoint)
        {
            return (localPoint + m_boundingBox.Min);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void value_ActionComplete(BaseAction action)
        {
        
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void value_ActionStarted(BaseAction baseAction)
        {
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public GameObjectType GameObjectType
        {
            get { return m_gameObjectType; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public String DebugText
        {
            get { return ""; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool DebugEnabled
        {
            get { return m_debugEnabled; }
            set { m_debugEnabled = value; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        // Delegates and events



        public delegate void OwnerChangedHandler(GameObject oldOwner,GameObject newOwner);
        public event OwnerChangedHandler OwnerChanged;

        public delegate void DamagedHandler(GameObject sender, EventArgs e);
        public event DamagedHandler Damaged;

        public delegate void SpellCastHandler(GameObject gameObject, Spell spell);
        public event SpellCastHandler SpellCast;







        // And others
        private List<Spell> m_activeSpells = new List<Spell>();
        private Dictionary<SpellType, SpellTemplate> m_spellTemplates = new Dictionary<SpellType, SpellTemplate>();

        private Dictionary<GameObjectAttributeType, GameObjectAttribute> m_attributes = new Dictionary<GameObjectAttributeType, GameObjectAttribute>();

        protected String m_id;
        protected Vector3 m_spawnPosition; // where the object starts in the world, used by AI
 
        protected GameObject m_owner; // if this is owned by another entitiy (e.g. manaballs,castles, balloons owned by magicians)

        protected BoundingBox m_boundingBox;
 
        protected Color m_badgeColor; // represents the color for multiplayer type stuff.

        protected GameObjectType m_gameObjectType;

        protected IMotionState m_motionState;
        protected CollisionObject m_collisionObject;
        protected BaseAction m_currentAction;
        protected Matrix m_scaleTransform;
        protected Model m_model;

        protected bool m_debugEnabled;
    }















}