using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Nightmare
{
    [AutoloadEquip(EquipType.HandsOn)]
    public class BoneRing : ModItem, INightmareWeapon
    {
        public override string Texture => AssetDirectory.NightmareItems + Name;

        public override void SetDefaults()
        {
            Item.useAnimation = Item.useTime = 45;
            Item.reuseDelay = 20;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.DamageType = DamageClass.Ranged;
            Item.rare = RarityType<NightmareRarity>();
            Item.value = Item.sellPrice(2, 0, 0, 0);
            Item.SetWeaponValues(320, 4, 4);
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useTurn = false;
            Item.shootSpeed = 24;
        }

        public override void HoldItem(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.equippedBoneRing = true;

            player.handon = 25;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }
    }

    /// <summary>
    /// 主体弹幕
    /// </summary>
    public class BoneHand : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float NPCIndex => ref Projectile.ai[1];
        public ref float State => ref Projectile.ai[2];
        public ref float Timer => ref Projectile.localAI[0];
        public ref float ShootCount => ref Projectile.localAI[1];

        public Player Owner => Main.player[Projectile.owner];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.friendly = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.scale = 0.75f;
        }

        public override bool? CanDamage()
        {
            if (State == 1)
                return true;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            NPCIndex = target.whoAmI;
        }

        public override void AI()
        {
            Projectile.timeLeft = 2;

            switch (State)
            {
                default:
                case 0://射出，直接挪到鼠标旁边
                    {
                        if (!Owner.channel)
                            State = 2;

                        NPCIndex = -1;
                        Vector2 center = Main.MouseWorld + (Main.GlobalTimeWrappedHourly/6 * MathHelper.TwoPi).ToRotationVector2() * 32;
                        Vector2 dir = center - Projectile.Center;

                        float velRot = Projectile.velocity.ToRotation();
                        float targetRot = dir.ToRotation();

                        float speed = Projectile.velocity.Length();
                        float aimSpeed = Math.Clamp(dir.Length() / 80f, 0, 1) * 24;

                        Projectile.velocity = velRot.AngleTowards(targetRot, 0.5f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.15f);
                        Projectile.rotation = Projectile.velocity.ToRotation();

                        if (Main.rand.NextBool(4))
                        {
                            Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(48, 48), DustID.VilePowder,
                                  Projectile.velocity * 0.4f,newColor:NightmarePlantera.nightmareRed, Scale: Main.rand.NextFloat(1f, 1.5f));
                            d.noGravity = true;
                        }
                    }
                    break;
                case 1://打到NPC，黏到NPC身上后不断射爪击弹幕
                    {
                        if (!Main.npc.IndexInRange((int)NPCIndex)||!Owner.channel)
                            State = 2;

                        NPC npc = Main.npc[(int)NPCIndex];

                        if (!npc.active||npc.active||!npc.CanBeChasedBy())
                            State = 2;

                        if (Timer>20)
                        {

                        }
                        Timer++;
                    }
                    break;
                case 2://返回玩家
                    {
                        Projectile.velocity = (Owner.Center - Projectile.Center).SafeNormalize(Vector2.One) * 24;
                        if (Projectile.Distance(Owner.Center) < 48)
                            Projectile.Kill();
                    }
                    break;
                case 3://撕开裂隙
                    break;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(ProjectileID.InsanityShadowFriendly);
            Texture2D mainTex = TextureAssets.Projectile[ProjectileID.InsanityShadowFriendly].Value;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Vector2 origin = mainTex.Size() / 2;

            Color c = NightmarePlantera.nightmareRed * 0.5f;

            Vector2 scale = new Vector2(0.75f, Projectile.scale);

            for (int i = 0; i < 3; i++)
            {
                Main.spriteBatch.Draw(mainTex, pos + (Main.GlobalTimeWrappedHourly / 5 + i * MathHelper.TwoPi / 3).ToRotationVector2() * 6
                    , null, c, Projectile.rotation, origin, scale, 0, 0);
            }

            Main.spriteBatch.Draw(mainTex, pos, null, Color.Black, Projectile.rotation, origin, scale, 0, 0);
            return false;
        }
    }

    /// <summary>
    /// 鬼手爪击<br></br>
    /// 使用ai0控制状态，0：直线爪，1：转圈抓<br></br>
    /// 使用ai1传入颜色，为1时可变成红色并可以获得梦魇光能<br></br>
    /// 使用ai2传入攻击方向
    /// </summary>
    public class BoneClaw:ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;
        public ref float State => ref Projectile.ai[0];
        public ref float ColorState => ref Projectile.ai[1];
        public ref float RotDir => ref Projectile.ai[2];

        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public override void AI()
        {
            base.AI();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }

    /// <summary>
    /// 追踪鬼手
    /// </summary>
    public class BoneHands
    {

    }

    /// <summary>
    /// 裂隙
    /// </summary>
    public class BoneSilt:ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

    }

    public class BoneRingDrawLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition()
        {
            return new AfterParent(PlayerDrawLayers.HandOnAcc);
        }

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return drawInfo.drawPlayer.handon == EquipLoader.GetEquipSlot(Mod, "BoneRing", EquipType.HandsOn);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Vector2 pos = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - (drawInfo.drawPlayer.bodyFrame.Width / 2) + (drawInfo.drawPlayer.width / 2)), (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawInfo.drawPlayer.height - drawInfo.drawPlayer.bodyFrame.Height + 4f))
                + drawInfo.drawPlayer.bodyPosition
                + drawInfo.drawPlayer.bodyFrame.Size() / 2;
            DrawData item = new DrawData(Request<Texture2D>(AssetDirectory.NightmareItems + "BoneRing_HandsOn").Value,
                pos,
                drawInfo.drawPlayer.bodyFrame,
                Color.White/*drawInfo.colorArmorBody*/,
                drawInfo.drawPlayer.bodyRotation,
                drawInfo.bodyVect, 1f,
                drawInfo.playerEffect);
            item.shader = drawInfo.cHandOn;
            drawInfo.DrawDataCache.Add(item);
        }
    }
}
