/*
* Created on 11-Jan-2006
*
* To change the template for this generated file go to
* Window - Preferences - Java - Code Generation - Code and Comments
*/
using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.collision;
using System.Collections.Generic;
using com.xexuxjy.magiccarpet.util;
using com.xexuxjy.magiccarpet.gameobjects;
using Microsoft.Xna.Framework.Graphics;
using BulletXNA.BulletCollision;
using com.xexuxjy.magiccarpet.interfaces;
using BulletXNA.LinearMath;
using com.xexuxjy.magiccarpet.renderer;
using BulletXNA;
using MagicCarpet.com.xexuxjy.magiccarpet.util;
using MagicCarpet.com.xexuxjy.magiccarpet.renderer;

namespace com.xexuxjy.magiccarpet.terrain
{
    public class Terrain : GameObject, ICollideable
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////

        public Terrain(IndexedVector3 position)
            : base(position, GameObjectType.terrain)
        {
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        virtual public int Width
        {
            get
            {
                return (int)(BoundingBox.Max.X - BoundingBox.Min.X);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        virtual public int Breadth
        {
            get
            {
                return (int)(BoundingBox.Max.Z - BoundingBox.Min.Z);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public void LoadOrCreateHeightMap(String textureName)
        {
            m_heightMap = new float[m_textureWidth * m_textureWidth];
            m_heightMapTexture = Globals.MCContentManager.GetTexture("TerrainHeightMap");
            //m_normalMapTexture = new Texture2D(Globals.GraphicsDevice, m_textureWidth, m_textureWidth, false, SurfaceFormat.Color);
            m_normalMapRenderTarget = (RenderTarget2D)Globals.MCContentManager.GetTexture("TerrainNormalMap");
            UpdateHeightMap();

        }


        ///////////////////////////////////////////////////////////////////////////////////////////////

        public override void BuildCollisionObject()
        {
            CollisionShape collisionShape = new HeightfieldTerrainShape(m_textureWidth, m_textureWidth, m_heightMap, 1f, -Globals.WorldHeight, Globals.WorldHeight, 1, true);
            CollisionFilterGroups collisionFlags = (CollisionFilterGroups)GameObjectType.terrain;
            CollisionFilterGroups collisionMask = (CollisionFilterGroups)(GameObjectType.spell | GameObjectType.manaball | GameObjectType.camera);
            m_collisionObject = Globals.CollisionManager.LocalCreateRigidBody(0f, IndexedMatrix.CreateTranslation(Position), collisionShape, m_motionState, true, this, collisionFlags, collisionMask);

        }


        ///////////////////////////////////////////////////////////////////////////////////////////////

        public override void Initialize()
        {
            m_terrainEffect = Globals.MCContentManager.GetEffect("Terrain");
            m_normalsEffect = Globals.MCContentManager.GetEffect("TerrainNormal");
            m_baseTexture = Globals.MCContentManager.GetTexture("TerrainBase");
            m_noiseTexture = Globals.MCContentManager.GetTexture("TerrainNoise");

            m_terrainEffect.Parameters["BaseTexture"].SetValue(m_baseTexture);
            m_terrainEffect.Parameters["NoiseTexture"].SetValue(m_noiseTexture);

            IndexedVector3 ambientLight = new IndexedVector3(0.1f);
            IndexedVector3 directionalLight = new IndexedVector3(0.4f);

            m_terrainEffect.Parameters["AmbientLight"].SetValue(ambientLight);
            m_terrainEffect.Parameters["DirectionalLight"].SetValue(directionalLight);
            m_terrainEffect.Parameters["LightPosition"].SetValue(new IndexedVector3(0, 40, 0));
            IndexedVector3 lightDirection = new IndexedVector3(10, -10, 0);
            lightDirection.Normalize();

            m_terrainEffect.Parameters["LightDirection"].SetValue(lightDirection);

            m_helperScreenQuad = new ScreenQuad(Game);
            m_helperScreenQuad.Initialize();

            BuildVertexBuffers();

            InitialiseWorldGrid();

            LoadOrCreateHeightMap(null);

            m_treeTexture = Globals.MCContentManager.GetTexture("TreeBillboard");

            base.Initialize();

            // after init so we get the right draw order.
            DrawOrder = Globals.TERRAIN_DRAW_ORDER;

            m_noCullState = new RasterizerState();
            m_noCullState.CullMode = CullMode.None;

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public void BuildVertexBuffers()
        {
            int adjustedVertices = (m_blockSize * m_multiplier) + 1;
            int adjustedIndices = (m_blockSize * m_multiplier);


            PosOnlyVertex[] blockVertices = new PosOnlyVertex[adjustedVertices * adjustedVertices];
            int[] blockIndices = new int[adjustedIndices * adjustedIndices * 6];
            int indexCounter = 0;
            int vertexCounter = 0;
            int stride = adjustedVertices;


            for (int y = 0; y < adjustedVertices; ++y)
            {
                for (int x = 0; x < adjustedVertices; ++x)
                {
                    Vector2 v = new Vector2(x, y) * m_oneOver;
                    PosOnlyVertex vpnt = new PosOnlyVertex(v);

                    blockVertices[vertexCounter++] = vpnt;
                    if (x < adjustedIndices && y < adjustedIndices)
                    {
                        blockIndices[indexCounter++] = (x + (y * stride));
                        blockIndices[indexCounter++] = (x + 1 + (y * stride));
                        blockIndices[indexCounter++] = (x + 1 + ((y + 1) * stride));

                        blockIndices[indexCounter++] = (x + 1 + ((y + 1) * stride));
                        blockIndices[indexCounter++] = (x + ((y + 1) * stride));
                        blockIndices[indexCounter++] = (x + (y * stride));
                    }
                }
            }

            m_blockVertexBuffer = new VertexBuffer(Globals.GraphicsDevice, PosOnlyVertex.VertexDeclaration, blockVertices.Length, BufferUsage.None);
            m_blockVertexBuffer.SetData<PosOnlyVertex>(blockVertices, 0, blockVertices.Length);
            m_blockIndexBuffer = new IndexBuffer(Globals.GraphicsDevice, IndexElementSize.ThirtyTwoBits, blockIndices.Length, BufferUsage.None);
            m_blockIndexBuffer.SetData<int>(blockIndices);

            BuildTreeMap();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public void UpdateHeightMapTexture()
        {
            if (HasTerrainChanged())
            {
                m_normalsEffect.Parameters["HeightMapTexture"].SetValue((Texture)null);
                m_terrainEffect.Parameters["HeightMapTexture"].SetValue((Texture)null);

                // need these here to unset the textures so I can change them.
                foreach (EffectPass pass in m_terrainEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                }

                foreach (EffectPass pass in m_normalsEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                }

                m_heightMapTexture.SetData<Single>(m_heightMap);
                m_terrainEffect.Parameters["HeightMapTexture"].SetValue(m_heightMapTexture);

                UpdateNormalMap();


                ClearTerrainChanged();
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public void UpdateNormalMap()
        {
            //m_terrainEffect.Parameters["NormalMapTexture"].SetValue((Texture)null);

            int width = m_heightMapTexture.Width;

            Globals.GraphicsDevice.SetRenderTarget(m_normalMapRenderTarget);
            Globals.GraphicsDevice.Clear(Color.White);

            m_normalsEffect.CurrentTechnique = m_normalsEffect.Techniques["ComputeNormals"];
            m_normalsEffect.Parameters["HeightMapTexture"].SetValue(m_heightMapTexture);
            float texelWidth = 1f / (float)width;
            m_normalsEffect.Parameters["TexelWidth"].SetValue(texelWidth);

            Matrix projection = Matrix.CreateOrthographicOffCenter(0, width, width, 0, 0, 1);
            Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

            m_normalsEffect.Parameters["MatrixTransform"].SetValue(halfPixelOffset * projection);

            foreach (EffectPass pass in m_normalsEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                m_helperScreenQuad.Draw();
            }

            //m_normalsEffect.Parameters["HeightMapTexture"].SetValue((Texture)null);

            Globals.GraphicsDevice.SetRenderTarget(null);
            m_terrainEffect.Parameters["NormalMapTexture"].SetValue(m_normalMapRenderTarget);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public override void Draw(GameTime gameTime)
        {
            UpdateHeightMapTexture();


            Matrix viewProjection = Globals.Camera.Projection * Globals.Camera.View;
            BoundingFrustum boundingFrustrum = new BoundingFrustum(viewProjection);

            //float oneOverTextureWidth = 1f/m_textureWidth;

            //IndexedVector3 maxPos = IndexedVector3.Zero;

            IndexedVector3 lastStartPosition = IndexedVector3.Zero;

            m_terrainEffect.Parameters["CameraPosition"].SetValue(Globals.Camera.Position);
            m_terrainEffect.Parameters["FogEnabled"].SetValue(true);
            m_terrainEffect.Parameters["FogStart"].SetValue(20);
            m_terrainEffect.Parameters["FogEnd"].SetValue(200);
            Vector4 scaleFactor = new Vector4(m_oneOver);
            m_terrainEffect.Parameters["ScaleFactor"].SetValue(scaleFactor);
            DrawTerrainBlocks(boundingFrustrum,ref viewProjection);
            DrawTrees(boundingFrustrum, ref viewProjection);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public void DrawTerrainBlocks(BoundingFrustum boundingFrustrum,ref Matrix viewProjection)
        {
            int numSpans = Globals.WorldWidth / m_blockSize;
            IndexedVector3 blockSize = new IndexedVector3(m_blockSize, 0, m_blockSize);
            IndexedVector3 startPosition = new IndexedVector3(-Globals.WorldWidth * 0.5f, 0, -Globals.WorldWidth * 0.5f);
            float oneOverTextureWidth = 1f / (m_textureWidth - 1);

            m_terrainEffect.CurrentTechnique = m_terrainEffect.Techniques["TileTerrain"];
            Globals.GraphicsDevice.Indices = m_blockIndexBuffer;
            Globals.GraphicsDevice.SetVertexBuffer(m_blockVertexBuffer);

            foreach (EffectPass pass in m_terrainEffect.CurrentTechnique.Passes)
            {
                for (int j = 0; j < numSpans; ++j)
                {
                    for (int i = 0; i < numSpans; ++i)
                    {
                        IndexedVector3 localPosition = new IndexedVector3((m_blockSize) * i, 0, (m_blockSize) * j);

                        IndexedVector3 worldPosition = localPosition + startPosition;

                        IndexedVector3 minbb = new IndexedVector3(worldPosition.X, -Globals.WorldHeight, worldPosition.Z);
                        IndexedVector3 maxbb = minbb + blockSize;
                        maxbb.Y = Globals.WorldHeight;

                        BoundingBox bb = new BoundingBox(minbb, maxbb);

                        if (boundingFrustrum.Intersects(bb))
                        {

                            Matrix transform = Matrix.CreateTranslation(startPosition) * viewProjection;

                            m_terrainEffect.Parameters["WorldViewProjMatrix"].SetValue(transform);
                            m_terrainEffect.Parameters["FineTextureBlockOrigin"].SetValue(new Vector4(oneOverTextureWidth, oneOverTextureWidth, localPosition.X, localPosition.Z));

                            // need apply on inner level to make sure latest vals copied across
                            pass.Apply();
                            Globals.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, m_blockVertexBuffer.VertexCount, 0, m_blockIndexBuffer.IndexCount / 3);

                        }
                        else
                        {
                            int ibreak = 0;
                        }

                    }
                }
            }

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public void DrawTrees(BoundingFrustum boundingFrustrum,ref Matrix viewProjection)
        {
            m_terrainEffect.CurrentTechnique = m_terrainEffect.Techniques["BillboardTrees"];
            Globals.GraphicsDevice.SetVertexBuffer(m_treeVertexBuffer);

            RasterizerState oldState = Globals.GraphicsDevice.RasterizerState;
            Globals.GraphicsDevice.RasterizerState = m_noCullState;

            Vector3 startPosition = Vector3.Zero;//new Vector3(-Globals.WorldWidth / 2f, 0, -Globals.WorldWidth / 2f);
            Matrix transform = Matrix.CreateTranslation(startPosition) * viewProjection;

            m_terrainEffect.Parameters["WorldViewProjMatrix"].SetValue(transform);
            m_terrainEffect.Parameters["TreeTexture"].SetValue(m_treeTexture);
         
            float oneOverTextureWidth = 1f / (m_textureWidth - 1);
            m_terrainEffect.Parameters["FineTextureBlockOrigin"].SetValue(new Vector4(oneOverTextureWidth, oneOverTextureWidth, 0, 0));

            BlendState oldBlendState = Globals.GraphicsDevice.BlendState;
            Globals.GraphicsDevice.BlendState = BlendState.AlphaBlend;


            foreach (EffectPass pass in m_terrainEffect.CurrentTechnique.Passes)
            {
                int noVertices = m_treeVertexBuffer.VertexCount;
                int noTriangles = noVertices / 3;
                pass.Apply();
                Globals.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, noTriangles);
            }

            Globals.GraphicsDevice.RasterizerState = oldState;
            Globals.GraphicsDevice.BlendState = oldBlendState;          
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        protected virtual void InitialiseWorldGrid()
        {
            m_terrainSquareGrid = new TerrainSquare[Globals.WorldWidth * Globals.WorldWidth];
            for (int i = 0; i < m_terrainSquareGrid.Length; ++i)
            {
                m_terrainSquareGrid[i] = new TerrainSquare();
            }

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public void AddPeak(IndexedVector3 point, float height)
        {
            point = WorldToLocal(point);
            AddPeak(point.X, point.Z, height);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////

        public void AddPeak(float x, float y, float height)
        {
            float defaultRadius = 15.0f;
            AddPeak(x, y, defaultRadius, height);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void AddPeak(float x, float z, float radius, float height)
        {
            //TerrainUpdater terrainUpdate = new TerrainUpdater(new IndexedVector3(x, 0, z), radius, s_terrainMoveTime, height, this);
            //m_terrainUpdaters.Add(terrainUpdate);
            TerrainUpdater.ApplyImmediate(new IndexedVector3(x, 0, z), radius, height, this);
            UpdateHeightMap();

        }
        ///////////////////////////////////////////////////////////////////////////////////////////////

        public virtual float GetHeightAtPointWorld(IndexedVector3 point)
        {
            // straight down
            float result = GetHeightAtPointWorld(point.X, point.Z);
            return result;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////

        public void SetHeightAtPointWorld(ref IndexedVector3 worldPoint)
        {
            IndexedVector3 local = WorldToLocal(worldPoint);
            int localX = (int)local.X;
            int localZ = (int)local.Z;
            SetHeightAtPointLocal(localX, localZ, worldPoint.Y);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public void SetHeightAtPointLocal(int localX, int localZ, float height)
        {
            Debug.Assert(localX >= 0 && localX <= Globals.WorldWidth);
            Debug.Assert(localZ >= 0 && localZ <= Globals.WorldWidth);
            //m_heightMap[(localZ * Globals.WorldWidth) + localX] = height;
            m_heightMap[(localZ * m_textureWidth) + localX] = height;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public virtual float GetHeightAtPointWorld(float x, float z)
        {
            IndexedVector3 local = WorldToLocal(new IndexedVector3(x, 0, z));
            int localX = (int)local.X;
            int localZ = (int)local.Z;
            return GetHeightAtPointLocal(localX, localZ);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public virtual float GetHeightAtPointLocal(int localX, int localZ)
        {
            Debug.Assert(localX >= 0 && localX <= Globals.WorldWidth);
            Debug.Assert(localZ >= 0 && localZ <= Globals.WorldWidth);
            //return m_heightMap[(localZ * Globals.WorldWidth) + localX];
            return m_heightMap[(localZ * m_textureWidth) + localX];
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public virtual void BuildRandomLandscape()
        {
            int counter = 0;
            int increment = 1;
            int maxHills = Width;
            int maxInstanceHeight = 10;
            int maxRadius = 50;
            int currentHills = 0;
            while (currentHills++ < maxHills)
            {
                if (counter == 5)
                {
                    increment = -1;
                }
                else if (counter == 0)
                {
                    increment = 1;
                }
                counter += increment;
                int xpos = (int)((float)Globals.s_random.NextDouble() * Width);
                int ypos = (int)((float)Globals.s_random.NextDouble() * Breadth);
                float radius = ((float)Globals.s_random.NextDouble() * maxRadius);
                // don't want too small a radius
                if (radius < 5f)
                {
                    radius = 5f;
                }


                float height = ((float)Globals.s_random.NextDouble() * maxInstanceHeight);
                bool up = (float)Globals.s_random.NextDouble() > 0.5;
                if (!up)
                {
                    height = -height;
                }


                TerrainUpdater.ApplyImmediate(new IndexedVector3(xpos, 0, ypos), radius, height, this);
            }

            UpdateHeightMap();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void ToggleHeightMethod()
        {
            m_defaultHeightMethod = !m_defaultHeightMethod;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        //// used to put a castle on the land, this will take the center of the castle and figure out 
        //// how much space it will need , flatten the land etc.
        //public void addCastle(Castle theCastle)
        //{
        //    IndexedVector3 castlePosition = theCastle.Position;
        //    BoundingBox box = theCastle.BoundingBox;
        //    int left = (int)box.Min.X;
        //    int right = (int)box.Max.X;
        //    int top = (int)box.Min.Z;
        //    int bottom = (int)box.Max.Z;

        //    // If it will fit then add it. otherwise no
        //    if((left > 0 ) && (right < Width) && (top > 0 ) && (bottom < Breadth))
        //    {
        //        // hmm what should the height be? height at 'centre' of castle
        //        float height = getHeightAtPoint(castlePosition);
        //        // go through and set the height on the squares
        //        for(int i=left;i<right;++i)
        //        {
        //            for(int j=top;j<bottom;++j)
        //            {
        //                TerrainSquare terrainSquare = getTerrainSquareAtPoint(i,j);
        //                terrainSquare.setTargetHeight(height);
        //                terrainSquare.Type = TerrainType.castle;
        //            }
        //        }
        //        IndexedVector3 castlePosition2 = new IndexedVector3();
        //        castlePosition2.X = theCastle.Position.X;
        //        castlePosition2.Z = theCastle.Position.Z;
        //        castlePosition2.Y = height;

        //        theCastle.Position = castlePosition2;
        //        m_terrainHasChanged = true;
        //    }
        //    else
        //    {
        //        // won't fit.
        //    }
        //}

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public TerrainSquare GetTerrainSquareAtPointWorld(ref IndexedVector3 worldPoint)
        {
            IndexedVector3 local = worldPoint - BoundingBox.Min;
            int localX = (int)local.X;
            int localZ = (int)local.Z;
            return GetTerrainSquareAtPointLocal(localX, localZ);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public TerrainSquare GetTerrainSquareAtPointLocal(int localX, int localZ)
        {
            Debug.Assert(localX >= 0 && localX < Globals.WorldWidth);
            Debug.Assert(localZ >= 0 && localZ < Globals.WorldWidth);
            return m_terrainSquareGrid[(localZ * Globals.WorldWidth) + localX];
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public bool HasTerrainChanged()
        {
            return m_terrainHasChanged;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public void ClearTerrainChanged()
        {
            m_terrainHasChanged = false;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public void SetTerrainTypeAndOccupier(IndexedVector3 position, TerrainType terrainType, GameObject occupier)
        {
            TerrainSquare square = GetTerrainSquareAtPointWorld(ref position);
            square.Type = terrainType;
            square.Occupier = occupier;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public void SetTerrainTypeOld(TerrainSquare terrainSquare, float val)
        {
            TerrainType result = TerrainType.water;
            if (val <= -5.0f)
            {
                result = TerrainType.deepwater;
            }
            else if (val > -5.0f && val <= -2.0f)
            {
                result = TerrainType.water;
            }
            else if (val > -2.0f && val <= 0.0f)
            {
                result = TerrainType.shallowwater;
            }
            else
                if (val > 0.0f && val <= 1.0f)
                {
                    result = TerrainType.beach;
                }
                else
                    if (val > 1.0f && val <= 3.0f)
                    {
                        result = TerrainType.grass;
                    }
                    else
                        if (val > 3.0f && val <= 5.0f)
                        {
                            result = TerrainType.grass2;
                        }
                        else
                            if (val > 5.0f && val <= 8.0f)
                            {
                                result = TerrainType.rock;
                            }
                            else
                                if (val > 8.0f)
                                {
                                    result = TerrainType.ice;
                                }
            terrainSquare.Type = result;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public bool IsPointInTerrain(ref IndexedVector3 point)
        {
            return ((point.X >= 0.0f && point.X <= Width) && (point.Z >= 0.0f && point.Z <= Breadth));

        }
        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public void AssertPointInTerrain(ref IndexedVector3 point)
        {
            //Debug.Assert(isPointInTerrain(ref point), "Point not in terrain");
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public override void Update(GameTime gameTime)
        {
            // go through and adjust our current and target heights to make the landscape move nicely.
            base.Update(gameTime);
            bool terrainChanged = false; ;
            foreach (TerrainUpdater terrainUpdate in m_terrainUpdaters)
            {
                terrainUpdate.Update(gameTime);
                if (!terrainUpdate.Complete())
                {
                    terrainUpdate.ApplyToTerrain();
                    terrainChanged = true;
                }
                else
                {
                    m_terrainUpdatersRemove.Add(terrainUpdate);
                }
            }

            foreach (TerrainUpdater terrainUpdate in m_terrainUpdatersRemove)
            {
                m_terrainUpdaters.Remove(terrainUpdate);
            }

            if (terrainChanged)
            {
                UpdateHeightMap();
            }

            m_terrainUpdatersRemove.Clear();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public TerrainSquare[] TerrainSquares
        {
            get { return m_terrainSquareGrid; }
            set { m_terrainSquareGrid = value; }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public IndexedVector3 GetRandomWorldPositionXZ()
        {
            IndexedVector3 result = new IndexedVector3();
            result.X = ((float)Globals.s_random.NextDouble() * Width);
            result.Z = ((float)Globals.s_random.NextDouble() * Breadth);
            result.Y = GetHeightAtPointLocal((int)result.X, (int)result.Z);
            return LocalToWorld(result);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public IndexedVector3 GetRandomWorldPositionXZWithRange(IndexedVector3 position, float distance)
        {
            IndexedVector3 result = new IndexedVector3();
            float sign = Globals.s_random.NextDouble() > 0.5f ? 1.0f : -1.0f;
            result.X = position.X + (sign * ((float)Globals.s_random.NextDouble() * distance));
            result.Z = position.Z + (sign * ((float)Globals.s_random.NextDouble() * distance));

            return LocalToWorld(result);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public void ClampToTerrain(ref IndexedVector3 position)
        {
            IndexedVector3 local = WorldToLocal(position);


            if (Globals.TerrainWrapEnabled)
            {
                // wrap the position
                if (local.X < 0)
                {
                    local.X += Width;
                }
                if (local.X > Width)
                {
                    local.X -= Width;
                }
                if (local.Z < 0)
                {
                    local.Z += Width;
                }
                if (local.Z > Width)
                {
                    local.Z -= Width;
                }
            }
            else
            {
                local.X = MathHelper.Clamp(local.X, 0, Width);
                local.Z = MathHelper.Clamp(local.Z, 0, Width);
            }

            position = LocalToWorld(local);
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////	

        // simple terrain with two levels
        public void BuildTestTerrain1()
        {
            for (int z = 0; z < m_textureWidth; ++z)
            {
                for (int x = 0; x < m_textureWidth / 2; ++x)
                {
                    m_heightMap[(z * m_textureWidth) + x] = 3.0f;
                }
            }
            UpdateHeightMap();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public void UpdateHeightMap()
        {
            m_terrainHasChanged = true;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public float TerrainMoveTime
        {
            get { return s_terrainMoveTime; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public void BuildFractalLandscape()
        {
            //m_heightMap = FractalUtil.GetRandomHeightData(m_textureWidth);
            m_heightMap = new float[m_textureWidth * m_textureWidth];
            float height = 10.0f;
            float smoothness = 0.7f;
            FractalUtil.Fill2DFractArray(m_heightMap, m_textureWidth - 1, 1, height, smoothness);
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public void BuildTreeMap()
        {
            // Based on example by Reimer at http://www.riemers.net/eng/Tutorials/XNA/Csharp/Series4/Region_growing.php
            int noiseTextureWidth = 128;

            RenderTarget2D treeTarget = new RenderTarget2D(Globals.GraphicsDevice, noiseTextureWidth, noiseTextureWidth, false,SurfaceFormat.Color, DepthFormat.Depth16);
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


            Vector3 halfWidth = new Vector3(Globals.WorldWidth, 0, Globals.WorldWidth)/2f;

            for (int z = 0; z < Globals.WorldWidth; z+=stepSize)
            {
                for (int x = 0; x < Globals.WorldWidth; x+=stepSize)
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


                        for (int currDetail = 0; currDetail < treeDensity; currDetail++)
                        {
                            float rand1 = (float)Globals.s_random.Next(1000) / 1000.0f;
                            float rand2 = (float)Globals.s_random.Next(1000) / 1000.0f;
                            float scale = (float)Globals.s_random.NextDouble();
                            //worldPosition
                            Vector3 tempPos = new Vector3((float)x - rand1, height, -(float)z - rand2);
                            tempPos -= halfWidth;
                            Vector4 treePos = new Vector4(tempPos,scale);
                            m_treePositions.Add(treePos);
                        }
                    }

                }

            }
            CreateBillboardVerticesFromList(m_treePositions, out m_treeVertexBuffer);

        }

        public void CreateBillboardVerticesFromList(List<Vector4> list,out VertexBuffer vertexBuffer)
        {
            VertexPositionScaleTexture[] billboardVertices = new VertexPositionScaleTexture[list.Count * 12];
            int i = 0;

            float width = 1f;
            float height = 10f;
            // rotate through half to get a checkboard.
            Matrix rotation = Matrix.CreateRotationY(MathUtil.SIMD_HALF_PI);

            Vector3 left = new Vector3(-width, 0, 0);
            Vector3 right = new Vector3(width, 0, 0);
            Vector3 baseUp = new Vector3(0, height, 0);


            foreach (Vector4 currentV4 in list)
            {
                Vector3 v = new Vector3(currentV4.X,currentV4.Y,currentV4.Z);
                float scale = currentV4.W;
                Vector3 up = baseUp * scale;


                billboardVertices[i++] = new VertexPositionScaleTexture(v+left, scale,new Vector2(0, 1));
                billboardVertices[i++] = new VertexPositionScaleTexture(v + right , scale, new Vector2(1, 1));
                billboardVertices[i++] = new VertexPositionScaleTexture(v + right + up, scale, new Vector2(1, 0));

                billboardVertices[i++] = new VertexPositionScaleTexture(v + right+up, scale, new Vector2(1, 0));
                billboardVertices[i++] = new VertexPositionScaleTexture(v + left+ up, scale, new Vector2(0, 0));
                billboardVertices[i++] = new VertexPositionScaleTexture(v + left, scale, new Vector2(0, 1));

                
                billboardVertices[i++] = new VertexPositionScaleTexture(v+Vector3.TransformNormal(left, rotation), scale, new Vector2(0, 1));
                billboardVertices[i++] = new VertexPositionScaleTexture(v+Vector3.TransformNormal(right,  rotation), scale, new Vector2(1, 1));
                billboardVertices[i++] = new VertexPositionScaleTexture(v+ Vector3.TransformNormal(right + up, rotation), scale, new Vector2(1, 0));

                billboardVertices[i++] = new VertexPositionScaleTexture(v + Vector3.TransformNormal(right+up, rotation), scale, new Vector2(1, 0));
                billboardVertices[i++] = new VertexPositionScaleTexture(v + Vector3.TransformNormal(left + up, rotation), scale, new Vector2(0, 0));
                billboardVertices[i++] = new VertexPositionScaleTexture(v + Vector3.TransformNormal(left, rotation), scale, new Vector2(0, 1));

            }

            vertexBuffer = new VertexBuffer(Globals.GraphicsDevice, VertexPositionScaleTexture.VertexDeclaration, billboardVertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData(billboardVertices);
        
        
        
        
        
        }




        ///////////////////////////////////////////////////////////////////////////////////////////////	

        private struct TerrainUpdater
        {
            public TerrainUpdater(IndexedVector3 position, float radius, float totalTime, float totalDeflection, Terrain terrain)
            {
                m_terrain = terrain;
                m_positionLocal = position;
                m_updateDeflection = 0f;

                BoundingBox terrainBB = m_terrain.BoundingBox;

                // need to adjust position based on midpoint of terrain
                //m_position -= new IndexedVector3(CommonSettings.worldWidth / 2, 0, CommonSettings.worldBreadth / 2);
                m_radius = radius;
                m_totalTime = totalTime;
                m_totalDeflection = totalDeflection;
                m_currentTime = 0f;


                m_minX = (int)System.Math.Max(0, position.X - radius);
                m_maxX = (int)System.Math.Min(Globals.WorldWidth, position.X + radius);
                m_minZ = (int)System.Math.Max(0, position.Z - radius);
                m_maxZ = (int)System.Math.Min(Globals.WorldWidth, position.Z + radius);
            }


            public static void ApplyImmediate(IndexedVector3 position, float radius, float totalDeflection, Terrain terrain)
            {
                int minX = (int)System.Math.Max(0, position.X - radius);
                int maxX = (int)System.Math.Min(Globals.WorldWidth, position.X + radius);
                int minZ = (int)System.Math.Max(0, position.Z - radius);
                int maxZ = (int)System.Math.Min(Globals.WorldWidth, position.Z + radius);

                float floatRadius2 = radius * radius;
                for (int j = minZ; j < maxZ; j++)
                {
                    for (int i = minX; i < maxX; i++)
                    {
                        IndexedVector3 worldPoint = new IndexedVector3(i, 0, j);
                        IndexedVector3 diff = worldPoint - position;
                        float diffLength2 = diff.LengthSquared();
                        if (diffLength2 < floatRadius2)
                        {
                            float lerpValue = (floatRadius2 - diffLength2) / floatRadius2;
                            lerpValue = (float)Math.Pow(lerpValue, 4f);
                            lerpValue = (float)Math.Sin(MathUtil.SIMD_HALF_PI * lerpValue);
                            //lerpValue += 1.0f;

                            // play with lerp value to smooth the terrain?
                            //                          lerpValue = (float)Math.Sqrt(lerpValue);
                            //lerpValue *= lerpValue;
                            //lerpValue = (float)Math.Pow(lerpValue, 4f);
                            // ToDo - fractal hill generation.
                            float currentHeight = terrain.GetHeightAtPointLocal(i, j);
                            //float oldHeight = getHeightAtPoint(i, j);
                            float newHeight = currentHeight + (totalDeflection * lerpValue);

                            newHeight = MathHelper.Clamp(newHeight, -Globals.WorldHeight, Globals.WorldHeight);
                            terrain.SetHeightAtPointLocal(i, j, newHeight);
                        }
                    }
                }
            }


            public void ApplyToTerrain()
            {
                if (m_currentTime < m_totalTime)
                {
                    float floatRadius2 = m_radius * m_radius;
                    for (int j = m_minZ; j < m_maxZ; j++)
                    {
                        for (int i = m_minX; i < m_maxX; i++)
                        {
                            IndexedVector3 worldPoint = new IndexedVector3(i, 0, j);
                            IndexedVector3 diff = worldPoint - m_positionLocal;
                            float diffLength2 = diff.LengthSquared();
                            if (diffLength2 < floatRadius2)
                            {
                                float lerpValue = (floatRadius2 - diffLength2) / floatRadius2;
                                lerpValue = (float)Math.Pow(lerpValue, 1.5f);
                                float currentHeight = m_terrain.GetHeightAtPointLocal(i, j);
                                float newHeight = currentHeight + (m_updateDeflection * lerpValue);

                                newHeight = MathHelper.Clamp(newHeight, -Globals.WorldHeight, Globals.WorldHeight);
                                m_terrain.SetHeightAtPointLocal(i, j, newHeight);
                            }
                        }
                    }
                }
            }


            public void Update(GameTime gameTime)
            {
                float timeStep = (float)gameTime.ElapsedGameTime.TotalSeconds;
                m_currentTime += timeStep;
                m_updateDeflection = (timeStep / m_totalTime) * m_totalDeflection;
            }

            public bool Complete()
            {
                return m_currentTime > m_totalTime;
            }



            private Terrain m_terrain;
            private IndexedVector3 m_positionLocal;
            //private IndexedVector3 m_midPoint;

            private float m_radius;
            private float m_totalTime;
            private float m_currentTime;
            private float m_totalDeflection;
            private float m_updateDeflection;
            int m_minX;
            int m_maxX;
            int m_minZ;
            int m_maxZ;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        private TerrainSquare[] m_terrainSquareGrid;
        private float[] m_heightMap;

        private List<TerrainUpdater> m_terrainUpdaters = new List<TerrainUpdater>();
        private List<TerrainUpdater> m_terrainUpdatersRemove = new List<TerrainUpdater>();
        private List<Vector4> m_treePositions = new List<Vector4>();


        private bool m_defaultHeightMethod = true;
        private bool m_terrainHasChanged;
        private float m_maxCoverage = 0.65f;
        private float m_minIslandSize = 5.0f;
        private float m_maxIslandSize = 15.0f;

        // the time taken for the complete terrain move.
        private float s_terrainMoveTime = 5.0f;

        const int m_blockSize = 256;
        const int m_textureWidth = Globals.WorldWidth + 1;

        const int m_multiplier = 1;
        const float m_oneOver = 1f / (float)(m_multiplier);


        VertexBuffer m_blockVertexBuffer;
        IndexBuffer m_blockIndexBuffer;
        Effect m_terrainEffect;
        Effect m_normalsEffect;

        VertexBuffer m_treeVertexBuffer;


        RasterizerState m_rasterizerState;
        Texture2D m_heightMapTexture;

        RenderTarget2D m_normalMapRenderTarget;

        ScreenQuad m_helperScreenQuad;

        Texture2D m_baseTexture;
        Texture2D m_noiseTexture;
        Texture2D m_portalTexture;
        Texture2D m_treeTexture;

        RasterizerState m_noCullState;


        public int GetCollisionMask()
        {
            throw new NotImplementedException();
        }

        public bool ShouldCollideWith(ICollideable partner)
        {
            return true;
        }


        public Texture2D BaseTexture
        {
            get { return m_baseTexture; }
        }
    }

    public struct PosOnlyVertex : IVertexType
    {

        public PosOnlyVertex(Vector2 v)
        {
            Position = v;
        }

        public Vector2 Position;

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        );

        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }
    };

    public struct VertexPositionScaleTexture : IVertexType
    {

        public VertexPositionScaleTexture(Vector3 v,float s,Vector2 uv)
        {
            Position = v;
            Scale = s;
            UV = uv;
        }

        public Vector3 Position;
        public float Scale;
        public Vector2 UV;

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(sizeof(float) * 4,VertexElementFormat.Vector2,VertexElementUsage.TextureCoordinate,1)
        );

        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }
    };







}