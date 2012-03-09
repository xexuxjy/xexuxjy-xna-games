using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BulletXNA.LinearMath;
using com.xexuxjy.magiccarpet.gameobjects;
using com.xexuxjy.magiccarpet;

namespace com.xexuxjy.magiccarpet.util.debug
{
    public class DebugObjectManager : DebugWindow
    {

        public DebugObjectManager(IDebugDraw debugDraw)
            : base("DebugObjectManager", debugDraw)
        {

        }

        public override void Draw(GameTime gameTime)
        {
            if (Enabled)
            {
                String outputString = "Not Active";
                if (m_debuggable != null)
                {
                    outputString = m_debuggable.DebugText;
                }
                DebugDraw.DrawText(outputString, m_debugWindowPosition, Vector3.One);
            }
        }

        public void NextObject()
        {
            IList<GameObject> objectList = Globals.GameObjectManager.DebugObjectList;
            if(m_debuggable == null && objectList.Count > 0)
            {
                m_debuggable = objectList[0];
            }
            else if(objectList.Count > 0)
            {
                int index = objectList.IndexOf(m_debuggable);
                if (index < objectList.Count - 1)
                {
                    index++;
                }
                else
                {
                    index = 0;
                }
                m_debuggable = objectList[index];
            }
        }

        public void PreviousObject()
        {
            IList<GameObject> objectList = Globals.GameObjectManager.DebugObjectList;
            if(m_debuggable == null && objectList.Count > 0)
            {
                m_debuggable = objectList[0];
            }
            else if(objectList.Count > 0)
            {
                int index = objectList.IndexOf(m_debuggable);
                if (index > 0)
                {
                    index--;
                }
                else
                {
                    index = objectList.Count -1;
                }
                m_debuggable = objectList[index];
            }

        }

        public GameObject DebugObject
        {
            get { return m_debuggable; }
            set { m_debuggable = value; }
        }

        private GameObject m_debuggable;
        private Vector3 m_debugWindowPosition = new Vector3(10, 0, 10);
    }
}
