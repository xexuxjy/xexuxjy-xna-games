using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Gladius.renderer;
using Gladius.gamestatemanagement.screenmanager;

namespace Gladius.util
{
    public abstract class GameScreenComponent : IGameComponent,IDrawable,IUpdateable
    {
        public GameScreenComponent(GameScreen gameScreen)
        {
            m_gameScreen = gameScreen;
            UpdateFrequency = 1;
        }

        public virtual void Draw(GameTime gameTime, ICamera camera)
        {

        }

        public virtual void Initialize()
        {

        }




        public void Update(GameTime gameTime)
        {
            ++m_updateCounter;
            if (m_updateCounter == UpdateFrequency)
            {
                m_updateCounter = 0;
                VariableUpdate(gameTime);
            }
        }

        public virtual void VariableUpdate(GameTime gameTime)
        {

        }

        public virtual void Draw(GameTime gameTime)
        {

        }

        public virtual void LoadContent()
        {

        }

        public virtual void RegisterListeners()
        {
        }

        public virtual void UnregisterListeners()
        {
        }

        public Game Game
        {
            get 
            {
                return m_gameScreen.ScreenManager.Game;
            }
        }

        public ThreadSafeContentManager ContentManager
        {
            get
            {
                return m_gameScreen.ContentManager;
            }
        }


        protected int m_updateCounter;


        public int UpdateFrequency
        {
            get;set;
        }

        public int UpdateOrder
        {
            get { return updateOrder; }
            set
            {
                if (updateOrder != value)
                {
                    updateOrder = value;
                    OnUpdateOrderChanged(this, EventArgs.Empty);
                }
            }
        }

        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    OnEnabledChanged(this, EventArgs.Empty);
                }
            }
        }

        protected virtual void OnEnabledChanged(object sender, EventArgs args)
        {
            if (EnabledChanged != null)
            {
                EnabledChanged(this, args);
            }
        }

        protected virtual void OnUpdateOrderChanged(object sender, EventArgs args)
        {
            if (UpdateOrderChanged != null)
            {
                UpdateOrderChanged(this, args);
            }
        }

        public bool Visible
        {
            get { return visible; }
            set
            {
                if (visible != value)
                {
                    visible = value;
                    OnVisibleChanged(this, EventArgs.Empty);
                }
            }
        }


        public int DrawOrder
        {
            get { return drawOrder; }
            set
            {
                if (drawOrder != value)
                {
                    drawOrder = value;
                    OnDrawOrderChanged(this, EventArgs.Empty);
                }
            }
        }

        public virtual void OnDrawOrderChanged(object sender, EventArgs args)
        {
            if (DrawOrderChanged != null)
            {
                DrawOrderChanged(sender, args);
            }
        }

        public virtual void OnVisibleChanged(object sender, EventArgs args)
        {
            if (VisibleChanged != null)
            {
                VisibleChanged(sender, args);
            }
        }

        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;
        public event EventHandler<EventArgs> Disposed;

        protected GameScreen m_gameScreen;
        private int updateOrder;

        private int drawOrder;
        private bool visible = true;
        private bool initialized;

        IGraphicsDeviceService deviceService;
        public event EventHandler<EventArgs> VisibleChanged;
        public event EventHandler<EventArgs> DrawOrderChanged;

        private bool enabled = true;    
    }
}
