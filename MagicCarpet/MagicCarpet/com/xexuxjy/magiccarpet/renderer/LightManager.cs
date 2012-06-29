using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace com.xexuxjy.magiccarpet.renderer
{
    public class LightManager
    {

        public static void ApplyLightToEffect(Effect effect)
        {
            Vector3 ambientLightColor = new Vector3(1f);
            float ambientLightIntensity = 0.1f;
            Vector3 directionalLightColor = new Vector3(1f);
            float directionalLightIntensity = 0.4f;

            Vector3 specularLightColor = new Vector3(1f);
            float specularLightIntensity = 0.4f;

            effect.Parameters["AmbientLightColor"].SetValue(ambientLightColor);
            effect.Parameters["AmbientLightIntensity"].SetValue(ambientLightIntensity);

            effect.Parameters["DirectionalLightColor"].SetValue(directionalLightColor);
            effect.Parameters["DirectionalLightIntensity"].SetValue(directionalLightIntensity);

            effect.Parameters["SpecularLightColor"].SetValue(specularLightColor);
            effect.Parameters["SpecularLightIntensity"].SetValue(specularLightIntensity);

            if (Globals.Player != null)
            {
                effect.Parameters["LightPosition"].SetValue(Globals.Player.Position);
                effect.Parameters["LightDirection"].SetValue(Globals.Player.Forward);
            }
            
            //effect.Parameters["LightPosition"].SetValue(new Vector3(0, 40, 0));
            //Vector3 lightDirection = new Vector3(10, -10, 0);
            //lightDirection.Normalize();

            //effect.Parameters["LightDirection"].SetValue(lightDirection);

        }

    }
}
