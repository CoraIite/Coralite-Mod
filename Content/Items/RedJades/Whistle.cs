using Coralite.Content.Bosses.ModReinforce.Bloodiancie;
using Coralite.Content.Bosses.Rediancie;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.RedJades
{
    public class Whistle : ModItem
    {
        public override string Texture => AssetDirectory.RedJadeItems + Name;

        public int useCount;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 100;
            Item.useTime = 27;
            Item.useAnimation = 27;
            Item.knockBack = 2f;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Generic;
            Item.value = Item.sellPrice(0, 3, 0, 0);
            Item.rare = ItemRarityID.Expert;
            Item.shoot = ProjectileType<WhistleProj>();

            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI);
            }

            return false;
        }
    }

    public class WhistleProj : BaseHeldProj
    {
        public override string Texture => AssetDirectory.RedJadeItems + "Whistle";

        public ref float DistanceToOwner => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[2];

        public bool release;
        private SlotId soundSlot;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 400;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            do
            {
                if (Timer < 120)
                {
                    DistanceToOwner = Helper.Lerp(48, 32, Timer / 120);
                    Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                    Projectile.rotation = ToMouseAngle + (Owner.direction > 1 ? 0 : 3.141f);

                    Vector2 dir = UnitToMouseV;
                    Projectile.Center = Owner.Center + (dir * DistanceToOwner);

                    float count = Timer / 30;
                    for (int i = 0; i < count; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(Projectile.Center - (dir * 24) + Main.rand.NextVector2Circular(count * 4, count * 4), DustID.GemRuby, Vector2.Zero, 0, default, 1f + (count * 0.06f));
                        dust.noGravity = true;
                    }

                    if (Owner.channel)
                    {
                        Projectile.timeLeft = 2;
                        Owner.itemAnimation = Owner.itemTime = 2;
                    }
                    else
                        Projectile.Kill();
                    break;
                }

                if (Timer == 120)
                {
                    Projectile.velocity = UnitToMouseV * 20;
                    Projectile.tileCollide = true;
                    Projectile.timeLeft = 120;
                    Owner.itemAnimation = Owner.itemTime = 2;

                    soundSlot = Helper.PlayPitched("Misc/Whistle", 0.4f, 0f, Projectile.Center);
                }

                if (Timer < 220)
                {
                    if (!release)
                    {
                        if (Owner.channel)
                        {
                            Owner.itemAnimation = Owner.itemTime = 2;
                            Owner.Center = Projectile.Center - (Projectile.velocity.SafeNormalize(Vector2.Zero) * DistanceToOwner);
                            if (Projectile.velocity.Y < 10)
                            {
                                Projectile.velocity.Y += 0.16f;
                                Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.direction > 1 ? 0 : 3.141f);
                            }
                        }
                        else
                        {
                            release = true;
                        }
                    }

                    for (int i = 0; i < 4; i++)
                        Projectile.SpawnTrailDust(DustID.GemRuby, 0.4f);

                    break;
                }

                Projectile.Kill();
            } while (false);
            Timer++;
        }

        public override bool? CanDamage()
        {
            if (Timer < 120)
                return false;
            return null;
        }

        public override void OnKill(int timeLeft)
        {
            if (SoundEngine.TryGetActiveSound(soundSlot, out ActiveSound sound))
                sound.Stop();
            if (Main.myPlayer == Projectile.owner && Timer > 120)
            {
                var source = Projectile.GetSource_FromAI();

                Projectile p = Projectile.NewProjectileDirect(source, Projectile.Center, Vector2.Zero, ProjectileType<Bloodiancie_BigBoom>()
                      , Helper.ScaleValueForDiffMode(100, 100, 90, 80), 5f);
                p.friendly = true;

                int timeleft = 8;

                for (int j = 0; j < 6; j++)
                {
                    float rot = Main.rand.NextFloat(MathHelper.TwoPi);
                    int howMany = Main.rand.Next(3, 6);
                    for (int i = 0; i < howMany; i++)
                    {
                        p = Projectile.NewProjectileDirect(source, Projectile.Center, rot.ToRotationVector2() * 12, ProjectileType<RedFirework>(),
                             Projectile.damage / 4, 5f, ai1: timeleft + (j * 8));
                        p.friendly = true;
                        rot += MathHelper.TwoPi / howMany;
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation - MathHelper.PiOver2, mainTex.Size() / 2,
                Projectile.scale, Projectile.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            return false;
        }
    }
}
