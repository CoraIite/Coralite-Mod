using Coralite.Core;
using Coralite.Core.Loaders;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.AlchorthentSeries
{
    public abstract class BaseAlchorthentMinion<TBuff> : ModProjectile where TBuff : ModBuff
    {
        public override string Texture => AssetDirectory.AlchorthentSeriesItems + Name;

        /// <summary>
        /// 状态
        /// </summary>
        public byte State { get; set; }
        public int Timer { get; set; }
        /// <summary>
        /// 目标索引
        /// </summary>
        public int Target { get; set; }

        public bool CanDamageNPC { get; set; }

        private bool init = true;

        public Player Owner => Main.player[Projectile.owner];

        public sealed override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            SetOtherDefault();
        }

        /// <summary>
        /// 已设置伤害类型，友好，启用本地无敌帧
        /// </summary>
        public virtual void SetOtherDefault()
        {

        }

        public override bool? CanDamage()
        {
            if (CanDamageNPC)
                return true;

            return false;
        }

        public override void AI()
        {
            if (init)
            {
                init = false;
                Target = -1;
                Initialize();
            }

            if (!CheckActive())
                return;

            Owner.AddBuff(BuffType<TBuff>(), 2);

            AIMoves();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Initialize()
        {

        }

        /// <summary>
        /// 具体的AI动作
        /// </summary>
        public virtual void AIMoves()
        {

        }

        /// <summary>
        /// 重力
        /// </summary>
        /// <param name="maxYSpeed"></param>
        /// <param name="gravityAccel"></param>
        public void Gravity(float maxYSpeed, float gravityAccel)
        {
            Projectile.velocity.Y += gravityAccel;
            if (Projectile.velocity.Y > maxYSpeed)
                Projectile.velocity.Y = maxYSpeed;
        }

        /// <summary>
        /// 检测玩家存活和BUFF
        /// </summary>
        /// <returns></returns>
        public bool CheckActive()
        {
            if (Owner.dead || !Owner.active)
            {
                Owner.ClearBuff(BuffType<TBuff>());
                return false;
            }

            if (Owner.HasBuff(BuffType<TBuff>()))
                Projectile.timeLeft = 2;

            return true;
        }

        /// <summary>
        /// 获取回归点
        /// </summary>
        /// <param name="selfIndex"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public virtual Vector2 GetIdlePos(int selfIndex, int totalCount)
        {
            return Owner.Center;
        }

        /// <summary>
        /// 是否在地上，判断Y速度为0并且中心一条上有实心物块
        /// </summary>
        public bool OnGround
        {
            get
            {
                if (Projectile.velocity.Y != 0)
                    return false;

                int x = (int)(Projectile.Center.X) / 16;
                int y = (int)Projectile.Bottom.Y / 16;

                //x += (int)Projectile.velocity.X;
                for (int j = x; j < x + (Projectile.width / 16) + 1; j++)
                    for (int k = 0; k < 2; k++)
                    {
                        Tile t = Framing.GetTileSafely(j, y + k);
                        if (t.HasUnactuatedTile && (Main.tileSolid[t.TileType] || Main.tileSolidTop[t.TileType]))
                            return true;
                    }

                return false;
            }
        }

        /// <summary>
        /// 飞翔到目标点
        /// </summary>
        /// <param name="aimPos"></param>
        /// <param name="acc"></param>
        /// <param name="baseVel"></param>
        public void FlyToAimPos(Vector2 aimPos, float acc, float baseVel, float accmulOnTurn = 1.5f, float closeDistance = 60f)
        {
            float vel = baseVel;
            if (vel < Math.Abs(Owner.velocity.X) + Math.Abs(Owner.velocity.Y))
                vel = Math.Abs(Owner.velocity.X) + Math.Abs(Owner.velocity.Y);

            Vector2 toPlayer = aimPos - Projectile.Center;
            float lengthToPlayer = toPlayer.Length();

            if (!(lengthToPlayer < closeDistance))
            {
                toPlayer = toPlayer.SafeNormalize(Vector2.Zero);
                toPlayer *= vel;

                if (Projectile.velocity.X < toPlayer.X)
                {
                    Projectile.velocity.X += acc;
                    if (Projectile.velocity.X < 0f)
                        Projectile.velocity.X += acc * accmulOnTurn;
                }
                else if (Projectile.velocity.X > toPlayer.X)
                {
                    Projectile.velocity.X -= acc;
                    if (Projectile.velocity.X > 0f)
                        Projectile.velocity.X -= acc * accmulOnTurn;
                }

                if (Projectile.velocity.Y < toPlayer.Y)
                {
                    Projectile.velocity.Y += acc;
                    if (Projectile.velocity.Y < 0f)
                        Projectile.velocity.Y += acc * accmulOnTurn;
                }
                else if (Projectile.velocity.Y > toPlayer.Y)
                {
                    Projectile.velocity.Y -= acc;
                    if (Projectile.velocity.Y > 0f)
                        Projectile.velocity.Y -= acc * accmulOnTurn;
                }
            }
        }

        /// <summary>
        /// 是否能切换回落地状态，判断与目标点的距离，玩家在地上（没有y速度），弹幕y位置比目标位置高，弹幕没有碰撞物块
        /// </summary>
        /// <param name="closeLength"></param>
        /// <param name="aimPos"></param>
        /// <returns></returns>
        public bool CanSwitchToLand(float closeLength, Vector2 aimPos)
        {
            return Vector2.Distance(aimPos, Projectile.Center) < closeLength && Owner.velocity.Y == 0f && Projectile.Center.Y <= aimPos.Y && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height);
        }

        /// <summary>
        /// 向一个方向查找实心物块
        /// </summary>
        /// <param name="startP"></param>
        /// <param name="dir"></param>
        /// <param name="searchLength"></param>
        /// <returns></returns>
        public bool GroundSearch(Point startP, Point dir, int searchLength)
        {
            for (int i = 0; i < searchLength; i++)
            {
                Tile t = Framing.GetTileSafely(startP + new Point(dir.X * i, dir.Y * i));
                if (t.HasUnactuatedTile && (Main.tileSolid[t.TileType] || Main.tileSolidTop[t.TileType]))
                {
                    return true;
                }
            }

            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public void DrawLine(Action<Effect> doDraw, Texture2D uFlowTex, float uTime, float flowAdd, float lineO, Color lineC, float powC, float lineEx)
        {
            Effect shader = ShaderLoader.GetShader("LineAdditive");

            shader.Parameters["uFlowTex"]?.SetValue(uFlowTex);
            shader.Parameters["uTime"]?.SetValue(uTime);
            shader.Parameters["flowAdd"]?.SetValue(flowAdd);
            shader.Parameters["lineO"]?.SetValue(lineO);
            shader.Parameters["lineC"]?.SetValue(lineC.ToVector4());
            shader.Parameters["powC"]?.SetValue(powC);
            shader.Parameters["lineEx"]?.SetValue(lineEx);
            shader.Parameters["transformMatrix"]?.SetValue(VaultUtils.GetTransfromMatrix());

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend
                , SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, shader, Main.GameViewMatrix.TransformationMatrix);

            doDraw(shader);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        }
    }
}
