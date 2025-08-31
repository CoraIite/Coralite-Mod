using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt
{
    [VaultLoaden(AssetDirectory.ZacurrentDragon)]
    public class ZacurrentSky : CustomSky
    {
        public static ATex BGNoise { get; set; }

        private bool _isActive;

        public int OwnerIndex;
        public int State;
        public int Timer;
        public int Timeleft = 0; //弄一个计时器，让天空能自己消失

        public const float BaseLight = 0.07f;
        //亮度
        public float light = 0.1f;
        public float c = 0.3f;
        public float oldLight = BaseLight;
        public float targetLight = BaseLight;
        public int LightTime;
        public int ExchangeTime;
        public int MaxExchangeTime;

        public override void Activate(Vector2 position, params object[] args)
        {
            OwnerIndex = NPC.FindFirstNPC(ModContent.NPCType<ZacurrentDragon>());
            State = 0;
            Timer = 0;
            _isActive = true;
            Timeleft = 3;
            LightTime = 0;
            light = BaseLight;
            //Main.StartRain();
        }

        public override void Deactivate(params object[] args)
        {
            OwnerIndex = -1;
            State = 0;
            Timer = 0;
            _isActive = false;
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (!OwnerIndex.GetNPCOwner<ZacurrentDragon>(out NPC owner))
                return;

            if (minDepth < 9 && maxDepth > 9)//绘制在背景景物后面，防止遮挡
            {
                Texture2D sky = ModContent.Request<Texture2D>(AssetDirectory.DefaultItem).Value;

                Rectangle screen = new(0, 0, Main.screenWidth, Main.screenHeight);
                Effect e = Filters.Scene["ZacurrentBackground"].GetShader().Shader;
                e.Parameters["iTime"].SetValue(Main.GlobalTimeWrappedHourly * 2);
                e.Parameters["bright"].SetValue(0.1f + light);
                e.Parameters["divide"].SetValue(4.5f / 10);
                e.Parameters["c"].SetValue(c);
                e.Parameters["worldSize"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
                e.Parameters["noise"].SetValue(BGNoise.Value);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, e);

                //VertexPositionColorTexture[] bar = new VertexPositionColorTexture[4];
                //bar[0] = new(Vector3.Zero, Color.White, new Vector2(0, 0));
                //bar[1] = new(new Vector3(0, Main.screenHeight,0), Color.White, new Vector2(0, 1));
                //bar[2] = new(new Vector3(Main.screenWidth, Main.screenHeight,0), Color.White, new Vector2(1, 0));
                //bar[3] = new(new Vector3(Main.screenWidth, Main.screenHeight,0), Color.White, new Vector2(1, 1));

                //Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bar, 0, 2);

                spriteBatch.Draw(sky, screen, Color.White * (Timeleft / 100f));

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, default, default, null, Main.BackgroundViewMatrix.ZoomMatrix);
            }
        }

        public override bool IsActive()
        {
            return _isActive;
        }

        public override void Reset()
        {
            OwnerIndex = -1;
            State = 0;
            Timer = 0;
            _isActive = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (Main.gamePaused)//游戏暂停时不执行
                return;

            if (Timeleft > 0)
                Timeleft--;//只要激活时就会减少，这样就会在外部没赋值时自己消失了
            else if (SkyManager.Instance["ZacurrentSky"].IsActive())
                SkyManager.Instance.Deactivate("ZacurrentSky");//消失

            if (!OwnerIndex.GetNPCOwner<ZacurrentDragon>(out NPC owner))
                return;

            c = (owner.ModNPC as ZacurrentDragon).PurpleVolt
                ? Helper.Lerp(c, -0.3f, 0.008f)
                : Helper.Lerp(c, 0.3f, 0.008f);

            float factor = Timeleft / 100f;
            if (owner.ai[0] == 1)
            {
                //Main.maxRaining = Math.Clamp(0.5f * factor, 0f, 1f);
                //Main.cloudAlpha = Math.Clamp(0.5f * factor, 0f, 1f);
                Main.windSpeedCurrent = 0;
                Main.windSpeedTarget = 0;
            }
            else
            {
                //Main.maxRaining = Math.Clamp(0.8f * factor, 0f, 1f);
                //Main.cloudAlpha = Math.Clamp(0.8f * factor, 0f, 1f);
                Main.windSpeedCurrent = 0;
                Main.windSpeedTarget = 0;
            }

            if (ExchangeTime > 0)
            {
                ExchangeTime--;
                light = Helper.Lerp(targetLight, oldLight, ExchangeTime / (float)MaxExchangeTime);
            }
            else if (LightTime > 0)
            {
                light -= (light - BaseLight) / LightTime;
                LightTime--;
            }
        }
    }

    public class ZacurrentSystem : ModSystem
    {
        public override void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor)
        {
            if (SkyManager.Instance["ZacurrentSky"].IsActive())
            {
                ZacurrentSky sky = (ZacurrentSky)SkyManager.Instance["ZacurrentSky"];
                Color c = backgroundColor;
                Color c2 = tileColor;
                backgroundColor = Color.Lerp(new Color(20, 20, 40, 255), new Color(180, 180, 255), sky.light);
                backgroundColor = Color.Lerp(c, backgroundColor, sky.Timeleft / 100f);
                tileColor = Color.Lerp(c2, backgroundColor, sky.Timeleft / 100f);
            }
        }
    }
}
