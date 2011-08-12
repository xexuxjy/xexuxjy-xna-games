/*
* Created on 11-Jan-2006
*
* To change the template for this generated file go to
* Window - Preferences - Java - Code Generation - Code and Comments
*/

using Microsoft.Xna.Framework;
using System.ComponentModel;
using com.xexuxjy.magiccarpet.collision;
using BulletXNA.BulletDynamics;
using BulletXNA;
using com.xexuxjy.magiccarpet.renderer;
using System;
namespace com.xexuxjy.magiccarpet.gameobjects
{

    public abstract class GameObject : GameComponent
	{

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public GameObject(Game game)
            : base(game)
        {
            m_motionState = new DefaultMotionState();
            game.Components.Add(this);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public GameObject(Vector3 startPosition, Game game)
            : base(game)
        {
            m_motionState = new DefaultMotionState(Matrix.CreateTranslation(startPosition), Matrix.Identity);

            game.Components.Add(this);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public override void Initialize()
        {
            if (m_defaultRenderer != null)
            {
                m_defaultRenderer.Initialize();
            }

            base.Initialize();
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

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        virtual public System.String ModelName
        {
            get { return m_modelName; }
            set { m_modelName = value; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public QuadTreeNode QuadTreeNode
        {
            get { return m_quadTreeNode; }
            set { m_quadTreeNode = value; }
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

        public RigidBody RigidBody
        {
            get { return m_rigidBody; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////


        protected virtual void BuildCollisionObject()
        {

        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////


        protected String m_id;
        protected String m_modelName;
        protected Vector3 m_spawnPosition; // where the object starts in the world, used by AI
 
        protected  GameObject m_owner; // if this is owned by another entitiy (e.g. manaballs,castles, balloons owned by magicians)

		protected  long m_currentElapsedTime = 0;

        protected BoundingBox m_boundingBox;
 
        protected QuadTreeNode m_quadTreeNode;
        protected bool m_isSelected;
        protected bool m_collider = true;
        protected bool m_aiControlled = false; // wether this is being controlled by the ai.

        protected Color m_badgeColor; // represents the color for multiplayer type stuff.
        protected uint m_reactObjects; // This is a bitmask of the object types we're interested in.
        protected uint m_threatObjects; // Objects that we'll attack.
        private float m_minGroundHeight; // used to lift object above the ground. 

        protected IMotionState m_motionState;
        protected RigidBody m_rigidBody;
        protected DefaultRenderer m_defaultRenderer;
    }
}