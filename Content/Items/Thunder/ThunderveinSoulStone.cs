using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Thunder
{
    public class ThunderveinSoulStone : ModItem
    {
        public override string Texture => AssetDirectory.ThunderItems + Name;

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ModContent.ProjectileType<ThunderveinSoul>(), ModContent.BuffType<ThunderveinSoulBuff>());
            Item.width = 28;
            Item.height = 20;
            Item.rare = ItemRarityID.Master;
            Item.master = true;
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

    public class ThunderveinSoulBuff : ModBuff
    {
        public override string Texture => AssetDirectory.ThunderItems + Name;

        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.lightPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;

            int projType = ModContent.ProjectileType<ThunderveinSoul>();

            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[projType] <= 0)
            {
                var entitySource = player.GetSource_Buff(buffIndex);
                Projectile.NewProjectile(entitySource, player.Center, Vector2.Zero, projType, 0, 0f, player.whoAmI);
            }
        }
    }

    public class ThunderveinSoul : ModProjectile
    {
        public override string Texture => AssetDirectory.ThunderItems + Name;

        ref float Timer => ref Projectile.ai[0];
        ref float State => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.LightPet[Projectile.type] = true;
            Main.projFrames[Projectile.type] = 4;
            Main.projPet[Projectile.type] = true;

        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.FairyQueenPet);
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];

            if (!Owner.active)
            {
                Projectile.Kill();
                return;
            }

            CheckActive(Owner);

            if (State == 1)
            {
                //绕着玩家飞一圈
                Vector2 center = Projectile.Center;
                const int RollingTime = 130;

                Vector2 center2 = Owner.Center + (Timer / RollingTime * MathHelper.TwoPi*2).ToRotationVector2() * 350;
                Vector2 dir3 = center2 - center;

                float velRot = Projectile.velocity.ToRotation();
                float targetRot2 = dir3.ToRotation();

                float speed = Projectile.velocity.Length();
                float aimSpeed = Math.Clamp(dir3.Length() / 450f, 0, 1) * 35f;

                Projectile.velocity = velRot.AngleTowards(targetRot2, 0.9f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.5f);
                Projectile.rotation = Projectile.velocity.ToRotation();
                Projectile.direction = Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);

                int index = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PortalBoltTrail, newColor: Coralite.Instance.ThunderveinYellow);
                Main.dust[index].noGravity = true;
                Main.dust[index].velocity = Helper.NextVec2Dir(2f,6f);

                Timer++;
                if (Timer > RollingTime)
                {
                    State = 0;
                    Timer = 0;
                }
            }
            else
                Idle(Owner);


            Lighting.AddLight(Projectile.Center, new Vector3(1.2f, 0.85f, 0.45f));
            if (++Projectile.frameCounter > 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame > 3)
                    Projectile.frame = 0;
            }
        }

        private void CheckActive(Player player)
        {
            if (!player.dead && player.HasBuff(ModContent.BuffType<ThunderveinSoulBuff>()))
            {
                Projectile.timeLeft = 2;
            }
        }

        private void Idle(Player Owner)
        {
            float _10 = 15f;

            Vector2 Center = Projectile.Center;
            Vector2 DistanceToOwner = Owner.Center - Center + new Vector2(Owner.direction * 32, -48);

            float LenthToOwner = DistanceToOwner.Length();

            if (Main.rand.NextBool(16))
            {
                int index = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PortalBoltTrail, newColor: Coralite.Instance.ThunderveinYellow);
                Main.dust[index].noGravity = true;
            }

            if (LenthToOwner > 2000f)//距离过远直接传送
                Projectile.Center = Owner.Center;

            if (Math.Abs(DistanceToOwner.X) > 30f || Math.Abs(DistanceToOwner.Y) > 20f)//距离玩家有一定距离时候
            {
                DistanceToOwner = DistanceToOwner.SafeNormalize(Vector2.Zero);
                DistanceToOwner *= _10;
                DistanceToOwner *= new Vector2(1.2f, 0.8f);
                Projectile.velocity = (Projectile.velocity * 15f + DistanceToOwner) / 16f;
                Projectile.direction = Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
            }
            else
            {
                if (Projectile.velocity.Length() > 2)//距离玩家近时候
                {
                    Projectile.velocity *= 0.97f;
                    Projectile.direction = Projectile.spriteDirection = Owner.direction;
                }
                else
                    Projectile.direction = Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
            }

            Projectile.rotation = (Projectile.spriteDirection > 0) ? 0 : 3.141f;
            Projectile.rotation += Projectile.velocity.X * 0.05f;

            if (Owner.velocity.Length() > 3)
                Timer++;

            if (Timer > 300)
            {
                Timer = 0;
                State = 1;
                Projectile.netUpdate = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var frameBox = mainTex.Frame(1, 4, 0, Projectile.frame);

            SpriteEffects effects = SpriteEffects.None;

            if (Projectile.spriteDirection < 0)
            {
                effects = SpriteEffects.FlipVertically;
            }

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, frameBox, lightColor, Projectile.rotation, frameBox.Size() / 2, Projectile.scale, effects, 0);

            return false;
        }
    }
}
