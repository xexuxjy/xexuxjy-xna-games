// Sample taken from http://www.dustinhorne.com/page/XNA-Terrain-Tutorial-Table-of-Contents.aspx - many thanks

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Gladius.util;

namespace Gladius.Terrain7
{
	public class QuadNode
	{
		readonly QuadNode _parent;
		readonly QuadTree _parentTree;
		//TODO:  _positionIndex can be gathered from the Top Left Vertex... no need to store the extra data.
		int _positionIndex;

		readonly int _nodeDepth;
		readonly int _nodeSize;

		bool _isActive;
		bool _isSplit;

		/// <summary>
		/// Parent node reference
		/// </summary>
		public QuadNode Parent
		{
			get { return _parent; }
		}

		#region VERTICES
		public QuadNodeVertex VertexTopLeft { get; private set; }
		public QuadNodeVertex VertexTop { get; private set; }
		public QuadNodeVertex VertexTopRight { get; private set; }
		public QuadNodeVertex VertexLeft { get; private set; }
		public QuadNodeVertex VertexCenter { get; private set; }
		public QuadNodeVertex VertexRight { get; private set; }
		public QuadNodeVertex VertexBottomLeft { get; private set; }
		public QuadNodeVertex VertexBottom { get; private set; }
		public QuadNodeVertex VertexBottomRight { get; private set; }
		#endregion

		#region CHILDREN
		public QuadNode ChildTopLeft { get; private set; }
		public QuadNode ChildTopRight { get; private set; }
		public QuadNode ChildBottomLeft { get; private set; }
		public QuadNode ChildBottomRight { get; private set; }
		#endregion

		#region NEIGHBORS
		public QuadNode NeighborTop { get; private set; }
		public QuadNode NeighborRight { get; private set; }
		public QuadNode NeighborBottom { get; private set; }
		public QuadNode NeighborLeft { get; private set; }
		#endregion

		public bool HasChildren { get; private set; }
		public Culling.Rectangle Bounds { get; private set; }
		public NodeType NodeType { get; private set; }

		public bool IsSplit { get { return _isSplit; } }

		public bool IsInView
		{
			get
			{
				return Bounds.Intersects(_parentTree.ClipShape);
                //return true;
			}
		}

		/// <summary>
		/// Returns true of the node contains enough vertices to split
		/// </summary>
		public bool CanSplit { get { return (_nodeSize >= 2); } }

		public bool IsActive { get { return _isActive; } internal set { _isActive = value; } }

		public QuadNode(NodeType nodeType, int nodeSize, int nodeDepth, QuadNode parent, QuadTree parentTree, int positionIndex)
		{
			NodeType = nodeType;
			_nodeSize = nodeSize;
			_nodeDepth = nodeDepth;
			_positionIndex = positionIndex;

			_parent = parent;
			_parentTree = parentTree;

			//Add the 9 vertices
			AddVertices();

			var tl = new Vector2(_parentTree.Vertices[VertexTopLeft.Index].Position.X, _parentTree.Vertices[VertexTopLeft.Index].Position.Z);
			var br = new Vector2(_parentTree.Vertices[VertexBottomRight.Index].Position.X, _parentTree.Vertices[VertexBottomRight.Index].Position.Z);

			Bounds = Culling.Rectangle.FromPoints(tl, br);

			if (nodeSize >= 4)
				AddChildren();

			//Make call to UpdateNeighbors from the parent node only.
			//This will update neighbors recursively for the children 
			//as well.  This ensures all nodes are created prior to 
			//updating neighbors.
			if (_nodeDepth == 1)
			{
				AddNeighbors();

				VertexTopLeft.Activated = true;
				VertexTopRight.Activated = true;
				VertexCenter.Activated = true;
				VertexBottomLeft.Activated = true;
				VertexBottomRight.Activated = true;

			}
		}

