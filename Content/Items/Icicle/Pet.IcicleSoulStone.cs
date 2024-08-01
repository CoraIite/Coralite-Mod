using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace Coralite.Content.Items.Icicle
{
    public class IcicleSoulStone : ModItem
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ModContent.ProjectileType<IcicleSoul>(), ModContent.BuffType<IcicleSoulBuff>());
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

    public class IcicleSoulBuff : ModBuff
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;

            int projType = ModContent.ProjectileType<IcicleSoul>();

            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[projType] <= 0)
            {
                var entitySource = player.GetSource_Buff(buffIndex);
                Projectile.NewProjectile(entitySource, player.Center, Vector2.Zero, projType, 0, 0f, player.whoAmI);
            }
        }
    }

    public class IcicleSoul : ModProjectile
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.FairyQueenPet);
            Projectile.tileCollide = false;
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

            Idle(Owner);

            Lighting.AddLight(Projectile.Center, new Vector3(1.2f, 0.85f, 0.45f));
            Projectile.UpdateFrameNormally(5, 3);
        }

        private void CheckActive(Player player)
        {
            if (!player.dead && player.HasBuff(ModContent.BuffType<IcicleSoulBuff>()))
            {
                Projectile.timeLeft = 2;
            }
        }

        private void Idle(Player Owner)
        {
            float _10 = 10f;

            Vector2 Center = Projectile.Center;
            Vector2 DistanceToOwner = Owner.Center - Center + new Vector2(-Owner.direction * 64, -24);

            float LenthToOwner = DistanceToOwner.Length();

            if (Main.rand.NextBool(16))
            {
                int index = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch, newColor: Coralite.ThunderveinYellow);
                Main.dust[index].noGravity = true;
            }

            if (LenthToOwner > 2000f)//距离过远直接传送
                Projectile.Center = Owner.Center;

            if (Math.Abs(DistanceToOwner.X) > 20f || Math.Abs(DistanceToOwner.Y) > 20f)//距离玩家有一定距离时候
            {
                DistanceToOwner = DistanceToOwner.SafeNormalize(Vector2.Zero);
                DistanceToOwner *= _10;
                DistanceToOwner *= new Vector2(1.1f, 0.8f);
                Projectile.velocity = (Projectile.velocity * 19f + DistanceToOwner) / 20f;
                Projectile.direction = Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
            }
            else
            {
                if (Projectile.velocity.Length() > 0.3f)//距离玩家近时候
                {
                    Projectile.velocity *= 0.96f;
                    Projectile.direction = Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
                }
                else
                    Projectile.direction = Projectile.spriteDirection = Owner.direction;
            }

            Projectile.rotation = (Projectile.spriteDirection > 0) ? 0 : 3.141f;
            Projectile.rotation += Projectile.velocity.X * 0.05f;
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
