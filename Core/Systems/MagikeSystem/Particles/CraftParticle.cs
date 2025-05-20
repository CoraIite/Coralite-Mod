//using Coralite.Content.Items.CoreKeeper;
//using Coralite.Helpers;
//using InnoVault.PRT;
//using Microsoft.Xna.Framework.Graphics;
//using Terraria;
//using Terraria.DataStructures;
//using Terraria.ID;
//using static Coralite.Helpers.MagikeHelper;

//namespace Coralite.Core.Systems.MagikeSystem.Particles
//{
//    /// <summary>
//    /// 绘制本体的不断收缩圆圈
//    /// 绘制旋转的物品
//    /// </summary>
//    public class CraftParticle : Particle
//    {
//        public override string Texture => AssetDirectory.Halos + "Circle";

//        private Point16 _pos;
//        private int _totalTime;
//        private float alpha;
//        private float Length;

//        public Item[] otherItems;

//        public override void SetProperty()
//        {
//            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
//            ShouldKillWhenOffScreen = false;
//            PRTDrawMode = PRTDrawModeEnum.NonPremultiplied;
//        }

//        public override void AI()
//        {
//            if (!TryGetEntityWithTopLeft(_pos, out _))
//            {
//                active = false;
//                return;
//            }

//            if (Opacity > 0)
//            {
//                float factor = 1 - Opacity / _totalTime;

//                if (factor < 0.1f)
//                    alpha = Helper.Lerp(0, 1, factor / 0.1f);
//                else if (factor > 0.7f)
//                    alpha = Helper.Lerp(1, 0, (factor - 0.7f) / 0.3f);

//                factor = Helper.BezierEase(Opacity / _totalTime);
//                Rotation = factor * MathHelper.TwoPi * 10;
//                Length = 12 + factor * 44;

//                if (Opacity > 120)
//                {
//                    if ((int)Opacity % 60 == 0)
//                        for (int i = 0; i < 20; i++)
//                        {
//                            Vector2 dir = (i * MathHelper.TwoPi / 20).ToRotationVector2();
//                            Dust dust = Dust.NewDustPerfect(Position - dir * 26, DustID.RainbowMk2, dir * Main.rand.NextFloat(1f, 2f)
//                                , newColor: Color, Scale: 0.8f);
//                            dust.noGravity = true;
//                        }

//                    if ((int)Opacity % 3 == 0 && Main.rand.NextBool())
//                    {
//                        Vector2 dir = Helper.NextVec2Dir();
//                        Dust dust = Dust.NewDustPerfect(Position - dir * 26, DustID.RainbowMk2, dir * Main.rand.NextFloat(1.5f, 3f)
//                            , newColor: Color, Scale: 0.8f);
//                        dust.noGravity = true;
//                    }
//                }
//            }

//            if (Opacity == 0)
//            {
//                for (int i = 0; i < 20; i++)
//                {
//                    Vector2 dir = (i * MathHelper.TwoPi / 20).ToRotationVector2();
//                    Dust dust = Dust.NewDustPerfect(Position, DustID.RainbowMk2, dir * Main.rand.NextFloat(2f, 5f)
//                        , newColor: Color, Scale: 1.2f);
//                    dust.noGravity = true;
//                    dust = Dust.NewDustPerfect(Position, DustID.RainbowMk2, dir * Main.rand.NextFloat(5f, 8f)
//                        , newColor: Color, Scale: 1.5f);
//                    dust.noGravity = true;
//                }

//                for (int i = 0; i < 5; i++)
//                {
//                    Dust dust = Dust.NewDustPerfect(Position, ModContent.DustType<Runes>(), Helper.NextVec2Dir(3f, 5f)
//                        , newColor: Color, Scale: 1f);
//                    dust.noGravity = true;
//                }

//                Helper.PlayPitched("UI/Success", 0.4f, -0.2f, Position);
//            }

//            if (Opacity < 0)
//            {
//                float factor = -Opacity / 20;
//                float factor2 = Helper.SqrtEase(factor);
//                float factor3 = Helper.X2Ease(factor);

//                Length = 12 + factor2 * 50;
//                alpha = 1 - factor3;
//            }

//            if (Opacity < -20)
//            {
//                active = false;
//            }

//            Opacity--;
//        }

//        public static CraftParticle Spawn(Point16 pos, Vector2 center, int craftTime, MagikeRecipe chosenRecipe)
//        {
//            CraftParticle p = PRTLoader.NewParticle<CraftParticle>(center, Vector2.Zero, Coralite.MagicCrystalPink);

//            if (chosenRecipe.RequiredItems != null && chosenRecipe.RequiredItems.Count > 0)
//            {
//                p.otherItems = new Item[chosenRecipe.RequiredItems.Count];
//                int i = 0;
//                foreach (var item in chosenRecipe.RequiredItems)
//                {
//                    p.otherItems[i] = item;
//                    i++;
//                }
//            }

//            p._totalTime = craftTime;
//            p.Opacity = craftTime;
//            p._pos = pos;

//            return p;
//        }

//        public override bool PreDraw(SpriteBatch spriteBatch)
//        {
//            Texture2D mainTex = TexValue;

//            Vector2 pos = Position - Main.screenPosition;
//            Color c = Color;
//            c.A = (byte)(200 * alpha);
//            var origin = mainTex.Size() / 2;
//            float scale = Length * 2 / mainTex.Width;
//            scale *= 1f;

//            spriteBatch.Draw(mainTex, pos, null, c, 0, origin, scale, 0, 0);

//            Texture2D tex2 = CoraliteAssets.Halo.Rune.Value;

//            origin = tex2.Size() / 2;
//            scale = Length * 2 / tex2.Width;
//            scale *= 1.1f;
//            spriteBatch.Draw(tex2, pos, null, c, Rotation, origin, scale, 0, 0);

//            //c = Color.White;
//            //c.A = (byte)(255 * alpha);

//            spriteBatch.Draw(tex2, pos, null, c, Rotation, origin, scale, 0, 0);

//            return false;
//        }

//        //TODO
//        //public override void DrawNonPremultiplied(SpriteBatch spriteBatch)
//        //{
//        //    if (otherItems == null)
//        //        return;

//        //    int total = otherItems.Length;
//        //    Color c = Color.White;
//        //    c.A = (byte)(255 * alpha);
//        //    Vector2 pos = Position - Main.screenPosition;

//        //    for (int i = 0; i < total; i++)
//        //    {
//        //        float rot = Rotation + (float)i / total * MathHelper.TwoPi;
//        //        DrawItem(spriteBatch, otherItems[i], pos + rot.ToRotationVector2() * Length*0.9f, 48, c);
//        //    }
//        //}
//    }
//}
