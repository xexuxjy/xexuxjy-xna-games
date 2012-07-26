﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using BulletXNA;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet;
using com.xexuxjy.magiccarpet.manager;
using com.xexuxjy.magiccarpet.gameobjects;
using com.xexuxjy.magiccarpet.renderer;
using LTreesLibrary.Trees;
using BulletXNA.LinearMath;

namespace com.xexuxjy.magiccarpet.terrain
{
    public class TreeManager : EmptyGameObject
    {

        public TreeManager()
            : base(GameObjectType.treemanager)
        {
        }

        public override void Initialize()
        {
            m_billboardTreeEffect = Globals.MCContentManager.GetEffect("InstancedTree");
            m_treeTexture = Globals.MCContentManager.GetTexture("TreeBillboard");
            List<Vector4> treePositions = new List<Vector4>();
            BuildTreeMap(treePositions);
            BuildPrettyTrees(treePositions);
            BuildBillboardTrees();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        // first pass brute force.
        public override void Update(GameTime gameTime)
        {
            Matrix m = Matrix.CreatePerspective(4, 3, m_nearPrettyPlane, m_farPrettyPlane);

            m_prettyFrustum.Matrix = Globals.Camera.ViewMatrix * m;

            m = m = Matrix.CreatePerspective(4, 3, m_farPrettyPlane+1, m_farBillBoardPlane);
            m_billboardFrustum.Matrix = Globals.Camera.ViewMatrix * m;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        #region PrettyTrees

        public void BuildPrettyTrees(List<Vector4> treePositions)
        {
            //m_simpleTree = Globals.MCContentManager.GetSimpleTree("Pine");
            m_simpleTree = Globals.MCContentManager.GetSimpleLowPolyTree("Pine",4); 

            m_instanceTrunkEffect = Globals.MCContentManager.GetEffect("InstancedTree");
            m_simpleTree.TrunkEffect = m_instanceTrunkEffect;
            m_simpleTree.LeafEffect = m_instanceTrunkEffect;

            //m_instanceTreeMatrices = new Matrix[m_treePositions.Count];

            BoundingSphere bs = m_simpleTree.TrunkMesh.BoundingSphere;
            float desiredSize = 3f;
            float scaledSize = desiredSize / bs.Radius;

            for (int i = 0; i < m_bindingMatrices.Length; ++i)
            {
                m_bindingMatrices[i] = Matrix.Identity;
            }

            for (int i = 0; i < m_bindingQuaternions.Length; ++i)
            {
                m_bindingQuaternions[i] = Quaternion.Identity;
            }


            int counter = 0;
            foreach (Vector4 currentV4 in treePositions)
            {
                float scale2 = currentV4.W;// *(float)BulletGlobals.gRandom.NextDouble();

                Vector3 v = new Vector3(currentV4.X, currentV4.Y, currentV4.Z);

                // pick a random rotation for each tree as a base...

                float baseAngle = MathUtil.SIMD_2_PI * (float)Globals.s_random.NextDouble();
                Matrix rotationMatrix = Matrix.CreateRotationY(baseAngle);
                Matrix scaleMatrix = Matrix.CreateScale(scale2 * scaledSize);

                Matrix resultMatrix = scaleMatrix * rotationMatrix;
                resultMatrix.Translation = v;

                TreeBounds treeBounds = new TreeBounds(resultMatrix);

                m_treeBoundsQuadTree.AddObject(treeBounds);
            }

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public void DrawInstanced(BoundingFrustum frustum,VertexBuffer vertexBuffer,IndexBuffer indexBuffer,Effect effect,int numVertices,int numTriangles)
        {

            m_instanceTreeMatricesList.Clear();

            m_activeBoundsList.Clear();
            m_treeBoundsQuadTree.FindObjectsInsideFrustum(frustum, m_activeBoundsList);
            foreach (TreeBounds treeBounds in m_activeBoundsList)
            {
                m_instanceTreeMatricesList.Add(treeBounds.m_matrix);
            }

            if (m_instanceTreeMatricesList.Count > 0)
            {

                // If we have more instances than room in our vertex buffer, grow it to the neccessary size.
                if ((m_instanceVertexBuffer == null) ||
                    (m_instanceTreeMatricesList.Count > m_instanceVertexBuffer.VertexCount))
                {
                    if (m_instanceVertexBuffer != null)
                    {
                        m_instanceVertexBuffer.Dispose();
                    }

                    m_instanceVertexBuffer = new DynamicVertexBuffer(Globals.GraphicsDevice, s_instanceVertexDeclaration,
                                                                   m_instanceTreeMatricesList.Count, BufferUsage.WriteOnly);
                }


                // Tell the GPU to read from both the model vertex buffer plus our instanceVertexBuffer.
                Globals.GraphicsDevice.SetVertexBuffers(
                    new VertexBufferBinding(vertexBuffer, 0, 0),
                    new VertexBufferBinding(m_instanceVertexBuffer, 0, 1)
                );

                Globals.GraphicsDevice.Indices = indexBuffer;


                m_instanceVertexBuffer.SetData<Matrix>(m_instanceTreeMatricesList.GetRawArray(), 0, m_instanceTreeMatricesList.Count, SetDataOptions.Discard);


                // Draw all the instance copies in a single call.
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    Globals.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                           numVertices, 0,
                                                           numTriangles, m_instanceTreeMatricesList.Count);
                }
            }

        }


        public void DrawPrettyTrees()
        {
            if (Globals.Terrain.HeightMapTexture == null)
            { 
                return;
            }
            

            Globals.MCContentManager.SaveBlendState();

            Globals.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Globals.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            // Set up the instance rendering effect.
            Effect effect = m_simpleTree.TrunkEffect;

            effect.CurrentTechnique = effect.Techniques["TrunkHardwareInstancing"];

            Globals.MCContentManager.ApplyCommonEffectParameters(m_instanceTrunkEffect);

            effect.Parameters["Texture"].SetValue(m_simpleTree.TrunkTexture);
            effect.Parameters["LeafTexture"].SetValue(m_simpleTree.LeafTexture);

            m_simpleTree.Skeleton.CopyBoneBindingMatricesTo(m_bindingMatrices, m_bindingQuaternions);
            effect.Parameters["Bones"].SetValue(m_bindingMatrices);

            effect.Parameters["WorldMatrix"].SetValue(Matrix.Identity);
            effect.Parameters["HeightMapTexelWidth"].SetValue(Globals.Terrain.OneOverTextureWidth);
            effect.Parameters["HeightMapTexture"].SetValue(Globals.Terrain.HeightMapTexture);

            // Draw trunks....

            bool drawTrunks = true;

            if (drawTrunks)
            {
                DrawInstanced(m_prettyFrustum, m_simpleTree.TrunkMesh.VertexBuffer, m_simpleTree.TrunkMesh.IndexBuffer, m_simpleTree.TrunkEffect,
                    m_simpleTree.TrunkMesh.NumberOfVertices, m_simpleTree.TrunkMesh.NumberOfTriangles);
            }
            bool drawLeaves = false;
            if (drawLeaves)
            {
                // And leaves
                effect.CurrentTechnique = effect.Techniques["LeafHardwareInstancing"];
                effect.Parameters["LeafScale"].SetValue(0.1f);
                effect.Parameters["Bones"].SetValue(m_bindingMatrices);

                effect.Parameters["WorldMatrix"].SetValue(Matrix.Identity);

                effect.Parameters["BillboardRight"].SetValue(Vector3.Right);
                effect.Parameters["BillboardUp"].SetValue(Vector3.Up);


                // Tell the GPU to read from both the model vertex buffer plus our instanceVertexBuffer.
                Globals.GraphicsDevice.SetVertexBuffers(
                    new VertexBufferBinding(m_simpleTree.LeafCloud.VertexBuffer, 0, 0),
                    new VertexBufferBinding(m_instanceVertexBuffer, 0, 1)
                );

                Globals.GraphicsDevice.Indices = m_simpleTree.LeafCloud.IndexBuffer;


                try
                {
                    // Draw all the instance copies in a single call.
                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                        Globals.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                               m_simpleTree.LeafCloud.NumberOfVertices, 0,
                                                               m_simpleTree.LeafCloud.NumberOfTriangles, m_instanceTreeMatricesList.Count);
                    }
                }
                catch (System.Exception ex)
                {
                    int ibreak = 0;                
                }

            }
            effect.Parameters["HeightMapTexture"].SetValue((Texture)null);

