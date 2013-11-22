using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;


namespace Gladius.renderer
{
    public class SimpleQuad
    {
        public Effect Effect;

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

        public SimpleQuad(ContentManager content)
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

            Effect = content.Load<Effect>("Effects/MovementGrid/MovementGrid");
            
            //sBasicEffect.EnableDefaultLighting();
            FillVertices();
        }

        private void FillVertices()
        {
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

        public void Draw(GraphicsDevice device,Texture2D texture, Matrix world, Vector3 normal,Vector3 scale,ICamera camera,Color teamColour,float alpha =1f)
        {
            Effect.Parameters["World"].SetValue(world);            
            Effect.Parameters["Texture"].SetValue(texture);
            Effect.Parameters["View"].SetValue(camera.View);
            Effect.Parameters["Projection"].SetValue(camera.Projection);
            //Effect.Parameters["Alpha"].SetValue(alpha);
            //Effect.Parameters["TeamColour"].SetValue(teamColour.ToVector3());
            //Effect.Parameters["BaseColour"].SetValue(Color.White.ToVector3());

            // ugly.
            Vertices[0].TextureCoordinate = textureLowerLeft;
            Vertices[1].TextureCoordinate = textureUpperLeft;
            Vertices[2].TextureCoordinate = textureLowerRight;
            Vertices[3].TextureCoordinate = textureUpperRight;

            foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
                    PrimitiveType.TriangleList, Vertices, 0, Vertices.Length, Indices, 0, Indices.Length/3);
            }
        }


        public void Draw(GraphicsDevice device, Texture2D texture, Matrix world, Vector3 normal, Vector3 scale, ICamera camera, Color teamColour, Vector4 texcoords,float alpha = 1f)
        {
            Effect.Parameters["World"].SetValue(world);
            Effect.Parameters["Texture"].SetValue(texture);
            Effect.Parameters["View"].SetValue(camera.View);
            Effect.Parameters["Projection"].SetValue(camera.Projection);
            //Effect.Parameters["Alpha"].SetValue(alpha);
            //Effect.Parameters["TeamColour"].SetValue(teamColour.ToVector3());
            //Effect.Parameters["BaseColour"].SetValue(Color.White.ToVector3());

            Vertices[0].TextureCoordinate = new Vector2(texcoords.X, texcoords.Y);
            Vertices[1].TextureCoordinate = new Vector2(texcoords.X, texcoords.W);
            Vertices[2].TextureCoordinate = new Vector2(texcoords.Z, texcoords.Y);
            Vertices[3].TextureCoordinate = new Vector2(texcoords.Z, texcoords.W);


            foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
                    PrimitiveType.TriangleList, Vertices, 0, Vertices.Length, Indices, 0, Indices.Length / 3);
            }
        }


        Vector2 textureUpperLeft = new Vector2(0.0f, 0.0f);
        Vector2 textureUpperRight = new Vector2(1.0f, 0.0f);
        Vector2 textureLowerLeft = new Vector2(0.0f, 1.0f);
        Vector2 textureLowerRight = new Vector2(1.0f, 1.0f);

    }

    
}