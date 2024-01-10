using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reflection;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
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

        public static GroupOptionButton<WorldDenguonID>[] DenguonButtons;

        public static WorldDenguonID DenguonType;

        public enum WorldDenguonID
        {
            Random,
            Denguon,
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

            state. Append(panel);

            int height = 10;
            AddDenguonSelect(panel,height);//添加地牢选择
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

        private static void AddDenguonSelect(UIElement container,float accumualtedHeight)
        {
            WorldDenguonID[] array = new WorldDenguonID[3] {
            WorldDenguonID.Random,
            WorldDenguonID.Denguon,
            WorldDenguonID.ShadowCastle
            };
            LocalizedText[] array2 = new LocalizedText[3] {
            Lang.misc[103],
            Language.GetOrRegister("World/DenguonTitle",()=>"地牢"),//Lang.misc[101],
             Language.GetOrRegister("World/ShadowCastleTitle",()=>"影之城")//Lang.misc[102]
            };

            LocalizedText[] array3 = new LocalizedText[3] {
            Language.GetOrRegister("World/DenguonRandomDescription",()=>"让大自然来决定你的世界中出现地牢还是影之城"),//Language.GetText("UI.WorldDescriptionEvilRandom"),
            Language.GetOrRegister("World/DenguonDescription",()=>"充满骷髅与亡魂的邪恶地牢"),//Language.GetText("UI.WorldDescriptionEvilCorrupt"),
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

            GroupOptionButton<WorldDenguonID>[] array6 = new GroupOptionButton<WorldDenguonID>[array.Length];
            for (int i = 0; i < array6.Length; i++)
            {
                GroupOptionButton<WorldDenguonID> groupOptionButton =
                    new GroupOptionButton<WorldDenguonID>(array[i], array2[i], array3[i], array4[i],
                    i == 0 ? array5[i] : null, 1f, 1f, 16f);
                groupOptionButton.Width = StyleDimension.FromPixelsAndPercent(-4 * (array6.Length - 1), 1f / array6.Length * 1f);
                groupOptionButton.Left = StyleDimension.FromPercent(1f - 1f);
                groupOptionButton.HAlign = i / (float)(array6.Length - 1);
                groupOptionButton.Top.Set(accumualtedHeight, 0f);
                groupOptionButton.OnLeftMouseDown += ClickDenguonOption;
                groupOptionButton.OnMouseOver += ShowOptionDescription;
                groupOptionButton.OnMouseOut += ClearOptionDescription;
                groupOptionButton.SetSnapPoint("Denguon", i);
                container.Append(groupOptionButton);
                array6[i] = groupOptionButton;
            }

            //私有是吧，全给你反射喽
            for (int i = 1; i < 3; i++)
            {
                Type t = array6[i].GetType();
                FieldInfo info = t.GetField("_iconTexture", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);
                info?.SetValue(array6[i], ModContent.Request<Texture2D>(AssetDirectory.UI + "DenguonType" + i));
            }

            DenguonButtons = array6;
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
            GroupOptionButton<WorldDenguonID>[] evilButtons = DenguonButtons;
            for (int i = 0; i < evilButtons.Length; i++)
            {
                evilButtons[i].SetCurrentOption(WorldDenguonID.Random);
            }
        }

        private static void ClickDenguonOption(UIMouseEvent evt, UIElement listeningElement)
        {
            GroupOptionButton<WorldDenguonID> groupOptionButton = (GroupOptionButton<WorldDenguonID>)listeningElement;
            DenguonType = groupOptionButton.OptionValue;
            GroupOptionButton<WorldDenguonID>[] evilButtons = DenguonButtons;
            for (int i = 0; i < evilButtons.Length; i++)
            {
                evilButtons[i].SetCurrentOption(groupOptionButton.OptionValue);
            }
        }

        public static void ShowOptionDescription(UIMouseEvent evt, UIElement listeningElement)
        {
            LocalizedText localizedText = null;
            if (listeningElement is GroupOptionButton<WorldDenguonID> groupOptionButton3)
                localizedText = groupOptionButton3.Description;

            if (localizedText != null)
                descriptionText.SetText(localizedText);
        }

        public static  void ClearOptionDescription(UIMouseEvent evt, UIElement listeningElement)
        {
            descriptionText.SetText(Language.GetText("UI.WorldDescriptionDefault"));
        }

    }
}
