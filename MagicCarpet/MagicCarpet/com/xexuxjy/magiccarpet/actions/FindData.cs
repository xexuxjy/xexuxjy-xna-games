using com.xexuxjy.magiccarpet.gameobjects;
namespace com.xexuxjy.magiccarpet.actions
{
    public struct FindData
    {
        public GameObject m_owner;
        public GameObject m_target;
        public GameObjectType m_findMask;
        public float m_findRadius ;

        public float m_magicianWeight ;
        public float m_manaballWeight ;
        public float m_castleWeight ;
        public float m_balloonWeight ;
        public float m_spellWeight ;
        public float m_playerWeight ;
        public float m_monsterWeight ;
        public float m_terrainWeight ;

        public float GetTotalWeight()
        {
            return m_magicianWeight + m_manaballWeight + m_castleWeight + m_balloonWeight + m_spellWeight + m_playerWeight + m_monsterWeight + m_terrainWeight;
        }


        public float GetWeightForType(GameObjectType gameObjectType)
        {
            float total = GetTotalWeight();
            float weight = 0f;
            if (total > 0f)
            {
                switch (gameObjectType)
                {
                    case GameObjectType.balloon :
                    {
                        weight = m_balloonWeight;
                        break;
                    }
                    case GameObjectType.castle:
                    {
                        weight = m_castleWeight;
                        break;
                    }
                    case GameObjectType.magician:
                    {
                        weight = m_magicianWeight;
                        break;
                    }
                    case GameObjectType.manaball:
                    {
                        weight = m_manaballWeight;
                        break;
                    }
                    case GameObjectType.monster:
                    {
                        weight = m_monsterWeight;
                        break;
                    }
                    case GameObjectType.player:
                    {
                        weight = m_playerWeight;
                        break;
                    }
                    case GameObjectType.spell:
                    {
                        weight = m_spellWeight;
                        break;
                    }
                    case GameObjectType.terrain:
                    {
                        weight = m_terrainWeight;
                        break;
                    }
                    default :
                        break;
                }

                return weight / total;
            }
            else
            {
                return 0f;
            }
        }

        public static FindData GetCastleFindData(GameObject owner, float searchRadius)
        {
            FindData findData = new FindData();
            findData.m_owner = owner;
            findData.m_findMask = GameObjectType.castle;
            findData.m_findRadius = searchRadius;
            findData.m_castleWeight = 1.0f;
            return findData;
        }

        public static FindData GetManaballFindData(GameObject owner, float searchRadius)
        {
            FindData findData = new FindData();
            findData.m_owner = owner;
            findData.m_findMask = GameObjectType.manaball;
            findData.m_findRadius = searchRadius;
            findData.m_manaballWeight = 1.0f;
            return findData;
        }


        public static FindData GetActionFindEnemy(GameObject owner, float searchRadius)
        {
            FindData findData = new FindData();
            findData.m_owner = owner;
            if (owner is Monster)
            {
                findData.m_findMask = GameObjectType.magician | GameObjectType.balloon | GameObjectType.castle;
                findData.m_magicianWeight = 1.0f;
                findData.m_balloonWeight = 0.7f;
                findData.m_castleWeight = 0.3f;
            }
            else if (owner is Magician)
            {
                findData.m_findMask = GameObjectType.magician | GameObjectType.balloon | GameObjectType.castle | GameObjectType.monster;
                findData.m_magicianWeight = 1.0f;
                findData.m_balloonWeight = 0.7f;
                findData.m_monsterWeight = 0.6f;
                findData.m_castleWeight = 0.3f;
            }

            findData.m_findRadius = searchRadius;
            findData.m_manaballWeight = 1.0f;
            return findData;
        }

    }
}
