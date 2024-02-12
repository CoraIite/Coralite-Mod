using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;

namespace Coralite.Core.Prefabs.Projectiles
{
    /// <summary>
    /// 规则：<br></br>
    /// </summary>
    public abstract class BaseFlyingShield : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];

        public bool CanChase = false;
        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];

        public abstract int FlyingTime { get; }
        public virtual string TrailTexture { get => AssetDirectory.OtherProjectiles + "EdgeTrail"; }

        public float BackSpeed = 10;
        public int trailCachesLength = 10;

        /// <summary>
        /// 反弹次数
        /// </summary>
        public float reflactCount;

        public enum FlyingShieldStates
        {
            Shooting,
            JustHited,
            Backing
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Timer = FlyingTime;
            UpdateShieldAccessory(accessory => accessory.OnInitialize(Projectile));
            Projectile.oldPos = new Vector2[trailCachesLength];
            Projectile.oldRot = new float[trailCachesLength];
            State = (int)FlyingShieldStates.Shooting;
        }

        public override void AI()
        {
            switch (State)
            {
                default:
                case (int)FlyingShieldStates.Shooting:
                    OnShootDusts();
                    Shooting();
                    break;
                case (int)FlyingShieldStates.JustHited:
                    OnJustHited();
                    break;
                case (int)FlyingShieldStates.Backing:
                    OnBacking();
                    break;
            }

            Projectile.UpdateOldPosCache();
            Projectile.UpdateOldRotCache();
        }

        public virtual void Shooting()
        {
            Timer--;
            if (Timer < 0)
                State = (int)FlyingShieldStates.Backing;
        }

        public virtual void OnShootDusts() { }

        public virtual void OnJustHited()
        {
            State = (int)FlyingShieldStates.Backing;
            UpdateShieldAccessory(accessory => accessory.OnJustHited(Projectile));
        }

        public virtual void OnBacking()
        {
            Projectile.velocity = (Owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * BackSpeed;

            if (Vector2.Distance(Owner.Center, Projectile.Center) < BackSpeed + 8)
                Projectile.Kill();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            UpdateShieldAccessory(accessory => accessory.OnTileCollide(Projectile));
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            State = (int)FlyingShieldStates.JustHited;
            UpdateShieldAccessory(accessory => accessory.OnHitNPC(Projectile, target, hit, damageDone));
        }

        public void UpdateShieldAccessory(Action<IFlyingShieldAccessory> action)
        {
            for (int i = 3; i < 10; i++)
            {
                if (!Owner.IsItemSlotUnlockedAndUsable(i))
                    continue;
                if (!Owner.armor[i].active)
                    continue;
                if (Owner.armor[i].ModItem is IFlyingShieldAccessory accessory)
                {
                    action(accessory);
                }
            }

            var loader = LoaderManager.Get<AccessorySlotLoader>();

            ModAccessorySlotPlayer masp = Owner.GetModPlayer<ModAccessorySlotPlayer>();
            for (int k = 0; k < masp.SlotCount; k++)
            {
                if (loader.ModdedIsItemSlotUnlockedAndUsable(k, Owner))
                {
                    Item i = loader.Get(k, Owner).FunctionalItem;
                    if (i.active && i.ModItem is IFlyingShieldAccessory accessory)
                    {
                        action(accessory);
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawTrails(lightColor);

            Texture2D mainTex = Projectile.GetTexture();
            var pos = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(mainTex, pos, null, lightColor, Projectile.rotation - 1.57f, mainTex.Size() / 2, Projectile.scale, 0, 0);

            return false;
        }

        public virtual void DrawTrails(Color lightColor)
        {
            Texture2D Texture = ModContent.Request<Texture2D>(TrailTexture).Value;

            List<CustomVertexInfo> bars = new List<CustomVertexInfo>();

            for (int i = 0; i < trailCachesLength; i++)
            {
                float factor = (float)i / trailCachesLength;
                Vector2 Center = Projectile.oldPos[i];
                Vector2 normal = (Projectile.oldRot[i] + MathHelper.PiOver2).ToRotationVector2();
                Vector2 Top = Center + normal * Projectile.width / 2;
                Vector2 Bottom = Center - normal * Projectile.width / 2;

                var Color =GetColor(factor).MultiplyRGB(lightColor);
                bars.Add(new(Top, Color, new Vector3(factor, 0, 1)));
                bars.Add(new(Bottom, Color, new Vector3(factor, 1, 1)));
            }

            List<CustomVertexInfo> Vx = new List<CustomVertexInfo>();
            if (bars.Count > 2)
            {
                for (int i = 0; i < bars.Count - 2; i += 2)
                {
                    Vx.Add(bars[i]);
                    Vx.Add(bars[i + 2]);
                    Vx.Add(bars[i + 1]);

                    Vx.Add(bars[i + 1]);
                    Vx.Add(bars[i + 2]);
                    Vx.Add(bars[i + 3]);
                }
            }

            Main.graphics.GraphicsDevice.Textures[0] = Texture;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, Vx.ToArray(), 0, Vx.Count / 3);
        }

        public virtual Color GetColor(float factor)
        {
            return Color.White;
        }
    }

    public interface IFlyingShieldAccessory
    {
        virtual void OnInitialize(Projectile projectile) { }

        virtual void OnTileCollide(Projectile projectile) { }

        virtual void OnJustHited(Projectile projectile) { }

        virtual void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone) { }
    }
}
