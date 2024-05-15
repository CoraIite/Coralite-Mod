﻿using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using System;
using Terraria;
using Terraria.Audio;

namespace Coralite.Content.Items.CoreKeeper.Bases
{
    /// <summary>
    /// 使用ai0传入制作时间
    /// </summary>
    public class CraftUIProj : BaseHeldProj, IDrawPrimitive
    {
        public Func<Player, bool> CheckCanCraft;
        /// <summary>
        /// 读条完毕后合成的时候调用，在这里消耗物品以及生成物品
        /// </summary>
        public event Action<Player> OnCraft;

        public override string Texture => AssetDirectory.CoreKeeperItems + "CraftUI";

        public ref float CraftMaxTime => ref Projectile.ai[0];
        public ref float CraftTimer => ref Projectile.ai[2];

        private ParticleGroup particles;
        private SlotId slotID;

        public override void SetDefaults()
        {
            Projectile.timeLeft = 100;
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;
        public override bool? CanDamage() => false;

        public override void AI()
        {
            particles ??= new ParticleGroup();

            Projectile.Center = Owner.Center;
            if (CraftTimer > CraftMaxTime)
            {
                if (CheckCanCraft != null && CheckCanCraft(Owner))
                {
                    OnCraft?.Invoke(Owner);
                    particles.NewParticle<OnCraftLightShot>(Owner.Bottom, Vector2.Zero);
                    particles.NewParticle<OnCraftLightExpand>(Owner.Center, Vector2.Zero);
                }

                if (SoundEngine.TryGetActiveSound(slotID, out ActiveSound result))
                    result.Stop();

                Projectile.Kill();
                return;
            }

            if (CraftTimer % 3 == 0)
            {
                float distanceToOwner = Main.rand.NextFloat(0, 32);
                float r = 56 * (48 - distanceToOwner) / 48f;
                float startRot = Main.rand.NextFloat(-0.6f, 0.4f);
                //总旋转路程除以转速
                float time = (Main.rand.NextFloat(1.6f, 4.2f) - startRot) / 0.06f;
                if (Main.rand.NextBool())
                    particles.Add(SpecialCraftParticle.Spawn(Owner.Center + new Vector2(0, distanceToOwner * Main.rand.NextFromList(-1, 1)),
                        r, time, startRot));

                if ((CheckCanCraft != null && !CheckCanCraft(Owner))
                    || Owner.velocity != Vector2.Zero || Owner.controlHook || Owner.controlJump || Owner.controlMount)
                {
                    if (SoundEngine.TryGetActiveSound(slotID, out ActiveSound result))
                        result.Stop();

                    Projectile.Kill();
                    return;
                }
            }

            if (CraftTimer % (9 * 60) == 0)
            {
                if (SoundEngine.TryGetActiveSound(slotID, out ActiveSound result))
                    result.Stop();

                slotID = Helper.PlayPitched("CoreKeeper/teleportLoop", 0.8f, 0, Owner.Center);
            }

            Lighting.AddLight(Projectile.Center, 0.3f, 0.3f, 1f);
            particles?.UpdateParticles();
            CraftTimer++;
            Projectile.timeLeft = 2;
            Owner.itemAnimation = Owner.itemTime = 2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();

            Vector2 pos = Owner.Center + new Vector2(0, 200) - Main.screenPosition;
            var frameBox = mainTex.Frame(1, 2, 0, 0);

            Vector2 origin = frameBox.Size() / 2;

            Main.spriteBatch.Draw(mainTex, pos, frameBox, Color.White, 0, origin, 1, 0, 0);

            frameBox = mainTex.Frame(1, 2, 0, 1);
            int width = frameBox.Width;
            frameBox.Width = 2 + (int)(width * CraftTimer / CraftMaxTime);

            Main.spriteBatch.Draw(mainTex, pos, frameBox, Color.White, 0, origin, 1, 0, 0);

            //绘制额外特效

            mainTex = ModContent.Request<Texture2D>(AssetDirectory.CoreKeeperItems + "LightPillar").Value;
            Color c = new Color(24, 133, 216, 0) * 0.3f;
            pos = Owner.Bottom - Main.screenPosition;
            origin = new Vector2(mainTex.Width / 2, mainTex.Height);

            Main.spriteBatch.Draw(mainTex, pos, null, c, 0, origin, 3, 0, 0);
            Main.spriteBatch.Draw(mainTex, pos, null, c, 0, origin, new Vector2(1, 2), 0, 0);
            return false;
        }

        public void DrawPrimitives()
        {
            particles?.DrawParticlesPrimitive();
        }
    }

    public class SpecialCraftParticle : TrailParticle
    {
        public override string Texture => AssetDirectory.Blank;

        static BasicEffect effect;

        public SpecialCraftParticle()
        {
            Main.QueueMainThreadAction(() =>
            {
                effect = new BasicEffect(Main.instance.GraphicsDevice);
                effect.VertexColorEnabled = true;
            });
        }

