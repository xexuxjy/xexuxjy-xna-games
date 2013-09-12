using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Dhpoware;

namespace Gladius.renderer
{
    public class SimpleQuad
    {

        public BasicEffect BasicEffect;



        public Vector3 Origin;
        public Vector3 UpperLeft;
        public Vector3 LowerLeft;
        public Vector3 UpperRight;
        public Vector3 LowerRight;
        public Vector3 Normal;
        public Vector3 Up;
        public Vector3 Left;

        public VertexPositionNormalTexture[] Vertices;
        public int[] Indices;

        public SimpleQuad(GraphicsDevice device)
        {
            Vertices = new VertexPositionNormalTexture[4];
            Indices = new int[6];

            Vector3 dims = Vector3.One;
            dims.Y = 0;
            Vector3 halfDims = dims /2f;
            UpperLeft = -halfDims;
            
            UpperRight = -halfDims;
            UpperRight.X = halfDims.X;

            LowerLeft = -halfDims;
            LowerLeft.Z = halfDims.Z;

            LowerRight = halfDims;
            
            BasicEffect = new BasicEffect(device);
            BasicEffect.TextureEnabled = true;
            //sBasicEffect.EnableDefaultLighting();
            FillVertices();
        }

        private void FillVertices()
        {
            // Fill in texture coordinates to display full texture
            // on quad
            Vector2 textureUpperLeft = new Vector2(0.0f, 0.0f);
            Vector2 textureUpperRight = new Vector2(1.0f, 0.0f);
            Vector2 textureLowerLeft = new Vector2(0.0f, 1.0f);
            Vector2 textureLowerRight = new Vector2(1.0f, 1.0f);

            // Provide a normal for each vertex
            for (int i = 0; i < Vertices.Length; i++)
            {
                Vertices[i].Normal = Normal;
            }

            // Set the position and texture coordinate for each
            // vertex
            Vertices[0].Position = LowerLeft;
            Vertices[0].TextureCoordinate = textureLowerLeft;
            Vertices[1].Position = UpperLeft;
            Vertices[1].TextureCoordinate = textureUpperLeft;
            Vertices[2].Position = LowerRight;
            Vertices[2].TextureCoordinate = textureLowerRight;
            Vertices[3].Position = UpperRight;
            Vertices[3].TextureCoordinate = textureUpperRight;

            // Set the index buffer for each vertex, using
            // clockwise winding
            Indices[0] = 0;
            Indices[1] = 1;
            Indices[2] = 2;
            Indices[3] = 2;
            Indices[4] = 1;
            Indices[5] = 3;
        }

        public void Draw(GraphicsDevice device,Texture2D texture, Matrix world, Vector3 normal,Vector3 scale,ICamera camera)
        {
            BasicEffect.World = world;
            BasicEffect.Texture = texture;
            BasicEffect.View = camera.ViewMatrix;
            BasicEffect.Projection = camera.ProjectionMatrix;

            foreach (EffectPass pass in BasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
                    PrimitiveType.TriangleList, Vertices, 0, Vertices.Length, Indices, 0, Indices.Length/3);
            }
        }
    
    }

    
}