using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Gladius.util;

namespace Gladius.renderer
{
    public static class GraphicsHelper
    {
        public static void DrawShadowedText(SpriteBatch sb, SpriteFont font, String text, Vector2 pos)
        {
            DrawShadowedText(sb,font,text,pos,Color.White);
        }

        public static void DrawShadowedText(SpriteBatch sb, SpriteFont font, String text, Vector2 pos,Color textColor)
        {
            // Shadow text.
            sb.DrawString(font, text, pos, Color.Black);
            sb.DrawString(font, text, pos + new Vector2(1), textColor);
        }

        public static void OffsetRect(ref Rectangle src, ref Rectangle dst)
        {
            dst.X += src.X;
            dst.Y += src.Y;
        }

        public static void CalculateBoundingBox(ModelMesh mm, ref BoundingBox bb)
        {
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

                Vector3[] vertexData = new Vector3[part.NumVertices];
                int num = part.NumVertices;

                part.VertexBuffer.GetData(0, vertexData, 0, num, stride);

                for (int i = 0; i < vertexData.Length; i++)
                {
                    Vector3 vertPosition = vertexData[i];
                    // update our values from this vertex
                    meshMin = Vector3.Min(meshMin, vertPosition);
                    meshMax = Vector3.Max(meshMax, vertPosition);
                    i += stride;
                }
            }

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

        public static Vector3 StringToVector3(String value)
        {
            string[] temp = value.Split(',');

            float x = float.Parse(temp[0]);

            float y = float.Parse(temp[1]);

            float z = float.Parse(temp[2]);

            Vector3 rValue = new Vector3(x, y, z);

            return rValue;
        }


    }


    public class ModelData
    {
        public Model Model;
        public Matrix[] BoneTransforms;
        public Texture2D Texture;
        public float HeightOffset;
        public BoundingBox BoundingBox;
        public Vector3 OriginalModelScale;
        public Vector3 ModelScale;
        public Matrix ModelRotation = Matrix.Identity;

        public List<ModelDataInstance> Instances = new List<ModelDataInstance>();

        public ModelData(String modelName, String textureName, ThreadSafeContentManager contentManager)
        {
            Model = contentManager.Load<Model>(modelName);
            Texture = contentManager.Load<Texture2D>(textureName);
            BoneTransforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(BoneTransforms);
        }



        public ModelData(Model _model, float desiredScale,float _heightOffset,Texture2D _texture)
        {
            Model = _model;
            HeightOffset = _heightOffset;
            Texture = _texture;
            BoneTransforms = new Matrix[_model.Bones.Count];
            _model.CopyAbsoluteBoneTransformsTo(BoneTransforms);
            CalcBounds(desiredScale,out BoundingBox,out ModelScale);
        }

        public ModelData(Model _model, Vector3 desiredScale, float _heightOffset, Texture2D _texture)
        {
            Model = _model;
            HeightOffset = _heightOffset;
            Texture = _texture;
            BoneTransforms = new Matrix[_model.Bones.Count];
            _model.CopyAbsoluteBoneTransformsTo(BoneTransforms);
            CalcBounds(desiredScale,out BoundingBox,out ModelScale);
        }



        public void CalcBounds(float desiredScale,out BoundingBox boundingBox,out Vector3 scale)
        {
            BoundingBox bb = new BoundingBox();
            foreach (ModelMesh mesh in Model.Meshes)
            {
                int ibreak = 0;
                GraphicsHelper.CalculateBoundingBox(mesh, ref bb);

            }
            Vector3 diff = bb.Max - bb.Min;
            float maxSpan = Math.Max(diff.X, Math.Max(diff.Y, diff.Z));
            scale = new Vector3(desiredScale / maxSpan);

            boundingBox = new BoundingBox(bb.Min * scale, bb.Max * scale);
        }

        public void CalcBounds(Vector3 desiredScale,out BoundingBox boundingBox,out Vector3 scale)
        {
            BoundingBox bb = new BoundingBox();
            foreach (ModelMesh mesh in Model.Meshes)
            {
                GraphicsHelper.CalculateBoundingBox(mesh, ref bb);
            }
            Vector3 diff = bb.Max - bb.Min;
            scale = new Vector3(desiredScale.X / diff.X, desiredScale.Y / diff.Y, desiredScale.Z / diff.Z);
            boundingBox = new BoundingBox(bb.Min * scale, bb.Max * scale);
        }


        public void Draw(ICamera camera, Vector3 position)
        {
            Draw(camera, position, ModelScale, Matrix.Identity);
        }

        public void Draw(ICamera camera, Matrix world)
        {
            Draw(camera, world.Translation, ModelScale, world);
        }

        public void Draw(ICamera camera, Vector3 position, Vector3 scale, Matrix origRotation)
        {
            // just want rotation bit.
            Matrix rotation = origRotation;
            rotation.Translation = Vector3.Zero;
            Matrix rot = ModelRotation * rotation;

            Matrix world = Matrix.CreateScale(scale) * rot * Matrix.CreateTranslation(position);
            foreach (ModelMesh mm in Model.Meshes)
            {
                foreach (BasicEffect effect in mm.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.TextureEnabled = true;
                    //effect.Texture = Texture;
                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                    effect.World = BoneTransforms[mm.ParentBone.Index] * world;
                }
                mm.Draw();
            }

        }

        public void AddInstance(Vector3 position, Vector3 euler, Vector3 scale)
        {
            ModelDataInstance instance = new ModelDataInstance();
            instance.Parent = this;
            instance.Position = position;
            instance.DesiredScale = scale;
            instance.Euler = euler;
            instance.Initialise();
            Instances.Add(instance);
        }

        public void DrawInstances(ICamera camera)
        {
            foreach (ModelDataInstance instance in Instances)
            {
                instance.Draw(camera);
            }
        }
    }


    public class ModelDataInstance
    {
        public Vector3 Position;
        public Vector3 DesiredScale;
        public Vector3 ModelScale;
        public Vector3 Euler;
        public Matrix Rotation;
        public BoundingBox BoundingBox;
        public ModelData Parent;

        public void Initialise()
        {
            Parent.CalcBounds(DesiredScale,out BoundingBox,out ModelScale);
        }

        public void Draw(ICamera camera)
        {
            // just want rotation bit.

            Matrix rot = Parent.ModelRotation * Rotation;

            Matrix world = Matrix.CreateScale(ModelScale) * rot * Matrix.CreateTranslation(Position);
            foreach (ModelMesh mm in Parent.Model.Meshes)
            {
                foreach (BasicEffect effect in mm.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.TextureEnabled = true;
                    //effect.Texture = Texture;
                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                    effect.World = Parent.BoneTransforms[mm.ParentBone.Index] * world;
                }
                mm.Draw();
            }
        }

    }

}
