//using Coralite.Content.Tiles.Machines;
//using Coralite.Core;
//using Coralite.Core.Loaders;
//using Coralite.Core.Systems.BotanicalSystem;
//using Coralite.Helpers;
//using Microsoft.Xna.Framework.Graphics;
//using System.Collections.Generic;
//using Terraria;
//using Terraria.Audio;
//using Terraria.GameContent;
//using Terraria.GameContent.UI.Elements;
//using Terraria.GameInput;
//using Terraria.ID;
//using Terraria.UI;
//using static Terraria.ModLoader.ModContent;

//namespace Coralite.Content.UI
//{
//    class CrossBreedUI : BetterUIState
//    {
//        public static bool visible = false;
//        public static CrossBreedMachineEntity machineEntity = null;
//        public static UIImageButton crossBreedButton = new UIImageButton(Request<Texture2D>(AssetDirectory.UI + "CrossBreedButton", ReLogic.Content.AssetRequestMode.ImmediateLoad));
//        public static CrossBreedPanel panel = new CrossBreedPanel();
//        public static PlantParentsSlot FatherSlot = new PlantParentsSlot();
//        public static PlantParentsSlot MotherSlot = new PlantParentsSlot();
//        public static PlantSonSlot SonSlot = new PlantSonSlot();
//        public static CatalystSlot CatalystSlot = new CatalystSlot();

//        public byte timer;
//        public string crossBreedState = "";
//        public static Vector2 basePos = new Vector2((Main.screenWidth / 2) + 180, (Main.screenHeight / 2) - 150);

//        public override int UILayer(List<GameInterfaceLayer> layers) => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

//        public override bool Visible => visible;

//        public override void OnInitialize()
//        {
//            Elements.Clear();

//            SetPositionOnInitialize(panel, 0, 0);

//            crossBreedButton.Left.Set(basePos.X + 92, 0);
//            crossBreedButton.Top.Set(basePos.Y + 98, 0);
//            crossBreedButton.Width.Set(64, 0);
//            crossBreedButton.Height.Set(38, 0);
//            crossBreedButton.OnLeftClick += CrossBreedButton_OnClick;

//            Append(crossBreedButton);

//            SetPositionOnInitialize(FatherSlot, 30, 40);

//            SetPositionOnInitialize(CatalystSlot, 106, 40);

//            SetPositionOnInitialize(MotherSlot, 182, 40);

//            SetPositionOnInitialize(SonSlot, 106, 136);
//        }

//        private void SetPositionOnInitialize(UIElement element, int x, int y)
//        {
//            element.Left.Set(basePos.X + x, 0);
//            element.Top.Set(basePos.Y + y, 0);

//            Append(element);
//        }

//        public override void Update(GameTime gameTime)
//        {
//            if (!Main.playerInventory)
//                visible = false;

//            if (timer > 0)
//                timer--;
//            else
//                crossBreedState = "";
//        }

//        public override void Recalculate()
//        {
//            FatherSlot.Item = machineEntity?.father;
//            MotherSlot.Item = machineEntity?.mother;
//            SonSlot.Item = machineEntity?.son;
//            CatalystSlot.Item = machineEntity?.catalyst;

//            base.Recalculate();
//        }

//        public static void SaveItem()
//        {
//            if (machineEntity is null)
//                return;
//            machineEntity.father = FatherSlot.Item;
//            machineEntity.mother = MotherSlot.Item;
//            machineEntity.son = SonSlot.Item;
//            machineEntity.catalyst = CatalystSlot.Item;
//        }

//        private void CrossBreedButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
//        {
//            if (machineEntity is null)
//                return;

//            if (machineEntity.son.IsAir)
//            {
//                machineEntity.son = CrossBreed(machineEntity.father, machineEntity.mother, machineEntity.catalyst, out string text);
//                crossBreedState = text;
//                timer = 180;
//            }

//            UILoader.GetUIState<CrossBreedUI>().Recalculate();
//        }

