using Coralite.Content.Items.CoreKeeper;
using Coralite.Content.UI;
using Coralite.Content.UI.MagikeApparatusPanel;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.UI;
using Terraria.ModLoader.UI.Elements;
using Terraria.ObjectData;
using Terraria.UI;
using static Coralite.Helpers.MagikeHelper;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public abstract class CraftAltar : MagikeFactory, IUpgradeable, IUIShowable
    {
        public MagikeRecipe ChosenResipe { get; set; }

        /// <summary>
        /// 还有需要多少魔能
        /// </summary>
        public int RequiredMagike { get; set; }
        /// <summary>
        /// 每次消耗多少魔能
        /// </summary>
        public int PerCost { get; set; }

        /// <summary>
        /// 消耗多少的百分比，根据等级提升
        /// </summary>
        public float CostPercent { get; set; }

        public ItemSpawnModes ItemSpawnMode { get; set; }

        /// <summary>
        /// 物品生成模式
        /// </summary>
        public enum ItemSpawnModes : byte
        {
            /// <summary>
            /// 放入物品容器内
            /// </summary>
            IntoSlot,
            /// <summary>
            /// 丢出来
            /// </summary>
            ThrowOut
        }

        public override void Initialize()
        {
            Upgrade(MALevel.None);
        }

        public virtual void Upgrade(MALevel incomeLevel) { }

        public virtual bool CanUpgrade(MALevel incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);

        public override void Work()
        {
            RequiredMagike = 0;
            PerCost = 0;

            if (ChosenResipe == null)
                return;

            //检测魔能量和条件是否足够
            //MagikeCraftAttempt attempt = new MagikeCraftAttempt();
            //ChosenResipe.CanCraft_CheckMagike(Entity.GetMagikeContainer().Magike, ref attempt);
            //ChosenResipe.CanCraft_CheckCondition(ref attempt);

            //if (!attempt.Success)
            //{
            //    PopupText.NewText(new AdvancedPopupRequest()
            //    {
            //        Color = Coralite.MagicCrystalPink,
            //        Text = attempt.OutputText(),
            //        DurationInFrames = 60,
            //        Velocity = -Vector2.UnitY
            //    }, Helper.GetMagikeTileCenter(Entity.Position) - (Vector2.UnitY * 32));
            //    return;
            //}

            //先减少魔能
            //Entity.GetMagikeContainer().ReduceMagike(ChosenResipe.magikeCost);

            //生成物品并放入
            switch (ItemSpawnMode)
            {
                default:
                case ItemSpawnModes.IntoSlot:
                    if (Entity.TryGetComponent(MagikeComponentID.ItemGetOnlyContainer, out GetOnlyItemContainer container))
                        container.AddItem(ChosenResipe.ResultItem.type, ChosenResipe.ResultItem.stack);
                    break;
                case ItemSpawnModes.ThrowOut:
                    {
                        MagikeTP entity1 = Entity;
                        Point16 pos1 = entity1.Position;
                        Tile t1 = Framing.GetTileSafely(pos1);
                        ModTile mt1 = TileLoader.GetTile(t1.TileType);

                        if (mt1 is BaseCraftAltarTile altartile1)
                        {
                            GetMagikeAlternateData(pos1.X, pos1.Y, out TileObjectData data, out MagikeAlternateStyle alternate);
                            float rotation = alternate.GetAlternateRotation();
                            var level = MagikeSystem.FrameToLevel(t1.TileType, t1.TileFrameX / data.CoordinateFullWidth);

                            Vector2 position = Helper.GetMagikeTileCenter(pos1.X, pos1.Y) + altartile1.GetFloatingOffset(rotation, level.Value);
                            if (!VaultUtils.isClient)
                                Item.NewItem(new EntitySource_TileUpdate(pos1.X, pos1.Y), position, ChosenResipe.ResultItem.type, ChosenResipe.ResultItem.stack);
                        }
                    }
                    break;
            }

            MagikeTP entity = Entity;
            Point16 pos = entity.Position;
            Tile t = Framing.GetTileSafely(pos);
            ModTile mt = TileLoader.GetTile(t.TileType);

            if (mt is BaseCraftAltarTile altartile)
            {
                GetMagikeAlternateData(pos.X, pos.Y, out TileObjectData data, out MagikeAlternateStyle alternate);
                float rotation = alternate.GetAlternateRotation();
                var level = MagikeSystem.FrameToLevel(t.TileType, t.TileFrameX / data.CoordinateFullWidth);

                Vector2 position = Helper.GetMagikeTileCenter(pos.X, pos.Y) + altartile.GetFloatingOffset(rotation, level.Value);

                for (int i = 0; i < 20; i++)
                {
                    Vector2 dir = (i * MathHelper.TwoPi / 20).ToRotationVector2();
                    Dust dust = Dust.NewDustPerfect(position, DustID.RainbowMk2, dir * Main.rand.NextFloat(2f, 5f)
                        , newColor: Coralite.MagicCrystalPink, Scale: 1.2f);
                    dust.noGravity = true;
                    dust = Dust.NewDustPerfect(position, DustID.RainbowMk2, dir * Main.rand.NextFloat(5f, 8f)
                        , newColor: Coralite.MagicCrystalPink, Scale: 1.5f);
                    dust.noGravity = true;
                }

                for (int i = 0; i < 5; i++)
                {
                    Dust dust = Dust.NewDustPerfect(position, ModContent.DustType<Runes>(), Helper.NextVec2Dir(3f, 5f)
                        , newColor: Coralite.MagicCrystalPink, Scale: 1f);
                    dust.noGravity = true;
                }

                Helper.PlayPitched("UI/Success", 0.4f, -0.2f, position);
            }
        }

        public override bool DuringWork()
        {
            OnWorking();

            //每隔固定时间消耗一次魔能
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

        public void SetPerCost()
        {
            PerCost = (int)(RequiredMagike * CostPercent);
            if (PerCost < 1)
                PerCost = 1;
        }

        /// <summary>
        /// 消耗主要物品
        /// </summary>
        /// <returns></returns>
        private bool WorkCheck_CostMainItem()
        {
            if (!Entity.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container))
                return false;

            int count = ChosenResipe.MainItem.stack;

            foreach (var item in container.Items)
            {
                if (item.IsAir)
                    continue;

                if (item.type != ChosenResipe.MainItem.type)
                    continue;

                int cost = Math.Min(item.stack, count);
                item.stack -= cost;

                if (item.stack < 1)
                    item.TurnToAir();

                count -= cost;
                if (count == 0)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 消耗其他物品
        /// </summary>
        /// <returns></returns>
        private bool WorkCheck_CostOtherItem()
        {
            //获取物品容器
            if (!Entity.TryGetComponent(MagikeComponentID.MagikeSender, out MagikeLinerSender linerSender))
                return false;

            //List<Item> otherItems = FillOtherItemList(linerSender);

            //挨个消耗物品
            foreach (var item in ChosenResipe.RequiredItems)
            {
                int howManyNeed = item.stack;

                foreach (var pos in linerSender.Receivers)
                {
                    if (!TryGetEntityWithTopLeft(pos, out MagikeTP receiverEntity))
                        continue;

                    Item[] tempItems = null;

                    if (receiverEntity.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container2))
                        tempItems = container2.Items;

                    if (receiverEntity.TryGetComponent(MagikeComponentID.ItemGetOnlyContainer, out GetOnlyItemContainer container3))
                        tempItems = container3.Items;

                    foreach (var tempitem in tempItems)
                        if (!tempitem.IsAir && tempitem.type == item.type)
                        {
                            int cost = Math.Min(tempitem.stack, howManyNeed);
                            tempitem.stack -= cost;

                            if (tempitem.stack < 1)
                                tempitem.TurnToAir();

                            howManyNeed -= cost;
                            if (howManyNeed < 1)
                                goto costEnd;
                        }
                }

                if (howManyNeed > 0)
                    return false;//这块给我写迷糊了，总之就是遍历所有的物品然后挨个消耗

                costEnd:
                ;
            }

            return true;
        }

        /// <summary>
        /// 获取连接的所有物品容器内的物品
        /// </summary>
        /// <param name="linerSender"></param>
        /// <returns></returns>
        private static List<Item> FillOtherItemList(MagikeLinerSender linerSender)
        {
            List<Item> otherItems = [];

            foreach (var pos in linerSender.Receivers)
            {
                if (!MagikeHelper.TryGetEntityWithTopLeft(pos, out MagikeTP entity))
                    continue;

                Item[] tempItems = null;

                if (entity.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container2))
                    tempItems = container2.Items;

                if (entity.TryGetComponent(MagikeComponentID.ItemGetOnlyContainer, out GetOnlyItemContainer container3))
                    tempItems = container3.Items;

                if (tempItems == null)
                    continue;

                //添加物品
                foreach (var tempitem in tempItems)
                    if (!tempitem.IsAir)
                        otherItems.Add(tempitem);
            }

            return otherItems;
        }

        /// <summary>
        /// STOP！
        /// </summary>
        public void StopWork()
        {
            IsWorking = false;
            Timer = 0;
        }

        #region 检测能否开始工作

        public override bool CanActivated_SpecialCheck(out string text)
        {
            text = "";

            //获取物品容器
            if (!GetItems(out Item[] items, out Dictionary<int, int> otherItems))
            {
                text = MagikeSystem.Error.Value;
                return false;
            }

            FrozenDictionary<int, int> otherItems2 = otherItems.ToFrozenDictionary();

            if (ChosenResipe != null)//快速检测主要物品以防止主要物品错误而需要删掉合成表
            {
                bool hasCorrectMainItem = false;
                foreach (var item in items)
                    if (ChosenResipe.MainItem.type == item.type)
                    {
                        hasCorrectMainItem = true;
                        break;
                    }

                if (!hasCorrectMainItem)
                    ChosenResipe = null;
            }

            if (ChosenResipe == null)
            {
                if (!CheckCanCraft_FindRecipe(items, otherItems2, out text))//寻找合成表
                    return false;
            }

            //检测物品是否能够合成
            if (!ChosenResipe.CanCraft(items, otherItems2, Entity.GetMagikeContainer().Magike, out text))
                return false;

            if (ChosenResipe.RequiredItems != null)//消耗次要物品
                if (!WorkCheck_CostOtherItem())
                    return false;

            if (!WorkCheck_CostMainItem())//消耗主要物品
                return false;

            return true;
        }

        /// <summary>
        /// 获取所有的物品
        /// </summary>
        /// <param name="mainItems"></param>
        /// <param name="otherItems"></param>
        /// <returns></returns>
        public bool GetItems(out Item[] mainItems, out Dictionary<int, int> otherItems)
        {
            mainItems = null;
            otherItems = null;

            if (!Entity.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container)
                || !Entity.TryGetComponent(MagikeComponentID.MagikeSender, out MagikeLinerSender linerSender))
            {
                return false;
            }

            mainItems = container.Items;
            otherItems = FillOtherItemDict(linerSender);

            return true;
        }

        /// <summary>
        /// 获取连接的所有物品容器内的物品
        /// </summary>
        /// <param name="linerSender"></param>
        /// <returns></returns>
        public static Dictionary<int, int> FillOtherItemDict(MagikeLinerSender linerSender)
        {
            Dictionary<int, int> otherItems = [];

            foreach (var pos in linerSender.Receivers)
            {
                if (!MagikeHelper.TryGetEntityWithTopLeft(pos, out MagikeTP entity))
                    continue;

                Item[] tempItems = null;

                if (entity.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container2))
                    tempItems = container2.Items;

                if (entity.TryGetComponent(MagikeComponentID.ItemGetOnlyContainer, out GetOnlyItemContainer container3))
                    tempItems = container3.Items;

                if (tempItems == null)
                    continue;

                //添加物品
                foreach (var tempitem in tempItems)
                    if (!tempitem.IsAir)
                    {
                        if (otherItems.TryGetValue(tempitem.type, out int current))
                            otherItems[tempitem.type] = tempitem.stack + current;
                        else
                            otherItems.Add(tempitem.type, tempitem.stack);
                    }
            }

            return otherItems;
        }

        /// <summary>
        /// 寻找当前大概能合成的合成表，仅检测物品类型，不检测物品数量和魔能等
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private bool CheckCanCraft_FindRecipe(Item[] mainItems, IDictionary<int, int> otherItems, out string text)
        {
            text = "";
            List<MagikeRecipe> remodelRecipes = [];
            MagikeRecipe polymerizeRecipe = null;
            float matchPercent = 0;

            foreach (Item mainItem in mainItems)
            {
                if (mainItem.IsAir || !MagikeSystem.TryGetMagikeCraftRecipes(mainItem.type, out List<MagikeRecipe> recipes))
                    continue;

                foreach (var recipe in recipes)
                {
                    if (recipe.recipeType != MagikeRecipe.RecipeType.MagikeCraft)//必须得是魔能合成的合成表
                        continue;

                    if (recipe.RequiredItems == null || recipe.RequiredItems.Count == 0)
                    {
                        remodelRecipes.Add(recipe);
                        continue;
                    }

                    int matchCount = 0;
                    int all = recipe.RequiredItems.Count;

                    foreach (var requiredItem in recipe.RequiredItems)//检测合成表中的物品与当前物品的匹配程度
                        if (otherItems.ContainsKey(requiredItem.type))
                            matchCount++;

                    if (matchCount / (float)all >= matchPercent)//如果匹配程度高于当前的就替换当前的
                        polymerizeRecipe = recipe;
                }
            }

            if (remodelRecipes.Count > 0)
            {
                ChosenResipe = remodelRecipes.MinBy(r => r.magikeCost);
                if (ChosenResipe != null)
                    return true;
            }

            if (polymerizeRecipe != null)
            {
                ChosenResipe = polymerizeRecipe;
                return true;
            }

            text = MagikeSystem.GetCraftText(MagikeSystem.CraftTextID.NoCraftRecipe);
            return false;
        }

        #endregion

        #region 开始工作部分

        public override void StarkWork()
        {
            base.StarkWork();

            RequiredMagike = ChosenResipe.magikeCost;
            SetPerCost();
        }

        #endregion

        #region UI部分

        public void ShowInUI(UIElement parent)
        {
            //添加显示在最上面的组件名称
            UIElement title = this.AddTitle(MagikeSystem.UITextID.CraftAltarName, parent);

            float top = title.Height.Pixels + 8;

            //显示所有物品格子与选择的合成表
            if (!Entity.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container))
                return;

            if (!Entity.TryGetComponent(MagikeComponentID.ItemGetOnlyContainer, out GetOnlyItemContainer getOnlyContainer))
                return;


            float left = 58f;

            AddContainerList(parent, container, getOnlyContainer, left, top);//添加左侧的物品格

            left += 8;

            AddButtons(parent, this, left, ref top); //切换显示样式的按钮
            AddController(parent, this, left, ref top);//控制器
            AddRecipeShow(parent, left, top);//添加合成条
        }

        public void AddContainerList(UIElement parent, ItemContainer container, GetOnlyItemContainer getOnlyContainer, float left, float top)
        {
            UIList list = [];

            for (int i = 0; i < container.Items.Length; i++)
            {
                ItemContainerSlot slot = new(container, i);
                slot.Width.Set(0, 1);
                list.Add(slot);
            }

            list.Add(new CraftArrow(this));

            for (int i = 0; i < getOnlyContainer.Items.Length; i++)
            {
                GetOnlyItemSlot slot = new(getOnlyContainer, i);
                slot.Width.Set(0, 1);
                list.Add(slot);
            }

            list.QuickInvisibleScrollbar();
            list.SetTopLeft(top, 4);
            list.SetSize(left, -top, 0, 1);

            parent.Append(list);
        }

        public static void AddButtons(UIElement parent, CraftAltar altar, float left, ref float top)
        {
            //CraftSelectButton selectButton = new();
            CraftShowButton showButton = new CraftShowButton();
            CraftItemSpawnButton itemSpawnButton = new CraftItemSpawnButton(altar);
            CraftShowRecipeButton showRecipeButton = new CraftShowRecipeButton();

            //selectButton.SetTopLeft(left, top);
            showButton.SetTopLeft( /*+ selectButton.Width.Pixels*/ top, left);
            itemSpawnButton.SetTopLeft(top, showButton.Width.Pixels + left);
            showRecipeButton.SetTopLeft(top, showButton.Width.Pixels + itemSpawnButton.Width.Pixels + left);
            //parent.Append(selectButton);
            parent.Append(showButton);
            parent.Append(itemSpawnButton);
            parent.Append(showRecipeButton);

            top += showButton.Height.Pixels;
        }

        public static void AddController(UIElement parent, CraftAltar altar, float left, ref float top)
        {
            CraftController controller = new CraftController(altar);
            controller.SetSize(-left, 12, 1, 0);
            controller.SetTopLeft(top, left);

            CraftController.Reset();
            parent.Append(controller);

            top += controller.Height.Pixels;
        }

        public static void AddRecipeShow(UIElement parent, float left, float top)
        {
            switch (CraftShowButton.CurrentShowStyle)
            {
                case CraftShowButton.ShowStyle.VerticleLine:
                    {
                        UIList list = new UIList();
                        list.SetTopLeft(top, left);
                        list.SetSize(-left, -top, 1, 1);
                        list.QuickInvisibleScrollbar();

                        foreach (var recipe in CraftController.Recipes)
                            list.Add(new CraftBar(recipe));

                        parent.Append(list);
                    }
                    break;
                default:
                case CraftShowButton.ShowStyle.Grid:
                    {
                        FixedUIGrid grid = new FixedUIGrid();
                        grid.SetTopLeft(top, left);
                        grid.SetSize(-left - 20, -top, 1, 1);
                        grid.QuickInvisibleScrollbar();

                        foreach (var recipe in CraftController.Recipes)
                        {
                            var slot = new CraftSlot(recipe, CraftSlot.SlotType.ResultItem);
                            slot.SetSize(46, 46);
                            grid.Add(slot);
                        }

                        parent.Append(grid);
                    }
                    break;
            }
        }

        #endregion

        #region 绘制

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsWorking || ChosenResipe == null)
                return;

            MagikeTP entity = Entity;
            Point16 pos = entity.Position;
            Tile t = Framing.GetTileSafely(pos);
            ModTile mt = TileLoader.GetTile(t.TileType);

            if (mt is BaseCraftAltarTile altartile)
            {
                GetMagikeAlternateData(pos.X, pos.Y, out TileObjectData data, out MagikeAlternateStyle alternate);

                float rotation = alternate.GetAlternateRotation();
                var level = MagikeSystem.FrameToLevel(t.TileType, t.TileFrameX / data.CoordinateFullWidth);

                if (!level.HasValue)
                    return;

                Vector2 position = Helper.GetMagikeTileCenter(pos.X, pos.Y) + altartile.GetFloatingOffset(rotation, level.Value);
                Texture2D mainTex = CoraliteAssets.Halo.CircleSPA.Value;
                float factor = Coralite.Instance.BezierEaseSmoother.Smoother((float)RequiredMagike / ChosenResipe.magikeCost);
                float Length = 12 + factor * 44;
                float alpha = 1;
                if (factor < 0.1f)
                    alpha = Helper.Lerp(0, 1, factor / 0.1f);
                else if (factor > 0.7f)
                    alpha = Helper.Lerp(1, 0, (factor - 0.7f) / 0.3f);

                if (Timer % 3 == 0 && Main.rand.NextBool())
                {
                    Vector2 dir = Helper.NextVec2Dir();
                    Dust dust = Dust.NewDustPerfect(position - dir * 26, DustID.RainbowMk2, dir * Main.rand.NextFloat(1.5f, 3f)
                        , newColor: Coralite.MagicCrystalPink, Scale: 0.8f);
                    dust.noGravity = true;
                }

                position = position - Main.screenPosition;
                Color c = Coralite.MagicCrystalPink;
                if (Entity.TryGetComponent(MagikeComponentID.ApparatusInformation, out ApparatusInformation info))
                    c = MagikeSystem.GetColor(info.CurrentLevel);

                c *= (200f / 255f * alpha);
                var origin = mainTex.Size() / 2;
                float scale = Length * 2 / mainTex.Width;
                scale *= 1f;

                spriteBatch.Draw(mainTex, position, null, c, 0, origin, scale, 0, 0);

                Texture2D tex2 = CoraliteAssets.Halo.RuneSPA.Value;

                origin = tex2.Size() / 2;
                scale = Length * 2 / tex2.Width;
                scale *= 1.1f;
                spriteBatch.Draw(tex2, position, null, c, Main.GlobalTimeWrappedHourly, origin, scale, 0, 0);

                //c = Color.White;
                //c.A = (byte)(255 * alpha);

                spriteBatch.Draw(tex2, position, null, c, Main.GlobalTimeWrappedHourly, origin, scale, 0, 0);

                int total = ChosenResipe.RequiredItems.Count;
                c = Color.White * alpha;

                for (int i = 0; i < total; i++)
                {
                    float rot = Main.GlobalTimeWrappedHourly + (float)i / total * MathHelper.TwoPi;
                    DrawItem(spriteBatch, ChosenResipe.RequiredItems[i], position + rot.ToRotationVector2() * Length * 0.9f, 48, c);
                }
            }
        }

        #endregion

        public override void SaveData(string preName, TagCompound tag)
        {
            base.SaveData(preName, tag);

            tag.Add(nameof(CostPercent), CostPercent);

            ChosenResipe?.Save(tag);

            if (IsWorking)
                tag.Add(nameof(RequiredMagike), RequiredMagike);

            if (ItemSpawnMode != ItemSpawnModes.IntoSlot)
                tag.Add(nameof(ItemSpawnMode), (byte)ItemSpawnMode);
        }

        public override void LoadData(string preName, TagCompound tag)
        {
            base.LoadData(preName, tag);

            CostPercent = tag.GetFloat(nameof(CostPercent));
            ChosenResipe = MagikeRecipe.Load(tag);

            if (IsWorking)
            {
                RequiredMagike = tag.GetInt(nameof(RequiredMagike));
                SetPerCost();
            }

            if (tag.TryGet(nameof(ItemSpawnMode), out byte m))
                ItemSpawnMode = (ItemSpawnModes)m;
        }
    }

    public class CraftArrow : UIElement
    {
        private CraftAltar _altar;
        private static Item _voidItem = new();
        private bool canCraft;
        private string FailText;

        public CraftArrow(CraftAltar altar)
        {
            Texture2D tex = MagikeSystem.CraftArrow.Value;
            Vector2 size = tex.Frame(1, 2).Size();

            Width.Set(size.X + 10, 0);
            Height.Set(size.Y, 0);

            _altar = altar;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_altar != null && _altar.ChosenResipe != null
                && _altar.GetItems(out Item[] items, out Dictionary<int, int> otherItems))
            {
                canCraft = _altar.ChosenResipe.CanCraft(items, otherItems, _altar.Entity.GetMagikeContainer().Magike, out FailText);
            }
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            Helper.PlayPitched("UI/Tick", 0.4f, 0);
            FailText = "";

            if (!_altar.IsWorking)
                _altar.ChosenResipe = null;
        }

        public override void RightClick(UIMouseEvent evt)
        {
            base.RightClick(evt);

            if (_altar.IsWorking)
            {
                Helper.PlayPitched("UI/Tick", 0.4f, 0);
                _altar.StopWork();
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D tex = MagikeSystem.CraftArrow.Value;

            CalculatedStyle Dimensions = GetDimensions();

            Rectangle frame = tex.Frame(1, 2);

            var pos = Dimensions.Position();
            var center = Dimensions.Center();

            spriteBatch.Draw(tex, center, frame, Color.White, 0, frame.Size() / 2, 1, 0, 0);


            if (_altar.IsWorking)//工作中就只显示百分比，不然就显示合成表
            {
                float percent = 1 - (float)_altar.RequiredMagike / _altar.ChosenResipe.magikeCost;
                string percentText = MathF.Round(100 * percent, 1) + "%";

                frame = new Rectangle(0, tex.Height / 2, tex.Width, (int)(tex.Height / 2 * percent));

                spriteBatch.Draw(tex, pos + new Vector2(3, 0), frame, Color.White, 0, Vector2.Zero, 1, 0, 0);

                Utils.DrawBorderString(spriteBatch, percentText, center, Color.White, 0.75f, anchorx: 0.5f, anchory: 0.5f);
                if (IsMouseHovering)
                {
                    string text = _altar.ChosenResipe.ResultItem.Name;
                    text = string.Concat(text, Environment.NewLine, _altar.ChosenResipe.magikeCost - _altar.RequiredMagike, "/", _altar.ChosenResipe.magikeCost, Environment.NewLine, MagikeSystem.RightClickStopCraft.Value);
                    UICommon.TooltipMouseText(text);
                }
            }
            else
            {
                Item i = _voidItem;
                if (_altar.ChosenResipe != null)
                {
                    i = _altar.ChosenResipe.ResultItem;
                }

                if (IsMouseHovering)
                {
                    string text = "";

                    if (canCraft && !i.IsAir)
                    {
                        Main.LocalPlayer.mouseInterface = true;
                        ItemSlot.OverrideHover(ref i, ItemSlot.Context.InventoryItem);
                        ItemSlot.MouseHover(ref i, ItemSlot.Context.InventoryItem);

                        text = _altar.ChosenResipe.ResultItem.Name;
                        text = string.Concat(text, Environment.NewLine, _altar.Entity.GetMagikeContainer().Magike, "/", _altar.ChosenResipe.magikeCost, Environment.NewLine, MagikeSystem.RightClickRemoveRecipe.Value);
                    }
                    else
                    {
                        text = FailText;
                    }

                    if (!string.IsNullOrEmpty(text))
                        UICommon.TooltipMouseText(text);
                }

                float scale = Main.inventoryScale;
                Main.inventoryScale = 1f;

                Vector2 position = center + new Vector2(-52 / 2, -40) * Main.inventoryScale;
                ItemSlot.Draw(spriteBatch, ref i, ItemSlot.Context.ShopItem, position, Color.White);
                Main.inventoryScale = scale;
            }

        }
    }

    public class CraftMaagikeBar : UIElement
    {
        private int magikeCount;

        public CraftMaagikeBar(MagikeRecipe recipe)
        {
            Texture2D tex = MagikeSystem.CraftMagikeBar.Value;
            Vector2 size = tex.Frame(2, 1).Size();

            this.SetSize(size.X + 8, 0, 0, 1);

            magikeCount = recipe.magikeCost;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D tex = MagikeSystem.CraftMagikeBar.Value;

            var frameBox = tex.Frame(2, 1);

            var dimensions = GetDimensions();
            Vector2 pos = dimensions.Position() + new Vector2((dimensions.Width - frameBox.Width) / 2, 0);
            Vector2 center = dimensions.Center();

            //绘制底层
            spriteBatch.Draw(tex, pos, frameBox, Color.White);
            float magike = CraftController.altar.Entity.GetMagikeContainer().Magike;
            float percent = Math.Clamp(magike / magikeCount, 0, 1);

            if (IsMouseHovering)
            {
                if (magike >= magikeCount)
                    UICommon.TooltipMouseText(MagikeSystem.CraftText[(int)MagikeSystem.CraftTextID.MagikeEnough].Format(magikeCount));
                else
                    UICommon.TooltipMouseText(MagikeSystem.CraftText[(int)MagikeSystem.CraftTextID.MagikeNotEnough].Format(magike, magikeCount));
            }

            Vector2 drawPos = pos + new Vector2(0, frameBox.Height);
            for (int i = 0; i < frameBox.Width / 2; i++)
            {
                int currentHeight = Math.Clamp(
                   (int)(tex.Height * (percent + (0.04f * MathF.Sin((((float)Main.timeForVisualEffects) * 0.1f) + (i * 0.3f)))))
                    , 0, tex.Height);

                Rectangle frameBox2 = new(frameBox.Width + (i * 2), tex.Height - currentHeight, 2, currentHeight);
                var origin = new Vector2(0, frameBox2.Height);
                spriteBatch.Draw(tex, drawPos + new Vector2(i * 2, 0), frameBox2, Color.White, 0, origin, 1f, 0, 0f);
            }

            string percentText = MathF.Round(100 * percent, 1) + "%";

            Utils.DrawBorderString(spriteBatch, percentText, center, Color.White, 0.75f, anchorx: 0.5f, anchory: 0.5f);
        }
    }

    //public class CraftSelectButton : UIElement
    //{
    //    public static SelectStyle CurrentSelectStyle;

    //    private float _scale = 1f;

    //    public CraftSelectButton()
    //    {
    //        Texture2D mainTex = MagikeSystem.CraftSelectButton.Value;

    //        var frameBox = mainTex.Frame(2, 1);
    //        this.SetSize(frameBox.Width+6, frameBox.Height+6);
    //    }

    //    /// <summary>
    //    /// 魔能合成UI的显示合成表的筛选
    //    /// </summary>
    //    public enum SelectStyle
    //    {
    //        All,
    //        CanCraft
    //    }

    //    public override void MouseOver(UIMouseEvent evt)
    //    {
    //        base.MouseOver(evt);
    //        Helper.PlayPitched("Fairy/FairyBottleClick", 0.3f, 0.4f);
    //    }

    //    public override void LeftClick(UIMouseEvent evt)
    //    {
    //        base.LeftClick(evt);

    //        CurrentSelectStyle = CurrentSelectStyle switch
    //        {
    //            SelectStyle.CanCraft => SelectStyle.All,
    //            _ => SelectStyle.CanCraft
    //        };

    //        Helper.PlayPitched("UI/Tick", 0.4f, 0);
    //        UILoader.GetUIState<MagikeApparatusPanel>().ComponentPanel.Recalculate();
    //    }

    //    protected override void DrawSelf(SpriteBatch spriteBatch)
    //    {
    //        Texture2D mainTex = MagikeSystem.CraftSelectButton.Value;

    //        var dimensions = GetDimensions();

    //        if (IsMouseHovering)
    //        {
    //            _scale = Helper.Lerp(_scale, 1.2f, 0.2f);
    //        }
    //        else
    //            _scale = Helper.Lerp(_scale, 1f, 0.2f);

    //        var framebox = mainTex.Frame(2, 1, (int)CurrentSelectStyle);
    //        spriteBatch.Draw(mainTex, dimensions.Center(), framebox, Color.White, 0, framebox.Size() / 2,_scale, 0, 0);
    //    }
    //}

    public class CraftShowButton : UIElement
    {
        public static ShowStyle CurrentShowStyle;

        private float _scale = 1f;

        public CraftShowButton()
        {
            Texture2D mainTex = MagikeSystem.CraftShowButton.Value;

            var frameBox = mainTex.Frame(2, 1);
            this.SetSize(frameBox.Width + 6, frameBox.Height + 6);
        }

        /// <summary>
        /// 魔能合成表UI的显示方式
        /// </summary>
        public enum ShowStyle
        {
            VerticleLine,
            Grid
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            Helper.PlayPitched("Fairy/FairyBottleClick", 0.3f, 0.4f);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            CurrentShowStyle = CurrentShowStyle switch
            {
                ShowStyle.VerticleLine => ShowStyle.Grid,
                _ => ShowStyle.VerticleLine
            };

            Helper.PlayPitched("UI/Tick", 0.4f, 0);
            UILoader.GetUIState<MagikeApparatusPanel>().ResetComponentPanel();
            UILoader.GetUIState<MagikeApparatusPanel>().RecalculateChildren();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = MagikeSystem.CraftShowButton.Value;
            var dimensions = GetDimensions();

            if (IsMouseHovering)
            {
                _scale = Helper.Lerp(_scale, 1.2f, 0.2f);

                string text = CurrentShowStyle switch
                {
                    ShowStyle.VerticleLine => MagikeSystem.GetUIText(MagikeSystem.UITextID.CraftAltarBarMode),
                    _ => MagikeSystem.GetUIText(MagikeSystem.UITextID.CraftAltarSlotMode),
                };

                UICommon.TooltipMouseText(text);
            }
            else
                _scale = Helper.Lerp(_scale, 1f, 0.2f);

            var framebox = mainTex.Frame(2, 1, (int)CurrentShowStyle);
            spriteBatch.Draw(mainTex, dimensions.Center(), framebox, Color.White, 0, framebox.Size() / 2, _scale, 0, 0);
        }
    }

    public class CraftItemSpawnButton : UIElement
    {
        private float _scale = 1f;
        private CraftAltar _altar;

        public CraftItemSpawnButton(CraftAltar altar)
        {
            Texture2D mainTex = MagikeSystem.CraftItemSpawnButton.Value;

            var frameBox = mainTex.Frame(2, 1);
            this.SetSize(frameBox.Width + 6, frameBox.Height + 6);

            _altar = altar;
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            Helper.PlayPitched("Fairy/FairyBottleClick", 0.3f, 0.4f);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            _altar.ItemSpawnMode++;
            if (_altar.ItemSpawnMode > CraftAltar.ItemSpawnModes.ThrowOut)
                _altar.ItemSpawnMode = CraftAltar.ItemSpawnModes.IntoSlot;

            Helper.PlayPitched("UI/Tick", 0.4f, 0);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = MagikeSystem.CraftItemSpawnButton.Value;
            var dimensions = GetDimensions();

            if (IsMouseHovering)
            {
                _scale = Helper.Lerp(_scale, 1.2f, 0.2f);

                string text = _altar.ItemSpawnMode switch
                {
                    CraftAltar.ItemSpawnModes.IntoSlot => MagikeSystem.GetUIText(MagikeSystem.UITextID.CraftAltarIntoSlot),
                    _ => MagikeSystem.GetUIText(MagikeSystem.UITextID.CraftAltarThrowOut),
                };

                UICommon.TooltipMouseText(text);
            }
            else
                _scale = Helper.Lerp(_scale, 1f, 0.2f);

            var framebox = mainTex.Frame(2, 1, (int)_altar.ItemSpawnMode);
            spriteBatch.Draw(mainTex, dimensions.Center(), framebox, Color.White, 0, framebox.Size() / 2, _scale, 0, 0);
        }
    }

    public class CraftShowRecipeButton : UIElement
    {
        private float _scale = 1f;

        public static ShowRecipeType CurrentShowRecipeType = ShowRecipeType.ShowAll;

        public enum ShowRecipeType
        {
            OnlyCanCraft,
            OnlyCantCraft,
            ShowAll
        }

        public CraftShowRecipeButton()
        {
            Texture2D mainTex = MagikeSystem.CraftShowRecipeButton.Value;

            var frameBox = mainTex.Frame(3, 1);
            this.SetSize(frameBox.Width + 6, frameBox.Height + 6);
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            Helper.PlayPitched("Fairy/FairyBottleClick", 0.3f, 0.4f);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            CurrentShowRecipeType++;
            if (CurrentShowRecipeType > ShowRecipeType.ShowAll)
                CurrentShowRecipeType = ShowRecipeType.OnlyCanCraft;

            Helper.PlayPitched("UI/Tick", 0.4f, 0);
            UILoader.GetUIState<MagikeApparatusPanel>().ResetComponentPanel();
            UILoader.GetUIState<MagikeApparatusPanel>().RecalculateChildren();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = MagikeSystem.CraftShowRecipeButton.Value;
            var dimensions = GetDimensions();

            if (IsMouseHovering)
            {
                _scale = Helper.Lerp(_scale, 1.2f, 0.2f);

                string text = CurrentShowRecipeType switch
                {
                    ShowRecipeType.OnlyCanCraft => MagikeSystem.GetUIText(MagikeSystem.UITextID.CraftAltarShowOnlyCanCraft),
                    ShowRecipeType.OnlyCantCraft => MagikeSystem.GetUIText(MagikeSystem.UITextID.CraftAltarShowOnlyCantCraft),
                    _ => MagikeSystem.GetUIText(MagikeSystem.UITextID.CraftAltarShowAll),
                };

                UICommon.TooltipMouseText(text);
            }
            else
                _scale = Helper.Lerp(_scale, 1f, 0.2f);

            var framebox = mainTex.Frame(3, 1, (int)CurrentShowRecipeType);
            spriteBatch.Draw(mainTex, dimensions.Center(), framebox, Color.White, 0, framebox.Size() / 2, _scale, 0, 0);
        }
    }

    /// <summary>
    /// 魔能合成UI的中控类，兼任绘制水平横条的任务
    /// </summary>
    public class CraftController : UIElement
    {
        /// <summary>
        /// 当前显示的所有合成表
        /// </summary>
        public static List<MagikeRecipe> Recipes = [];
        /// <summary>
        /// 当前的主要物品类型
        /// </summary>
        public static List<int> CurrentItemTypes = [];

        public static Dictionary<int, int> OtherItemTypes;

        public static CraftAltar altar;

        public CraftController(CraftAltar altar)
        {
            CraftController.altar = altar;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!altar.Entity.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container)
                || !altar.Entity.TryGetComponent(MagikeComponentID.MagikeSender, out MagikeLinerSender sender))
            {
                return;
            }

            Item[] items = container.Items;

            List<int> record = [];

            foreach (var item in items)
            {
                if (!record.Contains(item.type))
                    record.Add(item.type);

                if (CurrentItemTypes.Contains(item.type))
                    continue;

                MagikeApparatusPanel.ShouldResetComponentPanel = true;
                return;
            }

            if (record.Count != CurrentItemTypes.Count)
                MagikeApparatusPanel.ShouldResetComponentPanel = true;

            OtherItemTypes = CraftAltar.FillOtherItemDict(sender);
        }

        /// <summary>
        /// 刷新现有的合成表
        /// </summary>
        public static void Reset()
        {
            if (!altar.Entity.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container))
                return;
            if (altar == null || !altar.GetItems(out Item[] items, out Dictionary<int, int> otherItems))
                return;

            Recipes.Clear();
            CurrentItemTypes.Clear();

            int magike = altar.Entity.GetMagikeContainer().Magike;

            foreach (var item in container.Items)
            {
                if (!CurrentItemTypes.Contains(item.type))
                    CurrentItemTypes.Add(item.type);

                if (MagikeSystem.TryGetMagikeCraftRecipes(item.type, out List<MagikeRecipe> recipes))
                    foreach (var recipe in recipes)
                    {
                        if (recipe.recipeType != MagikeRecipe.RecipeType.MagikeCraft)//必须得是魔能合成的合成表
                            continue;

                        switch (CraftShowRecipeButton.CurrentShowRecipeType)
                        {
                            case CraftShowRecipeButton.ShowRecipeType.OnlyCanCraft:
                                if (recipe.CanCraftJustCheck(items, otherItems, magike))
                                    Recipes.Add(recipe);
                                break;
                            case CraftShowRecipeButton.ShowRecipeType.OnlyCantCraft:
                                if (!recipe.CanCraftJustCheck(items, otherItems, magike))
                                    Recipes.Add(recipe);
                                break;
                            default:
                            case CraftShowRecipeButton.ShowRecipeType.ShowAll:
                                Recipes.Add(recipe);
                                break;
                        }
                    }
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D line = TextureAssets.FishingLine.Value;

            var style = GetDimensions();
            Rectangle frame = new(0, 0, line.Width, (int)style.Width);

            spriteBatch.Draw(line, style.Center(), frame, Color.White, 1.57f, frame.Size() / 2, 1, 0, 0);
        }
    }

    //public class CanCraftShow : UIElement
    //{
    //    private MagikeCraftRecipe recipe;
    //    private bool canCraft;
    //    private string FailText;

    //    public CanCraftShow(MagikeCraftRecipe recipe)
    //    {
    //        this.recipe = recipe;

    //        this.SetSize(44, 52);
    //    }

    //    public override void Update(GameTime gameTime)
    //    {
    //        base.Update(gameTime);

    //        if (CraftController.altar != null
    //            && CraftController.altar.GetItems(out Item[] items, out Dictionary<int, int> otherItems))
    //        {
    //            canCraft = recipe.CanCraft(items, otherItems, CraftController.altar.Entity.GetMagikeContainer().Magike, out FailText);
    //        }
    //    }

    //    protected override void DrawSelf(SpriteBatch spriteBatch)
    //    {
    //        Texture2D mainTex = MagikeSystem.CanCraftShow.Value;

    //        float scale = 1;

    //        if (IsMouseHovering)
    //        {
    //            scale = 1.2f;
    //            if (canCraft)//显示鼠标信息
    //            {

    //            }
    //            else
    //            {

    //            }
    //        }

    //        Vector2 center = GetDimensions().Center();
    //        var frameBox = mainTex.Frame(2, 1, canCraft ? 0 : 1);
    //        spriteBatch.Draw(mainTex, center, frameBox, Color.White, 0, frameBox.Size() / 2, scale, 0, 0);
    //    }
    //}

    /// <summary>
    /// 魔能合成表的条形界面
    /// </summary>
    public class CraftBar : UIElement
    {
        private readonly UIGrid grid = new();

        public CraftBar(MagikeRecipe recipe)
        {
            SetPadding(6);

            this.SetSize(-4, 66, 1);

            grid.SetSize(1000, 0, 0, 1);

            var slot = new CraftSlot(recipe, CraftSlot.SlotType.ResultItem);
            slot.SetSize(46, 0, 0, 1);
            grid.Add(slot);
            //grid.Add(new CraftMaagikeBar(recipe));
            grid.Add(new UIVerticalLine());

            if (recipe.RequiredItems.Count > 0)
                for (int i = 0; i < recipe.RequiredItems.Count; i++)
                {
                    slot = new CraftSlot(recipe, CraftSlot.SlotType.RequiredItem, i);
                    slot.SetSize(46, 0, 0, 1);
                    grid.Add(slot);
                }

            Append(grid);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            var d = GetDimensions();
            var p = d.Position();
            Texture2D mainTex = MagikeSystem.AlphaBar.Value;

            var target = new Rectangle((int)p.X, (int)p.Y, (int)d.Width, (int)d.Height);
            var self = mainTex.Frame();

            spriteBatch.Draw(mainTex, target, self, MagikeApparatusPanel.BackgroundColor);
        }
    }

    public class CraftSlot : UIElement
    {
        private readonly Item showItem;
        private readonly MagikeRecipe recipe;
        private readonly SlotType slotType;

        private bool canCraft;
        private string FailText;
        private float _scale;

        public enum SlotType
        {
            ResultItem,
            RequiredItem
        }

        public CraftSlot(MagikeRecipe recipe, SlotType slotType, int requiredIndex = 0)
        {
            this.recipe = recipe;
            this.slotType = slotType;

            showItem = slotType switch
            {
                SlotType.RequiredItem => recipe.RequiredItems[requiredIndex].Clone(),
                _ => recipe.ResultItem.Clone(),
            };
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            switch (slotType)
            {
                case SlotType.ResultItem://对于最后的物品直接计算是否能合成
                    if (CraftController.altar != null
                        && CraftController.altar.GetItems(out Item[] items, out Dictionary<int, int> otherItems))
                    {
                        canCraft = recipe.CanCraft(items, otherItems, CraftController.altar.Entity.GetMagikeContainer().Magike, out FailText);
                    }
                    break;
                case SlotType.RequiredItem://其他物品只需要判断一下有没有足够的就行
                    if (CraftController.OtherItemTypes.TryGetValue(showItem.type, out int stack))
                    {
                        canCraft = stack >= showItem.stack;
                        if (!canCraft)
                            FailText = MagikeSystem.CraftText[(int)MagikeSystem.CraftTextID.OtherItemNotEnough].Format(showItem.Name, showItem.stack - stack);
                    }
                    else
                    {
                        canCraft = false;
                        FailText = MagikeSystem.CraftText[(int)MagikeSystem.CraftTextID.OtherItemNotEnough].Format(showItem.Name, showItem.stack - stack);
                    }

                    break;
                default:
                    break;
            }
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);

            //Helper.PlayPitched("Fairy/FairyBottleClick", 0.3f, 0.4f);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            Helper.PlayPitched("UI/Tick", 0.4f, 0);

            if (!CraftController.altar.IsWorking)
                CraftController.altar.ChosenResipe = recipe;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Item inv2 = showItem;

            int context = canCraft ? ItemSlot.Context.InventoryCoin : ItemSlot.Context.ChestItem;

            if (IsMouseHovering)
            {
                if (canCraft)
                {
                    Main.LocalPlayer.mouseInterface = true;
                    ItemSlot.OverrideHover(ref inv2, context);
                    ItemSlot.MouseHover(ref inv2, context);
                }
                else
                {
                    UICommon.TooltipMouseText(FailText);
                }

                _scale = Helper.Lerp(_scale, 0.85f, 0.2f);
            }
            else
                _scale = Helper.Lerp(_scale, 0.7f, 0.2f);

            float scale = Main.inventoryScale;
            Main.inventoryScale = _scale;

            Vector2 position = GetDimensions().Center() + (new Vector2(52f, 52f) * -0.5f * Main.inventoryScale);
            ItemSlot.Draw(spriteBatch, ref inv2, context, position, Color.White);

            Main.inventoryScale = scale;
        }
    }
}
