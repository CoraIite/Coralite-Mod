using Coralite.Content.UI;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.MagikeSystem.EnchantSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    public abstract class MagikeFactory_EnchantPool : MagikeFactory, IMagikeSingleItemContainer
    {
        public Item containsItem = new Item();

        public MagikeFactory_EnchantPool(int magikeMax, int workTimeMax) : base(magikeMax, workTimeMax) { }

        public abstract Color MainColor { get; }
        public Item ContainsItem { get => containsItem; set => containsItem = value; }

        public Enchant.ID? targetedEnchantSlot;

        public override void OnKill()
        {
            MagikeItemSlotPanel.visible = false;
            UILoader.GetUIState<MagikeItemSlotPanel>().Recalculate();

            if (!containsItem.IsAir)
                Item.NewItem(new EntitySource_TileEntity(this), Position.ToWorldCoordinates(16, -8), containsItem);
        }

        public override bool CanWork()
        {
            if (containsItem is not null && !containsItem.IsAir &&
                (containsItem.damage > 0 || 
                ((containsItem.accessory || containsItem.defense > 0) && containsItem.TryGetGlobalItem(out MagikeItem mi) && mi.accessoryOrArmorCanEnchant)) &&
                magike >= GetMagikeCost(this, containsItem))
            {
                return base.CanWork();
            }

            workTimer = -1;
            return false;
        }

        public override void DuringWork()
        {
            float factor = workTimer / (float)workTimeMax;

            Vector2 center = Position.ToWorldCoordinates(16, -16);
            if (workTimer % 8 == 0)
            {
                Dust dust = Dust.NewDustPerfect(center + new Vector2(Main.rand.Next(-16, 16), 32), DustID.FireworksRGB, -Vector2.UnitY * Main.rand.NextFloat(0.8f, 3f), newColor: MainColor);
                dust.noGravity = true;
            }

            float width = 24 - factor * 22;

            Dust dust2 = Dust.NewDustPerfect(center + Main.rand.NextVector2CircularEdge(width, width), DustID.LastPrism, Vector2.Zero, newColor: MainColor);
            dust2.noGravity = true;
        }

        public override void WorkFinish()
        {
            if (containsItem is not null && !containsItem.IsAir &&
                (containsItem.damage > 0 || containsItem.accessory || containsItem.defense > 0))
            {
                int cost = GetMagikeCost(this, containsItem);
                if (magike < cost)
                    return;

                int whichslot = Main.rand.NextFromList(0, 0, 0, 0, 0, 1, 1, 1, 2, 2);//50%概率为0，30概率为1，20%概率为2
                Enchant enchant = containsItem.GetGlobalItem<MagikeItem>().Enchant;
                if (enchant.datas == null)
                    return;

                //检测当前的注魔是否为最高等级，如果为最高等级就判断一下其他的等级
                LevelCheck(enchant, whichslot, out int checkedSlot);
                checkedSlot = MaxLevelCheck(checkedSlot, enchant);

                //定向注魔
                if (targetedEnchantSlot != null)
                    checkedSlot = (int)targetedEnchantSlot.Value;

                //获取注魔词条池
                EnchantEntityPool pool = GetEnchantPool(containsItem);
                //获取子注魔词条池
                Enchant.Level level = enchant.datas[checkedSlot] == null ? Enchant.Level.Nothing : enchant.datas[checkedSlot].level;
                IEnumerable<EnchantData> sonPool = pool.GetSonPool(checkedSlot, GetLevel(level));
                if (!sonPool.Any())
                    return;

                //从子词条池中随机挑选一个
                EnchantData finalData = sonPool.OrderBy(d => Guid.NewGuid()).FirstOrDefault();
                enchant.datas[finalData.whichSlot] = finalData;

                Charge(-cost);
                Vector2 position = Position.ToWorldCoordinates(24, -8);

                SoundEngine.PlaySound(CoraliteSoundID.ManaCrystal_Item29, position);
                MagikeHelper.SpawnDustOnGenerate(2, 2, Position + new Point16(0, -2), MainColor);
            }
        }

        public virtual bool CanInsertItem(Item item)
        {
            return item.damage > 0 || item.accessory || item.defense > 0;
        }

        public virtual Item GetItem()
        {
            return containsItem;
        }

        public virtual bool InsertItem(Item item)
        {
            containsItem = item;
            return true;
        }

        public bool CanGetItem()
        {
            return workTimer == -1;
        }

        public bool TryOutputItem(Func<bool> rule, out Item item)
        {
            item = containsItem;
            return true;
        }

        public override Vector2 GetWorldPosition()
        {
            return Position.ToWorldCoordinates(16, -16);
        }

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            if (!containsItem.IsAir)
            {
                tag.Add("containsItem", containsItem);
            }
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            if (tag.TryGet("containsItem", out Item item))
            {
                containsItem = item;
            }
        }

        public static int GetMagikeCost(IMagikeContainer container, Item item)
        {
            if (item.value < 1_00_00)
                return 75;
            else if (item.value < 5_00_00)
                return 150;
            else if (item.value < 10_00_00)
                return 300;
            else if (item.value < 15_00_00)
                return 450;
            else if (item.value < 20_00_00)
                return 750;
            else return 1000;
            //return item.rare switch
            //{
            //    ItemRarityID.White => 50,
            //    ItemRarityID.Blue => 100,
            //    ItemRarityID.Green => 150,
            //    ItemRarityID.Orange => 200,
            //    ItemRarityID.LightRed => 300,
            //    ItemRarityID.Pink => 400,
            //    ItemRarityID.LightPurple => 600,
            //    ItemRarityID.Lime => 800,
            //    ItemRarityID.Yellow => 1000,
            //    ItemRarityID.Cyan => 1200,
            //    ItemRarityID.Red => 1600,
            //    ItemRarityID.Purple => 2000,
            //    _ => container.MagikeMax
            //};
        }

        public static void LevelCheck(Enchant enchant, int currentSlot, out int checkedSlot)
        {
            EnchantData currentData = enchant.datas[currentSlot];
            if (currentData is null)
            {
                checkedSlot = currentSlot;
                return;
            }

            if (currentData.level == Enchant.Level.Max)
            {
                List<int> otherDataIndex =new List<int>();

                for (int i = 0; i < enchant.datas.Length; i++)
                {
                    if (enchant.datas[i] == null)
                    {
                        otherDataIndex.Add(i);
                        continue;
                    }

                    if (enchant.datas[i].whichSlot != currentData.whichSlot)
                        otherDataIndex.Add(i);
                }

                //from d in enchant.datas
                //where d.whichSlot != currentData.whichSlot
                //select d.whichSlot;

                foreach (var index in otherDataIndex)
                {
                    if (enchant.datas[index] == null)
                    {
                        checkedSlot = index;
                        return;
                    }

                    if (enchant.datas[index].level != Enchant.Level.Max)
                    {
                        checkedSlot = index;
                        return;
                    }
                }
            }

            checkedSlot = currentSlot;
        }

        /// <summary>
        /// 如果自身为0并且三个词条都是满级那就在另外两个里面随机
        /// </summary>
        /// <param name="currentSlot"></param>
        /// <param name="enchant"></param>
        /// <returns></returns>
        public static int MaxLevelCheck(int currentSlot, Enchant enchant)
        {
            if (currentSlot == 0 &&
                enchant.datas[0] != null && enchant.datas[0].level == Enchant.Level.Max &&
                enchant.datas[1] != null && enchant.datas[1].level == Enchant.Level.Max &&
                enchant.datas[2] != null && enchant.datas[2].level == Enchant.Level.Max)
            {
                return Main.rand.NextFromList(1, 1, 1, 2, 2);
            }

            return currentSlot;
        }

        public static EnchantEntityPool GetEnchantPool(Item item)
        {
            if (item.ModItem is ISpecialEnchantable special)
                return special.GetEntityPool();

            if (EnchantEntityPools.TryGetSlecialEnchantPool(item, out EnchantEntityPool pool))
                return pool;

            if (item.accessory)
                return EnchantEntityPools.accessoryPool;

            if (item.defense > 0)
                return EnchantEntityPools.armorPool;

            return EnchantEntityPools.weaponPool_Generic;
        }

        public static Enchant.Level GetLevel(Enchant.Level currentLevel)
        {
            return currentLevel switch
            {
                Enchant.Level.One => Enchant.Level.Two,
                Enchant.Level.Two => Enchant.Level.Three,
                Enchant.Level.Three => Enchant.Level.Four,
                Enchant.Level.Four => Enchant.Level.Five,
                Enchant.Level.Five => Enchant.Level.Max,
                Enchant.Level.Max => Enchant.Level.Max,
                _ => Enchant.Level.One,
            };
        }
    }
}