//        /// <summary>
//        /// 杂交并突变，仅改变该植物的类型，其他数值请自行调整
//        /// 输入父植物，母植物，催化剂，字典的来判断是否会产生突变
//        /// </summary>
//        /// <param name="father">父物品</param>
//        /// <param name="mother">母物品</param>
//        /// <param name="catalyst">催化剂物品</param>
//        /// <param name="sonPlantType">输出的子植物的类型</param>
//        /// <returns></returns>
//        public static CrossBreedState CrossBreed_Mutant(Item father, Item mother, Item catalyst, out int sonPlantType)
//        {
//            //一些前置条件的判断，不满足的话直接结束
//            if (father.IsAir || mother.IsAir)
//            {
//                sonPlantType = 0;
//                return CrossBreedState.error;
//            }

//            BotanicalItem bFather = father.GetBotanicalItem();
//            BotanicalItem bMother = mother.GetBotanicalItem();
//            if (!bFather.botanicalItem || !bMother.botanicalItem)
//            {
//                sonPlantType = 0;
//                return CrossBreedState.error;
//            }

//            int fatherType = father.type;
//            int motherType = mother.type;
//            //如果两个植物没有能突变的情况，直接随机父母
//            if (!CrossBreedLoader.FindCrossBreedRecipe(fatherType, motherType, out CrossBreedData? data))
//            {
//                sonPlantType = Main.rand.NextBool() ? father.type : mother.type;
//                return CrossBreedState.notfound;
//            }

//            if (data is null)
//                throw new System.Exception("出大问题！！！！");


//            int mutantPower = 0;//催化剂效果
//            if (!catalyst.IsAir && catalyst.GetCatalystItem().isCatalyst)
//                mutantPower = catalyst.GetCatalystItem().mutantPower;

//            int percentage = data.Value.Percentage + mutantPower;//这个是突变成功的概率，最大为100
//            if (percentage > 100)
//                percentage = 100;

//            if (Main.rand.NextBool(percentage, 100))
//            {
//                sonPlantType = data.Value.MutantPlantType;
//                return CrossBreedState.success;
//            }
//            else
//            {
//                sonPlantType = Main.rand.NextBool() ? father.type : mother.type;
//                return CrossBreedState.failure;
//            }
//        }

//        /// <summary>
//        /// 杂交or突变，设置子代的各种相关数值
//        /// </summary>
//        /// <param name="father"></param>
//        /// <param name="mother"></param>
//        /// <param name="catalyst"></param>
//        /// <param name="text"></param>
//        /// <returns></returns>
//        public static Item CrossBreed(Item father, Item mother, Item catalyst, out string text)
//        {
//            CrossBreedState state = CrossBreed_Mutant(father, mother, catalyst, out int sonPlantType);
//            //通过上面一行来判断物品是不是空还有是不是植物
//            if (state == CrossBreedState.error)
//            {
//                text = "发生错误！";
//                Helper.PlayPitched("UI/Error", 0.4f, 0f);
//                return new Item();
//            }

//            Item son = new Item();
//            son.SetDefaults(sonPlantType);
//            BotanicalItem bFather = father.GetBotanicalItem();
//            BotanicalItem bMother = mother.GetBotanicalItem();
//            BotanicalItem bSon = son.GetBotanicalItem();
//            int growTimePower = 0;
//            int levelPower = 0;

//            //催化剂相关，如果有催化剂那么就消耗掉
//            if (!catalyst.IsAir)
//            {
//                CrossBreedCatalystItem catalystItem = catalyst.GetCatalystItem();
//                if (catalystItem.isCatalyst)
//                {
//                    growTimePower = catalystItem.growTimePower;
//                    levelPower = catalystItem.levelPower;

//                    if (catalyst.stack > 1)
//                        catalyst.stack--;
//                    else
//                        catalyst.TurnToAir();
//                }
//            }

//            switch (state)
//            {
//                case CrossBreedState.notfound://未找到突变组合，普通杂交
//                    GeneRandom(bFather, bMother, bSon, growTimePower, levelPower);
//                    text = "杂交成功";
//                    break;

//                case CrossBreedState.success://突变成功，不需要改变它的各种基因
//                    text = "突变成功！";
//                    break;

//                case CrossBreedState.failure://突变失败，变成普通杂交
//                    GeneRandom(bFather, bMother, bSon, growTimePower, levelPower);
//                    text = "突变失败！";
//                    break;
//                default: text = ""; break;
//            }

