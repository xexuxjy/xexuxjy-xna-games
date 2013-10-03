using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xexuxjy.Gladius.util;
using GameStateManagement;

namespace Gladius.actors
{
    public class TestPlayer : BaseActor
    {
        public TestPlayer(GameScreen gameScreen) : base
            (gameScreen)
        {
            Name = "Player";
            SetAttributeValue(GameObjectAttributeType.Health, 100);
            SetAttributeValue(GameObjectAttributeType.Defense, 10);
            SetAttributeValue(GameObjectAttributeType.Accuracy, 10);
            SetAttributeValue(GameObjectAttributeType.Movement, 20);
        }


    }
}
