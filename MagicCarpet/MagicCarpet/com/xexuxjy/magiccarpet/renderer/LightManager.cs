using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using BulletXNA.LinearMath;

namespace com.xexuxjy.magiccarpet.renderer
{
    public class LightManager
    {

        public static void ApplyLightToEffect(Effect effect)
        {
            IndexedVector3 ambientLight = new IndexedVector3(0.1f);
            IndexedVector3 directionalLight = new IndexedVector3(0.4f);

            effect.Parameters["AmbientLight"].SetValue(ambientLight);
            effect.Parameters["DirectionalLight"].SetValue(directionalLight);
            effect.Parameters["LightPosition"].SetValue(new IndexedVector3(0, 40, 0));
            IndexedVector3 lightDirection = new IndexedVector3(10, -10, 0);
            lightDirection.Normalize();

            effect.Parameters["LightDirection"].SetValue(lightDirection);

        }

    }
}