//            //消耗掉父母
//            if (father.stack > 1)
//                father.stack--;
//            else
//                father.TurnToAir();

//            if (mother.stack > 1)
//                mother.stack--;
//            else
//                mother.TurnToAir();

//            Helper.PlayPitched("UI/Success", 0.4f, 0f);
//            return son;
//        }

//        /// <summary>
//        /// 对于基因的随机，参考孟德尔遗传定律
//        /// </summary>
//        /// <param name="bFather"></param>
//        /// <param name="bMother"></param>
//        /// <param name="bSon"></param>
//        /// <param name="growTimePower"></param>
//        /// <param name="levelPower"></param>
//        public static void GeneRandom(BotanicalItem bFather, BotanicalItem bMother, BotanicalItem bSon, int growTimePower, int levelPower)
//        {
//            List<int> Genes = new List<int>(4);
//            Genes.Insert(0, bFather.DominantGrowTime);
//            Genes.Insert(1, bMother.DominantGrowTime);
//            Genes.Insert(2, bFather.RecessiveGrowTime);
//            Genes.Insert(3, bMother.RecessiveGrowTime);

//            int index = Main.rand.Next(4);  //从4个里面随机一个基因给到显性基因
//            bSon.DominantGrowTime = Genes[index];

//            Genes.RemoveAt(index);  //删除使用了的基因

//            index = Main.rand.Next(3);  //从剩下3个里随机选一个给隐性基因
//            bSon.RecessiveGrowTime = Genes[index];

//            Genes.Clear();      //注入等级基因
//            Genes.Insert(0, bFather.DominantLevel);
//            Genes.Insert(1, bMother.DominantLevel);
//            Genes.Insert(2, bFather.RecessiveLevel);
//            Genes.Insert(3, bMother.RecessiveLevel);

//            index = Main.rand.Next(4);  //从4个里面随机一个基因给到显性基因
//            bSon.DominantLevel = Genes[index];

//            Genes.RemoveAt(index);  //删除使用了的基因

//            index = Main.rand.Next(3);  //从剩下3个里随机选一个给隐性基因
//            bSon.RecessiveLevel = Genes[index];


//            //催化剂对于显性生长时间的效果
//            bSon.DominantGrowTime -= growTimePower;
//            if (bSon.DominantGrowTime < 1)
//                bSon.DominantGrowTime = 1;
//            //催化剂对于显性强度的效果
//            bSon.DominantLevel += levelPower;
//            if (bSon.DominantLevel > 100)
//                bSon.DominantLevel = 100;
//        }
//    }

//    public class PlantParentsSlot : UIElement
//    {
//        public Item Item;

//        public PlantParentsSlot()
//        {
//            Item = new Item();
//        }

//        public override void OnInitialize()
//        {
//            Width.Set(56, 0);
//            Height.Set(56, 0);
//        }

//        public override void LeftClick(UIMouseEvent evt)
//        {
//            //快捷取回到玩家背包中
//            if (PlayerInput.Triggers.Current.SmartSelect)
//            {
//                int invSlot = Helper.GetFreeInventorySlot(Main.LocalPlayer);

//                if (!Item.IsAir && invSlot != -1)
//                {
//                    Main.LocalPlayer.GetItem(Main.myPlayer, Item.Clone(), GetItemSettings.InventoryUIToInventorySettings);
//                    Item.TurnToAir();
//                    SoundEngine.PlaySound(SoundID.Grab);
//                }
//                CrossBreedUI.SaveItem();
//                return;
//            }

//            if (Main.mouseItem.IsAir && !Item.IsAir) //鼠标物品为空且UI内有物品，直接取出
//            {
//                Main.mouseItem = Item.Clone();
//                Item.TurnToAir();
//                SoundEngine.PlaySound(SoundID.Grab);
//                CrossBreedUI.SaveItem();
//                return;
//            }

//            if (!Main.mouseItem.IsAir && Item.IsAir) //鼠标有物品并且UI内为空，放入
//            {
//                Item = Main.mouseItem.Clone();
//                Main.mouseItem.TurnToAir();
//                SoundEngine.PlaySound(SoundID.Grab);
//                CrossBreedUI.SaveItem();
//                return;
//            }

