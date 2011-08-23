﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BulletXNA.BulletCollision;
using com.xexuxjy.magiccarpet.renderer;

namespace com.xexuxjy.magiccarpet.gameobjects
{
    public class ManaBall : GameObject
    {

        public ManaBall(Vector3 startPosition,Game game)
            : base( startPosition,game)
        {
            m_defaultRenderer = new DefaultRenderer(game,this);
            m_modelName = Globals.manaballModel;

        }


        protected override void BuildCollisionObject()
        {
            if (s_collisionShape == null)
            {
                s_collisionShape = new SphereShape(1f);
            }

            m_rigidBody = Globals.CollisionManager.LocalCreateRigidBody(1f, Matrix.CreateTranslation(Position), s_collisionShape,m_motionState,true);
        }

        protected static CollisionShape s_collisionShape;
    }
}