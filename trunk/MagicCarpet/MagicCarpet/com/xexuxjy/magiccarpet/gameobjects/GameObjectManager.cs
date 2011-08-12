using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace com.xexuxjy.magiccarpet.gameobjects
{
    public class GameObjectManager : GameComponent
    {
        public GameObjectManager(Game game)
            : base(game)
        {
        }

        public GameObject CreateAndInitialiseGameObject(GameObjectType gameObjectType, Vector3 startPosition)
        {
            GameObject gameObject = null;
            if (gameObjectType == GameObjectType.ManaBall)
            {

                gameObject = new ManaBall(startPosition, Game);
            }

            if (gameObject != null)
            {
                gameObject.Initialize();
                m_gameObjectList.Add(gameObject);
            }
            return gameObject;
        }

        private IList<GameObject> m_gameObjectList = new List<GameObject>();
    }
}
