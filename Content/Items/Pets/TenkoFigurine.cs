using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.UI.Chat;

namespace Coralite.Content.Items.Pets
{
    public class TenkoFigurine : ModItem
    {
        public override string Texture => AssetDirectory.PetItems + Name;

        private static PrimitivePRTGroup group;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<CrystalBlossomShards>();
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ModContent.ProjectileType<Tenko>(), ModContent.BuffType<TenkoBuff>());
            Item.damage = 20;
            Item.width = 28;
            Item.height = 20;
            Item.rare = ModContent.RarityType<TenkoFigurineRarity>();
            Item.value = Item.sellPrice(0, 50);
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 15, true, false);
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            group?.Update();
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName")
            {
                Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "HorizontalLight").Value;

                Vector2 origin = new(0, mainTex.Height / 2);
                Color c = Color.SkyBlue;
                c.A = 0;
                c *= 0.25f + (MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.2f);

                for (int i = 0; i < 5; i++)
                {
                    Main.spriteBatch.Draw(mainTex, new Vector2(line.X - 10, line.Y), null, c,
                        i * 0.22f, origin, 0.7f - (i * 0.17f), 0, 0);
                }
            }

            return true;
        }

        public override void PostDrawTooltipLine(DrawableTooltipLine line)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName")
            {
                group ??= new PrimitivePRTGroup();
                if (group != null)
                {
                    if (!Main.gamePaused && Main.GameUpdateCount % 20 == 0)
                    {
                        Vector2 size = ChatManager.GetStringSize(line.Font, line.Text, line.BaseScale);
                        group.NewParticle(new Vector2(line.X, line.Y) + new Vector2(Main.rand.NextFloat(0, size.X), Main.rand.Next(-8, 0)),
                            Main.rand.NextFloat(0.585f - 0.3f, 0.585f + 0.3f).ToRotationVector2() * Main.rand.NextFloat(0.2f, 0.5f), CoraliteContent.ParticleType<Petal>()
                            , Color.SkyBlue, Main.rand.NextFloat(0.8f, 1f));
                    }
                }
                group?.DrawInUI(Main.spriteBatch);
            }
        }
    }

    public class TenkoFigurineRarity : ModRarity
    {
        public override Color RarityColor => Color.Lerp(new Color(228, 254, 255), new Color(152, 176, 255), Math.Abs(MathF.Sin(Main.GlobalTimeWrappedHourly * 3)));
    }

    public class TenkoBuff : ModBuff
    {
        public override string Texture => AssetDirectory.PetItems + Name;

        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;

            int projType = ModContent.ProjectileType<Tenko>();

            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[projType] <= 0)
            {
                var entitySource = player.GetSource_Buff(buffIndex);
                Projectile.NewProjectile(entitySource, player.Center, Vector2.Zero, projType, player.miscEquips[0].damage, 0f, player.whoAmI);
            }
        }
    }

    public class Tenko : BasePetProj
    {
        public override string Texture => AssetDirectory.PetItems + Name;
        protected override int PetBuffType => ModContent.BuffType<TenkoBuff>();

        public ref float State => ref Projectile.ai[0];
        public ref float Recorder => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[2];

        protected override void SetPetStaticDefaults()
        {
            Main.projFrames[Type] = 15;
        }

        protected override void SetPetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 34;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.timeLeft = 100;
            Projectile.minion = true;
            //Projectile.minionSlots = 1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 18;
            Projectile.decidesManualFallThrough = true;
        }

        public override bool MinionContactDamage() => false;

        public override void AI()
        {
            const int AttackLength = 170;

            Player player = Main.player[Projectile.owner];
            if (!player.active)
            {
                Projectile.active = false;
                return;
            }

            float num2 = 500f;
            float num3 = 300f;

            if (player.dead)
                player.ClearBuff(ModContent.BuffType<TenkoBuff>());

            if (player.HasBuff<TenkoBuff>())
                Projectile.timeLeft = 2;

            Vector2 vector = player.Center;
            if (player.direction > 0)
                vector.X -= 40 * player.direction;
            else
                vector.X -= (45 + player.width) * player.direction;

            Projectile.shouldFallThrough = player.position.Y + player.height - 12f > Projectile.position.Y + Projectile.height;
            Projectile.friendly = false;

            switch (State)
            {
                default:
                case 0:
                    {
                        Vector2 vector7 = player.Center - Projectile.Center;
                        if (vector7.Length() > 2000f)
                            Projectile.position = player.Center - (new Vector2(Projectile.width, Projectile.height) / 2f);
                        else if (vector7.Length() > num2 || Math.Abs(vector7.Y) > num3)
                        {
                            State = 1f;
                            Projectile.netUpdate = true;
                            if (Projectile.velocity.Y > 0f && vector7.Y < 0f)
                                Projectile.velocity.Y = 0f;

                            if (Projectile.velocity.Y < 0f && vector7.Y > 0f)
                                Projectile.velocity.Y = 0f;
                        }

                        if (Projectile.velocity.Y != 0)
                            Projectile.frame = 14;
                        else if (Math.Abs(Projectile.velocity.X) != 0)
                        {
                            Projectile.frameCounter += ((int)Math.Abs(Projectile.velocity.X) / 3) + 1;

                            if (Projectile.frameCounter > 7)
                            {
                                Projectile.frame++;
                                Projectile.frameCounter = 0;
                            }

                            if (Projectile.frame > 3)
                                Projectile.frame = 0;
                        }
                        else
                        {
                            Projectile.rotation = 0f;
                            Projectile.frame = 0;
                        }

                        GroundMovement(player.Center, maxSpeed: Math.Max(4f, Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y)),
                            acceleration: Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y) > 4f ? 0.7f : 0.5f,
                            followOffset: new Vector2(player.direction > 0 ? -40f : 45f + player.width, 0f));

                        Timer++;
                        if (Timer > 120)
                        {
                            Timer = 0;
                            if (Helper.TryFindClosestEnemy(Projectile.Center, AttackLength, n => n.CanBeChasedBy(), out _))
                            {
                                State = 2;
                                Recorder = 0;
                            }
                        }
                    }
                    break;
                case 1://飞行
                    {
                        FlyMovement(player);

                        Timer++;
                        if (Timer > 120)
                        {
                            Timer = 0;
                            if (Main.hardMode && Helper.TryFindClosestEnemy(Projectile.Center, AttackLength, n => n.CanBeChasedBy(), out _))
                            {
                                State = 2;
                                Recorder = 1;
                            }
                        }

                        Projectile.frameCounter++;
                        if (Projectile.frameCounter > 4)
                        {
                            Projectile.frame++;
                            Projectile.frameCounter = 0;
                        }

                        if (Projectile.frame > 11 || Projectile.frame < 10)
                            Projectile.frame = 10;

                        Lighting.AddLight(Projectile.Center, new Vector3(0.3f, 0.3f, 0.5f));
                        if (Main.rand.NextBool())
                        {
                            Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(6, 6) + new Vector2(-Projectile.spriteDirection * 14, 14).RotatedBy(Projectile.rotation), DustID.FireworkFountain_Blue,
                                 -Projectile.velocity * Main.rand.NextFloat(0.3f, 0.6f), 50, Scale: Main.rand.NextFloat(0.7f, 1f));
                            d.noGravity = true;
                        }

                        Projectile.rotation = Projectile.velocity.X * 0.05f;
                    }
                    break;
                case 2://攻击
                    {
                        Timer++;
                        Projectile.velocity.X *= 0.9f;

                        Vector2 dustPos = Projectile.Center + Helper.NextVec2Dir(30, 45);
                        Dust d;
                        if (Main.rand.NextBool())
                            d = Dust.NewDustPerfect(dustPos, DustID.Electric,
                             (dustPos - Projectile.Center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.5f, 6f), Scale: Main.rand.NextFloat(0.5f, 0.7f));
                        else
                            d = Dust.NewDustPerfect(dustPos, DustID.Electric,
                             (Projectile.Center - dustPos).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.5f, 2f));
                        d.noGravity = true;

                        if (Timer % 60 == 0)
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                Vector2 baseDir = ((Timer * 0.02f) + (i * MathHelper.TwoPi / 5)).ToRotationVector2();

                                for (int j = -1; j < 2; j += 2)
                                {
                                    Vector2 velocity = baseDir.RotatedBy(j * 0.6f);
                                    Vector2 pos = Projectile.Center + (baseDir * 32);
                                    for (int k = 0; k < 5; k++)
                                    {
                                        ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.ChlorophyteLeafCrystalShot, new ParticleOrchestraSettings
                                        {
                                            PositionInWorld = pos,
                                            MovementVector = velocity * 2,
                                            UniqueInfoPiece = (int)((0.75f - (0.2f / 5 * k)) * 255)
                                        });

                                        pos += velocity * 16;
                                        velocity = velocity.RotatedBy(-j * 0.075f);
                                    }
                                }

                                for (int l = -1; l < 2; l += 2)
                                {
                                    Vector2 velocity = baseDir.RotatedBy(l * 0.95f);
                                    ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.ChlorophyteLeafCrystalShot, new ParticleOrchestraSettings
                                    {
                                        PositionInWorld = Projectile.Center + (baseDir * (32 + (16 * 4) + 6)) + (velocity * 16),
                                        MovementVector = velocity * 3,
                                        UniqueInfoPiece = (int)(0.55f * 255)
                                    });
                                }
                            }

                            for (int i = 0; i < Main.maxNPCs; i++)
                            {
                                NPC n = Main.npc[i];
                                if (n.active && n.CanBeChasedBy() && !n.friendly && Vector2.Distance(n.Center, Projectile.Center) < AttackLength)
                                    n.SimpleStrikeNPC(20, Math.Sign(n.Center.X - Projectile.Center.X));
                            }

                            player.Heal(1);
                        }

                        if (Timer % 7 == 0)
                        {
                            float rot = Timer * 0.02f;
                            for (int i = 0; i < 9; i++)
                            {
                                Vector2 pos = Projectile.Center + ((rot + (i * MathHelper.TwoPi / 9)).ToRotationVector2() * AttackLength);
                                Vector2 vel = (rot + 1.57f + (i * MathHelper.TwoPi / 9)).ToRotationVector2();
                                ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.ChlorophyteLeafCrystalShot, new ParticleOrchestraSettings
                                {
                                    PositionInWorld = pos,
                                    MovementVector = vel * 2,
                                    UniqueInfoPiece = (int)(0.55f * 255)
                                });
                            }
                        }

                        if (Recorder == 0)//普通状态下
                        {
                            Vector2 vector13 = vector - Projectile.Center;

                            float num43 = Utils.GetLerpValue(0f, 100f, vector13.Y, clamped: true) * Utils.GetLerpValue(-2f, -6f, Projectile.velocity.Y, clamped: true);

                            Projectile.velocity.Y += 0.4f + (num43 * 1f);
                            if (Projectile.velocity.Y > 10f)
                                Projectile.velocity.Y = 10f;
                            if (++Projectile.frameCounter > 5)
                            {
                                Projectile.frameCounter = 0;
                                Projectile.frame++;
                            }
                            if (Projectile.frame > 9)
                                Projectile.frame = 4;
                        }
                        else//飞行状态
                        {
                            if (++Projectile.frameCounter > 8)
                            {
                                Projectile.frameCounter = 0;
                                Projectile.frame++;
                            }
                            if (Projectile.frame > 13)
                                Projectile.frame = 12;

                            Projectile.velocity *= 0.98f;
                            Projectile.rotation = Projectile.rotation.AngleLerp(0, 0.1f);
                        }

                        if (Timer > 60 * 5)
                            State = Recorder;
                    }
                    break;
            }
        }

        private void FlyMovement(Player player)
        {
            if (base.FlyMovement(player, 0.2f, 10f, 200f, 60f, 1.5f, 1.5f))
            {
                State = 0f;
                Projectile.netUpdate = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTextureValue();

            var pos = Projectile.Center - Main.screenPosition;
            var frameBox = mainTex.Frame(1, 15, 0, Projectile.frame);
            var origin = frameBox.Size() / 2;
            var effect = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, Projectile.rotation, origin, Projectile.scale, effect, 0);

            return false;
        }
    }
}
