using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour.HookGen;
using System;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Coralite.Core
{
    /// <summary>
    /// 这里的所有内容均源于fs49.org中的教学
    /// 感谢写教程的大佬
    /// 我只是简单的复制黏贴（甚至删了一堆注释）
    /// 请支持原作者Cyril大大
    /// </summary>
    //public class ModifyNameDraw : ModSystem
    //{
    //    public static Type _uiModItemType;
    //    public static MethodInfo _drawMethod;

    //    public static RenderTarget2D _renderTarget2D;

    //    public delegate void DrawDelegate(object uiModItem, SpriteBatch spriteBatch);

    //    public override void Load()
    //    {
    //        if (Main.dedServ)
    //            return;

    //        Main.QueueMainThreadAction(() =>
    //        {
    //            string text = Mod.DisplayName + " v" + Mod.Version;
    //            var size = ChatManager.GetStringSize(FontAssets.MouseText.Value, text, Vector2.One).ToPoint();

    //            _renderTarget2D = new RenderTarget2D(Main.graphics.GraphicsDevice, size.X, size.Y);

    //            Main.spriteBatch.Begin();

    //            Main.graphics.GraphicsDevice.SetRenderTarget(_renderTarget2D);
    //            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

    //            ChatManager.DrawColorCodedString(Main.spriteBatch, FontAssets.MouseText.Value, text, Vector2.Zero, Color.White, 0f, Vector2.Zero, Vector2.One);

    //            Main.spriteBatch.End();
    //            Main.graphics.GraphicsDevice.SetRenderTarget(null);
    //        });

    //        _uiModItemType = typeof(Main).Assembly.GetTypes().First(t => t.Name == "UIModItem");
    //        _drawMethod = _uiModItemType.GetMethod("Draw", BindingFlags.Instance | BindingFlags.Public);

    //        if (_drawMethod is not null)
    //            MonoModHooks.Add(_drawMethod, DrawMyName);
    //    }

    //    public override void Unload()
    //    {
    //        if (Main.dedServ)
    //            return;

    //        if (_drawMethod is not null)
    //            HookEndpointManager.Remove(_drawMethod, DrawMyName);

    //        if (_renderTarget2D is not null)
    //            _renderTarget2D = null;
    //    }

    //    private void DrawMyName(DrawDelegate orig, object uiModItem, SpriteBatch spriteBatch)
    //    {
    //        orig.Invoke(uiModItem, spriteBatch);

            //2023.2.1  仙布加这个特效，等什么时候内容多了什么时候加
            //if (_renderTarget2D is null)
            //    return;

            //if (_uiModItemType.GetField("_modName", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(uiModItem) is not UIText modName)
            //    throw new Exception("拟在赣神磨");

            //if (!modName.Text.Contains(Mod.DisplayName))
            //    return;

            //Texture2D texture = ModContent.Request<Texture2D>("Coralite/Assets/UI/CoraliteColorBar").Value;
            //Effect shader = ModContent.Request<Effect>("Coralite/Effects/NameDraw").Value;

            //shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.1f);
            //Main.instance.GraphicsDevice.Textures[1] = texture;

            //Vector2 position = modName.GetDimensions().Position() - new Vector2(0f, 2f);

            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Immediate, spriteBatch.GraphicsDevice.BlendState, spriteBatch.GraphicsDevice.SamplerStates[0],
            //                spriteBatch.GraphicsDevice.DepthStencilState, spriteBatch.GraphicsDevice.RasterizerState, shader, Main.UIScaleMatrix);

            //spriteBatch.Draw(_renderTarget2D, position, Color.White);

            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Deferred, spriteBatch.GraphicsDevice.BlendState, spriteBatch.GraphicsDevice.SamplerStates[0],
            //                spriteBatch.GraphicsDevice.DepthStencilState, spriteBatch.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);
    //    }
    //}
}
