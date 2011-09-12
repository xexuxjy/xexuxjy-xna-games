using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Dhpoware;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace ClipTerrainDemo
{
    public class ClipLevelManager : DrawableGameComponent
    {
        public ClipLevelManager(Game game, CameraComponent camera, Vector3 position)
            : base(game)
        {
            m_camera = camera;
            m_position = position;
            m_clipMapSize = 255;
            m_np1 = 255;
            m_nm1 = -1;

            m_clipTexSize = m_clipMapSize + 1;
            m_texNormalSize = m_clipTexSize;

            m_blockSize = (m_clipMapSize + 1) / 4;
            m_fixupOffset = 3;

            m_numLevels = 1;
            m_maxHeight = 5;

            m_mClipmapRTT = Matrix.CreateOrthographicOffCenter(0, m_clipTexSize, m_clipTexSize, 0, 0, 1);

        }

        //----------------------------------------------------------------------------------------------------

        public override void Initialize()
        {
            base.Initialize();
            m_effect = Game.Content.Load<Effect>("ClipTerrain");
            m_upsamplerEffect = Game.Content.Load<Effect>("Upsampler");
            m_normalEffect = Game.Content.Load<Effect>("ComputeNormals");

            Texture2D wrongFormatTexture = Game.Content.Load<Texture2D>("heightmap");
            m_heightMapTexture = new Texture2D(Game.GraphicsDevice, wrongFormatTexture.Width, wrongFormatTexture.Height, false, SurfaceFormat.Single);
            Color[] colorData = new Color[wrongFormatTexture.Width * wrongFormatTexture.Height];
            wrongFormatTexture.GetData<Color>(colorData);

            Single[] adjustedData = new Single[colorData.Length];
            m_heightMapTexture.GetData<Single>(adjustedData);
            float max = 0;

            float span = m_maxHeight * 2;


            // copy and scale the data;
            for (int i = 0; i < colorData.Length; ++i)
            {
                float pos = (float)colorData[i].R / 255.0f;
                adjustedData[i] = MathHelper.Lerp(0, span, pos);
            }

            m_heightMapTexture.SetData<Single>(adjustedData);

            //m_heightMapTexture.GetData<Single>

            m_effect.Parameters["fineLevelTexture"].SetValue(m_heightMapTexture);
            m_normalTexture = new Texture2D(Game.GraphicsDevice, m_heightMapTexture.Width * 2, m_heightMapTexture.Height * 2);
            m_effect.Parameters["normalsTexture"].SetValue(m_normalTexture);
            m_effect.Parameters["ZScaleFactor"].SetValue(1.0f);

            BuildVertexBuffers(Game.GraphicsDevice);
            m_clipLevels = new ClipLevel[m_numLevels];


            for (int i = 0; i < m_numLevels; ++i)
            {
                m_clipLevels[i] = new ClipLevel(i, this);
            }

            m_rasterizerState = new RasterizerState();
            m_rasterizerState.FillMode = FillMode.WireFrame;


        }

        //----------------------------------------------------------------------------------------------------

        public override void Draw(GameTime gameTime)
        {
            Game.GraphicsDevice.RasterizerState = m_rasterizerState;

            BoundingFrustum boundingFrustrum = new BoundingFrustum(m_camera.ViewProjectionMatrix);
            for (int i = 0; i < m_clipLevels.Length; ++i)
            {
                m_effect.Parameters["blockColor"].SetValue(ColorForRing(i));
                m_clipLevels[i].Draw(Game.GraphicsDevice, boundingFrustrum);
            }
        }

        //----------------------------------------------------------------------------------------------------

        public void BuildVertexBuffers(GraphicsDevice graphicsDevice)
        {
            PosOnlyVertex[] blockVertices = new PosOnlyVertex[m_blockSize * m_blockSize];
            int[] blockIndices = new int[(m_blockSize - 1) * (m_blockSize - 1) * 6];
            int indexCounter = 0;
            int vertexCounter = 0;
            int stride = m_blockSize;

            for (int y = 0; y < m_blockSize; ++y)
            {
                for (int x = 0; x < m_blockSize; ++x)
                {
                    Vector2 v = new Vector2(x, y);
                    PosOnlyVertex vpnt = new PosOnlyVertex(v);

                    blockVertices[vertexCounter++] = vpnt;
                    if (x < m_blockSize - 1 && y < m_blockSize - 1)
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
            int fixupSize = m_blockSize * m_fixupOffset;
            PosOnlyVertex[] fixupVertices = new PosOnlyVertex[fixupSize];
            int[] fixupIndices = new int[(m_blockSize - 1) * (m_fixupOffset - 1) * 6];
            vertexCounter = 0;
            indexCounter = 0;

            stride = m_blockSize;

            for (int y = 0; y < m_blockSize; ++y)
            {
                for (int x = 0; x < m_fixupOffset; ++x)
                {
                    if (x < m_fixupOffset - 1 && y < m_blockSize - 1)
                    {
                        fixupIndices[indexCounter++] = (x + (y * stride));
                        fixupIndices[indexCounter++] = (x + 1 + (y * stride));
                        fixupIndices[indexCounter++] = (x + 1 + ((y + 1) * stride));

                        fixupIndices[indexCounter++] = (x + 1 + ((y + 1) * stride));
                        fixupIndices[indexCounter++] = (x + ((y + 1) * stride));
                        fixupIndices[indexCounter++] = (x + (y * stride));

                    }
                }
            }
            m_fixupIndexBufferH = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, fixupIndices.Length, BufferUsage.None);
            m_fixupIndexBufferH.SetData<int>(fixupIndices);
            indexCounter = 0;

            stride = m_blockSize;

            for (int y = 0; y < m_fixupOffset; ++y)
            {
                for (int x = 0; x < m_blockSize; ++x)
                {
                    if (x < m_blockSize - 1 && y < m_fixupOffset - 1)
                    {
                        fixupIndices[indexCounter++] = (x + (y * stride));
                        fixupIndices[indexCounter++] = (x + 1 + (y * stride));
                        fixupIndices[indexCounter++] = (x + 1 + ((y + 1) * stride));

                        fixupIndices[indexCounter++] = (x + 1 + ((y + 1) * stride));
                        fixupIndices[indexCounter++] = (x + ((y + 1) * stride));
                        fixupIndices[indexCounter++] = (x + (y * stride));

                    }
                }
            }
            m_fixupIndexBufferV = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, fixupIndices.Length, BufferUsage.None);
            m_fixupIndexBufferV.SetData<int>(fixupIndices);



            //int numDegenerateIndices = (((m_clipMapSize - 1) / 2) * 3) * 4;
            //indexCounter = 0;

            //PosOnlyVertex[] degenerates = new PosOnlyVertex[numDegenerateIndices/6];

            //for(int i=0;i<m_blockSize;++i)
            //{
            //    degenerates[indexCounter] = new PosOnlyVertex(new Vector2(i,0));
            //    degenerates[indexCounter+m_blockSize] = new PosOnlyVertex(new Vector2(0,i));
            //    degenerates[indexCounter+(2*m_blockSize)] = new PosOnlyVertex(new Vector2(i,m_blockSize));
            //    degenerates[indexCounter+(3*m_blockSize)] = new PosOnlyVertex(new Vector2(m_blockSize,i));
            //    indexCounter++;
            //}

            //int[] degenerateIndices = new int[numDegenerateIndices];
            //vertexCounter = 0;
            //indexCounter = 0;

            //for (int i=0;i<m_blockSize-2;i+=2)
            //{
            //    degenerateIndices[indexCounter++] = i;
            //    degenerateIndices[indexCounter++] = i + 2;
            //    degenerateIndices[indexCounter++] = i + 1;
            //}

            //for (int i = 0; i < m_clipMapSize - 2; i+=2 )
            //{
            //    degenerateIndices[indexCounter++] = i * m_clipMapSize + (m_clipMapSize-1);
            //    degenerateIndices[indexCounter++] = (i + 2) * m_clipMapSize + (m_clipMapSize-1);
            //    degenerateIndices[indexCounter++] = (i + 1) * m_clipMapSize + (m_clipMapSize-1);
            //}

            //for (int i =  m_clipMapSize-1; i > 1; i-=2 )
            //{
            //    degenerateIndices[indexCounter++] = (m_clipMapSize-1) * m_clipMapSize + i;
            //    degenerateIndices[indexCounter++] = (m_clipMapSize-1) * m_clipMapSize + (i - 2);
            //    degenerateIndices[indexCounter++] = (m_clipMapSize-1) * m_clipMapSize + (i - 1);
            //}

            //for (int i = m_clipMapSize-1; i > 1; i-=2 )
            //{
            //    degenerateIndices[indexCounter++] = i * m_clipMapSize;
            //    degenerateIndices[indexCounter++] = (i - 2) * m_clipMapSize;
            //    degenerateIndices[indexCounter++] = (i - 1) * m_clipMapSize;
            //}



            m_blockVertexBuffer = new VertexBuffer(graphicsDevice, PosOnlyVertex.VertexDeclaration, blockVertices.Length, BufferUsage.None);
            //m_degenerateVertexBuffer = new VertexBuffer(graphicsDevice, PosOnlyVertex.VertexDeclaration, degenerates.Length, BufferUsage.None);

            m_blockVertexBuffer.SetData<PosOnlyVertex>(blockVertices, 0, blockVertices.Length);
            //m_degenerateVertexBuffer.SetData<PosOnlyVertex>(degenerates, 0, degenerates.Length);

            m_blockIndexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, blockIndices.Length, BufferUsage.None);
            //m_fixupIndexBufferH = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, fixupIndices.Length, BufferUsage.None);
            //m_degenerateIndexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, degenerateIndices.Length, BufferUsage.None);


            m_blockIndexBuffer.SetData<int>(blockIndices);
            //m_degenerateIndexBuffer.SetData(degenerateIndices);
        }

        //----------------------------------------------------------------------------------------------------

        public void CreateLShapes()
        {

            m_lShapesIndexBuffer = new IndexBuffer[4];
            // TODO : Remove the 2 or 4 vertices in double in index buffers 
            // Create Mesh
            int numPrimitives = (((m_clipMapSize - 1) - (m_blockSize - 1)) - (m_blockSize)) * 4;
            int[] indices = new int[numPrimitives];
            int numVertices = m_clipMapSize * m_clipMapSize;

            int N = m_clipMapSize;
            int M = m_blockSize;

            Vector3[] vertices = new Vector3[N * N];

            // Fill 0 .. N
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    vertices[i * N + j] = new Vector3(i, 0, j);
                }
            }

            int indexCounter = 0;

            for (int i = M - 1; i < M; i++)
            {
                for (int j = M - 1; j < ((N - 1) - (M - 1)); j++)
                {
                    indices[indexCounter++] = i * N + j;
                    indices[indexCounter++] = i * N + (j + 1);
                    indices[indexCounter++] = (i + 1) * N + (j + 1);

                    indices[indexCounter++] = (i + 1) * N + j;
                    indices[indexCounter++] = i * N + j;
                    indices[indexCounter++] = (i + 1) * N + (j + 1);
                }
            }

            for (int i = (N - 1) - (M - 1) - 1; i >= (M - 1); i--)
            {
                for (int j = (N - 1) - (M - 1) - 1; j >= (N - 1) - M; j--)
                {
                    indices[indexCounter++] = i * N + j;
                    indices[indexCounter++] = i * N + (j + 1);
                    indices[indexCounter++] = (i + 1) * N + (j + 1);

                    indices[indexCounter++] = (i + 1) * N + j;
                    indices[indexCounter++] = i * N + j;
                    indices[indexCounter++] = (i + 1) * N + (j + 1);
                }
            }

            m_lShapesIndexBuffer[0] = new IndexBuffer(m_graphicsDevice, IndexElementSize.ThirtyTwoBits, indexCounter, BufferUsage.None);
            m_lShapesIndexBuffer[0].SetData<int>(0, indices, 0, indexCounter);

            indexCounter = 0;
            for (int i = (N - 1) - (M - 1) - 1; i >= (M - 1); i--)
            {
                for (int j = (N - 1) - (M - 1) - 1; j >= (N - 1) - M; j--)
                {
                    indices[indexCounter++] = i * N + j;
                    indices[indexCounter++] = i * N + (j + 1);
                    indices[indexCounter++] = (i + 1) * N + (j + 1);

                    indices[indexCounter++] = (i + 1) * N + j;
                    indices[indexCounter++] = i * N + j;
                    indices[indexCounter++] = (i + 1) * N + (j + 1);
                }
            }

            for (int i = (N - 1) - (M - 1) - 1; i >= (M - 1); i--)
            {
                for (int j = (N - 1) - (M - 1) - 1; j >= (N - 1) - M; j--)
                {
                    indices[indexCounter++] = j * N + i;
                    indices[indexCounter++] = j * N + (i + 1);
                    indices[indexCounter++] = (j + 1) * N + (i + 1);

                    indices[indexCounter++] = (j + 1) * N + i;
                    indices[indexCounter++] = j * N + i;
                    indices[indexCounter++] = (j + 1) * N + (i + 1);
                }
            }
            m_lShapesIndexBuffer[1] = new IndexBuffer(m_graphicsDevice, IndexElementSize.ThirtyTwoBits, indexCounter, BufferUsage.None);
            m_lShapesIndexBuffer[1].SetData<int>(0, indices, 0, indexCounter);

            indexCounter = 0;
            for (int i = M - 1; i < M; i++)
            {
                for (int j = M - 1; j < ((N - 1) - (M - 1)); j++)
                {
                    indices[indexCounter++] = j * N + i;
                    indices[indexCounter++] = j * N + (i + 1);
                    indices[indexCounter++] = (j + 1) * N + (i + 1);

                    indices[indexCounter++] = (j + 1) * N + i;
                    indices[indexCounter++] = j * N + i;
                    indices[indexCounter++] = (j + 1) * N + (i + 1);
                }
            }

            for (int i = (N - 1) - (M - 1) - 1; i >= (M - 1); i--)
            {
                for (int j = (N - 1) - (M - 1) - 1; j >= (N - 1) - M; j--)
                {
                    indices[indexCounter++] = j * N + i;
                    indices[indexCounter++] = j * N + (i + 1);
                    indices[indexCounter++] = (j + 1) * N + (i + 1);

                    indices[indexCounter++] = (j + 1) * N + i;
                    indices[indexCounter++] = j * N + i;
                    indices[indexCounter++] = (j + 1) * N + (i + 1);
                }
            }
            m_lShapesIndexBuffer[2] = new IndexBuffer(m_graphicsDevice, IndexElementSize.ThirtyTwoBits, indexCounter, BufferUsage.None);
            m_lShapesIndexBuffer[2].SetData<int>(0, indices, 0, indexCounter);

            indexCounter = 0;
            for (int i = M - 1; i < M; i++)
            {
                for (int j = M - 1; j < ((N - 1) - (M - 1)); j++)
                {
                    indices[indexCounter++] = j * N + i;
                    indices[indexCounter++] = j * N + (i + 1);
                    indices[indexCounter++] = (j + 1) * N + (i + 1);

                    indices[indexCounter++] = (j + 1) * N + i;
                    indices[indexCounter++] = j * N + i;
                    indices[indexCounter++] = (j + 1) * N + (i + 1);
                }
            }

            for (int i = M - 1; i < M; i++)
            {
                for (int j = M - 1; j < ((N - 1) - (M - 1)); j++)
                {
                    indices[indexCounter++] = i * N + j;
                    indices[indexCounter++] = i * N + (j + 1);
                    indices[indexCounter++] = (i + 1) * N + (j + 1);

                    indices[indexCounter++] = (i + 1) * N + j;
                    indices[indexCounter++] = i * N + j;
                    indices[indexCounter++] = (i + 1) * N + (j + 1);
                }
            }
            m_lShapesIndexBuffer[3] = new IndexBuffer(m_graphicsDevice, IndexElementSize.ThirtyTwoBits, indexCounter, BufferUsage.None);
            m_lShapesIndexBuffer[3].SetData<int>(0, indices, 0, indexCounter);
        }

        //----------------------------------------------------------------------------------------------------

        public void UpdateClipmap(int l, int Dx, int Dz)
	    {
		RenderTarget2D pSurface;
        //RenderTarget2D pBackBuffer;
		RTT Vertex;

        int quadRttMultiplier = 6;

        RTT[] Quads = new RTT[quadRttMultiplier * 12];
        IntVector2 NewTexTorusOrigin;
		int NumQuads = 4;		
		int DX = 0, DY = 0;
		float TexOffsetX = 0.0f, TexOffsetY = 0.0f;

        //RenderTargetBinding[] renderTargets = m_graphicsDevice.GetRenderTargets();
        //pBackBuffer = renderTargets[0];


		NewTexTorusOrigin.X = m_clipLevels[l].m_texTorusOrigin.X;
		NewTexTorusOrigin.Y = m_clipLevels[l].m_texTorusOrigin.Y;

		if ( Math.Abs(Dx) < m_clipTexSize && Math.Abs(Dz) < m_clipTexSize)
		{
			// Partial update
				int TX, TXX;
				int EndX1 = (int)Math.Abs(Dx);

				if ( Dx > 0 ) 
                {
					DX = 1;
					TX = m_np1;
					TXX = m_np1 - Dx;
					if ( NewTexTorusOrigin.X >= m_clipTexSize ) 
                    { 
						NewTexTorusOrigin.X = NewTexTorusOrigin.X - m_clipTexSize; 
						EndX1 = Dx - NewTexTorusOrigin.X; 
						TX = TXX + EndX1;
					}
				} 
                else 
                {
					DX = -1;
					TX = m_nm1;
					TXX = m_nm1 + EndX1;
					if ( NewTexTorusOrigin.X <= 0 ) 
                    {
						NewTexTorusOrigin.X = NewTexTorusOrigin.X + m_clipTexSize;
						EndX1 = EndX1 - (EndX1 - m_clipLevels[l].m_lastTexTorusOrigin.X);
						TX = TXX - EndX1; 
					}
				}

				int X = m_clipLevels[l].m_lastTexTorusOrigin.X,
					  XX = X + DX * EndX1;

				int TY = m_np1 - NewTexTorusOrigin.Y,
					  TYY = m_nm1 + m_clipTexSize - NewTexTorusOrigin.Y;

                Quads[quadRttMultiplier * NumQuads+0].SetXYUV(XX, 0, TX, TY);
                Quads[quadRttMultiplier * NumQuads + 1].SetXYUV(X, 0, TXX, TY);
                Quads[quadRttMultiplier * NumQuads + 2].SetXYUV(XX, NewTexTorusOrigin.Y, TX, m_np1);
                Quads[quadRttMultiplier * NumQuads + 3] = Quads[quadRttMultiplier * NumQuads + 2];
                Quads[quadRttMultiplier * NumQuads + 4].SetXYUV(X, NewTexTorusOrigin.Y, TXX, m_np1);
                Quads[quadRttMultiplier * NumQuads + 5] = Quads[quadRttMultiplier * NumQuads + 1];
				NumQuads++;

                Quads[quadRttMultiplier * NumQuads + 0].SetXYUV(XX, NewTexTorusOrigin.Y, TX, m_nm1);
                Quads[quadRttMultiplier * NumQuads + 1].SetXYUV(X, NewTexTorusOrigin.Y, TXX, m_nm1);
                Quads[quadRttMultiplier * NumQuads + 2].SetXYUV(XX, m_clipTexSize, TX, TYY);
                Quads[quadRttMultiplier * NumQuads + 3] = Quads[quadRttMultiplier * NumQuads + 2];
                Quads[quadRttMultiplier * NumQuads + 4].SetXYUV(X, m_clipTexSize, TXX, TYY);
                Quads[quadRttMultiplier * NumQuads + 5] = Quads[quadRttMultiplier * NumQuads + 1];
				NumQuads++;

				int NXX = m_clipLevels[l].m_lastTexTorusOrigin.X - DX,
					  NTX = TXX - DX;

				// Overwrite Last and First row of normal map
                Quads[quadRttMultiplier * 0 + 0].SetXYUV(NXX, 0, NTX, TY);
                Quads[quadRttMultiplier * 0 + 1].SetXYUV(X, 0, TXX, TY);
                Quads[quadRttMultiplier * 0 + 2].SetXYUV(NXX, NewTexTorusOrigin.Y, NTX, m_np1);
                Quads[quadRttMultiplier * 0 + 3] = Quads[quadRttMultiplier * 0 + 2];
                Quads[quadRttMultiplier * 0 + 4].SetXYUV(X, NewTexTorusOrigin.Y, TXX, m_np1);
                Quads[quadRttMultiplier * 0 + 5] = Quads[quadRttMultiplier * 0 + 1];

                Quads[quadRttMultiplier * 1 + 0].SetXYUV(NXX, NewTexTorusOrigin.Y, NTX, m_nm1);
                Quads[quadRttMultiplier * 1 + 1].SetXYUV(X, NewTexTorusOrigin.Y, TXX, m_nm1);
                Quads[quadRttMultiplier * 1 + 2].SetXYUV(NXX, m_clipTexSize, NTX, TYY);
                Quads[quadRttMultiplier * 1 + 3] = Quads[1* quadRttMultiplier+2];
                Quads[quadRttMultiplier * 1 + 4].SetXYUV(X, m_clipTexSize, TXX, TYY);
                Quads[quadRttMultiplier * 1 + 5] = Quads[1*quadRttMultiplier + 1];

				if ( EndX1 != Math.Abs(Dx) )
				{
					// Cross x texture boundary
					X = DX > 0 ? 0 : m_clipTexSize; 

					TXX += DX * EndX1;
                    EndX1 = (int)Math.Abs(Dx) - EndX1;

					XX = X + DX * EndX1;
					TX += DX * EndX1;

					Quads[quadRttMultiplier * NumQuads + 0].SetXYUV( XX, 0, TX, TY );
					Quads[quadRttMultiplier * NumQuads + 1].SetXYUV( X, 0, TXX, TY );
					Quads[quadRttMultiplier * NumQuads + 2].SetXYUV( XX, NewTexTorusOrigin.Y, TX , m_np1 );
					Quads[quadRttMultiplier * NumQuads + 3] = Quads[quadRttMultiplier * NumQuads + 2];
					Quads[quadRttMultiplier * NumQuads + 4].SetXYUV( X , NewTexTorusOrigin.Y, TXX, m_np1 );
					Quads[quadRttMultiplier * NumQuads + 5] = Quads[quadRttMultiplier * NumQuads + 1];
					NumQuads++;

					Quads[quadRttMultiplier * NumQuads + 0].SetXYUV( XX, NewTexTorusOrigin.Y, TX, m_nm1 );
					Quads[quadRttMultiplier * NumQuads + 1].SetXYUV( X , NewTexTorusOrigin.Y, TXX, m_nm1 );
					Quads[quadRttMultiplier * NumQuads + 2].SetXYUV( XX, m_clipTexSize, TX, TYY );
					Quads[quadRttMultiplier * NumQuads + 3] = Quads[quadRttMultiplier * NumQuads + 2];
					Quads[quadRttMultiplier * NumQuads + 4].SetXYUV( X, m_clipTexSize, TXX, TYY );
					Quads[quadRttMultiplier * NumQuads + 5] = Quads[quadRttMultiplier * NumQuads + 1];
					NumQuads++;
				}

                int EndY1 = (int)Math.Abs(Dz);

				if ( Dz > 0 ) 
                {
					DY = 1;
					TY = m_np1;
					TYY = m_np1 - Dz ;
					if ( NewTexTorusOrigin.Y >= m_clipTexSize ) 
                    { 
						NewTexTorusOrigin.Y = NewTexTorusOrigin.Y - m_clipTexSize; 
						EndY1 = Dz - NewTexTorusOrigin.Y;
						TY = TYY + EndY1;
					}
				} 
                else 
                {
					DY = -1;
					TY = m_nm1;
					TYY = m_nm1 + EndY1 ;
					if ( NewTexTorusOrigin.Y <= 0 ) 
                    {
						NewTexTorusOrigin.Y = NewTexTorusOrigin.Y + m_clipTexSize; 
						EndY1 = EndY1 - (EndY1 - m_clipLevels[l].m_lastTexTorusOrigin.Y);
						TY = TYY - EndY1; 
					}
				}

				int Y = m_clipLevels[l].m_lastTexTorusOrigin.Y,
					  YY = Y + DY * EndY1;

				TX = m_np1 - NewTexTorusOrigin.X;
				TXX = m_nm1 + m_clipTexSize - NewTexTorusOrigin.X;

				Quads[quadRttMultiplier * NumQuads + 0].SetXYUV( 0, YY, TX, TY );
				Quads[quadRttMultiplier * NumQuads + 1].SetXYUV( 0, Y, TX , TYY );
				Quads[quadRttMultiplier * NumQuads + 2].SetXYUV( NewTexTorusOrigin.X, YY, m_np1, TY );
				Quads[quadRttMultiplier * NumQuads + 3] = Quads[quadRttMultiplier * NumQuads + 2];
				Quads[quadRttMultiplier * NumQuads + 4].SetXYUV( NewTexTorusOrigin.X, Y, m_np1, TYY );
				Quads[quadRttMultiplier * NumQuads + 5] = Quads[quadRttMultiplier * NumQuads + 1];
				NumQuads++;

				Quads[quadRttMultiplier * NumQuads + 0].SetXYUV( NewTexTorusOrigin.X, YY, m_nm1, TY );
				Quads[quadRttMultiplier * NumQuads + 1].SetXYUV( NewTexTorusOrigin.X, Y, m_nm1, TYY );
				Quads[quadRttMultiplier * NumQuads + 2].SetXYUV( m_clipTexSize        , YY, TXX , TY );
				Quads[quadRttMultiplier * NumQuads + 3] = Quads[quadRttMultiplier * NumQuads + 2];
				Quads[quadRttMultiplier * NumQuads + 4].SetXYUV( m_clipTexSize        , Y, TXX , TYY );
				Quads[quadRttMultiplier * NumQuads + 5] = Quads[quadRttMultiplier * NumQuads + 1];
				NumQuads++;

				int NYY = m_clipLevels[l].m_lastTexTorusOrigin.Y - DY,
					  NTY = TYY - DY;

				// Overwrite Last and First col of normal map
				Quads[quadRttMultiplier * 2 + 0].SetXYUV( 0, NYY, TX, NTY );
				Quads[quadRttMultiplier * 2 + 1].SetXYUV( 0, Y, TX , TYY );
				Quads[quadRttMultiplier * 2 + 2].SetXYUV( NewTexTorusOrigin.X, NYY, m_np1, NTY );
				Quads[quadRttMultiplier * 2 + 3] = Quads[quadRttMultiplier * 2 + 2];
				Quads[quadRttMultiplier * 2 + 4].SetXYUV( NewTexTorusOrigin.X, Y, m_np1, TYY );
				Quads[quadRttMultiplier * 2 + 5] = Quads[quadRttMultiplier * 2 + 1];

				Quads[quadRttMultiplier * 3 + 0].SetXYUV( NewTexTorusOrigin.X, NYY, m_nm1, NTY );
				Quads[quadRttMultiplier * 3 + 1].SetXYUV( NewTexTorusOrigin.X, Y, m_nm1, TYY );
				Quads[quadRttMultiplier * 3 + 2].SetXYUV( m_clipTexSize        , NYY, TXX , NTY );
				Quads[quadRttMultiplier * 3 + 3] = Quads[quadRttMultiplier * 3 + 2];
				Quads[quadRttMultiplier * 3 + 4].SetXYUV( m_clipTexSize        , Y, TXX , TYY );
				Quads[quadRttMultiplier * 3 + 5] = Quads[quadRttMultiplier * 3 + 1];

				if ( EndY1 != Math.Abs(Dz) )
				{
					// Cross y texture boundary
					Y = DY > 0 ? 0 : m_clipTexSize; 

					TYY += DY * EndY1;
                    EndY1 = (int)Math.Abs(Dz) - EndY1;

					YY = Y + DY * EndY1;
					TY += DY * EndY1;

					Quads[quadRttMultiplier * NumQuads + 0].SetXYUV( 0, YY, TX , TY );
					Quads[quadRttMultiplier * NumQuads + 1].SetXYUV( 0, Y , TX , TYY );
					Quads[quadRttMultiplier * NumQuads + 2].SetXYUV( NewTexTorusOrigin.X, YY, m_np1, TY );
					Quads[quadRttMultiplier * NumQuads + 3] = Quads[quadRttMultiplier * NumQuads + 2];
					Quads[quadRttMultiplier * NumQuads + 4].SetXYUV( NewTexTorusOrigin.X, Y, m_np1, TYY );
					Quads[quadRttMultiplier * NumQuads + 5] = Quads[quadRttMultiplier * NumQuads + 1];
					NumQuads++;

					Quads[quadRttMultiplier * NumQuads + 0].SetXYUV( NewTexTorusOrigin.X, YY, m_nm1, TY );
					Quads[quadRttMultiplier * NumQuads + 1].SetXYUV( NewTexTorusOrigin.X, Y, m_nm1, TYY );
					Quads[quadRttMultiplier * NumQuads + 2].SetXYUV( m_clipTexSize        , YY, TXX , TY );
					Quads[quadRttMultiplier * NumQuads + 3] = Quads[quadRttMultiplier * NumQuads + 2];
					Quads[quadRttMultiplier * NumQuads + 4].SetXYUV( m_clipTexSize    , Y, TXX , TYY );
					Quads[quadRttMultiplier * NumQuads + 5] = Quads[quadRttMultiplier * NumQuads + 1];
					NumQuads++;
				}
		        m_clipLevels[l].m_lastTexTorusOrigin.X = NewTexTorusOrigin.X;
		        m_clipLevels[l].m_lastTexTorusOrigin.Y = NewTexTorusOrigin.Y;

		        m_clipLevels[l].m_texTorusOrigin.X = NewTexTorusOrigin.X;
		        m_clipLevels[l].m_texTorusOrigin.Y = NewTexTorusOrigin.Y;
		    } 
		    else 
		    {
                //m_clipLevels[l].ShortC = false;
			    // update the entire level if this level is rendered
			    if ( Dx > 0 ) 
                {
				    while ( NewTexTorusOrigin.X >= m_clipTexSize ) 
                    { 
                        NewTexTorusOrigin.X = NewTexTorusOrigin.X - m_clipTexSize;	
                    }
			    } 
                else 
                {
				    while ( NewTexTorusOrigin.X <= 0 ) 
                    { 
                        NewTexTorusOrigin.X = NewTexTorusOrigin.X + m_clipTexSize; 
                    }
			    }

			    if ( Dz > 0 ) 
                {
				    while ( NewTexTorusOrigin.Y >= m_clipTexSize ) 
                    { 
                        NewTexTorusOrigin.Y = NewTexTorusOrigin.Y - m_clipTexSize; 
                    }
			    } 
                else 
                {
				    while ( NewTexTorusOrigin.Y <= 0 ) 
                    { 
                        NewTexTorusOrigin.Y = NewTexTorusOrigin.Y + m_clipTexSize; 
                    }
			    }
			
			    int Ex,Ey,Bx,By;
			    int Tx,Ty;

			    Ex = NewTexTorusOrigin.X;
			    Ey = NewTexTorusOrigin.Y;

			    Tx = m_np1 - NewTexTorusOrigin.X ;
			    Ty = m_np1 - NewTexTorusOrigin.Y ;
			
			    Bx = 0;
			    By = 0;

			    Quads[quadRttMultiplier * NumQuads + 0].SetXYUV( Ex, By, m_np1, Ty );
			    Quads[quadRttMultiplier * NumQuads + 1].SetXYUV( Bx, By, Tx , Ty );
			    Quads[quadRttMultiplier * NumQuads + 2].SetXYUV( Ex, Ey, m_np1, m_np1);
			    Quads[quadRttMultiplier * NumQuads + 3] = Quads[quadRttMultiplier * NumQuads + 2];
			    Quads[quadRttMultiplier * NumQuads + 4].SetXYUV( Bx, Ey, Tx , m_np1);
			    Quads[quadRttMultiplier * NumQuads + 5] = Quads[quadRttMultiplier * NumQuads + 1];
			    NumQuads++;

			    Bx = m_clipTexSize;
			    By = 0;

			    Quads[quadRttMultiplier * NumQuads + 0].SetXYUV( Ex, By, m_nm1, Ty );
			    Quads[quadRttMultiplier * NumQuads + 1].SetXYUV( Bx, By, Tx , Ty );
			    Quads[quadRttMultiplier * NumQuads + 2].SetXYUV( Ex, Ey, m_nm1 , m_np1 );
			    Quads[quadRttMultiplier * NumQuads + 3] = Quads[quadRttMultiplier * NumQuads + 2];
			    Quads[quadRttMultiplier * NumQuads + 4].SetXYUV( Bx, Ey, Tx , m_np1);
			    Quads[quadRttMultiplier * NumQuads + 5] = Quads[quadRttMultiplier * NumQuads + 1];
			    NumQuads++;

			    Bx = 0;
			    By = m_clipTexSize;

			    Quads[quadRttMultiplier * NumQuads + 0].SetXYUV( Ex, By, m_np1 , Ty );
			    Quads[quadRttMultiplier * NumQuads + 1].SetXYUV( Bx, By, Tx  , Ty );
			    Quads[quadRttMultiplier * NumQuads + 2].SetXYUV( Ex, Ey, m_np1 , m_nm1 );
			    Quads[quadRttMultiplier * NumQuads + 3] = Quads[quadRttMultiplier * NumQuads + 2];
			    Quads[quadRttMultiplier * NumQuads + 4].SetXYUV( Bx, Ey, Tx  , m_nm1 );
			    Quads[quadRttMultiplier * NumQuads + 5] = Quads[quadRttMultiplier * NumQuads + 1];
			    NumQuads++;

			    Bx = m_clipTexSize;
			    By = m_clipTexSize;

			    Quads[quadRttMultiplier * NumQuads + 0].SetXYUV( Ex, By, m_nm1 , Ty );
			    Quads[quadRttMultiplier * NumQuads + 1].SetXYUV( Bx, By, Tx  , Ty );
			    Quads[quadRttMultiplier * NumQuads + 2].SetXYUV( Ex, Ey, m_nm1 , m_nm1 );
			    Quads[quadRttMultiplier * NumQuads + 3] = Quads[quadRttMultiplier * NumQuads + 2];
			    Quads[quadRttMultiplier * NumQuads + 4].SetXYUV( Bx, Ey, Tx  , m_nm1 );
			    Quads[quadRttMultiplier * NumQuads + 5] = Quads[quadRttMultiplier * NumQuads + 1];
			    NumQuads++;

			    // Don't need to overwrite old first & last  row / col
			    Quads[quadRttMultiplier * 0 + 0].SetXYUV( -1, -1, -1, -1 );
			    Quads[quadRttMultiplier * 0 + 1] = Quads[quadRttMultiplier * 0 + 2] = Quads[quadRttMultiplier * 0 + 3] = Quads[quadRttMultiplier * 0 + 4] = Quads[quadRttMultiplier * 0 + 5] = Quads[quadRttMultiplier * 0 + 0];

                Quads[1 * quadRttMultiplier + 0] = Quads[1 * quadRttMultiplier + 1] = Quads[1 * quadRttMultiplier + 2] = Quads[1 * quadRttMultiplier + 3] = Quads[1 * quadRttMultiplier + 4] = Quads[1 * quadRttMultiplier + 5] = Quads[quadRttMultiplier * 0 + 0];
			    Quads[quadRttMultiplier * 2 + 0] = Quads[quadRttMultiplier * 2 + 1] = Quads[quadRttMultiplier * 2 + 2] = Quads[quadRttMultiplier * 2 + 3] = Quads[quadRttMultiplier * 2 + 4] = Quads[quadRttMultiplier * 2 + 5] = Quads[quadRttMultiplier * 0 + 0];
			    Quads[quadRttMultiplier * 3 + 0] = Quads[quadRttMultiplier * 3 + 1] = Quads[quadRttMultiplier * 3 + 2] = Quads[quadRttMultiplier * 3 + 3] = Quads[quadRttMultiplier * 3 + 4] = Quads[quadRttMultiplier * 3 + 5] = Quads[quadRttMultiplier * 0 + 0];

			    m_clipLevels[l].m_lastTexTorusOrigin.X = NewTexTorusOrigin.X;
			    m_clipLevels[l].m_lastTexTorusOrigin.Y = NewTexTorusOrigin.Y;

			    m_clipLevels[l].m_texTorusOrigin.X = NewTexTorusOrigin.X;
			    m_clipLevels[l].m_texTorusOrigin.Y = NewTexTorusOrigin.Y;
		    }

		    // Update clipmap

            //m_clipLevels[l].m_heightMap.GetSurfaceLevel(0,&pSurface);
            //DXUTGetD3DDevice().SetRenderTarget(0, pSurface);

            m_graphicsDevice.SetRenderTarget(m_clipLevels[l].m_heightMap);

			if (l+1 > m_numLevels-1) 
			{
				// Special case of coarsest Level
				m_upsamplerEffect.CurrentTechnique.Passes[1].Apply();
			}
			else
			{
				// Coarser tex offset for coarser value
				TexOffsetX = m_texCoarserOffset[(l%2)] + (m_clipLevels[l].m_textureScale.X * 0.5f);
				TexOffsetY = m_texCoarserOffset[(l%2)] + (m_clipLevels[l].m_textureScale.Y * 0.5f);

				m_upsamplerEffect.CurrentTechnique.Passes[0].Apply();
                m_upsamplerEffect.Parameters["tCoarserElevation"].SetValue(m_clipLevels[Math.Min(l + 1, m_numLevels - 1)].m_heightMap);
			}

            Vector4 Cto = new Vector4(TexOffsetX,TexOffsetY,0.0f,0.0f);

			m_upsamplerEffect.Parameters["CoarserTexOffset"].SetValue(Cto);
			m_upsamplerEffect.Parameters["OneOverTextureSize"].SetValue(m_texelClip);
			m_upsamplerEffect.Parameters["m_textureScale"].SetValue(m_clipLevels[l].m_vectorScale);
            m_upsamplerEffect.Parameters["FloatPrecision"].SetValue(m_floatPrecision);
			m_upsamplerEffect.Parameters["Proj"].SetValue(m_mClipmapRTT);
			m_upsamplerEffect.Parameters["tpermTexture"].SetValue(m_permTexture);
            m_upsamplerEffect.Parameters["tgradTexture"].SetValue(m_gradTexture);
            //m_upsamplerEffect.CommitChanges();

            //DXUTGetD3DDevice().BeginScene();
            //DXUTGetD3DDevice().SetVertexDeclaration( m_pUpdateDeclaration );
            //DXUTGetD3DDevice().DrawPrimitiveUP(D3DPT_TRIANGLELIST, (NumQuads - 4) * 2, &(Quads[4].Quad[0]), sizeof(RTT) );
            //DXUTGetD3DDevice().EndScene();
                
            m_graphicsDevice.DrawUserPrimitives<RTT>(PrimitiveType.TriangleList,Quads,0,(NumQuads - 4) * 2);
            //m_upsamplerEffect.EndPass();
            //m_upsamplerEffect.End();

		    // Update normal
            //SAFE_RELEASE( pSurface );
            //m_clipLevels[l].NormalMap.GetSurfaceLevel(0,&pSurface);
            //DXUTGetD3DDevice().SetRenderTarget(0, pSurface);
		
            m_graphicsDevice.SetRenderTarget(m_clipLevels[l].m_normalMap);

			m_normalEffect.Parameters["tElevation"].SetValue(m_clipLevels[l].m_heightMap);
			m_normalEffect.Parameters["tCoarserNormal"].SetValue(m_clipLevels[Math.Min(l+1,m_numLevels-1)].m_normalMap);


			if ( l+1 > m_numLevels-1 )
            {
				m_normalEffect.CurrentTechnique.Passes[1].Apply();
            }
			else
            {
				m_normalEffect.CurrentTechnique.Passes[0].Apply();
            }

            Cto = new Vector4(TexOffsetX,TexOffsetY,0,0);

            Vector4 ScaleFac = new Vector4(-0.5f * m_zScaleOverFp / m_clipLevels[l].m_vectorScale.X, -0.5f * m_zScaleOverFp / m_clipLevels[l].m_vectorScale.Y, 0.0f, 0.0f); 
				
			m_normalEffect.Parameters["CoarserTexOffset"].SetValue(Cto);
			m_normalEffect.Parameters["ScaleFac"].SetValue(ScaleFac);
			m_normalEffect.Parameters["OneOverTextureSize"].SetValue(m_texelClip);
			m_normalEffect.Parameters["m_zScaleOverFp"].SetValue(m_zScaleOverFp);
            m_normalEffect.Parameters["Proj"].SetValue(m_mClipmapRTT);
            //m_pComputeNormals.CommitChanges();

			// Reuse the same quads as for elevationmaps + 4 quads to overwrite first & last row/col 
            //DXUTGetD3DDevice().BeginScene();
            //DXUTGetD3DDevice().SetVertexDeclaration( m_pUpdateDeclaration );
            //DXUTGetD3DDevice().DrawPrimitiveUP(D3DPT_TRIANGLELIST, NumQuads * 2, &(Quads[quadRttMultiplier * 0 + 0]), sizeof(RTT) );
            //DXUTGetD3DDevice().EndScene();

            m_graphicsDevice.DrawUserPrimitives<RTT>(PrimitiveType.TriangleList, Quads, 0,NumQuads * 2);

            m_graphicsDevice.SetRenderTarget(null);


	    }

        //----------------------------------------------------------------------------------------------------

        public Vector4 ColorForRing(int ring)
        {
            switch (ring)
            {
                case (0):
                    return Color.White.ToVector4();
                case (1):
                    return Color.Yellow.ToVector4();
                case (2):
                    return Color.Red.ToVector4();
                case (3):
                    return Color.Green.ToVector4();
                case (4):
                    return Color.Blue.ToVector4();
                case (5):
                    return Color.Magenta.ToVector4();
                case (6):
                    return Color.Olive.ToVector4();

                default:
                    return Color.Black.ToVector4();
            }
        }

        //----------------------------------------------------------------------------------------------------

        public ClipLevel GetClipLevel(int i)
        {
            return m_clipLevels[i];
        }




        public BoundingBox TotalBoundingBox
        {
            get
            {
                return m_clipLevels[m_clipLevels.Length - 1].BoundingBox;
            }
        }

        public Effect m_effect;
        public Effect m_upsamplerEffect;
        public Effect m_normalEffect;

        private ClipLevel[] m_clipLevels;

        private RasterizerState m_rasterizerState;
        private GraphicsDevice m_graphicsDevice;
        public CameraComponent m_camera;
        public Texture2D m_heightMapTexture;
        private Texture2D m_normalTexture;

        private Texture2D m_permTexture;
        private Texture2D m_gradTexture;



        public VertexBuffer m_blockVertexBuffer;
        public VertexBuffer m_largeBlockVertexBuffer;

        public VertexBuffer m_degenerateVertexBuffer;

        public IndexBuffer m_blockIndexBuffer;
        public IndexBuffer m_fixupIndexBufferH;
        public IndexBuffer m_fixupIndexBufferV;

        public IndexBuffer m_degenerateIndexBuffer;

        public IndexBuffer[] m_lShapesIndexBuffer;


        public Matrix m_mClipmapRTT;
        public Vector3 m_position;

        public int m_clipMapSize;
        public int m_blockSize;
        public int m_clipTexSize;
        public int m_texNormalSize;
        public int m_np1;
        public int m_nm1;
        public float[] m_texCoarserOffset = new float[2];
        public float m_zScaleOverFp;
        public float m_texelClip;
        public float m_floatPrecision;


        public int m_fixupOffset;
        public int m_numLevels;
        public int m_maxHeight;

    }


    //public struct RTTQuad
    //{
    //    public RTT[] Quad;
    //    //public RTTQuad()
    //    //{
    //    //    Quad = new RTT[6];
    //    //}
    //}

    public struct RTT : IVertexType
    {
        public int x;
        public int y;
        public int u;
        public int v;

        public void SetXY(int a, int b)
        {
            x = a;
            y = b;
        }

        public void SetUV(int a, int b)
        {
            u = a;
            v = b;
        }

        public void SetXYUV(int a, int b, int c, int d)
        {
            x = a;
            y = b;
            u = c;
            v = d;
        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Short2, VertexElementUsage.Position, 0),
            new VertexElement(4, VertexElementFormat.Short2, VertexElementUsage.TextureCoordinate, 1)
        );

        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }
    };


    public struct IntVector2
    {
        public int X;
        public int Y;
    }


}
