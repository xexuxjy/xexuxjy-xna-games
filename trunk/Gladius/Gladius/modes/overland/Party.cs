using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gladius.util;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Gladius.renderer.animation;
using Gladius.events;
using Gladius.control;
using Microsoft.Xna.Framework.Graphics;
using Dhpoware;

namespace Gladius.modes.overland
{
    public class Party : GameScreenComponent
    {
        public Party(GameScreen gameScreen)
            : base(gameScreen)
        {
             m_animatedModel = new AnimatedModel();
             m_animatedModel.ModelRotation = Quaternion.CreateFromAxisAngle(Vector3.Up, (float)Math.PI);
             m_animatedModel.ModelName = "Models/ThirdParty/01_warrior";
             m_animatedModel.DebugName = "Party";
             Rotation = Quaternion.Identity;
        }

        public override void LoadContent()
        {
            m_animatedModel.LoadContent(ContentManager);
        }


        public override void VariableUpdate(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.VariableUpdate(gameTime);
        }


        public override void RegisterListeners()
        {
            EventManager.ActionPressed += new EventManager.ActionButtonPressed(EventManager_ActionPressed);
        }

        public override void UnregisterListeners()
        {
            //EventManager.ActionPressed -= new event ActionButtonPressed();
            EventManager.ActionPressed -= new EventManager.ActionButtonPressed(EventManager_ActionPressed);

        }

        void EventManager_ActionPressed(object sender, ActionButtonPressedArgs e)
        {
            switch (e.ActionButton)
            {
                case (ActionButton.ActionLeft):
                    {
                        
                        break;
                    }
                case (ActionButton.ActionRight):
                    {
                        
                        break;
                    }
                case (ActionButton.ActionUp):
                    {
                        
                        break;
                    }
                case (ActionButton.ActionDown):
                    {
                        
                        break;
                    }
                case (ActionButton.ActionButton1):
                    {
                        
                        break;
                    }
                // cancel
                case (ActionButton.ActionButton2):
                    {
                        
                        break;
                    }
            }
        }



        public override void Draw(GameTime gameTime)
        {
            Draw(Game.GraphicsDevice, Globals.Camera, gameTime);
        }

        public virtual void Draw(GraphicsDevice device, CameraComponent camera, GameTime gameTime)
        {
            if (m_animatedModel != null)
            {
                m_animatedModel.ActorRotation = Rotation;
                m_animatedModel.ActorPosition = Position;

                m_animatedModel.Draw(device, camera, gameTime);
            }
        }

        public Quaternion Rotation
        {
            get;
            set;
        }

        public Vector3 Position
        {
            get;
            set;
        }

        AnimatedModel m_animatedModel;

    }
}