//            if (!Main.mouseItem.IsAir && !Item.IsAir) //都有物品，进行交换,或堆叠
//            {
//                Item heldItem = Main.mouseItem;
//                if (heldItem.type != Item.type)
//                    goto Exchange;

//                BotanicalItem b1 = heldItem.GetBotanicalItem();
//                BotanicalItem b2 = Item.GetBotanicalItem();

//                if (b1.DominantGrowTime != b2.DominantGrowTime || b1.RecessiveGrowTime != b2.RecessiveGrowTime || b1.DominantLevel != b2.DominantLevel || b1.RecessiveLevel != b2.RecessiveLevel || b1.isIdentified != b2.isIdentified)
//                    goto Exchange;

//                if (heldItem.stack == heldItem.maxStack || Item.stack == Item.maxStack)
//                    goto Exchange;

//                if ((heldItem.stack + Item.stack) <= Item.maxStack)//两物品数量和小于最大堆叠数，直接全放入
//                {
//                    Item.stack += heldItem.stack;
//                    Main.mouseItem.TurnToAir();
//                    SoundEngine.PlaySound(SoundID.Grab);
//                }
//                else//两物品数量和大于最大堆叠数，框内物品变最大，鼠标物品为剩下的多少
//                {
//                    int count = Item.maxStack - Item.stack;
//                    Item.stack = Item.maxStack;
//                    Main.mouseItem.stack -= count;
//                    SoundEngine.PlaySound(SoundID.Grab);
//                }

//                return;

//            Exchange:
//                var temp = Item;
//                Item = Main.mouseItem;
//                Main.mouseItem = temp;
//                SoundEngine.PlaySound(SoundID.Grab);
//                CrossBreedUI.SaveItem();
//                return;
//            }
//        }

//        public override void RightClick(UIMouseEvent evt)
//        {
//            if (!Main.mouseItem.IsAir && !Item.IsAir && Main.mouseItem.ModItem is not null) //都有物品，且种类相同,一个个取出
//            {
//                Item heldItem = Main.mouseItem;
//                if (heldItem.type != Item.type)
//                    return;

//                BotanicalItem b1 = heldItem.GetBotanicalItem();
//                BotanicalItem b2 = Item.GetBotanicalItem();

//                if (b1.DominantGrowTime != b2.DominantGrowTime || b1.RecessiveGrowTime != b2.RecessiveGrowTime || b1.DominantLevel != b2.DominantLevel || b1.RecessiveLevel != b2.RecessiveLevel || b1.isIdentified != b2.isIdentified)
//                    return;

//                heldItem.stack++;
//                if (Item.stack > 1)
//                    Item.stack--;
//                else
//                    Item.TurnToAir();

//                //SoundEngine.PlaySound(SoundID.Grab);
//                CrossBreedUI.SaveItem();
//                return;
//            }

//            if (Main.mouseItem.IsAir && !Item.IsAir) //鼠标物品为空且UI内有物品，直接取出1个
//            {
//                if (Item.stack > 1)
//                {
//                    Main.mouseItem = Item.Clone();
//                    Main.mouseItem.stack = 1;
//                    Item.stack--;
//                    //SoundEngine.PlaySound(SoundID.Grab);
//                    CrossBreedUI.SaveItem();
//                }
//                else
//                {
//                    Main.mouseItem = Item.Clone();
//                    Item.TurnToAir();
//                    //SoundEngine.PlaySound(SoundID.Grab);
//                    CrossBreedUI.SaveItem();
//                }
//                return;
//            }
//        }

//        public override void Draw(SpriteBatch spriteBatch)
//        {
//            Texture2D mainTex = Request<Texture2D>(AssetDirectory.UI + "ParentsSlot").Value;
//            Vector2 pos = GetDimensions().Center();
//            spriteBatch.Draw(mainTex, pos, null, Color.White, 0, mainTex.Size() / 2, 1, SpriteEffects.None, 0);
//            if (IsMouseHovering)
//                Main.LocalPlayer.mouseInterface = true;
//            if (!Item.IsAir)
//            {
//                Texture2D itemTex = TextureAssets.Item[Item.type].Value;

