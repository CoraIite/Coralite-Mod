using Coralite.Content.ModPlayers;
using Coralite.Content.Prefixes.GemWeaponPrefixes;
using Coralite.Content.Tiles.RedJades;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Utilities;

namespace Coralite.Content.Items.LandOfTheLustrousSeries.Accessories
{
    public class SmokyRing : BaseGemWeapon,IHookPlayerShoot
    {
        public override void SetDefs()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Blue1, Item.sellPrice(0, 1));
            Item.SetWeaponValues(20, 4);

            Item.accessory = true;
        }

        public override int ChoosePrefix(UnifiedRandom rand)
        {
            int prefix = 0;
            var wr = new WeightedRandom<int>(rand);

            foreach (int pre in Item.GetVanillaPrefixes(PrefixCategory.Accessory))
                wr.Add(pre, 1);
            //foreach (int pre in Item.GetVanillaPrefixes(PrefixCategory.Magic))
            //    wr.Add(pre, 1);
            foreach (int pre in Item.GetVanillaPrefixes(PrefixCategory.AnyWeapon))
                wr.Add(pre, 1);

            float w = 0.5f;
            if (Main.LocalPlayer.GetModPlayer<CoralitePlayer>().HasEffect(nameof(EightsquareHand)))
                w = 3f;

            wr.Add(ModContent.PrefixType<VibrantAccessory>(), w);

            for (int i = 0; i < 50; i++)
                prefix = wr.Get();

            return prefix;
        }

        public override void DrawGemName(DrawableTooltipLine line)
        {
            DrawGemNameNormally(line, effect =>
            {
                effect.Parameters["scale"].SetValue(new Vector2(0.7f / Main.GameZoomTarget));
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
                effect.Parameters["lightRange"].SetValue(0.1f);
                effect.Parameters["lightLimit"].SetValue(0.75f);
                effect.Parameters["addC"].SetValue(0.55f);
                effect.Parameters["highlightC"].SetValue(SmokyCrystalProj.highlightC.ToVector4());
                effect.Parameters["brightC"].SetValue(SmokyCrystalProj.brightC.ToVector4());
                effect.Parameters["darkC"].SetValue(SmokyCrystalProj.darkC.ToVector4());
            }, 0.4f,
            effect =>
            {
                effect.Parameters["scale"].SetValue(new Vector2(1f / Main.GameZoomTarget));
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
                effect.Parameters["lightRange"].SetValue(0.1f);
                effect.Parameters["lightLimit"].SetValue(0.75f);
                effect.Parameters["addC"].SetValue(0.55f);
                effect.Parameters["highlightC"].SetValue(SmokyCrystalProj.highlightC.ToVector4());
                effect.Parameters["brightC"].SetValue(SmokyCrystalProj.brightC.ToVector4());
                effect.Parameters["darkC"].SetValue(SmokyCrystalProj.darkC.ToVector4());
            }, extraSize: new Point(35, 2));
        }

        public override void SpawnParticle(DrawableTooltipLine line)
        {
            SpawnParticleOnTooltipNormaly(line, SmokyCrystalProj.brightC, SmokyCrystalProj.highlightC);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SmokyCrystal>()
                .AddIngredient(ItemID.PlatinumBar, 12)
                .AddTile<MagicCraftStation>()
                .Register();
            CreateRecipe()
                .AddIngredient<SmokyCrystal>()
                .AddIngredient(ItemID.GoldBar, 12)
                .AddTile<MagicCraftStation>()
                .Register();
        }

        public void PlayerShoot(Player player, Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SmokyRingProj>()]<1)
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), position, Vector2.Zero, ModContent.ProjectileType<SmokyRingProj>(), player.GetWeaponDamage(Item), player.GetWeaponKnockback(Item), player.whoAmI);

            foreach (var p in Main.ActiveProjectiles)
                if (p.owner == player.whoAmI && p.type == type)
                {
                    (p.ModProjectile as SmokyRingProj).StartAttack();
                    break;
                }
        }
    }

    public class SmokyRingProj : BaseGemWeaponProj<SmokyRing>
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + Name;

        public ref float TargetProj => ref Projectile.ai[1];
        public ref float AttackState => ref Projectile.ai[2];
        public ref float Timer => ref Projectile.localAI[2];

        public override void InitializeGemWeapon()
        {
            TargetProj = -1;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 45;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override void BeforeMove()
        {
            if (!VaultUtils.isServer && (int)Main.timeForVisualEffects % 20 == 0 && Main.rand.NextBool(2))
            {
                float length = Main.rand.NextFloat(16, 32);
                Color c = Main.rand.NextFromList(Color.White, SmokyCrystalProj.brightC, SmokyCrystalProj.highlightC);
                var cs = CrystalShine.Spawn(Projectile.Center + Main.rand.NextVector2CircularEdge(length, length)
                     , Helper.NextVec2Dir(0.1f, 0.2f), 5, new Vector2(0.5f, 0.03f) * Main.rand.NextFloat(0.5f, 1f), c);
                cs.follow = () => Projectile.position - Projectile.oldPos[1];
                cs.TrailCount = 3;
                cs.fadeTime = Main.rand.Next(30, 50);
                cs.shineRange = 12;
            }
        }

        public override void Move()
        {
            Timer++;

            if (Timer % 20 == 0 && TargetProj == -1)//寻找宝石武器弹幕
            {
                foreach (var p in Main.ActiveProjectiles)
                    if (p.owner == Projectile.owner && p.whoAmI != Projectile.whoAmI && CoraliteSets.Projectiles.GemWeaponProj[p.type])
                    {
                        TargetProj = p.whoAmI;
                        break;
                    }
            }

            Vector2 idlePos;
            Vector2 rotVec2 = (Timer * 0.08f).ToRotationVector2() * 16 * 5;
            if (TargetProj.GetProjectileOwner(out Projectile proj, () => TargetProj = -1))
            {
                if (!CoraliteSets.Projectiles.GemWeaponProj[proj.type])
                    TargetProj = -1;

                idlePos = proj.Center;
            }
            else
                idlePos = Owner.Center;

            if (AttackTime > 0)
            {
                Vector2 dir = InMousePos - Projectile.Center;
                if (dir.Length() < 48)
                    idlePos += dir;
                else
                    idlePos += dir.SafeNormalize(Vector2.Zero) * 48;
                Projectile.netUpdate = true;
            }

            TargetPos = Vector2.SmoothStep(TargetPos, idlePos, 0.5f);
            Projectile.Center = Vector2.Lerp(Projectile.Center, TargetPos, 0.5f);
            Projectile.rotation = Projectile.rotation.AngleLerp(Timer * 0.08f, 0.2f);
            Lighting.AddLight(Projectile.Center, SmokyCrystalProj.brightC.ToVector3() / 2);
        }

        public override void Attack()
        {
            if (AttackTime > 0)
            {
                AttackTime--;
            }
        }

        public override void StartAttack()
        {
            if (AttackTime != 0)
                return;

            AttackTime = 120;
            AttackState++;
            if (AttackState > 3)
            {
                AttackState = 0;
                //射出烟水晶弹幕

                Projectile.NewProjectileFromThis<SmokyCrystalProj>(Projectile.Center, Projectile.rotation.ToRotationVector2() * 9, Projectile.damage, Projectile.knockBack);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTextureValue();
            Vector2 pos = Projectile.Center - Main.screenPosition;
            float rot = Projectile.rotation + MathHelper.PiOver2;

            const int maxFrameX = 4;

            if (AttackTime > 90 && AttackState != 0)//绘制切换动画
            {
                float f = 1 - (AttackTime - 90) / 30f;

                //绘制本体的高亮
                tex.QuickCenteredDraw(Main.spriteBatch, new Rectangle((int)AttackState, 1, maxFrameX, 2), pos, 0, Color.White * (1 - f), rot);

                //绘制上一个状态的高亮
                tex.QuickCenteredDraw(Main.spriteBatch, new Rectangle((int)AttackState - 1, 1, maxFrameX, 2), pos, 0, Color.White * f, rot);

            }
            else if (AttackTime > 60 && AttackState != 0)
            {
                float f = 1 - (AttackTime - 60) / 30f;

                //绘制本体
                tex.QuickCenteredDraw(Main.spriteBatch, new Rectangle((int)AttackState, 0, maxFrameX, 2), pos, 0, lightColor, rot);

                //绘制本体的高亮
                tex.QuickCenteredDraw(Main.spriteBatch, new Rectangle((int)AttackState, 1, maxFrameX, 2), pos, 0, Color.White * f, rot);

            }
            else
            {
                tex.QuickCenteredDraw(Main.spriteBatch, new Rectangle((int)AttackState, 0, maxFrameX, 2), pos, 0, lightColor, rot);
            }

            return false;
        }
    }

    public class SmokyCrystalProj : ModProjectile
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems+Name;

        public static Color highlightC = new(235, 232, 208);
        public static Color brightC = new(190, 120, 33);
        public static Color darkC = new(108, 61, 48);

        public ref float Target => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 10);
        }

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                Target = -1;
            }

            Timer++;

            if (!Target.GetNPCOwner(out NPC npc, () => Target = -1))
            {
                if (Timer % 4 == 0)
                {
                    if (Helper.TryFindClosestEnemy(Projectile.Center, 600, n => n.CanBeChasedBy(), out NPC target))
                        Target = target.whoAmI;
                }

                return;
            }

            Helper.ChaseGradually(Projectile, npc.Center, 15, 19, 20);

            if (!Projectile.tileCollide)//不再墙壁内就检测碰墙
                if (!Helper.PointInTile(Projectile.Center))
                    Projectile.tileCollide = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D exTex = TextureAssets.Extra[ExtrasID.SharpTears].Value;

            Vector2 toCenter = new Vector2(Projectile.width / 2, Projectile.height / 2) - Main.screenPosition;
            var origin = exTex.Size() / 2;

            for (int i = 0; i < 12; i++)
                Main.spriteBatch.Draw(exTex, Projectile.oldPos[i] + toCenter, null, Color.Lerp(brightC, darkC, i / 12f) * (0.4f - (i * 0.4f / 12)), Projectile.oldRot[i] + 1.57f, origin, new Vector2(0.5f,1), 0, 0);

            Projectile.QuickDraw(lightColor, 0);

            return false;
        }
    }

    public class SmokySmokeProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;
    }

    public class SmokyCrystalDust : ModDust
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + Name;

        public override void OnSpawn(Dust dust)
        {
            dust.frame.X = Main.rand.Next(3);
        }

        public override bool Update(Dust dust)
        {
            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            return false;
        }
    }
}
