using Coralite.Content.DamageClasses;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases
{
    public abstract class BaseLassoSwing(short TrailLength) : BaseSwingProj(trailCount: TrailLength)
    {
        public virtual string HandleTexture => Texture + "Handle";

        /// <summary>
        /// 仙灵物品的type
        /// </summary>
        public ref float ItemType => ref Projectile.ai[0];
        /// <summary>
        /// 为1时会在挥动时执行捕捉功能
        /// </summary>
        public ref float Catch => ref Projectile.ai[1];

        /// <summary>
        /// 每次射出多少只仙灵
        /// </summary>
        public virtual int HowManyPerShoot { get => 2; }
        /// <summary>
        /// 连线的绘制有多少个点，越多越平滑
        /// </summary>
        public virtual int LinePointCount { get => 10; }

        public Vector2 CursorCenter => Projectile.Center;

        /// <summary>
        /// 用于控制线条的贝塞尔曲线绘制的点
        /// </summary>
        public Vector2 middlePos;

        /// <summary>
        /// 甩动时距离玩家的距离
        /// </summary>
        public float minDistance = 48;

        private float _maxDistance = 100;

        /// <summary>
        /// 投出时距离玩家的距离<br></br>
        /// 使用<see cref="SetMaxDistance"/>设置
        /// </summary>
        public float MaxDistance
        {
            get => _maxDistance;
            set
            {
                float length = (Owner.Center - InMousePos).Length();

                if (value > length)
                    _maxDistance = length < minDistance ? minDistance : length;
                else
                    _maxDistance = value;
            }
        }

        public int delayTime = 8;
        public int shootTime = 12;

        private HashSet<int> IDs;

        public override void SetSwingProperty()
        {
            Projectile.DamageType = FairyDamage.Instance;
            Projectile.localNPCHitCooldown = -1;
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.hide = true;
            Projectile.extraUpdates = 2;
            distanceToOwner = minDistance / 2;
            minTime = 0;
            useShadowTrail = true;
            onHitFreeze = 0;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Timer > minTime)
                return null;

            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => null;

        protected override void InitializeSwing()
        {
            if (Projectile.IsOwnedByLocalPlayer())
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

            delayTime *= Projectile.extraUpdates + 1;
            shootTime *= Projectile.extraUpdates + 1;
            startAngle = 1.57f;
            minTime = Owner.itemTimeMax * (Projectile.extraUpdates + 1);
            maxTime = minTime + shootTime;
            Smoother = Coralite.Instance.NoSmootherInstance;
            middlePos = Projectile.Center;

            Projectile.velocity *= 0f;
            if (Owner.whoAmI == Main.myPlayer)
            {
                _Rotation = GetStartAngle() - DirSign * startAngle;//设定起始角度
            }

            Smoother.ReCalculate(maxTime - minTime);

            if (useShadowTrail || useSlashTrail)
            {
                oldRotate = new float[trailCount];
                oldDistanceToOwner = new float[trailCount];
                oldLength = new float[trailCount];
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
            float factor = Timer / minTime;
            _Rotation = GetStartAngle() - DirSign * (startAngle + factor * MathF.Sin(-factor * 1.5f * MathHelper.Pi) * 1.2f);
            distanceToOwner = Helper.Lerp(minDistance / 2, minDistance, factor);
            Slasher();

            middlePos = OwnerCenter() + (GetStartAngle() - DirSign * (startAngle + factor * MathF.Sin(-factor * 1.5f * MathHelper.Pi) * 0.75f)).ToRotationVector2() * distanceToOwner/2 * factor;

            if ((int)Timer == minTime)
            {
                startAngle = _Rotation;
                totalAngle = (InMousePos - Owner.Center).ToRotation();
                SetMaxDistance();
            }
        }

        /// <summary>
        /// 用这个东西来设置最大长度
        /// </summary>
        public virtual void SetMaxDistance() { }

        protected override void OnSlash()
        {
            float factor = (Timer - minTime) / shootTime;
            factor = Helper.X3Ease(factor);

            //第一，二象限时不改变角度
            int i = 1;
            if (totalAngle < 0)
                i = 0;
            float trueTargetangle = totalAngle + Owner.direction * MathHelper.TwoPi * i;

            _Rotation = startAngle.AngleLerp(trueTargetangle, factor);
            distanceToOwner = Helper.Lerp(minDistance, MaxDistance, Helper.X3Ease(factor)*factor);
            Slasher();

            middlePos = OwnerCenter()
                + startAngle.AngleLerp(trueTargetangle, Helper.Lerp(factor,Helper.SqrtEase(factor),0.35f)).ToRotationVector2() * distanceToOwner * (0.5f + 0.5f * (1 - factor));

            if (Catch == 1)
                Helper.CheckCollideWithFairyCircle(Owner, Projectile.getRect(), ref IDs);
        }

        protected override void AfterSlash()
        {
            float factor = (Timer - maxTime) / delayTime;
            distanceToOwner = Helper.Lerp(MaxDistance, 0, Helper.SqrtEase(factor));
            _Rotation += DirSign * 0.02f;

            if ((int)Timer == maxTime + trailCount + 1)
                ShootFairy();
            
            Slasher();
            middlePos = OwnerCenter() + _Rotation.ToRotationVector2() * distanceToOwner*0.5f;

            if (Catch == 1)
                Helper.CheckCollideWithFairyCircle(Owner, Projectile.getRect(),ref IDs);

            if (Timer > maxTime + delayTime)
                Projectile.Kill();
        }

        protected override void Slasher()
        {
            RotateVec2 = _Rotation.ToRotationVector2();
            Projectile.Center = OwnerCenter() + oldRotate[^1].ToRotationVector2() * oldDistanceToOwner[^1];
            Projectile.rotation = _Rotation;
            Owner.itemLocation = Owner.Center + RotateVec2 * 15;
        }

        protected virtual void ShootFairy()
        {
            OnShootFairy();

            if (ItemType < 1)
                return;

            Item heldItem = Item;
            if (heldItem.ModItem is BaseFairyCatcher catcher)
            {
                float speed = heldItem.shootSpeed;
                Vector2 velocity = oldRotate[^1].ToRotationVector2() * speed;
                Vector2 center = Projectile.Center;

                if (Owner.TryGetModPlayer(out FairyCatcherPlayer fcp) && fcp.FairyShoot_GetFairyBottle(out BaseFairyBottle bottle))
                {
                    float damage = Owner.GetWeaponDamage(heldItem);
                    fcp.TotalCatchPowerBonus(ref damage, heldItem);
                    Item[] fairies = bottle.FightFairies;

                    //for (int i = 0; i < fairies.Length; i++)
                    //{
                    //    catcher.currentFairyIndex++;
                    //    if (catcher.currentFairyIndex > fairies.Length - 1)
                    //        catcher.currentFairyIndex = 0;

                    //    if (bottle.CanShootFairy(catcher.currentFairyIndex, out IFairyItem fairyItem))
                    //    {
                    //        if (bottle.ShootFairy(catcher.currentFairyIndex, Owner, new EntitySource_ItemUse_WithAmmo(Owner, heldItem, 0)
                    //            , center, velocity, (int)damage + (int)fairyItem.FairyDamage, Owner.GetWeaponKnockback(heldItem)))
                    //            break;
                    //    }
                    //}
                }
            }
        }

        public virtual void OnShootFairy()
        {
            SoundEngine.PlaySound(CoraliteSoundID.WhipSwing_Item152, Projectile.Center);
        }

        protected override void InitializeCaches()
        {
            for (int j = trailCount - 1; j >= 0; j--)
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
            return Owner.itemLocation + RotateVec2 * handleTex.Size().Length() / 2;
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
        public virtual void DrawLine(Vector2 handlePos, Vector2 stringTipPos,out float rot)
        {
            Texture2D stringTex = GetStringTex();
            rot = 0;

            float halfLineWidth = stringTex.Height / 2;

            List<ColoredVertex> bars = new();

            Vector2 recordPos = handlePos - Main.screenPosition;
            float recordUV = 0;

            //贝塞尔曲线
            for (int i = 0; i < LinePointCount + 1; i++)
            {
                float factor = (float)i / LinePointCount;

                Vector2 P1 = Vector2.Lerp(handlePos, middlePos, factor);
                Vector2 P2 = Vector2.Lerp(middlePos, stringTipPos, factor);

                Vector2 Center = Vector2.Lerp(P1, P2, factor);
                var Color = GetStringColor(Center);
                Center -= Main.screenPosition;

                Vector2 normal = (P2 - P1).SafeNormalize(Vector2.One).RotatedBy(MathHelper.PiOver2);
                Vector2 Top = Center + normal * halfLineWidth;
                Vector2 Bottom = Center - normal * halfLineWidth;

                recordUV += (Center - recordPos).Length() / stringTex.Width;

                bars.Add(new(Top, Color, new Vector3(recordUV, 0, 1)));
                bars.Add(new(Bottom, Color, new Vector3(recordUV, 1, 1)));

                if (i == LinePointCount)
                    rot = (Center - recordPos).ToRotation();

                recordPos = Center;
            }

            var state = Main.graphics.GraphicsDevice.SamplerStates[0];
            Main.graphics.GraphicsDevice.Textures[0] = stringTex;
            Main.graphics.GraphicsDevice.SamplerStates[0]=SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
            Main.graphics.GraphicsDevice.SamplerStates[0] = state;
        }

        public virtual void DrawCursor(Texture2D cursorTex, float rotation)
        {
            var pos = CursorCenter - Main.screenPosition;
            var origin = new Vector2(0, cursorTex.Height / 2);

            Main.spriteBatch.Draw(cursorTex, pos, null,
                Lighting.GetColor(CursorCenter.ToTileCoordinates()), rotation, origin, 1, 0, 0);
        }

        public virtual void DrawHandle(Texture2D HandleTex)
        {
            Main.spriteBatch.Draw(HandleTex, Owner.itemLocation - Main.screenPosition, null,
                Lighting.GetColor(Owner.Center.ToTileCoordinates()), _Rotation + DirSign * spriteRotation, HandleTex.Size() / 2, 1f, Owner.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);
        }

        public virtual Color GetStringColor(Vector2 pos)
        {
            Color c = Color.White;
            c.A = (byte)(c.A * 0.4f);
            c = Lighting.GetColor((int)pos.X / 16, (int)(pos.Y / 16f), c);
            c *= 0.5f;
            return c;
        }

        public virtual Texture2D GetStringTex() => FairySystem.DefaultLine.Value;

        public virtual void DrawFairyItem()
        {
            Vector2 center = CursorCenter - Main.screenPosition;

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
            DrawLine(GetHandlePos(handleTex), GetStringTipPos(handleTex),out float rot);

            //绘制指针
            DrawCursor(cursorTex,rot);

            //绘制仙灵物品
            if (Timer < maxTime && ItemType > 0)
                DrawFairyItem();
            //绘制手持
            DrawHandle(handleTex);

        }

        protected override void DrawShadowTrail(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot) { }

        #endregion
    }
}
