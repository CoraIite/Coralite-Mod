using Coralite.Content.DamageClasses;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;

namespace Coralite.Core.Prefabs.Projectiles
{
    public class BaseLassoSwing : BaseSwingProj
    {
        public virtual string HandleTexture => Texture + "Handle";

        /// <summary>
        /// 仙灵物品的type
        /// </summary>
        public ref float ItemType => ref Projectile.ai[0];

        public Vector2 CursorCenter => Projectile.Center;
        public float cursorRotation;

        public override void SetDefs()
        {
            Projectile.DamageType = FairyDamage.Instance;
            Projectile.localNPCHitCooldown = 48;
            Projectile.width = 30;
            Projectile.height = 30;
            trailTopWidth = 2;
            distanceToOwner = 8;
            minTime = 0;
            useShadowTrail = true;
            onHitFreeze = 0;
        }

        /// <summary>
        /// 用于额外调整弹幕大小等
        /// </summary>
        public virtual void PostSetDefs() { }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => null;

        protected override void Initializer()
        {
            if (Main.myPlayer == Projectile.owner)
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

            startAngle = 1.57f;
            onHitFreeze = 8;
            minTime = Owner.itemTimeMax;
            maxTime = minTime + 8;
            Smoother = Coralite.Instance.NoSmootherInstance;

            Projectile.velocity *= 0f;
            if (Owner.whoAmI == Main.myPlayer)
            {
                _Rotation = GetStartAngle() - OwnerDirection * startAngle;//设定起始角度
            }

            Slasher();
            Smoother.ReCalculate(maxTime - minTime);

            if (useShadowTrail || useSlashTrail)
            {
                oldRotate = new float[trailLength];
                oldDistanceToOwner = new float[trailLength];
                oldLength = new float[trailLength];
                InitializeCaches();
            }

            onStart = false;
            Projectile.netUpdate = true;
        }

        protected override float GetStartAngle()
        {
            return Owner.direction > 0 ? 0 : 3.141f;
        }

        protected override void BeforeSlash()
        {
            _Rotation = GetStartAngle() - OwnerDirection * (startAngle + MathF.Sin(Timer * 0.1f) * 0.25f);
            Slasher();

            if ((int)Timer == minTime)
            {
                totalAngle = (Main.MouseWorld - Owner.Center).ToRotation();

                InitializeCaches();
            }
        }

        protected override void OnSlash()
        {
            base.OnSlash();
        }

        protected override void Slasher()
        {
            RotateVec2 = _Rotation.ToRotationVector2();
            Projectile.Center = OwnerCenter() + oldRotate[^1].ToRotationVector2() * (Projectile.scale * Projectile.height / 2 + oldDistanceToOwner[^1]);
            Projectile.rotation = _Rotation;
        }

        #region 绘制部分

        /// <summary>
        /// 手柄的与线段连接的位置
        /// </summary>
        /// <returns></returns>
        public virtual Vector2 GetHandlePos(Texture2D handleTex)
        {
            return Owner.itemLocation + new Vector2(Owner.direction * Owner.gravDir * DrawOriginOffsetX, DrawOriginOffsetY);
        }

        /// <summary>
        /// 获取线的末端的位置
        /// </summary>
        /// <returns></returns>
        public virtual Vector2 GetStringTipPos(Texture2D cursorTex)
        {
            return CursorCenter;
        }

