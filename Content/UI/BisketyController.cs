using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using static Coralite.Content.UI.BisketyController;

namespace Coralite.Content.UI
{
    public class BisketyController : BetterUIState
    {
        public static bool visible = false;
        public static Vector2 basePos = new((Main.screenWidth / 2) - 100, (Main.screenHeight / 2) - 150);

        public override int UILayer(List<GameInterfaceLayer> layers) => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

        public override bool Visible => visible;

        public enum ShowType
        {
            ShowTotalDamage,
            ShowProjectileDamage,
            ShowItemDamage,
            ShowOtherDamage,
            ShowTotalDPS,
            //ShowShortlyDPS,
        }

        public static bool[] ShowFlags = new bool[5];

        public override void OnInitialize()
        {
            for (int i = 0; i < ShowFlags.Length; i++)
                ShowFlags[i] = false;
            UIPanel panel = new();

            panel.Width.Set(240, 0);
            panel.Height.Set(400, 0);
            panel.Top.Set(basePos.Y, 0);
            panel.Left.Set(basePos.X, 0);

            Append(panel);

            UIList list = new();
            list.Width.Set(200, 0);
            list.Left.Set(40, 0);
            list.Height.Set(400, 0);

            for (int i = 0; i < ShowFlags.Length; i++)
            {
                BisketyButton uIImageButton = new(ModContent.Request<Texture2D>(AssetDirectory.MiscItems + "Biskety", AssetRequestMode.ImmediateLoad), i);
                list.Add(uIImageButton);
            }

            panel.Append(list);

            UIImageButton close = new(ModContent.Request<Texture2D>(AssetDirectory.MiscItems + "BisketyHead", AssetRequestMode.ImmediateLoad));
            close.OnLeftClick += Close_OnLeftClick;
            panel.Append(close);
        }

        private void Close_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            visible = false;
        }
    }

    public class BisketyButton : UIImageButton
    {
        public int type;

        public BisketyButton(Asset<Texture2D> texture, int type) : base(texture)
        {
            this.type = type;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            if (ShowFlags[type])
                ShowFlags[type] = false;
            else
                ShowFlags[type] = true;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            if (IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
            }
            var dimensions = GetDimensions();

            Color c = Color.DarkRed;

            if (ShowFlags[type])
                c = Color.LightSeaGreen;

            Utils.DrawBorderString(spriteBatch, GetText(type),
                dimensions.Position() + new Vector2(dimensions.Width + 10, (dimensions.Height / 2) + 4), c, anchory: 0.5f);
        }

        public static string GetText(int i)
        {
            return i switch
            {
                (int)ShowType.ShowTotalDamage => "显示总伤害",
                (int)ShowType.ShowProjectileDamage => "显示弹幕伤害",
                (int)ShowType.ShowItemDamage => "显示物品伤害",
                (int)ShowType.ShowOtherDamage => "显示其他来源伤害",
                (int)ShowType.ShowTotalDPS => "显示总DPS",
                //case (int)ShowType.ShowShortlyDPS:
                //    return "显示短期DPS";
                _ => "",
            };
        }
    }
}
