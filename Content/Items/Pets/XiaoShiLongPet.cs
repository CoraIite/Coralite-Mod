using Coralite.Content.Items.AlchorthentSeries;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using System;
using System.Threading;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace Coralite.Content.Items.Pets
{
    public class StickyBicolorCrystal : ModItem
    {
        public override string Texture => AssetDirectory.PetItems + Name;

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ModContent.ProjectileType<XiaoShiLongPetProj>(), ModContent.BuffType<XiaoShiLongPetBuff>());
            Item.width = 28;
            Item.height = 20;
            Item.rare = ModContent.RarityType<XiaoShiLongRarity>();
            Item.value = Item.sellPrice(0, 5);
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.ItemAnimationJustStarted)
                player.AddBuff(Item.buffType, 15, true, false);

            return base.UseItem(player);
        }
    }

    public class XiaoShiLongRarity : ModRarity
    {
        public override Color RarityColor
        {
            get
            {
                const int t1 = 45;

                float t = (int)(Main.timeForVisualEffects) % (t1 * 5);

                if (t < t1)
                {
                    return Color.Lerp(new Color(255, 17, 0), new Color(138, 202, 99), MathF.Sin(t / t1 * MathHelper.PiOver2)/2+0.5f);
                }
                else if (t < t1 * 2)
                {
                    return new Color(138, 202, 99);
                }
                else
                    return Color.Lerp(new Color(255, 17, 0), new Color(138, 202, 99),  MathF.Sin(MathHelper.PiOver2 + (t - t1 * 2) / (t1 * 3) * MathHelper.PiOver2 * 3) / 2 + 0.5f);
            }
        }
    }

    public class XiaoShiLongPetBuff : BasePetBuff<XiaoShiLongPetProj>
    {
    }

    public class XiaoShiLongPetProj : BasePetProj
    {
        public override string Texture => AssetDirectory.PetItems + Name;
        protected override int PetBuffType => ModContent.BuffType<XiaoShiLongPetBuff>();

        /// <summary>0：飞行，1：下落，2：睡觉</summary>
        public ref float State => ref Projectile.ai[0];
        public ref float IdleTimer => ref Projectile.ai[1];
        public ref float FrameX => ref Projectile.ai[2];

        protected override void SetPetStaticDefaults()
        {
            Main.projFrames[Type] = 24;
        }

        protected override void SetPetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.FairyQueenPet);
            Projectile.aiStyle = -1;
            Projectile.width = 32;
            Projectile.height = 38;
            Projectile.tileCollide = false;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            if (!CheckActive(Owner))
                return;

            bool teleported = TeleportToOwner(Owner);
            if (Owner.velocity != Vector2.Zero || teleported)
            {
                IdleTimer = 0;
                if (State != 0)
                    ChangeState(0);
            }

            Projectile.tileCollide = State == 2;
            switch (State)
            {
                default:
                case 0:
                    Fly(Owner);
                    Projectile.UpdateFrameNormally(4, 17);

                    if (Owner.velocity == Vector2.Zero && ++IdleTimer >= 60*5)
                        ChangeState(1);
                    break;
                case 1:
                    Projectile.velocity.X *= 0.95f;
                    Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + 0.2f, 10f);
                    Projectile.rotation *= 0.9f;
                    Projectile.UpdateFrameNormally(4, 17);

                    // 检测脚下的一行物块，实心物块和平台都可以作为睡觉的落点。
                    int tileY = (int)((Projectile.Bottom.Y + 16f) / 16f);
                    int left = (int)(Projectile.Left.X / 16f);
                    int right = (int)((Projectile.Right.X - 1f) / 16f);
                    for (int tileX = left; tileX <= right; tileX++)
                    {
                        if (Framing.GetTileSafely(tileX, tileY).HasSolidTopTile())
                        {
                            ChangeState(2);
                            break;
                        }
                    }
                    break;
                case 2:
                    if (Vector2.DistanceSquared(Projectile.Center,Owner.Center)>(16*15)*16*15)
                    {
                        ChangeState(0);
                        return;
                    }
                    Projectile.velocity.Y += 0.2f;
                    Projectile.rotation = 0;
                    Projectile.UpdateFrameNormally(5, 23);
                    IdleTimer ++;

                    if (!Main.dedServ && IdleTimer%40==0)
                        Dust.NewDustPerfect(Projectile.Top + new Vector2(Projectile.spriteDirection * 14, 20),
                            ModContent.DustType<SleepZZZDust>(), new Vector2(0.2f, -0.5f), 0, Color.White);
                    break;
            }
        }

        private void ChangeState(int state)
        {
            State = state;
            IdleTimer = 0;
            FrameX = state == 2 ? 1 : 0;
            Projectile.tileCollide = state == 2;
            if (state != 1)
            {
                Projectile.frame = 0;
                Projectile.frameCounter = 0;
            }

            if (state == 1)
                Projectile.velocity.Y = Math.Max(Projectile.velocity.Y, 0f);
            else if (state == 2)
            {
                Projectile.velocity.X = 0;
                Projectile.rotation = 0;
            }

            Projectile.netUpdate = true;
        }

        private void Fly(Player owner)
        {
            Vector2 toOwner = owner.Center - Projectile.Center + new Vector2(-owner.direction * 64, -24);
            if (Math.Abs(toOwner.X) > 20f || Math.Abs(toOwner.Y) > 20f)
            {
                Vector2 targetVelocity = toOwner.SafeNormalize(Vector2.Zero) * 10f * new Vector2(1.1f, 0.8f);
                Projectile.velocity = (Projectile.velocity * 19f + targetVelocity) / 20f;
            }
            else if (Projectile.velocity.Length() > 0.3f)
                Projectile.velocity *= 0.96f;

            if (Math.Abs(Projectile.velocity.X) > 0.1f)
                Projectile.direction = Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
            else
                Projectile.direction = Projectile.spriteDirection = owner.direction;
            Projectile.rotation = Projectile.velocity.X * 0.05f;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.QuickFrameDraw(new Rectangle((int)FrameX + (Projectile.spriteDirection > 0 ? 2 : 0), Projectile.frame, 4, 24), lightColor, 0);
            return false;
        }
    }
}
