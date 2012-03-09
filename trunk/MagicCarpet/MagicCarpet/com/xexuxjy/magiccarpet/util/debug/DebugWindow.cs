using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BulletXNA.LinearMath;
using GameStateManagement;
using com.xexuxjy.magiccarpet.gameobjects;
namespace com.xexuxjy.magiccarpet.util.debug
{
    /// <summary>
    ///  Provides a base for the different debug window type classes (profiler, console, etc)
    /// </summary>
    public class DebugWindow : EmptyGameObject
    {
        public DebugWindow(string id,IDebugDraw debugDraw)
            : base(GameObjectType.gui)

        {
            Id = id;
            m_debugDraw = debugDraw;
            DrawOrder = Globals.GUI_DRAW_ORDER; 
        }

        public Vector2 ScreenPosition
        {
            get { return m_screenPosition; }
            set { m_screenPosition = value; }
        }

        public string Id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        public IDebugDraw DebugDraw
        {
            get { return m_debugDraw; }
            set { m_debugDraw = value; }
        }


        private bool m_enabled;
        private string m_id;
        private Vector2 m_screenPosition;
        private IDebugDraw m_debugDraw;
    }
}
