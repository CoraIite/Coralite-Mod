using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Content.ModPlayers;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Nightmare
{
    public class Lycoris : ModItem, INightmareWeapon
    {
        public override string Texture => AssetDirectory.NightmareItems + Name;

        public int combo;

        public override void SetDefaults()
        {
            Item.useAnimation = Item.useTime = 45;
            Item.reuseDelay = 20;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.useAmmo = AmmoID.Bullet;
            Item.DamageType = DamageClass.Ranged;
            Item.rare = RarityType<NightmareRarity>();
            Item.value = Item.sellPrice(0, 30, 0, 0);
            Item.SetWeaponValues(300, 4, 4);
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useTurn = false;
            Item.shootSpeed = 24;
        }

        public override bool RangedPrefix() => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                PlayerNightmareEnergy.Spawn(player, Item);

                int projType = ProjectileType<LycorisBullet>();
                int heldProjType = ProjectileType<LycorisHeldProj>();

                if (player.TryGetModPlayer(out CoralitePlayer cp) && cp.nightmareEnergy == cp.nightmareEnergyMax)
                {
                    cp.nightmareEnergy = 0;
                    Helper.PlayPitched("Misc/Zaphkiel", 1f, 0f, position);

                    Projectile.NewProjectile(source, position, velocity, projType, damage * 10, knockback, player.whoAmI, 1);
                    Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero, heldProjType, 1, 1, player.whoAmI);

                    return false;
                }

                Helper.PlayPitched("Misc/Gun", 0.3f, 0f, position);

                Projectile.NewProjectile(source, position, velocity, projType, (int)(damage * 2.55f), knockback, player.whoAmI);
                Projectile.NewProjectile(source, player.Center, Vector2.Zero, heldProjType, 1, 1, player.whoAmI);
            }

            return false;
        }

    }

    public class LycorisHeldProj : BaseGunHeldProj
    {
        public LycorisHeldProj() : base(1f, 26, -10, AssetDirectory.NightmareItems) { }

        public override float Ease()
        {
            float x = 1.465f * Projectile.timeLeft / MaxTime;
            return x * MathF.Sin(x * x * x) / 1.186f;
        }

        public override void Initialize()
        {
            base.Initialize();
            float rotation = TargetRot + (OwnerDirection > 0 ? 0 : MathHelper.Pi);
            Vector2 dir = rotation.ToRotationVector2();
            Vector2 center = Projectile.Center + dir * 32;
            for (int i = 0; i < 6; i++)
            {
                Dust dust = Dust.NewDustPerfect(center + Main.rand.NextVector2Circular(8, 8), DustType<NightmareStar>(), dir.RotatedBy(Main.rand.NextFloat(-0.45f, 0.45f)) * Main.rand.NextFloat(1f, 6f),
                    newColor: NightmarePlantera.nightmareRed, Scale: Main.rand.NextFloat(0.8f, 1.5f));
                dust.noGravity = true;
                dust.rotation = dust.velocity.ToRotation() + MathHelper.PiOver2;
            }
        }
    }

    /// <summary>
    /// 使用ai0传入状态，为1时是强化攻击<br></br>
    /// 
    /// </summary>
    public class LycorisBullet : ModProjectile
    {
        public override string Texture => AssetDirectory.NightmareItems + Name;

        public ref float State => ref Projectile.ai[0];
        public ref float Init => ref Projectile.localAI[0];

        public Player Owner => Main.player[Projectile.owner];

        public bool CanSpawnSmallBullet
        {
            get => Projectile.ai[2] == 0;
            set
            {
                if (!value)
                {
                    Projectile.ai[2] = 1;
                    Projectile.netUpdate = true;
                }
            }
        }

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.timeLeft = 30;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 3;
            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void AI()
        {
            if (Init == 0)
            {
                Init = 1;
                float velLength = Projectile.velocity.Length();
                //velLength *= (Projectile.extraUpdates + 1);
                float distance = (Main.MouseWorld - Owner.Center).Length();
                if (distance < 1000)
                    distance = 1000;

                Projectile.timeLeft = (int)(distance / velLength);//手动修改持续时间，让它在鼠标位置死掉
                Projectile.rotation = Projectile.velocity.ToRotation();
            }

            if (Projectile.timeLeft % 3 == 0 && Main.rand.NextBool(3))
            {
                int howMany = Main.rand.Next(5, 8);
                Vector2 center = Projectile.Center + Main.rand.NextVector2Circular(16, 16);
                float baseRot = Main.rand.NextFloat(6.282f);
                for (int i = 0; i < howMany; i++)
                {
                    Dust dust = Dust.NewDustPerfect(center, DustType<NightmareStar>(), baseRot.ToRotationVector2().RotatedBy(Main.rand.NextFloat(-0.04f, 0.04f)) * Main.rand.NextFloat(0.5f, 1f),
                        200, Color.DarkRed, Scale: Main.rand.NextFloat(0.8f, 1.2f));
                    dust.noGravity = true;
                    dust.rotation = dust.velocity.ToRotation() + MathHelper.PiOver2;

                    baseRot += MathHelper.TwoPi / howMany;
                }
            }

            if (State == 1)
            {
                if (Projectile.timeLeft % 4 == 0 && Main.rand.NextBool(3))
                {
                    int howMany = Main.rand.Next(5, 8);
                    Vector2 center = Projectile.Center + Main.rand.NextVector2Circular(16, 16);
                    float baseRot = Main.rand.NextFloat(6.282f);
                    for (int i = 0; i < howMany; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(center, DustType<NightmareStar>(), baseRot.ToRotationVector2().RotatedBy(Main.rand.NextFloat(-0.04f, 0.04f)) * Main.rand.NextFloat(0.5f, 1f),
                            200, NightmarePlantera.nightmareRed, Scale: Main.rand.NextFloat(0.8f, 1.4f));
                        dust.noGravity = true;
                        dust.rotation = dust.velocity.ToRotation() + MathHelper.PiOver2;

                        baseRot += MathHelper.TwoPi / howMany;

                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Owner.TryGetModPlayer(out CoralitePlayer cp))
                cp.GetNightmareEnergy(1);
            CanSpawnSmallBullet = false;
        }

        public override void OnKill(int timeLeft)
        {
            if (CanSpawnSmallBullet || State == 1)//能生成或者是强化状态时才会生成小弹幕
            {
                if (Main.myPlayer == Projectile.owner)//生成一圈小子弹
                {
                    float rot = Main.rand.NextFloat(6.282f);
                    int howMany = 5;
                    int damage = (int)(Projectile.damage * 0.1f);
                    if (State == 1)
                    {
                        howMany = 7;
                        if (Owner.TryGetModPlayer(out CoralitePlayer cp) && cp.nightmareEnergyMax > 7)
                            howMany = 10;

                        damage = (int)(Projectile.damage * 0.28f);
                    }

                    for (int i = 0; i < howMany; i++)
                    {
                        Vector2 dir = rot.ToRotationVector2();
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, dir * 4,
                            ProjectileType<LycorisSmallBullet>(), damage, 0, Projectile.owner);

                        FlowLine.Spawn(Projectile.Center, dir.RotatedByRandom(0.2f) * Main.rand.NextFloat(10, 18), 2, 10, Main.rand.NextFloat(0.3f, 0.4f), NightmarePlantera.nightmareRed);
                        FlowLine.Spawn(Projectile.Center, dir.RotatedByRandom(-0.2f) * Main.rand.NextFloat(10, 18), 2, 10, Main.rand.NextFloat(0.3f, 0.4f), NightmarePlantera.nightmareRed);

                        FlowLine.Spawn(Projectile.Center, dir.RotatedByRandom(0.2f) * Main.rand.NextFloat(6, 10), 2, 10, Main.rand.NextFloat(0.1f, 0.2f), Color.DarkRed);
                        FlowLine.Spawn(Projectile.Center, dir.RotatedByRandom(-0.2f) * Main.rand.NextFloat(6, 10), 2, 10, Main.rand.NextFloat(0.1f, 0.2f), Color.DarkRed);

                        rot += MathHelper.TwoPi / howMany;
                    }
                }
            }
            else
            {
                int howMany = Main.rand.Next(5, 8);
                Vector2 center = Projectile.Center;
                float baseRot = Main.rand.NextFloat(6.282f);
                float vel = 1.5f;
                for (int i = 0; i < howMany; i++)
                {
                    Vector2 dir = baseRot.ToRotationVector2();
                    for (int j = 0; j < 3; j++)
                    {
                        Color c = NightmarePlantera.nightmareRed;
                        if (j > 1)
                            c = Color.DarkRed;
                        Dust dust = Dust.NewDustPerfect(center, DustType<NightmareStar>(), dir.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * j * vel,
                            200, c, Scale: Main.rand.NextFloat(0.8f, 1.4f));
                        dust.noGravity = true;
                        dust.rotation = dust.velocity.ToRotation() + MathHelper.PiOver2;
                    }

                    baseRot += MathHelper.TwoPi / howMany;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation
                , new Vector2(mainTex.Width, mainTex.Height / 2), 1, 0, 0);
            return false;
        }
    }

    public class LycorisSmallBullet : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 12);
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.penetrate = 3;
            Projectile.friendly = true;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }

        public override void AI()
        {
            //从夜光的弹幕里抄来的，删除了为敌对弹幕时使用的特殊ai
            bool readyState = false;
            bool flag2 = false;
            float readyTime = 180f;
            float num2 = 20f;
            float num3 = 0.97f;
            float value = 0.075f;
            float value2 = 0.125f;
            float num4 = 30f;

            if (Projectile.timeLeft == 238)
            {
                int num5 = Projectile.alpha;
                Projectile.alpha = 0;
                Color C = NightmarePlantera.nightmareRed;
                Projectile.alpha = num5;
                for (int i = 0; i < 3; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, 267, Main.rand.NextVector2CircularEdge(3f, 3f) * (Main.rand.NextFloat() * 0.5f + 0.5f), 0, C);
                    dust.scale *= 1.2f;
                    dust.noGravity = true;
                }
            }

            if (Projectile.timeLeft > readyTime)
                readyState = true;
            else if (Projectile.timeLeft > num2)
                flag2 = true;

            if (readyState)
            {
                float num6 = (float)Math.Cos(Projectile.whoAmI % 6f / 6f + Projectile.position.X / 320f + Projectile.position.Y / 160f);
                Projectile.velocity *= num3;
                Projectile.velocity = Projectile.velocity.RotatedBy(num6 * ((float)Math.PI * 2f) * 0.125f * 1f / 60f);
            }

            int num7 = (int)Projectile.ai[0];
            if (Main.npc.IndexInRange(num7) && !Main.npc[num7].CanBeChasedBy(this))
            {
                num7 = -1;
                Projectile.ai[0] = -1f;
                Projectile.netUpdate = true;
            }

            if (num7 == -1)
            {
                int num8 = Projectile.FindTargetWithLineOfSight();
                if (num8 != -1)
                {
                    Projectile.ai[0] = num8;
                    Projectile.netUpdate = true;
                }
            }

            if (flag2)
            {
                int num9 = (int)Projectile.ai[0];
                Vector2 value3 = Projectile.velocity;

                if (Main.npc.IndexInRange(num9))
                {
                    if (Projectile.timeLeft < 10)
                        Projectile.timeLeft = 10;

                    NPC nPC = Main.npc[num9];
                    value3 = Projectile.DirectionTo(nPC.Center) * num4;
                }
                else
                {
                    Projectile.timeLeft--;
                }

                float amount = MathHelper.Lerp(value, value2, Utils.GetLerpValue(readyTime, 30f, Projectile.timeLeft, clamped: true));
                Projectile.velocity = Vector2.SmoothStep(Projectile.velocity, value3, amount);
                Projectile.velocity *= MathHelper.Lerp(0.85f, 1f, Utils.GetLerpValue(0f, 90f, Projectile.timeLeft, clamped: true));
            }

            Projectile.Opacity = Utils.GetLerpValue(240f, 220f, Projectile.timeLeft, clamped: true);
            Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2f;
        }

        public override void OnKill(int timeLeft)
        {
            Color color = NightmarePlantera.nightmareRed;
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            Vector2 target2 = Projectile.Center;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 vector6 = Projectile.oldPos[i];
                if (vector6 == Vector2.Zero)
                    break;

                int num27 = Main.rand.Next(1, 3);
                float num28 = MathHelper.Lerp(0.3f, 1f, Utils.GetLerpValue(Projectile.oldPos.Length, 0f, i, clamped: true));
                if (i >= Projectile.oldPos.Length * 0.3f)
                    num27--;

                if (i >= Projectile.oldPos.Length * 0.75f)
                    num27 -= 2;

                vector6.DirectionTo(target2).SafeNormalize(Vector2.Zero);
                target2 = vector6;
                for (float j = 0f; j < num27; j++)
                {
                    int index = Dust.NewDust(vector6, Projectile.width, Projectile.height, DustID.RainbowMk2, 0f, 0f, 0, color);
                    Dust dust2 = Main.dust[index];
                    dust2.velocity *= Main.rand.NextFloat() * 0.8f;
                    Main.dust[index].noGravity = true;
                    Main.dust[index].scale = 0.9f + Main.rand.NextFloat() * 1.2f;
                    Main.dust[index].fadeIn = Main.rand.NextFloat() * 1.2f * num28;
                    dust2 = Main.dust[index];
                    dust2.scale *= num28;
                    if (index != 6000)
                    {
                        Dust dust12 = Dust.CloneDust(index);
                        dust2 = dust12;
                        dust2.scale /= 2f;
                        dust2 = dust12;
                        dust2.fadeIn *= 0.85f;
                        dust12.color = new Color(255, 255, 255, 255);
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.7f);
            int num45 = Projectile.FindTargetWithLineOfSight();
            if (num45 != -1)
            {
                Projectile.ai[0] = num45;
                Projectile.netUpdate = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //简易撞墙反弹
            float newVelX = Math.Abs(Projectile.velocity.X);
            float newVelY = Math.Abs(Projectile.velocity.Y);
            float oldVelX = Math.Abs(oldVelocity.X);
            float oldVelY = Math.Abs(oldVelocity.Y);
            if (oldVelX > newVelX)
                Projectile.velocity.X = -oldVelX;
            if (oldVelY > newVelY)
                Projectile.velocity.Y = -oldVelY;

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D extraTex = TextureAssets.Extra[98].Value;
            Main.instance.LoadProjectile(931);

            Texture2D mainTex = TextureAssets.Projectile[931].Value;
            Vector2 center = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

            Color baseColor = NightmarePlantera.nightmareRed;
            Color OtherColor = baseColor;
            Color color37 = baseColor * 0.5f;

            Vector2 extraOrigin = extraTex.Size() / 2f;

            Vector2 origin7 = extraTex.Size() / 2f;
            float num165 = Utils.GetLerpValue(15f, 30f, Projectile.timeLeft, clamped: true) * Utils.GetLerpValue(240f, 200f, Projectile.timeLeft, clamped: true) * (1f + 0.2f * (float)Math.Cos(Main.GlobalTimeWrappedHourly % 30f / 0.5f * ((float)Math.PI * 2f) * 3f)) * 0.8f;
            Vector2 vector31 = new Vector2(0.5f, 5f) * num165;
            Vector2 vector32 = new Vector2(0.5f, 2f) * num165;
            OtherColor *= num165;
            color37 *= num165;
            int num166 = 0;
            Vector2 position4 = center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 0.5f * num166;
            vector31 *= 0.4f;
            vector32 *= 0.4f;

            Vector2 toCenter = new(Projectile.width / 2, Projectile.height / 2);

            //残影
            for (int i = 1; i < 12; i++)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, null,
                    baseColor * (0.5f - i * 0.5f / 12), Projectile.oldRot[i], mainTex.Size() / 2, Projectile.scale, 0, 0);

            //一闪一闪的光
            Main.EntitySpriteDraw(extraTex, position4, null, OtherColor, (float)Math.PI / 2f, origin7, vector31, 0);
            Main.EntitySpriteDraw(extraTex, position4, null, OtherColor, 0f, origin7, vector32, 0);
            //Main.EntitySpriteDraw(extraTex, position4, null, color37, (float)Math.PI / 2f, origin7, vector31 * 0.6f, 0);
            //Main.EntitySpriteDraw(extraTex, position4, null, color37, 0f, origin7, vector32 * 0.6f, 0);

            //额外星星
            Helper.DrawPrettyStarSparkle(Projectile.Opacity, 0, center, NightmarePlantera.nightPurple, NightmarePlantera.lightPurple, 0.5f, 0, 0.5f, 0.5f, 1, Projectile.rotation, new Vector2(0.6f, 1.2f), Vector2.One);

            //主帖图
            Main.EntitySpriteDraw(extraTex, center, null, baseColor, Projectile.rotation, extraOrigin, Projectile.scale * 0.9f, 0);
            Main.EntitySpriteDraw(mainTex, center, null, Color.White, Projectile.rotation, mainTex.Size() / 2, Projectile.scale * 0.9f, 0);

            Color c = NightmarePlantera.lightPurple;
            Vector2 exOffset = (Projectile.rotation + MathHelper.Pi).ToRotationVector2() * 4;
            Main.EntitySpriteDraw(mainTex, center + exOffset, null, c, Projectile.rotation - 0.4f, mainTex.Size() / 2, Projectile.scale * 0.8f, 0);
            //exOffset = (Projectile.rotation + MathHelper.PiOver2 * 3 / 2).ToRotationVector2() * 6;
            Main.EntitySpriteDraw(mainTex, center - exOffset, null, c, Projectile.rotation + 0.4f, mainTex.Size() / 2, Projectile.scale * 0.7f, 0);

            return false;
        }
    }
}
