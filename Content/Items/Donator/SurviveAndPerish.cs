using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.Donator
{
    public class SurviveAndPerish : ModItem
    {
        public override string Texture => AssetDirectory.Donator + Name;

        public override void SetDefaults()
        {
            Item.damage = 19;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.knockBack = 6;
            Item.shootSpeed = 14;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Ranged;
            Item.value = Item.sellPrice(0, 2);
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<SurviveHeldProj>();
            Item.useAmmo = ItemID.Dynamite;

            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
        }

        public override void HoldItem(Player player)
        {
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                //检测弹幕状态
                Projectile p = Main.projectile.FirstOrDefault(proj => proj.active && proj.friendly
                    && proj.owner == player.whoAmI && proj.type == ModContent.ProjectileType<PerishProj>());

                if (p!=null)
                {
                    if (p.ai[0] == 0)
                    {
                        (p.ModProjectile as PerishProj).StartAttack();
                        return true;
                    }
                    else
                        return false;
                }
            }

            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer != player.whoAmI)
                return false;

            if (player.altFunctionUse == 2)
            {
                return false;
            }

            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center
                , Vector2.Zero, ModContent.ProjectileType<SurviveHeldProj>(), 0, knockback, player.whoAmI);

            SoundStyle st = CoraliteSoundID.Shotgun2_Item38;
            st.Pitch = -0.4f;
            st.Volume -= 0.4f;
            SoundEngine.PlaySound(st, player.Center);
            st = CoraliteSoundID.Gun3_Item41;
            st.Pitch = -0.8f;
            SoundEngine.PlaySound(st, player.Center);
            return false;
        }
    }

    public class SurviveHeldProj : BaseGunHeldProj
    {
        public override string Texture => AssetDirectory.Donator + "SurviveAndPerishHeldProj";

        public SurviveHeldProj() : base(0.15f, 18, -8, AssetDirectory.Donator) { }

        public override void Initialize()
        {
            int time = Owner.itemTimeMax;
            if (time < 6)
                time = 6;

            Projectile.timeLeft = time;
            MaxTime = time;
            if (Main.myPlayer == Projectile.owner)
            {
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                TargetRot = (Main.MouseWorld - Owner.Center).ToRotation() + (OwnerDirection > 0 ? 0f : MathHelper.Pi);
            }

            Projectile.netUpdate = true;
            HeldPositionY = 10;
        }

        public override float Ease()
        {
            float x = 1.465f * Projectile.timeLeft / MaxTime;
            return x * MathF.Sin(x * x * x) / 1.186f;
        }

        public override void ModifyAI(float factor)
        {
            Projectile.frame = (int)((1 - Projectile.timeLeft / MaxTime) * 6);
        }

        public override void GetFrame(Texture2D mainTex, out Rectangle? frame, out Vector2 origin)
        {
            frame = mainTex.Frame(1, 6, 0, Projectile.frame);
            origin = frame.Value.Size() / 2;
            origin.X -= OwnerDirection * frame.Value.Width / 6;
        }
    }

    public class PerishProj:BaseHeldProj
    {
        public override string Texture => AssetDirectory.Donator+Name;

        public int frameX;

        public ref float Timer => ref Projectile.ai[0]; 
        public ref float State => ref Projectile.ai[1]; 

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.timeLeft = 200;
        }

        public override void AI()
        {
            if (Owner.HeldItem.type != ModContent.ItemType<SurviveAndPerish>())
                return;

            Projectile.timeLeft = 2;

            switch (State)
            {
                default:
                case 0://可以发射导弹
                    break;
                case 1://发射期间
                    {

                    }
                    break;
                case 2://发射完毕，进入休整期的动画
                    {

                    }
                    break;
                case 3://休整期
                    {

                    }
                    break;
                case 4://重新装填的动画
                    {

                    }
                    break;
            }

            Projectile.Center = Owner.Center + new Vector2(-OwnerDirection * 12, -12);

            Projectile.rotation = Math.Clamp((Main.MouseWorld.Y-Projectile.Center.Y)/150,-0.4f,0.4f);
        }

        public void ShootMissile()
        {

        }

        public void StartAttack()
        {
            if (State != 0)
                return;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();

            var frame = mainTex.Frame(2, 11, frameX, Projectile.frame);
            var origin = new Vector2(frame.Width / 2 + OwnerDirection * frame.Width / 3, frame.Height * 3 / 4);
            var pos = Projectile.Center - Main.screenPosition;
            SpriteEffects eff = OwnerDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Main.spriteBatch.Draw(mainTex, pos, frame, lightColor,Projectile.rotation*OwnerDirection,origin,Projectile.scale,eff,0);

            return false;
        }
    }

    public class PerishMissile : ModProjectile,IDrawPrimitive,IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.Donator+Name;

        public ref float Target => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];

        private static BasicEffect effect;
        public Trail trail;

        public PerishMissile()
        {
            Main.QueueMainThreadAction(() =>
            {
                effect = new BasicEffect(Main.instance.GraphicsDevice);
                effect.VertexColorEnabled = true;
            });
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                trail = new Trail(Main.instance.GraphicsDevice, 14, new NoTip()
                    , factor => 2, factor => Color.Lerp(new Color(0, 0, 0, 0), Color.Orange, factor.X));
                Projectile.InitOldPosCache(14);
                FindTarget();
            }

            Chase();

            Projectile.UpdateOldPosCache(addVelocity: true);
            trail.Positions = Projectile.oldPos;
        }

        public void FindTarget()
        {
            int type = ModContent.ProjectileType<MiniDynamite>();

            Dictionary<int, int> targetRecord = new Dictionary<int, int>();

            bool hasTarget = false;

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (!p.active || !p.friendly || p.owner != Projectile.owner || p.type != type)
                    continue;

                hasTarget = true;

                if (targetRecord.TryGetValue((int)p.ai[0], out _))
                {
                    targetRecord[(int)p.ai[0]]++;
                }
                else
                    targetRecord.Add((int)p.ai[0], 1);
            }

            if (!hasTarget)
            {
                if (Helper.TryFindClosestEnemy(Projectile.Center, 800, n => n.CanBeChasedBy() && Collision.CanHit(Projectile, n), out NPC tar))
                    Target = tar.whoAmI;

                return;
            }
            var keyForMax = targetRecord.Keys.Aggregate((i, j) => targetRecord[i] >= targetRecord[j] ? i : j);
            Target = targetRecord[keyForMax];
        }

        public void Chase()
        {
            if (!Target.GetNPCOwner(out NPC target, () => Target = -1))
                return;

            float slowTime = Timer < 30 ? Coralite.Instance.X2Smoother.Smoother(Timer / 30) : 1;
            slowTime = Helper.Lerp(130, 38, slowTime);

            float num481 = 18f;
            Vector2 center = Projectile.Center;
            Vector2 dir = target.Center - center;
            float length = dir.Length();
            if (length < 100f)
                num481 = 13f;

            length = num481 / length;
            dir *= length;
            Projectile.velocity.X = (Projectile.velocity.X * slowTime + dir.X) / (slowTime + 1);
            Projectile.velocity.Y = (Projectile.velocity.Y * slowTime + dir.Y) / (slowTime + 1);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile p = Main.projectile.FirstOrDefault(p => p.active && p.friendly 
                && p.owner == Projectile.owner && p.type == ModContent.ProjectileType<MiniDynamite>());
            if (p != null)
            {
                (p.ModProjectile as MiniDynamite).BigBoom();
            }
        }

        public override void OnKill(int timeLeft)
        {
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawPrimitives()
        {
            if (trail == null)
                return;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.World = world;
            effect.View = view;
            effect.Projection = projection;

            trail?.Render(effect);
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            Projectile.QuickDraw(Projectile.GetTexture().Frame(1, 3, 0, Projectile.frame), Color.White, 1.57f);
        }
    }

    /// <summary>
    /// 使用ai0记录命中的目标
    /// </summary>
    public class MiniDynamite : ModProjectile
    {
        public override string Texture => AssetDirectory.Donator+Name;

        public ref float Target => ref Projectile.ai[0];

        public void BigBoom()
        {

        }
    }
}
