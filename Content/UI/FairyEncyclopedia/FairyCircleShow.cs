using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.UI;

namespace Coralite.Content.UI.FairyEncyclopedia
{
    public class FairyCircleShow : UIElement
    {
        public int Timer;
        public Fairy[] Fairies { get; private set; }

        /// <summary>
        /// 捕捉器的圆圈的半径大小
        /// </summary>
        public float webRadius;

        private Projectile p;
        private FairyCatcherProj proj;

        public FairyCircleShow()
        {
            this.SetSize(9 * 2 * 16, 9 * 2 * 16);
            Fairies = new Fairy[3];
        }

        public void Reset()
        {
            p ??= new Projectile();
            proj ??= (FairyCatcherProj)ProjectileLoader
                .GetProjectile(ModContent.ProjectileType<FairyCatcherProj>())
                .NewInstance(p);
            webRadius = 0;
            Timer = 0;

            var pos = GetDimensions().Center();
            p.Center = pos;
            proj.webCenter = pos;
            for (int i = 0; i < 3; i++)
            {
                Fairies[i] = null;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Timer < 15)
            {
                webRadius += (9 * 16) / 15f;
                proj.webRadius = webRadius;
                Timer++;
            }
            else if (Timer == 15)
            {
                if (FairySystem.FairyCaught[FairyEncyclopedia.ShowFairyID])
                {
                    Fairy f = FairyLoader.GetFairy(FairyEncyclopedia.ShowFairyID);
                    int baseX = (int)(proj.webCenter.X / 16);
                    int baseY = (int)(proj.webCenter.Y / 16);
                    for (int i = 0; i < 3; i++)
                    {
                        Fairies[i] = f.NewInstance();
                        Fairies[i].Spawn(new FairyAttempt()
                        {
                            X = baseX + Main.rand.Next(-4, 4),
                            Y = baseY + Main.rand.Next(-4, 4),
                            catcherProj = proj
                        });
                    }
                }

                Timer++;
            }
            else
            {
                if (FairySystem.FairyCaught[FairyEncyclopedia.ShowFairyID])
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Fairies[i].UpdateInCatcher(proj);
                        Fairies[i].freeMoveTimer = 100;
                    }
                }
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 circlePos = GetDimensions().Center();

            Color edgeColor = Color.White;
            Color innerColor = Color.DarkSlateBlue * 0.7f;

            if (Main.LocalPlayer.TryGetModPlayer(out FairyCatcherPlayer fcp) && fcp.FairyCircleCoreType > -1)
            {
                var core = CoraliteContent.GetFairyCircleCore(fcp.FairyCircleCoreType);
                edgeColor = core.EdgeColor ?? edgeColor;
                innerColor = core.InnerColor ?? innerColor;
            }

            DrawBack(circlePos, edgeColor, innerColor);
            DrawCatcherCore(Color.White);

            if (Fairies != null)
                foreach (var fairy in Fairies)
                    fairy?.Draw_InUI();

            DrawName(spriteBatch);
        }

        public void DrawBack(Vector2 pos, Color circleColor, Color backColor)
        {
            Texture2D texture = FairyCatcherProj.TwistTex.Value;
            Effect shader = Filters.Scene["FairyCircle"].GetShader().Shader;

            float dia = 9 * 2 * 15 + 50;

            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly / 10);
            shader.Parameters["r"].SetValue(webRadius);
            shader.Parameters["dia"].SetValue(dia);
            shader.Parameters["edgeColor"].SetValue(circleColor.ToVector4());
            shader.Parameters["innerColor"].SetValue(backColor.ToVector4());

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap,
                            Main.spriteBatch.GraphicsDevice.DepthStencilState, EffectLoader.OverflowHiddenRasterizerState, shader, Main.UIScaleMatrix);

            float scale = dia / texture.Width;
            Main.spriteBatch.Draw(texture, pos
                , null, Color.White, 0, texture.Size() / 2, scale, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, Main.spriteBatch.GraphicsDevice.BlendState, SamplerState.AnisotropicClamp,
                            Main.spriteBatch.GraphicsDevice.DepthStencilState, EffectLoader.OverflowHiddenRasterizerState, null, Main.UIScaleMatrix);
        }

        public void DrawCatcherCore(Color lightColor)
        {
            Vector2 pos = GetDimensions().Center();

            Texture2D tex = TextureAssets.Projectile[ModContent.ProjectileType<FairyCatcherProj>()].Value;
            if (Main.LocalPlayer.TryGetModPlayer(out FairyCatcherPlayer fcp) && fcp.FairyCircleCoreType > -1)
            {
                var core = CoraliteContent.GetFairyCircleCore(fcp.FairyCircleCoreType);
                tex = FairyAsset.FairyCatcherCoreAssets[core.Type].Value;
            }

            tex.QuickCenteredDraw(Main.spriteBatch, pos, lightColor, 0);
        }

        public void DrawName(SpriteBatch spriteBatch)
        {
            Fairy f = FairyLoader.GetFairy(FairyEncyclopedia.ShowFairyID);
            Vector2 center = GetDimensions().Center() ;

            const int nameOffset = -50;
            const int rarityOffset = 60;

            if (!FairySystem.FairyCaught[f.Type])//没抓到显示？？？
            {
                Utils.DrawBorderStringBig(spriteBatch, "? ? ?"
                    , center + new Vector2(0, nameOffset), Color.White, 0.9f, 0.5f, 0.5f);
                Utils.DrawBorderStringBig(spriteBatch, "? ? ?"
                    , center + new Vector2(0, rarityOffset), Color.White, 0.4f, 0.5f, 0.5f);
                return;
            }

            if (f != null)
            {
                Item i = ContentSamples.ItemsByType[f.ItemType];
                Utils.DrawBorderStringBig(spriteBatch, i.Name,
                    center + new Vector2(0, nameOffset), ItemRarity.GetColor(i.rare), 0.9f, 0.5f, 0.5f);

                Utils.DrawBorderStringBig(spriteBatch, FairySystem.GetRarityDescription(f.Rarity)
                    , center + new Vector2(0, rarityOffset), FairySystem.GetRarityColor(f.Rarity), 0.4f, 0.5f, 0.5f);
            }
        }
    }
}
