using Coralite.Content.DamageClasses;
using Coralite.Content.Dusts;
using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using Coralite.Core.Systems.FairyCatcherSystem.NormalSkills;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using static SDL2.SDL;

namespace Coralite.Content.Items.Fairies
{
    public class LightFairyItem : BaseFairyItem
    {
        public override int FairyType => CoraliteContent.FairyType<LightFairy>();
        public override FairyRarity Rarity => FairyRarity.C;

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(copper: 50);
            Item.shoot = ModContent.ProjectileType<LightFairyProj>();
        }

        public override int[] GetFairySkills()
        {
            return [
                CoraliteContent.FairySkillType<FSkill_ShootStar>()
                ];
        }
    }

    public class LightFairy : Fairy
    {
        public override int ItemType => ModContent.ItemType<LightFairyItem>();

        public override FairyRarity Rarity => FairyRarity.C;

        public override void RegisterSpawn()
        {
            FairySpawnController.Create(Type)
                .AddCondition(FairySpawnCondition.DownedSlimeKing)
                .AddCondition(FairySpawnCondition.DayTime)
                .RegisterToWall();
        }

        public override void Catching(FairyCatcherProj catcher)
        {
            EscapeNormally(catcher, (60, 100), (0.8f, 1f));
        }

        public override void OnCatch(Player player, ref int catchPower)
        {
            targetVelocity = Helper.NextVec2Dir(0.8f, 1f);

            for (int i = 0; i < 6; i++)
            {
                Dust d = Dust.NewDustPerfect(Center, DustID.YellowStarDust, Helper.NextVec2Dir(0.5f, 1.5f), 200);
                d.noGravity = true;
            }
        }
    }

    public class LightFairyProj : BaseFairyProjectile
    {
        public override string Texture => AssetDirectory.FairyItems + "LightFairy";

        public override FairySkill[] InitSkill()
            => [
                NewSkill<FSkill_ShootStar>()
                ];

        public override void SpawnFairyDust()
        {
            switch (State)
            {
                case AIStates.Shooting:
                case AIStates.Rest:
                case AIStates.Backing:
                    if (Main.rand.NextBool(3))
                        Projectile.SpawnTrailDust(DustID.YellowStarDust, Main.rand.NextFloat(0.1f, 0.5f), 200);
                    break;
                case AIStates.Skill:
                default:
                    Projectile.SpawnTrailDust(DustID.YellowStarDust, Main.rand.NextFloat(0.1f, 0.5f), 200);
                    break;
            }
        }

        public override Vector2 GetRestSpeed()
        {
            float a = Timer * 0.2f + Projectile.identity * MathHelper.TwoPi / 6;
            return new Vector2(MathF.Cos(a), MathF.Sin(a)) * 2;
        }

        public override void AIAfter()
        {
            Lighting.AddLight(Projectile.Center, new Vector3(0.2f, 0.2f, 0.1f));
        }

        public override void OnStartUseSkill(NPC target)
        {
            SoundEngine.PlaySound(CoraliteSoundID.Ding_Item4, Projectile.Center);
        }

        public override void OnKill_DeadEffect()
        {
            SoundEngine.PlaySound(CoraliteSoundID.Ding_Item4, Projectile.Center);

            for (int i = 0; i < 12; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.YellowStarDust, Helper.NextVec2Dir(1, 2), 200);
                d.noGravity = true;
            }
        }
    }

    public class ShootStar : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        private VertexStrip _vertexStrip;

        public ref float Count => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 18);
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = TrueFairyDamage.Instance;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.width = Projectile.height = 16;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, new Vector3(0.2f, 0.2f, 0.1f));

            if (Projectile.localAI[0] == 0)
            {
                if (!VaultUtils.isServer)
                    _vertexStrip = new();
                Projectile.localAI[0] = 1;
            }

            if (Main.rand.NextBool())
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(4, 4), ModContent.DustType<GlowBall>(),
                    -Projectile.velocity * 0.1f, 0, Color.LightGoldenrodYellow, 0.15f);
            }

            //int num18 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RainbowMk2, 0f, 0f, 100, new Color(162, 42, 131), 1f);
            //Main.dust[num18].velocity *= 0.1f;
            //Main.dust[num18].velocity += Projectile.velocity * 0.2f;
            //Main.dust[num18].position.X = Projectile.Center.X + 4f + Main.rand.Next(-4, 4);
            //Main.dust[num18].position.Y = Projectile.Center.Y + Main.rand.Next(-4, 4);
            //Main.dust[num18].noGravity = true;

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.YellowStarDust, Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-3, 3), 100, Coralite.MagicCrystalPink, 1f);
                dust.noGravity = true;
            }

            for (int i = 0; i < Count; i++)
            {
                Projectile.NewProjectileFromThis<StarArrow>(Projectile.Center
                    , (i / Count * MathHelper.TwoPi).ToRotationVector2() * (3 + Count)
                    , (int)(Projectile.damage * 0.5f), Projectile.knockBack);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (_vertexStrip == null)
                return false;

            MiscShaderData miscShaderData = GameShaders.Misc["RainbowRod"];
            miscShaderData.UseSaturation(-1.8f);
            miscShaderData.UseOpacity(2f);
            miscShaderData.Apply();
            _vertexStrip.PrepareStripWithProceduralPadding(Projectile.oldPos, Projectile.oldRot, StripColors, StripWidth, -Main.screenPosition + (Projectile.Size / 2f));
            _vertexStrip.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            //ProjectilesHelper.DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, Projectile.oldPos[0]+new Vector2(8,8) - Main.screenPosition,
            //    Color.White * 0.8f, Coralite.MagicCrystalPink, 0.5f, 0f, 0.5f, 0.5f, 0f, 0f,
            //    new Vector2(0.5f, 0.5f), Vector2.One*0.5f);
            return false;
        }

        private Color StripColors(float progressOnStrip)
        {
            Color result = Color.Lerp(Color.White, Color.LightGoldenrodYellow, Utils.GetLerpValue(-0.2f, 0.5f, progressOnStrip, clamped: true)) * (1f - Utils.GetLerpValue(0f, 0.98f, progressOnStrip));
            result.A = 64;
            return result;
        }

        private float StripWidth(float progressOnStrip) => MathHelper.Lerp(4f, 16f, Utils.GetLerpValue(0f, 0.2f, progressOnStrip, clamped: true)) * Utils.GetLerpValue(0f, 0.07f, progressOnStrip, clamped: true);
    }

    public class StarArrow:ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float Timer => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.DamageType = TrueFairyDamage.Instance;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;
            Projectile.width = Projectile.height = 40;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, new Vector3(0.2f, 0.2f, 0.1f));

            Timer++;
            Projectile.velocity *= 0.92f;
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Timer> 14)
                Projectile.Kill();
        }

        public override void OnKill(int timeLeft)
        {
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.9f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = CoraliteAssets.Trail.ArrowSPA.Value;

            float alpha = 1 - Timer / 14;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Color c = Color.LightGoldenrodYellow * 0.75f * alpha;
            float scale = Projectile.scale * 0.4f;
            Vector2 Scale = new Vector2(0.8f, alpha) * scale;

            Main.spriteBatch.Draw(tex, pos, null, c, Projectile.rotation, tex.Size() / 2
                , Scale, 0, 0);

            c.A = 0;
            Main.spriteBatch.Draw(tex, pos, null, c, Projectile.rotation, tex.Size() / 2
                , Scale, 0, 0);

            return false;
        }
    }

    public class FSkill_ShootStar : FSkill_ShootProj
    {
        public override string Texture => AssetDirectory.FairySkillIcons + "Light";

        public LocalizedText Description { get; set; }

        public override Color SkillTextColor => Color.Yellow;
        protected override float ShootSpeed => 16;

        protected override int ProjType => ModContent.ProjectileType<ShootStar>();

        protected override float ChaseDistance => 300;

        public override string GetSkillTips(Player player, FairyIV iv)
        {
            int level = iv.SkillLevel;
            if (player.TryGetModPlayer(out FairyCatcherPlayer fcp))
                level = fcp.GetFairySkillBonus(Type, level);

            return Description.Format(GetEXProjCount(level), GetDamageBonus(iv.Damage, level));
        }

        public int GetEXProjCount(int level)
        {
            int count = 4;
            if (level > 4)
                count++;
            if (level > 7)
                count++;
            if (level > 10)
                count += 2;

            return count;
        }

        public override void ShootProj(BaseFairyProjectile fairyProj, Vector2 center, Vector2 velocity, int damage)
        {
            int level = fairyProj.FairyItem.FairyIV.SkillLevel;
            if (fairyProj.Owner.TryGetModPlayer(out FairyCatcherPlayer fcp))
                level = fcp.GetFairySkillBonus(Type, level);

            int count = GetEXProjCount(level);
            int proj = fairyProj.Projectile.NewProjectileFromThis(fairyProj.Projectile.Center
                 , velocity, ProjType, damage, fairyProj.Projectile.knockBack,count);
        }
    }
}
