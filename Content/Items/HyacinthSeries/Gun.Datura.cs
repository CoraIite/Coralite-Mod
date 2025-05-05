using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.HyacinthSeries
{
    public class Datura : ModItem
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(24, 2);
            Item.DefaultToRangedWeapon(ProjectileType<DaturaProj>(), AmmoID.Bullet, 21, 11f, true);

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.value = Item.sellPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            var st = CoraliteSoundID.Shotgun2_Item38;
            st.Pitch = 0.9f;
            Item.UseSound =st;

            Item.useTurn = false;
            Item.noUseGraphic = true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position += new Vector2(0, -4);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, velocity, ProjectileType<DaturaHeldProj>(), 0, knockback, player.whoAmI);

            //生成两大一小弹幕
            damage = (int)(damage * 0.57f);
            for (int i = 0; i < 2; i++)
            {
                Projectile.NewProjectile(source, position, velocity.RotateByRandom(-0.06f, 0.06f) * Main.rand.NextFloat(0.9f, 1.1f)
                    , ProjectileType<DaturaProj>(), damage, knockback, player.whoAmI);
                Projectile.NewProjectile(source, position, velocity.RotateByRandom(-0.06f, 0.06f) * Main.rand.NextFloat(0.9f, 1.1f)
                    , ProjectileType<DaturaProj>(), damage, knockback, player.whoAmI, 1);
            }

            Projectile.NewProjectile(source, position, velocity.RotateByRandom(-0.08f, 0.08f), type, damage, knockback, player.whoAmI, 1);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Boomstick)
                .AddIngredient(ItemID.SoulofNight, 5)
                .AddIngredient(ItemID.Ichor, 15)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    [AutoLoadTexture(Path = AssetDirectory.HyacinthSeriesItems)]
    public class DaturaHeldProj : BaseGunHeldProj
    {
        public DaturaHeldProj() : base(0.1f, 24, -6, AssetDirectory.HyacinthSeriesItems) { }

        public static ATex DaturaFire { get; private set; }

        protected override float HeldPositionY => -2;

        private int FrameX;

        public override void InitializeGun()
        {
            FrameX = Main.rand.Next(4);
        }

        public override void ModifyAI(float factor)
        {
            Lighting.AddLight(Projectile.Center, Color.Gold.ToVector3() / 2);
            if (Projectile.timeLeft != MaxTime && Projectile.timeLeft % 2 == 0)
            {
                Projectile.frame++;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            base.PreDraw(ref lightColor);

            if (Projectile.frame > 6)
                return false;

            Texture2D effect = DaturaFire.Value;
            Rectangle frameBox = effect.Frame(4, 7, FrameX, Projectile.frame);
            //SpriteEffects effects = DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            float rot = Projectile.rotation + (DirSign > 0 ? 0 : MathHelper.Pi);
            float n = rot - DirSign * MathHelper.PiOver2;

            Main.spriteBatch.Draw(effect, Projectile.Center + rot.ToRotationVector2() * 20 + n.ToRotationVector2() * 4 - Main.screenPosition, frameBox, Color.Lerp(lightColor, Color.White, 0.5f)
                , rot, new Vector2(0, frameBox.Height / 2), Projectile.scale * 0.8f, 0, 0f);
            return false;
        }
    }

    /// <summary>
    /// 使用ai0判断弹幕类型，0大弹幕，1小弹幕
    /// </summary>
    public class DaturaProj : ModProjectile
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;

        public ref float ProjType => ref Projectile.ai[0];
        public ref float State => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[2];
        public ref float Alpha => ref Projectile.localAI[0];

        private float scaleMult = 1;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 15;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (Projectile.localAI[1] == 0)
            {
                Projectile.localAI[1] = 1;
                Projectile.localAI[2] = Main.rand.Next(2);
            }

            switch (State)
            {
                default:
                case 0://生成
                    {
                        Alpha += 0.15f;
                        if (Alpha > 1)
                            Alpha = 1;

                        if (++Timer > 8)
                        {
                            Timer = 0;
                            State = 1;
                        }

                        Projectile.velocity *= 1.02f;
                        Projectile.SpawnTrailDust(DustID.Ichor, -Main.rand.NextFloat(0.2f, 0.4f)
                            , Scale: Main.rand.NextFloat(0.6f, 1f));
                        Projectile.rotation = Projectile.velocity.ToRotation();
                    }
                    break;
                case 1:
                    {
                        Timer++;

                        int maxTime = 45;
                        if (ProjType == 1)
                            maxTime = 25;

                        if (Timer < maxTime)
                        {
                            Projectile.rotation = Projectile.velocity.ToRotation();

                            Projectile.SpawnTrailDust(DustID.Ichor, -Main.rand.NextFloat(0.2f, 0.4f)
                                , Scale: Main.rand.NextFloat(0.6f, 1f));

                            return;
                        }

                        if (Timer == maxTime)
                        {
                            int size = 90;
                            if (ProjType == 1)
                                size = 60;

                            Projectile.Resize(size, size);
                            Helper.PlayPitched(CoraliteSoundID.FireBallExplosion_DD2_BetsyFireballImpact, Projectile.Center, pitch: -0.8f);
                            return;
                        }

                        int currTime = (int)Timer - maxTime;

                        if (currTime < 20)
                            scaleMult += 0.01f;

                        Projectile.velocity *= 0.86f;

                        if (currTime == 3 * 4)
                            Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                        else if (currTime > 3 * 4)
                            Projectile.rotation += Projectile.velocity.Length() / 25;
 
                        if (currTime > 3 * 3 && currTime < 3 * 9 && Main.rand.NextBool(3))
                            PRTLoader.NewParticle<TwistFog>(Projectile.Center + Helper.NextVec2Dir(Projectile.width * 0.1f, Projectile.width * 0.3f)
                                , Helper.NextVec2Dir(0.2f, 0.4f)
                                , Main.rand.NextFromList(Color.DarkGoldenrod, Color.Gold) with { A = 100 }, Main.rand.NextFloat(0.3f, 0.8f));

                        //for (int i = 0; i < 2; i++)
                        if (Main.rand.NextBool(2))
                        {
                            Vector2 dir = Helper.NextVec2Dir(Projectile.width * 0.3f, Projectile.width * 0.6f);
                            Vector2 pos = Projectile.Center + dir;
                            Vector2 velocity = dir.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(2, 4);

                            Dust d = Dust.NewDustPerfect(pos, DustID.IchorTorch, velocity, Scale: Main.rand.NextFloat(1, 1.5f));
                            d.noGravity = true;
                        }

                        if (currTime % 3 == 0)
                        {
                            Projectile.frame++;
                        }

                        if (currTime > 3 * 12)
                            Projectile.Kill();
                    }
                    break;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Fade();
            Projectile.tileCollide = false;
            Projectile.velocity *= 0;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Ichor, 60 * 10);//灵液debuff
            Fade();

            //Projectile.damage = (int)(Projectile.damage * 0.95f);
            Projectile.velocity *= 0.9f;
        }

        public void Fade()
        {
            State = 1;
            Alpha = 1;
            int maxTime = 45;
            int size = 60;

            if (ProjType == 1)
            {
                size = 36;
                maxTime = 30;
            }

            if (Timer < maxTime)
            {
                Helper.PlayPitched(CoraliteSoundID.FireBallExplosion_DD2_BetsyFireballImpact, Projectile.Center,pitch:-0.8f);
                Timer = maxTime;
            }
            if (Projectile.width != size)
                Projectile.Resize(size, size);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.frame > 11)
                return false;

            var frame = Projectile.GetTexture()
                .Frame(4, 12, (int)(ProjType * 2) + (int)Projectile.localAI[2], Projectile.frame);

            Projectile.QuickDraw(frame, lightColor, 0, scaleMult);

            return false;
        }
    }
}