		/// <summary>
		/// Add the 9 vertices for this quad
		/// </summary>
		private void AddVertices()
		{
			switch (NodeType)
			{
				case NodeType.TopLeft:
					VertexTopLeft = _parent.VertexTopLeft;
					VertexTopRight = _parent.VertexTop;
					VertexBottomLeft = _parent.VertexLeft;
					VertexBottomRight = _parent.VertexCenter;
					break;

				case NodeType.TopRight:
					VertexTopLeft = _parent.VertexTop;
					VertexTopRight = _parent.VertexTopRight;
					VertexBottomLeft = _parent.VertexCenter;
					VertexBottomRight = _parent.VertexRight;
					break;

				case NodeType.BottomLeft:
					VertexTopLeft = _parent.VertexLeft;
					VertexTopRight = _parent.VertexCenter;
					VertexBottomLeft = _parent.VertexBottomLeft;
					VertexBottomRight = _parent.VertexBottom;
					break;

				case NodeType.BottomRight:
					VertexTopLeft = _parent.VertexCenter;
					VertexTopRight = _parent.VertexRight;
					VertexBottomLeft = _parent.VertexBottom;
					VertexBottomRight = _parent.VertexBottomRight;
					break;

				default:
					VertexTopLeft = new QuadNodeVertex
					{
						Activated = true, 
						Index = 0
					};

					VertexTopRight = new QuadNodeVertex
					{
					    Activated = true,
					    Index = VertexTopLeft.Index + _nodeSize
					};

					VertexBottomLeft = new QuadNodeVertex
					{
					    Activated = true,
					    Index = (_nodeSize + 1) * _parentTree.TopNodeSize
					};

					VertexBottomRight = new QuadNodeVertex
					{
					    Activated = true,
					    Index = VertexBottomLeft.Index + _nodeSize
					};

					break;
			}

            float skirtSize = 0.3f;
            int index = 0;
            m_skirtVertices[0] = _parentTree.Vertices[VertexTopLeft.Index];
            m_skirtVertices[1] = _parentTree.Vertices[VertexTopRight.Index];
            m_skirtVertices[2] = _parentTree.Vertices[VertexBottomRight.Index];
            m_skirtVertices[3] = _parentTree.Vertices[VertexBottomLeft.Index];


            //m_skirtVertices[4] = _parentTree.Vertices[VertexTopLeft.Index];
            //m_skirtVertices[4].Position += new Vector3(-skirtSize,-skirtSize,-skirtSize);

            //m_skirtVertices[5] = _parentTree.Vertices[VertexTopRight.Index];
            //m_skirtVertices[5].Position += new Vector3(skirtSize, -skirtSize, -skirtSize);

            //m_skirtVertices[6] = _parentTree.Vertices[VertexBottomRight.Index];
            //m_skirtVertices[6].Position += new Vector3(skirtSize, -skirtSize, skirtSize);

            //m_skirtVertices[7] = _parentTree.Vertices[VertexBottomLeft.Index];
            //m_skirtVertices[7].Position += new Vector3(-skirtSize, -skirtSize, skirtSize);


            Vector3 skirtOffset = new Vector3(0, -skirtSize, 0);
            m_skirtVertices[4] = _parentTree.Vertices[VertexTopLeft.Index];
            m_skirtVertices[4].Position += skirtOffset;

            m_skirtVertices[5] = _parentTree.Vertices[VertexTopRight.Index];
            m_skirtVertices[5].Position += skirtOffset;

            m_skirtVertices[6] = _parentTree.Vertices[VertexBottomRight.Index];
            m_skirtVertices[6].Position += skirtOffset;

            m_skirtVertices[7] = _parentTree.Vertices[VertexBottomLeft.Index];
            m_skirtVertices[7].Position += skirtOffset;



			VertexTop = new QuadNodeVertex
			{
				Activated = false,
				Index = VertexTopLeft.Index + (_nodeSize / 2)
			};

			VertexLeft = new QuadNodeVertex
			{ 
				Activated = false,
				Index = VertexTopLeft.Index + (_parentTree.TopNodeSize + 1) * (_nodeSize / 2) 
			};

			VertexCenter = new QuadNodeVertex
			{
				Activated = false,
				Index = VertexLeft.Index + (_nodeSize / 2)
			};

			VertexRight = new QuadNodeVertex
			{ 
				Activated = false, Index = VertexLeft.Index + _nodeSize
			};

			VertexBottom = new QuadNodeVertex
			{
				Activated = false,
				Index = VertexBottomLeft.Index + (_nodeSize / 2)
			};
		}

		/// <summary>
		/// Add child nodes
		/// </summary>
		private void AddChildren()
		{
			//Add top left (northwest) child
			ChildTopLeft = new QuadNode(NodeType.TopLeft, _nodeSize / 2, _nodeDepth + 1, this, _parentTree, VertexTopLeft.Index);
			
			//Add top right (northeast) child
			ChildTopRight = new QuadNode(NodeType.TopRight, _nodeSize / 2, _nodeDepth + 1, this, _parentTree, VertexTop.Index);

			//Add bottom left (southwest) child
			ChildBottomLeft = new QuadNode(NodeType.BottomLeft, _nodeSize / 2, _nodeDepth + 1, this, _parentTree, VertexLeft.Index);

			//Add bottom right (southeast) child
			ChildBottomRight = new QuadNode(NodeType.BottomRight, _nodeSize / 2, _nodeDepth + 1, this, _parentTree, VertexCenter.Index);

			HasChildren = true;
		}

