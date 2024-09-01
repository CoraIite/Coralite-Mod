using Coralite.Core;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor
{
    public class StickyGel : ModProjectile, IDrawPrimitive, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.SlimeEmperor + Name;

        BasicEffect effect;
        private Trail trail;

        public StickyGel()
        {
            if (Main.dedServ)
            {
                return;
            }

            Main.QueueMainThreadAction(() =>
            {
                effect = new BasicEffect(Main.instance.GraphicsDevice);
                effect.VertexColorEnabled = true;
                effect.Texture = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "LaserTrail", AssetRequestMode.ImmediateLoad).Value;
                effect.TextureEnabled = true;
            });
        }


        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 80;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.oldPos = new Vector2[12];
            for (int i = 0; i < 12; i++)
                Projectile.oldPos[i] = Projectile.Center;
        }

        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];
            if (Vector2.Distance(Owner.Center, Projectile.Center) < 64)
                Projectile.Kill();

            if (Projectile.timeLeft > 50)
            {
                Projectile.velocity += Vector2.Normalize(Owner.Center - Projectile.Center);

                if (Projectile.velocity.Length() > 16)
                    Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 16;

                Projectile.rotation = Projectile.velocity.ToRotation();
            }

            trail ??= new Trail(Main.instance.GraphicsDevice, 12, new NoTip(), factor => Helper.Lerp(4, 10, factor), factor =>
            {
                return Color.Lerp(Color.Transparent, new Color(78, 136, 255, 80), factor.X);
            });

            for (int i = 0; i < 11; i++)
                Projectile.oldPos[i] = Projectile.oldPos[i + 1];

            Projectile.oldPos[11] = Projectile.Center + Projectile.velocity;
            trail.Positions = Projectile.oldPos;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(CoraliteSoundID.QueenSlime2_Bubble_Item155);
            SpawnWebs();
            for (int i = 0; i < 16; i++)
            {
                float rot = i * MathHelper.TwoPi / 16;
                Dust.NewDustPerfect(Projectile.Center, DustID.t_Slime, rot.ToRotationVector2() * Main.rand.NextFloat(1, 6), 150, new Color(78, 136, 255, 80), Main.rand.NextFloat(1.5f, 2f));
            }
        }

        private void SpawnWebs()
        {
            int num = 6;
            int num2 = (int)(Projectile.Center.X / 16f);
            int num3 = (int)(Projectile.Center.Y / 16f);
            int num4 = num2 - num;
            int num5 = num2 + num;
            int num6 = num3 - num;
            int num7 = num3 + num;
            if (num4 < 1)
                num4 = 1;

            if (num5 > Main.maxTilesX - 1)
                num5 = Main.maxTilesX - 1;

            if (num6 < 1)
                num6 = 1;

            if (num7 > Main.maxTilesY - 1)
                num7 = Main.maxTilesY - 1;

            for (int i = num4; i < num5; i++)
            {
                for (int j = num6; j < num7; j++)
                {
                    if (!Main.tile[i, j].HasTile && !Main.rand.NextBool(5) && (Math.Abs(i - num2) * Math.Abs(i - num2)) + (Math.Abs(j - num3) * Math.Abs(j - num3)) < num * num)
                    {
                        WorldGen.PlaceTile(i, j, ModContent.TileType<StickyGelTile>());
                        if (Main.zenithWorld)
                        {
                            WorldGen.paintCoatTile(i, j, Main.rand.NextFromList(PaintCoatingID.Glow, PaintCoatingID.Echo));
                            WorldGen.paintTile(i, j, (byte)Main.rand.Next(PaintID.Old_IlluminantPaint + 1));
                        }
                        if (Main.tile[i, j].HasTile && Main.netMode == NetmodeID.Server)
                            NetMessage.SendTileSquare(-1, i, j);
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawPrimitives()
        {
            if (effect == null)
                return;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.World = world;
            effect.View = view;
            effect.Projection = projection;

            trail?.Render(effect);
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var pos = Projectile.Center - Main.screenPosition;
            Color color = Color.White;
            color.A = 150;
            if (Main.zenithWorld)
                color = SlimeEmperor.BlackSlimeColor;

            var frameBox = mainTex.Frame(1, 2, 0, 0);
            float scale = Projectile.scale;

            //绘制自己
            Main.spriteBatch.Draw(mainTex, pos, frameBox, color, Projectile.rotation, frameBox.Size() / 2, scale, 0, 0);

            float factor = MathF.Sin(Main.GlobalTimeWrappedHourly);
            color = new Color(50, 152 + (int)(100 * factor), 225);
            color *= 0.75f;

            //绘制发光
            frameBox = mainTex.Frame(1, 2, 0, 1);

            Main.spriteBatch.Draw(mainTex, pos, frameBox, color, Projectile.rotation, frameBox.Size() / 2, scale, 0, 0);
        }
    }

}
