using Coralite.Content.Raritys;
using Coralite.Content.WorldGeneration;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.SmoothFunctions;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries1
{
    public class LeprechaunBait : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries1Item + Name;

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;

            Item.noUseGraphic = true;
            Item.channel = true;

            Item.shoot = ModContent.ProjectileType<LeprechaunBaitHeldProj>();
            Item.shootSpeed = 3;
            Item.useTime = Item.useAnimation = 20;
            Item.rare = ModContent.RarityType<MagicCrystalRarity>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood, 10)
                .AddIngredient<MagicCrystal>()
                .AddIngredient(ItemID.Rope)
                .Register();
        }
    }

    public class LeprechaunBaitHeldProj : BaseHeldProj
    {
        public override string Texture => AssetDirectory.MagikeSeries1Item + Name;

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];

        public bool hasFairy;
        public Vector2 fairyPos;
        public Vector2 targetPos;
        public SecondOrderDynamics_Vec2 posSmoother;

        public static Asset<Texture2D> baitTex;
        public static Asset<Texture2D> fairyTex;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            baitTex = ModContent.Request<Texture2D>(AssetDirectory.MagikeSeries1Item + "LeprechaunBaitTip");
            fairyTex = ModContent.Request<Texture2D>(AssetDirectory.MagikeNPCs + "CrystalFairy");
        }

        public override void Unload()
        {
            baitTex = null;
            fairyTex = null;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 10000;
            Projectile.width = Projectile.height = 16;
        }

        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            SetHeld();
            Owner.itemTime = Owner.itemAnimation = 2;

            Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

            if (Item.type != ModContent.ItemType<LeprechaunBait>())
            {
                Projectile.Kill();
                return;
            }

            switch (State)
            {
                default:
                case 0://刚拿出来
                    {
                        Projectile.velocity.X *= 0.99f;
                        if (Projectile.velocity.Y < 5)
                            Projectile.velocity.Y += 0.3f;
                        else
                            Projectile.velocity.Y = 5;

                        Timer++;

                        if (Timer > 80)
                        {
                            if (CoraliteWorld.MagicCrystalCaveCenters == null || CoraliteWorld.MagicCrystalCaveCenters.Count < 1)
                            {
                                Projectile.Kill();
                                return;
                            }

                            State = 1;
                            Timer = 0;
                            hasFairy = true;
                            fairyPos = Projectile.Center;

                            posSmoother = new SecondOrderDynamics_Vec2(1, 0.5f, 0, fairyPos);

                            targetPos = CoraliteWorld.MagicCrystalCaveCenters
                                .MinBy(p => p.ToWorldCoordinates().Distance(Owner.MountedCenter)).ToWorldCoordinates();

                            SoundEngine.PlaySound(CoraliteSoundID.FairyDeath_NPCDeath7, Projectile.Center);

                            for (int i = 0; i < 10; i++)
                            {
                                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.CrystalSerpent_Pink
                                    , (i * MathHelper.TwoPi / 10).ToRotationVector2() * Main.rand.NextFloat(2, 3));
                                d.noGravity = true;
                            }
                        }
                    }
                    break;
                case 1://被仙灵追
                    {
                        if (Owner.controlUseItem)
                        {
                            Projectile.Kill();
                        }

                        if (Timer < 80)
                        {
                            fairyPos = posSmoother.Update(1 / 60f, Projectile.Center + (Timer * 0.2f).ToRotationVector2() * 32);
                        }
                        else
                        {
                            Vector2 tPos = Projectile.Center + (targetPos - Owner.MountedCenter).SafeNormalize(Vector2.Zero) * Helpers.Helper.Lerp(32, 200, (Timer - 80) / 80);
                            fairyPos = posSmoother.Update(1 / 60f, tPos);
                        }

                        if (Timer > 160)
                        {
                            targetPos = CoraliteWorld.MagicCrystalCaveCenters
                                .MinBy(p => p.ToWorldCoordinates().Distance(Owner.MountedCenter)).ToWorldCoordinates();

                            Timer = 0;
                        }

                        Projectile.velocity = (targetPos - Owner.MountedCenter).SafeNormalize(Vector2.Zero) * 3;

                        Dust d = Dust.NewDustPerfect(fairyPos + Main.rand.NextVector2Circular(4, 4),
                            DustID.CrystalSerpent_Pink, Vector2.Zero);
                        d.noGravity = true;

                        Timer++;
                    }
                    break;
            }

            Projectile.position += Projectile.velocity;
            LimitPosition();
        }

        public void LimitPosition()
        {
            Vector2 handlePos = GetHandleTopPos();
            Vector2 pos = Projectile.Center;
            if (pos.Distance(handlePos) > 50)
            {
                Vector2 dir = (pos - handlePos).SafeNormalize(Vector2.Zero);
                Projectile.Center = handlePos + dir * 50;
            }
        }

        public Vector2 GetHandleTopPos()
        {
            return Owner.MountedCenter + new Vector2(DirSign * 26, -26);
        }

        public virtual void DrawLine(Vector2 handlePos, Vector2 stringTipPos)
        {
            bool flag = true;
            handlePos.Y += Owner.gfxOffY;

            float distanceX = stringTipPos.X - handlePos.X;
            float distanceY = stringTipPos.Y - handlePos.Y;
            bool flag2 = true;
            float rot = (float)Math.Atan2(distanceY, distanceX) - 1.57f;

            Texture2D stringTex = TextureAssets.FishingLine.Value;

            float halfWidth = stringTex.Width / 2;
            float halfHeight = stringTex.Height / 2;
            Vector2 origin = new(halfWidth, 0f);

            if (distanceX == 0f && distanceY == 0f)
            {
                flag = false;
            }
            else
            {
                float distance = (float)Math.Sqrt((distanceX * distanceX) + (distanceY * distanceY));
                distance = stringTex.Height / distance;
                distanceX *= distance;
                distanceY *= distance;
                handlePos.X -= distanceX * 0.1f;
                handlePos.Y -= distanceY * 0.1f;
                distanceX = stringTipPos.X - handlePos.X;
                distanceY = stringTipPos.Y - handlePos.Y;
            }

            while (flag)
            {
                float sourceHeight = stringTex.Height;
                float distance1 = (float)Math.Sqrt((distanceX * distanceX) + (distanceY * distanceY));
                float distance2 = distance1;
                if (float.IsNaN(distance1) || float.IsNaN(distance2))
                {
                    flag = false;
                    continue;
                }

                if (distance1 < stringTex.Height + 8)
                {
                    sourceHeight = distance1 - 8f;
                    flag = false;
                }

                distance1 = stringTex.Height / distance1;
                distanceX *= distance1;
                distanceY *= distance1;
                if (flag2)
                {
                    flag2 = false;
                }
                else
                {
                    handlePos.X += distanceX;
                    handlePos.Y += distanceY;
                }

                distanceX = stringTipPos.X - handlePos.X;
                distanceY = stringTipPos.Y - handlePos.Y;
                if (distance2 > stringTex.Height)
                {
                    float num9 = 0.3f;
                    float num10 = Math.Abs(Owner.velocity.X) + Math.Abs(Owner.velocity.Y);
                    if (num10 > 16f)
                        num10 = 16f;

                    num10 = 1f - (num10 / 16f);
                    num9 *= num10;
                    num10 = distance2 / 80f;
                    if (num10 > 1f)
                        num10 = 1f;

                    num9 *= num10;
                    if (num9 < 0f)
                        num9 = 0f;

                    num9 *= num10;
                    num9 *= 0.5f;
                    if (distanceY > 0f)
                    {
                        distanceY *= 1f + num9;
                        distanceX *= 1f - num9;
                    }
                    else
                    {
                        num10 = Math.Abs(Owner.velocity.X) / 3f;
                        if (num10 > 1f)
                            num10 = 1f;

                        num10 -= 0.5f;
                        num9 *= num10;
                        if (num9 > 0f)
                            num9 *= 2f;

                        distanceY *= 1f + num9;
                        distanceX *= 1f - num9;
                    }
                }

                rot = (float)Math.Atan2(distanceY, distanceX) - 1.57f;
                Color c = GetStringColor(handlePos);

                Main.EntitySpriteDraw(
                    color: c, texture: stringTex,
                    position: handlePos - Main.screenPosition + new Vector2(0, halfHeight), sourceRectangle: new Rectangle(0, 0, stringTex.Width, (int)sourceHeight), rotation: rot, origin: origin, scale: 1f, effects: SpriteEffects.None);
            }
        }

        public virtual Color GetStringColor(Vector2 pos)
        {
            Color c = Color.Gray;
            c.A = (byte)(c.A * 0.4f);
            c = Lighting.GetColor((int)pos.X / 16, (int)(pos.Y / 16f), c);
            c *= 0.5f;
            return c;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 handleTopPos = GetHandleTopPos();
            Vector2 center = Projectile.Center;

            DrawLine(handleTopPos, center);

            Texture2D mainTex = Projectile.GetTexture();
            SpriteEffects effects = DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.Draw(mainTex, Owner.MountedCenter + new Vector2(base.DirSign * 16, -8) - Main.screenPosition, null
                , Lighting.GetColor(Owner.MountedCenter.ToTileCoordinates()), 0, mainTex.Size() / 2, 1, effects, 0);

            mainTex = baitTex.Value;
            Main.spriteBatch.Draw(mainTex, center - Main.screenPosition, null
                , lightColor, 0, mainTex.Size() / 2, 1, 0, 0);

            if (hasFairy)
            {
                mainTex = fairyTex.Value;
                Main.spriteBatch.Draw(mainTex, fairyPos - Main.screenPosition, null
                    , lightColor, Main.GlobalTimeWrappedHourly, mainTex.Size() / 2, 1, 0, 0);
            }

            return false;
        }
    }
}
