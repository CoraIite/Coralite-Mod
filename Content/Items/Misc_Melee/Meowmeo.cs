using Coralite.Content.WorldGeneration;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;

namespace Coralite.Content.Items.Misc_Melee
{
    public class Meowmeo : ModItem
    {
        public override string Texture => AssetDirectory.Misc_Melee + Name;

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.Meowmere);
            Item.damage = 320;
            Item.knockBack = 4f;
            Item.useStyle = ItemUseStyleID.Rapier; // Makes the player do the proper arm motion
            Item.useAnimation = 11;
            Item.useTime = 11;
            Item.UseSound = CoraliteSoundID.Zenith_Item169;
            Item.autoReuse = true;
            Item.noUseGraphic = true; // The sword is actually a "projectile", so the item should not be visible when used
            Item.noMelee = true; // The projectile will do the damage and not the item

            Item.shoot = ModContent.ProjectileType<MeowmeoProj>(); // The projectile is what makes a shortsword work
            Item.shootSpeed = 3.3f; // This value bleeds into the behavior of the projectile as velocity, keep that in mind when tweaking values
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(0, 40);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = -1; i < 2; i += 2)
            {
                int texType = -Main.rand.Next(1, 22);

                if (CoraliteWorld.chaosWorld)
                    texType = Main.rand.Next(1, ItemLoader.ItemCount);

                Projectile.NewProjectile(source, position, velocity.RotatedBy((i * 0.25f) + Main.rand.NextFloat(-0.1f, 0.1f)).SafeNormalize(Vector2.Zero) * 14, ModContent.ProjectileType<MeowmeoSpecialProj>(),
                    damage, knockback, player.whoAmI, ai2: texType);
            }

            Projectile.NewProjectile(source, position, velocity.SafeNormalize(Vector2.Zero) * 16, ProjectileID.Meowmere,
                damage, knockback, player.whoAmI);
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Meowmere)
                .AddIngredient<Cattongue>()//红：猫舌头
                .AddIngredient<FlyingDragonBaby>()
                .AddIngredient<TheCatMansBlade>()//橙：南瓜剑
                .AddIngredient(ItemID.GoldShortsword)//黄:金短剑
                .AddIngredient<TerraShortSword>()
                .AddIngredient<Treeler>()//绿：种子弯刀
                .AddIngredient<CatTreeSword>()
                .AddIngredient<BeamShortSword>()//青：光束剑
                .AddIngredient(ItemID.PiercingStarlight)
                .AddIngredient<IceShortSword>()//蓝：冰雪刃
                .AddIngredient(ItemID.ShadowFlameKnife)//紫：暗影焰刀
                .AddIngredient<SmallBee>()
                .AddTile(TileID.RainbowBrick)
                .AddCondition(CoraliteConditions.CoralCat)
                .Register();

            Recipe.Create(ItemID.RainbowBrick)
                .AddIngredient(ItemID.LunarBar, 4)
                .AddTile(TileID.LunarCraftingStation)
                .AddCondition(CoraliteConditions.CoralCat)
                .AddDecraftCondition(CoraliteConditions.CoralCat, Condition.DownedMoonLord)
                .Register();
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D rotTex = ModContent.Request<Texture2D>(AssetDirectory.Misc_Melee + "MeowmeoRotation").Value;
            var frame = rotTex.Frame(1, 30, 0, (int)(Main.GlobalTimeWrappedHourly * 35 % 30));

            spriteBatch.Draw(rotTex, Item.Bottom - Main.screenPosition, frame, lightColor.MultiplyRGBA(alphaColor), rotation, new Vector2(frame.Width / 2, frame.Height * 0.9f), scale, 0, 0);
            return false;
        }
    }

    public class MeowmeoProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Misc_Melee + "Meowmeo";

        public const int FadeInDuration = 6;
        public const int FadeOutDuration = 3;

        public const int TotalDuration = 16;

        // The "width" of the blade
        public float CollisionWidth => 10f * Projectile.scale;

        public int Timer
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(36); // This sets width and height to the same value (important when projectiles can rotate)
            Projectile.aiStyle = -1; // Use our own AI to customize how it behaves, if you don't want that, keep this at ProjAIStyleID.ShortSword. You would still need to use the code in SetVisualOffsets() though
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.scale = 1f;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ownerHitCheck = true; // Prevents hits through tiles. Most melee weapons that use projectiles have this
            Projectile.extraUpdates = 1; // Update 1+extraUpdates times per tick
            Projectile.timeLeft = 360; // This value does not matter since we manually kill it earlier, it just has to be higher than the duration we use in AI
            Projectile.hide = true; // Important when used alongside player.heldProj. "Hidden" projectiles have special draw conditions
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Timer += 1;
            if (Timer >= TotalDuration)
            {
                // Kill the projectile if it reaches it's intented lifetime
                Projectile.Kill();
                return;
            }
            else
            {
                // Important so that the sprite draws "in" the player's hand and not fully infront or behind the player
                player.heldProj = Projectile.whoAmI;
            }

            // Fade in and out
            // GetLerpValue returns a value between 0f and 1f - if clamped is true - representing how far Timer got along the "distance" defined by the first two parameters
            // The first call handles the fade in, the second one the fade out.
            // Notice the second call's parameters are swapped, this means the result will be reverted
            Projectile.Opacity = Utils.GetLerpValue(0f, FadeInDuration, Timer, clamped: true) * Utils.GetLerpValue(TotalDuration, TotalDuration - FadeOutDuration, Timer, clamped: true);

            // Keep locked onto the player, but extend further based on the given velocity (Requires ShouldUpdatePosition returning false to work)
            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, reverseRotation: false, addGfxOffY: false);
            Projectile.Center = playerCenter + (Projectile.velocity * (Timer - 1f));

            // Set spriteDirection based on moving left or right. Left -1, right 1
            Projectile.spriteDirection = (Vector2.Dot(Projectile.velocity, Vector2.UnitX) >= 0f).ToDirectionInt();

            // Point towards where it is moving, applied offset for top right of the sprite respecting spriteDirection
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 - (MathHelper.PiOver4 * Projectile.spriteDirection);

            // The code in this method is important to align the sprite with the hitbox how we want it to
            SetVisualOffsets();
        }

        private void SetVisualOffsets()
        {
            // 32 is the sprite size (here both width and height equal)
            const int HalfSpriteWidth = 36 / 2;
            const int HalfSpriteHeight = 44 / 2;

            int HalfProjWidth = Projectile.width / 2;
            int HalfProjHeight = Projectile.height / 2;

            // Vanilla configuration for "hitbox in middle of sprite"
            DrawOriginOffsetX = 0;
            DrawOffsetX = -(HalfSpriteWidth - HalfProjWidth);
            DrawOriginOffsetY = -(HalfSpriteHeight - HalfProjHeight);

            // Vanilla configuration for "hitbox towards the end"
            //if (Projectile.spriteDirection == 1) {
            //	DrawOriginOffsetX = -(HalfProjWidth - HalfSpriteWidth);
            //	DrawOffsetX = (int)-DrawOriginOffsetX * 2;
            //	DrawOriginOffsetY = 0;
            //}
            //else {
            //	DrawOriginOffsetX = (HalfProjWidth - HalfSpriteWidth);
            //	DrawOffsetX = 0;
            //	DrawOriginOffsetY = 0;
            //}
        }

        public override bool ShouldUpdatePosition()
        {
            // Update Projectile.Center manually
            return false;
        }

        public override void CutTiles()
        {
            // "cutting tiles" refers to breaking pots, grass, queen bee larva, etc.
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 start = Projectile.Center;
            Vector2 end = start + (Projectile.velocity.SafeNormalize(-Vector2.UnitY) * 10f);
            Utils.PlotTileLine(start, end, CollisionWidth, DelegateMethods.CutTiles);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // "Hit anything between the player and the tip of the sword"
            // shootSpeed is 2.1f for reference, so this is basically plotting 12 pixels ahead from the center
            Vector2 start = Projectile.Center;
            Vector2 end = start + (Projectile.velocity * 6f);
            float collisionPoint = 0f; // Don't need that variable, but required as parameter
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, CollisionWidth, ref collisionPoint);
        }
    }

    public class MeowmeoSpecialProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public enum ShortSwordType
        {
            BladeOfCatnip = 1,
            Excatbar,
            NightsCage,
            Nuranasa,
            ShadowsBane,
            SmallVolcano,
            TerraShortSword,
            TomatoButcherer,
            TrueExcatbar,
            TrueNightsCage,
            SmallBee,
            Cattongue,
            CatTreeSword,
            BeamShortSword,
            TheCatMansBlade,
            FlyingDragonBaby,
            IceShortSword,

            GoldShortsword,
            ChlorophyteSaber,
            PiercingStarlight,
            ShadowFlameKnife,
        }

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.OnlyPosition, 10);
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 8;
            AIType = ProjectileID.Meowmere;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] == 1)
                SoundEngine.PlaySound(CoraliteSoundID.Meowmere, Projectile.Center);
            if (Projectile.ai[0] >= 5f)
            {
                Projectile.position += Projectile.velocity;
                Projectile.Kill();
            }
            else
            {
                if (Projectile.velocity.Y != oldVelocity.Y)
                    Projectile.velocity.Y = 0f - oldVelocity.Y;

                if (Projectile.velocity.X != oldVelocity.X)
                    Projectile.velocity.X = 0f - oldVelocity.X;
            }

            Vector2 spinningpoint = new Vector2(0f, -3f - (Projectile.ai[0] * 0.7f)).RotatedByRandom(3.1415927410125732);
            float num21 = 10f + (Projectile.ai[0] * 3f);
            Vector2 vector19 = new(1.05f, 1f);
            for (float i = 0f; i < num21; i += 1f)
            {
                int num23 = Dust.NewDust(Projectile.Center, 0, 0, DustID.RainbowTorch, 0f, 0f, 0, Color.Transparent);
                Main.dust[num23].position = Projectile.Center;
                Main.dust[num23].velocity = spinningpoint.RotatedBy((float)Math.PI * 2f * i / num21) * vector19 * (0.6f + (Main.rand.NextFloat() * 0.3f));
                Main.dust[num23].color = Main.hslToRgb(i / num21, 1f, 0.5f);
                Main.dust[num23].noGravity = true;
                Main.dust[num23].scale = 1f + (Projectile.ai[0] / 4f);
            }

            if (Projectile.IsOwnedByLocalPlayer())
            {
                int num24 = Projectile.width;
                int num25 = Projectile.height;
                int num26 = Projectile.penetrate;
                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = 40 + (8 * (int)Projectile.ai[0]);
                Projectile.Center = Projectile.position;
                Projectile.penetrate = -1;
                Projectile.Damage();
                Projectile.penetrate = num26;
                Projectile.position = Projectile.Center;
                Projectile.width = num24;
                Projectile.height = num25;
                Projectile.Center = Projectile.position;
            }

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            Vector2 vector44 = new Vector2(Projectile.width, Projectile.height) / 2f;
            for (int num401 = 0; num401 < Projectile.oldPos.Length; num401++)
            {
                if (!(Projectile.oldPos[num401] == Vector2.Zero))
                {
                    int num402 = Dust.NewDust(Projectile.oldPos[num401] + vector44, 0, 0, DustID.RainbowTorch, 0f, 0f, 150, Color.Transparent, 0.7f);
                    Main.dust[num402].color = Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.5f);
                    Main.dust[num402].noGravity = true;
                }
            }
        }

        public static int? GetTexture(int type, out float ExRot)
        {
            ExRot = 0f;
            if (type > 0)
                return type;

            ExRot = 0.785f;
            switch (-type)
            {
                default: return null;
                case (int)ShortSwordType.BladeOfCatnip:
                    return ModContent.ItemType<BladeOfCatnip>();
                case (int)ShortSwordType.Excatbar:
                    return ModContent.ItemType<Excatbar>();
                case (int)ShortSwordType.NightsCage:
                    return ModContent.ItemType<NightsCage>();
                case (int)ShortSwordType.Nuranasa:
                    return ModContent.ItemType<Nuranasa>();
                case (int)ShortSwordType.ShadowsBane:
                    return ModContent.ItemType<ShadowsBane>();
                case (int)ShortSwordType.SmallVolcano:
                    return ModContent.ItemType<SmallVolcano>();
                case (int)ShortSwordType.TerraShortSword:
                    return ModContent.ItemType<TerraShortSword>();
                case (int)ShortSwordType.TomatoButcherer:
                    return ModContent.ItemType<TomatoButcherer>();
                case (int)ShortSwordType.TrueExcatbar:
                    return ModContent.ItemType<TrueExcatbar>();
                case (int)ShortSwordType.TrueNightsCage:
                    return ModContent.ItemType<TrueNightsCage>();
                case (int)ShortSwordType.SmallBee:
                    return ModContent.ItemType<SmallBee>();
                case (int)ShortSwordType.Cattongue:
                    return ModContent.ItemType<Cattongue>();
                case (int)ShortSwordType.CatTreeSword:
                    return ModContent.ItemType<CatTreeSword>();
                case (int)ShortSwordType.BeamShortSword:
                    return ModContent.ItemType<BeamShortSword>();
                case (int)ShortSwordType.TheCatMansBlade:
                    return ModContent.ItemType<TheCatMansBlade>();
                case (int)ShortSwordType.FlyingDragonBaby:
                    return ModContent.ItemType<FlyingDragonBaby>();
                case (int)ShortSwordType.IceShortSword:
                    return ModContent.ItemType<IceShortSword>();
                case (int)ShortSwordType.GoldShortsword:
                    return ItemID.GoldShortsword;
                case (int)ShortSwordType.ChlorophyteSaber:
                    return ItemID.ChlorophyteSaber;
                case (int)ShortSwordType.PiercingStarlight:
                    return ItemID.PiercingStarlight;
                case (int)ShortSwordType.ShadowFlameKnife:
                    ExRot = 0;
                    return ItemID.ShadowFlameKnife;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects dir = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                dir = SpriteEffects.FlipHorizontally;

            Vector2 center = Projectile.Center + (Vector2.UnitY * Projectile.gfxOffY) - Main.screenPosition;

            Texture2D selfTex;

            int? projTexType = GetTexture((int)Projectile.ai[2], out float exRot);
            if (projTexType.HasValue)
            {
                Main.instance.LoadItem(projTexType.Value);
                selfTex = TextureAssets.Item[projTexType.Value].Value;
            }
            else
            {
                Main.instance.LoadProjectile(ProjectileID.Meowmere);
                selfTex = TextureAssets.Projectile[ProjectileID.Meowmere].Value;
            }

            Vector2 origin = selfTex.Size() / 2f;
            float rotation = Projectile.rotation - (Projectile.spriteDirection * exRot);
            Vector2 scale = Vector2.One * Projectile.scale;

            Main.instance.LoadProjectile(250);
            Texture2D value84 = TextureAssets.Projectile[250].Value;
            Vector2 origin22 = new(value84.Width / 2, 0f);
            Vector2 vector75 = new Vector2(Projectile.width, Projectile.height) / 2f;
            Color white3 = Color.White;
            white3.A = 127;
            for (int num316 = Projectile.oldPos.Length - 1; num316 > 0; num316--)
            {
                Vector2 vector76 = Projectile.oldPos[num316] + vector75;
                if (!(vector76 == vector75))
                {
                    Vector2 vector77 = Projectile.oldPos[num316 - 1] + vector75;
                    float rotation27 = (vector77 - vector76).ToRotation() - ((float)Math.PI / 2f);
                    Vector2 scale7 = new(1f, Vector2.Distance(vector76, vector77) / value84.Height);
                    Color color82 = white3 * (1f - (num316 / (float)Projectile.oldPos.Length));
                    Main.EntitySpriteDraw(value84, vector76 - Main.screenPosition, null, color82, rotation27, origin22, scale7, dir);
                }
            }

            Main.EntitySpriteDraw(selfTex, center, null, lightColor, rotation, origin, scale, dir);

            return false;
        }
    }
}
