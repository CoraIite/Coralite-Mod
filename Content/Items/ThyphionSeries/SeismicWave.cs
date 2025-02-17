using Coralite.Content.Dusts;
using Coralite.Content.Items.Steel;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.CameraSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.ThyphionSeries
{
    public class SeismicWave : ModItem, IDashable
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;

        public float Priority => IDashable.HeldItemDash;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(58, 8f);
            Item.DefaultToRangedWeapon(10, AmmoID.Arrow, 25, 13f);

            Item.rare = ItemRarityID.Pink;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.value = Item.sellPrice(0, 4);

            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.useTurn = false;
            Item.UseSound = CoraliteSoundID.Bow_Item5;
        }

        public override void HoldItem(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.AddDash(this);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.One);
            float rot = dir.ToRotation();

            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item)
                , player.Center, Vector2.Zero, ProjectileType<SeismicWaveHeldProj>(), damage, knockback, player.whoAmI, rot);

            Projectile.NewProjectile(source, player.Center, velocity, type, (int)(damage * 0.7f), knockback, player.whoAmI);
            Projectile.NewProjectile(source, player.Center, velocity.RotateByRandom(-0.1f, 0.1f), type, (int)(damage * 0.7f), knockback, player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(10)
                .AddIngredient(ItemID.HallowedBar, 10)
                .AddIngredient(ItemID.SoulofMight, 15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }

        public bool Dash(Player Player, int DashDir)
        {
            Vector2 newVelocity = Player.velocity;
            switch (DashDir)
            {
                case CoralitePlayer.DashDown:
                    break;
                default:
                    return false;
            }

            Player.GetModPlayer<CoralitePlayer>().DashDelay = 90;
            Player.GetModPlayer<CoralitePlayer>().DashTimer = 24;

            float rot = (Main.MouseWorld - Player.Center).ToRotation();
            if (rot < -1.57f)
            {
                rot += MathHelper.TwoPi;
            }
            rot = MathHelper.Clamp(rot, 1.57f - 0.3f, 1.57f + 0.3f);

            Player.velocity = rot.ToRotationVector2() * 16;
            Player.AddImmuneTime(ImmunityCooldownID.General, 14);
            Player.immune = true;

            Main.instance.CameraModifiers.Add(new MoveModifyer(5, 10));

            if (Player.whoAmI == Main.myPlayer)
            {
                //Helper.PlayPitched("Misc/HallowDash", 0.4f, -0.2f, Player.Center);
                Helper.PlayPitched(CoraliteSoundID.Swing_Item1, Player.Center);

                foreach (var proj in from proj in Main.projectile
                                     where proj.active && proj.friendly && proj.owner == Player.whoAmI && proj.type == ProjectileType<SeismicWaveHeldProj>()
                                     select proj)
                {
                    proj.Kill();
                    break;
                }

                //生成手持弹幕
                Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, newVelocity, ProjectileType<SeismicWaveHeldProj>(),
                    Player.GetWeaponDamage(Item), Player.HeldItem.knockBack, Player.whoAmI, rot, 1, 24);
            }

            return true;
        }
    }

    [AutoLoadTexture(Path = AssetDirectory.ThyphionSeriesItems)]
    public class SeismicWaveHeldProj : BaseDashBow
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + nameof(SeismicWave);

        public ref float Timer => ref Projectile.localAI[1];
        public ref float Hited => ref Projectile.localAI[2];

        [AutoLoadTexture(Name = "SeismicWave_Glow")]
        public static ATex GlowTex { get; private set; }

        public override int GetItemType()
            => ItemType<SeismicWave>();

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 70;
        }

        public override bool? CanDamage()
        {
            if (Special == 1 && Timer < DashTime + 2)
                return null;

            return false;
        }

        public override void DashAttackAI()
        {
            if (Timer < DashTime + 2)
            {
                Owner.direction = InMousePos.X > Owner.Center.X ? 1 : -1;

                //Rotation = Projectile.velocity.ToRotation();
                Point p = Projectile.Center.ToTileCoordinates();

                if (Hited == 0)
                {
                    Owner.velocity = Rotation.ToRotationVector2() * 16;
                    for (int i = -1; i < 2; i++)
                        for (int j = -2; j < 2; j++)
                        {
                            Tile t = Framing.GetTileSafely(p.X + i, p.Y + j);
                            if (t.HasUnactuatedTile && (Main.tileSolid[t.TileType] || Main.tileSolidTop[t.TileType]))
                            {
                                Strike();
                                break;
                            }
                        }

                    var particle = PRTLoader.NewParticle<PixelLine>(Projectile.Center + Main.rand.NextVector2Circular(40, 40)
                         , -Rotation.ToRotationVector2() * Main.rand.NextFloat(2, 7), Main.rand.NextFromList(Color.Silver, new Color(68, 235, 229)));
                    particle.TrailCount = Main.rand.Next(10, 24);
                }
            }
            else
                Projectile.Kill();

            Projectile.rotation = Rotation;
            Timer++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Hited == 1)
                return;

            Owner.velocity.Y = -12;
            Strike();
        }

        public void Strike()
        {
            if (Hited == 1)
                return;

            Hited = 1;
            Timer = DashTime - 5;
            //生成震波弹幕
            Collision.HitTiles(Projectile.Center - new Vector2(100, 20), new Vector2(0, 0), 200, 100);

            Projectile.NewProjectileFromThis<SeismicWaveStrike>(Projectile.Center, Vector2.Zero, (int)(Owner.GetWeaponDamage(Item) * 1.2f)
                , 8);

            Helper.PlayPitched(CoraliteSoundID.StaffOfEarth_Item69, Projectile.Center, pitch: 0.8f);
            Helper.PlayPitched(CoraliteSoundID.DungeonSpirit1_Zombie53, Projectile.Center, pitch: -0.5f);

            if (Owner.immuneTime < 20)
                Owner.AddImmuneTime(ImmunityCooldownID.General, 30);
            Owner.immune = true;

            Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Vector2.UnitY, 15, 8, 15, 800));
        }

        public override Vector2 GetOffset()
            => new Vector2(20, 0);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 center = Projectile.Center - Main.screenPosition;
            var effect = DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            var origin = mainTex.Size() / 2;

            Main.spriteBatch.Draw(mainTex, center, null, lightColor, Projectile.rotation, origin, 1, effect, 0f);
            Main.spriteBatch.Draw(GlowTex.Value, center, null, Color.White, Projectile.rotation, origin, 1, effect, 0f);

            return false;
        }
    }

    public class SeismicWaveStrike : ModProjectile, IDrawWarp
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + "SeismicWaveImpact";

        public ref float Scale => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];
        public ref float Alpha => ref Projectile.ai[2];

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = Projectile.height = 320;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 1000;
            Projectile.penetrate = -1;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 12;
        }

        public override void AI()
        {
            Scale = Coralite.Instance.X2Smoother.Smoother(Timer / 16f) * 2;
            Timer++;

            if (Timer < 6)
                Alpha = Timer / 6f;
            else
                Alpha = 1 - (Timer - 6) / 18f;

            float length = 20 + Timer / 16f * 130;

            for (int i = 0; i < 5; i++)
            {
                Vector2 dir = Helper.NextVec2Dir();
                Dust d = Dust.NewDustPerfect(Projectile.Center + dir * Main.rand.NextFloat(length - 10, length + 10)
                    , DustID.SilverFlame, dir * Main.rand.NextFloat(2, 6), Alpha: Main.rand.Next(0, 100), Scale: Main.rand.NextFloat(1, 1.5f));
                d.noGravity = true;
            }

            if (Timer > 16)
                Projectile.Kill();
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = MathF.Sign(target.Center.X - Projectile.Center.X);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = CoraliteAssets.Halo.CircleSPA.Value;
            var pos = Projectile.Center - Main.screenPosition;
            Vector2 origin = mainTex.Size() / 2;

            Color c = Color.LightGray * Alpha * 0.3f;

            Main.spriteBatch.Draw(mainTex, pos, null, c, Projectile.rotation, origin, Scale, 0, 0);
            Main.spriteBatch.Draw(mainTex, pos, null, c, Projectile.rotation + 0.785f, origin, Scale, 0, 0);

            return false;
        }

        public void DrawWarp()
        {
            Texture2D mainTex = Projectile.GetTexture();
            var pos = Projectile.Center - Main.screenPosition;
            Vector2 origin = mainTex.Size() / 2;

            Color c = Color.White * Alpha;

            Main.spriteBatch.Draw(mainTex, pos, null, c, Projectile.rotation, origin, Scale, 0, 0);
            Main.spriteBatch.Draw(mainTex, pos, null, c, Projectile.rotation + MathHelper.TwoPi / 3, origin, Scale, 0, 0);
            Main.spriteBatch.Draw(mainTex, pos, null, c, Projectile.rotation + MathHelper.TwoPi / 3 * 2, origin, Scale, 0, 0);
        }
    }
}
