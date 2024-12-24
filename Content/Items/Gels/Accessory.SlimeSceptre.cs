using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.Gels
{
    public class SlimeSceptre : ModItem, IDashable
    {
        public override string Texture => AssetDirectory.GelItems + Name;

        public float Priority => IDashable.HeldItemDash;

        public bool Dash(Player Player, int DashDir)
        {
            var offset = DashDir switch
            {
                CoralitePlayer.DashRight => new Vector2(-25, 0),
                CoralitePlayer.DashLeft => new Vector2(25, 0),
                CoralitePlayer.DashDown => new Vector2(0, -25),
                CoralitePlayer.DashUp => new Vector2(0, 35),
                _ => Vector2.Zero
            };

            if (Main.myPlayer == Player.whoAmI)
            {
                Projectile.NewProjectile(Player.GetSource_Accessory(Item), Player.Center + offset,
                    Player.velocity * 0.9f, Item.shoot, Player.GetWeaponDamage(Item), Item.knockBack, Player.whoAmI);
            }

            Player.GetModPlayer<CoralitePlayer>().DashDelay = 80;
            Player.GetModPlayer<CoralitePlayer>().DashTimer = 10;

            return true;
        }

        public override void SetDefaults()
        {
            Item.damage = 38;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.mana = 14;
            Item.useAnimation = Item.useTime = 24;
            Item.DamageType = DamageClass.Magic;
            Item.shootSpeed = 7;
            Item.shoot = ModContent.ProjectileType<SlimeSceptreBall>();
            Item.knockBack = 10;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.accessory = true;
            Item.noMelee = true;
            Item.UseSound = CoraliteSoundID.SlimeMount_Item81;
        }

        public override void HoldItem(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.AddDash(this);
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.moveSpeed += 0.08f;
            player.GetDamage(DamageClass.Generic) += 0.04f;
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.AddDash(this);
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(source, player.Center + ((Main.MouseWorld - player.Center).SafeNormalize(Vector2.Zero) * 32),
                    (Main.MouseWorld - player.Center).SafeNormalize(Vector2.Zero) * Item.shootSpeed, type, damage, knockback, player.whoAmI);
            }
            return false;
        }
    }

    public class SlimeSceptreBall : ModProjectile
    {
        public override string Texture => AssetDirectory.SlimeEmperor + "ElasticGelBall";

        public ref float State => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 54;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 30;
            Projectile.tileCollide = false;
            Projectile.friendly = true;

            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];

            switch ((int)State)
            {
                case 0:
                    Projectile.ai[2] += 0.1f;  //膨胀小动画
                    Projectile.velocity.X *= 0.99f;
                    //if (Projectile.velocity.Y < 16)
                    //    Projectile.velocity.Y += 0.25f;

                    if (Projectile.ai[2] > Projectile.scale)
                    {
                        float targetRot = Projectile.velocity.Length() * 0.04f;
                        Projectile.rotation = Projectile.rotation.AngleTowards(targetRot, 0.01f);
                        State = 1;
                        Projectile.ai[2] = Projectile.scale;
                        Projectile.tileCollide = true;
                    }
                    break;
                case 1:
                    Projectile.velocity *= 0.99f;
                    Projectile.ai[1]++;
                    Projectile.ai[2] = Projectile.scale + (MathF.Sin(Main.GlobalTimeWrappedHourly * 2) * 0.1f);
                    if (Vector2.Distance(Projectile.Center, Owner.Center) < 64)//玩家在附近，md跟你爆了
                    {
                        State = 2;
                        Vector2 dir = (Owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                        Owner.velocity.X = dir.X * 12;
                        Owner.velocity.Y = dir.Y * 16;
                        Owner.AddBuff(BuffID.Slimed, 180);
                        SoundEngine.PlaySound(CoraliteSoundID.QueenSlime_Item154, Projectile.Center);
                    }
                    if (Projectile.ai[1] > 60)
                    {
                        SoundEngine.PlaySound(CoraliteSoundID.QueenSlime_Item154, Projectile.Center);
                        State = 2;
                    }
                    break;
                case 2:
                    Projectile.velocity = Vector2.Zero;
                    Projectile.frameCounter++;
                    if (Projectile.frameCounter > 2)
                    {
                        Projectile.frameCounter = 0;
                        Projectile.frame++;
                        if (Projectile.frame > 6)
                            Projectile.Kill();
                    }
                    break;
                default:
                    break;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.localAI[0]++;
            //简易撞墙反弹
            float newVelX = Math.Abs(Projectile.velocity.X);
            float newVelY = Math.Abs(Projectile.velocity.Y);
            float oldVelX = Math.Abs(oldVelocity.X);
            float oldVelY = Math.Abs(oldVelocity.Y);
            if (oldVelX > newVelX)
                Projectile.velocity.X = -oldVelX * 0.8f;
            if (oldVelY > newVelY)
                Projectile.velocity.Y = -oldVelY * 0.8f;

            //Projectile.rotation = Projectile.velocity.ToRotation();
            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), DustID.t_Slime,
                       -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f)) * Main.rand.NextFloat(0.1f, 0.3f), 150, new Color(78, 136, 255, 80), Main.rand.NextFloat(1f, 1.4f));
                dust.noGravity = true;
            }

            if (Projectile.localAI[0] > 3)
            {
                Projectile.tileCollide = false;
                State = 2;
            }

            return false;
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            State = 2;
        }

        public override bool? CanDamage()
        {
            if ((State == 1 && Projectile.ai[1] > 5) || State == 2)
                return null;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var frameBox = mainTex.Frame(1, 7, 0, Projectile.frame);
            var origin = frameBox.Size() / 2;

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, frameBox, lightColor * Projectile.ai[2], Projectile.rotation, origin, Projectile.ai[2], 0, 0);
            return false;
        }
    }
}
