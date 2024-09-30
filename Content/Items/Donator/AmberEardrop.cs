using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Content.Items.Icicle;
using Coralite.Content.Items.LandOfTheLustrousSeries;
using Coralite.Content.NPCs.GlobalNPC;
using Coralite.Content.Tiles.RedJades;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.UI.Chat;

namespace Coralite.Content.Items.Donator
{
    public class AmberEardrop : BaseGemWeapon
    {
        public override string Texture => AssetDirectory.Donator + Name;

        public override void SetDefs()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Orange3, Item.sellPrice(0, 3));
            Item.SetWeaponValues(23, 4);
            Item.useTime = Item.useAnimation = 35;
            Item.mana = 6;

            Item.shoot = ModContent.ProjectileType<AmberEardropProj>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer != player.whoAmI)
                return false;

            if (player.altFunctionUse == 2)
                return false;

            if (player.ownedProjectileCounts[type] < 1)
                Projectile.NewProjectile(source, position, Vector2.Zero, type, 0, knockback, player.whoAmI);
            else
            {
                foreach (var proj in Main.projectile.Where(p => p.active && p.owner == player.whoAmI && p.type == type))
                    (proj.ModProjectile as AmberEardropProj).StartAttack();
            }

            return false;
        }

        public override void DrawGemName(DrawableTooltipLine line)
        {
            SpriteBatch sb = Main.spriteBatch;
            Effect effect = Filters.Scene["Crystal"].GetShader().Shader;

            rand.X += 0.6f;
            rand.Y += 0.01f;
            if (rand.X > 100000)
                rand.X = 10;

            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            Texture2D noiseTex = GemTextures.CellNoise.Value;//[(int)(Main.timeForVisualEffects / 7) % 20].Value;

            effect.Parameters["transformMatrix"].SetValue(projection);
            effect.Parameters["basePos"].SetValue(rand + new Vector2(line.X, line.Y));
            effect.Parameters["scale"].SetValue(new Vector2(5f / Main.GameZoomTarget));
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
            effect.Parameters["lightRange"].SetValue(0.05f);
            effect.Parameters["lightLimit"].SetValue(0.75f);
            effect.Parameters["addC"].SetValue(0.55f);
            effect.Parameters["highlightC"].SetValue(AmberProj.highlightC.ToVector4());
            effect.Parameters["brightC"].SetValue(AmberProj.brightC.ToVector4());
            effect.Parameters["darkC"].SetValue(AmberProj.darkC.ToVector4());

            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, effect, Main.UIScaleMatrix);

            Main.graphics.GraphicsDevice.Textures[1] = noiseTex;
            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, line.Font, line.Text, new Vector2(line.X, line.Y)
                , Color.White, line.Rotation, line.Origin, line.BaseScale, line.MaxWidth, line.Spread);

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);
        }

        public override void SpawnParticle(DrawableTooltipLine line)
        {
            SpawnParticleOnTooltipNormaly(line, AmberProj.brightC, AmberProj.highlightC);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Amber)
                .AddIngredient(ItemID.ShadowScale, 12)
                .AddIngredient(ItemID.FossilOre)
                .AddTile<MagicCraftStation>()
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.Amber)
                .AddIngredient(ItemID.TissueSample, 12)
                .AddIngredient(ItemID.FossilOre)
                .AddTile<MagicCraftStation>()
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.Amber)
                .AddIngredient<IcicleScale>(7)
                .AddIngredient(ItemID.FossilOre)
                .AddTile<MagicCraftStation>()
                .Register();
        }
    }

    public class AmberEardropProj : BaseGemWeaponProj<AmberEardrop>
    {
        public override string Texture => AssetDirectory.Donator + Name;

        private int frameX;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 4);
            Main.projFrames[Type] = 20;
        }

        public override void BeforeMove()
        {
            if ((int)Main.timeForVisualEffects % 20 == 0 && Main.rand.NextBool(2))
            {
                float length = Main.rand.NextFloat(16, 32);
                Color c = Main.rand.NextFromList(AmberProj.darkC, AmberProj.brightC, AmberProj.highlightC);
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
            Vector2 idlePos = Owner.Center + new Vector2(OwnerDirection * 48, 0);
            for (int i = 0; i < 8; i++)//检测头顶2个方块并尝试找到没有物块阻挡的那个
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

            if (AttackTime > 0)
            {
                Vector2 dir = Main.MouseWorld - Projectile.Center;
                if (dir.Length() < 48)
                    idlePos += dir;
                else
                    idlePos += dir.SafeNormalize(Vector2.Zero) * 48;
            }

            TargetPos = Vector2.SmoothStep(TargetPos, idlePos, 0.3f);
            Projectile.Center = Vector2.Lerp(Projectile.Center, TargetPos, 0.5f);
            Projectile.rotation = Projectile.rotation.AngleLerp(Owner.velocity.X / 20, 0.2f);
            Lighting.AddLight(Projectile.Center, AmberProj.brightC.ToVector3());
        }

        public override void Attack()
        {
            if (AttackTime > 0)
            {
                if (++Projectile.frameCounter > 5)
                {
                    Projectile.frameCounter = 0;
                    if (++Projectile.frame > 5)
                        Projectile.frame = 1;
                }

                if (AttackTime % 7 == 0)
                {
                    Color c = Main.rand.NextFromList(AmberProj.brightC, AmberProj.highlightC, AmberProj.darkC);
                    Vector2 dir = Helper.NextVec2Dir();
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + new Vector2(0, 12), ModContent.DustType<NightmareStar>(),
                        dir * Main.rand.NextFloat(1f, 4f), newColor: c, Scale: Main.rand.NextFloat(1f, 1.75f));
                    dust.rotation = dir.ToRotation() + MathHelper.PiOver2;
                }

                if ((int)AttackTime % (Owner.itemTimeMax / 4) == 0 && Owner.CheckMana(Owner.HeldItem.mana, true))
                {
                    Owner.manaRegenDelay = 40;

                    Vector2 pos = Main.MouseWorld + new Vector2(Main.rand.Next(-300, 300), -800);
                    Vector2 vel = (Main.MouseWorld - pos).SafeNormalize(Vector2.Zero)
                        .RotateByRandom(-0.05f, 0.05f) * Main.rand.NextFloat(8, 11);

                    bool special = Main.rand.NextBool(5);
                    int dam = Owner.GetWeaponDamage(Owner.HeldItem);

                    if (special)
                        dam = (int)(1.35f * dam);
                    int type = special ? ModContent.ProjectileType<BigAmber>() : ModContent.ProjectileType<AmberProj>();
                    Projectile.NewProjectileFromThis(pos, vel, type, dam, Projectile.knockBack, Main.MouseWorld.Y);

                    for (int i = 0; i < 4; i++)
                    {
                        Dust d = Dust.NewDustPerfect(Projectile.Center + new Vector2(0, 12), ModContent.DustType<AmberDust>(), Helper.NextVec2Dir(2, 4), Scale: Main.rand.NextFloat(1f, 1.5f));
                        d.noGravity = true;
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 dir = Helper.NextVec2Dir();
                        AmberProj.SpawnTriangleParticle(Projectile.Center + new Vector2(0, 12) + (dir * Main.rand.NextFloat(6, 12)), dir * Main.rand.NextFloat(1f, 3f));
                    }
                }

                AttackTime--;
            }
            else
            {
                if (frameX == 0 && Projectile.frame > 5)
                {
                    frameX = 1;
                    Projectile.frame = 0;
                }
                Projectile.UpdateFrameNormally(3, 19);
            }
        }

        public override void StartAttack()
        {
            Projectile.frame = 0;
            frameX = 0;
            AttackTime = Owner.itemTimeMax;
            Helper.PlayPitched("Crystal/CrystalShoot", 0.4f, 0, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var frame = mainTex.Frame(2, Main.projFrames[Projectile.type], frameX, Projectile.frame);
            var origin = new Vector2(frame.Width / 2, 0);
            Vector2 toCenter = new(Projectile.width / 2, 0);

            for (int i = 0; i < 4; i++)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, frame,
                   AmberProj.brightC * (0.3f - (i * 0.3f / 4)), Projectile.oldRot[i] + 0, origin, Projectile.scale, 0, 0);

            Main.spriteBatch.Draw(mainTex, Projectile.Top - Main.screenPosition, frame, lightColor, Projectile.rotation,
                origin, Projectile.scale, 0, 0);
            return false;
        }
    }

    public class AmberProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Donator + Name;

        public static Color highlightC = new(255, 251, 201);
        public static Color brightC = new(252, 193, 45);
        public static Color darkC = new(142, 45, 0);

        public ref float RecordY => ref Projectile.ai[0];
        public ref float TileCollide => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;

            Projectile.tileCollide = false;
            Projectile.width = Projectile.height = 16;
            Projectile.extraUpdates = 2;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 300;
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

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (TileCollide == 0)
            {
                Tile t = Framing.GetTileSafely(Projectile.Center);
                if (t.HasSolidTile())
                {

                    TileCollide = 1;
                    Projectile.damage = (int)(Projectile.damage * 0.75f);
                }
            }

            if (!Projectile.tileCollide && Projectile.Center.Y > RecordY)
            {
                Projectile.tileCollide = true;
            }

            if (Projectile.timeLeft % 3 == 0)
                SpawnTriangleParticle(Projectile.Center + Main.rand.NextVector2Circular(12, 12), Projectile.velocity * Main.rand.NextFloat(0.2f, 0.4f));
            if (Main.rand.NextBool(5))
                Projectile.SpawnTrailDust(8f, ModContent.DustType<AmberDust>(), Main.rand.NextFloat(0.2f, 0.4f));
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(CoraliteSoundID.Hit_Item10, Projectile.Center);

            if (VisualEffectSystem.HitEffect_Dusts)
                for (int i = 0; i < 6; i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<AmberDust>(), Helper.NextVec2Dir(2, 4), Scale: Main.rand.NextFloat(1f, 1.5f));
                    d.noGravity = true;
                }

            if (VisualEffectSystem.HitEffect_SpecialParticles)
                for (int i = 0; i < 3; i++)
                {
                    Vector2 dir = Helper.NextVec2Dir();
                    SpawnTriangleParticle(Projectile.Center + (dir * Main.rand.NextFloat(6, 12)), dir * Main.rand.NextFloat(1f, 3f));
                }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.QuickDraw(lightColor, -1.57f - 0.785f);
            return false;
        }
    }

    public class BigAmber : AmberProj, IDrawPrimitive, IDrawNonPremultiplied
    {
        public Vector2 rand = Main.rand.NextVector2CircularEdge(64, 64);

        private Trail trail;

        public int frameC;
        public int frame;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 22;
        }

        public override void AI()
        {
            if (trail == null)
            {
                const int maxPoint = 16;
                trail ??= new Trail(Main.graphics.GraphicsDevice, maxPoint, new NoTip()
                    , factor => Helper.Lerp(2, 18, factor),
                      factor =>
                      {
                          return Color.Lerp(new Color(0, 0, 0, 0), Color.White * 0.65f, factor.X);
                      });

                Projectile.InitOldPosCache(maxPoint);
            }

            if (++frameC > 4)
            {
                frameC = 0;
                if (++frame > 11)
                    frame = 0;
            }

            base.AI();
            Projectile.UpdateFrameNormally(8, 19);
            Projectile.UpdateOldPosCache();
            trail.Positions = Projectile.oldPos;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<AmberDebuff>(), 60);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(CoraliteSoundID.CrushedIce_Item27, Projectile.Center);
            if (VisualEffectSystem.HitEffect_Dusts)
                for (int i = 0; i < 8; i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<AmberDust>(), Helper.NextVec2Dir(2, 6), Scale: Main.rand.NextFloat(1f, 1.5f));
                    d.noGravity = true;
                }

            if (VisualEffectSystem.HitEffect_SpecialParticles)
                for (int i = 0; i < 6; i++)
                {
                    Vector2 dir = Helper.NextVec2Dir();
                    SpawnTriangleParticle(Projectile.Center + (dir * Main.rand.NextFloat(6, 12)), dir * Main.rand.NextFloat(1f, 5f));
                }
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawPrimitives()
        {
            if (trail == null)
                return;

            rand -= Projectile.velocity / 10;
            Effect effect = Filters.Scene["CrystalTrail"].GetShader().Shader;

            Texture2D noiseTex = GemTextures.CellNoise.Value;//[(int)(Main.timeForVisualEffects / 7) % 20].Value;

            effect.Parameters["noiseTexture"].SetValue(noiseTex);
            effect.Parameters["TrailTexture"].SetValue(ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "ExtraLaser").Value);
            effect.Parameters["transformMatrix"].SetValue(Helper.GetTransfromMaxrix());
            effect.Parameters["basePos"].SetValue((Projectile.Center + rand - Main.screenPosition) * Main.GameZoomTarget);
            effect.Parameters["scale"].SetValue(new Vector2(5f / Main.GameZoomTarget));
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
            effect.Parameters["lightRange"].SetValue(0.1f);
            effect.Parameters["lightLimit"].SetValue(0.75f);
            effect.Parameters["addC"].SetValue(0.75f);
            effect.Parameters["highlightC"].SetValue(highlightC.ToVector4());
            effect.Parameters["brightC"].SetValue(brightC.ToVector4());
            effect.Parameters["darkC"].SetValue(darkC.ToVector4());

            trail.Render(effect);
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            Projectile.QuickDraw(Projectile.GetTexture().Frame(1, 12, 0, frame), Color.White, -MathHelper.PiOver2 - MathHelper.PiOver4);
        }
    }

    public class AmberDust : ModDust
    {
        public override string Texture => AssetDirectory.Donator + Name;

        public override void OnSpawn(Dust dust)
        {
            UpdateType = DustID.OrangeTorch;
            dust.frame = new Rectangle(0, 18 * Main.rand.Next(4), 18, 18);
            dust.rotation = Main.rand.NextFloat(6.282f);
            dust.scale *= 0.8f;
        }

        public override bool PreDraw(Dust dust)
        {
            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, Lighting.GetColor(dust.position.ToTileCoordinates())
                , dust.rotation, dust.frame.Size() / 2, dust.scale, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class AmberDebuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + "Buff";

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (Main.rand.NextBool(6))
                AmberProj.SpawnTriangleParticle(Main.rand.NextVector2FromRectangle(npc.getRect()), -Vector2.UnitY * Main.rand.NextFloat(0.2f, 2f));
            if (Main.rand.NextBool(3))
            {
                int d = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<AmberDust>(), SpeedY: -2);
                Main.dust[d].noGravity = true;
            }

            if (npc.TryGetGlobalNPC(out CoraliteGlobalNPC cgnpc))
            {
                cgnpc.StopHitPlayer = true;
                cgnpc.SlowDownPercent = 0.5f;
            }
        }
    }
}
