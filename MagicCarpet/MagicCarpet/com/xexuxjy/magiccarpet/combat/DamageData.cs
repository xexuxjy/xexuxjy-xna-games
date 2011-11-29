using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;

namespace com.xexuxjy.magiccarpet.combat
{
    public struct DamageData
    {
        public DamageData(GameObject damager, float damage)
        {
            m_damager = damager;
            m_damage = damage;

        }

        public GameObject m_damager;
        public float m_damage;
    }
}
