using Coralite.Content.UI.BookUI;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.UI;

namespace Coralite.Content.CoraliteNotes
{
    public class CoraliteNoteUIState : BetterUIState
    {
        /// <summary> 是否可见 </summary>
        public override bool Visible { get => visible; set => visible = value; }
        public override int UILayer(List<GameInterfaceLayer> layers) => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

        public static CoraliteNotePanel BookPanel = new();

        public static BookPageArrow LeftArrow;
        public static BookPageArrow RightArrow;

        public bool visible;
        public bool openingBook;
        public bool closeingBook;
        public bool drawExtra;
        public Vector2 bookSize;
        public float bookWidth;
        public int Timer;
        //public readonly ParticleGroup particles = new();

        //public MagicCircle magicCircle;

        public override void OnInitialize()
        {
        }

        public void Init()
        {
            BookPanel.InitSize();
            BookPanel.SetPosition();
            BookPanel.InitPageGroups();
            BookPanel.InitGroups();
            BookPanel.InitArrows(ModContent.Request<Texture2D>(AssetDirectory.UI+ "PageArrowLeft",ReLogic.Content.AssetRequestMode.ImmediateLoad)
                , ModContent.Request<Texture2D>(AssetDirectory.UI + "PageArrowRight", ReLogic.Content.AssetRequestMode.ImmediateLoad));
            BookPanel.OnScrollWheel += PlaySound;
            Append(BookPanel);
        }

        private void PlaySound(UIScrollWheelEvent evt, UIElement listeningElement)
        {
            Helper.PlayPitched("Misc/Pages", 0.4f, 0f, Main.LocalPlayer.Center);
        }

        public override void Recalculate()
        {
            //BookPanel = new();
            //BookPanel.SetPosition();
            //BookPanel.InitSize();
            BookPanel?.SetPosition();
            //BookPanel.InitPageGroups();
            //BookPanel.InitGroups();
            //BookPanel.InitArrows(ModContent.Request<Texture2D>(AssetDirectory.UI + "PageArrowLeft", ReLogic.Content.AssetRequestMode.ImmediateLoad)
            //    , ModContent.Request<Texture2D>(AssetDirectory.UI + "PageArrowRight", ReLogic.Content.AssetRequestMode.ImmediateLoad));
            //BookPanel.OnScrollWheel += PlaySound;

            //RemoveAllChildren();
            //Append(BookPanel);
            base.Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            if (Main.playerInventory)
                closeingBook = true;

            UpdateVisualEffect();
            if (UpdateOpeningAnmi())
                UpdateClosingAnmi();

            base.Update(gameTime);
        }

        public void UpdateVisualEffect()
        {
            if (!drawExtra)
                return;

            //更新特效
            //particles.UpdateParticles();

            //if (magicCircle != null)
            //{
            //    magicCircle.Update();
            //    if (!magicCircle.active)
            //        magicCircle = null;
            //}

            if (!openingBook /*&& !particles.Any()*/)
                drawExtra = false;
        }

        public bool UpdateOpeningAnmi()
        {
            if (!openingBook)
                return true;

            //bookWidth += 25;
            //BookPanel.Width.Set(bookWidth, 0f);

            if (/*bookWidth >= bookSize.X ||*/ Timer > 30)        //超出时跳出
            {
                openingBook = false;
                BookPanel.OverflowHidden = false;
                BookPanel.Top.Percent = 0.5f;
                BookPanel.alpha = 1;
                //BookPanel.Width.Set(bookSize.X, 0f);

                base.Recalculate();
                return true;
            }

            float factor = Coralite.Instance.SqrtSmoother.Smoother(Timer / 30f);
            BookPanel.Top.Percent = Helper.Lerp(-1f, 0.5f, factor);
            BookPanel.alpha = Helper.Lerp(0, 1, factor);

            Timer++;
            base.Recalculate();

            return false;
        }

        public void UpdateClosingAnmi()
        {
            if (!closeingBook)//关闭书本时的效果
                return;

            BookPanel.alpha -= 0.05f;
            BookPanel.Top.Pixels += 80;
            if (BookPanel.alpha < 0.01f)
            {
                BookPanel.alpha = 0;
                closeingBook = false;
                visible = false;
                drawExtra = false;
            }

            base.Recalculate();
        }

        //public override void Draw(SpriteBatch spriteBatch)
        //{
        //    //if (drawExtra)      //分成两半绘制magicCircle
        //    //{
        //    //    magicCircle?.DrawFrontCircle(spriteBatch);
        //    //}

        //    base.Draw(spriteBatch);

        //    if (drawExtra)
        //    {
        //        //RasterizerState rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
        //        //spriteBatch.End();

        //        //Main.graphics.GraphicsDevice.BlendState = BlendState.NonPremultiplied;

        //        ////绘制特效
        //        ////foreach (var particle in particles)
        //        ////{
        //        ////    if (particle is IDrawParticlePrimitive idpp)
        //        ////        idpp.DrawPrimitives();
        //        ////}

        //        //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);

        //        ////particles.DrawParticles(spriteBatch);

        //        //spriteBatch.End();
        //        //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, rasterizerState, null, Main.UIScaleMatrix);

        //        magicCircle?.DrawBackCircle(spriteBatch);
        //    }
        //}

        public void OpenBook()
        {
            //Helper.PlayPitched("Stars/StarsSpawn", 0.4f, 0f, Main.LocalPlayer.Center);
            BookPanel.alpha = 0;
            BookPanel.OverflowHidden = true;
            bookSize = BookPanel.PanelTex.Size();
            bookWidth = 0;
            //BookPanel.Width.Set(0, 0);
            Timer = 0;

            //初始化特效
            //particles.Clear();
            Vector2 position = BookPanel.GetDimensions().Position();
            //magicCircle = new MagicCircle(bookSize.X + 50, (bookSize.Y / 2) + 40)
            //{
            //    center = position + new Vector2(25, bookSize.Y / 2),
            //    color = Color.White,
            //    velocity = new Vector2(25, 0),
            //};

            BookPanel.Top.Percent = -1f;

            openingBook = true;
            closeingBook = false;
            visible = true;
            drawExtra = true;
        }
    }
}
