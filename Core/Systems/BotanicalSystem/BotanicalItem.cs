//using Coralite.Content.Items.Botanical;
//using Coralite.Helpers;
//using System.Collections.Generic;
//using Terraria;
//using Terraria.Audio;
//using Terraria.ID;
//using Terraria.ModLoader.IO;
//using static Terraria.ModLoader.ModContent;

//namespace Coralite.Core.Systems.BotanicalSystem
//{
//    public class BotanicalItem : GlobalItem
//    {
//        public override bool InstancePerEntity => true;
//        protected override bool CloneNewInstances => true;

//        public bool botanicalItem = false;
//        public bool isIdentified = false;

//        public int DominantGrowTime = 100;//显性植物生长速度基因
//        public int RecessiveGrowTime = 100;//隐性植物生长速度基因

//        public int DominantLevel;//显性强度基因
//        public int RecessiveLevel;//隐性强度基因

//        public override GlobalItem Clone(Item from, Item to)
//        {
//            if (from == null || to == null)
//                return base.Clone(from, to);
//            if (from.IsAir || to.IsAir)
//                return base.Clone(from, to);
//            BotanicalItem BFrom = from.GetBotanicalItem();
//            BotanicalItem BTo = to.GetBotanicalItem();

//            if (!BFrom.botanicalItem)
//                return base.Clone(from, to);

//            BTo.DominantGrowTime = BFrom.DominantGrowTime;
//            BTo.RecessiveGrowTime = BFrom.RecessiveGrowTime;
//            BTo.DominantLevel = BFrom.DominantLevel;
//            BTo.RecessiveLevel = BFrom.RecessiveLevel;
//            return base.Clone(from, to);
//        }

//        public override bool CanStack(Item item1, Item item2)
//        {
//            if (!item1.TryGetGlobalItem(out BotanicalItem botanicalItem1) || !item2.TryGetGlobalItem(out BotanicalItem botanicalItem2))
//                return base.CanStack(item1, item2);

//            if (!botanicalItem1.botanicalItem)
//                return base.CanStack(item1, item2);
//            if (!botanicalItem2.botanicalItem)
//                return base.CanStack(item1, item2);

//            if (botanicalItem1.isIdentified != botanicalItem2.isIdentified)
//                return false;

//            if (botanicalItem1.DominantGrowTime != botanicalItem2.DominantGrowTime)
//                return false;
//            if (botanicalItem1.RecessiveGrowTime != botanicalItem2.RecessiveGrowTime)
//                return false;

//            if (botanicalItem1.DominantLevel != botanicalItem2.DominantLevel)
//                return false;
//            if (botanicalItem1.RecessiveLevel != botanicalItem2.RecessiveLevel)
//                return false;

//            if (item1.stack == item1.maxStack || item2.stack == item2.maxStack)
//                return false;

//            return true;
//        }

//        public override void SetDefaults(Item item)
//        {
//            //switch (item.type)
//            //{
//            //    case ItemID.DaybloomSeeds://太阳花种子
//            //        SetVanillaPlant(item, 10, 10, 0, 0, TileType<CoraliteDaybloom>());
//            //        break;

//            //    case ItemID.BlinkrootSeeds://闪耀根种子
//            //        SetVanillaPlant(item, 10, 10, 0, 0, TileType<CoraliteBlinkroot>());
//            //        break;

//            //    case ItemID.MoonglowSeeds://月光草种子
//            //        SetVanillaPlant(item, 10, 10, 0, 0, TileType<CoraliteMoonglow>());
//            //        break;

//            //    case ItemID.WaterleafSeeds://幌菊种子
//            //        SetVanillaPlant(item, 10, 10, 0, 0, TileType<CoraliteWaterleaf>());
//            //        break;

//            //    case ItemID.ShiverthornSeeds://寒颤棘种子
//            //        SetVanillaPlant(item, 10, 10, 0, 0, TileType<CoraliteShiverthorn>());
//            //        break;

//            //    case ItemID.DeathweedSeeds://死亡草种子
//            //        SetVanillaPlant(item, 10, 10, 0, 0, TileType<CoraliteDeathweed>());
//            //        break;

