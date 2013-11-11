using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Gladius.renderer
{
    public static class GraphicsHelper
    {
        public static void CalculateBoundingBox(ModelMesh mm, ref BoundingBox bb)
        {
            //bb = new BoundingBox();
            bool first = true;
            Matrix x = Matrix.Identity;
            ModelBone mb = mm.ParentBone;
            while (mb != null)
            {
                x = x * mb.Transform;
                mb = mb.Parent;
            }


            Vector3 meshMax = new Vector3(float.MinValue);
            Vector3 meshMin = new Vector3(float.MaxValue);

            foreach (ModelMeshPart part in mm.MeshParts)
            {
                int stride = part.VertexBuffer.VertexDeclaration.VertexStride;

                //VertexPositionNormalTexture[] vertexData = new VertexPositionNormalTexture[part.NumVertices];
                Vector3[] vertexData = new Vector3[part.NumVertices];
                //int num = (part.NumVertices - part.VertexOffset);
                int num = part.NumVertices;

                //GetData(offsetFromStartOfVertexBufferInBytes, arrayOfVector3, 0, arrayOfVector3.Length, sizeOfEachVertexInBytes);

                part.VertexBuffer.GetData(0, vertexData, 0, num, stride);
                //part.VertexBuffer.GetData(part.VertexOffset * stride, vertexData, 0, num, stride);

                // Find minimum and maximum xyz values for this mesh part
                //Vector3 vertPosition = new Vector3();

                for (int i = 0; i < vertexData.Length; i++)
                {
                    Vector3 vertPosition = vertexData[i];
                    //vertPosition.X = vertexData[i];
                    //vertPosition.Y = vertexData[i + 1];
                    //vertPosition.Z = vertexData[i + 2];

                    // update our values from this vertex
                    meshMin = Vector3.Min(meshMin, vertPosition);
                    meshMax = Vector3.Max(meshMax, vertPosition);
                    i += stride;
                }
            }

            // transform by mesh bone matrix
            //meshMin = Vector3.Transform(meshMin, meshTransform);
            //meshMax = Vector3.Transform(meshMax, meshTransform);

            // Create the bounding box
            //BoundingBox box = new BoundingBox(meshMin, meshMax);

            BoundingBox newbox = new BoundingBox(meshMin, meshMax);
            bb = MergeBoxes(bb, newbox);
        }

        public static BoundingBox MergeBoxes(BoundingBox one, BoundingBox two)
        {
            Vector3 min = one.Min;
            Vector3 max = one.Max;

            min.X = Math.Min(one.Min.X, two.Min.X);
            min.Y = Math.Min(one.Min.Y, two.Min.Y);
            min.Z = Math.Min(one.Min.Z, two.Min.Z);

            max.X = Math.Max(one.Max.X, two.Max.X);
            max.Y = Math.Max(one.Max.Y, two.Max.Y);
            max.Z = Math.Max(one.Max.Z, two.Max.Z);

            return new BoundingBox(min, max);
        }


    }


    public class ModelData
    {
        public Model Model;
        public Matrix[] BoneTransforms;
        public Texture2D Texture;
        public Texture2D Texture2;
        public float HeightOffset;
        public BoundingBox BoundingBox;
        public Vector3 ModelScale;

        public ModelData(Model _model, float desiredScale,float _heightOffset,Texture2D _texture, Texture2D _texture2=null)
        {
            Model = _model;
            HeightOffset = _heightOffset;
            Texture = _texture;
            Texture2 = _texture2;
            BoneTransforms = new Matrix[_model.Bones.Count];
            _model.CopyAbsoluteBoneTransformsTo(BoneTransforms);
            CalcBounds(desiredScale);
        }

        public ModelData(Model _model, Vector3 desiredScale, float _heightOffset, Texture2D _texture, Texture2D _texture2 = null)
        {
            Model = _model;
            HeightOffset = _heightOffset;
            Texture = _texture;
            Texture2 = _texture2;
            BoneTransforms = new Matrix[_model.Bones.Count];
            _model.CopyAbsoluteBoneTransformsTo(BoneTransforms);
            CalcBounds(desiredScale);
        }



        private void CalcBounds(float desiredScale)
        {
            BoundingBox bb = new BoundingBox();
            foreach (ModelMesh mesh in Model.Meshes)
            {
                int ibreak = 0;
                GraphicsHelper.CalculateBoundingBox(mesh, ref bb);

            }
            Vector3 diff = bb.Max - bb.Min;
            float maxSpan = Math.Max(diff.X, Math.Max(diff.Y, diff.Z));
            //BoundingSphere actorBs = m_model.Meshes[0].BoundingSphere;
            ModelScale = new Vector3(desiredScale / maxSpan);

            BoundingBox = new BoundingBox(bb.Min * ModelScale, bb.Max * ModelScale);
        }

        private void CalcBounds(Vector3 desiredScale)
        {
            BoundingBox bb = new BoundingBox();
            foreach (ModelMesh mesh in Model.Meshes)
            {
                int ibreak = 0;
                GraphicsHelper.CalculateBoundingBox(mesh, ref bb);

            }
            Vector3 diff = bb.Max - bb.Min;
            //float maxSpan = Math.Max(diff.X, Math.Max(diff.Y, diff.Z));
            //BoundingSphere actorBs = m_model.Meshes[0].BoundingSphere;
            ModelScale = new Vector3(desiredScale.X / diff.X, desiredScale.Y / diff.Y, desiredScale.Z / diff.Z);
            BoundingBox = new BoundingBox(bb.Min * ModelScale, bb.Max * ModelScale);
        }



        public void Draw(ICamera camera,Vector3 position)
        {
            Draw(camera,position, ModelScale, Matrix.Identity);
        }

        public void Draw(ICamera camera, Vector3 position, Vector3 scale, Matrix rotation)
        {
            position.Y += HeightOffset;

            if (Texture2 == null)
            {
                Matrix world = Matrix.CreateScale(scale) * rotation * Matrix.CreateTranslation(position);
                foreach (ModelMesh mm in Model.Meshes)
                {
                    foreach (BasicEffect effect in mm.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.TextureEnabled = true;
                        effect.Texture = Texture;
                        effect.View = camera.View;
                        effect.Projection = camera.Projection;
                        effect.World = BoneTransforms[mm.ParentBone.Index] * world;
                    }
                    mm.Draw();
                }
            }
            else
            {
                DrawParts(camera, position, scale, rotation);
            }
        }

        public void DrawParts(ICamera camera,Vector3 position, Vector3 scale, Matrix rotation)
        {
            Matrix world = Matrix.CreateScale(scale) * rotation * Matrix.CreateTranslation(position);
            //Matrix world = Matrix.CreateTranslation(position);
            foreach (ModelMesh mm in Model.Meshes)
            {
                int count = 0;
                foreach (ModelMeshPart mp in mm.MeshParts)
                {
                    //GraphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
                    //GraphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;
                    ++count;
                    BasicEffect effect = mp.Effect as BasicEffect;
                    effect.EnableDefaultLighting();
                    effect.TextureEnabled = true;
                    effect.Texture = (count == 2 && Texture2 != null) ? Texture : Texture2;
                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                    effect.World = BoneTransforms[mm.ParentBone.Index] * world;
                }
                mm.Draw();
            }
        }


    }

}
