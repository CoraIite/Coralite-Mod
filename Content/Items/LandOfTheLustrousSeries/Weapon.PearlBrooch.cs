using Coralite.Content.Tiles.RedJades;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Loaders;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.UI.Chat;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class PearlBrooch : BaseGemWeapon
    {
        public override void SetDefs()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Pink5, Item.sellPrice(0, 8));
            Item.SetWeaponValues(45, 4);
            Item.useTime = Item.useAnimation = 23;
            Item.mana = 9;

            Item.shoot = ModContent.ProjectileType<PearlBroochProj>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.ownedProjectileCounts[type] < 1)
                Projectile.NewProjectile(source, position, Vector2.Zero, type, 0, knockback, player.whoAmI);

            foreach (var p in Main.ActiveProjectiles)
                if (p.owner == player.whoAmI && p.type == type)
                {
                    (p.ModProjectile as PearlBroochProj).StartAttack();
                    break;
                }

            return false;
        }

        public override void DrawGemName(DrawableTooltipLine line)
        {
            SpriteBatch sb = Main.spriteBatch;
            Effect effect = ShaderLoader.GetShader("Crystal");

            rand.X += 0.6f;
            rand.Y += 0.1f;
            if (rand.X > 100000)
                rand.X = 10;

            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            Texture2D noiseTex = GemTextures.CellNoise2.Value;//[(int)(Main.timeForVisualEffects / 7) % 20].Value;

            Color c1 = PearlProj.brightC * 0.75f;
            c1.A = 255;
            Color c2 = PearlProj.darkC * 0.75f;
            c1.A = 255;

            effect.Parameters["transformMatrix"].SetValue(projection);
            effect.Parameters["basePos"].SetValue(rand + new Vector2(line.X, line.Y));
            effect.Parameters["scale"].SetValue(new Vector2(3f) / Main.GameZoomTarget);
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
            effect.Parameters["lightRange"].SetValue(0.2f);
            effect.Parameters["lightLimit"].SetValue(0.25f);
            effect.Parameters["addC"].SetValue(0.15f);
            effect.Parameters["highlightC"].SetValue(PearlProj.highlightC.ToVector4());
            effect.Parameters["brightC"].SetValue(c1.ToVector4());
            effect.Parameters["darkC"].SetValue(c2.ToVector4());

            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, effect, Main.UIScaleMatrix);

            Main.graphics.GraphicsDevice.Textures[1] = GemTextures.CellNoise2.Value;

            Vector2 textSize = ChatManager.GetStringSize(line.Font, line.Text, line.BaseScale);
            Texture2D mainTex = CoraliteAssets.LightBall.BallA.Value;

            int xExpand = 45;
            int yExpand = 6;

            sb.Draw(mainTex, new Rectangle(line.X - xExpand, line.Y - 4 - yExpand, (int)textSize.X + xExpand * 2, (int)textSize.Y + yExpand * 2), null, Color.White * 0.8f);


            effect.Parameters["transformMatrix"].SetValue(projection);
            effect.Parameters["basePos"].SetValue(rand + new Vector2(line.X, line.Y));
            effect.Parameters["scale"].SetValue(new Vector2(1f) / Main.GameZoomTarget);
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
            effect.Parameters["lightRange"].SetValue(0.2f);
            effect.Parameters["lightLimit"].SetValue(0.25f);
            effect.Parameters["addC"].SetValue(0.15f);
            effect.Parameters["highlightC"].SetValue(PearlProj.highlightC.ToVector4());
            effect.Parameters["brightC"].SetValue(PearlProj.brightC.ToVector4());
            effect.Parameters["darkC"].SetValue(PearlProj.darkC.ToVector4());

            //sb.End();
            //sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, effect, Main.UIScaleMatrix);

            Main.graphics.GraphicsDevice.Textures[1] = noiseTex;
            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, line.Font, line.Text, new Vector2(line.X, line.Y)
                , Color.White, line.Rotation, line.Origin, line.BaseScale, line.MaxWidth, line.Spread);

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);
        }

        public override void SpawnParticle(DrawableTooltipLine line)
        {
            SpawnParticleOnTooltipNormaly(line, PearlProj.brightC, PearlProj.darkC);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.WhitePearl)
                .AddIngredient(ItemID.GelBalloon, 12)
                .AddIngredient(ItemID.HolyWater)
                .AddTile<MagicCraftStation>()
                .Register();
        }
    }

    public class PearlBroochProj : BaseGemWeaponProj<PearlBrooch>
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + "PearlBrooch";

        public ref float ShootCount => ref Projectile.ai[1];

        public override bool CanFire => AttackTime > 0;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 5);
        }

        public override void BeforeMove()
        {
            if ((int)Main.timeForVisualEffects % 30 == 0 && Main.rand.NextBool(2))
            {
                float length = Main.rand.NextFloat(24, 32);
                Color c = Main.rand.NextFromList(Color.White, PearlProj.brightC, PearlProj.darkC);
                var cs = CrystalShine.Spawn(Projectile.Center + Main.rand.NextVector2CircularEdge(length, length)
                     , Helper.NextVec2Dir(0.1f, 0.2f), 5, new Vector2(0.5f, 0.03f) * Main.rand.NextFloat(0.5f, 1f), c);
                cs.follow = () => Projectile.position - Projectile.oldPos[1];
                cs.TrailCount = 3;
                cs.fadeTime = Main.rand.Next(40, 70);
                cs.shineRange = 12;
            }

            Lighting.AddLight(Projectile.Center, Color.Pink.ToVector3() / 2);
        }

        public override void Move()
        {
            Vector2 idlePos = Owner.Center + new Vector2(Owner.direction * 32, 0);

            if (DownLeft)
            {
                idlePos = Owner.Center;
                Vector2 dir = InMousePos - Owner.Center;
                Vector2 dirN = dir.SafeNormalize(Vector2.Zero);

                const int maxLength = 16 * 15;
                int length =(int) dir.Length();

                if (length > maxLength)
                    length = maxLength;

                for (int i = 0; i < length / 8; i++)
                {
                    if (Helper.PointInTile(idlePos))
                        break;

                    idlePos += dirN * 8;
                }

                Projectile.netUpdate = true;
            }
            else
            {
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

                idlePos += (Main.GlobalTimeWrappedHourly * 2).ToRotationVector2() * 18;
            }

            TargetPos = Vector2.Lerp(TargetPos, idlePos, 0.3f);
            Projectile.Center = Vector2.Lerp(Projectile.Center, TargetPos, 0.4f);
            Projectile.rotation = Projectile.rotation.AngleLerp(Owner.velocity.X / 20, 0.2f);
        }

        public override void Attack()
        {
            if (AttackTime > 0)
            {
                Projectile.rotation = MathF.Sin((1 - (AttackTime / Owner.itemTimeMax)) * MathHelper.TwoPi) * 0.3f;
                if ((int)AttackTime == 1 && !VaultUtils.isServer)
                {
                    ShootPearl();
                    ShootCount++;

                    Helper.PlayPitched("Crystal/CrystalShoot", 0.05f, 0.2f, Projectile.Center);

                    for (int i = 0; i < 8; i++)
                    {
                        Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.PinkTorch, Helper.NextVec2Dir(2, 4), Scale: Main.rand.NextFloat(1f, 1.5f));
                        d.noGravity = true;
                    }

                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 dir = Helper.NextVec2Dir();
                        PearlProj.SpawnTriangleParticle(Projectile.Center + (dir * Main.rand.NextFloat(6, 12)), dir * Main.rand.NextFloat(1f, 3f));
                    }
                }

                AttackTime--;
            }
        }

        private void ShootPearl()
        {
            int type = (int)ShootCount % 3;

            float angle = ShootCount * MathHelper.TwoPi / 12;

            Projectile.NewProjectileFromThis<PearlProj>(Projectile.Center
                , angle.ToRotationVector2(), Owner.GetWeaponDamage(Item), Projectile.knockBack, type, Projectile.whoAmI,ShootCount%3>1?2:1);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTextureValue();
            Vector2 toCenter = new(Projectile.width / 2, Projectile.height / 2);

            for (int i = 0; i < 5; i++)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, null,
                    Main.hslToRgb(Main.GlobalTimeWrappedHourly + (i * 0.1f), 0.9f, 0.9f) * (0.5f - (i * 0.5f / 5)), Projectile.oldRot[i], mainTex.Size() / 2, Projectile.scale, 0, 0);

            Projectile.QuickDraw(lightColor, 0);
            return false;
        }
    }

    /// <summary>
    /// ai0传入贴图类型，ai1传入持有者，ai2传入状态
    /// </summary>
    public class PearlProj : ModProjectile, IDrawNonPremultiplied, IDrawPrimitive
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + Name;

        public static Color highlightC = Color.White;
        public static Color brightC = new(226, 174, 214);
        public static Color darkC = new(109, 214, 214);

        public ref float State => ref Projectile.ai[2];
        public ref float Timer => ref Projectile.localAI[1];
        public ref float Tex => ref Projectile.ai[0];
        public ref float Owner => ref Projectile.ai[1];

        private VertexStrip _vertexStrip = new();

        private const int TrailCount = 12;
        private const int MaxTime = 180;

        public Vector2[] newOldPos;

        public override void SetStaticDefaults()
        {
            //Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 15);
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.timeLeft = MaxTime;
            Projectile.penetrate = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 25;
            Projectile.tileCollide = false;
        }

        //public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        //{
        //    width = 16;
        //    height = 16;
        //    return true;
        //}

        public override void AI()
        {
            if (!Owner.GetProjectileOwner<PearlBroochProj>(out Projectile owner,Projectile.Kill))
                return;

            if (!VaultUtils.isServer && Projectile.localAI[2] == 0)
            {
                _vertexStrip = new VertexStrip();

                if (Projectile.oldPos.Length != TrailCount)
                    Array.Resize(ref Projectile.oldPos, TrailCount);
                if (Projectile.oldRot.Length != TrailCount)
                    Array.Resize(ref Projectile.oldRot, TrailCount);
                if (Projectile.oldSpriteDirection.Length != TrailCount)
                    Array.Resize(ref Projectile.oldSpriteDirection, TrailCount);

                newOldPos = new Vector2[TrailCount];

                for (int i = 0; i < TrailCount; i++)
                {
                    Projectile.oldPos[i] = Projectile.Center - owner.Center;
                    newOldPos[i] = owner.Center;
                }

                Projectile.InitOldRotCache(TrailCount);

                Projectile.localAI[2] = 1;
            }

            switch (State)
            {
                default:
                case 0://内圈弹幕AI
                    {
                        Vector2 dir = Projectile.velocity.RotatedBy(Timer * MathHelper.TwoPi / (MaxTime / 4));
                        float length = MathF.Sin(Timer / MaxTime * MathHelper.Pi);

                        Projectile.Center = owner.Center + dir * length * 16 * 5;
                    }
                    break;
                case 1://超级转圈圈弹幕AI
                    {
                        float r = Timer * MathHelper.TwoPi / MaxTime;
                        Vector2 dir = Projectile.velocity.RotatedBy(r);
                        float length = MathF.Sin(3 * r);

                        Projectile.Center = owner.Center + dir * length * 16 * 8;
                    }
                    break;
                case 2://外圈弹幕AI，对数螺线
                    {
                        float r = Timer * MathHelper.TwoPi / MaxTime * 3;
                        Vector2 dir = Projectile.velocity.RotatedBy(r);
                        float length = MathF.Pow(2,r/MathHelper.TwoPi);

                        Projectile.Center = owner.Center + dir * length * 16*1.5f;
                    }
                    break;
                case 3://神秘圈圈
                    {
                        float r = MathHelper.Pi * 3 + Timer * MathHelper.TwoPi / MaxTime * 5;
                        Vector2 dir = Projectile.velocity.RotatedBy(r);
                        float length = MathF.Cos(r / 5);

                        Projectile.Center = owner.Center + dir * length * 16 * 7;
                    }
                    break;
            }

            Timer++;

            for (int i = 0; i < Projectile.oldPos.Length - 1; i++)
                Projectile.oldPos[i] = Projectile.oldPos[i + 1];
            Projectile.oldPos[^1] = Projectile.Center - owner.Center;
            for (int i = 0; i < Projectile.oldPos.Length ; i++)
                newOldPos[i] = Projectile.oldPos[TrailCount-i-1]+owner.Center;

            Projectile.rotation = (Projectile.Center - newOldPos[1]).ToRotation();

            Lighting.AddLight(Projectile.Center, new Vector3(0.6f));
            Projectile.UpdateOldRotCache();

            if (Projectile.timeLeft % 8 == 0)
                SpawnTriangleParticle(Projectile.Center + Main.rand.NextVector2Circular(6, 6), Projectile.velocity * Main.rand.NextFloat(0.2f, 0.4f));

            if (Main.rand.NextBool(12))
                Projectile.SpawnTrailDust(8f, DustID.PinkTorch, Main.rand.NextFloat(0.2f, 0.4f));
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
            CrystalTriangle.Spawn(pos, velocity, c, 7, Main.rand.NextFloat(0.05f, 0.2f));
        }

        //public override bool OnTileCollide(Vector2 oldVelocity)
        //{
        //    if (State == 0)
        //    {
        //        if (Projectile.velocity.X != oldVelocity.X)
        //            Projectile.velocity.X = oldVelocity.X * -0.8f;

        //        if (Projectile.velocity.Y != oldVelocity.Y)
        //            Projectile.velocity.Y = oldVelocity.Y * -0.8f;

        //        Projectile.netUpdate = true;

        //        return false;
        //    }
        //    return base.OnTileCollide(oldVelocity);
        //}

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Helper.PlayPitched(CoraliteSoundID.NoUse_WaterDrop_Item86, Projectile.Center,volume:0.2f);
                Projectile.NewProjectileFromThis<PearlExplosion>(Projectile.Center, Vector2.Zero, Projectile.damage, Projectile.knockBack / 2);
            }

            if (VisualEffectSystem.HitEffect_Dusts)
                for (int i = 0; i < 4; i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.PinkTorch, Helper.NextVec2Dir(4, 6), Scale: Main.rand.NextFloat(1f, 1.5f));
                    d.noGravity = true;
                }

            if (VisualEffectSystem.HitEffect_SpecialParticles)
            {
                Vector2 dir = -Projectile.velocity.SafeNormalize(Vector2.Zero);
                Vector2 toCenter = new(Projectile.width / 2, Projectile.height / 2);
                for (int i = 0; i < 4; i++)
                {
                    Vector2 dir2 = dir.RotateByRandom(-0.6f, 0.6f);
                    SpawnTriangleParticle(Projectile.Center + (dir2 * Main.rand.NextFloat(6, 12)), dir2 * Main.rand.NextFloat(3f, 6f));
                }

                for (int i = 0; i < Projectile.oldPos.Length - 5; i++)
                {
                    Vector2 dir2 = (Projectile.oldPos[i + 1] - Projectile.oldPos[i]).SafeNormalize(Vector2.Zero).RotateByRandom(-0.2f, 0.2f);
                    SpawnTriangleParticle(Projectile.oldPos[i] + toCenter + (dir2 * Main.rand.NextFloat(6)), dir2 * Main.rand.NextFloat(1f, 6f));
                }
            }
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = Projectile.GetTextureValue();
            var frameBox = mainTex.Frame(1, 3, 0, (int)Tex);
            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, frameBox, Color.White, 0, frameBox.Size() / 2, Projectile.scale, 0, 0);
        }

        private Color StripColors(float progressOnStrip)
        {
            Color result = Color.Lerp(Color.White, Color.Lerp(brightC, darkC, (MathF.Sin(Main.GlobalTimeWrappedHourly) / 2) + 0.5f), Utils.GetLerpValue(-0.2f, 0.5f, progressOnStrip, clamped: true)) * (1f - Utils.GetLerpValue(0f, 0.98f, progressOnStrip));
            result.A = 64;
            return result;
        }

        private float StripWidth(float progressOnStrip) => MathHelper.Lerp(10f, 24f, Utils.GetLerpValue(0f, 0.2f, progressOnStrip, clamped: true)) * Utils.GetLerpValue(0f, 0.07f, progressOnStrip, clamped: true);

        public override bool PreDraw(ref Color lightColor)
        {
            //if (!Owner.GetProjectileOwner<PearlBroochProj>(out Projectile owner))
            //    return false;

            //Texture2D mainTex = Projectile.GetTextureValue();
            //var frameBox = mainTex.Frame(1, 3, 0, (int)Tex);

            //Color c = Color.White;
            //c.A = 0;

            //for (int i = 1; i < TrailCount; i ++)
            //    Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + owner.Center - Main.screenPosition, frameBox, c * (0.7f - (i * 0.7f/ TrailCount)), 0, frameBox.Size() / 2, 1f*i/TrailCount, 0, 0);

            //Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, frameBox, Color.White, 0,frameBox.Size() / 2, Projectile.scale, 0, 0);

            return false;
        }

        public void DrawPrimitives()
        {
            if (newOldPos == null)
                return;

            MiscShaderData miscShaderData = GameShaders.Misc["RainbowRod"];
            miscShaderData.UseSaturation(-1.8f);
            miscShaderData.UseOpacity(2f);
            miscShaderData.Apply();
            _vertexStrip.PrepareStripWithProceduralPadding(newOldPos, Projectile.oldRot, StripColors, StripWidth, -Main.screenPosition);
            _vertexStrip.DrawTrail();
        }
    }

    public class PearlExplosion : ModProjectile
    {
        public override string Texture => AssetDirectory.Halos + "HighlightCircleA";

        public Vector2 rand = Main.rand.NextVector2CircularEdge(64, 64);

        public float scale1;
        public float scale2;

        public ref float Timer => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.timeLeft = 1200;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.alpha = 0;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            const int halfTime = 50;
            const int totalTime = halfTime + 10;

            if (Timer < halfTime)
            {
                float factor = Timer / halfTime;
                float sqrtF = Helper.SqrtEase(factor);

                scale1 = Helper.Lerp(0, 0.3f, sqrtF);
                scale2 = Helper.Lerp(0, 0.2f, Helper.X2Ease(factor));
                Projectile.alpha = (int)Helper.Lerp(0, 200, sqrtF);
            }
            else if (Timer < totalTime)
            {
                float factor = (Timer - halfTime) / (totalTime - halfTime);
                float x2F = Helper.X2Ease(factor);

                scale1 = Helper.Lerp(0.3f, 0, x2F);
                scale2 = Helper.Lerp(0.2f, 0, Helper.SqrtEase(factor));
                Projectile.alpha = (int)Helper.Lerp(200, 0, x2F);
            }
            else
            {
                Projectile.Kill();
            }

            Lighting.AddLight(Projectile.Center, new Vector3(0.6f));
            Timer++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.88f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            rand.X += 1;
            Effect effect = ShaderLoader.GetShader("Crystal");

            Texture2D noiseTex = GemTextures.CellNoise2.Value;//[(int)(Main.timeForVisualEffects / 7) % 20].Value;

            effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
            effect.Parameters["basePos"].SetValue((Projectile.Center + rand - Main.screenPosition) * Main.GameZoomTarget);
            effect.Parameters["scale"].SetValue(Vector2.One / Main.GameZoomTarget);
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
            effect.Parameters["lightRange"].SetValue(0.8f);
            effect.Parameters["lightLimit"].SetValue(0.6f);
            effect.Parameters["addC"].SetValue(0.37f);
            effect.Parameters["highlightC"].SetValue(PearlProj.highlightC.ToVector4());
            effect.Parameters["brightC"].SetValue(PearlProj.brightC.ToVector4());
            effect.Parameters["darkC"].SetValue(PearlProj.darkC.ToVector4());

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, effect, Main.GameViewMatrix.ZoomMatrix);

            Main.graphics.GraphicsDevice.Textures[1] = noiseTex;
            Texture2D mainTex = Projectile.GetTextureValue();
            Color c = lightColor;
            c *= Projectile.alpha / 255f;
            Main.spriteBatch.Draw(mainTex, Projectile.Center, null, c, 0, mainTex.Size() / 2, scale1, 0, 0);
            Main.spriteBatch.Draw(mainTex, Projectile.Center, null, c, 0, mainTex.Size() / 2, scale2, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}
