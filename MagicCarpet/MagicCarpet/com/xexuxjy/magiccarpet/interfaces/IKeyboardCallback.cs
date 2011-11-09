using Microsoft.Xna.Framework.Input;

namespace com.xexuxjy.magiccarpet.interfaces
{
    public interface IKeyboardCallback
    {
        void KeyboardCallback(Keys key, bool released, ref KeyboardState newState, ref KeyboardState oldState);
    }
}
