using Coralite.Content.DamageClasses;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace Coralite.Core.Prefabs.Projectiles
{
    public abstract class BaseLassoSwing(short TrailLength) : BaseSwingProj(trailLength:TrailLength)
    {
        public virtual string HandleTexture => Texture + "Handle";

        /// <summary>
        /// 仙灵物品的type
        /// </summary>
        public ref float ItemType => ref Projectile.ai[0];

        public Vector2 CursorCenter => Projectile.Center;
        public float cursorRotation;

        /// <summary>
        /// 甩动时距离玩家的距离
        /// </summary>
        public float minDistance=48;
        /// <summary>
        /// 投出时距离玩家的距离
        /// </summary>
        public float maxDistance = 100;

        public int delayTime = 8;
        public int shootTime = 12;

        public override void SetDefs()
        {
            Projectile.DamageType = FairyDamage.Instance;
            Projectile.localNPCHitCooldown = -1;
            Projectile.width = 24;
            Projectile.height = 24;
            distanceToOwner = minDistance;
            minTime = 0;
            useShadowTrail = true;
            onHitFreeze = 0;
            
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => null;

        protected override void Initializer()
        {
            if (Main.myPlayer == Projectile.owner)
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

            startAngle = 1.57f;
            minTime = Owner.itemTimeMax;
            maxTime = minTime + shootTime;
            Smoother = Coralite.Instance.NoSmootherInstance;

            Projectile.velocity *= 0f;
            if (Owner.whoAmI == Main.myPlayer)
            {
                _Rotation = GetStartAngle() - OwnerDirection * startAngle;//设定起始角度
            }

            Smoother.ReCalculate(maxTime - minTime);

            if (useShadowTrail || useSlashTrail)
            {
                oldRotate = new float[trailLength];
                oldDistanceToOwner = new float[trailLength];
                oldLength = new float[trailLength];
                InitializeCaches();
            }

            Slasher();

            onStart = false;
            Projectile.netUpdate = true;
        }

        protected override void AIAfter()
        {
            Owner.itemRotation = _Rotation + (Owner.direction > 0 ? 0 : MathHelper.Pi);

            if (useShadowTrail || useSlashTrail)
                UpdateCaches();
        }

        protected override float GetStartAngle()
        {
            return Owner.direction > 0 ? 0 : 3.141f;
        }

        protected override void BeforeSlash()
        {
            //Projectile.Kill();
            _Rotation = GetStartAngle() - OwnerDirection * (startAngle + MathF.Sin(Timer / minTime * 2.5f * MathHelper.Pi) * 0.55f);
            Slasher();

            if ((int)Timer == minTime)
            {
                startAngle = _Rotation;
                totalAngle = (Main.MouseWorld - Owner.Center).ToRotation();
            }
        }

        protected override void OnSlash()
        {
            float factor = (Timer - minTime) / shootTime;
            factor = Coralite.Instance.X2Smoother.Smoother(factor);
            _Rotation = startAngle.AngleLerp(totalAngle, factor);
            distanceToOwner = Helper.Lerp(minDistance, maxDistance, factor);
            Slasher();
        }

        protected override void AfterSlash()
        {
            float factor = (Timer - maxTime) / delayTime;
            distanceToOwner = Helper.Lerp(maxDistance, 0, Coralite.Instance.SqrtSmoother.Smoother(factor));
            _Rotation += OwnerDirection * 0.05f;

            if ((int)Timer == maxTime + trailLength+1)
                ShootFairy();

            Slasher();
            if (Timer > maxTime + delayTime)
                Projectile.Kill();
        }

        protected override void Slasher()
        {
            RotateVec2 = _Rotation.ToRotationVector2();
            Projectile.Center = OwnerCenter() + oldRotate[^1].ToRotationVector2() * oldDistanceToOwner[^1];
            Projectile.rotation = _Rotation;
            cursorRotation = oldRotate[^1];
            Owner.itemLocation = Owner.Center + RotateVec2 * 15;
        }

        protected virtual void ShootFairy()
        {
            if (ItemType < 1)
                return;

            Item heldItem = Owner.HeldItem;
            if (heldItem.ModItem is BaseFairyCatcher catcher)
            {
                float speed = heldItem.shootSpeed/2;
                Vector2 velocity = oldRotate[^1].ToRotationVector2() * speed;
                Vector2 center = Projectile.Center;
                catcher.ModifyFairyStats(Owner, ref center, ref velocity);

                if (Owner.TryGetModPlayer(out FairyCatcherPlayer fcp) && fcp.FairyShoot_GetFairyBottle(out IFairyBottle bottle))
                {
                    float damage = Owner.GetWeaponDamage(heldItem);
                    fcp.TotalCatchPowerBonus(ref damage, heldItem);
                    Item[] fairies = bottle.Fairies;

                    for (int i = 0; i < fairies.Length; i++)
                    {
                        catcher.currentFairyIndex++;
                        if (catcher.currentFairyIndex > fairies.Length - 1)
                            catcher.currentFairyIndex = 0;

                        if (bottle.CanShootFairy(catcher.currentFairyIndex, out IFairyItem fairyItem))
                        {
                            if (bottle.ShootFairy(catcher.currentFairyIndex, Owner, new EntitySource_ItemUse_WithAmmo(Owner, heldItem, 0)
                                , center, velocity, (int)damage + (int)fairyItem.FairyDamage, Owner.GetWeaponKnockback(heldItem)))
                                break;
                        }
                    }

                    OnShootFairy();
                }
            }
        }

        public virtual void OnShootFairy() { }

        protected override void InitializeCaches()
        {
            for (int j = trailLength - 1; j >= 0; j--)
            {
                oldRotate[j] = _Rotation;
                oldDistanceToOwner[j] = distanceToOwner;
                oldLength[j] = Projectile.height * Projectile.scale;
            }
        }

        #region 绘制部分

        /// <summary>
        /// 手柄的与线段连接的位置
        /// </summary>
        /// <returns></returns>
        public virtual Vector2 GetHandlePos(Texture2D handleTex)
        {
            return Owner.itemLocation + RotateVec2 * DrawOriginOffsetX;
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
                Lighting.GetColor(Owner.Center.ToTileCoordinates()), _Rotation + OwnerDirection * spriteRotation, HandleTex.Size() / 2, 1f, Owner.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);
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

        public virtual void DrawFairyItem()
        {
            Vector2 center = CursorCenter-Main.screenPosition;

            int itemType = (int)ItemType;
            Main.instance.LoadItem(itemType);
            Texture2D mainTex = TextureAssets.Item[itemType].Value;
            Rectangle rectangle2;
            Color c = Lighting.GetColor(CursorCenter.ToTileCoordinates());

            if (Main.itemAnimations[itemType] != null)
                rectangle2 = Main.itemAnimations[itemType].GetFrame(mainTex, -1);
            else
                rectangle2 = mainTex.Frame();

            float itemScale = 1f;

            Main.spriteBatch.Draw(mainTex, center, new Rectangle?(rectangle2), c, 0f, rectangle2.Size() / 2, itemScale, Owner.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
        }

        protected override void DrawSelf(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
        {
            Texture2D handleTex = ModContent.Request<Texture2D>(HandleTexture).Value;
            Texture2D cursorTex = mainTex;

            //绘制连线
            DrawLine(GetHandlePos(handleTex), GetStringTipPos(handleTex));

            //绘制指针
            DrawCursor(cursorTex);

            //绘制仙灵物品
            if (Timer < maxTime&&ItemType>0)
                DrawFairyItem();
            //绘制手持
            DrawHandle(handleTex);

        }

        protected override void DrawShadowTrail(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot) { }

        #endregion
    }
}
