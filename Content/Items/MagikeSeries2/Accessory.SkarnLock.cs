using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.MagikeInterstitial3;
using Coralite.Content.Dusts;
using Coralite.Content.NPCs.Crystalline;
using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.KeySystem;
using Coralite.Helpers;
using InnoVault.PRT;
using System;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class SkarnLock() : BaseAccessory(ModContent.RarityType<CrystallineMagikeRarity>(), Item.sellPrice(0, 2)), IConsultableItem
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;
        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<MagikeInterstitial3Knowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<MagikeInterstitial3Page4>();

        private int EquipCount;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DamageType = DamageClass.Summon;
            Item.damage = 46;
            Item.knockBack = 1.5f;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            int damage = player.GetWeaponDamage(Item);
            float knockBack = player.GetWeaponKnockback(Item);

            EquipCount++;

            if (EquipCount < 10)
                return;

            EquipCount = 0;
            int projType = ModContent.ProjectileType<SkarnLockProj>();

            int projCount = 0;
            foreach (var proj in Main.ActiveProjectiles)
                if (proj.owner == player.whoAmI && proj.friendly && proj.type == projType && proj.ai[0] == 0)
                    projCount++;

            if (Main.rand.Next(10) < projCount || projCount >= 9)
                return;

            int tryCount = 50;
            int size = 24;
            int searchWidth = 90;
            for (int j = 0; j < tryCount; j++)
            {
                int num5 = Main.rand.Next(150 - j * 2, 300 + j * 2);
                Vector2 center = player.Center;
                center.X += Main.rand.Next(-num5, num5 + 1);
                center.Y += Main.rand.Next(-num5, num5 + 1);
                if (Collision.SolidCollision(center, size, size) || Collision.WetCollision(center, size, size))
                    continue;

                center.X += size / 2;
                center.Y += size / 2;
                if (!Collision.CanHit(new Vector2(player.Center.X, player.position.Y), 1, 1, center, 1, 1) && !Collision.CanHit(new Vector2(player.Center.X, player.position.Y - 50f), 1, 1, center, 1, 1))
                    continue;

                int x = (int)center.X / 16;
                int y = (int)center.Y / 16;
                bool flag = false;
                if (Main.rand.NextBool(3) && Main.tile[x, y] != null && Main.tile[x, y].WallType > WallID.None)
                {
                    flag = true;
                }
                else
                {
                    center.X -= searchWidth / 2;
                    center.Y -= searchWidth / 2;
                    if (Collision.SolidCollision(center, searchWidth, searchWidth))
                    {
                        center.X += searchWidth / 2;
                        center.Y += searchWidth / 2;
                        flag = true;
                    }
                    else if (Main.tile[x, y] != null && Main.tile[x, y].HasTile && Main.tile[x, y].TileType == TileID.Platforms)
                    {
                        flag = true;
                    }
                }

                if (!flag)
                    continue;

                foreach (var proj in Main.ActiveProjectiles)
                {
                    if (proj.owner == player.whoAmI && proj.friendly && proj.type == projType && proj.ai[0] == 0 && (center - proj.Center).Length() < size * 2)
                        return;
                }

                if (flag && Main.myPlayer == player.whoAmI)
                {
                    Projectile.NewProjectile(player.GetSource_Accessory(Item), center, Vector2.Zero, projType, damage, knockBack, player.whoAmI, 0);
                    break;
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CrystallineEngram>()
                .AddTile<SkarnCutterTile>()
                .Register();
        }
    }

    [VaultLoaden(AssetDirectory.MagikeSeries2Item)]
    public class SkarnLockProj : ModProjectile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public static ATex SkarnLockProjHighlight { get; private set; }

        public ref float RockStyle => ref Projectile.ai[0];
        public ref float TargetIndex => ref Projectile.ai[1];
        public ref float State => ref Projectile.ai[2];
        public ref float Timer => ref Projectile.localAI[0];

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 32;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = 60 * 5;
        }

        public override void AI()
        {
            if (Projectile.localAI[2] == 0)
            {
                TargetIndex = -1;
                Projectile.alpha = 0;
                Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                switch (RockStyle)
                {
                    default:
                    case 0://最大的石块
                        Projectile.Resize(36, 36);
                        Projectile.frame = Main.rand.Next(2);
                        break;
                    case 1://中等石块
                        Projectile.Resize(28, 28);
                        Projectile.frame = Main.rand.Next(2, 6);
                        State = 1;
                        break;
                    case 2://小石块
                        Projectile.Resize(16, 16);
                        Projectile.frame = Main.rand.Next(6, 9);
                        State = 1;
                        break;
                }

                Projectile.localAI[2] = 1;
                return;
            }

            Lighting.AddLight(Projectile.Center, Coralite.CrystallinePurple.ToVector3() * 0.4f);

            //缓慢靠近敌人，大的向目标发射小的石头
            switch (State)
            {
                default:
                case 0://刚生成，向上飘起来
                    {
                        Timer++;

                        float factor = Helper.SqrtEase(Timer / 25f);

                        Projectile.alpha = (int)(255 * factor);
                        Projectile.velocity = new Vector2(0, -factor * 2f);
                        Projectile.rotation += factor * 0.2f;

                        if (Timer > 25)
                        {
                            State = 1;
                            Timer = 0;
                        }
                    }
                    break;
                case 1://追踪敌怪
                    {
                        if (TargetIndex.GetNPCOwner(out NPC target, () =>
                        {
                            TargetIndex = -1;
                            Projectile.tileCollide = true;
                        }))
                        {
                            //缓慢追踪
                            Projectile.tileCollide = false;
                            Projectile.ChaseGradually(target.Center, 4 + RockStyle * 4, 25, 26);

                            Timer++;
                            if (Timer > 60)//分裂
                            {
                                switch (RockStyle)
                                {
                                    default:
                                    case 0:
                                        {
                                            Projectile.Kill();

                                            //生成2个中等的
                                            Vector2 dir = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2);

                                            for (int i = 0; i < 2; i++)
                                            {
                                                Projectile.NewProjectileFromThis<SkarnLockProj>(Projectile.Center, dir * 4, (int)(Projectile.damage * 0.75f), Projectile.knockBack / 3, 1);

                                                var blast = PRTLoader.NewParticle<CrystallineRockBlast>(Projectile.Center, Vector2.Zero);
                                                blast.Rotation = dir.ToRotation();

                                                dir = dir.RotatedBy(MathHelper.Pi);
                                            }
                                        }
                                        break;
                                    case 1:
                                        {
                                            Projectile.Kill();

                                            //生成3个小的
                                            Vector2 dir = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);

                                            for (int i = 0; i < 3; i++)
                                            {
                                                Projectile.NewProjectileFromThis<SkarnLockProj>(Projectile.Center, dir * 4, (int)(Projectile.damage * 0.5f), Projectile.knockBack / 3, 2);

                                                var blast = PRTLoader.NewParticle<CrystallineRockBlast>(Projectile.Center, Vector2.Zero);
                                                blast.Rotation = dir.ToRotation();

                                                dir = dir.RotatedBy(MathHelper.TwoPi / 3);
                                            }
                                        }
                                        break;
                                    case 2:
                                        if (Timer > 60 * 10)
                                        {
                                            Projectile.Kill();
                                        }
                                        break;
                                }
                            }
                        }
                        else
                        {
                            Timer++;
                            Projectile.velocity *= 0.97f;
                            //查找NPC，没有就消失
                            if (Timer > 0 && Timer % 30 == 0)
                            {
                                if (Helper.TryFindClosestEnemy(Projectile.Center, 16 * 40, n => n.CanBeChasedBy(), out NPC target2))
                                {
                                    TargetIndex = target2.whoAmI;
                                    Timer = 0;
                                    Projectile.timeLeft = 60 * 10;
                                }
                            }
                        }

                        Projectile.alpha = 255;

                        Projectile.rotation += Math.Sign(Projectile.velocity.X) * Projectile.velocity.Length() / 50f;
                    }
                    break;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 2 * (4-RockStyle); i++)
            {
               Dust d= Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), ModContent.DustType<SkarnDust>(), Helper.NextVec2Dir(1, 3), Scale: Main.rand.NextFloat(0.7f, 1f));
                d.noGravity=Main.rand.NextBool();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var rect = new Rectangle(0, Projectile.frame, 1, 9);
            float alpha = Projectile.alpha / 255f;
            Projectile.QuickFrameDraw(rect, lightColor * alpha, 0);

            Color c = Color.Lerp(Color.Transparent, Color.White, MathF.Sin((int)Main.timeForVisualEffects * 0.1f + Projectile.whoAmI * MathHelper.Pi / 5) * 0.5f + .5f) * alpha;

            SkarnLockProjHighlight.Value.QuickCenteredDraw(Main.spriteBatch, rect, Projectile.Center - Main.screenPosition, c, Projectile.rotation, Projectile.scale);

            return false;
        }
    }
}
