using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Gladius.renderer
{
    class GraphicsHelper
    {
    }


    public class ModelData
    {
        public Model Model;
        public Matrix[] BoneTransforms;
        public Texture2D Texture;


        public ModelData(Model _model,Texture2D _texture)
        {
            Model = _model;
            Texture = _texture;
            BoneTransforms = new Matrix[_model.Bones.Count];
            _model.CopyAbsoluteBoneTransformsTo(BoneTransforms);
        }
    }

}
