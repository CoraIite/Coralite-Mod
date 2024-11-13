﻿using System;
using System.IO;
using Terraria;

namespace Coralite.Core.Prefabs.Projectiles
{
    public abstract class BaseHeldProj : ModProjectile
    {
        #region Data
        private bool old_downLeftValue;
        private bool downLeftValue;
        private bool old_downRightValue;
        private bool downRightValue;
        /// <summary>
        /// 玩家左键控制
        /// </summary>
        protected bool DownLeft { get; private set; }
        /// <summary>
        /// 玩家右键控制
        /// </summary>
        protected bool DownRight { get; private set; }
        /// <summary>
        /// 一般情况下我们默认该弹幕的玩家作为弹幕主人，因此，弹幕的<see cref="Projectile.owner"/>属性需要被正确设置
        /// </summary>
        public virtual Player Owner => Main.player[Projectile.owner];
        /// <summary>
        /// 安全的获取一个重力倒转值
        /// </summary>
        public int SafeGravDir => Math.Sign(Owner.gravDir);
        /// <summary>
        /// 弹幕的理论朝向
        /// </summary>
        public virtual int DirSign => Owner.direction * SafeGravDir;
        /// <summary>
        /// 获取玩家到鼠标的向量
        /// </summary>
        public virtual Vector2 ToMouse { get; private protected set; }
        /// <summary>
        /// 获取玩家鼠标的位置
        /// </summary>
        public virtual Vector2 InMousePos { get; private protected set; }
        /// <summary>
        /// 获取玩家到鼠标的角度
        /// </summary>
        public virtual float ToMouseA { get; private protected set; }
        /// <summary>
        /// 获取玩家鼠标的单位向量
        /// </summary>
        public virtual Vector2 UnitToMouseV { get; private protected set; }
        /// <summary>
        /// 这个值用于在联机同步中使用，一般来讲，应该使用<see cref="ToMouse"/>
        /// </summary>
        private Vector2 toMouseVecterDate;
        private Vector2 _old_toMouseVecterDate;
        private const float toMouseVer_variationMode = 0.5f;
        /// <summary>
        /// 是否处于开火时间
        /// </summary>
        public virtual bool CanFire => false;

        #endregion

        protected void LockOwnerItemTime(int value = 2) => Owner.itemTime = Owner.itemAnimation = value;

        /// <summary>
        /// 单独处理玩家到鼠标的方向向量，同时处理对应的网络逻辑
        /// </summary>
        /// <returns></returns>
        private Vector2 UpdateToMouse()
        {
            if (Projectile.IsOwnedByLocalPlayer())
            {
                toMouseVecterDate = Owner.GetPlayerStabilityCenter().To(Main.MouseWorld);

                //int grgDir = 1;
                //if (CLMod.Instance.gravityDontFlipScreen != null && SafeGravDir < 0)
                //{
                //    grgDir *= -1;
                //}
                ////因为重力翻转后进行了坐标变换，这里反转一下Y值以保证旋转角正常
                //toMouseVecterDate.Y *= grgDir;

                bool difference = Math.Abs(toMouseVecterDate.X - _old_toMouseVecterDate.X) > toMouseVer_variationMode
                    || Math.Abs(toMouseVecterDate.Y - _old_toMouseVecterDate.Y) > toMouseVer_variationMode;
                if (difference && CanFire)
                {
                    NetUpdate();
                }
                _old_toMouseVecterDate = toMouseVecterDate;
            }
            return toMouseVecterDate;
        }
        /// <summary>
        /// 处理左键点击的更新逻辑，同时处理对应的网络逻辑
        /// </summary>
        /// <returns></returns>
        private bool UpdateDownLeftStart()
        {
            if (Projectile.IsOwnedByLocalPlayer())
            {
                downLeftValue = Owner.PressKey();
                if (old_downLeftValue != downLeftValue)
                {
                    NetUpdate();
                }
                old_downLeftValue = downLeftValue;
            }
            return downLeftValue;
        }
        /// <summary>
        /// 处理右键点击的更新逻辑，同时处理对应的网络逻辑
        /// </summary>
        /// <returns></returns>
        private bool UpdateDownRightStart()
        {
            if (Projectile.IsOwnedByLocalPlayer())
            {
                downRightValue = Owner.PressKey(false);
                if (old_downRightValue != downRightValue)
                {
                    NetUpdate();
                }
                old_downRightValue = downRightValue;
            }
            return downRightValue;
        }

        /// <summary>
        /// 更新玩家到鼠标的相关数据
        /// </summary>
        private void UpdateMouseData()
        {
            DownLeft = UpdateDownLeftStart();
            DownRight = UpdateDownRightStart();
            ToMouse = UpdateToMouse();
            ToMouseA = ToMouse.ToRotation();
            UnitToMouseV = ToMouse.UnitVector();
            InMousePos = ToMouse + Owner.GetPlayerStabilityCenter();
        }
        /// <summary>
        /// 在AI更新前进行数据更新
        /// </summary>
        public sealed override bool PreAI()
        {
            UpdateMouseData();
            ExtraPreSet();
            return PreUpdate();
        }

        public sealed override void PostAI()
        {
            if (!Owner.Alives())
            {
                Projectile.Kill();
            }
        }
        /// <summary>
        /// 发送一个比特体，存储8个栏位的布尔值，
        /// 如果子类准备重写，需要尊重父类的使用逻辑，当前已经占用至1号位
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public virtual BitsByte SandBitsByte(BitsByte flags)
        {
            flags[0] = downLeftValue;
            flags[1] = downRightValue;
            return flags;
        }
        /// <summary>
        /// 接受一个比特体，最多处理8个布尔属性的网络更新，
        /// 如果子类准备重写，需要尊重父类的使用逻辑，当前已经占用至1号位
        /// </summary>
        /// <param name="flags"></param>
        public virtual void ReceiveBitsByte(BitsByte flags)
        {
            downLeftValue = flags[0];
            downRightValue = flags[1];
        }

        public sealed override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(toMouseVecterDate);
            writer.Write(SandBitsByte(new BitsByte()));
            NetCodeHeldSend(writer);
        }

        public sealed override void ReceiveExtraAI(BinaryReader reader)
        {
            toMouseVecterDate = reader.ReadVector2();
            ReceiveBitsByte(reader.ReadByte());
            NetCodeReceiveHeld(reader);
        }

        public virtual void NetCodeHeldSend(BinaryWriter writer)
        {

        }

        public virtual void NetCodeReceiveHeld(BinaryReader reader)
        {

        }

        public virtual void ExtraPreSet()
        {

        }

        public virtual bool PreUpdate()
        {
            return true;
        }

        protected void SetHeld() => Owner.heldProj = Projectile.whoAmI;

        protected void SetDirection() => Owner.direction = Math.Sign(ToMouse.X);

        protected void NetUpdate() => Projectile.netUpdate = true;
    }
}
