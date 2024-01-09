using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
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

        public static UIPanel panel=new UIPanel();
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
            Language.GetText("UI.WorldDescriptionEvilRandom"),
            Language.GetText("UI.WorldDescriptionEvilCorrupt"),
            Language.GetText("UI.WorldDescriptionEvilCrimson")
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
                groupOptionButton.Top.Set(10f, 0f);
                groupOptionButton.OnLeftMouseDown += ClickDenguonOption;
                //groupOptionButton.OnMouseOver += ShowOptionDescription;
                //groupOptionButton.OnMouseOut += ClearOptionDescription;
                groupOptionButton.SetSnapPoint("Denguon", i);
                panel.Append(groupOptionButton);
                array6[i] = groupOptionButton;
            }


            for (int i = 1; i < 3; i++)
            {
                Type t = array6[i].GetType();
                FieldInfo info = t.GetField("_iconTexture", BindingFlags.NonPublic |BindingFlags.Instance|BindingFlags.GetField|BindingFlags.DeclaredOnly);
                info?.SetValue(array6[i], ModContent.Request<Texture2D>(AssetDirectory.UI + "DenguonType" + i));
            }

            DenguonButtons = array6;
        }

        //public override void Update(GameTime gameTime)
        //{
        //    base.Update(gameTime);
        //}

        //public override void Recalculate()
        //{
        //    base.Recalculate();
        //}

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
    }
}
