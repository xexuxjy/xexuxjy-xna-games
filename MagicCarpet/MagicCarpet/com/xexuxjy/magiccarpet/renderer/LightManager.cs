using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace com.xexuxjy.magiccarpet.renderer
{
    public class LightManager
    {

        public static void ApplyLightToEffect(Effect effect)
        {
            Vector3 ambientLight = new Vector3(0.1f);
            Vector3 directionalLight = new Vector3(0.4f);

            effect.Parameters["AmbientLight"].SetValue(ambientLight);
            effect.Parameters["DirectionalLight"].SetValue(directionalLight);
            effect.Parameters["LightPosition"].SetValue(new Vector3(0, 40, 0));
            Vector3 lightDirection = new Vector3(10, -10, 0);
            lightDirection.Normalize();

            effect.Parameters["LightDirection"].SetValue(lightDirection);

        }

    }
}
