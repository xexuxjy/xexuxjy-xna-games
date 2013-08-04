using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xexuxjy.Gladius.util;

namespace Gladius.actors
{
    public class TestPlayer : BaseActor
    {
        public TestPlayer()
        {
            Name = "Player";
            SetAttributeValue(GameObjectAttributeType.Health, 100);
            SetAttributeValue(GameObjectAttributeType.Defense, 10);
            SetAttributeValue(GameObjectAttributeType.Agility, 10);
            SetAttributeValue(GameObjectAttributeType.Movement, 20);
        }


    }
}