        /// <summary>
        /// 绘制指针与手持物品间的连线
        /// </summary>
        public virtual void DrawLine(Vector2 handlePos, Vector2 stringTipPos)
        {
            bool flag = true;
            handlePos.Y += Owner.gfxOffY;

            float distanceX = stringTipPos.X - handlePos.X;
            float distanceY = stringTipPos.Y - handlePos.Y;
            bool flag2 = true;
            float rot = (float)Math.Atan2(distanceY, distanceX) - 1.57f;

            Texture2D stringTex = GetStringTex();

            float halfWidth = stringTex.Width / 2;
            float halfHeight = stringTex.Height / 2;
            Vector2 origin = new Vector2(halfWidth, 0f);

            if (distanceX == 0f && distanceY == 0f)
            {
                flag = false;
            }
            else
            {
                float distance = (float)Math.Sqrt(distanceX * distanceX + distanceY * distanceY);
                distance = stringTex.Height / distance;
                distanceX *= distance;
                distanceY *= distance;
                handlePos.X -= distanceX * 0.1f;
                handlePos.Y -= distanceY * 0.1f;
                distanceX = stringTipPos.X - handlePos.X;
                distanceY = stringTipPos.Y - handlePos.Y;
            }

            while (flag)
            {
                float sourceHeight = stringTex.Height;
                float distance1 = (float)Math.Sqrt(distanceX * distanceX + distanceY * distanceY);
                float distance2 = distance1;
                if (float.IsNaN(distance1) || float.IsNaN(distance2))
                {
                    flag = false;
                    continue;
                }

                if (distance1 < stringTex.Height + 8)
                {
                    sourceHeight = distance1 - 8f;
                    flag = false;
                }

                distance1 = stringTex.Height / distance1;
                distanceX *= distance1;
                distanceY *= distance1;
                if (flag2)
                {
                    flag2 = false;
                }
                else
                {
                    handlePos.X += distanceX;
                    handlePos.Y += distanceY;
                }

                distanceX = stringTipPos.X - handlePos.X;
                distanceY = stringTipPos.Y - handlePos.Y;
                if (distance2 > stringTex.Height)
                {
                    float num9 = 0.3f;
                    float num10 = Math.Abs(Owner.velocity.X) + Math.Abs(Owner.velocity.Y);
                    if (num10 > 16f)
                        num10 = 16f;

                    num10 = 1f - num10 / 16f;
                    num9 *= num10;
                    num10 = distance2 / 80f;
                    if (num10 > 1f)
                        num10 = 1f;

                    num9 *= num10;
                    if (num9 < 0f)
                        num9 = 0f;

                    num9 *= num10;
                    num9 *= 0.5f;
                    if (distanceY > 0f)
                    {
                        distanceY *= 1f + num9;
                        distanceX *= 1f - num9;
                    }
                    else
                    {
                        num10 = Math.Abs(Projectile.velocity.X) / 3f;
                        if (num10 > 1f)
                            num10 = 1f;

                        num10 -= 0.5f;
                        num9 *= num10;
                        if (num9 > 0f)
                            num9 *= 2f;

                        distanceY *= 1f + num9;
                        distanceX *= 1f - num9;
                    }
                }

                rot = (float)Math.Atan2(distanceY, distanceX) - 1.57f;
                Color c = GetStringColor(handlePos);

                Main.EntitySpriteDraw(
                    color: c, texture: stringTex,
                    position: handlePos - Main.screenPosition + new Vector2(0, halfHeight), sourceRectangle: new Rectangle(0, 0, stringTex.Width, (int)sourceHeight), rotation: rot, origin: origin, scale: 1f, effects: SpriteEffects.None);
            }
        }

        public virtual void DrawCursor(Texture2D cursorTex)
        {
            var pos = CursorCenter - Main.screenPosition;
            var origin = cursorTex.Size() / 2;
            Main.spriteBatch.Draw(cursorTex, pos, null,
                Lighting.GetColor(CursorCenter.ToTileCoordinates()), cursorRotation, origin, 1, 0, 0);
        }

        public virtual void DrawHandle(Texture2D HandleTex)
        {
            Main.spriteBatch.Draw(HandleTex, Owner.itemLocation - Main.screenPosition, null,
                Lighting.GetColor(Owner.Center.ToTileCoordinates()), cursorRotation + spriteRotation, HandleTex.Size() / 2, 1f, 0, 0);
        }

        public virtual Color GetStringColor(Vector2 pos)
        {
            Color c = Color.White;
            c.A = (byte)(c.A * 0.4f);
            c = Lighting.GetColor((int)pos.X / 16, (int)(pos.Y / 16f), c);
            c *= 0.5f;
            return c;
        }

        public virtual Texture2D GetStringTex() => TextureAssets.FishingLine.Value;

        protected override void DrawSelf(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
        {
            Texture2D handleTex = ModContent.Request<Texture2D>(HandleTexture).Value;
            Texture2D cursorTex = mainTex;

            //绘制连线
            DrawLine(GetHandlePos(handleTex), GetStringTipPos(handleTex));
            //绘制指针
            DrawCursor(cursorTex);
            //绘制仙灵物品



            //绘制手持
            DrawHandle(handleTex);

        }

        protected override void DrawShadowTrail(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot) { }

        #endregion
    }
}