//                Rectangle rectangle2;
//                if (Main.itemAnimations[Item.type] != null)
//                    rectangle2 = Main.itemAnimations[Item.type].GetFrame(itemTex, -1);
//                else
//                    rectangle2 = Utils.Frame(itemTex, 1, 1, 0, 0, 0, 0);

//                float itemScale = 1f;
//                float pixelWidth = mainTex.Width * 0.6f;
//                if (rectangle2.Width > pixelWidth || rectangle2.Height > pixelWidth)
//                {
//                    if (rectangle2.Width > itemTex.Height)
//                        itemScale = pixelWidth / rectangle2.Width;
//                    else
//                        itemScale = pixelWidth / rectangle2.Height;
//                }
//                Vector2 pos2 = GetDimensions().Position();
//                pos2.X += mainTex.Width / 2f - rectangle2.Width * itemScale / 2f;
//                pos2.Y += mainTex.Height / 2f - rectangle2.Height * itemScale / 2f;

//                spriteBatch.Draw(itemTex, pos2, new Rectangle?(rectangle2), Item.GetAlpha(Color.White), 0f, Vector2.Zero, itemScale, 0, 0f);
//                if (Item.color != default(Color))
//                    spriteBatch.Draw(itemTex, pos2, new Rectangle?(rectangle2), Item.GetColor(Color.White), 0f, Vector2.Zero, itemScale, 0, 0f);

//                if (Item.stack > 1)
//                    Utils.DrawBorderString(spriteBatch, Item.stack.ToString(), pos + new Vector2(-3, 14), Color.White, 0.8f, 1, 0.5f);
//                if (IsMouseHovering)
//                {
//                    Main.HoverItem = Item.Clone();
//                    Main.hoverItemName = "CoraliteCrossBreed";
//                }
//            }

//        }
//    }

//    public class CatalystSlot : UIElement
//    {
//        public Item Item;

//        public CatalystSlot()
//        {
//            Item = new Item();
//        }

//        public override void OnInitialize()
//        {
//            Width.Set(56, 0);
//            Height.Set(56, 0);
//        }

//        public override void LeftClick(UIMouseEvent evt)
//        {
//            //快捷取回到玩家背包中
//            if (PlayerInput.Triggers.Current.SmartSelect)
//            {
//                int invSlot = Helper.GetFreeInventorySlot(Main.LocalPlayer);

//                if (!Item.IsAir && invSlot != -1)
//                {
//                    Main.LocalPlayer.GetItem(Main.myPlayer, Item.Clone(), GetItemSettings.InventoryUIToInventorySettings);
//                    Item.TurnToAir();
//                    SoundEngine.PlaySound(SoundID.Grab);
//                }
//                CrossBreedUI.SaveItem();
//                return;
//            }

//            if (Main.mouseItem.IsAir && !Item.IsAir) //鼠标物品为空且UI内有物品，直接取出
//            {
//                Main.mouseItem = Item.Clone();
//                Item.TurnToAir();
//                SoundEngine.PlaySound(SoundID.Grab);
//                CrossBreedUI.SaveItem();
//                return;
//            }

//            if (!Main.mouseItem.IsAir && Item.IsAir) //鼠标有物品并且UI内为空，放入
//            {
//                Item = Main.mouseItem.Clone();
//                Main.mouseItem.TurnToAir();
//                SoundEngine.PlaySound(SoundID.Grab);
//                CrossBreedUI.SaveItem();
//                return;
//            }

//            if (!Main.mouseItem.IsAir && !Item.IsAir) //都有物品，进行交换,或堆叠
//            {
//                Item heldItem = Main.mouseItem;
//                if (heldItem.type != Item.type)
//                    goto Exchange;

//                BotanicalItem b1 = heldItem.GetBotanicalItem();
//                BotanicalItem b2 = Item.GetBotanicalItem();

//                if (b1.DominantGrowTime != b2.DominantGrowTime || b1.RecessiveGrowTime != b2.RecessiveGrowTime || b1.DominantLevel != b2.DominantLevel || b1.RecessiveLevel != b2.RecessiveLevel || b1.isIdentified != b2.isIdentified)
//                    goto Exchange;

