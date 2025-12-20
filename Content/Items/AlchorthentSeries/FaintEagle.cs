using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Prefabs.Particles;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Items.AlchorthentSeries
{
    public class FaintEagle : BaseAlchorthentItem
    {
        public override void SetOtherDefaults()
        {
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = Item.useAnimation = 30;
            Item.shoot = 10;

            Item.SetWeaponValues(15, 4);
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Blue1, Item.sellPrice(0, 0, 50));
        }

        public override void SpecialAttack(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<FaintEagleHeldProj>(), damage, knockback, player.whoAmI, 1);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.StoneBlock, 3)
                .AddIngredient(ItemID.ClayBlock, 10)
                .AddIngredient(ItemID.CopperBar, 6)
                .AddTile(TileID.Campfire)
                .AddCondition(Condition.NearWater)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.StoneBlock, 3)
                .AddIngredient(ItemID.ClayBlock, 10)
                .AddIngredient(ItemID.TinBar, 6)
                .AddTile(TileID.Campfire)
                .AddCondition(Condition.NearWater)
                .Register();
        }
    }

    public class FaintEagleHeldProj : BaseHeldProj
    {
        public override string Texture => AssetDirectory.AlchorthentSeriesItems + Name;

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = Projectile.height = 16;
        }

        public override bool? CanDamage() => false;
        public override bool? CanCutTiles() => false;
        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            if (Owner.dead || Owner.HeldItem.type != ModContent.ItemType<FaintEagle>())
            {
                Projectile.Kill();
                return;
            }

            Projectile.UpdateFrameNormally(3, 7);
            SetHeld();

            Projectile.rotation = ToMouseA;
            Owner.itemRotation = Projectile.rotation + (DirSign > 0 ? 0 : MathHelper.Pi);
            Projectile.Center = Owner.Center + UnitToMouseV * 32;

            if (State == 1)
                SpecialAttack();
            else
                NormalState();
        }

        public void SpecialAttack()
        {
            if (Projectile.IsOwnedByLocalPlayer() && Owner.TryGetModPlayer(out CoralitePlayer cp) && cp.useSpecialAttack)
            {
                Projectile.timeLeft = 2;
                Owner.itemTime = Owner.itemAnimation = 2;
                Owner.direction = ToMouse.X > 0 ? 1 : -1;
            }

            Timer++;

           Dust d= Dust.NewDustPerfect(Projectile.Center+ UnitToMouseV*8 + Main.rand.NextVector2Circular(12, 12)
                , DustID.Torch, UnitToMouseV * Main.rand.NextFloat(1, 5), Scale: Main.rand.NextFloat(0.8f, 1.2f));
            d.noGravity = true;

            if (Timer > 20)
            {
                if (Projectile.soundDelay==0)
                {
                    Projectile.soundDelay =30;
                    Helper.PlayPitched(CoraliteSoundID.Flamethrower_Item34, Projectile.Center,pitchAdjust:0.4f);
                }

                //生成火焰弹幕
                if (Timer % 4 == 0)
                    Projectile.NewProjectileFromThis<FaintEagleFire>(Projectile.Center + UnitToMouseV * 30, UnitToMouseV.RotateByRandom(-0.05f, 0.05f) * Main.rand.NextFloat(9, 13), Projectile.damage / 2, Projectile.knockBack, Main.rand.Next(3), Main.rand.Next(2));
            }
        }

        public void NormalState()
        {
            if (Timer == 0)
            {
                Timer++;
                Projectile.timeLeft = Owner.itemTimeMax;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.QuickFrameDraw(new Rectangle(0, Projectile.frame, 1, 8), lightColor, MathHelper.PiOver4);

            return false;
        }

    }

    [VaultLoaden(AssetDirectory.AlchorthentSeriesItems)]
    public class FaintEagleFire : ModProjectile
    {
        public override string Texture => AssetDirectory.AlchorthentSeriesItems + Name;

        public static ATex FaintEagleFire2 { get; set; }
        public static ATex FaintEagleFire3 { get; set; }

        public ref float TexType => ref Projectile.ai[0];
        public ref float Flip => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.localAI[1];
        public ref float Alpha => ref Projectile.localAI[2];
        public bool Heated
        {
            get => Projectile.ai[2] == 1;
            set
            {
                if (value)
                    Projectile.ai[2] = 1;
                else
                    Projectile.ai[2] = 0;
            }
        }

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 6);
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;
            Projectile.width = Projectile.height = 48;
            Projectile.scale = 0.8f;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Projectile.UpdateFrameNormally(2, 16);

            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.IsOwnedByLocalPlayer() && Projectile.frame > 15)
                Projectile.Kill();

            if (Timer == 0)
            {
                Alpha = 1;
            }

            if (Projectile.frame > 10)
            {
                Alpha *= 0.95f;
                Projectile.velocity *= 0.93f;
            }

            Projectile.velocity *= 0.975f;

            Timer++;
            if (Timer == 15)
                Projectile.Resize(50, 50);

            HeatEagle();
            SpawnParticle();
        }

        public void HeatEagle()
        {
            if (Heated)
                return;

            if (Timer % 2 == 0)//找火鹰弹幕并给它加能量
            {

            }
        }

        public void SpawnParticle()
        {
            if (Projectile.frame > 10)
                return;

            if (Timer % 10 == 0)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width * 0.7f, Projectile.height * 0.7f);
                switch (Main.rand.Next(3))
                {
                    default:
                    case 0://生成代表火的三角形
                        PRTLoader.NewParticle<FaintEagleParticle1>(pos, Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(-0.7f,0.7f), Color.White with { A = 120 }, Main.rand.NextFloat(0.7f, 1f));
                        break;
                    case 1:
                        {
                            Vector2 dir = Projectile.velocity.RotateByRandom(-0.2f, 0.2f);
                            var particle = PRTLoader.NewParticle<FaintEagleParticle2>(pos, dir * 0.3f, Color.White with { A = 120 }, Main.rand.NextFloat(1f, 1.5f));

                            particle.Rotation = dir.ToRotation();
                        }
                        break;
                    case 2:
                        {
                            Vector2 dir = Projectile.velocity.RotateByRandom(-0.2f, 0.2f);
                            var particle = PRTLoader.NewParticle<FaintEagleParticle3>(pos, dir * 0.4f, Color.White with { A = 120 }, Main.rand.NextFloat(1f, 1.5f));

                            particle.Rotation = dir.ToRotation();
                        }
                        break;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = oldVelocity * 0.3f;
            Projectile.tileCollide = false;

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Timer == 0)
                return false;

            SpriteEffects effect = Flip == 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Color backColor = Color.White with { A = 0 } * Alpha;

            Texture2D mainTex;

            if (TexType == 0)
                mainTex = TextureAssets.Projectile[Projectile.type].Value;
            else if (TexType == 1)
                mainTex = FaintEagleFire2.Value;
            else
                mainTex = FaintEagleFire3.Value;

            Vector2 toCenter = new(Projectile.width / 2, Projectile.height / 2);
            var rect = mainTex.Frame(1, 15, 0, Projectile.frame);

            for (int i = 1; i < 6; i += 1)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, rect,
                    backColor * (0.2f - (i * 0.2f / 6)), Projectile.oldRot[i] + MathHelper.Pi, rect.Size() / 2, Projectile.scale, effect, 0);


            //绘制一层更大的在后面
            Vector2 pos = Projectile.Center - Main.screenPosition;
            float rot = Projectile.rotation + MathHelper.Pi;

            Main.spriteBatch.Draw(mainTex, pos, rect, backColor * 0.15f, rot,
                rect.Size() / 2, Projectile.scale * 1.2f, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos, rect, Color.White * Alpha * 0.8f, rot,
                rect.Size() / 2, Projectile.scale, effect, 0);

            return false;
        }
    }

    /// <summary>
    /// 三角形的粒子，炼金术元素的火
    /// </summary>
    public class FaintEagleParticle1() : BaseFrameParticle(1, 15, 2)
    {
        public override string Texture => AssetDirectory.AlchorthentSeriesItems + Name;

        public override void AI()
        {
            base.AI();
            Lighting.AddLight(Position, new Vector3(0.3f, 0.1f, 0.1f));
            Color *= 0.98f;
        }

        public override Color GetColor()
        {
            return Color;
        }
    }

    public class FaintEagleParticle2() : BaseFrameParticle(1, 9, 1)
    {
        public override string Texture => AssetDirectory.AlchorthentSeriesItems + Name;

        public override void AI()
        {
            base.AI();
            Lighting.AddLight(Position, new Vector3(0.3f, 0.1f, 0.1f));
            Color *= 0.96f;
        }

        public override Color GetColor()
        {
            return Color;
        }
    }

    public class FaintEagleParticle3() : BaseFrameParticle(1, 8, 1)
    {
        public override string Texture => AssetDirectory.AlchorthentSeriesItems + Name;

        public override void AI()
        {
            base.AI();
            Lighting.AddLight(Position, new Vector3(0.3f, 0.1f, 0.1f));
            Color *= 0.96f;
        }

        public override Color GetColor()
        {
            return Color;
        }
    }
}
