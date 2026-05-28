using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.SteelChapter;
using Coralite.Content.DamageClasses;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Systems.KeySystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Steel
{
    /// <summary>
    /// 战士头
    /// </summary>
    [AutoloadEquip(EquipType.Head)]
    public class B9LaserMask : ModItem, IConsultableItem
    {
        public override string Texture => AssetDirectory.SteelItems + Name;
        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<SteelKnowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<SteelPage1>();

        public override void SetDefaults()
        {
            Item.SetShopValues(ItemRarityColor.LightRed4, Item.sellPrice(0, 3));
            Item.defense = 20;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<B9Breastplate>() && legs.type == ItemType<B9Legs>();
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Melee) += 0.08f;
            player.GetCritChance(DamageClass.Melee) += 5f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.1f;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = B9Breastplate.bonus.Value;
            B9Breastplate.B9ArmorSet(player);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<B9Alloy>(12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    /// <summary>
    /// 射手头
    /// </summary>
    [AutoloadEquip(EquipType.Head)]
    public class B9MonitorHead : ModItem, IConsultableItem
    {
        public override string Texture => AssetDirectory.SteelItems + Name;
        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<SteelKnowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<SteelPage1>();

        public override void SetDefaults()
        {
            Item.SetShopValues(ItemRarityColor.LightRed4, Item.sellPrice(0, 3));
            Item.defense = 6;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<B9Breastplate>() && legs.type == ItemType<B9Legs>();
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Ranged) += 0.15f;
            player.GetCritChance(DamageClass.Ranged) += 5f;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = B9Breastplate.bonus.Value;
            B9Breastplate.B9ArmorSet(player);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<B9Alloy>(12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    /// <summary>
    /// 法师头
    /// </summary>
    [AutoloadEquip(EquipType.Head)]
    public class B9SpaceHelmet : ModItem, IConsultableItem
    {
        public override string Texture => AssetDirectory.SteelItems + Name;
        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<SteelKnowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<SteelPage1>();

        public override void SetDefaults()
        {
            Item.SetShopValues(ItemRarityColor.LightRed4, Item.sellPrice(0, 3));
            Item.defense = 3;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<B9Breastplate>() && legs.type == ItemType<B9Legs>();
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Magic) += 0.15f;
            player.GetCritChance(DamageClass.Magic) += 5f;
            player.statManaMax2 += 100;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = B9Breastplate.bonus.Value;
            B9Breastplate.B9ArmorSet(player);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<B9Alloy>(12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    /// <summary>
    /// 召唤头
    /// </summary>
    [AutoloadEquip(EquipType.Head)]
    public class B9PlaneHead : ModItem, IConsultableItem
    {
        public override string Texture => AssetDirectory.SteelItems + Name;
        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<SteelKnowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<SteelPage1>();

        public static LocalizedText bonus;

        public override void Load()
        {
            bonus = this.GetLocalization("ArmorBonus");
        }

        public override void Unload()
        {
            bonus = null;
        }

        public override void SetDefaults()
        {
            Item.SetShopValues(ItemRarityColor.LightRed4, Item.sellPrice(0, 3));
            Item.defense = 1;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<B9Breastplate>() && legs.type == ItemType<B9Legs>();
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Summon) += 0.12f;
            player.whipRangeMultiplier += 0.1f;
            player.maxMinions += 1;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.maxMinions += 2;
            player.setBonus = bonus.Value;
            B9Breastplate.B9ArmorSet(player);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<B9Alloy>(12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    /// <summary>
    /// 仙灵头
    /// </summary>
    [AutoloadEquip(EquipType.Head)]
    public class B9BatteryHead : ModItem, IConsultableItem
    {
        public override string Texture => AssetDirectory.SteelItems + Name;
        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<SteelKnowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<SteelPage1>();

        public static LocalizedText bonus;

        public override void Load()
        {
            bonus = this.GetLocalization("ArmorBonus");
        }

        public override void Unload()
        {
            bonus = null;
        }

        public override void SetDefaults()
        {
            Item.SetShopValues(ItemRarityColor.LightRed4, Item.sellPrice(0, 3));
            Item.defense = 5;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<B9Breastplate>() && legs.type == ItemType<B9Legs>();
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<FairyDamage>() += 0.15f;
            player.GetCritChance<FairyDamage>() += 5f;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = B9Breastplate.bonus.Value;
            B9Breastplate.B9ArmorSet(player);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<B9Alloy>(12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    [PlayerEffect]
    [AutoloadEquip(EquipType.Body)]
    public class B9Breastplate : ModItem, IConsultableItem
    {
        public override string Texture => AssetDirectory.SteelItems + Name;
        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<SteelKnowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<SteelPage1>();

        public static LocalizedText bonus;
        public const int BonusAffectRadius = 16 * 5+8;

        public override void Load()
        {
            bonus = this.GetLocalization("ArmorBonus");
        }

        public override void Unload()
        {
            bonus = null;
        }

        public override void SetDefaults()
        {
            Item.SetShopValues(ItemRarityColor.LightRed4, Item.sellPrice(0, 2, 50));
            Item.defense = 13;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.06f;
        }

        public static void B9ArmorSet(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.AddEffect(nameof(B9Breastplate));

            if (player.Alives() && player.ownedProjectileCounts[ProjectileType<B9ArmorEffectProj>()] < 1)
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ProjectileType<B9ArmorEffectProj>(), 0, 0, player.whoAmI);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<B9Alloy>(22)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class B9Legs : ModItem, IConsultableItem
    {
        public override string Texture => AssetDirectory.SteelItems + Name;
        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<SteelKnowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<SteelPage1>();

        public override void SetDefaults()
        {
            Item.SetShopValues(ItemRarityColor.LightRed4, Item.sellPrice(0, 1, 50));
            Item.defense = 8;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.05f;
            player.moveSpeed += 0.06f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<B9Alloy>(16)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    [VaultLoaden(AssetDirectory.SteelItems)]
    public class B9ArmorEffectProj : ModProjectile
    {
        public override string Texture => AssetDirectory.SteelItems + Name;

        public Player Owner => Main.player[Projectile.owner];

        public static ATex B9ArmorEffectProj2 { get; set; }

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }

        public override bool? CanDamage() => false;
        public override bool? CanCutTiles() => false;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;
        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            if (!Owner.Alives() || !Owner.TryGetModPlayer(out CoralitePlayer cp) || !cp.HasEffect(nameof(B9Breastplate)))
            {
                Projectile.Kill();
                return;
            }

            if (Projectile.IsOwnedByLocalPlayer())//只有主人端正常搞搞
                Projectile.Center = Main.MouseWorld;
            else
                Projectile.Center = Owner.Center;

            Projectile.ai[0]++;
            if (Owner.ItemAnimationActive)
                Projectile.ai[0]++;
            if (Projectile.ai[0] > 60 * 2)
                Projectile.ai[0] = 0;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.IsOwnedByLocalPlayer() || Owner.HeldItem.damage < 1)
                return false;

            Texture2D tex = Projectile.GetTextureValue();
            Texture2D tex2 = B9ArmorEffectProj2.Value;
            Vector2 center = Projectile.Center - Main.screenPosition;
            Color c = new Color(255, 75, 30);
            float mouseLength = 6;
            float mouseScale = 0.75f;

            float f = Projectile.ai[0] / (60 * 2);
            float angle = 0;

            if (!Owner.ItemAnimationActive)
            {
                c *= 0.5f;
                mouseLength = 16;
                mouseScale = 1;
                angle = (int)Main.timeForVisualEffects * 0.02f;
            }

            //绘制在框框上
            Draw(center + new Vector2(-B9Breastplate.BonusAffectRadius), 0, 1, c);
            Draw(center + new Vector2(B9Breastplate.BonusAffectRadius, -B9Breastplate.BonusAffectRadius), MathHelper.PiOver2, 1, c);
            Draw(center + new Vector2(-B9Breastplate.BonusAffectRadius, B9Breastplate.BonusAffectRadius), -MathHelper.PiOver2, 1, c);
            Draw(center + new Vector2(B9Breastplate.BonusAffectRadius), MathHelper.Pi, 1, c);

            //绘制在鼠标旁边
            Draw2(center + new Vector2(-mouseLength).RotatedBy(angle), MathHelper.Pi + angle, mouseScale, c);
            Draw2(center + new Vector2(mouseLength, -mouseLength).RotatedBy(angle), -MathHelper.PiOver2 + angle, mouseScale, c);
            Draw2(center + new Vector2(-mouseLength, mouseLength).RotatedBy(angle), MathHelper.PiOver2 + angle, mouseScale, c);
            Draw2(center + new Vector2(mouseLength).RotatedBy(angle), angle, mouseScale, c);

            //绘制在中间扩散
            float length = Helper.BezierEase(f) * B9Breastplate.BonusAffectRadius;
            Color c2 = c * f;
            float scale = 0.5f + 0.5f * f;
            Draw(center + new Vector2(-length), 0, scale, c2);
            Draw(center + new Vector2(length, -length), MathHelper.PiOver2, scale, c2);
            Draw(center + new Vector2(-length, length), -MathHelper.PiOver2, scale, c2);
            Draw(center + new Vector2(length), MathHelper.Pi, scale, c2);

            Rectangle r = Utils.CenteredRectangle(Main.MouseWorld, new Vector2(B9Breastplate.BonusAffectRadius * 2));

            foreach (var npc in Main.ActiveNPCs)
                if (npc.CanBeChasedBy() && r.Contains((int)npc.Center.X, (int)npc.Center.Y))
                    DrawTargetNPC(npc.Center - Main.screenPosition);

            void Draw(Vector2 pos, float rot, float scale, Color c)
            {
                Main.spriteBatch.Draw(tex, pos, null, c, rot, Vector2.Zero, scale, 0, 0);
            }

            void Draw2(Vector2 pos, float rot, float scale, Color c)
            {
                for (int i = 0; i < 4; i++)
                {
                    Main.spriteBatch.Draw(tex2, pos + (i * MathHelper.PiOver2).ToRotationVector2(), null, c * 0.3f, rot, Vector2.Zero, scale, 0, 0);
                }
                Main.spriteBatch.Draw(tex2, pos, null, c, rot, Vector2.Zero, scale, 0, 0);
            }

            void DrawTargetNPC(Vector2 pos)
            {
                Draw2(pos + new Vector2(-mouseLength).RotatedBy(angle), MathHelper.Pi + angle, mouseScale, c);
                Draw2(pos + new Vector2(mouseLength, -mouseLength).RotatedBy(angle), -MathHelper.PiOver2 + angle, mouseScale, c);
                Draw2(pos + new Vector2(-mouseLength, mouseLength).RotatedBy(angle), MathHelper.PiOver2 + angle, mouseScale, c);
                Draw2(pos + new Vector2(mouseLength).RotatedBy(angle), angle, mouseScale, c);
            }

            return false;
        }
    }
}
