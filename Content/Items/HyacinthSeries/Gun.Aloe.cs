using Coralite.Content.Items.ThyphionSeries;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Prefabs.Particles;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.Trails;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.HyacinthSeries
{
    public class Aloe : ModItem
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(26, 2);
            Item.DefaultToRangedWeapon(ProjectileType<DaturaProj>(), AmmoID.Bullet, 30, 11f, true);

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Pink;

            Item.useTurn = false;
            Item.noUseGraphic = true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            //position += new Vector2(0, -4);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, velocity, ProjectileType<AloeHeldProj>(), 0, knockback, player.whoAmI);
            Helper.PlayPitched(CoraliteSoundID.Gun3_Item41, player.Center, volumeAdjust: -0.5f);

            return true;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.NextBool(2, 5);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Gatligator)
                .AddIngredient(ItemID.GelBalloon, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    [AutoLoadTexture(Path = AssetDirectory.HyacinthSeriesItems)]
    public class AloeHeldProj : BaseGunHeldProj
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;

        public AloeHeldProj() : base(0.05f, 24, -6, AssetDirectory.HyacinthSeriesItems) { }

        public static ATex AloeFire { get; private set; }

        protected override float HeldPositionY => 0;

        protected ref float Timer => ref Projectile.ai[2];
        public int ShootCount;

        private int frame2;
        private int frameCounter2;

        private const int MaxShootCount = 8;

        public override void InitializeGun()
        {
            Projectile.timeLeft = Owner.itemTimeMax + 2;
            MaxTime = Owner.itemTimeMax;
            Timer = MaxTime;

            Owner.direction = InMousePos.X > Owner.Center.X ? 1 : -1;
            TargetRot = (InMousePos - Owner.Center).ToRotation() + (DirSign > 0 ? 0f : MathHelper.Pi);
            Projectile.netUpdate = true;
        }

        public override float Ease()
        {
            float x = 1.772f * Timer / MaxTime;
            return x * MathF.Sin(x * x) / 1.3076f;
        }

        public override void ModifyAI(float factor)
        {
            if (Owner.HeldItem.type != ItemType<Aloe>())
            {
                Projectile.Kill();
                return;
            }

            Lighting.AddLight(Projectile.Center, new Vector3(0.4f));
            Projectile.UpdateFrameNormally(2, 15);
            Owner.itemTime = Owner.itemAnimation = 2;
            UpdateGunFrame();

            Timer--;
            if (Timer != 0)
                return;

            Owner.direction = InMousePos.X > Owner.Center.X ? 1 : -1;
            TargetRot = (InMousePos - Owner.Center).ToRotation() + (DirSign > 0 ? 0f : MathHelper.Pi);

            if (Projectile.IsOwnedByLocalPlayer() && DownLeft)
            {
                //设置时间
                ShootCount++;
                int timeMax = Owner.itemTimeMax;
                timeMax = (int)Helper.Lerp(timeMax, 9, Math.Clamp(ShootCount / (float)MaxShootCount, 0, 1));

                Timer = timeMax;
                Projectile.timeLeft = timeMax + 2;

                float rot = Projectile.rotation + (DirSign > 0 ? 0 : MathHelper.Pi);
                Vector2 dir = rot.ToRotationVector2();

                //射弹幕
                if (Owner.PickAmmo(Owner.HeldItem, out int projType, out float speed, out int damage, out float kb, out _))
                {
                    Helper.PlayPitched(CoraliteSoundID.Gun3_Item41, Projectile.Center, volumeAdjust: -0.5f);
                    if (ShootCount >= MaxShootCount)//特殊追踪芦荟子弹
                    {
                        float angle = MathF.Sin(0.1f + ShootCount * 1.4f) * 0.25f;
                        Vector2 dir2 = UnitToMouseV.RotatedBy(angle);
                        Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(8, 8);

                        var p2 = PRTLoader.NewParticle<AloeParticle>(pos + dir2 * 60, Vector2.Zero);
                        p2.Rotation = dir2.ToRotation();
                        p2.FollowProjIndex = Projectile.whoAmI;

                        int? targetIndex = FindEnemy(dir2);
                        Projectile.NewProjectileFromThis<AloeChaseProj>(Owner.Center, dir2 * 14
                            , damage / 2, Projectile.knockBack, targetIndex ?? -1);
                    }

                    Projectile.NewProjectileFromThis(Owner.Center, UnitToMouseV * speed, projType, damage, kb);
                    //生成弹壳
                    Dust.NewDustPerfect(Projectile.Center - dir * 20, DustType<AloePetal>(), -dir.RotateByRandom(-0.2f, 0.2f) * Main.rand.NextFloat());
                }
                else
                    Projectile.Kill();

                //生成粒子
                var p1 = PRTLoader.NewParticle<AloeBigParticle>(Projectile.Center + dir * 60 + Helper.NextVec2Dir(2, 6)
                    , Vector2.Zero);
                p1.Rotation = ToMouseA;
                p1.FollowProjIndex = Projectile.whoAmI;
            }
            else
                Projectile.Kill();
        }

        public int? FindEnemy(Vector2 dir2)
        {
            int index = -1;
            Vector2 dir = dir2 * 1000;
            float a = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC n = Main.npc[i];

                if (!n.CanBeChasedBy())
                    continue;

                if (Collision.CanHit(Projectile, n) &&
                    Collision.CheckAABBvLineCollision(n.TopLeft, n.Size, Projectile.Center, Projectile.Center + dir, 400, ref a))
                {
                    index = i;
                    break;
                }
            }

            if (index < 0)
                return null;

            return index;
        }


        public void UpdateGunFrame()
        {
            if (++frameCounter2 > 3)
            {
                frameCounter2 = 0;

                if (ShootCount < MaxShootCount)
                {
                    if (++frame2 > 3)
                        frame2 = 0;
                }
                else
                {
                    if (++frame2 > 7)
                        frame2 = 4;
                }
            }
        }

        public override void GetFrame(Texture2D mainTex, out Rectangle? frame, out Vector2 origin)
        {
            frame = mainTex.Frame(1, 8, 0, frame2);
            origin = frame.Value.Size() / 2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            base.PreDraw(ref lightColor);

            if (ShootCount < MaxShootCount)
                return false;
            
            Texture2D effect = AloeFire.Value;
            Rectangle frameBox = effect.Frame(1, 16, 0, Projectile.frame);

            float rot = Projectile.rotation + (DirSign > 0 ? 0 : MathHelper.Pi);
            float n = rot - DirSign * MathHelper.PiOver2;

            Main.spriteBatch.Draw(effect, Projectile.Center + rot.ToRotationVector2() * 10 + n.ToRotationVector2() * 4 - Main.screenPosition, frameBox, Color.Lerp(lightColor, Color.White, 0.5f)
                , rot, new Vector2(0, frameBox.Height / 2), Projectile.scale, 0, 0f);
            return false;
        }
    }

    [AutoLoadTexture(Path = AssetDirectory.HyacinthSeriesItems)]
    public class AloeChaseProj : ModProjectile, IDrawPrimitive
    {
        public override string Texture => AssetDirectory.Blank;

        public static ATex AloeGradient { get; private set; }

        public ref float TargetIndex => ref Projectile.ai[0];
        public Trail trail;
        private bool init = true;

        public ref float State => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[2];
        public int trailCount = 8;
        public int trailWidth = 12;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.extraUpdates = 2;
            Projectile.friendly = true;
            Projectile.timeLeft = 60 * Projectile.MaxUpdates * 4;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.penetrate = 1;
        }

        public override void AI()
        {
            Initialize();
            Projectile.rotation = Projectile.velocity.ToRotation();

            Timer++;
            if (TargetIndex.GetNPCOwner(out NPC target, () => TargetIndex = -1))
            {
                float num481 = 20f;
                Vector2 center = Projectile.Center;
                Vector2 targetCenter = target.Center;
                Vector2 dir = targetCenter - center;
                float length = dir.Length();
                if (length < 100f)
                    num481 = 14f;

                length = num481 / length;
                dir *= length;
                Projectile.velocity.X = ((Projectile.velocity.X * 24f) + dir.X) / 25f;
                Projectile.velocity.Y = ((Projectile.velocity.Y * 24f) + dir.Y) / 25f;
            }

            UpdateOldPos();
            SpawnDust();
        }

        public void Initialize()
        {
            if (init)
            {
                init = false;

                if (!VaultUtils.isServer)
                {
                    Projectile.InitOldPosCache(trailCount);
                    trail = new Trail(Main.instance.GraphicsDevice, trailCount + 4, new EmptyMeshGenerator()
                        , f => trailWidth, f => new Color(255, 255, 255, 170));//=> Color.Lerp(Color.Transparent, Color.White,f.X));
                }
            }
        }

        private void UpdateOldPos()
        {
            if (!VaultUtils.isServer)
            {
                Projectile.UpdateOldPosCache();

                Vector2[] pos2 = new Vector2[trailCount + 4];

                //延长一下拖尾数组，因为使用的贴图比较特别
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                    pos2[i] = Projectile.oldPos[i] + Projectile.velocity;

                Vector2 dir = Projectile.rotation.ToRotationVector2();
                int exLength = 4;

                for (int i = 1; i < 5; i++)
                    pos2[trailCount + i - 1] = Projectile.oldPos[^1] + dir * i * exLength + Projectile.velocity;

                trail.TrailPositions = pos2;
            }
        }

        private void SpawnDust()
        {
            if (Main.rand.NextBool(2))
            {
                int width = 8;

                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(width, width), DustID.UnusedWhiteBluePurple
                    , Projectile.velocity * Main.rand.NextFloat(-0.4f, 0.8f), 125, Scale: Main.rand.NextFloat(1, 1.4f));
                d.noGravity = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            for (int i = 0; i < 6; i++)
            {
                PRTLoader.NewParticle<AloeGelBallParticle>(Projectile.Center, dir.RotateByRandom(-0.6f, 0.6f) * Main.rand.NextFloat(1, 2) * Main.rand.NextFromList(-1, 1)
                   , Scale: Main.rand.NextFloat(0.5f, 1f));
            }
        }

        public void DrawPrimitives()
        {
            if (trail == null)
                return;

            Effect effect = Filters.Scene["TurbulenceArrow"].GetShader().Shader;

            effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.08f);
            effect.Parameters["uTimeG"].SetValue(Main.GlobalTimeWrappedHourly * 0.2f);
            effect.Parameters["udissolveS"].SetValue(1f);
            effect.Parameters["uBaseImage"].SetValue(CoraliteAssets.Trail.LightShot.Value);
            effect.Parameters["uFlow"].SetValue(CoraliteAssets.Laser.Airflow.Value);
            effect.Parameters["uGradient"].SetValue(AloeGradient.Value);
            effect.Parameters["uDissolve"].SetValue(TurbulenceArrow.TurbulenceFlow.Value);

            Main.graphics.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            trail?.DrawTrail(effect);
            Main.graphics.GraphicsDevice.BlendState = BlendState.Additive;
            trail?.DrawTrail(effect);

            Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        }
    }

    public class AloeParticle() : BaseFrameParticle(4, 3, 2)
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;
    }
    public class AloeBigParticle() : BaseFrameParticle(4, 3, 2)
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;
    }
    public class AloeGelBallParticle() : BaseFrameParticle(4, 5, 3)
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;

        public override void AI()
        {
            base.AI();
            Lighting.AddLight(Position, new Vector3(0.4f));
        }
    }

    public class AloePetal : ModDust
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;

        public override void OnSpawn(Dust dust)
        {
            dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            dust.color = Color.White * 0.75f;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.rotation += 0.1f;
            dust.velocity *= 0.99f;
            dust.velocity.Y += 0.02f;

            if (dust.fadeIn > 30)
                dust.color *= 0.84f;

            if (!dust.noGravity && dust.velocity.Y < 5)
            {
                dust.velocity.Y += 0.05f;
            }

            dust.fadeIn++;
            if (dust.fadeIn > 45)
                dust.active = false;
            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Color c = Lighting.GetColor(dust.position.ToTileCoordinates());
            c *= dust.color.A / 255f;
            Texture2D.Value.QuickCenteredDraw(Main.spriteBatch, dust.position - Main.screenPosition, c, dust.rotation, scale: dust.scale);

            return false;
        }
    }
}