            Globals.MCContentManager.RestoreBlendState();
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////////
        #region BillboardTrees
        public void BuildTreeMap(List<Vector4> treePositions)
        {
            // Based on example by Reimer at http://www.riemers.net/eng/Tutorials/XNA/Csharp/Series4/Region_growing.php
            int noiseTextureWidth = 256;

            RenderTarget2D treeTarget = new RenderTarget2D(Globals.GraphicsDevice, noiseTextureWidth, noiseTextureWidth, false, SurfaceFormat.Color, DepthFormat.Depth16);
            PerlinNoiseGenerator.GeneratePerlinNoise(noiseTextureWidth, treeTarget);
            Color[] pixels = new Color[noiseTextureWidth * noiseTextureWidth];
            treeTarget.GetData<Color>(pixels);
            int[,] noiseData = new int[noiseTextureWidth, noiseTextureWidth];
            for (int x = 0; x < noiseTextureWidth; x++)
            {
                for (int y = 0; y < noiseTextureWidth; y++)
                {
                    noiseData[x, y] = pixels[y + x * noiseTextureWidth].R;
                }
            }


            int stepSize = 4;


            Vector3 halfWidth = new Vector3(Globals.WorldWidth, 0, Globals.WorldWidth) / 2f;

            for (int z = 0; z < Globals.WorldWidth; z += stepSize)
            {
                for (int x = 0; x < Globals.WorldWidth; x += stepSize)
                {
                    //float height = GetHeightAtPointLocal(x, z);
                    float height = 0;
                    if (height > -4 && height < 20)
                    {
                        float relZ = (float)z / (float)Globals.WorldWidth;
                        float relX = (float)x / (float)Globals.WorldWidth;

                        float treeDensity = 0f;
                        float noiseAtPoint = noiseData[(int)(relX * noiseTextureWidth), (int)(relZ * noiseTextureWidth)];
                        if (noiseAtPoint > 200)
                        {
                            treeDensity = 5;
                        }
                        else if (noiseAtPoint > 150)
                        {
                            treeDensity = 4;
                        }
                        else if (noiseAtPoint > 100)
                        {
                            treeDensity = 3;
                        }
                        else
                        {
                            treeDensity = 0;
                        }

                        treeDensity *= 10;


                        for (int currDetail = 0; currDetail < treeDensity; currDetail++)
                        {
                            float rand1 = (float)Globals.s_random.NextDouble() * stepSize;
                            float rand2 = (float)Globals.s_random.NextDouble() * stepSize;

                            rand1 *= Globals.s_random.NextDouble() < 0.5 ? 1 : -1;
                            rand2 *= Globals.s_random.NextDouble() < 0.5 ? 1 : -1;


                            float scale = (float)Globals.s_random.NextDouble();
                            
                            //worldPosition
                            Vector3 tempPos = new Vector3((float)x - rand1, height, (float)z - rand2);
                            tempPos -= halfWidth;
                            Vector4 treePos = new Vector4(tempPos, scale);
                            treePositions.Add(treePos);
                        }
                    }

                }

            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public void BuildBillboardTrees()
        {
            int i = 0;

            float width = 1f;
            float height = 10f;
            // rotate through half to get a checkboard.
            //Matrix rotation = Matrix.CreateRotationY(MathUtil.SIMD_PI);

            int numIterations = 1;
            float stepSize = MathUtil.SIMD_2_PI / numIterations;

            VertexPositionTexture[] billboardVertices = new VertexPositionTexture[4];
            Vector3 left = new Vector3(-width, 0, 0);
            Vector3 right = new Vector3(width, 0, 0);
            Vector3 baseUp = new Vector3(0, height, 0);

            Matrix rotation = Matrix.Identity;
            float scale = 1f;

            billboardVertices[i++] = new VertexPositionTexture(Vector3.TransformNormal(left, rotation), new Vector2(0, 1));
            billboardVertices[i++] = new VertexPositionTexture(Vector3.TransformNormal(right, rotation), new Vector2(1, 1));
            billboardVertices[i++] = new VertexPositionTexture(Vector3.TransformNormal(right + baseUp, rotation), new Vector2(1, 0));
            billboardVertices[i++] = new VertexPositionTexture(Vector3.TransformNormal(left + baseUp, rotation), new Vector2(0, 0));

            short[] indices = new short[] { 0, 1, 2, 2, 3, 0 };

            m_billboardVertexBuffer  = new VertexBuffer(Globals.GraphicsDevice, VertexPositionTexture.VertexDeclaration, billboardVertices.Length, BufferUsage.WriteOnly);
            m_billboardVertexBuffer.SetData<VertexPositionTexture>(billboardVertices);

            m_billboardIndexBuffer = new IndexBuffer(Globals.GraphicsDevice, IndexElementSize.SixteenBits, indices.Length, BufferUsage.WriteOnly);
            m_billboardIndexBuffer.SetData<short>(indices);

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public override void Draw(GameTime gameTime)
        {
            DrawBillboardTrees();
            //DrawPrettyTrees();
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////

        public void DrawBillboardTrees()
        {
            m_billboardTreeEffect.CurrentTechnique = m_billboardTreeEffect.Techniques["BillboardTrees"];

            Matrix transform = Matrix.Identity;
            Globals.MCContentManager.ApplyCommonEffectParameters(m_billboardTreeEffect);

            m_billboardTreeEffect.Parameters["WorldMatrix"].SetValue(transform);
            m_billboardTreeEffect.Parameters["BillboardTreeTexture"].SetValue(m_treeTexture);
            m_billboardTreeEffect.Parameters["HeightMapTexelWidth"].SetValue(Globals.Terrain.OneOverTextureWidth);
            m_billboardTreeEffect.Parameters["HeightMapTexture"].SetValue(Globals.Terrain.HeightMapTexture);

            DrawInstanced(m_billboardFrustum, m_billboardVertexBuffer, m_billboardIndexBuffer, m_billboardTreeEffect, 4, 2);

        }
        #endregion


        // To store instance transform matrices in a vertex buffer, we use this custom
        // vertex type which encodes 4x4 matrices as a set of four Vector4 values.
        static VertexDeclaration s_instanceVertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
            new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 1),
            new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 2),
            new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 3)
        );


