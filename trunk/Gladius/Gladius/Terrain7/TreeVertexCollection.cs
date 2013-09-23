// Sample taken from http://www.dustinhorne.com/page/XNA-Terrain-Tutorial-Table-of-Contents.aspx - many thanks

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System;

namespace Gladius.Terrain7
{
	public class TreeVertexCollection
	{
		public VertexPositionNormalTexture[] Vertices;
		int _topSize;
		int _halfSize;
		int _vertexCount;
		int _scale;
        float  _heightScaling;

		public VertexPositionNormalTexture this[int index]
		{
			get { return Vertices[index]; }
			set { Vertices[index] = value; }
		}


		public TreeVertexCollection(Texture2D heightMap, int scale,float heightScaling,int heightOverride=0)
		{
            int hmWidth = heightOverride > 0 ? heightOverride : heightMap.Width;

			_scale = scale;
			_topSize = hmWidth - 1;
			_halfSize = _topSize / 2;
            _vertexCount = hmWidth * hmWidth;
            _heightScaling = heightScaling;

			//Initialize our array to hold the vertices
			Vertices = new VertexPositionNormalTexture[_vertexCount];

			//Our method to populate the vertex collection
			BuildVertices(heightMap,hmWidth);

			//Our method to  calculate the normals for all vertices
			CalculateAllNormals();
		}

        public float GetHeightAtPoint(int x, int z)
        {
            int width = (_topSize + 1);
            x = MathHelper.Clamp(x,0, width);
            z = MathHelper.Clamp(z, 0, width);

            int index = (z * width) + x;
            Debug.Assert(index < Vertices.Length);
            if (index < Vertices.Length)
            {
                return Vertices[index].Position.Y;
            }
            return 0f;
        }

		private void BuildVertices(Texture2D heightMap,int hmWidth)
		{
            var heightMapColors = new Color[_vertexCount];
            heightMap.GetData(heightMapColors);

			float x = 0;
			float z = 0;
			float y = 0;
			float maxX = x + _topSize;

            float fixHeight = 0f;

			for (int i = 0; i < _vertexCount; i++)
			{
				if (x > maxX)
				{
					x = 0;
					z++;
				}

				y = (heightMapColors[i].R / _heightScaling);
				var vert = new VertexPositionNormalTexture(new Vector3(x * _scale, y * _scale, z * _scale), Vector3.Zero, Vector2.Zero);
				vert.TextureCoordinate = new Vector2((vert.Position.X) / _topSize, (vert.Position.Z) / _topSize);
				Vertices[i] = vert;
				x++;
			}
		}

        private void BuildSkirts()
        {
            // each node level needs an aditional 4 vertices to hold the skirt values.
            int baseNumVertices = Vertices.Length;
            int depth = 6;

            int skirtIndicesPerNode = 24;
            int len = (4 * depth * skirtIndicesPerNode);


        }


		private void CalculateAllNormals()
		{
			if (_vertexCount < 9)
				return;

			int i = _topSize + 2, j = 0, k = i + _topSize;

			for (int n = 0; i <= (_vertexCount - _topSize) - 2; i += 2, n++, j += 2, k += 2)
			{

				if (n == _halfSize)
				{
					n = 0;
					i += _topSize + 2;
					j += _topSize + 2;
					k += _topSize + 2;
				}

				//Calculate normals for each of the 8 triangles
				SetNormals(i, j, j + 1);
				SetNormals(i, j + 1, j + 2);
				SetNormals(i, j + 2, i + 1);
				SetNormals(i, i + 1, k + 2);
				SetNormals(i, k + 2, k + 1);
				SetNormals(i, k + 1, k);
				SetNormals(i, k, i - 1);
				SetNormals(i, i - 1, j);
			}

            for (int n = 0; n < Vertices.Length; ++n)
            {
                if (Vertices[n].Normal.LengthSquared() != 0)
                {
                    Vertices[n].Normal.Normalize();
                }
                else
                {
                    Vertices[n].Normal = Vector3.Up;
                }
            }

		}

        private void SetNormals(int idx1, int idx2, int idx3)
		{
			if (idx3 >= Vertices.Length)
				idx3 = Vertices.Length - 1;

			var normal = Vector3.Cross(Vertices[idx2].Position - Vertices[idx1].Position, Vertices[idx1].Position - Vertices[idx3].Position);
			normal.Normalize();
			Vertices[idx1].Normal += normal;
			Vertices[idx2].Normal += normal;
			Vertices[idx3].Normal += normal;
		}
	}
}