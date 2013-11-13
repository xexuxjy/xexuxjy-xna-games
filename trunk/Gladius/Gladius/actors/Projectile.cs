using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gladius.util;
using Microsoft.Xna.Framework;
using Gladius.renderer;
using Microsoft.Xna.Framework.Graphics;
using Gladius.gamestatemanagement.screenmanager;
using Gladius.gamestatemanagement.screens;

namespace Gladius.actors
{
    public class Projectile : GameScreenComponent
    {
        public Projectile(GameScreen gameScreen)
            : base(gameScreen)
        {
            Speed =1f;
        }

        public ArenaScreen ArenaScreen
        {
            get
            {
                return m_gameScreen as ArenaScreen;
            }
        }


        public override void VariableUpdate(GameTime gameTime)
        {
            base.VariableUpdate(gameTime);
            Position += m_velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector3 diff = Target.Position - Position;
            // don't care about height check.
            diff.Y = 0;
            float closeEnough = 0.1f;
            float len = diff.Length();
            if (len < closeEnough)
            {
                // do damage.
                // end state.
                ArenaScreen.CombatEngine.ResolveAttack(Owner, Target, Owner.CurrentAttackSkill);
                //Owner.Attacking = false;
                Owner.StopAttack(); 
                Enabled = false;
                Visible = false;

            }

            Matrix newWorld = Matrix.CreateTranslation(Position);
            newWorld.Up = Vector3.Up;
            newWorld.Forward = Vector3.Normalize(diff);
            newWorld.Right = Vector3.Cross(newWorld.Up,newWorld.Forward);

            World = newWorld;

        }

        public override void Draw(GameTime gameTime, ICamera camera)
        {
            m_modelData.Draw(camera,World);
        }

        public override void LoadContent()
        {
            base.LoadContent();
            ModelName = "Models/Shapes/UnitCylinder";
            m_modelData = new ModelData(ContentManager.Load<Model>(ModelName), new Vector3(0.05f,0.5f,0.05f),0f, ContentManager.GetColourTexture(Color.Magenta));
            m_modelData.ModelRotation = Matrix.CreateFromYawPitchRoll(0, MathHelper.PiOver2, 0);
            DrawOrder = Globals.CharacterDrawOrder;
        }

        public Matrix World
        { get; set; }

        public String ModelName
        {
            get;
            set;
        }

        private BaseActor m_target;
        private Vector3 m_velocity;

        public BaseActor Target
        {
            get
            {
                return m_target;
            }
            set
            {
                m_target = value;
                Vector3 diff = m_target.Position - Position;
                diff.Y = 0;
                diff.Normalize();
                m_velocity = diff * Speed;

            }


        }


        

        public BaseActor Owner
        {
            get;
            set;
        }

        public float Speed
        {
            get;
            set;
        }

        //public Vector3 Velocity
        //{
        //    get;
        //    set;
        //}

        public Vector3 Position
        {
            get;
            set;
        }

        private ModelData m_modelData;
    }
}
