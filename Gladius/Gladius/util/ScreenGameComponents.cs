using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gladius.util
{
    public class ScreenGameComponents
    {
        public ScreenGameComponents(Game game)
        {
            game = game;
            Components = new GameComponentCollection();
            Components.ComponentAdded += new EventHandler<GameComponentCollectionEventArgs>(GameComponentAdded);
            Components.ComponentRemoved += new EventHandler<GameComponentCollectionEventArgs>(GameComponentRemoved);
        }

        public void Initialize()
        {
            while (notYetInitialized.Count != 0)
            {
                notYetInitialized[0].Initialize();
                notYetInitialized.RemoveAt(0);
            }
        }

        public void Update(GameTime gameTime)
        {
            for (int index = 0; index < updateableComponents.Count; ++index)
            {
                currentlyUpdatingComponents.Add(updateableComponents[index]);
            }

            for (int index = 0; index < currentlyUpdatingComponents.Count; ++index)
            {
                IUpdateable updateable = currentlyUpdatingComponents[index];
                if (updateable.Enabled)
                {
                    updateable.Update(gameTime);
                }
            }
            currentlyUpdatingComponents.Clear();
            //FrameworkDispatcher.Update();
            //doneFirstUpdate = true;
        }

        public void Draw(GameTime gameTime)
        {
            for (int index = 0; index < drawableComponents.Count; ++index)
            {
                currentlyDrawingComponents.Add(drawableComponents[index]);
            }
            for (int index = 0; index < currentlyDrawingComponents.Count; ++index)
            {
                IDrawable drawable = currentlyDrawingComponents[index];
                if (drawable.Visible)
                {
                    drawable.Draw(gameTime);
                }
            }
            currentlyDrawingComponents.Clear();
        }

        private void GameComponentRemoved(object sender, GameComponentCollectionEventArgs e)
        {
            if (!inRun)
            {
                notYetInitialized.Remove(e.GameComponent);
            }
            IUpdateable updateable = e.GameComponent as IUpdateable;
            if (updateable != null)
            {
                updateableComponents.Remove(updateable);
                updateable.UpdateOrderChanged -= new EventHandler<EventArgs>(UpdateableUpdateOrderChanged);
            }
            IDrawable drawable = e.GameComponent as IDrawable;
            if (drawable == null)
            {
                return;
            }
            drawableComponents.Remove(drawable);
            drawable.DrawOrderChanged -= new EventHandler<EventArgs>(DrawableDrawOrderChanged);
        }

        private void GameComponentAdded(object sender, GameComponentCollectionEventArgs e)
        {
            if (inRun)
            {
                e.GameComponent.Initialize();
            }
            else
            {
                notYetInitialized.Add(e.GameComponent);
            }
            IUpdateable updateable = e.GameComponent as IUpdateable;
            if (updateable != null)
            {
                int num = updateableComponents.BinarySearch(updateable, (IComparer<IUpdateable>)UpdateOrderComparer.Default);
                if (num < 0)
                {
                    int index = ~num;
                    while (index < updateableComponents.Count && updateableComponents[index].UpdateOrder == updateable.UpdateOrder)
                    {
                        ++index;
                    }
                    updateableComponents.Insert(index, updateable);
                    updateable.UpdateOrderChanged += new EventHandler<EventArgs>(UpdateableUpdateOrderChanged);
                }
            }
            IDrawable drawable = e.GameComponent as IDrawable;
            if (drawable == null)
            {
                return;
            }
            int num1 = drawableComponents.BinarySearch(drawable, (IComparer<IDrawable>)DrawOrderComparer.Default);
            if (num1 >= 0)
            {
                return;
            }
            int index1 = ~num1;
            while (index1 < drawableComponents.Count && drawableComponents[index1].DrawOrder == drawable.DrawOrder)
            {
                ++index1;
            }
            drawableComponents.Insert(index1, drawable);
            drawable.DrawOrderChanged += new EventHandler<EventArgs>(DrawableDrawOrderChanged);
        }

        private void DrawableDrawOrderChanged(object sender, EventArgs e)
        {
            IDrawable drawable = sender as IDrawable;
            drawableComponents.Remove(drawable);
            int num = drawableComponents.BinarySearch(drawable, (IComparer<IDrawable>)DrawOrderComparer.Default);
            if (num >= 0)
            {
                return;
            }
            int index = ~num;
            while (index < drawableComponents.Count && drawableComponents[index].DrawOrder == drawable.DrawOrder)
            {
                ++index;
            }
            drawableComponents.Insert(index, drawable);
        }

        private void UpdateableUpdateOrderChanged(object sender, EventArgs e)
        {
            IUpdateable updateable = sender as IUpdateable;
            updateableComponents.Remove(updateable);
            int num = updateableComponents.BinarySearch(updateable, (IComparer<IUpdateable>)UpdateOrderComparer.Default);
            if (num >= 0)
            {
                return;
            }
            int index = ~num;
            while (index < updateableComponents.Count && updateableComponents[index].UpdateOrder == updateable.UpdateOrder)
            {
                ++index;
            }
            updateableComponents.Insert(index, updateable);
        }


        private Game game;
        public GameComponentCollection Components;

        private List<IUpdateable> updateableComponents = new List<IUpdateable>();
        private List<IUpdateable> currentlyUpdatingComponents = new List<IUpdateable>();
        private List<IDrawable> drawableComponents = new List<IDrawable>();
        private List<IDrawable> currentlyDrawingComponents = new List<IDrawable>();
        private List<IGameComponent> notYetInitialized = new List<IGameComponent>();
        private bool inRun = true;
    }

    internal class DrawOrderComparer : IComparer<IDrawable>
    {
        public static readonly DrawOrderComparer Default = new DrawOrderComparer();

        static DrawOrderComparer()
        {
        }

        public int Compare(IDrawable x, IDrawable y)
        {
            if (x == null && y == null)
                return 0;
            if (x == null)
                return 1;
            if (y == null)
                return -1;
            if (x.Equals((object)y))
                return 0;
            return x.DrawOrder < y.DrawOrder ? -1 : 1;
        }
    }

    internal class UpdateOrderComparer : IComparer<IUpdateable>
    {
        public static readonly UpdateOrderComparer Default = new UpdateOrderComparer();

        static UpdateOrderComparer()
        {
        }

        public int Compare(IUpdateable x, IUpdateable y)
        {
            if (x == null && y == null)
                return 0;
            if (x == null)
                return 1;
            if (y == null)
                return -1;
            if (x.Equals((object)y))
                return 0;
            return x.UpdateOrder < y.UpdateOrder ? -1 : 1;
        }
    }
}
