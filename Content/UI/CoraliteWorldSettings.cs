using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reflection;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;

namespace Coralite.Content.UI
{
    public class CoraliteWorldSettings //: BetterUIState
    {
        //public override int UILayer(List<GameInterfaceLayer> layers) => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

        //public override bool Visible => visible;
        //public static bool visible = false;

        public static UIPanel panel = new UIPanel();
        public static UIText descriptionText;

        public static GroupOptionButton<WorldDungeonID>[] DungeonButtons;

        public static WorldDungeonID DungeonType;

        public enum WorldDungeonID
        {
            Random,
            Dungeon,
            ShadowCastle
        }

        public static void OnInitialize(UIState state)
        {
            panel = new UIPanel()
            {
                BackgroundColor = new Color(33, 43, 79) * 0.8f,
            };
            panel.Width.Set(500, 0);
            panel.Height.Set(400, 0);
            panel.Top.Set(-240, 0.5f);
            panel.Left.Set(60, 0);

            state.Append(panel);

            int height = 10;
            AddDungeonSelect(panel, height);//添加地牢选择
            height += 48;
            AddHorizontalSeparator(panel, height);//添加分割线
            AddDescriptionPanel(panel, height, "desc");//添加描述文本

            SetDefaultOptions();
        }

        //public override void Update(GameTime gameTime)
        //{
        //    base.Update(gameTime);
        //}

        //public override void Recalculate()
        //{
        //    base.Recalculate();
        //}

        private static void AddDungeonSelect(UIElement container, float accumualtedHeight)
        {
            WorldDungeonID[] array = new WorldDungeonID[3] {
            WorldDungeonID.Random,
            WorldDungeonID.Dungeon,
            WorldDungeonID.ShadowCastle
            };
            LocalizedText[] array2 = new LocalizedText[3] {
            Lang.misc[103],
            Language.GetOrRegister("World/DungeonTitle",()=>"地牢"),//Lang.misc[101],
             Language.GetOrRegister("World/ShadowCastleTitle",()=>"影之城")//Lang.misc[102]
            };

            LocalizedText[] array3 = new LocalizedText[3] {
            Language.GetOrRegister("World/DungeonRandomDescription",()=>"让大自然来决定你的世界中出现地牢还是影之城"),//Language.GetText("UI.WorldDescriptionEvilRandom"),
            Language.GetOrRegister("World/DungeonDescription",()=>"充满骷髅与亡魂的邪恶地牢"),//Language.GetText("UI.WorldDescriptionEvilCorrupt"),
            Language.GetOrRegister("World/ShadowCastleDescription",()=>"影子们的城堡"),//Language.GetText("UI.WorldDescriptionEvilCrimson")
            };

            Color[] array4 = new Color[3] {
            Color.White,
            new Color(160,89,130),
            new Color(189,109,255)
            };

            string[] array5 = new string[3] {
            "Images/UI/WorldCreation/IconEvilRandom",
            "Images/UI/WorldCreation/IconEvilCorruption",
            "Images/UI/WorldCreation/IconEvilCrimson"
            };

            //Asset<Texture2D> t = Main.Assets.Request<Texture2D>("Coralite/Assets/Items/DefaultItem");

            GroupOptionButton<WorldDungeonID>[] array6 = new GroupOptionButton<WorldDungeonID>[array.Length];
            for (int i = 0; i < array6.Length; i++)
            {
                GroupOptionButton<WorldDungeonID> groupOptionButton =
                    new GroupOptionButton<WorldDungeonID>(array[i], array2[i], array3[i], array4[i],
                    i == 0 ? array5[i] : null, 1f, 1f, 16f);
                groupOptionButton.Width = StyleDimension.FromPixelsAndPercent(-4 * (array6.Length - 1), 1f / array6.Length * 1f);
                groupOptionButton.Left = StyleDimension.FromPercent(1f - 1f);
                groupOptionButton.HAlign = i / (float)(array6.Length - 1);
                groupOptionButton.Top.Set(accumualtedHeight, 0f);
                groupOptionButton.OnLeftMouseDown += ClickDungeonOption;
                groupOptionButton.OnMouseOver += ShowOptionDescription;
                groupOptionButton.OnMouseOut += ClearOptionDescription;
                groupOptionButton.SetSnapPoint("Dungeon", i);
                container.Append(groupOptionButton);
                array6[i] = groupOptionButton;
            }

            //私有是吧，全给你反射喽
            for (int i = 1; i < 3; i++)
            {
                Type t = array6[i].GetType();
                FieldInfo info = t.GetField("_iconTexture", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);
                info?.SetValue(array6[i], ModContent.Request<Texture2D>(AssetDirectory.UI + "DungeonType" + i));
            }

            DungeonButtons = array6;
        }

        private static void AddDescriptionPanel(UIElement container, float accumulatedHeight, string tagGroup)
        {
            float num = 0f;
            UISlicedImage uISlicedImage = new UISlicedImage(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelHighlight"))
            {
                HAlign = 0.5f,
                VAlign = 1f,
                Width = StyleDimension.FromPixelsAndPercent((0f - num) * 2f, 1f),
                Left = StyleDimension.FromPixels(0f - num),
                Height = StyleDimension.FromPixelsAndPercent(40f, 0f),
                Top = StyleDimension.FromPixels(2f)
            };

            uISlicedImage.SetSliceDepths(10);
            uISlicedImage.Color = Color.LightGray * 0.7f;
            container.Append(uISlicedImage);
            UIText uIText = new UIText(Language.GetText("UI.WorldDescriptionDefault"), 0.82f)
            {
                HAlign = 0f,
                VAlign = 0f,
                Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Top = StyleDimension.FromPixelsAndPercent(5f, 0f)
            };

            uIText.PaddingLeft = 20f;
            uIText.PaddingRight = 20f;
            uIText.PaddingTop = 6f;
            uISlicedImage.Append(uIText);
            descriptionText = uIText;
        }

        private static void AddHorizontalSeparator(UIElement Container, float accumualtedHeight)
        {
            UIHorizontalSeparator element = new UIHorizontalSeparator
            {
                Width = StyleDimension.FromPercent(1f),
                Top = StyleDimension.FromPixels(accumualtedHeight - 8f),
                Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
            };

            Container.Append(element);
        }


        private static void SetDefaultOptions()
        {
            GroupOptionButton<WorldDungeonID>[] evilButtons = DungeonButtons;
            for (int i = 0; i < evilButtons.Length; i++)
            {
                evilButtons[i].SetCurrentOption(WorldDungeonID.Random);
            }
        }

        private static void ClickDungeonOption(UIMouseEvent evt, UIElement listeningElement)
        {
            GroupOptionButton<WorldDungeonID> groupOptionButton = (GroupOptionButton<WorldDungeonID>)listeningElement;
            DungeonType = groupOptionButton.OptionValue;
            GroupOptionButton<WorldDungeonID>[] evilButtons = DungeonButtons;
            for (int i = 0; i < evilButtons.Length; i++)
            {
                evilButtons[i].SetCurrentOption(groupOptionButton.OptionValue);
            }
        }

        public static void ShowOptionDescription(UIMouseEvent evt, UIElement listeningElement)
        {
            LocalizedText localizedText = null;
            if (listeningElement is GroupOptionButton<WorldDungeonID> groupOptionButton3)
                localizedText = groupOptionButton3.Description;

            if (localizedText != null)
                descriptionText.SetText(localizedText);
        }

        public static void ClearOptionDescription(UIMouseEvent evt, UIElement listeningElement)
        {
            descriptionText.SetText(Language.GetText("UI.WorldDescriptionDefault"));
        }

    }
}
