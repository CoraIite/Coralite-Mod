using Coralite.Content.CoraliteNotes.DashBowChapter;
using Coralite.Content.CoraliteNotes.FlowerGunChapter;
using Coralite.Content.Items.Materials;
using Coralite.Content.Items.Misc_Melee;
using Coralite.Content.ModPlayers;
using Coralite.Content.WorldGeneration;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Items;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.GlobalItems
{
    public partial class CoraliteGlobalItem : GlobalItem, ILocalizedModType, IVariantItem
    {
        public static LocalizedText CopperCoralCat;
        public static LocalizedText RubyCoralCat;
        public static LocalizedText TerraCoralCat;
        public static LocalizedText RainBowCoralCat;

        public static LocalizedText DashPriority;
        public static LocalizedText Dashable;
        public static LocalizedText[] DashPriorityLevel;

        /// <summary>
        /// 0：特殊使用<br></br>
        /// 1：寒冷伤害<br></br>
        /// 2：美味伤害
        /// </summary>
        private BitsByte bit1;

        /// <summary>
        /// 是否能通过特殊攻击键来使用武器
        /// </summary>
        public bool SpecialUse
        {
            get => bit1[0];
            set => bit1[0] = value;
        }

        public override bool InstancePerEntity => true;

        public string LocalizationCategory => "GlobalItems";

        public override void Load()
        {
            if (Main.dedServ)
                return;

            CopperCoralCat = this.GetLocalization("CopperCoralCat", () => "铜质猫猫币");
            RubyCoralCat = this.GetLocalization("RubyCoralCat", () => "红玉猫猫币");
            TerraCoralCat = this.GetLocalization("TerraCoralCat", () => "泰拉猫猫币");
            RainBowCoralCat = this.GetLocalization("RainBowCoralCat", () => "彩虹猫猫币");
            Cold = this.GetLocalization("Cold", () => "[c/5cd7f9:寒冷]");
            Edible = this.GetLocalization("Edible", () => "[c/f0d0b7:可食用]");

            DashPriority = this.GetLocalization("DashPriority");
            Dashable = this.GetLocalization("Dashable");
            DashPriorityLevel = new LocalizedText[5];
            for (int i = 0; i < 5; i++)
                DashPriorityLevel[i] = this.GetLocalization("DashPriorityLevel" + i);
        }

        public override void Unload()
        {
            CopperCoralCat = null;
            RubyCoralCat = null;
            TerraCoralCat = null;
            RainBowCoralCat = null;
            Cold = null;
            Edible = null;
            DashPriority = null;
            DashPriorityLevel = null;
            Dashable = null;
        }

        public override void SetDefaults(Item item)
        {
            if (item.ModItem != null && item.ModItem.Mod is Coralite)
                item.width = item.height = 28;

            switch (item.type)
            {
                default: break;
                case ItemID.PhoenixBlaster: //削弱凤凰爆破枪，目的是为了给幽兰让路
                    item.damage = 32;
                    break;
                case ItemID.IceRod:     //削弱冰雪魔仗，因为它现在在肉前就能获得
                    item.damage = (int)(item.damage * 0.45f);
                    item.value = Item.sellPrice(0, 0, 10, 0);
                    item.mana = 9;
                    item.rare = ItemRarityID.Orange;
                    item.useTime = item.useAnimation = 8;
                    break;
                case ItemID.Coal:
                    item.maxStack = Item.CommonMaxStack;
                    break;
                case ItemID.Dynamite: //雷管当弹药
                    item.ammo = ItemID.Dynamite;
                    break;
                case ItemID.Meowmere:
                    if (item.Variant == ItemVariants.WeakerVariant)
                    {
                        item.value = Item.sellPrice(0, 0, 0, 99);
                        item.rare = ItemRarityID.White;
                        item.damage = 20;
                        item.useAnimation = 15;
                        item.useTime = 15;
                        item.shoot = ProjectileID.None;
                    }

                    break;
                case ItemID.MeowmereMinecart:
                    if (item.Variant == ItemVariants.StrongerVariant)
                    {
                        item.value = Item.sellPrice(0, 99);
                        item.rare = ItemRarityID.Red;
                        item.damage = 256;
                        item.useAnimation = 10;
                        item.useTime = 10;
                        item.useStyle = ItemUseStyleID.Swing;
                        item.shoot = ProjectileID.Meowmere;
                        item.shootSpeed = 14;
                        item.autoReuse = true;
                        item.DamageType = DamageClass.Melee;
                        item.UseSound = CoraliteSoundID.Swing_Item1;
                    }
                    break;
                case ItemID.TerraFartMinecart:
                    if (item.Variant == ItemVariants.StrongerVariant)
                    {
                        item.value = Item.sellPrice(0, 20);
                        item.rare = ItemRarityID.Yellow;
                        item.damage = 120;
                        item.useAnimation = 15;
                        item.useTime = 15;
                        item.useStyle = ItemUseStyleID.Swing;
                        item.shoot = ProjectileID.TerraBeam;
                        item.shootSpeed = 14;
                        item.autoReuse = true;
                        item.DamageType = DamageClass.Melee;
                        item.UseSound = CoraliteSoundID.TerraBlade_Item60;
                    }
                    break;
            }

            RegisterColdDamageWeapon(item.type);
            RegisterEdibleDamageWeapon(item.type);
        }

        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (CoraliteWorld.CoralCatWorld)
            {
                switch (item.type)
                {
                    default:
                        break;
                    case ItemID.CopperCoin:
                        {
                            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.MiscItems + "CopperCoralCat").Value;
                            spriteBatch.Draw(mainTex, position, null, drawColor, 0, mainTex.Size() / 2, scale, 0, 0);
                        }
                        return false;
                    case ItemID.SilverCoin:
                        {
                            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.MiscItems + "RubyCoralCat").Value;
                            spriteBatch.Draw(mainTex, position, null, drawColor, 0, mainTex.Size() / 2, scale, 0, 0);
                        }
                        return false;
                    case ItemID.GoldCoin:
                        {
                            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.MiscItems + "TerraCoralCat").Value;
                            spriteBatch.Draw(mainTex, position, null, drawColor, 0, mainTex.Size() / 2, scale, 0, 0);
                        }
                        return false;
                    case ItemID.PlatinumCoin:
                        {
                            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.MiscItems + "RainBowCoralCat").Value;
                            spriteBatch.Draw(mainTex, position, null, drawColor, 0, mainTex.Size() / 2, scale, 0, 0);
                        }
                        return false;
                }
            }

            return base.PreDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }

        public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Vector2 center = item.Center - Main.screenPosition;
            if (CoraliteWorld.CoralCatWorld)
            {
                switch (item.type)
                {
                    default:
                        break;
                    case ItemID.CopperCoin:
                        {
                            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.MiscItems + "CopperCoralCat").Value;
                            spriteBatch.Draw(mainTex, center, null, lightColor, 0, mainTex.Size() / 2, scale, 0, 0);
                        }
                        return false;
                    case ItemID.SilverCoin:
                        {
                            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.MiscItems + "RubyCoralCat").Value;
                            spriteBatch.Draw(mainTex, center, null, lightColor, 0, mainTex.Size() / 2, scale, 0, 0);
                        }
                        return false;
                    case ItemID.GoldCoin:
                        {
                            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.MiscItems + "TerraCoralCat").Value;
                            spriteBatch.Draw(mainTex, center, null, lightColor, 0, mainTex.Size() / 2, scale, 0, 0);
                        }
                        return false;
                    case ItemID.PlatinumCoin:
                        {
                            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.MiscItems + "RainBowCoralCat").Value;
                            spriteBatch.Draw(mainTex, center, null, lightColor, 0, mainTex.Size() / 2, scale, 0, 0);
                        }
                        return false;
                }
            }

            return base.PreDrawInWorld(item, spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
        }

        public override void PickAmmo(Item weapon, Item ammo, Player player, ref int type, ref float speed, ref StatModifier damage, ref float knockback)
        {
            ApplySpecialDamage(player, ref damage);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (CoraliteWorld.CoralCatWorld)
            {
                TooltipLine name = tooltips.Find(line => line.Mod == "Terraria" && line.Name == "ItemName");
                if (name != null)
                    switch (item.type)
                    {
                        default:
                            break;
                        case ItemID.CopperCoin:
                            name.Text = CopperCoralCat.Value;
                            return;
                        case ItemID.SilverCoin:
                            name.Text = RubyCoralCat.Value;
                            return;
                        case ItemID.GoldCoin:
                            name.Text = TerraCoralCat.Value;
                            return;
                        case ItemID.PlatinumCoin:
                            name.Text = RainBowCoralCat.Value;
                            return;
                    }
            }

            TooltipLine damage = tooltips.Find(line => line.Mod == "Terraria" && line.Name == "Damage");
            if (damage != null)
            {
                string[] text = damage.Text.Split(" ");
                if (text.Length > 1)
                {
                    List<string> newText = new()
                        {
                            text[0],
                            " "
                        };

                    if (ColdDamage)
                    {
                        newText.Add(Cold.Value);
                        newText.Add(" ");
                    }

                    if (EdibleDamage)
                    {
                        newText.Add(Edible.Value);
                        newText.Add(" ");
                    }

                    newText.Add(text[1]);
                    damage.Text = string.Concat([.. newText]);
                }
            }

            ModifyFairyTooltips(item,tooltips);

            if (item.ModItem is IDashable dash&&dash.DrawTips)
            {
                TooltipLine line2 = new TooltipLine(Mod, "Dashable", Dashable.Value);
                tooltips.Add(line2);

                float priority = dash.Priority;

                string level = priority switch
                {
                    >= IDashable.AccessoryDashLow => DashPriorityLevel[4].Value,//最低
                    >= 70 and < IDashable.AccessoryDashLow => DashPriorityLevel[3].Value,//较低
                    >= 40 and < 70 => DashPriorityLevel[2].Value,//中
                    >= IDashable.AccessoryDashHigh and < 40 => DashPriorityLevel[1].Value,//较高
                    _ => DashPriorityLevel[0].Value,//最高
                };
                TooltipLine line = new TooltipLine(Mod, "DashPriority", DashPriority.Value + level);
                tooltips.Add(line);
            }
        }

        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            ApplySpecialDamage(player, ref damage);
        }

        public override void UpdateInventory(Item item, Player player)
        {
            CoralCatWorldTransForm(item);
            if (item.useAmmo == AmmoID.Bullet)
                KnowledgeSystem.CheckForUnlock<FlowerGunKnowledge>(player.Center, Color.Red);
            else if (item.useAmmo == AmmoID.Arrow)
                KnowledgeSystem.CheckForUnlock<DashBowKnowledge>(player.Center, new Color(0,228,205));
        }

        public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
        {
            CoralCatWorldTransForm(item);
        }

        public static void CoralCatWorldTransForm(Item item)
        {
            if (!CoraliteWorld.CoralCatWorld)
                return;

            switch (item.type)
            {
                default: return;
                case ItemID.BrokenHeroSword:
                    item.SetDefaults(ModContent.ItemType<BrokenHeroShortSword>());
                    break;
                case ItemID.TerraBlade:
                    item.SetDefaults(ModContent.ItemType<TerraShortSword>());
                    break;
                case ItemID.TrueNightsEdge:
                    item.SetDefaults(ModContent.ItemType<TrueNightsCage>());
                    break;
                case ItemID.TrueExcalibur:
                    item.SetDefaults(ModContent.ItemType<TrueExcatbar>());
                    break;
                case ItemID.Excalibur:
                    item.SetDefaults(ModContent.ItemType<Excatbar>());
                    break;
                case ItemID.NightsEdge:
                    item.SetDefaults(ModContent.ItemType<NightsCage>());
                    break;
                case ItemID.LightsBane:
                    item.SetDefaults(ModContent.ItemType<ShadowsBane>());
                    break;
                case ItemID.BloodButcherer:
                    item.SetDefaults(ModContent.ItemType<TomatoButcherer>());
                    break;
                case ItemID.Muramasa:
                    item.SetDefaults(ModContent.ItemType<Nuranasa>());
                    break;
                case ItemID.FieryGreatsword:
                    item.SetDefaults(ModContent.ItemType<SmallVolcano>());
                    break;
                case ItemID.BladeofGrass:
                    item.SetDefaults(ModContent.ItemType<BladeOfCatnip>());
                    break;
                case ItemID.BeeKeeper:
                    item.SetDefaults(ModContent.ItemType<SmallBee>());
                    break;
                case ItemID.Bladetongue:
                    item.SetDefaults(ModContent.ItemType<Cattongue>());
                    break;
                case ItemID.ChristmasTreeSword:
                    item.SetDefaults(ModContent.ItemType<CatTreeSword>());
                    break;
                case ItemID.BeamSword:
                    item.SetDefaults(ModContent.ItemType<BeamShortSword>());
                    break;
                case ItemID.TheHorsemansBlade:
                    item.SetDefaults(ModContent.ItemType<TheCatMansBlade>());
                    break;
                case ItemID.DD2SquireBetsySword:
                    item.SetDefaults(ModContent.ItemType<FlyingDragonBaby>());
                    break;
                case ItemID.IceBlade:
                    item.SetDefaults(ModContent.ItemType<IceShortSword>());
                    break;
                case ItemID.Seedler:
                    item.SetDefaults(ModContent.ItemType<Treeler>());
                    break;
            }

            SoundEngine.PlaySound(CoraliteSoundID.Meowmere);
        }

        public void ApplySpecialDamage(Player player, ref StatModifier damage)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                if (ColdDamage)
                    damage = damage.CombineWith(cp.coldDamageBonus);
                if (EdibleDamage)
                    damage = damage.CombineWith(cp.deliciousDamageBonus);
            }
        }

        public void AddVarient()
        {
            ItemVariants.AddVariant(ItemID.Meowmere, ItemVariants.WeakerVariant, CoraliteConditions.CoralCat);
            ItemVariants.AddVariant(ItemID.MeowmereMinecart, ItemVariants.StrongerVariant, CoraliteConditions.CoralCat);
            ItemVariants.AddVariant(ItemID.TerraFartMinecart, ItemVariants.StrongerVariant, CoraliteConditions.CoralCat);
        }
    }
}