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
namespace com.xexuxjy.magiccarpet.terrain
{

    public abstract class WorldObject : GameComponent
	{

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public WorldObject(Game game)
            : base(game)
        {
            game.Components.Add(this);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public WorldObject(Vector3 startPosition, Game game)
            : base(game)
        {
            game.Components.Add(this);
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////	
        [DescriptionAttribute("Position in the world")]
		virtual public Vector3 Position
		{
            get { return m_position; }
            set { m_position = value; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        [DescriptionAttribute("Spawn Position in the world")]
        virtual public Vector3 SpawnPosition
        {
            get{return m_spawnPosition;}
            set{m_spawnPosition = value;}
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	
        
        [DescriptionAttribute("Scale Factor")]
        virtual public Vector3 ScaleVector
        {
            get{return m_scaleVector;}
            set{m_scaleVector = value;}
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	
 
        [DescriptionAttribute("Id")]
		virtual public System.String Id
		{
			get{return m_id;}
            set{m_id = value;}
		}

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public bool Initialised
        {
            get { return m_initialised; }
            set { m_initialised = value; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

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


        protected System.String m_id;
	
        protected Vector3 m_spawnPosition; // where the object starts in the world, used by AI
        protected Vector3 m_position; // where the object starts in the world, used by AI
 
        protected  Vector3 m_scaleVector;

        protected  WorldObject m_owner; // if this is owned by another entitiy (e.g. manaballs,castles, balloons owned by magicians)

		protected  long m_currentElapsedTime = 0;

        protected BoundingBox m_boundingBox;
 
        protected QuadTreeNode m_quadTreeNode;
        protected bool m_isSelected;
        protected bool m_collider = true;
        protected bool m_aiControlled = false; // wether this is being controlled by the ai.
        protected Vector3 m_objectSize; // This is the desired object size in the game world, the scale matrix is calced from this and the true model size.    
        protected Color m_badgeColor; // represents the color for multiplayer type stuff.
        protected uint m_reactObjects; // This is a bitmask of the object types we're interested in.
        protected uint m_threatObjects; // Objects that we'll attack.
        private float m_minGroundHeight; // used to lift object above the ground. 

        private bool m_initialised = false;
        protected Vector3 m_heading;

        protected RigidBody m_rigidBody;

    }
}