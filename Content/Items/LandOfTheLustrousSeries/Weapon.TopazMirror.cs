using Coralite.Content.Tiles.RedJades;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Utilities;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class TopazMirror : BaseGemWeapon
    {
        public override void SetDefs()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Cyan9, Item.sellPrice(0, 20));
            Item.SetWeaponValues(100, 4);
            Item.useTime = Item.useAnimation = 28;
            Item.mana = 20;

            Item.shoot = ModContent.ProjectileType<TopazMirrorProj>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer != player.whoAmI)
                return false;

            if (player.ownedProjectileCounts[type] < 1)
                Projectile.NewProjectile(source, position, Vector2.Zero, type, 0, knockback, player.whoAmI);
            else
            {
                foreach (var proj in Main.projectile.Where(p => p.active && p.owner == player.whoAmI && p.type == type))
                    (proj.ModProjectile as TopazMirrorProj).StartAttack();
            }

            return false;
        }

        public override void DrawGemName(DrawableTooltipLine line)
        {
            DrawGemNameNormally(line, effect =>
            {
                effect.Parameters["scale"].SetValue(new Vector2(0.4f) / Main.GameZoomTarget);
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.015f);
                effect.Parameters["lightRange"].SetValue(0.1f);
                effect.Parameters["lightLimit"].SetValue(0.15f);
                effect.Parameters["addC"].SetValue(0.7f);
                effect.Parameters["highlightC"].SetValue(TopazProj.highlightC.ToVector4());
                effect.Parameters["brightC"].SetValue(TopazProj.brightC.ToVector4());
                effect.Parameters["darkC"].SetValue(TopazProj.darkC.ToVector4());
            }, 0.2f);
        }

        public override void SpawnParticle(DrawableTooltipLine line)
        {
            SpawnParticleOnTooltipNormaly(line, TopazProj.brightC, TopazProj.highlightC);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Sapphire)
                .AddIngredient(ItemID.MarbleBlock, 30)
                .AddIngredient(ItemID.FallenStar)
                .AddTile<MagicCraftStation>()
                .Register();
        }
    }

    public class TopazMirrorProj : BaseGemWeaponProj<TopazMirror>
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + "TopazMirror";

        public int attackType;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 4);
        }

        public override void BeforeMove()
        {
            if ((int)Main.timeForVisualEffects % 30 == 0 && Main.rand.NextBool(2))
            {
                float length = Main.rand.NextFloat(24, 32);
                Color c = Main.rand.NextFromList(Color.White, TopazProj.brightC, TopazProj.highlightC);
                var cs = CrystalShine.Spawn(Projectile.Center + Main.rand.NextVector2CircularEdge(length, length)
                     , Helper.NextVec2Dir(0.1f, 0.2f), 5, new Vector2(0.5f, 0.03f) * Main.rand.NextFloat(0.5f, 1f), c);
                cs.follow = () => Projectile.position - Projectile.oldPos[1];
                cs.TrailCount = 3;
                cs.fadeTime = Main.rand.Next(40, 70);
                cs.shineRange = 12;
            }

            Lighting.AddLight(Projectile.Center, new Vector3(0.4f, 0.3f, 0));
        }

        public override void Move()
        {
            Vector2 idlePos = Owner.Center + new Vector2(Owner.direction * 32, 0);

            for (int i = 0; i < 8; i++)//检测头顶4个方块并尝试找到没有物块阻挡的那个
            {
                Tile idleTile = Framing.GetTileSafely(idlePos.ToTileCoordinates());
                if (idleTile.HasTile && Main.tileSolid[idleTile.TileType] && !Main.tileSolidTop[idleTile.TileType])
                {
                    idlePos -= new Vector2(0, -4);
                    break;
                }
                else
                    idlePos += new Vector2(0, -4);
            }

            if (AttackTime != 0)
            {
                Vector2 dir = Main.MouseWorld - Projectile.Center;

                if (dir.Length() < 80)
                    idlePos += dir;
                else
                    idlePos += dir.SafeNormalize(Vector2.Zero) * 80;
            }

            if (AttackTime>0)
            {
                AttackTime--;
            }

            TargetPos = Vector2.Lerp(TargetPos, idlePos, 0.3f);
            Projectile.Center = Vector2.Lerp(Projectile.Center, TargetPos, 0.5f);
            Projectile.rotation = Owner.velocity.X / 40;
        }

        public override void StartAttack()
        {
            attackType--;
            if (attackType < 0)
            {
                var wr = new WeightedRandom<int>();
                wr.Add((int)TopazProj.AttackType.ShootSword, 1);
                //wr.Add((int)TopazProj.AttackType.ShootSword, 1);
                attackType = wr.Get();
            }

            int time = Owner.itemTimeMax;

            switch (attackType)
            {
                default:
                    break;
                case (int)TopazProj.AttackType.ShootSword:
                    time = (int)(time * 1.5f);
                    break;
            }

            Projectile.NewProjectileFromThis<TopazProj>(Projectile.Center, Vector2.Zero, 0, Projectile.knockBack
                , Projectile.whoAmI, attackType, time);

            AttackTime = time;
            Owner.itemTime = time;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawShadowTrails(SapphireProj.darkC, 0.3f, 0.3f / 4, 0, 4, 1, 0, -1);
            Projectile.QuickDraw(lightColor, 0);

            if (AttackTime > 0)
            {
                Vector2 pos = Projectile.Center - Main.screenPosition;
                float factor = AttackTime / Owner.itemTimeMax;
                Helper.DrawPrettyStarSparkle(1, 0, pos, SapphireProj.highlightC, SapphireProj.brightC
                    , factor, 0, 0.5f, 0.5f, 1
                    , 0, new Vector2(2f, 0.8f), Vector2.One / 3);
                Helper.DrawPrettyStarSparkle(1, 0, pos, SapphireProj.highlightC, SapphireProj.brightC
                    , factor, 0, 0.5f, 0.5f, 1
                    , 0, new Vector2(3f, 1f), Vector2.One / 3);
            }
            return false;
        }
    }

    public class TopazProj : BaseHeldProj
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float POwner => ref Projectile.ai[0];
        public ref float State => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[2];

        private float alpha;

        public enum AttackType
        {
            /// <summary>
            /// 裂天剑
            /// </summary>
            ShootSword,
            /// <summary>
            /// 血荆棘
            /// </summary>
            SpawnSpike,
        }

        public static Color highlightC = new Color(255, 234, 186);
        public static Color brightC = new Color(255, 132, 0);
        public static Color darkC = new Color(136, 47, 0);

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            if (!POwner.GetProjectileOwner(out Projectile owner, Projectile.Kill))
                return;

            Projectile.Center = owner.Center;

            Timer--;

            Attack();

            if (Timer < 0)
                Projectile.Kill();
        }

        public void Attack()
        {
            switch ((int)State)
            {
                default:
                    Projectile.Kill(); return;
                case (int)AttackType.ShootSword:
                    ShootSword();
                    break;
                case (int)AttackType.SpawnSpike:
                    SpawnSpike();
                    break;
            }
        }

        public void ShootSword()
        {
            if ((int)Timer % (Owner.itemTimeMax*3 / 8) == 0)
            {
                int count = (int)Timer / (Owner.itemTimeMax*3 / 8);
                Vector2 pos = Projectile.Center + ( count * MathHelper.PiOver2).ToRotationVector2() * 64;
                Vector2 dir = (Main.MouseWorld - pos).SafeNormalize(Vector2.Zero);
               int index= Projectile.NewProjectileFromThis<TopazFlySword>(pos,dir  * 15
                    , Owner.GetWeaponDamage(Owner.HeldItem), Projectile.knockBack);
                Main.projectile[index].rotation = dir.ToRotation() + 0.785f;
            }
        }

        public void SpawnSpike()
        {

        }

        public static void SpawnTriangleParticle(Vector2 pos, Vector2 velocity)
        {
            Color c1 = highlightC;
            c1.A = 125;
            Color c2 = brightC;
            c2.A = 125;
            Color c3 = darkC;
            c3.A = 100;
            Color c = Main.rand.NextFromList(highlightC, brightC, c1, c2, c3);
            CrystalTriangle.Spawn(pos, velocity, c, 9, Main.rand.NextFloat(0.05f, 0.3f));
        }

        public Texture2D GetTexture(out float extraRot)
        {
            extraRot = 0;
            switch ((int)State)
            {
                default:
                    return Projectile.GetTexture();
                case (int)AttackType.ShootSword:
                    extraRot = -0.785f;
                    Main.instance.LoadItem(ItemID.SkyFracture);
                    return TextureAssets.Item[ItemID.SkyFracture].Value;
                case (int)AttackType.SpawnSpike:
                    Main.instance.LoadItem(ItemID.SharpTears);
                    return TextureAssets.Item[ItemID.SkyFracture].Value;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = GetTexture(out float exRot);


            return false;
        }
    }

    public class TopazFlySword : ModProjectile
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + Name;

        public ref float Timer => ref Projectile.ai[0];
        public Vector2 rand = Main.rand.NextVector2CircularEdge(64, 64);

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 4);
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;

            Projectile.timeLeft = 1200;
            Projectile.extraUpdates = 2;
            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }

        #region AI

        public override void AI()
        {
            Default();

            if (Projectile.timeLeft % 5 == 0)
                TopazProj.SpawnTriangleParticle(Projectile.Center + Main.rand.NextVector2Circular(12, 12), Projectile.velocity * Main.rand.NextFloat(0.2f, 0.4f));
            if (Main.rand.NextBool(7))
                Projectile.SpawnTrailDust(8f, DustID.YellowTorch, Main.rand.NextFloat(0.2f, 0.4f));

            Timer++;
        }

        protected void Default()
        {
            if (Timer == 2)
            {
                Projectile.hide = false;
                Projectile.penetrate = 1;

                SpawnFractureDust(3f, 1f);

                Projectile.netUpdate = true;
                Projectile.rotation = Projectile.velocity.ToRotation() + 0.785f;  //pi / 4
                return;
            }

            if (Timer==7)
            {
                SpawnFractureDust(1.6f, 1f);
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + 0.785f;
        }

        protected void SpawnFractureDust(float widthDarker, float widthLighter)
        {
            float r = Projectile.velocity.ToRotation();
            for (int i = 0; i < 12; i++)
            {
                r += MathHelper.TwoPi / 12;
                Vector2 dir = r.ToRotationVector2() * Helper.EllipticalEase(2.1f + MathHelper.TwoPi/12 * i, 1f, 3f) * 0.5f;

                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.YellowTorch, dir * widthDarker, 0, default, 1.8f);
                dust.noGravity = true;
            }
        }

        #endregion

        #region Draw

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 origin = mainTex.Size() / 2;

            Vector2 toCenter = Projectile.Size / 2;
            for (int i = 0; i < 4; i++)     //这里是绘制类似于影子拖尾的东西，简单讲就是随机位置画几个透明度低的自己
            {
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, null,
                      TopazProj.highlightC * (0.5f - i * 0.5f / 4), Projectile.oldRot[i], origin, Projectile.scale + i * 0.2f, SpriteEffects.None, 0);
            }

            rand -= Projectile.velocity / 20;

            Helper.DrawCrystal(Main.spriteBatch, Projectile.frame, Projectile.Center + rand, new Vector2(0.7f)
                , (float)(Main.timeForVisualEffects + Projectile.timeLeft) * (Main.gamePaused ? 0.02f : 0.01f) + Projectile.whoAmI / 3f
                , TopazProj.highlightC, TopazProj.brightC, TopazProj.darkC, () =>
                {
                    Main.spriteBatch.Draw(Projectile.GetTexture(), Projectile.Center, null,
                         Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
                }, sb =>
                {
                    sb.End();
                    sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                }, 0.1f, 0.15f, 0.7f);
            return false;
        }

        #endregion

        public override void OnKill(int timeLeft)
        {
            if (VisualEffectSystem.HitEffect_Dusts)
                for (int i = 0; i < 6; i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.YellowTorch, Helper.NextVec2Dir(2, 4), Scale: Main.rand.NextFloat(1f, 1.5f));
                    d.noGravity = true;
                }

            if (VisualEffectSystem.HitEffect_SpecialParticles)
                for (int i = 0; i < 3; i++)
                {
                    Vector2 dir = Helper.NextVec2Dir();
                    TopazProj.SpawnTriangleParticle(Projectile.Center + dir * Main.rand.NextFloat(6, 12), dir * Main.rand.NextFloat(1f, 3f));
                }

            if (Projectile.ai[0] != 1)
                SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
        }
    }
}
