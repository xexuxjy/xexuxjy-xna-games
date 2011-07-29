using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using com.xexuxjy.magiccarpet.terrain;
using MagicCarpet.com.xexuxjy.magiccarpet;

namespace com.xexuxjy.magiccarpet.collision
{
    public class QuadTree
    {
        public enum eConstants
        {
            k_minimumTreeDepth = 1,
            k_maximumTreeDepth = 9, //  must be a value between 1 and 9
        };

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static QuadTree GetInstance()
        {
            if (null == s_quadTree)
            {
                s_quadTree = new QuadTree(new BoundingBox(new Vector3(0.0f, Globals.containmentMinHeight, 0.0f), new Vector3(256.0f, Globals.containmentMaxHeight, 256.0f)),(int)eConstants.k_maximumTreeDepth);
            }
            return s_quadTree;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        private QuadTree(BoundingBox boundingBox, int depth)
        {
            //Debug.Assert(!isReady(), "the quad tree has already been created");
            //Debug.Assert(depth >= (int)eConstants.k_minimumTreeDepth, "invalid tree depth");
            //Debug.Assert(depth <= (int)eConstants.k_maximumTreeDepth, "invalid tree depth");

            m_depth = depth;
            m_worldExtents = boundingBox.Max - boundingBox.Min;
            m_worldOffset = -boundingBox.Min;

            // build the recursive node list.
            m_rootNode = new QuadTreeNode(null, boundingBox,0,"");
            //Processor processor = ((IProcessorService)GlobalUtils.GameServices.GetService(typeof(IProcessorService))).Processor;
            //processor.addRenderable(this);
        }
        
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        public bool IsReady()
        {
            return true;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public QuadTreeNode GetRootNode()
        {
            return m_rootNode;
        }
        
        
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void CalculateVisibility(Object o)
        {
            //m_rootNode.
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Update(float elapsedTime)
        {
            CalculateVisibility(null);
        }
        
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Destroy()
        {
            // Nothing to do here yet.
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void GetObjectsInRange(ref BoundingBox boundingBox, List<WorldObject> resultList)
        {
            resultList.Clear();
            // find the node that best includes position and radius.
            QuadTreeNode bestFitNode = FindAppropriateNode(ref boundingBox);
            //bestFitNode.
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private QuadTreeNode FindAppropriateNode(ref BoundingBox boundingBox)
        {
            // probably a better way then this (as we're aligned on power of 2 boundaries)
            QuadTreeNode node = m_rootNode;
            QuadTreeNode lastNode = null;
            while(node != null)
            {
                // reset node so we can break out the loop.
                lastNode = node;
                node = null;
                if (lastNode.Contains(ref boundingBox) && lastNode.HasChildren())
                {
                    QuadTreeNode[] childNodes = lastNode.GetChildNodes();
                    for (int i = 0; i < childNodes.Length; ++i)
                    {
                        if (childNodes[i].Contains(ref boundingBox))
                        {
                            node = childNodes[i];
                            break;
                        }
                    }
                }
                if (node == null && lastNode != null)
                {
                    break;
                }
            }
            return node;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void AddWorldObject(WorldObject worldObject)
        {
            m_rootNode.AddWorldObject(worldObject);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void RemoveWorldObject(WorldObject worldObject)
        {
            QuadTreeNode currentHolder = m_rootNode.FindNodeForObject(worldObject);
            // should always exist.
            Debug.Assert(currentHolder != null, "Unable to find object in quadtree");
            currentHolder.RemoveObject(worldObject);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Moves the world object from one node to another
        /// </summary>
        /// <param name="worldObject"></param>
        /// <param name="currentOwner"></param>
        public void MoveObject(WorldObject worldObject, QuadTreeNode currentOwner)
        {
            // just make sure it's where it says it is.
            Debug.Assert(currentOwner.DoesNodeContainObject(worldObject));
            currentOwner.RemoveObject(worldObject);
            // and search the tree to re-assign it.
            AddWorldObject(worldObject);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private QuadTreeNode m_rootNode;
	    private Vector3 m_worldExtents;
	    private Vector3 m_worldOffset;
	    private int m_depth;
        private static QuadTree s_quadTree;
    }
}
