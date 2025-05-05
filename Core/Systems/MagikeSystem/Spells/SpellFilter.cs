using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using Terraria.UI;

namespace Coralite.Core.Systems.MagikeSystem.Spells
{
    /// <summary>
    /// 法术滤镜<br></br>
    /// 需要有魔能工作站的功能
    /// </summary>
    public abstract class SpellFilter : MagikeFilter, ITimerTriggerComponent, IUIShowable
    {
        /// <summary> 是否正在工作中 </summary>
        public bool IsWorking { get; set; }

        /// <summary> 基础工作时间 </summary>
        public int WorkTimeBase { get => DelayBase; set => DelayBase = value; }
        /// <summary> 工作时间增幅 </summary>
        public float WorkTimeBonus { get => DelayBonus; set => DelayBonus = value; }

        /// <summary> 工作时间 </summary>
        public int WorkTime { get => Math.Clamp((int)(WorkTimeBase * WorkTimeBonus), 1, int.MaxValue); }

        public int DelayBase { get; set; } = 15;
        public float DelayBonus { get; set; } = 1f;
        public int Timer { get; set; }

        public bool TimeResetable => true;

        /// <summary>
        /// 还有需要多少魔能
        /// </summary>
        public int RequiredMagike { get; set; }
        /// <summary>
        /// 每次消耗多少魔能
        /// </summary>
        public int PerCost { get; set; }

        /// <summary>
        /// 消耗多少的百分比
        /// </summary>
        public abstract float CostPercent { get; }

        public override void Update()
        {
            if (!IsWorking)
                return;

            if (OnWorking())
                return;

            WorkFinish();
        }

        /// <summary>
        /// 获取魔能合成表
        /// </summary>
        /// <returns></returns>
        public virtual MagikeRecipe GetRecipe()
        {
            if (MagikeSystem.TryGetSpellRecipes(out var recipes))
            {
                int type = GetCraftResultItemType();
                return recipes.First(r => r.ResultItem.type == type);
            }

            throw new Exception("未找到指定的合成表！");
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        public abstract int GetCraftResultItemType();
        public abstract SpellStructure GetSpellStructure();

        public override bool CanInsert_SpecialCheck(MagikeTP entity, ref string text)
        {
            text = "";

            if (entity.HasComponent<SpellFactory>())
                return true;

            text = MagikeSystem.GetFilterText(MagikeSystem.FilterID.NotSpellCore);
            return false;
        }

        #region 工作部分

        /// <summary>
        /// 激活并开始工作
        /// </summary>
        public void StartWork()
        {
            IsWorking = true;
            Timer = WorkTime;

            MagikeRecipe recipe = GetRecipe();
            RequiredMagike = recipe.magikeCost;
            PerCost = (int)(RequiredMagike * CostPercent);
        }

        /// <summary>
        /// 能否启动
        /// </summary>
        /// <returns></returns>
        public bool CanActivated_SpecialCheck()
        {
            SpellStructure spellStructure = GetSpellStructure();

            Point16 p = GetEntityCenter();

            //检测连接
            if (!spellStructure.CheckSpell(p.ToPoint()))
                return false;

            return true;
        }

        public virtual bool UpdateTime()
        {
            Timer--;

            if (Timer <= 0)
            {
                Timer = 0;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 工作中，逐步消耗魔能
        /// </summary>
        public virtual bool OnWorking()
        {
            if (UpdateTime())
            {
                if (CostMagike())
                    return false;
                else
                {
                    Timer = WorkTime;
                    return true;
                }
            }

            return true;
        }

        /// <summary>
        /// 消耗魔能
        /// </summary>
        /// <returns>如果消耗完魔能，返回 <see cref="true"/> </returns>
        public bool CostMagike()
        {
            MagikeContainer magikeContainer = Entity.GetMagikeContainer();

            int magikeCost = PerCost;

            if (magikeCost > RequiredMagike)
                magikeCost = RequiredMagike;
            if (magikeCost > magikeContainer.Magike)
                magikeCost = magikeContainer.Magike;

            magikeContainer.ReduceMagike(magikeCost);
            RequiredMagike -= magikeCost;

            return RequiredMagike < 1;
        }

        public void WorkFinish()
        {
            IsWorking = false;

            //生成物品


        }

        public Point16 GetEntityCenter()
        {
            Point16 p = Entity.Position;
            Tile tile = Framing.GetTileSafely(p);
            TileObjectData data = TileObjectData.GetTileData(tile);

            if (data != null)
            {
                if (CoraliteSetsSystem.MagikeTileTypes.TryGetValue(tile.TileType, out var placeType)
                    && placeType != CoraliteSetsSystem.MagikeTileType.None)
                    MagikeHelper.GetMagikeAlternateData(p.X, p.Y, out data, out _);

                int x = data == null ? 1 : data.Width;
                int y = data == null ? 1 : data.Height;

                p += new Point16(x / 2, y / 2);
            }

            return p;
        }

        #endregion

        #region UI部分

        public void ShowInUI(UIElement parent)
        {
            var button = new UIImageButton(ModContent.Request<Texture2D>(AssetDirectory.DefaultItem, ReLogic.Content.AssetRequestMode.ImmediateLoad));
            button.OnLeftClick += Button_OnLeftClick;

            parent.Append(button);
        }

        private void Button_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            var structure = GetSpellStructure();
            structure.CheckStructure(GetEntityCenter().ToPoint());
        }

        #endregion

        #region 同步部分

        public override void SendData(ModPacket data)
        {
            data.Write(WorkTimeBase);
            data.Write(WorkTimeBonus);

            data.Write(IsWorking);
            data.Write(Timer);
        }

        public override void ReceiveData(BinaryReader reader, int whoAmI)
        {
            WorkTimeBase = reader.ReadInt32();
            WorkTimeBonus = reader.ReadSingle();

            IsWorking = reader.ReadBoolean();
            Timer = reader.ReadInt32();
        }

        #endregion

        #region 存储

        public override void SaveData(string preName, TagCompound tag)
        {
            tag.Add(preName + nameof(WorkTimeBase), WorkTimeBase);
            tag.Add(preName + nameof(WorkTimeBonus), WorkTimeBonus);

            if (IsWorking)
            {
                tag.Add(preName + nameof(IsWorking), true);
                tag.Add(preName + nameof(Timer), Timer);
            }
        }

        public override void LoadData(string preName, TagCompound tag)
        {
            WorkTimeBase = tag.GetInt(preName + nameof(WorkTimeBase));
            WorkTimeBonus = tag.GetFloat(preName + nameof(WorkTimeBonus));

            IsWorking = tag.ContainsKey(preName + nameof(IsWorking));
            if (IsWorking)
                Timer = tag.GetInt(preName + nameof(Timer));
        }


        #endregion
    }
}