        public override void OnSpawn()
        {
            color = Main.rand.Next(2) switch
            {
                0 => new Color(148, 247, 221),
                _ => new Color(24, 133, 216)
            };
            trail = new Trail(Main.instance.GraphicsDevice, 16, new NoTip(), factor => 1 * Scale, factor =>
            {
                if (factor.X < 0.7f)
                    return Color.Lerp(new Color(0, 0, 0, 0), color, factor.X / 0.7f);

                return Color.Lerp(color, Color.White, (factor.X - 0.7f) / 0.3f);
            });
            float length = Helper.EllipticalEase(Rotation, 0.3f, out float overrideAngle) * Velocity.X;
            Vector2 center = this.Center + overrideAngle.ToRotationVector2() * length;
            oldCenter = new Vector2[16];
            for (int i = 0; i < 16; i++)
                oldCenter[i] = center;
        }

        public override bool ShouldUpdateCenter() => false;

        public override void Update()
        {
            Rotation += 0.06f;

            if (fadeIn < Velocity.Y)
            {
                float length = Helper.EllipticalEase(Rotation, 0.3f, out float overrideAngle) * Velocity.X;

                for (int i = 0; i < 16 - 1; i++)
                    oldCenter[i] = oldCenter[i + 1];

                oldCenter[16 - 1] = Center + overrideAngle.ToRotationVector2() * length;
            }
            else if (fadeIn < Velocity.Y + 16)
            {
                for (int i = 0; i < 16 - 1; i++)
                    oldCenter[i] = oldCenter[i + 1];
            }
            else
            {
                active = false;
            }

            fadeIn++;
            trail.Positions = oldCenter;
        }

        public override void Draw(SpriteBatch spriteBatch) { }

        public static SpecialCraftParticle Spawn(Vector2 center, float r, float time, float startRot)
        {
            SpecialCraftParticle p = ParticleLoader.GetParticle(CoraliteContent.ParticleType<SpecialCraftParticle>()).NewInstance() as SpecialCraftParticle;
            p.Center = center;
            p.Velocity = new Vector2(r, time);
            p.Rotation = startRot;
            p.active = true;
            p.shouldKilledOutScreen = false;
            p.Scale = 1;

            p.OnSpawn();

            return p;
        }

        public override void DrawPrimitives()
        {
            if (effect == null)
                return;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            //effect.Texture = Texture2D.Value;
            effect.World = world;
            effect.View = view;
            effect.Projection = projection;

            trail?.Render(effect);
        }
    }

    public class OnCraftLightShot : Particle
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + "LightPillar";

        public override void OnSpawn()
        {
            color = new Color(148, 247, 221, 100);
            Velocity = new Vector2(2, 8);
        }

        public override bool ShouldUpdateCenter() => false;

        public override void Update()
        {
            Velocity.X *= 0.99f;
            color *= 0.99f;
            Velocity.Y *= 0.93f;

            if (Velocity.Y < 0.5f)
            {
                active = false;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = GetTexture().Value;
            Vector2 origin = new Vector2(mainTex.Width / 2, mainTex.Height);
            Vector2 pos = Center - Main.screenPosition;
            spriteBatch.Draw(mainTex, pos, null, color, 0, origin, Velocity, SpriteEffects.None, 0f);
            spriteBatch.Draw(mainTex, pos, null, color, 0, origin, Velocity * 0.9f, SpriteEffects.None, 0f);
            spriteBatch.Draw(mainTex, pos, null, color, 0, origin, Velocity * 0.9f, SpriteEffects.None, 0f);
        }
    }

    public class OnCraftLightExpand : Particle
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + "CircleLight";

        public override void OnSpawn()
        {
            Scale = 0.1f;
            color = new Color(148, 247, 221);
        }

        public override bool ShouldUpdateCenter() => false;

        public override void Update()
        {
            if (fadeIn > 7)
            {
                color *= 0.8f;
                Scale += 0.08f;
                Velocity += new Vector2(1, 0.5f) * 0.03f;
            }
            else
            {
                Scale += 0.08f;
                Velocity = new Vector2(1, 0.5f) * Scale;
            }

            if (fadeIn > 30 || color.A < 10)
                active = false;

            fadeIn++;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.CoreKeeperItems + "CircleLight2").Value;
            Vector2 origin = new Vector2(mainTex.Width / 2, mainTex.Height / 2);
            Vector2 pos = Center - Main.screenPosition;
            spriteBatch.Draw(mainTex, pos, null, color, Rotation, origin, Scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(mainTex, pos, null, color, Rotation, origin, Scale, SpriteEffects.None, 0f);
            mainTex = GetTexture().Value;

            spriteBatch.Draw(mainTex, pos, null, color, Rotation, origin, Velocity * 1.2f, SpriteEffects.FlipVertically, 0f);
            spriteBatch.Draw(mainTex, pos, null, color, Rotation, origin, Velocity * 1.2f, SpriteEffects.FlipVertically, 0f);
        }
    }
}
