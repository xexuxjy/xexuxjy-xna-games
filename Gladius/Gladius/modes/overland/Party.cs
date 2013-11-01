using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gladius.util;
using Microsoft.Xna.Framework;
using Gladius.renderer.animation;
using Gladius.events;
using Gladius.control;
using Microsoft.Xna.Framework.Graphics;
using Gladius.renderer;
using Gladius.gamestatemanagement.screenmanager;

namespace Gladius.modes.overland
{
    public class Party : GameScreenComponent
    {
        public Party(GameScreen gameScreen,Terrain terrain)
            : base(gameScreen)
        {
            ModelHeight = 0.2f;
            m_animatedModel = new AnimatedModel(ModelHeight);
             m_animatedModel.ModelRotation = Quaternion.CreateFromAxisAngle(Vector3.Up, (float)Math.PI);
             m_animatedModel.ModelName = "Models/ThirdParty/01_warrior";
             m_animatedModel.DebugName = "Party";
             Rotation = Quaternion.Identity;
             m_terrain = terrain;
             Gold = 1000;
        }

        public override void LoadContent()
        {
            m_animatedModel.LoadContent(ContentManager);
            // test for now.
            m_animatedModel.SetMeshActive("w_helmet_01", false);
            m_animatedModel.SetMeshActive("w_shoulder_01", false);

            m_animatedModel.SetMeshActive("bow_01", false);
            m_animatedModel.SetMeshActive("shield_01", false);

            m_animatedModel.PlayAnimation(AnimationEnum.Walk);
        }


        public override void VariableUpdate(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.VariableUpdate(gameTime);


            Matrix model = Matrix.CreateFromQuaternion(Rotation);
            Globals.Camera.Target = LookAtPoint;
            Globals.Camera.Up = model.Up;

            //Vector3 direction = (3 * model.Forward) + model.Up;
            //direction.Normalize();

            Globals.Camera.TargetDirection = model.Forward;

            UpdateMovement(gameTime);

            m_animatedModel.Update(gameTime);
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

        public void UpdateMovement(GameTime gameTime)
        {
            float speed = 0f;
            float rotateSpeed = (float)Math.PI;

            float multiplier = 10f;
            if (Globals.UserControl.CursorLeftHeld() || Globals.UserControl.CursorRightHeld() || Globals.UserControl.CursorUpHeld() || Globals.UserControl.CursorDownHeld() )
            {
                speed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (Globals.UserControl.CursorLeftHeld())
            {
                UpdateYaw((float)(rotateSpeed * gameTime.ElapsedGameTime.TotalSeconds));
            }
            else if (Globals.UserControl.CursorRightHeld())
            {
                UpdateYaw((float)(-rotateSpeed * gameTime.ElapsedGameTime.TotalSeconds));
            }

            if(Globals.UserControl.CursorUpHeld())
            {
                speed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                speed *= multiplier;
            }

            Position += ((Matrix.CreateFromQuaternion(m_animatedModel.ActorRotation).Forward) * speed);
            
            Vector3 pos = Position;
            m_terrain.GetHeightAtPoint(ref pos);
            Position = pos;

        }


        void EventManager_ActionPressed(object sender, ActionButtonPressedArgs e)
        {
            switch (e.ActionButton)
            {
                //case (ActionButton.ActionLeft):
                //    {
                //        Quaternion face = Quaternion.CreateFromAxisAngle(Vector3.Up, (float)Math.PI / 2);
                        
                //        Rotation = face;
                //        break;
                //    }
                //case (ActionButton.ActionRight):
                //    {
                //        Quaternion face = Quaternion.CreateFromAxisAngle(Vector3.Up, (float)(3*Math.PI) / 2);
                //        Rotation = face;
                        
                //        break;
                //    }
                //case (ActionButton.ActionUp):
                //    {
                //        Quaternion face = Quaternion.CreateFromAxisAngle(Vector3.Up, (float)Math.PI);
                //        Rotation = face;
 
                //        break;
                //    }
                //case (ActionButton.ActionDown):
                //    {
                //        Quaternion face = Quaternion.CreateFromAxisAngle(Vector3.Up, 0f);
                //        Rotation = face;
 
                //        break;
                //    }
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

        public virtual void Draw(GraphicsDevice device, ICamera camera, GameTime gameTime)
        {
            if (m_animatedModel != null)
            {
                m_animatedModel.ActorRotation = Rotation;
                m_animatedModel.ActorPosition = Position;

                m_animatedModel.Draw(device, camera, gameTime);
            }
        }

        private Quaternion _quat;
        public Quaternion Rotation
        {
            get
            {
                return _quat;
            }
            set
            {
                _quat = value;
                m_animatedModel.ActorRotation = value;
            }
        }

        public Vector3 Position
        {
            get;
            set;
        }

        public Vector3 LookAtPoint
        {
            get
            {
                Vector3 position = Position;
                position.Y += (m_animatedModel.BoundingBox.Max.Y - m_animatedModel.BoundingBox.Min.Y);
                return position;
            }
        }

        public void UpdateYaw(float val)
        {
            m_accumulatedYaw += val;
            Rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, m_accumulatedYaw);
        }

        public float ModelHeight
        {
            get;
            set;
        }

        public int Gold
        {
            get;
            set;
        }

        public String LeagueRank
        {
            get { return "Bronze"; }
        }


        float m_accumulatedYaw;
        AnimatedModel m_animatedModel;
        Terrain m_terrain;


    }
}
