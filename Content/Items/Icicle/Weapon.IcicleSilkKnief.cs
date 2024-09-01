using Coralite.Content.Items.GlobalItems;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Icicle
{
    public class IcicleSilkKnief : BaseSilkKnifeItem
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 34;
            Item.useTime = 21;
            Item.useAnimation = 21;
            Item.knockBack = 2f;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Ranged;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.shoot = ProjectileType<IcicleKnief>();
            Item.shootSpeed = 14;

            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            CoraliteGlobalItem.SetColdDamage(Item);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer != player.whoAmI)
                return false;

            if (player.altFunctionUse == 2)
            {
                foreach (var proj in Main.projectile.Where(p => p.active && p.owner == player.whoAmI && p.type == ProjectileType<IcicleKniefChain>()))
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
                Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero, ProjectileType<IcicleKniefChain>(), damage, knockback, player.whoAmI);
                return false;
            }

            //生成弹幕
            Projectile.NewProjectile(source, player.Center, velocity, type, damage, knockback, player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IcicleCrystal>()
                .AddIngredient<IcicleScale>(2)
                .AddIngredient(ItemID.WhiteString)
                .AddTile(TileID.IceMachine)
                .Register();
        }
    }

    public class IcicleKnief : ModProjectile
    {
        public override string Texture => AssetDirectory.IcicleItems + "IcicleSilkKnief";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 22;
            Projectile.timeLeft = 600;
            Projectile.aiStyle = -1;

            Projectile.coldDamage = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;

            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void AI()
        {
            if (Projectile.timeLeft < 560)
            {
                Projectile.velocity.X *= 0.98f;
                Projectile.velocity.Y += 0.2f;
                if (Projectile.velocity.Y > 12)
                    Projectile.velocity.Y = 12;

                Projectile.rotation += 0.3f + (Projectile.timeLeft / 560 * 0.5f);
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            foreach (var proj in Main.projectile.Where(p => p.active && p.friendly && p.owner == Projectile.owner
                && p.type == ProjectileType<IcicleKniefChain>() && p.ai[2] == (int)BaseSilkKnifeSpecialProj.AIStates.onHit))
            {
                if (proj.ai[1] == target.whoAmI)
                {
                    proj.ai[0]++;
                    if (proj.ai[0] > 6)
                        proj.ai[0] = 6;
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(CoraliteSoundID.CrushedIce_Item27, Projectile.Center);
            if (VisualEffectSystem.HitEffect_Dusts)
                for (int i = 0; i < 6; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, DustID.Ice, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(0.2f, 0.5f));
                    Dust.NewDustPerfect(Projectile.Center, DustID.SnowBlock, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(0.1f, 0.3f));
                }
        }
    }

    public class IcicleKniefChain : BaseSilkKnifeSpecialProj
    {
        public override string Texture => AssetDirectory.IcicleItems + "IcicleSilkKnief";

        public static Asset<Texture2D> ChainTex;

        public ref float Count => ref Projectile.ai[0];

        public IcicleKniefChain() : base(20 * 24, 28, 20, 20)
        {
        }

        public override void Load()
        {
            ChainTex = Request<Texture2D>(AssetDirectory.IcicleItems + "IcicleKniefChain");
        }

        public override void Unload()
        {
            ChainTex = null;
        }

        public override void SetDefaults()
        {
            Projectile.coldDamage = true;

            Projectile.usesLocalNPCImmunity = false;
            Projectile.localNPCHitCooldown = 20;
            Projectile.width = Projectile.height = 32;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
        }

        public override void Dragging()
        {
            if (Timer == 0)
            {
                if (Count > 3)
                {
                    Particle.NewParticle(Projectile.Center, Vector2.Zero, CoraliteContent.ParticleType<IceHalo>(), Scale: 0.6f);
                    Particle.NewParticle(Projectile.Center, Vector2.Zero, CoraliteContent.ParticleType<IceHalo>(), Scale: 0.4f);
                    for (int j = 0; j < 8; j++)
                    {
                        Dust.NewDustPerfect(Projectile.Center, DustType<CrushedIceDust>(), -Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(2f, 5f),
                            Scale: Main.rand.NextFloat(1f, 1.4f));
                    }

                    if (VisualEffectSystem.HitEffect_ScreenShaking)
                    {
                        var modifyer = new PunchCameraModifier(Projectile.Center, Helpers.Helper.NextVec2Dir(), 10, 10, 7, 1000);
                        Main.instance.CameraModifiers.Add(modifyer);
                    }

                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero,
                        ProjectileType<IcicleThornExplosion>(), (int)(Projectile.damage * (Count - 3)), Projectile.knockBack, Projectile.owner);
                }
            }

            if (Timer < 8)
            {
                Projectile.Center = Vector2.Lerp(Projectile.Center, Owner.Center, 0.15f);
                Projectile.rotation = (Projectile.Center - Owner.Center).ToRotation();
            }
            else
            {
                SoundEngine.PlaySound(CoraliteSoundID.Swing_Item1, Projectile.Center);
                Projectile.Kill();
            }

            Timer++;
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.Server && (int)HookState < 3 && (int)HookState > -1)
            {
                SoundEngine.PlaySound(CoraliteSoundID.CrushedIce_Item27, Projectile.Center);
                Vector2 direction = (Owner.Center - Projectile.Center).SafeNormalize(Vector2.One);
                float length = (Owner.Center - Projectile.Center).Length();
                for (int i = 0; i < length; i += 8)
                {
                    Dust.NewDustPerfect(Projectile.Center + (direction * i) + Main.rand.NextVector2Circular(4, 4), DustID.Ice, Scale: Main.rand.NextFloat(1f, 1.2f));
                }
            }
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
            var origin2 = new Vector2(0, chainTex.Height / 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(default, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(chainTex, laserTarget, laserSource, lightColor, Projectile.rotation, origin2, 0, 0);

            Texture2D mainTex = Projectile.GetTexture();

            if (Count > 0)
            {
                Texture2D snowTex = Request<Texture2D>(AssetDirectory.IcicleItems + "SnowFlower").Value;
                Texture2D bigSnowTex = Request<Texture2D>(AssetDirectory.IcicleItems + "BigSnowFlower").Value;
                Main.spriteBatch.Draw(bigSnowTex, endPos, null, lightColor * 0.75f, Projectile.rotation + (Main.GlobalTimeWrappedHourly * 2), bigSnowTex.Size() / 2, Projectile.scale, 0, 0);

                for (int i = 0; i < Count; i++)
                {
                    float rot = i * MathHelper.TwoPi / 6;
                    Main.spriteBatch.Draw(snowTex, endPos + (rot.ToRotationVector2() * 32), null, lightColor * 0.6f, rot + (Main.GlobalTimeWrappedHourly * 2), snowTex.Size() / 2, Projectile.scale, 0, 0);
                }
            }

            //绘制自己
            Main.spriteBatch.Draw(mainTex, endPos, null, lightColor, Projectile.rotation + MathHelper.PiOver2, mainTex.Size() / 2, Projectile.scale, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);

            return false;
        }
    }
}
