// Sample taken from http://www.dustinhorne.com/page/XNA-Terrain-Tutorial-Table-of-Contents.aspx - many thanks

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Gladius.util;

namespace Gladius.Terrain7
{
    internal class BufferManager
    {
        int _active = 0;
        internal VertexBuffer VertexBuffer;
        internal VertexBuffer SkirtVertexBuffer;
        internal IndexBuffer SkirtIndexBuffer;


        IndexBuffer[] _IndexBuffers;
        GraphicsDevice _device;

        internal BufferManager(VertexPositionNormalTexture[] vertices, GraphicsDevice device)
        {
            _device = device;
            VertexBuffer = new VertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);

            VertexBuffer.SetData(vertices);

            _IndexBuffers = new IndexBuffer[]
            {
                new IndexBuffer(_device, IndexElementSize.ThirtyTwoBits, 100000, BufferUsage.WriteOnly),
                new IndexBuffer(_device, IndexElementSize.ThirtyTwoBits, 100000, BufferUsage.WriteOnly)
            };
        }

        public void SetSkirtData(ObjectArray<VertexPositionNormalTexture> vertices, ObjectArray<int> indices)
        {
            if (vertices.Count > 0)
            {
                if (SkirtVertexBuffer == null || SkirtVertexBuffer.VertexCount < vertices.Count)
                {
                    float grow = 1.2f;
                    int vbSize = (int)((float)vertices.Count * grow);
                    SkirtVertexBuffer = new VertexBuffer(_device, VertexPositionNormalTexture.VertexDeclaration, vbSize, BufferUsage.WriteOnly);
                }
                SkirtVertexBuffer.SetData(vertices.GetRawArray(), 0, vertices.Count);
            }
            if (indices.Count > 0)
            {
                if (SkirtIndexBuffer == null || SkirtIndexBuffer.IndexCount < indices.Count)
                {
                    float grow = 1.2f;
                    int ibSize = (int)((float)indices.Count * grow);
                    SkirtIndexBuffer = new IndexBuffer(_device, IndexElementSize.ThirtyTwoBits, ibSize, BufferUsage.WriteOnly);

                }

                SkirtIndexBuffer.SetData(indices.GetRawArray(), 0, indices.Count);
            }
        }


        internal IndexBuffer IndexBuffer
        {
            get { return _IndexBuffers[_active]; }
        }

        internal void UpdateIndexBuffer(int[] indices, int indexCount)
        {
            _IndexBuffers[_active].SetData(indices, 0, indexCount);
            //int inactive = _active == 0 ? 1 : 0;

            //if(_IndexBuffers[inactive] != _device.Indices)
            //    _IndexBuffers[inactive].SetData(indices, 0, indexCount);
        }

        internal void SwapBuffer()
        {
            //_active = _active == 0 ? 1 : 0;
        }
    }
}
