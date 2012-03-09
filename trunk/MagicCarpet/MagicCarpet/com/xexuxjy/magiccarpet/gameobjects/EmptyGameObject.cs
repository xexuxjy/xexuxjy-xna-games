using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace com.xexuxjy.magiccarpet.gameobjects
{
    public class EmptyGameObject : GameObject
    {
        public EmptyGameObject(GameObjectType type)
            : base(type)
        {
        }

        public override void Initialize()
        {
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(GameTime gameTime)
        {
        }
    }
}
