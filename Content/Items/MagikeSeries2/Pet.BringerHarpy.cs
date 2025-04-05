using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.Creative;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class UnsentLetter : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ModContent.ProjectileType<BringerHarpy>(), ModContent.BuffType<BringerHarpyBuff>());
            Item.width = 28;
            Item.height = 20;
            Item.rare = ModContent.RarityType<CrystallineMagikeRarity>();
            Item.value = Item.sellPrice(0, 5);
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 15, true, false);
            }
        }
    }

    public class BringerHarpy:ModProjectile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public ref float State => ref Projectile.ai[0];
        public ref float Recorder => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[2];

        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
            Main.projFrames[Type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.timeLeft = 100;
        }

        public override bool MinionContactDamage() => false;

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (!owner.active)
            {
                Projectile.active = false;
                return;
            }

            if (owner.dead)
                owner.ClearBuff(ModContent.BuffType<BringerHarpyBuff>());

            if (owner.HasBuff<BringerHarpyBuff>())
                Projectile.timeLeft = 2;

            switch (State)
            {
                default:
                case 0://在玩家身边飞
                    {
                        FlyMovement(owner);

                        Timer++;
                        if (Timer > 60)
                        {
                            Timer = 0;
                            int? itemIndex = FindItem(owner);
                            if (itemIndex!=null)
                            {
                                Main.item[itemIndex.Value].noGrabDelay = 2;
                                State = 1;
                                Recorder = itemIndex.Value;
                                Timer = 0;
                            }
                        }
                    }
                    break;
                case 1://飞到物品旁边捡回来
                    {
                        Vector2 toPlayer = owner.Center - Projectile.Center;
                        float lengthToPlayer = toPlayer.Length();
                        if (lengthToPlayer > 2000f)
                            Projectile.Center = owner.Center;

                        Item i = Main.item[(int)Recorder];
                        Vector2 targetPos = i.Top - new Vector2(0, Projectile.height / 2);
                        float distance = Projectile.Center.Distance(targetPos);
                        if (i.IsAir || distance > 1500 || Timer > 60 * 20)
                        {
                            State = 0;
                            Recorder = 0;
                            Timer = 0;
                            break;
                        }

                        i.noGrabDelay = 2;

                        if (distance < 12)
                        {
                            State = 2;
                            Timer = 0;
                            break;
                        }

                        Timer++;
                        Vector2 dir = targetPos - Projectile.Center;

                        float velRot = Projectile.velocity.ToRotation();
                        float targetRot = dir.ToRotation();

                        float speed = Projectile.velocity.Length();
                        float aimSpeed =3+ Math.Clamp(dir.Length() / 1000f, 0, 1) * 8;

                        Projectile.velocity = velRot.AngleTowards(targetRot, 0.25f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.25f);
                    }
                    break;
                case 2:
                    {
                        Vector2 toPlayer = owner.Center - Projectile.Center;
                        float lengthToPlayer = toPlayer.Length();
                        if (lengthToPlayer > 2000f)
                            Projectile.Center = owner.Center;

                        Item i = Main.item[(int)Recorder];
                        Vector2 targetPos = i.Top - new Vector2(0, Projectile.height / 2);
                        float distance = Projectile.Center.Distance(targetPos);
                        if (i.IsAir || distance > 1500 || Timer > 60 * 20)
                        {
                            State = 0;
                            Recorder = 0;
                            Timer = 0;
                            break;
                        }

                        i.noGrabDelay = 2;
                        float distanceToOwner = Projectile.Center.Distance(owner.Center);

                        if (distanceToOwner < Player.defaultItemGrabRange - 16)
                        {
                            State = 0;
                            Recorder = 0;
                            Timer = 0;
                            break;
                        }

                        Timer++;
                        Vector2 dir = owner.Center - Projectile.Center;

                        float velRot = Projectile.velocity.ToRotation();
                        float targetRot = dir.ToRotation();

                        float speed = Projectile.velocity.Length();
                        float aimSpeed = 3 + Math.Clamp(dir.Length() / 1000f, 0, 1) * 12;
                        Projectile.velocity = velRot.AngleTowards(targetRot, 0.25f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.25f);

                        i.Center = Vector2.SmoothStep(i.Center, Projectile.Bottom, Math.Clamp(Timer / 15, 0, 1));
                    }
                    break;
            }

            if (Projectile.velocity.X != 0 && MathF.Abs(Projectile.Center.X - owner.Center.X) > 8)
                Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);

            Projectile.frameCounter += ((int)Math.Abs(Projectile.velocity.X) / 6) + 1;

            if (Projectile.frameCounter > 7)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }

            if (Projectile.frame > 5)
                Projectile.frame = 0;

            Projectile.rotation = Projectile.velocity.X / 25;
        }

        private void FlyMovement(Player player)
        {
            Projectile.tileCollide = false;
            float acc = 0.2f;//加速度
            float num18 = 10f;
            int num19 = 200;
            if (num18 < Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y))
                num18 = Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y);

            Vector2 toPlayer = player.Center - Projectile.Center;
            float lengthToPlayer = toPlayer.Length();
            if (lengthToPlayer > 2000f)
                Projectile.Center = player.Center;

            if (lengthToPlayer < num19 && player.velocity.Y == 0f && Projectile.position.Y + Projectile.height <= player.position.Y + player.height && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.netUpdate = true;
                if (Projectile.velocity.Y < -6f)
                    Projectile.velocity.Y = -6f;
            }

            if (!(lengthToPlayer < 60f))
            {
                toPlayer.SafeNormalize(Vector2.Zero);
                toPlayer *= num18;
                if (Projectile.velocity.X < toPlayer.X)
                {
                    Projectile.velocity.X += acc;
                    if (Projectile.velocity.X < 0f)
                        Projectile.velocity.X += acc * 2.5f;
                }

                if (Projectile.velocity.X > toPlayer.X)
                {
                    Projectile.velocity.X -= acc;
                    if (Projectile.velocity.X > 0f)
                        Projectile.velocity.X -= acc * 2.5f;
                }

                if (Projectile.velocity.Y < toPlayer.Y)
                {
                    Projectile.velocity.Y += acc;
                    if (Projectile.velocity.Y < 0f)
                        Projectile.velocity.Y += acc * 2.5f;
                }

                if (Projectile.velocity.Y > toPlayer.Y)
                {
                    Projectile.velocity.Y -= acc;
                    if (Projectile.velocity.Y > 0f)
                        Projectile.velocity.Y -= acc * 2.5f;
                }
            }
        }

        public int? FindItem(Player player)
        {
            for (int i = 0; i < Main.maxItems; i++)
            {
                Item item = Main.item[i];
                if (item == null || item.IsAir)
                    continue;

                int range = player.GetItemGrabRange(item);
                float distance = Vector2.Distance(item.Center, player.Center);
                if (distance > range + 16 * 12 && distance < 1500)
                    return i;
            }

            return null;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.QuickDraw(Projectile.GetTexture().Frame(1, 6, 0, Projectile.frame)
                , Projectile.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, lightColor, 0);
            return false;
        }
    }

    public class BringerHarpyBuff : ModBuff
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;

            int projType = ModContent.ProjectileType<BringerHarpy>();

            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[projType] <= 0)
            {
                var entitySource = player.GetSource_Buff(buffIndex);
                Projectile.NewProjectile(entitySource, player.Center, Vector2.Zero, projType, 0, 0f, player.whoAmI);
            }
        }
    }
}
