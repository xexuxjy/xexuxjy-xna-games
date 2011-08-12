using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using com.xexuxjy.magiccarpet.terrain;
using com.xexuxjy.magiccarpet.renderer;
using Dhpoware;
using com.xexuxjy.magiccarpet.gameobjects;

namespace com.xexuxjy.magiccarpet.collision
{
    public class QuadTreeNode
    {
        public QuadTreeNode(QuadTreeNode parent, BoundingBox extents, int depth, String debugId)
        {
            m_debugId = debugId;
            m_parentNode = parent;
            m_depth = depth;
            m_extents = extents;
            BuildChildren();
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // recursively build the children to max depth.
        private void BuildChildren()
        {
            if (m_depth + 1 < (int)com.xexuxjy.magiccarpet.collision.QuadTree.eConstants.k_maximumTreeDepth)
            {
                m_childNodes = new QuadTreeNode[4];
                Vector3 topLeft = m_extents.Min;
                Vector3 stepSize = m_extents.Max - m_extents.Min;
                Vector3 diffVec = new Vector3(stepSize.X / 2.0f, stepSize.Y, stepSize.Z / 2.0f);

                Vector3 sq1 = topLeft;
                Vector3 sq2 = sq1 + diffVec;
                Vector3 sq3 = sq1 + new Vector3(diffVec.X, stepSize.Y, 0.0f);
                Vector3 sq4 = sq3 + diffVec;
                Vector3 sq5 = sq1 + new Vector3(0.0f, stepSize.Y, diffVec.Z);
                Vector3 sq6 = sq5 + diffVec;
                Vector3 sq7 = sq1 + diffVec;
                Vector3 sq8 = sq7 + diffVec;

                // fix up the height values.
                sq1.Y = sq3.Y = sq5.Y = sq7.Y = m_extents.Min.Y;
                sq2.Y = sq4.Y = sq6.Y = sq8.Y = m_extents.Max.Y;

                BoundingBox t1 = new BoundingBox(sq1, sq2);
                BoundingBox t2 = new BoundingBox(sq3, sq4);
                BoundingBox t3 = new BoundingBox(sq5, sq6);
                BoundingBox t4 = new BoundingBox(sq7, sq8);

                m_childNodes[0] = new QuadTreeNode(this, t1, m_depth + 1, DebugId + "0");
                m_childNodes[1] = new QuadTreeNode(this, t2, m_depth + 1, DebugId + "1");
                m_childNodes[2] = new QuadTreeNode(this, t3, m_depth + 1, DebugId + "2");
                m_childNodes[3] = new QuadTreeNode(this, t4, m_depth + 1, DebugId + "3");
            }
            else
            {
                m_childNodes = new QuadTreeNode[0];
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool AddWorldObject(GameObject worldObject)
        {
            bool fitsInChild = false;
            // try and add it in all of our child nodes first?
            for (int i = 0; i < m_childNodes.Length; ++i)
            {
                fitsInChild = m_childNodes[i].AddWorldObject(worldObject);
                if (fitsInChild)
                {
                    break;
                }
            }

            if (false == fitsInChild && DoesObjectFitInNode(worldObject))
            {
                m_objectsInNode.Add(worldObject);
                // update the worldobject so it knows which node it's in.
                worldObject.QuadTreeNode = this;
                fitsInChild = true;
            }
            return fitsInChild;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Removes the object from the node, this makes the assumption that the node does contain the object.
        /// Methods on quadtree itself can confirm this but the main use for this function is that objects should
        /// track their owner node so a straight remove is ok.
        /// </summary>
        /// <param name="worldObject"></param>
        public void RemoveObject(GameObject worldObject)
       {
           worldObject.QuadTreeNode = null;
           m_objectsInNode.Remove(worldObject);
       }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////


        public bool IsEmpty()
        {
            return m_objectsInNode.Count == 0;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public QuadTreeNode GetChildAt(int x, int y)
        {
            return m_childNodes[(1 << y) + x];
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public QuadTreeNode[] GetChildNodes()
        {
            return m_childNodes;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool HasChildren()
        {
            return m_childNodes.Length > 0;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void BuildVisibilityList(List<GameObject> objectList, ICamera camera)
        {
            // foreach (WorldObject worldObject in m_objectsInNode)
            // {
            //    if (camera.IsInViewFrustum(worldObject))
            //    {
            //        objectList.Add(worldObject);
            //    }
            //}

            //for (int i = 0; i < m_childNodes.Length; ++i)
            //{
            //    m_childNodes[i].BuildVisibilityList(objectList, camera);
            //}
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool DoesObjectFitInNode(GameObject worldObject)
        {
            BoundingBox boundingBox = worldObject.BoundingBox;
            return m_extents.Contains(boundingBox) == ContainmentType.Contains;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool Contains(ref BoundingBox boundingBox)
        {
            return m_extents.Contains(boundingBox) == ContainmentType.Contains;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool Contains(ref Vector3 position)
        {
            return m_extents.Contains(position) == ContainmentType.Contains;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void GetObjectsInNode(List<GameObject> resultList)
        {
            resultList.AddRange(m_objectsInNode);
            // and do the same on the children.
            for (int i = 0; i < m_childNodes.Length; ++i)
            {
                m_childNodes[i].GetObjectsInNode(resultList);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool DoesNodeContainObject(GameObject wo)
        {
            bool found = m_objectsInNode.Contains(wo);
            return found;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Searches through the tree and finds the node that holds the object.
        /// </summary>
        /// <param name="worldObject"></param>
        /// <returns></returns>
        public QuadTreeNode FindNodeForObject(GameObject worldObject)
        {
            QuadTreeNode returnNode = null;
            if (DoesNodeContainObject(worldObject))
            {
                returnNode = this;
            }
            else
            {
                for (int i = 0; i < m_childNodes.Length; ++i)
                {
                    returnNode = m_childNodes[i].FindNodeForObject(worldObject);
                    if (returnNode != null)
                    {
                        break;
                    }
                }
            }
            return returnNode;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public String DebugId
        {
            get { return m_debugId; }
            set { m_debugId = value; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // These are only built when the quadtreenode belongs to only one terrain section.
        public void BuildTerrainIndicies()
        {
            //Terrain terrain = Globals.Terrain;

            //TerrainSection terrainSection = terrain.GetSectionForBoundingBox(ref m_extents);
            //if (terrainSection != null)
            //{
            //    m_terrainSection = terrainSection;
            //    int minX = (int)m_extents.Min.X;
            //    int maxX = (int)m_extents.Max.X;
            //    int minZ = (int)m_extents.Min.Z;
            //    int maxZ = (int)m_extents.Max.Z;

            //    // only makes sense to do this if granulatrity is at least one square
            //    if (maxX - minX > 0)
            //    {
            //        m_terrainIndices = ((TerrainSectionRenderer)m_terrainSection.Renderer).GetOffsetIndices(minX, minZ, maxX, maxZ);
            //        for (int i = 0; i < m_terrainIndices.Length; ++i)
            //        {
            //            if (m_terrainIndices[i] > 6000)
            //            {
            //                int ibreak = 0;
            //            }
            //        }
            //    }
            //}
            ////else
            //{
            //    for (int i = 0; i < m_childNodes.Length; ++i)
            //    {
            //        m_childNodes[i].BuildTerrainIndicies();
            //    }
            //}
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool FindTerrainIndicesForMove(ref BoundingBox objectMove, List<TerrainIndicesHolder> results)
        {
            bool childContains = false;
            // check and see if it's within our bounding area, if so keep applying further down.
            ContainmentType type = m_extents.Contains(objectMove);
            if (type == ContainmentType.Contains || type == ContainmentType.Intersects)
            {
                for (int i = 0; i < m_childNodes.Length; ++i)
                {
                    childContains |= m_childNodes[i].FindTerrainIndicesForMove(ref objectMove, results);
                }
                // if one of our children contains the move then they will populate it, otherwise
                // we're responsible.
                // FIXME - look at splitting the move so that a diagonal doesn't introduce un-necessary indices.
                if (!childContains)
                {
                    results.Add(new TerrainIndicesHolder(m_terrainSection, m_terrainIndices));
                    childContains = true;
                }
            }
            return childContains;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////



        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private QuadTreeNode[] m_childNodes;
        private QuadTreeNode m_parentNode;
        private BoundingBox m_extents;
        private int m_depth;
        private List<GameObject> m_objectsInNode = new List<GameObject>();
        private String m_debugId;
        private TerrainSection m_terrainSection;
        private int[] m_terrainIndices;
    }
}
