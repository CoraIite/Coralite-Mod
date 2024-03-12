using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Misc_Summon
{
    public class CrystalBlossomShards : ModItem
    {
        public override string Texture => AssetDirectory.Misc_Summon + Name;

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ModContent.ProjectileType<Kitsune>(), ModContent.BuffType<KitsuneBuff>());
            Item.damage = 40;
            Item.width = 28;
            Item.height = 20;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(0, 50);
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 15, true, false);
            }
        }
    }

    public class KitsuneBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Misc_Summon + Name;

        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;

            int projType = ModContent.ProjectileType<Kitsune>();

            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[projType] <= 0)
            {
                var entitySource = player.GetSource_Buff(buffIndex);
                Projectile.NewProjectile(entitySource, player.Center, Vector2.Zero, projType, player.miscEquips[0].damage, 0f, player.whoAmI);
            }
        }
    }

    public class Kitsune : ModProjectile
    {
        public override string Texture => AssetDirectory.Misc_Summon + Name;

        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
            Main.projFrames[Type] = 15;
        }

        public override void SetDefaults()
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

        public override bool MinionContactDamage() => true;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active)
            {
                Projectile.active = false;
                return;
            }

            int num = 800;
            float num2 = 500f;
            float num3 = 300f;

            if (player.dead)
                player.ClearBuff(ModContent.BuffType<KitsuneBuff>());

            if (player.HasBuff<KitsuneBuff>())
                Projectile.timeLeft = 2;

            Vector2 vector = player.Center;
            if (player.direction>0)
                vector.X -= (40) * player.direction;
            else
                vector.X -= (45 + player.width) * player.direction;

            Projectile.shouldFallThrough = player.position.Y + player.height - 12f > Projectile.position.Y + (float)Projectile.height;
            Projectile.friendly = false;
            int num8 = 0;
            int num9 = 20;
            int attackTarget = -1;
            bool flag10 = Projectile.ai[0] == 5f;
            bool flag11 = Projectile.ai[0] == 0f;

            if (flag11)
                Projectile.Minion_FindTargetInRange(num, ref attackTarget, skipIfCannotHitWithOwnBody: true);

            if (Projectile.ai[0] == 1f)
            {
                Projectile.tileCollide = false;
                float num17 = 0.2f;
                float num18 = 10f;
                int num19 = 200;
                if (num18 < Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y))
                    num18 = Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y);

                Vector2 vector6 = player.Center - Projectile.Center;
                float num20 = vector6.Length();
                if (num20 > 2000f)
                    Projectile.position = player.Center - new Vector2(Projectile.width, Projectile.height) / 2f;

                if (num20 < num19 && player.velocity.Y == 0f && Projectile.position.Y + Projectile.height <= player.position.Y + player.height && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                {
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                    if (Projectile.velocity.Y < -6f)
                        Projectile.velocity.Y = -6f;
                }

                if (!(num20 < 60f))
                {
                    vector6.Normalize();
                    vector6 *= num18;
                    if (Projectile.velocity.X < vector6.X)
                    {
                        Projectile.velocity.X += num17;
                        if (Projectile.velocity.X < 0f)
                            Projectile.velocity.X += num17 * 1.5f;
                    }

                    if (Projectile.velocity.X > vector6.X)
                    {
                        Projectile.velocity.X -= num17;
                        if (Projectile.velocity.X > 0f)
                            Projectile.velocity.X -= num17 * 1.5f;
                    }

                    if (Projectile.velocity.Y < vector6.Y)
                    {
                        Projectile.velocity.Y += num17;
                        if (Projectile.velocity.Y < 0f)
                            Projectile.velocity.Y += num17 * 1.5f;
                    }

                    if (Projectile.velocity.Y > vector6.Y)
                    {
                        Projectile.velocity.Y -= num17;
                        if (Projectile.velocity.Y > 0f)
                            Projectile.velocity.Y -= num17 * 1.5f;
                    }
                }

                if (Projectile.velocity.X != 0f)
                    Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);

                Projectile.frameCounter++;
                if (Projectile.frameCounter > 3)
                {
                    Projectile.frame++;
                    Projectile.frameCounter = 0;
                }

                if ((Projectile.frame < 12) | (Projectile.frame > 13))
                    Projectile.frame = 12;

                Lighting.AddLight(Projectile.Center, new Vector3(0.4f, 0.3f, 0.3f));
                if (Main.rand.NextBool())
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center+Main.rand.NextVector2Circular(6,6) + new Vector2(-Projectile.spriteDirection * 14, 14).RotatedBy(Projectile.rotation), DustID.Firework_Pink,
                         -Projectile.velocity*Main.rand.NextFloat(0.3f,0.6f),50,Scale:Main.rand.NextFloat(0.7f,1f));
                    d.noGravity = true;
                }
                Projectile.rotation = Projectile.velocity.X * 0.05f;
            }

            if (Projectile.ai[0] == 2f && Projectile.ai[1] < 0f)
            {
                Projectile.friendly = false;
                Projectile.ai[1] += 1f;
                if (num9 >= 0)
                {
                    Projectile.ai[1] = 0f;
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                    return;
                }
            }
            else if (Projectile.ai[0] == 2f)
            {
                Projectile.spriteDirection = Projectile.direction;
                Projectile.rotation = 0f;
                Projectile.friendly = true;
                Projectile.frame = 4 + (int)(num9 - Projectile.ai[1]) / (num9 / 3);
                //Main.NewText(Math.Abs(Projectile.velocity.X));
                if (Math.Abs(Projectile.velocity.X) > 4.9f)
                    Projectile.frame += 4;

                Projectile.velocity.Y += 0.4f;
                if (Projectile.velocity.Y > 10f)
                    Projectile.velocity.Y = 10f;

                //Projectile.velocity.X *= 0.7f;
                Projectile.ai[1] -= 1f;
                if (Projectile.ai[1] <= 0f)
                {
                    if (num8 <= 0)
                    {
                        Projectile.ai[1] = 0f;
                        Projectile.ai[0] = 0f;
                        Projectile.netUpdate = true;
                        return;
                    }

                    Projectile.ai[1] = -num8;
                }
            }

            if (attackTarget >= 0)
            {
                float maxDistance2 = num;
                float num25 = 20f;

                NPC nPC2 = Main.npc[attackTarget];
                Vector2 center = nPC2.Center;
                vector = center;
                if (Projectile.IsInRangeOfMeOrMyOwner(nPC2, maxDistance2, out var _, out var _, out var _))
                {
                    Projectile.shouldFallThrough = nPC2.Center.Y > Projectile.Bottom.Y;
                    bool flag12 = Projectile.velocity.Y == 0f;
                    if (Projectile.wet && Projectile.velocity.Y > 0f && !Projectile.shouldFallThrough)
                        flag12 = true;

                    if (center.Y < Projectile.Center.Y - 30f && flag12)
                    {
                        float num26 = (center.Y - Projectile.Center.Y) * -1f;
                        float num27 = 0.4f;
                        float num28 = (float)Math.Sqrt(num26 * 2f * num27);
                        if (num28 > 26f)
                            num28 = 26f;

                        Projectile.velocity.Y = 0f - num28;
                    }

                    if (Vector2.Distance(Projectile.Center, vector) < num25)
                    {
                        if (Projectile.velocity.Length() > 10f)
                            Projectile.velocity /= Projectile.velocity.Length() / 10f;

                        Projectile.ai[0] = 2f;
                        Projectile.ai[1] = num9;
                        Projectile.netUpdate = true;
                        Projectile.direction = ((center.X - Projectile.Center.X > 0f) ? 1 : (-1));
                    }
                }
            }

            if (Projectile.ai[0] == 0f && attackTarget < 0)
            {
                if (Main.player[Projectile.owner].rocketDelay2 > 0)
                {
                    Projectile.ai[0] = 1f;
                    Projectile.netUpdate = true;
                }

                Vector2 vector7 = player.Center - Projectile.Center;
                if (vector7.Length() > 2000f)
                {
                    Projectile.position = player.Center - new Vector2(Projectile.width, Projectile.height) / 2f;
                }
                else if (vector7.Length() > num2 || Math.Abs(vector7.Y) > num3)
                {
                    Projectile.ai[0] = 1f;
                    Projectile.netUpdate = true;
                    if (Projectile.velocity.Y > 0f && vector7.Y < 0f)
                        Projectile.velocity.Y = 0f;

                    if (Projectile.velocity.Y < 0f && vector7.Y > 0f)
                        Projectile.velocity.Y = 0f;
                }
            }

            if (Projectile.ai[0] == 0f)
            {
                if (attackTarget < 0)
                {
                    if (Projectile.Distance(player.Center) > 60f && Projectile.Distance(vector) > 60f && Math.Sign(vector.X - player.Center.X) != Math.Sign(Projectile.Center.X - player.Center.X))
                        vector = player.Center;

                    Rectangle r = Utils.CenteredRectangle(vector, Projectile.Size);
                    for (int i = 0; i < 20; i++)
                    {
                        if (Collision.SolidCollision(r.TopLeft(), r.Width, r.Height))
                            break;

                        r.Y += 16;
                        vector.Y += 16f;
                    }

                    Vector2 vector8 = Collision.TileCollision(player.Center - Projectile.Size / 2f, vector - player.Center, Projectile.width, Projectile.height);
                    vector = player.Center - Projectile.Size / 2f + vector8;
                    if (Projectile.Distance(vector) < 32f)
                    {
                        float num32 = player.Center.Distance(vector);
                        if (player.Center.Distance(Projectile.Center) < num32)
                            vector = Projectile.Center;
                    }

                    Vector2 vector9 = player.Center - vector;
                    if (vector9.Length() > num2 || Math.Abs(vector9.Y) > num3)
                    {
                        Rectangle r2 = Utils.CenteredRectangle(player.Center, Projectile.Size);
                        Vector2 vector10 = vector - player.Center;
                        Vector2 vector11 = r2.TopLeft();
                        for (float num33 = 0f; num33 < 1f; num33 += 0.05f)
                        {
                            Vector2 vector12 = r2.TopLeft() + vector10 * num33;
                            if (Collision.SolidCollision(r2.TopLeft() + vector10 * num33, r.Width, r.Height))
                                break;

                            vector11 = vector12;
                        }

                        vector = vector11 + Projectile.Size / 2f;
                    }
                }

                Projectile.tileCollide = true;
                float num34 = 0.5f;
                float num35 = 4f;
                float num36 = 4f;
                float num37 = 0.1f;

                if (attackTarget != -1)
                {
                    num34 = 1f;
                    num35 = 8f;
                    num36 = 8f;
                }

                if (num36 < Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y))
                {
                    num36 = Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y);
                    num34 = 0.7f;
                }

                int num39 = 0;
                bool flag13 = false;
                float num40 = vector.X - Projectile.Center.X;
                Vector2 vector13 = vector - Projectile.Center;
                if (Math.Abs(num40) > 5f)
                {
                    if (num40 < 0f)
                    {
                        num39 = -1;
                        if (Projectile.velocity.X > 0f - num35)
                            Projectile.velocity.X -= num34;
                        else
                            Projectile.velocity.X -= num37;
                    }
                    else
                    {
                        num39 = 1;
                        if (Projectile.velocity.X < num35)
                            Projectile.velocity.X += num34;
                        else
                            Projectile.velocity.X += num37;
                    }
                }
                else
                {
                    Projectile.velocity.X *= 0.9f;
                    if (Math.Abs(Projectile.velocity.X) < num34 * 2f)
                        Projectile.velocity.X = 0f;
                }

                bool flag15 = Math.Abs(vector13.X) >= 64f || (vector13.Y <= -48f && Math.Abs(vector13.X) >= 8f);
                if (num39 != 0 && flag15)
                {
                    int num41 = (int)(Projectile.position.X + (Projectile.width / 2)) / 16;
                    int num42 = (int)Projectile.position.Y / 16;
                    num41 += num39;
                    num41 += (int)Projectile.velocity.X;
                    for (int j = num42; j < num42 + Projectile.height / 16 + 1; j++)
                    {
                        if (WorldGen.SolidTile(num41, j))
                            flag13 = true;
                    }
                }

                Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);
                float num43 = Utils.GetLerpValue(0f, 100f, vector13.Y, clamped: true) * Utils.GetLerpValue(-2f, -6f, Projectile.velocity.Y, clamped: true);
                if (Projectile.velocity.Y == 0f)
                {
                    if (flag13)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            int num44 = (int)(Projectile.position.X + (Projectile.width / 2)) / 16;
                            if (k == 0)
                                num44 = (int)Projectile.position.X / 16;

                            if (k == 2)
                                num44 = (int)(Projectile.position.X + Projectile.width) / 16;

                            int num45 = (int)(Projectile.position.Y + Projectile.height) / 16;
                            if (!WorldGen.SolidTile(num44, num45) && !Main.tile[num44, num45].IsHalfBlock && Main.tile[num44, num45].Slope <= 0 && (!TileID.Sets.Platforms[Main.tile[num44, num45].TileType]))
                                continue;

                            try
                            {
                                num44 = (int)(Projectile.position.X + (Projectile.width / 2)) / 16;
                                num45 = (int)(Projectile.position.Y + (Projectile.height / 2)) / 16;
                                num44 += num39;
                                num44 += (int)Projectile.velocity.X;
                                if (!WorldGen.SolidTile(num44, num45 - 1) && !WorldGen.SolidTile(num44, num45 - 2))
                                    Projectile.velocity.Y = -5.1f;
                                else if (!WorldGen.SolidTile(num44, num45 - 2))
                                    Projectile.velocity.Y = -7.1f;
                                else if (WorldGen.SolidTile(num44, num45 - 5))
                                    Projectile.velocity.Y = -11.1f;
                                else if (WorldGen.SolidTile(num44, num45 - 4))
                                    Projectile.velocity.Y = -10.1f;
                                else
                                    Projectile.velocity.Y = -9.1f;
                            }
                            catch
                            {
                                Projectile.velocity.Y = -9.1f;
                            }
                        }

                        if (vector.Y - Projectile.Center.Y < -48f)
                        {
                            float num46 = vector.Y - Projectile.Center.Y;
                            num46 *= -1f;
                            if (num46 < 60f)
                                Projectile.velocity.Y = -6f;
                            else if (num46 < 80f)
                                Projectile.velocity.Y = -7f;
                            else if (num46 < 100f)
                                Projectile.velocity.Y = -8f;
                            else if (num46 < 120f)
                                Projectile.velocity.Y = -9f;
                            else if (num46 < 140f)
                                Projectile.velocity.Y = -10f;
                            else if (num46 < 160f)
                                Projectile.velocity.Y = -11f;
                            else if (num46 < 190f)
                                Projectile.velocity.Y = -12f;
                            else if (num46 < 210f)
                                Projectile.velocity.Y = -13f;
                            else if (num46 < 270f)
                                Projectile.velocity.Y = -14f;
                            else if (num46 < 310f)
                                Projectile.velocity.Y = -15f;
                            else
                                Projectile.velocity.Y = -16f;
                        }

                        if (Projectile.wet && num43 == 0f)
                            Projectile.velocity.Y *= 2f;
                    }
                }

                if (Projectile.velocity.X > num36)
                    Projectile.velocity.X = num36;

                if (Projectile.velocity.X < 0f - num36)
                    Projectile.velocity.X = 0f - num36;

                if (Projectile.velocity.X < 0f)
                    Projectile.direction = -1;

                if (Projectile.velocity.X > 0f)
                    Projectile.direction = 1;

                if (Projectile.velocity.X == 0f)
                    Projectile.direction = ((player.Center.X > Projectile.Center.X) ? 1 : (-1));

                if (Projectile.velocity.X > num34 && num39 == 1)
                    Projectile.direction = 1;

                if (Projectile.velocity.X < 0f - num34 && num39 == -1)
                    Projectile.direction = -1;

                Projectile.spriteDirection = Projectile.direction;

                Projectile.rotation = 0f;
                if (Projectile.velocity.Y == 0f)
                {
                    if (Projectile.velocity.X == 0f)
                    {
                        Projectile.frame = 0;
                        Projectile.frameCounter = 0;
                    }
                    else if (Math.Abs(Projectile.velocity.X) >= 0.5f)
                    {
                        Projectile.frameCounter += (int)Math.Abs(Projectile.velocity.X);
                        if (++Projectile.frameCounter > 10)
                        {
                            Projectile.frame++;
                            Projectile.frameCounter = 0;
                        }

                        if (Projectile.frame >= 4)
                            Projectile.frame = 0;
                    }
                    else
                    {
                        Projectile.frame = 0;
                        Projectile.frameCounter = 0;
                    }
                }
                else if (Projectile.velocity.Y != 0f)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame = 14;
                }

                Projectile.velocity.Y += 0.4f + num43 * 1f;
                if (Projectile.velocity.Y > 10f)
                    Projectile.velocity.Y = 10f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();

            var pos = Projectile.Center - Main.screenPosition;
            var frameBox = mainTex.Frame(1, 15, 0, Projectile.frame);
            var origin = frameBox.Size() / 2;
            var effect = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, Projectile.rotation, origin, Projectile.scale, effect, 0);
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture + "Glow").Value, pos, frameBox, Color.White, Projectile.rotation, origin, Projectile.scale, effect, 0);

            return false;
        }
    }
}
