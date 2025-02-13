using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Misc_Melee
{
    public class CancerFlail : ModItem
    {
        public override string Texture => AssetDirectory.Misc_Melee + Name;

        public int ShootCount;

        public override void SetDefaults()
        {
            Item.useAnimation = Item.useTime = 32;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileType<CancerFlailProj>();
            Item.DamageType = DamageClass.Melee;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.LightRed4, Item.sellPrice(0, 1, 0, 0));
            Item.SetWeaponValues(50, 8);
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer != player.whoAmI)
                return false;

            //生成弹幕
            if (ShootCount == 0)
            {
                //直线拳
                Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI);
                ShootCount = 1;
            }
            else
            {
                //抓取拳
                Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, 0, player.whoAmI, 1);
                ShootCount = 0;
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.TitaniumBar, 12)
                .AddIngredient(ItemID.SoulofFlight, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.AdamantiteBar, 12)
                .AddIngredient(ItemID.SoulofFlight, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    [AutoLoadTexture(Path =AssetDirectory.Misc_Melee)]
    public class CancerFlailProj : BaseSilkKnifeSpecialProj
    {
        public override string Texture => AssetDirectory.Misc_Melee + Name;

        [AutoLoadTexture(Name = "CancerFlailChain")]
        public static ATex ChainTex { get; private set; }
        [AutoLoadTexture(Name = "CancerFlailHandle")]
        public static ATex HandleTex { get; private set; }

        public ref float Combo => ref Projectile.ai[0];

        public CancerFlailProj() : base(16 * 25, 40, 22, 18)
        {
        }

        public override void SetDefaults()
        {
            Projectile.usesLocalNPCImmunity = false;
            Projectile.localNPCHitCooldown = 20;
            Projectile.width = Projectile.height = 32;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
        }

        public override void Dragging()
        {
            Projectile.Kill();
        }

        public override void Shoot()
        {
            base.Shoot();
            Lighting.AddLight(Projectile.Center, Color.Gold.ToVector3());
            if (Timer % 4 == 0)
            {
                float range = Main.rand.NextFloat(0, 1);
                for (int i = 0; i < 4; i++)
                {
                    Vector2 dir = (i * MathHelper.PiOver2).ToRotationVector2();
                    for (int j = 0; j < 3; j++)
                    {
                        Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Clentaminator_Cyan, dir * (1 + (j * (0.55f + (0.25f * range))))
                            , Scale: 0.8f + (range * 0.4f) - (j * 0.15f));
                        d.noGravity = true;
                    }
                }
            }
        }

        public override void RollingInHand()
        {
            Projectile.spriteDirection = Owner.direction;
            if (Owner.itemTime > 2)
            {
                Owner.heldProj = Projectile.whoAmI;
                Projectile.rotation += Owner.direction * MathHelper.TwoPi * 2 / Owner.itemTimeMax;

                Owner.itemRotation = Projectile.rotation + (DirSign > 0 ? 0 : MathHelper.Pi);
                Projectile.Center = Owner.Center + (Projectile.rotation.ToRotationVector2() * rollingLength);
            }
            else
            {
                SoundEngine.PlaySound(CoraliteSoundID.WhipSwing_Item152, Projectile.Center);
                Vector2 dir = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.Zero);
                Projectile.Center = Owner.Center + (dir * 64);
                Projectile.velocity = dir * shootSpeed;
                Projectile.rotation = dir.ToRotation();
                HookState = (int)AIStates.shoot;
                Projectile.tileCollide = true;
                Projectile.netUpdate = true;
                Projectile.damage *= 3;
            }
        }

        public override void OnHookedToNPC()
        {
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            for (int i = 0; i < 6; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GoldFlame, dir.RotateByRandom(-0.4f, 0.4f) * Main.rand.NextFloat(2f, 6f)
                    , Scale: Main.rand.NextFloat(1f, 1.5f));
                d.noGravity = false;
            }

            for (int i = 0; i < 6; i++)
            {
                Vector2 offset = dir.RotateByRandom(-0.5f, 0.5f);
                Dust d = Dust.NewDustPerfect(Projectile.Center + (offset * Main.rand.Next(16, 32)), DustID.GoldFlame, offset * Main.rand.NextFloat(2f, 4f)
                    , Scale: Main.rand.NextFloat(2.5f, 3f));
                d.noGravity = true;
            }

            for (int i = 0; i < 4; i++)
            {
                Vector2 dir2 = (i * MathHelper.PiOver2).ToRotationVector2();
                for (int j = 0; j < 6; j++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Clentaminator_Cyan, dir2 * (1 + (j * 0.8f)), Scale: 1.6f - (j * 0.15f));
                    d.noGravity = true;
                }
            }

            Projectile.damage /= 3;
            HookState = (int)AIStates.back;
            Timer = 0;
        }

        public override void BackToOwner()
        {
            base.BackToOwner();
            Timer++;
            if (Combo == 1 && Timer < 8 && Timer > 0)
            {
                Projectile.Center = Vector2.Lerp(Projectile.Center, Owner.Center, 0.15f);
                Projectile.rotation = (Projectile.Center - Owner.Center).ToRotation();

                if (!Target.GetNPCOwner(out NPC npc))
                {
                    return;
                }
                if (!npc.active || !npc.CanBeChasedBy())
                    return;

                npc.velocity += (Owner.Center - npc.Center).SafeNormalize(Vector2.Zero) * 2 * npc.knockBackResist;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var frameBox = mainTex.Frame(1, 2, 0, (int)Combo);
            Vector2 endPos = Projectile.Center - Main.screenPosition;//终止点
            SpriteEffects effect = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            //绘制链条
            if (HookState == (int)AIStates.rolling)
            {
                Texture2D handleTex = HandleTex.Value;

                Main.spriteBatch.Draw(mainTex, endPos, frameBox, lightColor, Projectile.rotation, frameBox.Size() / 2, Projectile.scale, effect, 0);
                Main.spriteBatch.Draw(handleTex, Owner.Center + ((Owner.itemRotation + (Owner.direction > 0 ? 0 : 3.141f)).ToRotationVector2() * 16) - Main.screenPosition
                    , null, lightColor, Projectile.rotation, handleTex.Size() / 2, Projectile.scale, effect, 0);

                return false;
            }

            Texture2D chainTex = ChainTex.Value;

            int width = (int)(Projectile.Center - Owner.Center).Length();   //链条长度

            Vector2 startPos = Owner.Center - Main.screenPosition;//起始点

            var laserTarget = new Rectangle((int)startPos.X, (int)startPos.Y, width, chainTex.Height);  //目标矩形
            var laserSource = new Rectangle((int)(Main.GlobalTimeWrappedHourly * 50), 0, width, chainTex.Height);   //把自身拉伸到目标矩形
            var origin2 = new Vector2(0, chainTex.Height / 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(default, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(chainTex, laserTarget, laserSource, Color.White * 0.8f, Projectile.rotation, origin2, 0, 0);

            //绘制自己
            Main.spriteBatch.Draw(mainTex, endPos, frameBox, lightColor, Projectile.rotation, frameBox.Size() / 2, Projectile.scale, effect, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);

            return false;
        }
    }

}
