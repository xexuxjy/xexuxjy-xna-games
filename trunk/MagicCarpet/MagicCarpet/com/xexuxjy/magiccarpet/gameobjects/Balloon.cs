using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.actions;
using System.Diagnostics;

namespace com.xexuxjy.magiccarpet.gameobjects
{
    public class Balloon : GameObject
    {
        public Balloon(Game game)
            : base(game,GameObjectType.Balloon)
        {
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public Balloon(Vector3 startPosition, Game game)
            : base(startPosition,game)
        {
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Initialize()
        {
            base.Initialize();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (m_currentAction.ActionState == ActionState.Unloading)
            {
                // todo transfer mana to castle over time 

            }



        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////


        public void FindTarget()
        {


        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void MoveToTarget()
        {

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Load(ManaBall manaBall)
        {
            CurrentAction = new ActionLoad(this,manaBall);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Unload(Castle castle)
        {
            CurrentAction = new ActionUnload(this,castle);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void value_ActionComplete(BaseAction action)
        {
            switch (action.ActionState)
            {
                case (ActionState.Loading):
                    {
                        ManaBall manaball = action.Target as ManaBall;
                        Debug.Assert(manaball != null);
                        m_currentLoad += manaball.ManaValue;
                        // loaded now so remove object.
                        Globals.GameObjectManager.RemoveGameObject(manaball);

                        break;
                    }

                case (ActionState.Unloading):
                    {
                        // change this so unload is gradual
                        Castle castle = action.Target as Castle;
                        Debug.Assert(castle != null);
                        castle.StoredMana += CurrentLoad;
                        CurrentLoad = 0f;

                        break;
                    }
                case (ActionState.Dead):
                    {
                        // drop current load as series of mana balls
                        // then remove ourselves from the game.
                        Globals.GameObjectManager.RemoveGameObject(this);
                        break;
                    }
                default:
                    break;
            }

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public float CurrentLoad
        {
            get { return m_currentLoad; }
            set { m_currentLoad = value; }
        }

        
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void value_ActionStarted(BaseAction action)
        {
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private float m_currentLoad;
        private float m_maxLoad;
        

    }

}