//                if (heldItem.stack == heldItem.maxStack || Item.stack == Item.maxStack)
//                    goto Exchange;

//                if ((heldItem.stack + Item.stack) <= Item.maxStack)//两物品数量和小于最大堆叠数，直接全放入
//                {
//                    Item.stack += heldItem.stack;
//                    Main.mouseItem.TurnToAir();
//                    SoundEngine.PlaySound(SoundID.Grab);
//                }
//                else//两物品数量和大于最大堆叠数，框内物品变最大，鼠标物品为剩下的多少
//                {
//                    int count = Item.maxStack - Item.stack;
//                    Item.stack = Item.maxStack;
//                    Main.mouseItem.stack -= count;
//                    SoundEngine.PlaySound(SoundID.Grab);
//                }

//                return;

//            Exchange:
//                var temp = Item;
//                Item = Main.mouseItem;
//                Main.mouseItem = temp;
//                SoundEngine.PlaySound(SoundID.Grab);
//                CrossBreedUI.SaveItem();
//                return;
//            }
//        }

//        public override void RightClick(UIMouseEvent evt)
//        {
//            if (!Main.mouseItem.IsAir && !Item.IsAir && Main.mouseItem.ModItem is not null) //都有物品，且种类相同,一个个取出
//            {
//                Item heldItem = Main.mouseItem;
//                if (heldItem.type != Item.type)
//                    return;

//                BotanicalItem b1 = heldItem.GetBotanicalItem();
//                BotanicalItem b2 = Item.GetBotanicalItem();

//                if (b1.DominantGrowTime != b2.DominantGrowTime || b1.RecessiveGrowTime != b2.RecessiveGrowTime || b1.DominantLevel != b2.DominantLevel || b1.RecessiveLevel != b2.RecessiveLevel || b1.isIdentified != b2.isIdentified)
//                    return;

//                heldItem.stack++;
//                if (Item.stack > 1)
//                    Item.stack--;
//                else
//                    Item.TurnToAir();

//                //SoundEngine.PlaySound(SoundID.Grab);
//                CrossBreedUI.SaveItem();
//                return;
//            }

//            if (Main.mouseItem.IsAir && !Item.IsAir) //鼠标物品为空且UI内有物品，直接取出1个
//            {
//                if (Item.stack > 1)
//                {
//                    Main.mouseItem = Item.Clone();
//                    Main.mouseItem.stack = 1;
//                    Item.stack--;
//                    //SoundEngine.PlaySound(SoundID.Grab);
//                    CrossBreedUI.SaveItem();
//                }
//                else
//                {
//                    Main.mouseItem = Item.Clone();
//                    Item.TurnToAir();
//                    //SoundEngine.PlaySound(SoundID.Grab);
//                    CrossBreedUI.SaveItem();
//                }
//                return;
//            }
//        }

//        public override void Draw(SpriteBatch spriteBatch)
//        {
//            Texture2D mainTex = Request<Texture2D>(AssetDirectory.UI + "CatalystSlot").Value;
//            Vector2 pos = GetDimensions().Center();
//            spriteBatch.Draw(mainTex, pos, null, Color.White, 0, mainTex.Size() / 2, 1, SpriteEffects.None, 0);
//            if (IsMouseHovering)
//                Main.LocalPlayer.mouseInterface = true;
//            if (!Item.IsAir)
//            {
//                Texture2D itemTex = TextureAssets.Item[Item.type].Value;

//                Rectangle rectangle2;
//                if (Main.itemAnimations[Item.type] != null)
//                    rectangle2 = Main.itemAnimations[Item.type].GetFrame(itemTex, -1);
//                else
//                    rectangle2 = Utils.Frame(itemTex, 1, 1, 0, 0, 0, 0);

//                float itemScale = 1f;
//                float pixelWidth = mainTex.Width * 0.6f;
//                if (rectangle2.Width > pixelWidth || rectangle2.Height > pixelWidth)
//                {
//                    if (rectangle2.Width > itemTex.Height)
//                        itemScale = pixelWidth / rectangle2.Width;
//                    else
//                        itemScale = pixelWidth / rectangle2.Height;
//                }
//                Vector2 pos2 = GetDimensions().Position();
//                pos2.X += mainTex.Width / 2f - rectangle2.Width * itemScale / 2f;
//                pos2.Y += mainTex.Height / 2f - rectangle2.Height * itemScale / 2f;

