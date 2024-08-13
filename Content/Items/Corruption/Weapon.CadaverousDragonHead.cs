using Coralite.Content.Items.Crimson;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Items.Corruption
{
    public class CadaverousDragonHead : ModItem
    {
        public override string Texture => AssetDirectory.CorruptionItems + Name;

        public override void SetStaticDefaults()
        {
            //ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<VertebraeBlade>();
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 24;
            Item.useTime = 6;
            Item.width = 50;
            Item.height = 18;
            Item.shoot = ModContent.ProjectileType<CadaverousDragonHeadProj>();
            Item.UseSound = CoraliteSoundID.Flamethrower_Item34;
            Item.damage = 14;
            Item.knockBack = 0.3f;
            Item.shootSpeed = 6.5f;
            Item.mana = 10;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Orange;
            Item.DamageType = DamageClass.Magic;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool MagicPrefix() => true;

        //public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        //{
        //    if (player.altFunctionUse == 2 && !player.ItemAnimationJustStarted)
        //    {
        //        reduce -= 1f;
        //    }
        //}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                //if (player.altFunctionUse == 2)
                //{
                //    if (player.ItemAnimationJustStarted)//只有第一帧才会射火球
                //    {
                //        Projectile head1 = SpawnHead(player, source);
                //        //射火球

                //    }

                //    return false;
                //}

                //射火焰
                Projectile head2 = SpawnHead(player, source);
                Projectile.NewProjectile(source, head2.Center, (Main.MouseWorld - head2.Center).SafeNormalize(Vector2.Zero) * 6, ModContent.ProjectileType<CadaverousDragonBreath>(), (int)(damage * 0.95f), knockback, player.whoAmI);
                head2.ai[0] = player.itemTimeMax;
                player.CheckMana(player.GetManaCost(player.HeldItem) / 2, pay: true);
            }
            return false;
        }

        public static Projectile SpawnHead(Player player, IEntitySource source)
        {
            Projectile p = Main.projectile.FirstOrDefault(p => p.active && p.owner == player.whoAmI && p.type == ModContent.ProjectileType<CadaverousDragonHeadProj>());
            p ??= Projectile.NewProjectileDirect(source, player.Center, Vector2.Zero, ModContent.ProjectileType<CadaverousDragonHeadProj>(), 1, 0, player.whoAmI);

            return p;
        }
    }

    public class CadaverousDragonHeadProj : ModProjectile
    {
        public override string Texture => AssetDirectory.CorruptionItems + Name;

        public ref float ShootTimer => ref Projectile.ai[0];
        public Vector2 TargetPos
        {
            get
            {
                return new Vector2(Projectile.localAI[0], Projectile.localAI[1]);
            }
            set
            {
                Projectile.localAI[0] = value.X;
                Projectile.localAI[1] = value.Y;
            }
        }

        public Player Owner => Main.player[Projectile.owner];

        public float mouseAngle;

        public override void OnSpawn(IEntitySource source)
        {
            TargetPos = Projectile.Center;
        }

        public override void AI()
        {
            //常态在玩家头顶

            if (Owner.HeldItem.type == ModContent.ItemType<CadaverousDragonHead>())
                Projectile.timeLeft = 2;
            else
                Projectile.Kill();

            //Point p = Projectile.Center.ToTileCoordinates();
            Vector2 idlePos = Owner.Center + new Vector2(Owner.direction * 32, 0);
            //Tile tile = Framing.GetTileSafely(p);
            //idlePos = Owner.Center;
            for (int i = 0; i < 4; i++)//检测头顶4个方块并尝试找到没有物块阻挡的那个
            {
                Tile idleTile = Framing.GetTileSafely(idlePos.ToTileCoordinates());
                if (idleTile.HasTile && Main.tileSolid[idleTile.TileType] && !Main.tileSolidTop[idleTile.TileType])
                {
                    idlePos -= new Vector2(0, -16);
                    break;
                }
                else
                    idlePos += new Vector2(-Owner.direction * 8, -16);
            }

            TargetPos = Vector2.Lerp(TargetPos, idlePos, 0.1f);

            //if (tile.HasTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType])
            //{
            //}
            //else
            //    TargetPos = TargetPos.MoveTowards(idlePos, 16);
            Vector2 pos = TargetPos;

            //ai0大于0时张嘴射
            if (ShootTimer > 0)
            {
                Projectile.rotation = (Main.MouseWorld - Projectile.Center).ToRotation() + (Projectile.spriteDirection > 0 ? 0 : 3.141f);
                Projectile.direction = Projectile.spriteDirection = Main.MouseWorld.X > Projectile.Center.X ? 1 : -1;
                mouseAngle = mouseAngle.AngleLerp(0.6f, 0.1f);
                pos += (Main.MouseWorld - pos).SafeNormalize(Vector2.Zero) * Math.Clamp((Main.MouseWorld - pos).Length() / 700, 0, 1) * 96;
                ShootTimer--;
            }
            else
            {
                Projectile.rotation = Owner.velocity.X / 15;
                Projectile.direction = Projectile.spriteDirection = Owner.direction;
                mouseAngle = mouseAngle.AngleLerp(0, 0.05f);
            }

            Projectile.Center = Vector2.Lerp(Projectile.Center, pos, 0.25f);

            Vector2 neckPos = Projectile.Center - (Projectile.rotation + (Projectile.spriteDirection > 0 ? 0 : 3.141f) - Projectile.spriteDirection * 0.6f).ToRotationVector2() * 10;
            //在尾部生成粒子
            Dust d = Dust.NewDustPerfect(neckPos + Main.rand.NextVector2Circular(6, 6), DustID.Shadowflame, Helpers.Helper.NextVec2Dir(0.5f, 1f));
            d.noGravity = true;

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 pos = Projectile.Center - Main.screenPosition;

            //绘制上下鄂
            Texture2D mainTex = Projectile.GetTexture();
            Rectangle frameBox = mainTex.Frame(1, 2, 0, 1);
            Vector2 origin = new(frameBox.Width * 0.3f, frameBox.Height / 2);
            Color c = lightColor;

            SpriteEffects effects = SpriteEffects.None;
            float exRot = 0;
            if (Projectile.spriteDirection < 0)
            {
                effects = SpriteEffects.FlipHorizontally;
                //exRot = MathHelper.Pi ;
                origin = new Vector2(frameBox.Width * 0.7f, frameBox.Height / 2);
            }

            float rot = Projectile.rotation + Projectile.spriteDirection * mouseAngle + exRot;
            Main.spriteBatch.Draw(mainTex, pos, frameBox, c, rot, origin, Projectile.scale, effects, 0);

            rot = Projectile.rotation - Projectile.spriteDirection * mouseAngle + exRot;
            frameBox = mainTex.Frame(1, 2, 0, 0);
            Main.spriteBatch.Draw(mainTex, pos, frameBox, c, rot, origin, Projectile.scale, effects, 0);

            return false;
        }
    }

    public class CadaverousDragonBreath : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public override void SetDefaults()
        {
            Projectile.ArmorPenetration = 15; // Added by TML

            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 4;
            Projectile.extraUpdates = 2;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = oldVelocity * 0.95f;
            Projectile.position -= Projectile.velocity;

            return false;
        }

        public override void AI()
        {
            Projectile.localAI[0] += 1f;
            int shootTime = 40;
            int fadeTime = 12;
            int totalTime = shootTime + fadeTime;
            if (Projectile.localAI[0] >= totalTime)
                Projectile.Kill();

            if (Projectile.localAI[0] >= shootTime)
                Projectile.velocity *= 0.95f;

            Lighting.AddLight(Projectile.Center, Color.MediumPurple.ToVector3());

            if (Projectile.localAI[0] < shootTime && Main.rand.NextFloat() < 0.25f)
            {
                short type = Main.rand.NextBool() ? DustID.ShadowbeamStaff : DustID.Shadowflame;
                Dust dust = Dust.NewDustDirect(Projectile.Center + Main.rand.NextVector2Circular(60f, 60f) * Utils.Remap(Projectile.localAI[0], 0f, 72f, 0.5f, 1f), 4, 4, type, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100);
                if (Main.rand.NextBool(4))
                {
                    dust.noGravity = true;
                    dust.scale *= 1.2f;
                    dust.velocity.X *= 2f;
                    dust.velocity.Y *= 2f;
                }
                //else
                //dust.scale *= 1f;

                //dust.scale *= 1.5f;
                dust.velocity *= 1.2f;
                dust.velocity += Projectile.velocity * 1f * Utils.Remap(Projectile.localAI[0], 0f, shootTime * 0.75f, 1f, 0.1f) * Utils.Remap(Projectile.localAI[0], 0f, shootTime * 0.1f, 0.1f, 1f);
                dust.customData = 1;
            }

            if (Projectile.localAI[0] >= shootTime && Main.rand.NextBool())
            {
                Vector2 center = Main.player[Projectile.owner].Center;
                Vector2 vector = (Projectile.Center - center).SafeNormalize(Vector2.Zero).RotatedByRandom(0.19634954631328583) * 7f;
                short num7 = DustID.Smoke;
                Dust dust2 = Dust.NewDustDirect(Projectile.Center + Main.rand.NextVector2Circular(50f, 50f) - vector * 2f, 4, 4, num7, 0f, 0f, 150, new Color(80, 80, 80));
                dust2.noGravity = true;
                dust2.velocity = vector;
                dust2.scale *= 1.1f + Main.rand.NextFloat() * 0.2f;
                dust2.customData = -0.3f - 0.15f * Main.rand.NextFloat();
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 7)
            {
                Projectile.frameCounter = 0;
                if (Projectile.frame < 7)
                    Projectile.frame++;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.65);
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            int num = (int)Utils.Remap(Projectile.localAI[0], 0f, 72f, 10f, 40f);
            hitbox.Inflate(num, num);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!projHitbox.Intersects(targetHitbox))
                return false;

            return Collision.CanHit(Projectile.Center, 0, 0, targetHitbox.Center.ToVector2(), 0, 0);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 pos = Projectile.Center - Main.screenPosition;

            Main.instance.LoadProjectile(ProjectileID.Flames);
            Texture2D mainTex = TextureAssets.Projectile[ProjectileID.Flames].Value;
            //Rectangle frameBox = mainTex.Frame(1, 7, 0, Projectile.frame);
            //Main.spriteBatch.Draw(mainTex, pos, frameBox, Color.MediumPurple, Projectile.rotation, frameBox.Size()/2, Projectile.scale, 0, 0);

            float shootTime = 50f;
            float fadeTime = 12f;
            float totalTime = shootTime + fadeTime;
            Color transparent = Color.Transparent;
            Color color = new(100, 90, 255, 200);
            Color color2 = new(130, 80, 200, 70);
            Color color3 = Color.Lerp(new Color(100, 90, 255, 100), color2, 0.25f);
            Color color4 = new(80, 80, 80, 100);
            float num3 = 0.35f;
            float num4 = 0.7f;
            float num5 = 0.85f;
            float num6 = (Projectile.localAI[0] > shootTime - 10f) ? 0.175f : 0.2f;

            int verticalFrames = 7;
            float num9 = Utils.Remap(Projectile.localAI[0], shootTime, totalTime, 1f, 0f);
            float num10 = Math.Min(Projectile.localAI[0], 20f);
            float num11 = Utils.Remap(Projectile.localAI[0], 0f, totalTime, 0f, 1f);
            float num12 = Utils.Remap(num11, 0.2f, 0.5f, 0.25f, 1f);
            Rectangle frameBox = mainTex.Frame(1, verticalFrames, 0, 3);
            if (!(num11 < 1f))
                return false;

            for (int i = 0; i < 2; i++)
            {
                for (float j = 1f; j >= 0f; j -= num6)
                {
                    transparent = (num11 < 0.1f) ?
                        Color.Lerp(Color.Transparent, color, Utils.GetLerpValue(0f, 0.1f, num11, clamped: true)) :
                        ((num11 < 0.2f) ?
                            Color.Lerp(color, color2, Utils.GetLerpValue(0.1f, 0.2f, num11, clamped: true)) :
                            ((num11 < num3) ?
                                color2 :
                                ((num11 < num4) ?
                                    Color.Lerp(color2, color3, Utils.GetLerpValue(num3, num4, num11, clamped: true)) :
                                    ((num11 < num5) ?
                                        Color.Lerp(color3, color4, Utils.GetLerpValue(num4, num5, num11, clamped: true)) :
                                        ((!(num11 < 1f)) ? Color.Transparent : Color.Lerp(color4, Color.Transparent, Utils.GetLerpValue(num5, 1f, num11, clamped: true)))))));
                    float num14 = (1f - j) * Utils.Remap(num11, 0f, 0.2f, 0f, 1f);
                    Vector2 vector = Projectile.Center - Main.screenPosition + Projectile.velocity * (0f - num10) * j;
                    Color color5 = transparent * num14;
                    Color color6 = color5;
                    color6.G /= 2;
                    color6.B /= 2;
                    color6.A = (byte)Math.Min(color5.A + 80f * num14, 255f);
                    Utils.Remap(Projectile.localAI[0], 20f, totalTime, 0f, 1f);

                    float factor = 1f / num6 * (j + 1f);
                    float num16 = Projectile.rotation + j * MathHelper.PiOver2 + Main.GlobalTimeWrappedHourly * factor * 2f;
                    float num17 = Projectile.rotation - j * MathHelper.PiOver2 - Main.GlobalTimeWrappedHourly * factor * 2f;
                    switch (i)
                    {
                        case 0:
                            Main.EntitySpriteDraw(mainTex, vector + Projectile.velocity * (0f - num10) * num6 * 0.5f, frameBox, color6 * num9 * 0.25f, num16 + (float)Math.PI / 4f, frameBox.Size() / 2f, num12, SpriteEffects.None);
                            Main.EntitySpriteDraw(mainTex, vector, frameBox, color6 * num9, num17, frameBox.Size() / 2f, num12, SpriteEffects.None);
                            break;
                        case 1:
                            Main.EntitySpriteDraw(mainTex, vector + Projectile.velocity * (0f - num10) * num6 * 0.2f, frameBox, color5 * num9 * 0.25f, num16 + (float)Math.PI / 2f, frameBox.Size() / 2f, num12 * 0.75f, SpriteEffects.None);
                            Main.EntitySpriteDraw(mainTex, vector, frameBox, color5 * num9, num17 + (float)Math.PI / 2f, frameBox.Size() / 2f, num12 * 0.75f, SpriteEffects.None);
                            break;
                    }
                }
            }

            return false;
        }
    }

    //被砍掉的右键功能
    //public class CadaverousDragonFireBall
    //{

    //}
}
