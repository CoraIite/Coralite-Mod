using Coralite.Content.Dusts;
using Coralite.Content.NPCs.Magike;
using Coralite.Content.Particles;
using Coralite.Content.Tiles.RedJades;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class AmethystNecklace : BaseGemWeapon
    {
        public bool ShootSound = true;

        public override void SetDefs()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Blue1, Item.sellPrice(0, 1));
            Item.SetWeaponValues(19, 4);
            Item.useTime = Item.useAnimation = 28;
            Item.mana = 16;

            Item.shoot = ModContent.ProjectileType<AmethystNecklaceProj>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.noUseGraphic = false;
                Item.useStyle = ItemUseStyleID.HoldUp;
                if (ShootSound)
                    ShootSound = false;
                else
                    ShootSound = true;
            }
            else
            {
                Item.noUseGraphic = true;
                Item.useStyle = ItemUseStyleID.Shoot;
            }
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer != player.whoAmI)
                return false;

            if (player.altFunctionUse==2)
                return false;

            if (player.ownedProjectileCounts[type] < 1)
                Projectile.NewProjectile(source, position, Vector2.Zero, type, 0, knockback, player.whoAmI);
            else
            {
                foreach (var proj in Main.projectile.Where(p => p.active && p.owner == player.whoAmI && p.type == type))
                    (proj.ModProjectile as AmethystNecklaceProj).StartAttack();
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Amethyst)
                .AddIngredient(ItemID.DemoniteBar,5)
                .AddIngredient(ItemID.GoldCoin)
                .AddTile<MagicCraftStation>()
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.Amethyst)
                .AddIngredient(ItemID.CrimtaneBar,5)
                .AddIngredient(ItemID.GoldCoin)
                .AddTile<MagicCraftStation>()
                .Register();
        }

        public override void DrawGemName(DrawableTooltipLine line)
        {
            DrawGemNameNormally(line, effect =>
            {
                effect.Parameters["scale"].SetValue(new Vector2(0.7f / Main.GameZoomTarget));
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
                effect.Parameters["lightRange"].SetValue(0.1f);
                effect.Parameters["lightLimit"].SetValue(0.55f);
                effect.Parameters["addC"].SetValue(0.75f);
                effect.Parameters["highlightC"].SetValue((AmethystLaser.brightC * 1.3f).ToVector4());
                effect.Parameters["brightC"].SetValue(AmethystLaser.brightC.ToVector4());
                effect.Parameters["darkC"].SetValue(new Color(110, 60, 110).ToVector4());
            });
        }

        public override void SpawnParticle(DrawableTooltipLine line)
        {
            SpawnParticleOnTooltipNormaly(line, AmethystLaser.brightC, AmethystLaser.highlightC);
        }
    }

    public class AmethystNecklaceProj : BaseGemWeaponProj<AmethystNecklace>
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems+Name;

        public ref float AttackCD => ref Projectile.ai[1];
        public ref float LengthToCenter => ref Projectile.ai[2];
        public ref float Rot => ref Projectile.localAI[2];

        private float factorTop;
        private float factorBottom;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.OnlyPosition, 3);
        }

        public override void Initialize()
        {
            TargetPos = Owner.Center;
        }

        public override void BeforeMove()
        {
            if ((int)Main.timeForVisualEffects % 20 == 0 && Main.rand.NextBool(2))
            {
                float length = Main.rand.NextFloat(48, 64);
                Color c = Main.rand.NextFromList(Color.White, AmethystLaser.brightC, AmethystLaser.highlightC);
                var cs = CrystalShine.Spawn(Projectile.Center + Main.rand.NextVector2CircularEdge(length, length)
                     , Helper.NextVec2Dir(0.1f, 0.2f), 5, new Vector2(0.5f, 0.03f) * Main.rand.NextFloat(0.5f, 1f), c);
                cs.follow = () => Projectile.position - Projectile.oldPos[1];
                cs.TrailCount = 3;
                cs.fadeTime = Main.rand.Next(40, 70);
                cs.shineRange = 12;
            }
        }

        public override void Move()
        {
            Vector2 idlePos = Owner.Center + new Vector2(-OwnerDirection * 16, 0);
            for (int i = 0; i < 12; i++)//检测头顶4个方块并尝试找到没有物块阻挡的那个
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

            float lerpFactor = 0.3f;
            if (AttackCD > 0)
            {
                idlePos += (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero) * 48;
                lerpFactor = 0.1f;
            }

            TargetPos = Vector2.SmoothStep(TargetPos, idlePos, lerpFactor);
            Projectile.Center = Vector2.Lerp(Projectile.Center, TargetPos, 0.5f);

            Lighting.AddLight(Projectile.Center, AmethystLaser.brightC.ToVector3());
        }

        public override void Attack()
        {
            if (AttackTime > 0)
            {
                factorTop = 1 - AttackTime / Owner.itemTimeMax;
                LengthToCenter = Helper.Lerp(54, 32,factorTop);
                Rot += 0.05f + (1 - factorTop) * 0.2f;
                Projectile.rotation = Projectile.rotation.AngleLerp((Main.MouseWorld - Projectile.Center).ToRotation(), 0.2f);
                if (AttackTime == 1)//生成射线
                {
                    Projectile.NewProjectileFromThis<AmethystLaser>(Projectile.Center, Vector2.Zero, Owner.GetWeaponDamage(Owner.HeldItem)
                        , Projectile.knockBack, Projectile.whoAmI,AmethystLaser.TotalAttackTime+AmethystLaser.delayTime);
                    Helper.PlayPitched("Crystal/CrystalStrike", 0.4f, 0, Projectile.Center);
                }

                AttackTime--;
                return;
            }

            if (AttackCD > 0)
            {
                Rot += 0.01f;
                LengthToCenter = Helper.Lerp(32, 48, 0.1f);
                Projectile.rotation = Projectile.rotation.AngleTowards((Main.MouseWorld - Projectile.Center).ToRotation(), 0.015f);
                if (AttackCD < AmethystLaser.delayTime)
                {
                    factorBottom = 1 - AttackCD / AmethystLaser.delayTime;
                }

                AttackCD--;
            }
            else
            {
                LengthToCenter = Helper.Lerp(LengthToCenter, 54, 0.2f);
                Projectile.rotation = Projectile.rotation.AngleLerp( OwnerDirection > 0 ? 0 : MathHelper.Pi, 0.1f);
                Rot += 0.03f;
            }
        }

        public override void StartAttack()
        {
            if (AttackCD > 0)
                return;

            AttackTime = Owner.itemTimeMax;
            AttackCD = AmethystLaser.TotalAttackTime + AmethystLaser.delayTime;
            factorTop = 0;
            factorBottom = 0;
        }

        public void TurnToDelay()
        {
            AttackCD = AmethystLaser.delayTime;
        }

        public void GetCrystalDrawData(int index, out Vector2 pos, out float scale)
        {
            Vector2 dir = (Rot + index * MathHelper.TwoPi / 6).ToRotationVector2();

            Matrix zRot = Matrix.CreateRotationZ(Projectile.rotation);
            Matrix yRot = Matrix.CreateRotationY(-0.9f);

            Vector3 vector3D = Vector3.Transform(dir.Vec3(), yRot);
            vector3D = Vector3.Transform(vector3D, zRot);

            float k1 = -1000 / (vector3D.Z - 1000);
            Vector2 CircleDir = k1 * new Vector2(vector3D.X, vector3D.Y)* LengthToCenter;
            pos = Projectile.Center + CircleDir;
            scale = 1f - vector3D.Z * 0.2f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2[] positions = new Vector2[6];
            float[] scales = new float[6];

            for (int i = 0; i < 6; i++)
            {
                GetCrystalDrawData(i, out Vector2 pos, out float scale);
                positions[i] = pos;
                scales[i] = scale;
            }

            //绘制连线
            if (AttackCD > 0)
                DrawLine(positions, scales);
            //绘制自己
            DrawSelf(lightColor, positions, scales);
            return false;
        }

        public void DrawLine(Vector2[] positions, float[] scales)
        {
            Texture2D lineTex = TextureAssets.FishingLine.Value;

            for (int i = 0; i < 6; i++)
            {
                Vector2 bottomPos = positions[i] - Main.screenPosition;
                Vector2 topPos = positions[(i + 2) % 6] - Main.screenPosition;

                Vector2 tip = Vector2.Lerp(bottomPos, topPos, factorTop);
                Vector2 bot = Vector2.Lerp(bottomPos, topPos, factorBottom);

                float length = Vector2.Distance(bot, tip);

                var rect = new Rectangle((int)bot.X, (int)bot.Y, lineTex.Width, (int)length);

                Main.spriteBatch.Draw(lineTex, rect, null, AmethystLaser.brightC ,
                    (topPos - bottomPos).ToRotation()-1.57f, new Vector2(lineTex.Width / 2, 0), 0, 0);
            }
        }

        public void DrawSelf(Color lightColor, Vector2[] positions, float[] scales)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var origin = mainTex.Size() / 2;

            for (int i = 0; i < 6; i++)
            {
                Vector2 pos = positions[i];
                float rot = (pos - Projectile.Center).ToRotation();
                float scale = scales[i];

                Main.spriteBatch.Draw(mainTex, pos - Main.screenPosition, null, AmethystLaser.brightC * 0.5f, rot, origin, scale*1.1f, 0, 0);
                Main.spriteBatch.Draw(mainTex, pos - Main.screenPosition, null, lightColor, rot, origin, scale, 0, 0);
            }
        }
    }

    public class AmethystLaser : ModProjectile, IDrawAdditive
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "LaserCore";

        public const int PerManaCostTime = 30;
        public const int TotalAttackTime = PerManaCostTime * 10;
        public const int delayTime = 20;

        public ref float Owner => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];
        public ref float LaserRotation => ref Projectile.ai[2];
        public ref float LaserHeight => ref Projectile.localAI[0];
        public ref float HitCount => ref Projectile.localAI[1];

        public Vector2 endPoint;
        private SlotId soundSlot;

        public static Color highlightC = new Color(235, 201, 238);
        public static Color brightC = new Color(200, 123, 193);
        public static Color darkC = new Color(71, 34, 76);

        public Vector2 rand = Main.rand.NextVector2CircularEdge(64, 64);

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.timeLeft = 400;
            Projectile.width = Projectile.height = 2;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.usesIDStaticNPCImmunity = true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Timer > delayTime)
            {
                float a = 0;
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, endPoint, 16, ref a);
            }

            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Main.player[Projectile.owner].HeldItem.ModItem is AmethystNecklace an && an.ShootSound)
                soundSlot = Helper.PlayPitched("Crystal/CrystalLaser", 0.2f, 0, Projectile.Center);
        }

        public override void AI()
        {
            Projectile owner;
            if (!Main.projectile.IndexInRange((int)Owner))
            {
                Projectile.Kill();
                return;
            }

            owner = Main.projectile[(int)Owner];
            if (!owner.active || owner.owner != Projectile.owner || owner.type != ModContent.ProjectileType<AmethystNecklaceProj>())
            {
                Projectile.Kill();
                return;
            }

            Projectile.Center = owner.Center;
            LaserRotation = owner.rotation;
            HitCount = 5;
            for (int k = 0; k < 160; k++)
            {
                Vector2 posCheck = Projectile.Center + Vector2.UnitX.RotatedBy(LaserRotation) * k * 8;

                if (Helper.PointInTile(posCheck) || k == 159)
                {
                    endPoint = posCheck;
                    break;
                }
            }

            int width = (int)(Projectile.Center - endPoint).Length();
            Vector2 dir = Vector2.UnitX.RotatedBy(LaserRotation);
            Color color = brightC;

            do
            {
                float height = 16 * LaserHeight;
                float min = width / 140f;
                float max = width / 120f;

                for (int i = 0; i < width; i += 16)
                {
                    Lighting.AddLight(Projectile.position + Vector2.UnitX.RotatedBy(LaserRotation) * i, color.ToVector3() * height * 0.030f);
                    if (Main.rand.NextBool(30))
                    {
                        SpawnTriangleParticle(Projectile.Center + dir * i + Main.rand.NextVector2Circular(8, 8)
                            , dir * Main.rand.NextFloat(min, max));
                    }
                }

                if (Timer > TotalAttackTime)
                {
                    LaserHeight = Helper.Lerp(0, 2, 1 - (Timer - TotalAttackTime) / delayTime);
                    break;
                }

                if (Timer > delayTime)
                {
                    if (SoundEngine.TryGetActiveSound(soundSlot, out var sound2))
                        sound2.Position = Projectile.Center;
                    LaserHeight = Helper.Lerp(2, 1, 0.2f);
                    if ((Timer - delayTime) % (PerManaCostTime/2) == 0)
                    {
                        if (!Main.player[Projectile.owner].CheckMana(4, true))
                        {
                            (owner.ModProjectile as AmethystNecklaceProj).TurnToDelay();
                            Timer = delayTime;
                        }
                        else
                            Main.player[Projectile.owner].manaRegenDelay = (int)Main.player[Projectile.owner].maxRegenDelay;
                    }

                    if (VisualEffectSystem.HitEffect_Lightning && Main.rand.NextBool(3))
                    {
                        Color c1 = highlightC;
                        c1.A = 125;
                        Color c2 = brightC;
                        c2.A = 125;
                        Color c3 = darkC;
                        c3.A = 100;
                        Color c = Main.rand.NextFromList(c1, c2, c3);

                        Color c4 = darkC;
                        c4.A = 0;
                        LightTrailParticle.Spawn(endPoint, (LaserRotation + MathHelper.Pi + Main.rand.NextFloat(-0.3f, 0.3f)).ToRotationVector2() * Main.rand.NextFloat(6f, 8f)
                            , c, Main.rand.NextFloat(0.3f, 0.6f), c4, 8);
                        SpawnTriangleParticle(endPoint, Helper.NextVec2Dir(0.5f, 2f));
                    }

                    break;
                }

                if (Timer == delayTime / 2)
                {
                    for (int i = 0; i < width - 128; i += 24)
                    {
                        Vector2 pos = Projectile.Center + dir * i + Main.rand.NextVector2Circular(8, 8);
                        if (Main.rand.NextBool())
                        Dust.NewDustPerfect(pos, ModContent.DustType<GlowBall>(),
                            dir * Main.rand.NextFloat(width / 160f), 0, color, 0.35f);
                        else
                            SpawnTriangleParticle(pos, dir * Main.rand.NextFloat(width / 160f));
                    }
                }

                if (SoundEngine.TryGetActiveSound(soundSlot, out var sound))
                {
                    sound.Volume -= 1f / delayTime;
                }

                LaserHeight -= 1f / delayTime;

            } while (false);

            Timer--;
            if (Timer < 1)
                Projectile.Kill();
            Projectile.UpdateFrameNormally(8, 19);
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

        public override void OnKill(int timeLeft)
        {
            if (SoundEngine.TryGetActiveSound(soundSlot, out var sound))
                sound.Stop();
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (HitCount < 4)
                modifiers.SourceDamage -= 1 / HitCount;
            if (HitCount > 2)
                HitCount--;
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            Texture2D laserTex = Projectile.GetTexture();
            Texture2D flowTex = CrystalLaser.FlowTex.Value;

            rand += LaserRotation.ToRotationVector2()*3;
            Color color = darkC;

            float height = LaserHeight * laserTex.Height / 4f;
            int width = (int)(Projectile.Center - endPoint).Length();   //这个就是激光长度

            Vector2 startPos = Projectile.Center;
            Vector2 endPos = endPoint - Main.screenPosition;

            var laserTarget = new Rectangle((int)startPos.X, (int)startPos.Y, width, (int)(height));
            var flowTarget = new Rectangle((int)startPos.X, (int)startPos.Y, width, (int)(height * 0.5f));

            var laserSource = new Rectangle((int)(Projectile.timeLeft / 30f * laserTex.Width), 0, laserTex.Width, laserTex.Height);
            var flowSource = new Rectangle((int)(Projectile.timeLeft / 45f * flowTex.Width), 0, flowTex.Width, flowTex.Height);

            var origin = new Vector2(0, laserTex.Height / 2);
            var origin2 = new Vector2(0, flowTex.Height / 2);
            
            Helper.DrawCrystal(spriteBatch, Projectile.frame, Projectile.Center + rand, Vector2.One*0.8f
                , (float)Main.timeForVisualEffects * 0.02f + Projectile.whoAmI / 3f
                , highlightC, brightC, darkC, () =>
                {
                    //绘制流动效果
                    spriteBatch.Draw(laserTex, laserTarget, laserSource, Color.White, LaserRotation, origin, 0, 0);
                    spriteBatch.Draw(flowTex, flowTarget, flowSource, Color.White * 0.3f, LaserRotation, origin2, 0, 0);
                }, sb =>
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        spriteBatch.End();
                        spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);

                        Texture2D texDark = CrystalLaser.BlackTex.Value;
                        float opacity = height / (laserTex.Height / 2f) * 0.5f;

                        spriteBatch.Draw(texDark, startPos-Main.screenPosition, null, Color.White * opacity, LaserRotation, new Vector2(texDark.Width / 2, 0), 50, 0, 0);
                        spriteBatch.Draw(texDark, startPos - Main.screenPosition, null, Color.White * opacity, LaserRotation - 3.14f, new Vector2(texDark.Width / 2, 0), 50, 0, 0);
                    }

                    spriteBatch.End();
                    spriteBatch.Begin(default, BlendState.Additive, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);
                },0.1f,0.35f,0f);

            //绘制主体光束
            Texture2D bodyTex = CrystalLaser.LaserBodyTex.Value;

            color = brightC;

            startPos = Projectile.Center-Main.screenPosition;

            laserTarget = new Rectangle((int)startPos.X, (int)startPos.Y, width, (int)(height * 0.5f));
            spriteBatch.Draw(bodyTex, laserTarget, laserSource, color, LaserRotation, new Vector2(0, bodyTex.Height / 2), 0, 0);
            spriteBatch.Draw(bodyTex, laserTarget, laserSource, color, LaserRotation, new Vector2(0, bodyTex.Height / 2), 0, 0);

            //绘制用于遮盖首尾的亮光
            Texture2D glowTex = CrystalLaser.GlowTex.Value;
            Texture2D starTex = CrystalLaser.StarTex.Value;

            spriteBatch.Draw(glowTex, endPos, null, color * (height * 0.02f), 0, glowTex.Size() / 2, 0.4f, 0, 0);
            spriteBatch.Draw(starTex, endPos, null, color , Main.GlobalTimeWrappedHourly, starTex.Size() / 2, 0.16f, 0, 0);

            spriteBatch.Draw(glowTex, startPos, null, color * (height * 0.02f), 0, glowTex.Size() / 2, 0.5f, 0, 0);
            spriteBatch.Draw(starTex, startPos, null, color , Main.GlobalTimeWrappedHourly * -1.5f, starTex.Size() / 2, 0.25f, 0, 0);
        }
    }
}
