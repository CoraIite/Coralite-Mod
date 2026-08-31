using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.VanillaRework
{
    public class NightsEdgeRE0 : ModItem
    {
        public override string Texture => AssetDirectory.Vanilla + "Item_273";

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.NightsEdge);
            Item.damage = 65;
            Item.useTime = Item.useAnimation = 15;
            Item.knockBack = 4f;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Melee;
            Item.shoot = ProjectileType<NightsEdgeRESlash>();

            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI, Item.type);
            return false;
        }
    }

    public class NightsEdgeRE1
    {
    }

    public class NightsEdgeRE2
    {
    }

    [VaultLoaden(AssetDirectory.VanillaRework)]
    public class NightsEdgeRESlash() : BaseSwingProj_ScaledItem(trailCount: 30),IDrawWarp
    {
        public override string Texture => AssetDirectory.Blank;

        public static ATex NightsColor0 { get; private set; }
        public static ATex NightsColorB0 { get; private set; }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 65 * Projectile.scale;
        }

        public override Texture2D GetGradient()
        {
            //if (ItemType==ModContent.)
            {

            }

            return NightsColor0.Value;
        }

        public Texture2D GetBackGradient()
        {
            //if (ItemType==ModContent.)
            {

            }

            return NightsColorB0.Value;
        }



        public override void SetSwingProperty()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.localNPCHitCooldown = 60;
            Projectile.width = 40;
            Projectile.height = 85;
            trailTopWidth = 0;
            distanceToOwner = 8;
            onHitFreeze = 6;
            useSlashTrail = true;
            
            Projectile.hide = true;

            distanceToOwner = 4;
        }

        protected override void InitializeSwing()
        {
            InitDirection();
            Projectile.extraUpdates = 6;

            switch (Combo)
            {
                default:
                    Projectile.Kill();
                    return;
                case 0:
                    {
                        startAngle = 0.8f;
                        BeforeAngle = 1.9f;
                        extraScaleAngle =DirSign* 0.3f;
                        beforeTime = 5 * Projectile.MaxUpdates;
                        beforeSmoother = Coralite.Instance.SqrtSmoother;
                        totalAngle = 5.2f;
                        minTime = 15 * Projectile.MaxUpdates;
                        maxTime = minTime + (int)(Owner.itemTimeMax*0.8f) * Projectile.MaxUpdates;
                        Smoother = Coralite.Instance.BezierEaseSmoother;
                        xScale = 1.3f;
                        yScale = 0.8f;

                        Delay = Projectile.extraUpdates * 10;
                    }
                    break;
                case 1:
                    {
                        startAngle = 2.6f;
                        BeforeAngle = 0.1f;
                        extraScaleAngle = -DirSign * 0.3f;
                        beforeTime = 5 * Projectile.MaxUpdates;
                        beforeSmoother = Coralite.Instance.SqrtSmoother;
                        totalAngle = 5.2f;
                        minTime = 15 * Projectile.MaxUpdates;
                        maxTime = minTime + (int)(Owner.itemTimeMax * 0.8f) * Projectile.MaxUpdates;
                        Smoother = Coralite.Instance.BezierEaseSmoother;
                        xScale = 1.3f;
                        yScale = 0.8f;

                        Delay = Projectile.extraUpdates * 20;
                    }
                    break;
            }

            base.InitializeSwing();
        }

        protected override void OnSlash()
        {
            alpha = 150;
            base.OnSlash();
            SetScale();
        }

        public override Effect ApplyBottomColorShader()
        {
            Effect effect = ShaderLoader.GetShader("NoHLGradientTrail");

            effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
            effect.Parameters["sampleTexture"].SetValue(CoraliteAssets.Trail.Split2.Value);
            effect.Parameters["gradientTexture"].SetValue(GetBackGradient());
            return effect;
        }

        public override Effect ApplyHighlightColor()
        {
            Effect effect = ShaderLoader.GetShader("NoHLGradientTrail");

            effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
            effect.Parameters["sampleTexture"].SetValue(CoraliteAssets.Trail.SlashFlatFade.Value);
            effect.Parameters["gradientTexture"].SetValue(GetGradient());
            return effect;
        }

        public override void ApplyHighlight(List<ColoredVertex> bars2)
        {
            //for (int i = 0; i < 2; i++)
            base.ApplyHighlight(bars2);
        }

        public void DrawWarp()
        {
            if (oldRotate != null)
                WarpDrawer(0.75f,warpStrength:0.15f);
        }


        public override Color AdditiveColor(float f)
        {
            return Color.White*0.8f * Utils.Remap(alpha, 0, 150, 0, 1);
        }


    }
}
