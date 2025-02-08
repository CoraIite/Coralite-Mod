using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields
{
    public class TortoiseshellFortress : BaseFlyingShieldItem<TortoiseshellFortressGuard>, IDashable
    {
        public TortoiseshellFortress() : base(Item.sellPrice(0, 2, 0, 0), ItemRarityID.Lime, AssetDirectory.FlyingShieldItems)
        { }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 15;
            Item.shoot = ModContent.ProjectileType<TortoiseshellFortressProj>();
            Item.knockBack = 6;
            Item.shootSpeed = 10;
            Item.damage = 95;
        }

        public override void HoldItem(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.AddDash(this);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.TurtleShell, 2)
                .AddIngredient(ItemID.ChlorophyteBar, 12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }

        public override bool Dash(Player Player, int DashDir)
        {
            Vector2 newVelocity = Player.velocity;
            switch (DashDir)
            {
                case CoralitePlayer.DashLeft:
                case CoralitePlayer.DashRight:
                    {
                        float dashDirection = DashDir == CoralitePlayer.DashRight ? 1 : -1;
                        newVelocity.X = dashDirection * 24;
                        break;
                    }
                default:
                    return false;
            }

            Player.GetModPlayer<CoralitePlayer>().DashDelay = 60 * 3;
            Player.GetModPlayer<CoralitePlayer>().DashTimer = 80;
            Player.immuneTime = 20;
            Player.immune = true;
            Player.velocity = newVelocity;

            if (Player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(CoraliteSoundID.GiantTortoise_NPCHit24, Player.Center);

                for (int i = 0; i < 1000; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj.active && proj.friendly && proj.owner == Player.whoAmI && proj.ModProjectile is TortoiseshellFortressGuard)
                    {
                        proj.Kill();
                        break;
                    }
                }

                //生成手持弹幕
                Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, Vector2.Zero, ModContent.ProjectileType<TortoiseshellFortressDash>(),
                    Player.HeldItem.damage * 4, Player.HeldItem.knockBack, Player.whoAmI, (Main.MouseWorld - Player.Center).ToRotation(), 1);
            }

            return true;

        }
    }

    public class TortoiseshellFortressProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "TortoiseshellFortress";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 40;
        }

        public override void SetOtherValues()
        {
            flyingTime = 15;
            backTime = 12;
            backSpeed = 12;
            trailCachesLength = 9;
            trailWidth = 34 / 2;
        }

        public override void OnShootDusts()
        {
            extraRotation += 0.4f;
        }

        public override void OnBackDusts()
        {
            extraRotation += 0.4f;
        }

        public override void DrawTrails(Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var origin = mainTex.Size() / 2;
            for (int i = trailCachesLength - 1; i > 4; i--)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] - Main.screenPosition, null,
                lightColor * 0.6f * ((i - 4) * 1 / 3f), Projectile.oldRot[i] - 1.57f + extraRotation, origin, Projectile.scale, 0, 0);

            base.DrawTrails(lightColor);
        }

        public override Color GetColor(float factor)
        {
            return new Color(114, 84, 66) * factor;
        }
    }

    public class TortoiseshellFortressGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "TortoiseshellFortress";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 40;
            Projectile.height = 42;
            Projectile.scale = 1.35f;
        }

        public override void SetOtherValues()
        {
            damageReduce = 0.3f;
            strongGuard += 0.1f;
            distanceAdder = 1.5f;
            delayTime = 26;
            scalePercent = 1.6f;
        }

        public override void OnHoldShield()
        {
            Owner.wingTime = 0;
        }

        public override void OnGuard()
        {
            DistanceToOwner /= 2;
            SoundEngine.PlaySound(CoraliteSoundID.GiantTortoise_Zombie33, Projectile.Center);
        }

        public override float GetWidth()
        {
            return Projectile.width / 2 / Projectile.scale;
        }
    }

    public class TortoiseshellFortressDash : BaseHeldProj
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "Tortoiseshell";

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 8);
        }

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.width = Projectile.height = 52;
            Projectile.scale = 1.3f;
            Projectile.timeLeft = 80;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.hide = true;
        }

        public override void AI()
        {
            Owner.itemTime = Owner.itemAnimation = 2;

            Owner.immuneTime = 5;
            Owner.immune = true;

            if (Owner.velocity.Length() < 0.5f)
            {
                Projectile.Kill();
            }

            Projectile.Center = Owner.Center;
            Projectile.rotation += 0.35f;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Owner.velocity.X = -MathF.Sign(Owner.velocity.X) * 16;
            Owner.velocity.Y += Owner.Center.Y > target.Center.Y ? 6f : -8f;
            Projectile.Kill();
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(CoraliteSoundID.GiantTortoise_NPCDeath27, Projectile.Center);
            Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(Owner.velocity.X / 10, -5), 177);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();

            Projectile.DrawShadowTrails(lightColor, 0.5f, 0.5f / 8f, 1, 8, 1);
            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, lightColor,
                Projectile.rotation, mainTex.Size() / 2, Projectile.scale, 0, 0);

            return false;
        }
    }
}
