using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace com.xexuxjy.magiccarpet.debug
{
    /// <summary>
    ///  Provides a base for the different debug window type classes (profiler, console, etc)
    /// </summary>
    public class DebugWindow : DrawableGameComponent
    {
        public DebugWindow(string id,Game game) : base(game)
        {
            Id = id;
            m_spriteBatch = new SpriteBatch(game.GraphicsDevice);
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

        public SpriteBatch SpriteBatch
        {
            get { return m_spriteBatch; }
        }

        private string m_id;
        private bool m_enabled;
        private Vector2 m_screenPosition;
        private SpriteBatch m_spriteBatch;
    }
}