        private SimpleTree m_simpleTree;
        //private List<Vector4> m_treePositions = new List<Vector4>();
        //private Matrix[] m_instanceTreeMatrices = null;
        private ObjectArray<Matrix> m_instanceTreeMatricesList = new ObjectArray<Matrix>();
        private List<TreeBounds> m_activeBoundsList = new List<TreeBounds>();
        private float m_nearPrettyPlane = 1;
        private float m_farPrettyPlane = 20;
        private float m_farBillBoardPlane = 300;

        private BoundingFrustum m_prettyFrustum = new BoundingFrustum(Matrix.Identity);
        private BoundingFrustum m_billboardFrustum= new BoundingFrustum(Matrix.Identity);



        private Texture2D m_treeTexture;
        private Texture2D m_heightMapTexture;

        private Effect m_billboardTreeEffect;
        private Effect m_instanceTrunkEffect;
        private Effect m_instanceLeafEffect;

        private VertexBuffer m_treeVertexBuffer;
        private DynamicVertexBuffer m_instanceVertexBuffer;
        private Matrix[] m_bindingMatrices = new Matrix[4];
        private Quaternion[] m_bindingQuaternions = new Quaternion[4];
        private VertexBuffer m_billboardVertexBuffer;
        private IndexBuffer m_billboardIndexBuffer;


        private QuadTree<TreeBounds> m_treeBoundsQuadTree = new QuadTree<TreeBounds>(4,Vector3.Zero,(Globals.worldMaxPos - Globals.worldMinPos)/2f);




    }
        public class TreeBounds : ISpatialNode
        {
            public Matrix m_matrix = Matrix.Identity;

            public TreeBounds(Matrix m)
            {
                m_matrix = m;
            }

            public Vector3 Position
            {
                get{return m_matrix.Translation;}
            }

            public BoundingSphere BoundSphere
            {
                // need to get scale?
                get
                {
                    return new BoundingSphere(m_matrix.Translation,1f);
                }
            }
                    

        }
}