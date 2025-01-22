using Coralite.Content.DamageClasses;
using Coralite.Content.ModPlayers.DigDigDig;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Core.Systems.DigSystem
{
    /// <summary>
    /// 投镐基类，可直接使用也可继承使用<br></br>
    /// ai0传入物品贴图，ai1传入弹幕尺寸
    /// </summary>
    public class ThrownPickaxe : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float TexType => ref Projectile.ai[0];
        public ref float Width => ref Projectile.ai[1];

        public int State { get; set; }
        public int Timer { get; set; }

        public enum ThrowState
        {
            /// <summary>
            /// 飞行中
            /// </summary>
            Flying,
            /// <summary>
            /// 坠落
            /// </summary>
            Falling
        }

        #region 各种属性

        /// <summary> 飞行时间，过了飞行时间之后就会下坠 </summary>
        public int FlyTime { get; set; } = 30;

        /// <summary> 飞行时的重力 </summary>
        public float FlyingGravity { get; set; } = 0.15f;

        /// <summary> 弹回次数，这个值仅在<see cref="Initialize"/>的时候设置 </summary>
        public int JumpCount { get; private set; }
        /// <summary> 穿透次数，这个值仅在<see cref="Initialize"/>的时候设置 </summary>
        public int PenetrateCount { get; private set; }

        /// <summary> 是否能够挖掘物块 </summary>
        public bool CanDigTile { get; set; }
        /// <summary> 挖东西的范围 </summary>
        public int PickRange { get; private set; }

        /// <summary> 发光蒙版 </summary>
        public int GlowMask { get; set; } = -1;

        private bool init = true;

        /// <summary>
        /// 仅用作本地端获取颜色
        /// </summary>
        public Item item;

        #endregion

        public Player Owner => Main.player[Projectile.owner];

        public override void SetDefaults()
        {
            Projectile.DamageType = CreatePickaxeDamage.Instance;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        #region AI

        public override void AI()
        {
            if (init)
            {
                Projectile.Resize((int)Width, (int)Width);
                Initialize();

                UpdateShieldAccessory(accessory => accessory.OnInitialize(this));
                Projectile.rotation = Projectile.velocity.ToRotation();

                init = false;
            }

            switch (State)
            {
                default:
                    SpecialAI();
                    break;
                case (int)ThrowState.Flying:
                    FlyingAI();
                    break;
                case (int)ThrowState.Falling:
                    FallingAI();
                    break;
            }

            SetRotation();
        }

        /// <summary>
        /// 初始化各种值
        /// </summary>
        public virtual void Initialize()
        {

        }

        /// <summary>
        /// 可以在这里写各种特殊AI
        /// </summary>
        public virtual void SpecialAI() { }

        /// <summary>
        /// 飞行时的AI
        /// </summary>
        public virtual void FlyingAI()
        {
            if (Timer > FlyTime)
                TurnToFalling();

            if (Projectile.velocity.Y < 18)
                Projectile.velocity.Y += FlyingGravity;

            Timer++;
        }

        /// <summary>
        /// 下坠时的AI
        /// </summary>
        public virtual void FallingAI()
        {
            if (Projectile.velocity.Y < 18)
                Projectile.velocity.Y += FlyingGravity * 1.5f;

            Projectile.velocity.X *= 0.9f;
        }

        public virtual void SetRotation()
        {
            Projectile.rotation += Math.Sign(Projectile.velocity.X) * Projectile.velocity.Length() / 35;
        }

        #endregion

        #region 状态切换

        /// <summary>
        /// 切换至下落状态，包括操作<br></br>
        /// 重置 <see cref="Timer"/>，设置 <see cref="Projectile.timeLeft"/> 为10秒<br></br>
        /// 将弹幕伤害变为10%
        /// </summary>
        public virtual void TurnToFalling()
        {
            State = (int)ThrowState.Falling;
            Timer = 0;
            Projectile.timeLeft = Projectile.MaxUpdates * 60 * 10;

            Projectile.velocity.X *= 0.5f;
            Projectile.damage = (int)(Projectile.damage * 0.1f);
            if (Projectile.damage < 1)
                Projectile.damage = 1;

            Projectile.tileCollide = true;
        }

        #endregion

        #region 攻击判定

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            UpdateShieldAccessory(accessory => accessory.OnHitNPC(this, target, hit, damageDone));

            if (PenetrateCount > 0)//穿透
            {
                OnPenetrate();
                PenetrateCount--;
                return;
            }

            if (JumpCount > 0)//弹跳
            {
                OnJump();
                JumpCount--;
                return;
            }

            Projectile.Kill();
        }

        /// <summary>
        /// 在穿透时调用
        /// </summary>
        public virtual void OnPenetrate()
        {

        }

        /// <summary>
        /// 获取弹跳后的速度
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public virtual Vector2 JumpVelocity(NPC target)
        {
            float length = Projectile.velocity.Length();
            Vector2 dir = target.Center - Projectile.Center;
            dir.Normalize();
            dir *= length;
            Projectile.netUpdate = true;
            return -dir * 0.9f;
        }

        /// <summary>
        /// 在弹跳时调用
        /// </summary>
        public virtual void OnJump()
        {

        }

        #endregion

        #region 帮助方法

        public void UpdateShieldAccessory(Action<ICreatePickaxeAccessory> action)
        {
            if (Owner.TryGetModPlayer(out DigDigDigPlayer dddp))
                foreach (var accessory in dddp.ThrownPickaxeAccessories)
                    action(accessory);
        }

        public virtual void DigTile(Vector2 oldVelocity)
        {
            bool dig = false;
            Vector2 dir = (Projectile.rotation - Projectile.direction * 0.785f).ToRotationVector2() * Projectile.width * 0.707f;//二分之根号二
        recheck:

            Vector2 pos = Projectile.Center + dir;
            //Dust d = Dust.NewDustPerfect(pos, DustID.AncientLight, Vector2.Zero, Scale: 2);
            //d.noGravity = true;

            Vector2 sign = new Vector2(Math.Sign(dir.X), Math.Sign(dir.Y));

            pos -= new Vector2(sign.X * Projectile.width / 2, sign.Y * Projectile.height / 2);

            int xCount = 0;
            int yCount = 0;

            int wCount = Projectile.width / 16 + 2;
            int hCount = Projectile.height / 16 + 2;

            for (int i = 0; i < wCount; i++)
            {
                for (int j = 0; j < hCount; j++)
                {
                    Point p = (pos + new Vector2(sign.X * xCount, sign.Y * yCount)).ToTileCoordinates();
                    Tile t = Framing.GetTileSafely(p);
                    //Dust d2 = Dust.NewDustPerfect(p.ToWorldCoordinates(), DustID.AncientLight, Vector2.Zero, newColor:Color.Coral,Scale: 2);
                    //d2.noGravity = true;

                    if (t.HasTile)
                    {
                        Owner.PickTile(p.X, p.Y, ContentSamples.ItemsByType[(int)TexType].pick);
                        dig = true;
                        PickRange--;
                        if (PickRange < 0)
                            return;
                    }

                    yCount += 16;
                    if (yCount > Projectile.height)
                        yCount = Projectile.height;
                }

                xCount += 16;
                if (xCount > Projectile.height)
                    xCount = Projectile.height;

                yCount = 0;
            }

            if (!dig)
            {
                Projectile.Resize(Projectile.width + 16, Projectile.height + 16);
                goto recheck;
            }
        }

        #endregion

        #region 其他原版方法

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            if (Projectile.width > 20)
                width = 20;
            if (Projectile.height > 20)
                height = 20;

            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.position += oldVelocity;
            if (CanDigTile)
                DigTile(oldVelocity);

            return true;
        }

        #endregion

        #region 同步

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(CanDigTile);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            CanDigTile = reader.ReadBoolean();
        }

        #endregion

        #region 绘制

        public override bool PreDraw(ref Color lightColor)
        {
            DrawPickaxe(lightColor);
            return false;
        }

        public virtual void DrawPickaxe(Color lightColor)
        {
            int itemType = (int)TexType;
            Main.instance.LoadItem(itemType);

            Texture2D mainTex = TextureAssets.Item[itemType].Value;
            Rectangle frameBox;

            if (Main.itemAnimations[itemType] != null)
                frameBox = Main.itemAnimations[itemType].GetFrame(mainTex, -1);
            else
                frameBox = mainTex.Frame();

            if (item!=null)
                lightColor = item.GetAlpha(lightColor);

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, frameBox, lightColor
                , Projectile.rotation, frameBox.Size() / 2, Projectile.scale, Projectile.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);

            if (GlowMask > 0)
            {
                Texture2D glowTex = TextureAssets.GlowMask[GlowMask].Value;
                if (Main.itemAnimations[itemType] != null)
                    frameBox = Main.itemAnimations[itemType].GetFrame(glowTex, -1);
                else
                    frameBox = mainTex.Frame();

                Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, frameBox, Color.White
                    , Projectile.rotation, frameBox.Size() / 2, Projectile.scale, Projectile.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);
            }
        }

        #endregion
    }
}
