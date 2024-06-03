using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Misc_Melee
{
    public class IronSilkKnief : BaseSilkKnifeItem
    {
        public override string Texture => AssetDirectory.Misc_Melee + Name;

        public override void SetDefaults()
        {
            Item.useAnimation = Item.useTime = 20;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileType<IronSilkKniefSlash>();
            Item.DamageType = DamageClass.Melee;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Blue1, Item.sellPrice(0, 0, 20, 0));
            Item.SetWeaponValues(16, 4);
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer != player.whoAmI)
                return false;

            if (player.altFunctionUse == 2)
            {
                foreach (var proj in Main.projectile.Where(p => p.active && p.owner == player.whoAmI && p.type == ProjectileType<IronSilkKniefChain>()))
                {
                    if ((int)proj.ai[2] == (int)BaseSilkKnifeSpecialProj.AIStates.onHit)
                    {
                        for (int i = 0; i < proj.localNPCImmunity.Length; i++)
                            proj.localNPCImmunity[i] = 0;

                        proj.ai[2] = (int)BaseSilkKnifeSpecialProj.AIStates.drag;
                        proj.netUpdate = true;
                    }
                    return false;
                }

                //生成弹幕
                Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<IronSilkKniefChain>(), damage, knockback, player.whoAmI);
                return false;
            }

            //生成弹幕
            Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.IronBar, 13)
                .AddIngredient(ItemID.WhiteString)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class IronSilkKniefSlash : BaseSwingProj
    {
        public override string Texture => AssetDirectory.Misc_Melee + "IronSilkKniefProj";

        public static Asset<Texture2D> ChainTex;

        public ref float Combo => ref Projectile.ai[0];

        public override void Load()
        {
            ChainTex = Request<Texture2D>(AssetDirectory.Misc_Melee + "IronSilkKniefChain");
        }

        public override void Unload()
        {
            ChainTex = null;
        }

        public IronSilkKniefSlash() : base(1.37f) { }

        public override void SetDefs()
        {
            Projectile.width = Projectile.height = 48;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.hide= true;
            Projectile.localNPCHitCooldown = 32;
            onHitFreeze = 0;
        }

        protected override void Initializer()
        {
            Projectile.extraUpdates = 1;
            Projectile.scale = 1f;

            SoundEngine.PlaySound(CoraliteSoundID.Swing_Item1, Projectile.Center);
            maxTime = Owner.itemTimeMax * 2;
            startAngle = 2.6f;
            totalAngle = 3.3f;
            distanceToOwner = 20;
            Smoother = Coralite.Instance.SqrtSmoother;

            base.Initializer();
        }

        protected override void OnSlash()
        {
            //Vector2 dir = RotateVec2.RotatedBy(1.57f * Math.Sign(totalAngle));
            //Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), DustID.Crimson,
            //       dir * Main.rand.NextFloat(0.5f, 2f));
            //dust.noGravity = true;

            if (Timer < maxTime / 2f)
            {
                distanceToOwner = MathHelper.Lerp(20, 60, Timer / (maxTime / 2f));
            }
            else
            {
                distanceToOwner = MathHelper.Lerp(60, 30, (Timer - maxTime / 2f) / (maxTime / 2f));
            }

            base.OnSlash();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //绘制链条
            Texture2D chainTex = ChainTex.Value;

            int width = (int)(Projectile.Center - Owner.Center).Length();   //链条长度

            Vector2 startPos = Owner.Center - Main.screenPosition;//起始点
            Vector2 endPos = Projectile.Center - Main.screenPosition;//终止点

            var laserTarget = new Rectangle((int)startPos.X, (int)startPos.Y, width, chainTex.Height);  //目标矩形
            var laserSource = new Rectangle(0, 0, width, chainTex.Height);   //把自身拉伸到目标矩形
            var origin = new Vector2(0, chainTex.Height / 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(default, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(chainTex, laserTarget, laserSource, lightColor, Projectile.rotation, origin, 0, 0);

            Texture2D mainTex = Projectile.GetTexture();

            //绘制影子拖尾
            //Projectile.DrawShadowTrails(lightColor, 0.3f, 0.03f, 1, 8, 2, 2f, -1);

            //绘制自己
            int dir = Math.Sign(totalAngle);
            float extraRot = OwnerDirection < 0 ? MathHelper.Pi : 0;
            extraRot += OwnerDirection == dir ? 0 : MathHelper.Pi;
            extraRot += spriteRotation * dir;

            Main.spriteBatch.Draw(mainTex, endPos, null, lightColor, Projectile.rotation + extraRot, mainTex.Size() / 2, Projectile.scale, CheckEffect(), 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);

            return false;
        }

        public override void PostDraw(Color lightColor) { }
    }

    public class IronSilkKniefChain : BaseSilkKnifeSpecialProj
    {
        public override string Texture => AssetDirectory.Misc_Melee + "IronSilkKniefProj";

        public IronSilkKniefChain() : base(16 * 24, 28, 18, 12)
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
            if (Timer == 0)
                canDamage = true;
            else
                canDamage = false;

            if (Timer < 8 && Timer > 0)
            {
                Projectile.Center = Vector2.Lerp(Projectile.Center, Owner.Center, 0.15f);
                Projectile.rotation = (Projectile.Center - Owner.Center).ToRotation();

                if (Target < 0 || Target > Main.maxNPCs)
                    goto end;
                NPC npc = Main.npc[(int)Target];
                if (!npc.active || npc.dontTakeDamage)
                    goto end;

                npc.velocity += (Owner.Center - npc.Center).SafeNormalize(Vector2.Zero) * 2 * npc.knockBackResist;
            }
            else if (Timer > 8)
            {
                //将玩家回弹
                SoundEngine.PlaySound(CoraliteSoundID.Swing_Item1, Projectile.Center);
                Projectile.Kill();
            }

        end:
            Timer++;
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.Server && (int)HookState < 3 && (int)HookState > -1)
            {
                SoundEngine.PlaySound(CoraliteSoundID.WafflesIron_Item178, Projectile.Center);
                Vector2 direction = (Owner.Center - Projectile.Center).SafeNormalize(Vector2.One);
                float length = (Owner.Center - Projectile.Center).Length();
                for (int i = 0; i < length; i += 8)
                {
                    Dust.NewDustPerfect(Projectile.Center + direction * i + Main.rand.NextVector2Circular(4, 4), DustID.Iron, Scale: Main.rand.NextFloat(1f, 1.2f));
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //绘制链条
            Texture2D chainTex = IronSilkKniefSlash.ChainTex.Value;

            int width = (int)(Projectile.Center - Owner.Center).Length();   //链条长度

            Vector2 startPos = Owner.Center - Main.screenPosition;//起始点
            Vector2 endPos = Projectile.Center - Main.screenPosition;//终止点

            var laserTarget = new Rectangle((int)startPos.X, (int)startPos.Y, width, chainTex.Height);  //目标矩形
            var laserSource = new Rectangle(0, 0, width, chainTex.Height);   //把自身拉伸到目标矩形
            var origin2 = new Vector2(0, chainTex.Height / 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(default, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(chainTex, laserTarget, laserSource, lightColor, Projectile.rotation, origin2, 0, 0);

            Texture2D mainTex = Projectile.GetTexture();

            //绘制自己
            Main.spriteBatch.Draw(mainTex, endPos, null, lightColor, Projectile.rotation + 1.37f, mainTex.Size() / 2, Projectile.scale, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);

            return false;
        }
    }
}
