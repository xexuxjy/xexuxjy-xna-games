using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace com.xexuxjy.magiccarpet.gui
{
    public class PlayerHud : GuiComponent
    {
        public PlayerHud(int x, int y, int width) : base(x,y,width)
        {
        }





        private SpellSelector m_spellSelector;
        private MiniMap m_miniMap;
        private EventWindow m_eventWindow;
    }
}