//            //    case ItemID.FireblossomSeeds://火焰花种子
//            //        SetVanillaPlant(item, 10, 10, 0, 0, TileType<CoraliteFireblossom>());
//            //        break;

//            //    case ItemID.Daybloom:
//            //    case ItemID.Blinkroot:
//            //    case ItemID.Moonglow:
//            //    case ItemID.Waterleaf:
//            //    case ItemID.Shiverthorn:
//            //    case ItemID.Deathweed:
//            //    case ItemID.Fireblossom:
//            //        botanicalItem = true;
//            //        DominantGrowTime = 10;
//            //        RecessiveGrowTime = 10;
//            //        break;

//            //    default: break;
//            //}
//        }

//        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
//        {
//            if (!botanicalItem)
//                return;

//            //string Genotypes;
//            //if (isIdentified)
//            //{
//            //    //显性生长时间
//            //    Genotypes = $"生长时间: {DominantGrowTime}";
//            //    TooltipLine line = new TooltipLine(Mod, "DGrowTime", Genotypes);
//            //    tooltips.Add(line);

//            //    //隐性生长时间
//            //    Genotypes = $"生长时间: {RecessiveGrowTime}";
//            //    line = new TooltipLine(Mod, "DGrowTime", Genotypes);
//            //    line.OverrideColor = Color.Gray;
//            //    tooltips.Add(line);

//            //    //显性强度
//            //    Genotypes = $"等级: {DominantLevel}";
//            //    line = new TooltipLine(Mod, "DGrowTime", Genotypes);
//            //    tooltips.Add(line);

//            //    //隐性强度
//            //    Genotypes = $"等级: {RecessiveLevel}";
//            //    line = new TooltipLine(Mod, "DGrowTime", Genotypes);
//            //    line.OverrideColor = Color.Gray;
//            //    tooltips.Add(line);
//            //}
//            //else
//            //{
//            //    string notDentified = "该植物还未被鉴定";
//            //    TooltipLine line = new TooltipLine(Mod, "NotDentified", notDentified);
//            //    line.OverrideColor = Color.Gray;
//            //    tooltips.Add(line);
//            //}
//        }

//        public override bool CanRightClick(Item item)
//        {
//            return botanicalItem && !isIdentified && Main.LocalPlayer.HasItem(ItemType<IdentifyLoupe>());
//        }

//        public override void RightClick(Item item, Player player)
//        {
//            if (!botanicalItem || item.IsAir)
//                return;

//            item.stack++;

//            if (!player.HasItem(ItemType<IdentifyLoupe>()))
//                return;

//            //消耗5魔力
//            if (player.statMana < 20)
//                return;

//            player.statMana -= 20;

//            BotanicalItem bItem = item.GetBotanicalItem();
//            if (!bItem.botanicalItem || bItem.isIdentified)
//                return;

//            bItem.isIdentified = true;
//            item.RebuildTooltip();
//            SoundEngine.PlaySound(SoundID.Item4, player.position);
//        }

//        public override void SaveData(Item item, TagCompound tag)
//        {
//            tag.Add("isIdentified", isIdentified);
//            tag.Add("DominantGrowTime", DominantGrowTime);
//            tag.Add("RecessiveGrowTime", RecessiveGrowTime);
//            tag.Add("DominantLevel", DominantLevel);
//            tag.Add("RecessiveLevel", RecessiveLevel);
//        }

//        public override void LoadData(Item item, TagCompound tag)
//        {
//            isIdentified = tag.GetBool("isIdentified");
//            DominantGrowTime = tag.GetInt("DominantGrowTime");
//            RecessiveGrowTime = tag.GetInt("RecessiveGrowTime");
//            DominantLevel = tag.GetInt("DominantLevel");
//            RecessiveLevel = tag.GetInt("RecessiveLevel");
//        }

//        public void SetVanillaPlant(Item item, int DGrowTime, int RGrowTime, int DLevel, int Rlevel, int TileType)
//        {
//            botanicalItem = true;
//            DominantGrowTime = DGrowTime;
//            RecessiveGrowTime = RGrowTime;
//            DominantLevel = DLevel;
//            RecessiveLevel = Rlevel;
//            item.createTile = TileType;
//        }
//    }
//}
