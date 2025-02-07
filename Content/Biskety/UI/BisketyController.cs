using Coralite.Content.UI;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.Localization;
using Terraria.UI;
using static Coralite.Content.Biskety.UI.BisketyController;

namespace Coralite.Content.Biskety.UI
{
    public class BisketyController : BetterUIState, ILocalizedModType
    {
        public static bool visible = false;
        public static Vector2 basePos = new(Main.screenWidth / 2 - 100, Main.screenHeight / 2 - 150);

        public override int UILayer(List<GameInterfaceLayer> layers) => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

        public static int BisketyDefence;
        public static int BisketyAI;

        public override bool Visible => visible;

        public static LocalizedText[] DamageShowController { get; set; }
        public static LocalizedText ChangeDefence { get; set; }

        public string LocalizationCategory => "Systems";

        public Mod Mod => Coralite.Instance;
        public string Name => GetType().Name;
        public string FullName => (Mod?.Name ?? "Terraria") + "/" + Name;

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
            UIDragablePanel panel = new UIDragablePanel(ModContent.Request<Texture2D>(AssetDirectory.UI + "MagikePanelBackground"),
                ModContent.Request<Texture2D>(AssetDirectory.UI + "MagikePanelBorder"), dragable: true);

            panel.Width.Set(300, 0);
            panel.Height.Set(400, 0);
            panel.Top.Set(basePos.Y, 0);
            panel.Left.Set(basePos.X, 0);

            Append(panel);

            UIList list = new();
            list.SetSize(300, 400);

            list.QuickInvisibleScrollbar();

            UIImageButton close = new(ModContent.Request<Texture2D>(AssetDirectory.MiscItems + "BisketyHead", AssetRequestMode.ImmediateLoad));
            close.OnLeftClick += Close_OnLeftClick;
            panel.Append(close);

            for (int i = 0; i < ShowFlags.Length; i++)
            {
                BisketyButton uIImageButton = new(ModContent.Request<Texture2D>(AssetDirectory.MiscItems + "Biskety", AssetRequestMode.ImmediateLoad), i);
                list.Add(uIImageButton);
            }

            panel.Append(list);

            BisketyDefenceController bisketyDefenceController = new BisketyDefenceController();
            panel.Append(bisketyDefenceController);

            InitText();
        }

        public override void Recalculate()
        {
            RemoveAllChildren();
            for (int i = 0; i < ShowFlags.Length; i++)
                ShowFlags[i] = false;
            UIDragablePanel panel = new UIDragablePanel(ModContent.Request<Texture2D>(AssetDirectory.UI + "MagikePanelBackground"),
                ModContent.Request<Texture2D>(AssetDirectory.UI + "MagikePanelBorder"), dragable: true);

            panel.Width.Set(300, 0);
            panel.Height.Set(400, 0);
            panel.Top.Set(basePos.Y, 0);
            panel.Left.Set(basePos.X, 0);

            Append(panel);

            UIList list = new();
            list.SetSize(300, 390);
            list.SetTopLeft(10, 0);

            list.QuickInvisibleScrollbar();

            UIImageButton close = new UIImageButton(ModContent.Request<Texture2D>(AssetDirectory.MiscItems + "BisketyHead", AssetRequestMode.ImmediateLoad));
            close.SetSize(300, 40);
            close.OnLeftClick += Close_OnLeftClick;
            list.Add(close);

            for (int i = 0; i < ShowFlags.Length; i++)
            {
                BisketyButton uIImageButton = new BisketyButton(ModContent.Request<Texture2D>(AssetDirectory.MiscItems + "Biskety", AssetRequestMode.ImmediateLoad), i);
                uIImageButton.SetSize(300, 60);
                uIImageButton.OverflowHidden = false;
                list.Add(uIImageButton);
            }

            panel.Append(list);

            //BisketyDefenceController bisketyDefenceController = new BisketyDefenceController();
            //list.Add(bisketyDefenceController);

            base.Recalculate();
        }

        public void InitText()
        {
            DamageShowController = [
                this.GetLocalization("ShowTotalDamage"),
                this.GetLocalization("ShowProjectileDamage"),
                this.GetLocalization("ShowMeleeDamage"),
                this.GetLocalization("ShowOtherDamage"),
                this.GetLocalization("ShowLongTimeDPS"),
                ];

            ChangeDefence = this.GetLocalization("ChangeDefence");
        }

        private void Close_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            visible = false;
        }
    }

    public class BisketyButton(ATex texture, int type) : UIImageButton(texture)
    {
        public int type = type;

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

            string t = GetText(type);
            Utils.DrawBorderString(spriteBatch, t,
                dimensions.Position() + new Vector2(60, dimensions.Height / 2 + 4), c, anchory: 0.5f);
        }

        public static string GetText(int i)
        {
            return DamageShowController[i].Value;
        }
    }

    public class BisketyDefenceController : UIElement
    {
        public static int Defence { get; private set; } = 0;

        public UIText Text;

        public BisketyDefenceController()
        {
            this.SetSize(0, 60, 1);
            var box = new UIVirtualKeyboard("???????????????????????????????", "0", OnTextChange, () => { }, allowEmpty: true);
            Text = new UIText("0");
            Text.SetSize(100, 20);
            Text.OnLeftMouseDown += Click_SetDefence;

            Append(Text);

            //UIImageButton changeButton = new UIImageButton(ModContent.Request<Texture2D>(AssetDirectory.UI + "ButtonPlay", AssetRequestMode.ImmediateLoad));
            //changeButton.SetTopLeft(10, 90);
            //changeButton.OnLeftClick += ChangeButton_OnLeftClick;

            //Append(changeButton);
        }

        private void Click_SetDefence(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(CoraliteSoundID.MenuTick);
            Main.clrInput();
            UIVirtualKeyboard uIVirtualKeyboard = new UIVirtualKeyboard("???????????????????????????????", "", OnTextChange, () => { }, allowEmpty: true);
            Main.NewText(1);

            //UIVirtualKeyboard uIVirtualKeyboard = new UIVirtualKeyboard(Lang.menu[48].Value, "", OnFinishedSettingName, GoBackHere, 0, allowEmpty: true);
            uIVirtualKeyboard.SetMaxInputLength(10);
            Main.MenuUI.SetState(uIVirtualKeyboard);
        }

        private void OnTextChange(string text)
        {
            Main.NewText(1);
            Defence = int.Parse(text);
            Text.SetText(text);
            foreach (var npc in Main.npc)
            {
                if (npc.type == ModContent.NPCType<Biskety>())
                    npc.defense = Defence;
            }
        }

        //private void ChangeButton_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        //{
        //    Main.NewText(1);
        //    Defence = int.Parse(box.Text);
        //    foreach (var npc in Main.npc)
        //    {
        //        if (npc.type == ModContent.NPCType<Biskety>())
        //            npc.defense = Defence;
        //    }
        //}

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 p = GetDimensions().Position();
            Vector2 c = GetDimensions().Center();
            Utils.DrawBorderString(spriteBatch, ChangeDefence.Value, new Vector2(p.X + 120, c.Y - 4), Color.White, anchory: 0.5f);
        }
    }
}
