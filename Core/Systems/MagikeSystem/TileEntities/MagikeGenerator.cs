using Coralite.Content.UI;
using Coralite.Core.Loaders;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    /// <summary>
    /// 魔能生产器，能够存储，发送和生产魔能，如何生产请自定义
    /// </summary>
    public abstract class MagikeGenerator : MagikeSender_Line, IMagikeGenerator
    {
        public MagikeGenerator(int magikeMax, int connectLenghMax, int howManyCanConnect = 1) : base(magikeMax, connectLenghMax, howManyCanConnect)
        { }

        public event Action OnGenerated;

        public override void Update()
        {
            Generate();
            base.Update();//基类中包含了send方法
        }

        public virtual void GenerateAndChargeSelf(int howMany)
        {
            Charge(howMany);
            OnGenerated?.Invoke();
        }

        /// <summary>
        /// 如何生产魔能
        /// </summary>
        public virtual void Generate() { }

        /// <summary>
        /// 是否能生产
        /// </summary>
        /// <returns></returns>
        public abstract bool CanGenerate();

        /// <summary>
        /// 每次生产多少
        /// </summary>
        /// <returns></returns>
        public abstract int HowManyToGenerate { get; }

        /// <summary>
        /// ！重要！请在这个方法中调用Generate(add)!!!!!!
        /// </summary>
        public abstract void OnGenerate(int howMany);

    }

    public abstract class MagikeGenerator_Normal : MagikeGenerator
    {
        /// <summary> 计时器 </summary>
        public int generateTimer;
        /// <summary> 每隔多久生成一次魔能  </summary>
        public readonly int generateDelay;

        protected MagikeGenerator_Normal(int magikeMax, int connectLenghMax, int generateDelay, int howManyCanConnect = 1) : base(magikeMax, connectLenghMax, howManyCanConnect)
        {
            this.generateDelay = generateDelay;
        }

        public override void Generate()
        {
            generateTimer++;
            if (generateTimer < generateDelay) //每隔固定时间检测物品，并消耗
                return;

            generateTimer = 0;
            CheckActive();
            if (!CanGenerate())
                return;

            int add = HowManyToGenerate;    //消耗前检测一下，防止出现意外事故
            if (add <= 0 || magike + add > magikeMax)
                return;

            OnGenerate(add);
        }

        public override void CheckActive()
        {
            active = CanGenerate();
        }
    }

    /// <summary>
    /// 通过放入含有魔能的物品并消耗它来获取魔能
    /// </summary>
    public abstract class MagikeGenerator_FromMagItem : MagikeGenerator_Normal, IMagikeSingleItemContainer
    {
        /// <summary> 内部存储的物品，仅当物品魔能含量不小于0是才能消耗物品并获得魔能  </summary>
        public Item itemToCosume = new Item();

        public MagikeGenerator_FromMagItem(int magikeMax, int connectLenghMax, int generateDelay, int howManyCanConnect = 1) : base(magikeMax, connectLenghMax, generateDelay, howManyCanConnect) { }

        public override void OnGenerate(int howMany)
        {
            itemToCosume.stack--;
            if (itemToCosume.stack < 1)
                itemToCosume.TurnToAir();   //消耗物品并获得魔能

            GenerateAndChargeSelf(howMany);
        }

        public override bool CanGenerate() => !itemToCosume.IsAir;

        public override int HowManyToGenerate => itemToCosume.GetMagikeItem().magikeAmount;
        public Item ContainsItem { get => itemToCosume; set => itemToCosume = value; }


        public override void CheckActive()
        {
            active = !itemToCosume.IsAir;
        }

        public override void OnKill()
        {
            MagikeItemSlotPanel.visible = false;
            UILoader.GetUIState<MagikeItemSlotPanel>().Recalculate();

            if (!itemToCosume.IsAir)
                Item.NewItem(new EntitySource_TileEntity(this), new Rectangle(Position.X * 16, Position.Y * 16, 16, 16), itemToCosume);
        }

        public virtual bool CanInsertItem(Item item)
        {
            return item.GetMagikeItem().magikeAmount > 0;
        }

        public bool InsertItem(Item item)
        {
            itemToCosume = item;
            return true;
        }

        bool ISingleItemContainer.TryOutputItem(Func<bool> rule, out Item item)
        {
            throw new NotSupportedException();
        }

        public Item GetItem()
        {
            return itemToCosume;
        }

        public bool CanGetItem() => true;


        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            if (!itemToCosume.IsAir)
            {
                tag.Add("itemToCosume", itemToCosume);
            }
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            if (tag.TryGet("itemToCosume", out Item item))
            {
                itemToCosume = item;
            }
        }


    }
}
