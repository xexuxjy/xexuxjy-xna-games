using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using com.xexuxjy.magiccarpet;

namespace com.xexuxjy.magiccarpet.util
{
    public static class ObjectBuilderUtil
    {
        public static void BuildClothObject(int numSegments,float clothWidth,float clothLength,out Vector4 clothDimensions,out VertexBuffer vertexBuffer)
        {
            float clothHeightDeflection = 0.02f;
            float clothThickness = 0.0001f;
            float segmentStep = clothLength / numSegments;

            clothDimensions = new Vector4(clothWidth, clothHeightDeflection, clothLength,segmentStep);
                

            int counter = 0;
            float uvStep = 1f / (float)numSegments;

            Vector3 clothWidthOffset = new Vector3(-clothWidth / 2, 0, -clothLength/2);

            VertexPositionTexture[] vertices = new VertexPositionTexture[numSegments * 12];

            for (int i = 0; i < numSegments; ++i)
            {
                Vector3 bl = new Vector3(0, 0, i * segmentStep) + clothWidthOffset;
                Vector3 br = new Vector3(clothWidth, 0, i * segmentStep) + clothWidthOffset;
                Vector3 tl = new Vector3(0, 0, (i + 1) * segmentStep) + clothWidthOffset;
                Vector3 tr = new Vector3(clothWidth, 0, (i + 1) * segmentStep) + clothWidthOffset;

                Vector2 tbl = new Vector2(0, i * uvStep);
                Vector2 tbr = new Vector2(1, i * uvStep);
                Vector2 ttl = new Vector2(0, (i+1) * uvStep);
                Vector2 ttr = new Vector2(1, (i+1) * uvStep);

                // top face of cloth...
                vertices[counter++] = new VertexPositionTexture(bl,  tbl);
                vertices[counter++] = new VertexPositionTexture(br,  tbr);
                vertices[counter++] = new VertexPositionTexture(tl,  ttl);

                vertices[counter++] = new VertexPositionTexture(br,  tbr);
                vertices[counter++] = new VertexPositionTexture(tr,  ttr);
                vertices[counter++] = new VertexPositionTexture(tl,  ttl);


                // bottom face of cloth...

                Vector3 bottomOffset = new Vector3(0, -clothThickness, 0);


                vertices[counter++] = new VertexPositionTexture(bl+bottomOffset, tbl);
                vertices[counter++] = new VertexPositionTexture(tl + bottomOffset, ttl);
                vertices[counter++] = new VertexPositionTexture(br + bottomOffset, tbr);

                vertices[counter++] = new VertexPositionTexture(br + bottomOffset, tbr);
                vertices[counter++] = new VertexPositionTexture(tl + bottomOffset, ttl);
                vertices[counter++] = new VertexPositionTexture(tr + bottomOffset, ttr);


            }

            vertexBuffer = new VertexBuffer(Globals.GraphicsDevice, VertexPositionTexture.VertexDeclaration, counter, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertices,0,counter);
        }

    }
}
