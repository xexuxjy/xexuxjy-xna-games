using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.spells;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.util;
using com.xexuxjy.magiccarpet.actions;
using com.xexuxjy.magiccarpet.combat;
using System.Diagnostics;
using com.xexuxjy.magiccarpet.manager;
using BulletXNA.LinearMath;
using Microsoft.Xna.Framework.Graphics;
using BulletXNA;
using com.xexuxjy.magiccarpet.renderer;
using BulletXNA.BulletCollision;

namespace com.xexuxjy.magiccarpet.gameobjects
{
    public class Magician : GameObject
    {
        public Magician(Vector3 startPosition)
            : base(startPosition, GameObjectType.magician)
        {
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void LoadContent()
        {
            base.LoadContent();
            if (m_carpetVertexBuffer == null)
            {
                float carpetWidth = 1f;
                float carpetLength = 1f;
                int carpetSegments = 99;
                float segmentStep = carpetLength / carpetSegments;

                ObjectBuilderUtil.BuildClothObject(carpetSegments, carpetWidth, carpetLength, out m_carpetDimensions,out m_carpetVertexBuffer);

                m_carpetEffect = Globals.MCContentManager.GetEffect("OwnerColour");
                m_carpetTexture = Globals.MCContentManager.GetTexture("Carpet2");
            }
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void BuildCollisionObject()
        {
            Vector3 magicianDimensions = new Vector3(m_carpetDimensions.X, 0.2f, m_carpetDimensions.Z);
            CollisionShape collisionShape = new BoxShape(magicianDimensions / 2);
            CollisionFilterGroups collisionFlags = (CollisionFilterGroups)GameObjectType.magician;
            CollisionFilterGroups collisionMask = (CollisionFilterGroups)(GameObjectType.spell | GameObjectType.manaball | GameObjectType.camera | GameObjectType.terrain);
            m_collisionObject = Globals.CollisionManager.LocalCreateRigidBody(0f, Matrix.CreateTranslation(Position), collisionShape, GetMotionState(), true, this, collisionFlags, collisionMask);
            m_collisionObject.SetCollisionFlags(m_collisionObject.GetCollisionFlags() | CollisionFlags.CF_KINEMATIC_OBJECT);
            BuildRayCallback();
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override float GetHoverHeight()
        {
            return 1.0f;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Initialize()
        {
            base.Initialize();
            PlayerControlled = true;
            m_scaleTransform = Matrix.CreateScale(0.2f);
            // after init so we get the right draw order.
            DrawOrder = Globals.GUI_DRAW_ORDER;
            StickToGround = false;


        }

        public SpellType SelectedSpell1
        {
            get { return m_selectedSpell1; }
            set { m_selectedSpell1 = value; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public SpellType SelectedSpell2
        {
            get { return m_selectedSpell2; }
            set { m_selectedSpell2 = value; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void CastSpell1(Vector3 start, Vector3 direction)
        {
            CastSpell(m_selectedSpell1, start, direction);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void CastSpell2(Vector3 start, Vector3 direction)
        {
            CastSpell(m_selectedSpell2, start, direction);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!m_playerControlled)
            {
                if (ActionState == ActionState.None)
                {
                    QueueAction(Globals.ActionPool.GetActionIdle(this));
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Draw(GameTime gameTime)
        {
            if (Globals.s_currentCameraFrustrum.Intersects(BoundingBox))
            {

                Globals.MCContentManager.ApplyCommonEffectParameters(m_carpetEffect);

                Globals.GraphicsDevice.SetVertexBuffer(m_carpetVertexBuffer);

                Vector3 startPosition = Position;
                Matrix worldMatrix = WorldTransform;

                Vector3 forward = WorldTransform.Forward;

                m_carpetEffect.Parameters["WorldMatrix"].SetValue(worldMatrix);
                m_carpetEffect.Parameters["ClothTexture"].SetValue(m_carpetTexture);
                m_carpetEffect.Parameters["OwnerColor"].SetValue(BadgeColor.ToVector3());

                float timeScalar = 4f;
                float frequency = 4f;
                m_carpetMovementOffset += (timeScalar * m_carpetDimensions.W * (float)gameTime.ElapsedGameTime.TotalSeconds);

                m_carpetEffect.Parameters["Frequency"].SetValue(frequency);
                m_carpetEffect.Parameters["Amplitude"].SetValue(m_carpetDimensions.Y);
                m_carpetEffect.Parameters["ClothLength"].SetValue(m_carpetDimensions.Z);


                m_carpetEffect.Parameters["ClothMovementOffset"].SetValue(m_carpetMovementOffset);



                int noTriangles = m_carpetVertexBuffer.VertexCount / 3;

                m_carpetEffect.CurrentTechnique = m_carpetEffect.Techniques["DrawCloth"];

                foreach (EffectPass pass in m_carpetEffect.CurrentTechnique.Passes)
                {
                    int noVertices = m_carpetVertexBuffer.VertexCount;
                    pass.Apply();
                    Globals.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, noTriangles);
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////


        public override void ActionComplete(BaseAction action)
        {
            switch (action.ActionState)
            {
                case (ActionState.Searching):
                    {

                        ActionFind actionFind = action as ActionFind;
                        Debug.Assert(actionFind != null);

                        if (actionFind.TargetLocation.HasValue)
                        {
                            // Going to a location so queue that up.
                            QueueAction(Globals.ActionPool.GetActionTravel(this, null, actionFind.TargetLocation, Globals.s_magicianTravelSpeed));
                        }
                        else if(action.Target != null)
                        {
                            bool shouldAttack = GameObjectManager.IsAttackable(action.Target);

                            if(action.Target.Alive)
                            {
                                if (shouldAttack)
                                {
                                    if (GameUtil.InRange(this, action.Target, Globals.s_magicianMeleeRange))
                                    {
                                        QueueAction(Globals.ActionPool.GetActionAttackMelee(this, action.Target, Globals.s_magicianMeleeRange, Globals.s_magicianMeleeDamage));
                                    }
                                    else if (GameUtil.InRange(this, action.Target, Globals.s_magicianRangedDamage))
                                    {
                                        QueueAction(Globals.ActionPool.GetActionAttackRange(this, action.Target, Globals.s_magicianRangedRange, Globals.s_magicianRangedDamage, SpellType.Fireball));
                                    }
                                    else
                                    {
                                        // Move towards the target?
                                        // need something a bit cleverer here , depending on the target
                                        // have min distance of spell cast range for items?
                                        QueueAction(Globals.ActionPool.GetActionTravel(this, action.Target, null, Globals.s_magicianTravelSpeed, 0f, Globals.s_magicianMaxFollowRange));
                                        QueueAction(Globals.ActionPool.GetActionAttackRange(this, action.Target, Globals.s_magicianRangedRange, Globals.s_magicianRangedDamage, SpellType.Fireball));
                                    }
                                }
                                else
                                {
                                    // Not an attack. 
                                    // Move towards it anyway. 
                                    // clever stuff again to decide what do 
                                    // e.g. - goto castle to heal, goto manaball to convert

                                    QueueAction(Globals.ActionPool.GetActionTravel(this, action.Target, null, Globals.s_magicianTravelSpeed, 0f, Globals.s_magicianMaxFollowRange));
                                    if (action.Target.GameObjectType == GameObjectType.manaball)
                                    {
                                        QueueAction(Globals.ActionPool.GetActionCastSpell(this,action.Target,null,SpellType.Convert));
                                    }
                                }
                            }

                        }

                        break;
                    }
                case (ActionState.Idle):
                    {
                        // if we don't have a castle then travel to a location and create one.
                        // need to make sure we don't enqueue this multiple times.
                        if (m_castles.Count == 0)
                        {
                            Vector3 castlePosition = FindCastleLocation();
                            QueueAction(Globals.ActionPool.GetActionTravel(this, null, castlePosition, Globals.s_magicianTravelSpeed));
                            QueueAction(Globals.ActionPool.GetActionCastSpell(this, null, castlePosition, SpellType.Castle));
                        }
                        else
                        {
                            // find something useful to do, loko for nearby enemy magicians, then monsters or manaballs, 
                            FindData findData = new FindData();
                            findData.m_owner = this;
                            findData.m_findMask = GameObjectType.magician | GameObjectType.castle | GameObjectType.balloon | GameObjectType.monster | GameObjectType.manaball;
                            findData.m_findRadius = Globals.s_magicianSearchRadiusManaball;
                            findData.m_magicianWeight = 1.0f;
                            findData.m_monsterWeight = 0.8f;
                            findData.m_manaballWeight = 0.8f;
                            findData.m_balloonWeight = 0.6f;
                            findData.m_castleWeight = 0.4f;
                            findData.m_includeOwner = false;

                            QueueAction(Globals.ActionPool.GetActionFind(findData));

                        }
                        break;
                    }
                case (ActionState.AttackingMelee):
                    {
                        if (action.Target.Alive && GameUtil.InRange(this, action.Target, Globals.s_monsterMeleeRange))
                        {
                            QueueAction(Globals.ActionPool.GetActionAttackMelee(this, action.Target, Globals.s_monsterMeleeRange, Globals.s_monsterMeleeDamage));
                        }
                        break;
                    }
                case (ActionState.AttackingRange):
                    {
                        // if we've finished attacking at range. then we need to see if 
                        // our target is dead or if we should do something else.

                        if (action.Target.Alive && GameUtil.InRange(this, action.Target, Globals.s_monsterRangedDamage))
                        {
                            QueueAction(Globals.ActionPool.GetActionAttackRange(this, action.Target, Globals.s_monsterRangedRange, Globals.s_monsterRangedDamage, SpellType.Fireball));
                        }
                        break;
                    }

                case (ActionState.Travelling):
                    {
                        TargetSpeed = 0f;
                        break;
                    }
                case (ActionState.Dieing):
                    {
                        // when we've finished dieing then we want to spawn a manaball here.
                        Globals.GameObjectManager.CreateAndInitialiseGameObject(GameObjectType.manaball, Position);
                        Cleanup();
                        break;
                    }

                default:
                    {
                        QueueAction(Globals.ActionPool.GetActionIdle(this));
                        break;
                    }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public Vector3 FindCastleLocation()
        {
            return Globals.Terrain.GetRandomWorldPositionXZ();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Damaged(DamageData damageData)
        {
            base.Damaged(damageData);

            // if something attacks us then we need to hit back?

            float currentHealthPercentage = GetAttributePercentage(GameObjectAttributeType.Health);

            if (currentHealthPercentage > Globals.s_monsterFleeHealthPercentage)
            {
                ActionState currentActionState = CurrentActionState;
                // if we're in a passive state then maybe attack back?
                if (BaseAction.IsPassive(currentActionState))
                {
                    m_actionComponent.ClearAllActions();

                    if (damageData.m_damager.Alive)
                    {
                        if (GameUtil.InRange(this, damageData.m_damager, Globals.s_monsterMeleeRange))
                        {
                            QueueAction(Globals.ActionPool.GetActionAttackMelee(this, damageData.m_damager, Globals.s_monsterMeleeRange, Globals.s_monsterMeleeDamage));
                        }
                        else if (GameUtil.InRange(this, damageData.m_damager, Globals.s_monsterRangedDamage))
                        {
                            QueueAction(Globals.ActionPool.GetActionAttackRange(this, damageData.m_damager, Globals.s_monsterRangedRange, Globals.s_monsterRangedDamage, SpellType.Fireball));
                        }
                    }
                }
            }
            else
            {
                // if we're being attacked and damaged then run away if we're below 1/4 health.
                // FIXME - shouldn't really clear action if we're dead / dieing?
                ClearAllActions();
                QueueAction(Globals.ActionPool.GetActionFlee(this, m_threatComponent.GetFleeDirection(), Globals.s_magicianFleeSpeed));
            }
        }



        public override String DebugText
        {
            get
            {
                return String.Format("Magician Id [{0}] Pos[{1}] Health[{2}] Mana[{3}] Spell1[{4}] Spell2[{5}.", Id, Position, GetAttribute(GameObjectAttributeType.Health).CurrentValue, GetAttribute(GameObjectAttributeType.Mana).CurrentValue, SelectedSpell1, SelectedSpell2);
            }
        }

        public override void NotifyOwnershipGained(GameObject gameObject)   
        {
            base.NotifyOwnershipGained(gameObject);
            Castle tryCastle = gameObject as Castle;
            if (tryCastle != null)
            {
                m_castles.Add(tryCastle);
            }

            Balloon tryBalloon = gameObject as Balloon;
            if (tryBalloon != null)
            {
                m_balloons.Add(tryBalloon);
            }
        }

        public override void NotifyOwnershipLost(GameObject gameObject)
        {
            base.NotifyOwnershipLost(gameObject);
            Castle tryCastle = gameObject as Castle;
            if (tryCastle != null)
            {
                m_castles.Remove(tryCastle);
            }

            Balloon tryBalloon = gameObject as Balloon;
            if (tryBalloon != null)
            {
                m_balloons.Remove(tryBalloon);
            }

        }

        public override Vector3 Position
        {
            set
            {
                // make sure we don't crash into the ground.
                Vector3 oldVal = WorldTransform.Translation;
                Vector3 result = CheckForGroundCollision(oldVal, value);

                base.Position = result;
            }
        }



        protected void BuildRayCallback()
        {
            IndexedVector3 iv3start = IndexedVector3.Zero;
            IndexedVector3 iv3end = IndexedVector3.Zero;
            m_groundCallback = new ClosestRayResultCallback(ref iv3start, ref iv3end);
            m_groundCallback.m_collisionFilterMask = (CollisionFilterGroups)(GameObjectType.terrain);
            m_groundCallback.m_collisionFilterGroup = (CollisionFilterGroups)(GameObjectType.magician);

        }

        protected Vector3 CheckForGroundCollision(Vector3 startPos, Vector3 endPos)
        {
            //startPos = new Vector3(0, 10, 0);
            //endPos = new Vector3(0, -10, 0);
            if (Globals.TerrainCollisionEnabled)
            {
                m_groundCallback.Initialize(startPos, endPos);
                Globals.CollisionManager.CastRay(startPos, endPos, m_groundCallback);
                if (m_groundCallback.HasHit())
                {
                    Vector3 normalPushBack = m_groundCallback.m_hitNormalWorld * 1f;
                    endPos = m_groundCallback.m_hitPointWorld + normalPushBack;

                }
            }
            return endPos;
        }


        private Vector4 m_carpetDimensions;

        private VertexBuffer m_carpetVertexBuffer;
        private Texture2D m_carpetTexture;
        private Effect m_carpetEffect;
        private float m_carpetMovementOffset;

        private ClosestRayResultCallback m_groundCallback;

        private List<Castle> m_castles = new List<Castle>();
        private List<Balloon> m_balloons = new List<Balloon>();


        private SpellType m_selectedSpell1 = SpellType.Convert;
        private SpellType m_selectedSpell2 = SpellType.Raise;


    }
}