		/// <summary>
		/// Update reference to neighboring quads
		/// </summary>
		private void AddNeighbors()
		{
			switch (NodeType)
			{
				case NodeType.TopLeft: //Top Left Corner
					//Top neighbor
					if (_parent.NeighborTop != null)
						NeighborTop = _parent.NeighborTop.ChildBottomLeft;

					//Right neighbor
					NeighborRight = _parent.ChildTopRight;
					//Bottom neighbor
					NeighborBottom = _parent.ChildBottomLeft;

					//Left neighbor
					if (_parent.NeighborLeft != null)
						NeighborLeft = _parent.NeighborLeft.ChildTopRight;

					break;

				case NodeType.TopRight: //Top Right Corner
					//Top neighbor
					if (_parent.NeighborTop != null)
						NeighborTop = _parent.NeighborTop.ChildBottomRight;

					//Right neighbor
					if (_parent.NeighborRight != null)
						NeighborRight = _parent.NeighborRight.ChildTopLeft;

					//Bottom neighbor
					NeighborBottom = _parent.ChildBottomRight;

					//Left neighbor
					NeighborLeft = _parent.ChildTopLeft;

					break;

				case NodeType.BottomLeft: //Bottom Left Corner
					//Top neighbor
					NeighborTop = _parent.ChildTopLeft;

					//Right neighbor
					NeighborRight = _parent.ChildBottomRight;

					//Bottom neighbor
					if (_parent.NeighborBottom != null)
						NeighborBottom = _parent.NeighborBottom.ChildTopLeft;

					//Left neighbor
					if (_parent.NeighborLeft != null)
						NeighborLeft = _parent.NeighborLeft.ChildBottomRight;

					break;

				case NodeType.BottomRight: //Bottom Right Corner
					//Top neighbor
					NeighborTop = _parent.ChildTopRight;

					//Right neighbor
					if (_parent.NeighborRight != null)
						NeighborRight = _parent.NeighborRight.ChildBottomLeft;

					//Bottom neighbor
					if (_parent.NeighborBottom != null)
						NeighborBottom = _parent.NeighborBottom.ChildTopRight;

					//Left neighbor
					NeighborLeft = _parent.ChildBottomLeft;

					break;
			}


			if (HasChildren)
			{
				ChildTopLeft.AddNeighbors();
				ChildTopRight.AddNeighbors();
				ChildBottomLeft.AddNeighbors();
				ChildBottomRight.AddNeighbors();
			}

		}

		/// <summary>
		/// Push the relevant active vertex indices to the parent tree 
		/// </summary>
		internal void SetActiveVertices()
		{
			if (_parentTree.Cull && !IsInView)
				return;

			if (_isSplit && HasChildren)
			{
				ChildTopLeft.SetActiveVertices();
				ChildTopRight.SetActiveVertices();
				ChildBottomLeft.SetActiveVertices();
				ChildBottomRight.SetActiveVertices();
				return;
			}

			//Top Triangle(s)
			_parentTree.UpdateBuffer(VertexCenter.Index);
			_parentTree.UpdateBuffer(VertexTopLeft.Index);

			if (VertexTop.Activated)
			{
				_parentTree.UpdateBuffer(VertexTop.Index);

				_parentTree.UpdateBuffer(VertexCenter.Index);
				_parentTree.UpdateBuffer(VertexTop.Index);
			}
			_parentTree.UpdateBuffer(VertexTopRight.Index);

			//Right Triangle(s)
			_parentTree.UpdateBuffer(VertexCenter.Index);
			_parentTree.UpdateBuffer(VertexTopRight.Index);

			if (VertexRight.Activated)
			{
				_parentTree.UpdateBuffer(VertexRight.Index);

				_parentTree.UpdateBuffer(VertexCenter.Index);
				_parentTree.UpdateBuffer(VertexRight.Index);
			}
			_parentTree.UpdateBuffer(VertexBottomRight.Index);

			//Bottom Triangle(s)
			_parentTree.UpdateBuffer(VertexCenter.Index);
			_parentTree.UpdateBuffer(VertexBottomRight.Index);

			if (VertexBottom.Activated)
			{
				_parentTree.UpdateBuffer(VertexBottom.Index);

				_parentTree.UpdateBuffer(VertexCenter.Index);
				_parentTree.UpdateBuffer(VertexBottom.Index);
			}
			_parentTree.UpdateBuffer(VertexBottomLeft.Index);

			//Left Triangle(s)
			_parentTree.UpdateBuffer(VertexCenter.Index);
			_parentTree.UpdateBuffer(VertexBottomLeft.Index);

			if (VertexLeft.Activated)
			{
				_parentTree.UpdateBuffer(VertexLeft.Index);

				_parentTree.UpdateBuffer(VertexCenter.Index);
				_parentTree.UpdateBuffer(VertexLeft.Index);
			}
			_parentTree.UpdateBuffer(VertexTopLeft.Index);

		}

