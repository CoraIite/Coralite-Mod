using Coralite.Content.Tiles.Machines;
using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.BotanicalSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.UI;
using static System.Formats.Asn1.AsnWriter;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.UI
{
    class CrossBreedUI : BetterUIState
    {
        public static bool visible = false;
        public static BaseMachineEntity machineEntity = null;
        public static UIImageButton crossBreedButton = new UIImageButton(Request<Texture2D>(AssetDirectory.UI + "CrossBreedButton", ReLogic.Content.AssetRequestMode.ImmediateLoad));
        public static PlantParentsSlot FatherSlot = new PlantParentsSlot();
        public static PlantParentsSlot MotherSlot = new PlantParentsSlot();
        public static PlantSonSlot SonSlot = new PlantSonSlot();
        public static PlantParentsSlot CatalystSlot = new PlantParentsSlot();

        public byte timer;
        public string crossBreedState = "";
        public static Vector2 basePos = new Vector2((Main.screenWidth / 2) + 80, (Main.screenHeight / 2) - 150);

        public override int UILayer(List<GameInterfaceLayer> layers) => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

        public override bool Visible => visible;

        public override void OnInitialize()
        {
            Elements.Clear();


            SetPositionOnInitialize(FatherSlot, 100, 80);
            crossBreedButton.Width.Set(64, 0);
            crossBreedButton.Height.Set(38, 0);
            crossBreedButton.OnClick += CrossBreedButton_OnClick;

            Append(crossBreedButton);

            SetPositionOnInitialize(FatherSlot, 0, 0);


            SetPositionOnInitialize(CatalystSlot, 80, 0);


            SetPositionOnInitialize(MotherSlot, 160, 0);



            SetPositionOnInitialize(SonSlot, 80, 200);
            SonSlot.Width.Set(32, 0);
            SonSlot.Height.Set(32, 0);

        }

        private void SetPositionOnInitialize(UIElement element, int x, int y)
        {
            element.Left.Set(basePos.X + x, 0);
            element.Top.Set(basePos.Y + y, 0);

            Append(element);
        }

        public override void Update(GameTime gameTime)
        {
            if (!Main.playerInventory)
                visible = false;

            if (timer > 0)
                timer--;
            else
                crossBreedState = "";
        }

        public override void Recalculate()
        {
            FatherSlot.Item = machineEntity?.father;
            MotherSlot.Item = machineEntity?.mother;
            SonSlot.Item = machineEntity?.son;
            CatalystSlot.Item = machineEntity?.catalyst;

            base.Recalculate();
        }

        public static void SaveItem()
        {
            if (machineEntity is null)
                return;
            machineEntity.father=FatherSlot.Item;
            machineEntity.mother= MotherSlot.Item;
            machineEntity.son=SonSlot.Item;
            machineEntity.catalyst=CatalystSlot.Item ;
        }

        private void CrossBreedButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (machineEntity is null)
                return;

            if (machineEntity.son.IsAir)
            {
                machineEntity.son = CrossBreed(machineEntity.father, machineEntity.mother, machineEntity.catalyst, out string text);
                crossBreedState = text;
                timer = 180;
            }

            UILoader.GetUIState<CrossBreedUI>().Recalculate();
        }

        /// <summary>
        /// 杂交并突变，仅改变该植物的类型，其他数值请自行调整
        /// 输入父植物，母植物，催化剂，根据父母植物中的字典来判断是否会产生突变
        /// </summary>
        /// <param name="father">父物品</param>
        /// <param name="mother">母物品</param>
        /// <param name="catalyst">催化剂物品</param>
        /// <param name="sonPlantType">输出的子植物的类型</param>
        /// <returns></returns>
        public static CrossBreedState CrossBreed_Mutant(Item father, Item mother, Item catalyst, out int sonPlantType)
        {
            //一些前置条件的判断，不满足的话直接结束
            if (father.IsAir || mother.IsAir)
            {
                sonPlantType = 0;
                return CrossBreedState.error;
            }

            BotanicalItem bFather = father.GetBotanicalItem();
            BotanicalItem bMother = mother.GetBotanicalItem();
            if (!bFather.botanicalItem || !bMother.botanicalItem)
            {
                sonPlantType = 0;
                return CrossBreedState.error;
            }

            //如果两个植物没有能突变的情况，直接随机父母
            if (bFather.CrossBreedDatas == null && bMother.CrossBreedDatas == null)
            {
                sonPlantType = Main.rand.NextBool() ? father.type : mother.type;
                return CrossBreedState.notfound;
            }

            int fatherType = father.type;
            int motherType = mother.type;
            //检测父物品的杂交种类字典
            if (bFather.CrossBreedDatas != null)
            {
                foreach (var item in bFather.CrossBreedDatas)
                {
                    if (item.Key == motherType)
                    {
                        int mutantPower = 0;//催化剂效果
                        if (!catalyst.IsAir)
                            if (catalyst.GetCatalystItem().isCatalyst)
                                mutantPower = catalyst.GetCatalystItem().mutantPower;

                        int percentage = item.Value.Percentage - mutantPower;//这个是突变成功的概率，最小为1
                        if (percentage < 1)
                            percentage = 1;

                        if (Main.rand.NextBool(percentage))
                        {
                            sonPlantType = item.Value.MutantPlantType;
                            return CrossBreedState.success;
                        }
                        else
                        {
                            sonPlantType = Main.rand.NextBool() ? father.type : mother.type;
                            return CrossBreedState.failure;
                        }
                    }
                }
            }
            //检测母物品的杂交种类字典
            if (bMother.CrossBreedDatas != null)
            {
                foreach (var item in bMother.CrossBreedDatas)
                {
                    if (item.Key == fatherType)
                    {
                        int mutantPower = 0;//催化剂效果
                        if (!catalyst.IsAir)
                            if (catalyst.GetCatalystItem().isCatalyst)
                                mutantPower = catalyst.GetCatalystItem().mutantPower;

                        int percentage = item.Value.Percentage - mutantPower;//这个是突变成功的概率，最小为1
                        if (percentage < 1)
                            percentage = 1;

                        if (Main.rand.NextBool(percentage))
                        {
                            sonPlantType = item.Value.MutantPlantType;
                            return CrossBreedState.success;
                        }
                        else
                        {
                            sonPlantType = Main.rand.NextBool() ? father.type : mother.type;
                            return CrossBreedState.failure;
                        }
                    }
                }
            }

            //遍历2个字典都没能找到能突变的情况的话直接在这里随机返回父母植物类型
            sonPlantType = Main.rand.NextBool() ? father.type : mother.type;
            return CrossBreedState.notfound;
        }

        /// <summary>
        /// 杂交or突变，设置子代的各种相关数值
        /// </summary>
        /// <param name="father"></param>
        /// <param name="mother"></param>
        /// <param name="catalyst"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Item CrossBreed(Item father, Item mother, Item catalyst, out string text)
        {
            CrossBreedState state = CrossBreed_Mutant(father, mother, catalyst, out int sonPlantType);
            //通过上面一行来判断物品是不是空还有是不是植物
            if (state == CrossBreedState.error)
            {
                text = "发生错误！";
                Helper.PlayPitched("UI/Error", 0.4f, 0f);
                return new Item();
            }

            Item son = new Item();
            son.SetDefaults(sonPlantType);
            BotanicalItem bFather = father.GetBotanicalItem();
            BotanicalItem bMother = mother.GetBotanicalItem();
            BotanicalItem bSon = son.GetBotanicalItem();
            int growTimePower = 0;
            int levelPower = 0;

            //催化剂相关，如果有催化剂那么就消耗掉
            if (!catalyst.IsAir)
            {
                CrossBreedCatalystItem catalystItem = catalyst.GetCatalystItem();
                if (catalystItem.isCatalyst)
                {
                    growTimePower = catalystItem.growTimePower;
                    levelPower = catalystItem.levelPower;

                    if (catalyst.stack > 1)
                        catalyst.stack--;
                    else
                        catalyst.TurnToAir();
                }
            }

            switch (state)
            {
                case CrossBreedState.notfound://未找到突变组合，普通杂交
                    bSon.DominantGrowTime = Main.rand.NextBool() ? bFather.DominantGrowTime : bMother.DominantGrowTime;
                    bSon.RecessiveGrowTime = Main.rand.NextBool() ? bFather.RecessiveGrowTime : bMother.RecessiveGrowTime;
                    bSon.DominantLevel = Main.rand.NextBool() ? bFather.DominantLevel : bMother.DominantLevel;
                    bSon.RecessiveLevel = Main.rand.NextBool() ? bFather.RecessiveLevel : bMother.RecessiveGrowTime;
                    //催化剂对于显性生长时间的效果
                    bSon.DominantGrowTime -= growTimePower;
                    if (bSon.DominantGrowTime < 1)
                        bSon.DominantGrowTime = 1;
                    //催化剂对于显性强度的效果
                    bSon.DominantLevel += levelPower;
                    if (bSon.DominantLevel > 100)
                        bSon.DominantLevel = 100;

                    text = "杂交成功";
                    break;

                case CrossBreedState.success://突变成功，不需要改变它的各种基因
                    text = "突变成功！";
                    break;

                case CrossBreedState.failure://突变失败，变成普通杂交
                    bSon.DominantGrowTime = Main.rand.NextBool() ? bFather.DominantGrowTime : bMother.DominantGrowTime;
                    bSon.RecessiveGrowTime = Main.rand.NextBool() ? bFather.RecessiveGrowTime : bMother.RecessiveGrowTime;
                    bSon.DominantLevel = Main.rand.NextBool() ? bFather.DominantLevel : bMother.DominantLevel;
                    bSon.RecessiveLevel = Main.rand.NextBool() ? bFather.RecessiveLevel : bMother.RecessiveGrowTime;
                    //催化剂对于显性生长时间的效果
                    bSon.DominantGrowTime -= growTimePower;
                    if (bSon.DominantGrowTime < 1)
                        bSon.DominantGrowTime = 1;
                    //催化剂对于显性强度的效果
                    bSon.DominantLevel += levelPower;
                    if (bSon.DominantLevel > 100)
                        bSon.DominantLevel = 100;

                    text = "突变失败！";
                    break;
                default: text = ""; break;
            }

            //消耗掉父母和催化剂
            if (father.stack > 1)
                father.stack--;
            else
                father.TurnToAir();

            if (mother.stack > 1)
                mother.stack--;
            else
                mother.TurnToAir();

            Helper.PlayPitched("UI/Success", 0.4f, 0f);
            return son;
        }
    }

    public class PlantParentsSlot : UIElement
    {
        public Item Item;

        public PlantParentsSlot()
        {
            Item = new Item();
        }

        public override void OnInitialize()
        {
            Width.Set(32, 0);
            Height.Set(32, 0);
        }

        public override void Click(UIMouseEvent evt)
        {
            //快捷取回到玩家背包中
            if (PlayerInput.Triggers.Current.SmartSelect)
            {
                int invSlot = Helper.GetFreeInventorySlot(Main.LocalPlayer);

                if (!Item.IsAir && invSlot != -1)
                {
                    Main.LocalPlayer.GetItem(Main.myPlayer, Item.Clone(), GetItemSettings.InventoryUIToInventorySettings);
                    Item.TurnToAir();
                    SoundEngine.PlaySound(SoundID.Grab);
                }
                CrossBreedUI.SaveItem();
                return;
            }

            if (Main.mouseItem.IsAir && !Item.IsAir) //鼠标物品为空且UI内有物品，直接取出
            {
                Main.mouseItem = Item.Clone();
                Item.TurnToAir();
                SoundEngine.PlaySound(SoundID.Grab);
                CrossBreedUI.SaveItem();
                return;
            }

            if (!Main.LocalPlayer.HeldItem.IsAir && Item.IsAir && Main.LocalPlayer.HeldItem.ModItem is not null) //鼠标有物品并且UI内为空，放入
            {
                Item = Main.LocalPlayer.HeldItem.Clone();
                Main.LocalPlayer.HeldItem.TurnToAir();
                Main.mouseItem.TurnToAir();
                SoundEngine.PlaySound(SoundID.Grab);
                CrossBreedUI.SaveItem();
                return;
            }

            if (!Main.LocalPlayer.HeldItem.IsAir && !Item.IsAir&&Main.LocalPlayer.HeldItem.ModItem is not null) //都有物品，进行交换
            {
                var temp = Item;
                Item = Main.LocalPlayer.HeldItem;
                Main.mouseItem = temp;
                SoundEngine.PlaySound(SoundID.Grab);
                CrossBreedUI.SaveItem();
                return;
            }
        }

        public override void RightClick(UIMouseEvent evt)
        {
            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = Request<Texture2D>(AssetDirectory.UI + "testSlot").Value;
            Vector2 pos = GetDimensions().Center();
            spriteBatch.Draw(mainTex, pos, null, Color.White, 0, mainTex.Size() / 2, 1, SpriteEffects.None, 0);
            if (!Item.IsAir)
            {
                Texture2D itemTex = Request<Texture2D>(Item.ModItem.Texture).Value;
                if (Item.stack > 1)
                    Utils.DrawBorderString(spriteBatch, Item.stack.ToString(), pos + new Vector2(-1, 1) * 14, Color.White, 0.8f, 1, 0.5f);
                spriteBatch.Draw(itemTex, pos, itemTex.Frame(), Color.White, 0, itemTex.Size() / 2, (mainTex.Size() * 0.8f) / itemTex.Size(), 0, 0);
                if (IsMouseHovering)
                {
                    Main.LocalPlayer.mouseInterface = true;
                    Main.HoverItem = Item.Clone();
                    Main.hoverItemName = "CoraliteCrossBreed";
                }
            }
                
        }
    }

    public class PlantSonSlot : UIElement
    {
        public Item Item;

        public PlantSonSlot()
        {
            Item = new Item();
        }

        public override void Click(UIMouseEvent evt)
        {
            //快捷取回到玩家背包中
            if (PlayerInput.Triggers.Current.SmartSelect)
            {
                int invSlot = Helper.GetFreeInventorySlot(Main.LocalPlayer);

                if (!Item.IsAir && invSlot != -1)
                {
                    Main.LocalPlayer.GetItem(Main.myPlayer, Item.Clone(), GetItemSettings.InventoryUIToInventorySettings);
                    Item.TurnToAir();
                    SoundEngine.PlaySound(SoundID.Grab);
                }
                CrossBreedUI.SaveItem();
                return;
            }

            if (Main.mouseItem.IsAir && !Item.IsAir) //鼠标物品为空且UI内有物品，直接取出
            {
                Main.mouseItem = Item.Clone();
                Item.TurnToAir();
                SoundEngine.PlaySound(SoundID.Grab);
                CrossBreedUI.SaveItem();
                return;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = Request<Texture2D>(AssetDirectory.UI + "testSlot").Value;
            Vector2 pos = GetDimensions().Center();
            spriteBatch.Draw(mainTex, pos, null, Color.White, 0, mainTex.Size() / 2, 1, SpriteEffects.None, 0);
            if (!Item.IsAir)
            {
                Texture2D itemTex = Request<Texture2D>(Item.ModItem.Texture).Value;
                if (Item.stack > 1)
                    Utils.DrawBorderString(spriteBatch, Item.stack.ToString(), pos + new Vector2(-1, 1) * 14, Color.White, 0.8f, 1, 0.5f);
                spriteBatch.Draw(itemTex, pos, itemTex.Frame(), Color.White, 0, itemTex.Size() / 2, (mainTex.Size() * 0.8f) / itemTex.Size(), 0, 0);
                if (IsMouseHovering)
                {
                    Main.LocalPlayer.mouseInterface = true;
                    Main.HoverItem = Item.Clone();
                    Main.hoverItemName = "CoraliteCrossBreed";
                }
            }
        }
    }
}
