using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace Coralite.Content.UI.UILib
{
    /// <summary>
    /// 书页
    /// </summary>
    public abstract class UIPage : UIElement, ILocalizedModType
    {
        public static readonly RasterizerState OverflowHiddenRasterizerState = new RasterizerState
        {
            CullMode = CullMode.None,
            ScissorTestEnable = true
        };

        /// <summary>
        /// 是否能在书中显示
        /// </summary>
        public abstract bool CanShowInBook { get; }

        public float PageWidth => GetInnerDimensions().Width;
        public float PageHeight => GetInnerDimensions().Height;
        public Vector2 Center => GetInnerDimensions().Center();
        public Vector2 PageTop
        {
            get
            {
                CalculatedStyle c = GetInnerDimensions();
                return c.Position() + new Vector2(c.Width / 2, 0);
            }
        }
        public Vector2 Bottom
        {
            get
            {
                CalculatedStyle c = GetInnerDimensions();
                return c.Position() + new Vector2(c.Width / 2, c.Height);
            }
        }
        public Vector2 BottomLeft
        {
            get
            {
                CalculatedStyle c = GetInnerDimensions();
                return c.Position() + new Vector2(0, c.Height);
            }
        }
        public Vector2 BottomRight
        {
            get
            {
                CalculatedStyle c = GetInnerDimensions();
                return c.Position() + new Vector2(c.Width, c.Height);
            }
        }

        public Vector2 Position => GetInnerDimensions().Position();

        public abstract string LocalizationCategory { get; }

        public Mod Mod => Coralite.Instance;

        public string Name => GetType().Name;

        public string FullName => (Mod?.Name ?? "Terraria") + "/" + Name;

        public UIPage()
        {
            OverflowHidden = true;
        }


    }
}