		/// <summary>
		/// Activate the current node and corresponding vertices
		/// </summary>
		internal void Activate()
		{
			VertexTopLeft.Activated = true;
			VertexTopRight.Activated = true;
			VertexCenter.Activated = true;
			VertexBottomLeft.Activated = true;
			VertexBottomRight.Activated = true;

			_isActive = true;
		}

		/// <summary>
		/// Force the quad tree to split to the minimum depth
		/// </summary>
		internal void EnforceMinimumDepth()
		{
			if (_nodeDepth < _parentTree.MinimumDepth)
			{
				if (HasChildren)
				{
					_isActive = false;
					_isSplit = true;

					ChildTopLeft.EnforceMinimumDepth();
					ChildTopRight.EnforceMinimumDepth();
					ChildBottomLeft.EnforceMinimumDepth();
					ChildBottomRight.EnforceMinimumDepth();
				}
				else
				{
					Activate();
					_isSplit = false;
				}

				return;
			}

			if (_nodeDepth == _parentTree.MinimumDepth)
			{
				Activate();
				_isSplit = false;
			}
		}

		/// <summary>
		/// Returns true if the QuadNode contains a specific point.
		/// </summary>
		/// <param name="point">Vector3 representing the target point</param>
		/// <returns>True if point is contained in the node's bounding rectangle</returns>
		public bool Contains(Vector2 point)
		{
			return (point.X >= Bounds.Point1.X && point.X <= Bounds.Point3.X
				&& point.Y >= Bounds.Point1.Y && point.Y <= Bounds.Point3.Y);

		}

		/// <summary>
		/// Get a reference to the deepest node that contains a point.
		/// </summary>
		/// <param name="point">Vector2 representing the target point</param>
		/// <returns>Deepest quad node containing target point</returns>
		public QuadNode DeepestNodeWithPoint(Vector2 point)
		{
			//If the point isn't in this node, return null
			if (!Contains(point))
				return null;

			if (HasChildren)
			{
				if (ChildTopLeft.Contains(point))
					return ChildTopLeft.DeepestNodeWithPoint(point);

				if (ChildTopRight.Contains(point))
					return ChildTopRight.DeepestNodeWithPoint(point);

				if (ChildBottomLeft.Contains(point))
					return ChildBottomLeft.DeepestNodeWithPoint(point);

				//It's contained in this node and not in the first 3 
				//children, so has to be in 4th child.  No need to check.
				return ChildBottomRight.DeepestNodeWithPoint(point);
			}

			//No children, return this QuadNode
			return this;
		}

		/// <summary>
		/// Split the node by activating vertices
		/// </summary>
		public void Split()
		{
			if (_parentTree.Cull && !IsInView)
				return;

			//Make sure parent node is split
			if (_parent != null && !_parent.IsSplit)
				_parent.Split();

			if (CanSplit)
			{
				//Set active nodes
				if (HasChildren)
				{
					ChildTopLeft.Activate();
					ChildTopRight.Activate();
					ChildBottomLeft.Activate();
					ChildBottomRight.Activate();

					_isActive = false;

				}
				else
				{
					_isActive = true;
				}

				_isSplit = true;
				//Set active vertices
				VertexTop.Activated = true;
				VertexLeft.Activated = true;
				VertexRight.Activated = true;
				VertexBottom.Activated = true;
			}

			//Make sure neighbor parents are split
			EnsureNeighborParentSplit(NeighborTop);
			EnsureNeighborParentSplit(NeighborRight);
			EnsureNeighborParentSplit(NeighborBottom);
			EnsureNeighborParentSplit(NeighborLeft);

			//Make sure neighbor vertices are active
			if (NeighborTop != null)
				NeighborTop.VertexBottom.Activated = true;

			if (NeighborRight != null)
				NeighborRight.VertexLeft.Activated = true;

			if (NeighborBottom != null)
				NeighborBottom.VertexTop.Activated = true;

			if (NeighborLeft != null)
				NeighborLeft.VertexRight.Activated = true;
		}

