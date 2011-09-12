using Microsoft.Xna.Framework;
using Dhpoware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace ClipTerrainDemo
{
    public class ClipLevel
    {
        public ClipLevel(int scaleLevel, ClipLevelManager clipLevelManager)
        {
            m_scaleLevel = scaleLevel;

            m_clipLevelManager = clipLevelManager;
            m_graphicsDevice = m_clipLevelManager.Game.GraphicsDevice;

            Vector3 min = new Vector3(0, -m_clipLevelManager.m_maxHeight, 0);
            // if we're after the last one then set our start pos based on previous minimum
            if (m_scaleLevel > 0)
            {
                ClipLevel previous = m_clipLevelManager.GetClipLevel(m_scaleLevel - 1);
                BoundingBox previousBB = previous.BoundingBox;
                min = previousBB.Min;
                min -= new Vector3(m_clipLevelManager.m_blockSize * ScaleFactor, 0, m_clipLevelManager.m_blockSize * ScaleFactor);
                m_offset = min;
            }

            Vector3 max = new Vector3(m_clipLevelManager.m_clipMapSize, 0, m_clipLevelManager.m_clipMapSize);
            max *= ScaleFactor;
            max.Y = m_clipLevelManager.m_maxHeight;

            m_totalBoundingBox = new BoundingBox(min, max);
            BuildBoundingBoxes();
            BuildTextures();
        }

        public void Create(float lastShift)
        {
            float gridSpacing = ScaleFactor;
            float shifting = GridShifting(m_scaleLevel, lastShift);
            m_hpos = m_vpos = m_scaleLevel % 2;
            m_vectorShift = (float)Math.Pow(2.0f, (float)(m_scaleLevel + 1));
            m_vectorScale = new Vector4(gridSpacing, gridSpacing, -(m_clipLevelManager.m_clipMapSize / 2) * gridSpacing + shifting, -(m_clipLevelManager.m_clipMapSize / 2) * gridSpacing + shifting);
            m_aabbDiff = new Vector2(m_clipLevelManager.m_blockSize - 1 * gridSpacing, m_clipLevelManager.m_blockSize - 1 * gridSpacing);
            m_textureScale = new Vector2(0.0f, 0.0f);
            m_texTorusOrigin.X = m_texTorusOrigin.Y = m_lastTexTorusOrigin.X = m_lastTexTorusOrigin.Y = m_clipLevelManager.m_np1;

            // Create elevation map
            m_heightMap = new RenderTarget2D(m_graphicsDevice, m_clipLevelManager.m_clipTexSize, m_clipLevelManager.m_clipTexSize, false, SurfaceFormat.Single,DepthFormat.None);
            m_normalMap = new RenderTarget2D(m_graphicsDevice, m_clipLevelManager.m_texNormalSize, m_clipLevelManager.m_texNormalSize, false, SurfaceFormat.Color,DepthFormat.None);
        }







        public void BuildTextures()
        {
            m_heightMap = new RenderTarget2D(m_graphicsDevice, m_clipLevelManager.m_clipTexSize, m_clipLevelManager.m_clipTexSize, false, SurfaceFormat.Single, DepthFormat.None);
            //m_normalMap = new Texture2D(m_graphicsDevice, m_clipLevelManager.m_texNormalSize, m_clipLevelManager.m_texNormalSize, false, SurfaceFormat.Color);


            Vector2 textureSize = new Vector2(m_clipLevelManager.m_heightMapTexture.Width, m_clipLevelManager.m_heightMapTexture.Height);

            BoundingBox maxExtents = m_totalBoundingBox;
            BoundingBox localExtents = BoundingBox;

            Vector3 maxSpan = maxExtents.Max - maxExtents.Min;
            Vector3 localSpan = localExtents.Max - localExtents.Min;

            float scaling = localSpan.X / maxSpan.X;

            // starting corner of this texture in full texture
            Vector2 textureOffset = new Vector2(Math.Abs(maxExtents.Min.X - localExtents.Min.X) * scaling);

            float oneOverStepSizeLocal = 1f / (float)m_clipLevelManager.m_clipMapSize;
            float oneOverStepSizeTotal = 1f / (float)m_clipLevelManager.m_clipMapSize;

            Matrix orthographic = Matrix.CreateOrthographic(m_clipLevelManager.m_clipMapSize, m_clipLevelManager.m_clipMapSize, 0, 1);

            m_clipLevelManager.m_upsamplerEffect.Parameters["lookupTexture"].SetValue(m_heightMap);
            m_clipLevelManager.m_upsamplerEffect.Parameters["oneOverStepSizeLocal"].SetValue(oneOverStepSizeLocal);
            m_clipLevelManager.m_upsamplerEffect.Parameters["oneOverStepSizeTotal"].SetValue(oneOverStepSizeTotal);
            m_clipLevelManager.m_upsamplerEffect.Parameters["view"].SetValue(orthographic);

            RenderTarget2D target1 = new RenderTarget2D(m_graphicsDevice, m_clipLevelManager.m_clipTexSize, m_clipLevelManager.m_clipTexSize);
            m_graphicsDevice.SetRenderTarget(target1);
            m_graphicsDevice.SetVertexBuffer(m_clipLevelManager.m_blockVertexBuffer);

            
            // Ok...
            /*
             * We can define one set of texture coordinates and attach them to each block.
             * These texture coordinates will be based on 1/max span assume unit stepping
             * Then these coordinates can be scaled inside the shader by the scale factor
             * of the current level. 
             * In addition an offset or starting point is needed to get the overall texture
             * coordinate.
            
             * Question - how does that work assuming we want to keep the highest resolution
             * grid at our current location? (with movement)
             
             */







        }

        public static float GridShifting(int Level, float LastLevel)
        {
            // (x,z) vertices translation at beginning
            if (Level > 2) return (2 * Math.Abs(LastLevel) + ((Level % 2) == 0 ? -1.0f : 1.0f)) * ((Level % 2) == 0 ? -1.0f : 1.0f);
            else
            {
                if (Level == 1)
                {
                    return 1.0f;
                }
                return -1.0f;
            }
        }




        public void BuildBoundingBoxes()
        {
            if (m_scaleLevel == 0)
            {
                m_subBoxes = new BoundingBox[4];
                m_fixupBoundingBoxes = new BoundingBox[0];
                Vector3 scale = new Vector3(ScaleFactor, 1, ScaleFactor);
                Vector3 boxSize = new Vector3(m_clipLevelManager.m_blockSize, 2 * m_clipLevelManager.m_maxHeight, m_clipLevelManager.m_blockSize) * scale; // twice to avoid -ve offset

                Vector3 boxOrigin = new Vector3(m_clipLevelManager.m_blockSize, -m_clipLevelManager.m_maxHeight, m_clipLevelManager.m_blockSize) * scale;
                m_subBoxes[0] = new BoundingBox(m_offset + boxOrigin, m_offset + boxOrigin + boxSize);
                boxOrigin = new Vector3((m_clipLevelManager.m_blockSize * 2) - 1, -m_clipLevelManager.m_maxHeight, m_clipLevelManager.m_blockSize) * scale;
                m_subBoxes[1] = new BoundingBox(m_offset + boxOrigin, m_offset + boxOrigin + boxSize);
                boxOrigin = new Vector3(m_clipLevelManager.m_blockSize, -m_clipLevelManager.m_maxHeight, (m_clipLevelManager.m_blockSize * 2) - 1) * scale;
                m_subBoxes[2] = new BoundingBox(m_offset + boxOrigin, m_offset + boxOrigin + boxSize);
                boxOrigin = new Vector3((m_clipLevelManager.m_blockSize * 2) - 1, -m_clipLevelManager.m_maxHeight, (m_clipLevelManager.m_blockSize * 2) - 1) * scale;
                m_subBoxes[3] = new BoundingBox(m_offset + boxOrigin, m_offset + boxOrigin + boxSize);
            }
            else
            {
                m_subBoxes = new BoundingBox[12];
                Vector3 scale = new Vector3(ScaleFactor, 1, ScaleFactor);

                Vector3 boxSize = new Vector3(m_clipLevelManager.m_blockSize, 2 * m_clipLevelManager.m_maxHeight, m_clipLevelManager.m_blockSize); // twice to avoid -ve offset
                boxSize *= scale;
                boxSize.Y = 2 * m_clipLevelManager.m_maxHeight; // twice to avoid -ve offset

                Vector3 boxOrigin = new Vector3(0, -m_clipLevelManager.m_maxHeight, 0);
                m_subBoxes[0] = new BoundingBox(m_offset + boxOrigin, m_offset + boxOrigin + boxSize);

                boxOrigin = new Vector3(m_clipLevelManager.m_blockSize - 1, -m_clipLevelManager.m_maxHeight, 0) * scale;
                m_subBoxes[1] = new BoundingBox(m_offset + boxOrigin, m_offset + boxOrigin + boxSize);

                boxOrigin = new Vector3(m_clipLevelManager.m_blockSize * 2, -m_clipLevelManager.m_maxHeight, 0) * scale;
                m_subBoxes[2] = new BoundingBox(m_offset + boxOrigin, m_offset + boxOrigin + boxSize);

                boxOrigin = new Vector3((m_clipLevelManager.m_blockSize * 3) - 1, -m_clipLevelManager.m_maxHeight, 0) * scale;
                m_subBoxes[3] = new BoundingBox(m_offset + boxOrigin, m_offset + boxOrigin + boxSize);

                boxOrigin = new Vector3((m_clipLevelManager.m_blockSize * 3) - 1, -m_clipLevelManager.m_maxHeight, m_clipLevelManager.m_blockSize - 1) * scale;
                m_subBoxes[4] = new BoundingBox(m_offset + boxOrigin, m_offset + boxOrigin + boxSize);

                boxOrigin = new Vector3((m_clipLevelManager.m_blockSize * 3) - 1, -m_clipLevelManager.m_maxHeight, m_clipLevelManager.m_blockSize * 2) * scale;
                m_subBoxes[5] = new BoundingBox(m_offset + boxOrigin, m_offset + boxOrigin + boxSize);

                boxOrigin = new Vector3((m_clipLevelManager.m_blockSize * 3) - 1, -m_clipLevelManager.m_maxHeight, (m_clipLevelManager.m_blockSize * 3) - 1) * scale;
                m_subBoxes[6] = new BoundingBox(m_offset + boxOrigin, m_offset + boxOrigin + boxSize);

                boxOrigin = new Vector3((m_clipLevelManager.m_blockSize * 2), -m_clipLevelManager.m_maxHeight, (m_clipLevelManager.m_blockSize * 3) - 1) * scale;
                m_subBoxes[7] = new BoundingBox(m_offset + boxOrigin, m_offset + boxOrigin + boxSize);

                boxOrigin = new Vector3((m_clipLevelManager.m_blockSize) - 1, -m_clipLevelManager.m_maxHeight, (m_clipLevelManager.m_blockSize * 3) - 1) * scale;
                m_subBoxes[8] = new BoundingBox(m_offset + boxOrigin, m_offset + boxOrigin + boxSize);

                boxOrigin = new Vector3(0, -m_clipLevelManager.m_maxHeight, (m_clipLevelManager.m_blockSize * 3) - 1) * scale;
                m_subBoxes[9] = new BoundingBox(m_offset + boxOrigin, m_offset + boxOrigin + boxSize);

                boxOrigin = new Vector3(0, -m_clipLevelManager.m_maxHeight, (m_clipLevelManager.m_blockSize * 2)) * scale;
                m_subBoxes[10] = new BoundingBox(m_offset + boxOrigin, m_offset + boxOrigin + boxSize);

                boxOrigin = new Vector3(0, -m_clipLevelManager.m_maxHeight, (m_clipLevelManager.m_blockSize - 1)) * scale;
                m_subBoxes[11] = new BoundingBox(m_offset + boxOrigin, m_offset + boxOrigin + boxSize);


                m_fixupBoundingBoxes = new BoundingBox[4];
                Vector3 vertBox = new Vector3((m_clipLevelManager.m_fixupOffset - 1), 2 * m_clipLevelManager.m_maxHeight, (m_clipLevelManager.m_blockSize - 1)) * scale; ;
                Vector3 horizBox = new Vector3((m_clipLevelManager.m_blockSize - 1), 2 * m_clipLevelManager.m_maxHeight, (m_clipLevelManager.m_fixupOffset - 1)) * scale;


                boxOrigin = new Vector3((m_clipLevelManager.m_blockSize * 2) - 2, -m_clipLevelManager.m_maxHeight, 0) * scale;
                m_fixupBoundingBoxes[0] = new BoundingBox(m_offset + boxOrigin, m_offset + boxOrigin + vertBox);

                boxOrigin = new Vector3(((m_clipLevelManager.m_blockSize * 2) - 2), -m_clipLevelManager.m_maxHeight, ((m_clipLevelManager.m_blockSize * 3) - 1)) * scale;
                m_fixupBoundingBoxes[1] = new BoundingBox(m_offset + boxOrigin, m_offset + boxOrigin + vertBox);

                boxOrigin = new Vector3(((m_clipLevelManager.m_blockSize * 3) - 1), -m_clipLevelManager.m_maxHeight, ((m_clipLevelManager.m_blockSize * 2) - 2)) * scale;
                m_fixupBoundingBoxes[2] = new BoundingBox(m_offset + boxOrigin, m_offset + boxOrigin + horizBox);

                boxOrigin = new Vector3(0, -m_clipLevelManager.m_maxHeight, ((m_clipLevelManager.m_blockSize * 2) - 2)) * scale;
                m_fixupBoundingBoxes[3] = new BoundingBox(m_offset + boxOrigin, m_offset + boxOrigin + horizBox);
            }
        }


        public int ScaleFactor
        {
            get
            {
                return (int)Math.Pow(2, m_scaleLevel);
            }
        }


        public void Draw(GraphicsDevice graphicsDevice, BoundingFrustum boundingFrustrum)
        {
            // cornera from top left so  go from -2 . 1

            graphicsDevice.Indices = m_clipLevelManager.m_blockIndexBuffer;
            graphicsDevice.SetVertexBuffer(m_clipLevelManager.m_blockVertexBuffer);


            //if (m_scaleLevel == 0)
            //{
            //    DrawCore(boundingFrustrum);
            //}
            //else
            {
                DrawRing(boundingFrustrum);
            }

        }


        public void DrawRing(BoundingFrustum boundingFrustrum)
        {
            m_graphicsDevice.Indices = m_clipLevelManager.m_blockIndexBuffer;
            for (int i = 0; i < m_subBoxes.Length; ++i)
            {
                DrawAtOffset(ref m_subBoxes[i], boundingFrustrum, m_clipLevelManager.m_blockIndexBuffer.IndexCount);
            }

            if (m_fixupBoundingBoxes.Length > 0)
            {
                m_graphicsDevice.Indices = m_clipLevelManager.m_fixupIndexBufferV;
                DrawAtOffset(ref m_fixupBoundingBoxes[2], boundingFrustrum, m_clipLevelManager.m_fixupIndexBufferV.IndexCount);
                DrawAtOffset(ref m_fixupBoundingBoxes[3], boundingFrustrum, m_clipLevelManager.m_fixupIndexBufferV.IndexCount);
                m_graphicsDevice.Indices = m_clipLevelManager.m_fixupIndexBufferH;
                DrawAtOffset(ref m_fixupBoundingBoxes[0], boundingFrustrum, m_clipLevelManager.m_fixupIndexBufferH.IndexCount);
                DrawAtOffset(ref m_fixupBoundingBoxes[1], boundingFrustrum, m_clipLevelManager.m_fixupIndexBufferH.IndexCount);
            }
        }



        public void DrawAtOffset(ref BoundingBox boundingBox, BoundingFrustum boundingFrustrum, int numIndices)
        {
            Matrix worldMatrix = Matrix.Identity;
            InternalDraw(ref boundingBox, boundingFrustrum, ref worldMatrix, m_clipLevelManager.m_blockVertexBuffer.VertexCount, numIndices);
        }


        public void InternalDraw(ref BoundingBox boundingBox, BoundingFrustum boundingFrustrum, ref Matrix world, int numVertices, int numIndices)
        {
            if (boundingFrustrum.Intersects(boundingBox))
            {

                BoundingBox totalBox = m_clipLevelManager.TotalBoundingBox;
                Vector3 totalSpan = totalBox.Max - totalBox.Min;
                Vector3 localSpan = boundingBox.Max - boundingBox.Min;

                Vector3 relativePos = localSpan / totalSpan;

                Vector4 scaleFactor = new Vector4(ScaleFactor, ScaleFactor, boundingBox.Min.X, boundingBox.Min.Z);
                Vector4 blockOrigin = new Vector4(1.0f / m_clipLevelManager.m_heightMapTexture.Width,
                                                        1.0f / m_clipLevelManager.m_heightMapTexture.Height,
                                                        relativePos.X * (float)m_clipLevelManager.m_heightMapTexture.Width,
                                                        relativePos.Z * (float)m_clipLevelManager.m_heightMapTexture.Height);

                m_clipLevelManager.m_effect.Parameters["WorldViewProjMatrix"].SetValue(world * m_clipLevelManager.m_camera.ViewMatrix * m_clipLevelManager.m_camera.ProjectionMatrix);
                m_clipLevelManager.m_effect.Parameters["ScaleFactor"].SetValue(scaleFactor);
                m_clipLevelManager.m_effect.Parameters["FineTextureBlockOrigin"].SetValue(blockOrigin);

                foreach (EffectPass pass in m_clipLevelManager.m_effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    m_graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, numVertices, 0, numIndices / 3);
                }
            }
        }

        public BoundingBox BoundingBox
        {
            get
            {
                return m_totalBoundingBox;
            }
        }



        public RenderTarget2D m_heightMap;
        public RenderTarget2D m_normalMap;

        public BoundingBox[] m_subBoxes;
        public BoundingBox[] m_fixupBoundingBoxes;
        public BoundingBox m_totalBoundingBox;
        public ClipLevelManager m_clipLevelManager;
        public GraphicsDevice m_graphicsDevice;
        public int m_scaleLevel;
        public Vector3 m_offset;
        public Vector4 m_vectorScale;
        public Vector2 m_textureScale;
        public Vector2 m_lastTextureScale;
        public Vector2 m_aabbDiff;
        public IntVector2 m_texTorusOrigin;
        public IntVector2 m_lastTexTorusOrigin;
        public float m_vectorShift;
        int m_hpos;
        int m_vpos;
        int m_finestLevelNumInstances;

    }


}
