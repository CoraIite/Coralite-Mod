using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Items.Misc_Shoot
{
    public class WhiteGardenia : ModItem
    {
        public override string Texture => AssetDirectory.Misc_Shoot + Name;

        public int shootCount;
        public static short GlowMaskID;

        public override void SetStaticDefaults()
        {
            Array.Resize(ref TextureAssets.GlowMask, TextureAssets.GlowMask.Length + 1);
            TextureAssets.GlowMask[^1] = ModContent.Request<Texture2D>(Texture + "_Glow");
            GlowMaskID = (short)(TextureAssets.GlowMask.Length - 1);
        }

        public override void SetDefaults()
        {
            Item.SetWeaponValues(150, 3f);
            Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Bullet, 5, 11f);

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.value = Item.sellPrice(0, 15, 0, 0);
            Item.rare = ItemRarityID.Red;

            Item.useTurn = false;
            Item.noUseGraphic = true;
            Item.autoReuse = true;

            Item.glowMask = GlowMaskID;
        }

        public override void HoldItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<WhiteGardeniaAimProj>()] < 1)
                {
                    Projectile.NewProjectile(new EntitySource_ItemUse(player, Item)
                        , player.Center, Vector2.Zero, ModContent.ProjectileType<WhiteGardeniaAimProj>(), 0, 0, player.whoAmI);
                }
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            return false;
        }
    }

    public class WhiteGardeniaHeldProj : BaseGunHeldProj
    {
        public WhiteGardeniaHeldProj() : base(0.4f, 6, -6, AssetDirectory.Misc_Shoot) { }
    }

    public class WhiteGardeniaFloat : ModProjectile
    {
        public override string Texture => AssetDirectory.Misc_Shoot + Name;

        public ref float Index => ref Projectile.ai[0];
        public ref float State => ref Projectile.ai[1];

        public Player Owner => Main.player[Projectile.owner];

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 20;
        }

        public override bool? CanDamage()
            => false;

        public override void AI()
        {
            if (Owner.HeldItem.type != ModContent.ItemType<WhiteGardenia>())
            {
                Projectile.Kill();
                return;
            }

            Projectile.timeLeft = 2;

            switch (State)
            {
                default:
                case 0://返回玩家身边
                    break;
                case 1://在玩家身边悬浮，查找敌人
                    break;
                case 2://飞到敌人头上
                    break;
                case 3://射击
                    break;
            }
        }

        public void FindEnemy()
        {

        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }

    public class WhiteGardeniaFloatLaser : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;
    }

    /// <summary>
    /// 使用ai0记录目标
    /// </summary>
    [AutoLoadTexture(Path = AssetDirectory.Misc_Shoot)]
    public class WhiteGardeniaAimProj : BaseHeldProj
    {
        public override string Texture => AssetDirectory.Misc_Shoot + Name;

        public ref float Target => ref Projectile.ai[0];
        public Vector2 AimPosition { get => Projectile.velocity; set => Projectile.velocity = value; }

        [AutoLoadTexture(Name = "WhiteGardeniaNumber")]
        public static ATex NumberTex { get; private set; }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
        }

        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => false;

        public override void AI()
        {
            if (Owner.HeldItem.type != ModContent.ItemType<WhiteGardenia>())
            {
                Projectile.Kill();
                return;
            }

            Projectile.Center = Owner.Center;
            Projectile.timeLeft = 2;

            int? target = FindTarget();
            if (target.HasValue)
                Target = target.Value;
            else
                Target = -1;


            if (Projectile.IsOwnedByLocalPlayer())//找目标
            {
                float length = ToMouse.Length();
                Vector2 dir = UnitToMouseV;
                bool collide = false;

                for (int i = 0; i < length; i += 8)
                {
                    Vector2 pos = Projectile.Center + dir * i;
                    Tile t = Framing.GetTileSafely(pos);

                    if (t.HasSolidTile())
                    {
                        AimPosition = pos;
                        collide = true;
                        break;
                    }
                }

                if (!collide)
                    AimPosition = MousePos;
            }
        }

        public int? FindTarget()
        {
            Vector2 mouseWorld = MousePos;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.CanBeChasedBy())
                    continue;

                if (npc.Hitbox.Contains((int)mouseWorld.X, (int)mouseWorld.Y)
                    && Collision.CanHitLine(Projectile.Center, 1, 1, npc.Center, npc.width, npc.height))
                    return i;
            }

            return null;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            #region 绘制光标
            Texture2D aimTex = Projectile.GetTexture();

            Vector2 aimPos = AimPosition;
            int frame = 0;

            if (Main.npc.IndexInRange((int)Target))
            {
                aimPos = Main.npc[(int)Target].Center;
                frame = 1;

            }

            Rectangle frameBox = aimTex.Frame(2, 1, frame);

            Main.spriteBatch.Draw(aimTex, aimPos - Main.screenPosition, frameBox, Color.White,0,frameBox.Size()/2,1,0,0);

            #endregion

            #region 绘制线
            Texture2D lineTex = TextureAssets.FishingLine.Value;

            float length = (aimPos - Owner.Center).Length();
            if (length < 20)
                length = 0;
            else
                length -= 20;

            Vector2 linePos = Owner.Center - Main.screenPosition;
            Rectangle targetRect = new Rectangle((int)linePos.X, (int)linePos.Y, lineTex.Width, (int)length);

            Main.spriteBatch.Draw(lineTex, targetRect, null, new Color(57,255,133), (aimPos - Owner.Center).ToRotation() - MathHelper.PiOver2
                , new Vector2(lineTex.Width / 2, 0), 0, 0);
            #endregion

            #region 绘制玩家数字条

            Texture2D numberTex = NumberTex.Value;

            if (Owner.HeldItem.ModItem is WhiteGardenia gardenia)
            {
                int count = gardenia.shootCount;

                if (count > 0 && count < 11)
                {
                    float scale = 1f + 0.3f * count / 10f;

                    Rectangle number = numberTex.Frame(10, 1, count - 1);

                    Main.spriteBatch.Draw(numberTex, Owner.Center - Main.screenPosition + new Vector2(0, -42)
                        , frameBox, Color.White, 0, new Vector2(numberTex.Width / 2, numberTex.Height), scale, 0, 0);
                }
            }

            #endregion

            return false;
        }
    }
}