		/// <summary>
		/// Merge split quad nodes
		/// </summary>
		public void Merge()
		{
            //return;
			//Only perform the merge if there are children
			VertexTop.Activated = false;
			VertexLeft.Activated = false;
			VertexRight.Activated = false;
			VertexBottom.Activated = false;

			if (NodeType != NodeType.FullNode)
			{
				VertexTopLeft.Activated = false;
				VertexTopRight.Activated = false;
				VertexBottomLeft.Activated = false;
				VertexBottomRight.Activated = false;
			}
			_isActive = true;
			_isSplit = false;

			if (HasChildren)
			{

				if (ChildTopLeft.IsSplit)
				{
					ChildTopLeft.Merge();
					ChildTopLeft.IsActive = false;
				}
				else
				{
					ChildTopLeft.VertexTop.Activated = false;
					ChildTopLeft.VertexLeft.Activated = false;
					ChildTopLeft.VertexRight.Activated = false;
					ChildTopLeft.VertexBottom.Activated = false;
				}

				if (ChildTopRight.IsSplit)
				{
					ChildTopRight.Merge();
					ChildTopRight.IsActive = false;
				}
				else
				{
					ChildTopRight.VertexTop.Activated = false;
					ChildTopRight.VertexLeft.Activated = false;
					ChildTopRight.VertexRight.Activated = false;
					ChildTopRight.VertexBottom.Activated = false;
				}


				if (ChildBottomLeft.IsSplit)
				{
					ChildBottomLeft.Merge();
					ChildBottomLeft.IsActive = false;
				}
				else
				{
					ChildBottomLeft.VertexTop.Activated = false;
					ChildBottomLeft.VertexLeft.Activated = false;
					ChildBottomLeft.VertexRight.Activated = false;
					ChildBottomLeft.VertexBottom.Activated = false;
				}


				if (ChildBottomRight.IsSplit)
				{
					ChildBottomRight.Merge();
					ChildBottomRight.IsActive = false;
				}
				else
				{
					ChildBottomRight.VertexTop.Activated = false;
					ChildBottomRight.VertexLeft.Activated = false;
					ChildBottomRight.VertexRight.Activated = false;
					ChildBottomRight.VertexBottom.Activated = false;
				}
			}
		}

		/// <summary>
		/// Make sure neighbor parents are split to ensure 
		/// consistency with vertex sharing and tessellation.
		/// </summary>
		private static void EnsureNeighborParentSplit(QuadNode neighbor)
		{
			//Checking for null neighbor and null parent in case 
			//we recode for additional quad trees in the future.
			if (neighbor != null && neighbor.Parent != null)
				if (!neighbor.Parent.IsSplit)
					neighbor.Parent.Split();
		}

        public void BuildSkirt(ObjectArray<VertexPositionNormalTexture> vertices, ObjectArray<int> indices)
        {
            // only add skirts for active nodes. if we're not then check children.
            if (IsActive)
            {
                int vcount = vertices.Count;
                for (int i = 0; i < m_skirtVertices.Length; ++i)
                {
                    vertices.Add(m_skirtVertices[i]);
                }

                int icount = vcount;

                indices.Add(icount);
                indices.Add(icount + 4);
                indices.Add(icount + 5);

                indices.Add(icount + 5);
                indices.Add(icount + 1);
                indices.Add(icount);

                indices.Add(icount + 1);
                indices.Add(icount + 5);
                indices.Add(icount + 6);

                indices.Add(icount + 6);
                indices.Add(icount + 2);
                indices.Add(icount + 1);

                indices.Add(icount + 2);
                indices.Add(icount + 6);
                indices.Add(icount + 7);

                indices.Add(icount + 7);
                indices.Add(icount + 3);
                indices.Add(icount + 2);

                indices.Add(icount + 3);
                indices.Add(icount + 7);
                indices.Add(icount + 4);

                indices.Add(icount + 4);
                indices.Add(icount);
                indices.Add(icount + 3);
            }
            else
            {
                int depthCheck = 20;

                if (HasChildren && _nodeDepth < depthCheck)
                {
                    ChildTopLeft.BuildSkirt(vertices, indices);
                    ChildTopRight.BuildSkirt(vertices, indices);
                    ChildBottomLeft.BuildSkirt(vertices, indices);
                    ChildBottomRight.BuildSkirt(vertices, indices);
                }
            }
        }



        public VertexPositionNormalTexture[] m_skirtVertices = new VertexPositionNormalTexture[8];




	}
}