//                spriteBatch.Draw(itemTex, pos2, new Rectangle?(rectangle2), Item.GetAlpha(Color.White), 0f, Vector2.Zero, itemScale, 0, 0f);
//                if (Item.color != default(Color))
//                    spriteBatch.Draw(itemTex, pos2, new Rectangle?(rectangle2), Item.GetColor(Color.White), 0f, Vector2.Zero, itemScale, 0, 0f);

//                if (Item.stack > 1)
//                    Utils.DrawBorderString(spriteBatch, Item.stack.ToString(), pos + new Vector2(-3, 14), Color.White, 0.8f, 1, 0.5f);
//                if (IsMouseHovering)
//                {
//                    Main.HoverItem = Item.Clone();
//                    Main.hoverItemName = "CoraliteCrossBreed";
//                }
//            }

//        }
//    }

//    public class PlantSonSlot : UIElement
//    {
//        public Item Item;

//        public PlantSonSlot()
//        {
//            Item = new Item();
//        }

//        public override void OnInitialize()
//        {
//            Width.Set(56, 0);
//            Height.Set(56, 0);
//        }

//        public override void LeftClick(UIMouseEvent evt)
//        {
//            //快捷取回到玩家背包中
//            if (PlayerInput.Triggers.Current.SmartSelect)
//            {
//                int invSlot = Helper.GetFreeInventorySlot(Main.LocalPlayer);

//                if (!Item.IsAir && invSlot != -1)
//                {
//                    Main.LocalPlayer.GetItem(Main.myPlayer, Item.Clone(), GetItemSettings.InventoryUIToInventorySettings);
//                    Item.TurnToAir();
//                    SoundEngine.PlaySound(SoundID.Grab);
//                }
//                CrossBreedUI.SaveItem();
//                return;
//            }

//            if (Main.mouseItem.IsAir && !Item.IsAir) //鼠标物品为空且UI内有物品，直接取出
//            {
//                Main.mouseItem = Item.Clone();
//                Item.TurnToAir();
//                SoundEngine.PlaySound(SoundID.Grab);
//                CrossBreedUI.SaveItem();
//                return;
//            }
//        }

//        public override void Draw(SpriteBatch spriteBatch)
//        {
//            Texture2D mainTex = Request<Texture2D>(AssetDirectory.UI + "SonSlot").Value;
//            Vector2 pos = GetDimensions().Center();
//            spriteBatch.Draw(mainTex, pos, null, Color.White, 0, mainTex.Size() / 2, 1, SpriteEffects.None, 0);
//            if (IsMouseHovering)
//                Main.LocalPlayer.mouseInterface = true;
//            if (!Item.IsAir)
//            {
//                Texture2D itemTex = Request<Texture2D>(Item.ModItem.Texture).Value;
//                spriteBatch.Draw(itemTex, pos, itemTex.Frame(), Color.White, 0, itemTex.Size() / 2, 1f, 0, 0);
//                if (Item.stack > 1)
//                    Utils.DrawBorderString(spriteBatch, Item.stack.ToString(), pos + new Vector2(-3, 14), Color.White, 0.8f, 1, 0.5f);
//                if (IsMouseHovering)
//                {
//                    Main.HoverItem = Item.Clone();
//                    Main.hoverItemName = "CoraliteCrossBreed";
//                }
//            }
//        }
//    }

//    public class CrossBreedPanel : UIElement
//    {
//        public override void OnInitialize()
//        {
//            Width.Set(308, 0);
//            Height.Set(214, 0);
//        }

//        public override void Draw(SpriteBatch spriteBatch)
//        {
//            Texture2D mainTex = Request<Texture2D>(AssetDirectory.UI + "CrossBreedPanel").Value;
//            spriteBatch.Draw(mainTex, GetDimensions().Center(), Color.White);

//            if (IsMouseHovering)
//                Main.LocalPlayer.mouseInterface = true;
//        }

//    }
//}
