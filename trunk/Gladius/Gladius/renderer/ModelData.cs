using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Gladius.renderer
{
    public class ModelData
    {
        public ModelData(Model model, Texture2D texture)
        {
            Model = model;
            Texture = texture;
            BoneTransforms = new Matrix[model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(BoneTransforms);
        }


        public Model Model;
        public Texture2D Texture;
        public Matrix[] BoneTransforms;
    }
}
