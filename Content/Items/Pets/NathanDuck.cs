using Coralite.Content.Items.Glistent;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace Coralite.Content.Items.Pets
{
    public class NathanDuck : ModItem
    {
        public override string Texture => AssetDirectory.PetItems + Name;

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ModContent.ProjectileType<NathanDuckProj>(), ModContent.BuffType<NathanDuckBuff>());
            Item.width = 28;
            Item.height = 20;
            Item.rare = ModContent.RarityType<NathanRarity>();
            Item.value = Item.sellPrice(0, 5);
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.ItemAnimationJustStarted)
                player.AddBuff(Item.buffType, 15, true, false);

            return base.UseItem(player);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Duck)
                .AddIngredient<GlistentBar>(5)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class NathanRarity:ModRarity
    {
        public override Color RarityColor => Color.Lerp(new Color(255, 214, 84), new Color(255, 253, 217), Math.Abs(MathF.Sin(Main.GlobalTimeWrappedHourly * 3)));
    }

    public class NathanDuckBuff : BasePetBuff<NathanDuckProj>
    {
    }

    public class NathanDuckProj : BasePetProj
    {
        public override string Texture => AssetDirectory.PetItems + Name;
        protected override int PetBuffType => ModContent.BuffType<NathanDuckBuff>();

        /// <summary> 0：走地，1：飞行，2：特殊待机动画 </summary>
        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];
        public ref float FrameX => ref Projectile.localAI[0];
        public ref int FrameY => ref Projectile.frame;

        protected override void SetPetStaticDefaults()
        {
            // 暂用单帧，待贴图确定后补充动画帧数。
            Main.projFrames[Type] = 1;
        }

        protected override void SetPetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 44;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.timeLeft = 100;
            Projectile.friendly = false;
            Projectile.tileCollide = true;
            Projectile.decidesManualFallThrough = true;
        }

        public override bool? CanDamage() => false;

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override void AI()
        {
            if (!CheckActive(Owner))
                return;

            if (TeleportToOwner(Owner))
                Projectile.netUpdate = true;

            Projectile.shouldFallThrough = Owner.Bottom.Y - 12f > Projectile.Bottom.Y;

            switch (State)
            {
                case 0: // 走地
                case 2: // 特殊待机动画，使用相同的地面移动检测
                    AI_Walking(Owner);
                    break;
                case 1: // 飞行
                    AI_Flying(Owner);
                    break;
            }
        }

        private void AI_Walking(Player player)
        {
            Vector2 toPlayer = player.Center - Projectile.Center;
            if (toPlayer.Length() > 500f || Math.Abs(toPlayer.Y) > 200f)
            {
                ChangeState(1);
                return;
            }

            // 地面碰撞后Y速度为0，GroundMovement会在最后加入重力。
            // 保留加入重力前的运动状态，同时检测本帧是否触发了跳跃。
            const float gravity = 0.4f;
            bool movingVertically = Projectile.velocity.Y != 0f;
            GroundMovement(player.Center, maxSpeed: 6f,acceleration:0.6f,braking:0.7f, gravity: gravity,
                followOffset: new Vector2(-40f * player.direction, 0f));
            movingVertically |= Projectile.velocity.Y != gravity;
            bool needsMovement = Projectile.velocity.X != 0f || movingVertically;

            if (State == 2)
            {
                if (needsMovement)
                {
                    ChangeState(0);
                    UpdateWalkingFrame(movingVertically);
                    return;
                }

                FrameX = 1;
                Projectile.UpdateFrameNormally(5, 18);
                if (FrameY >= 18)
                    ChangeState(0);
                return;
            }

            UpdateWalkingFrame(movingVertically);
            if (needsMovement)
                Timer = 0;
            else if (++Timer >= 120)
                ChangeState(2);
        }

        private void UpdateWalkingFrame(bool movingVertically)
        {
            FrameX = 0;
            if (movingVertically)
            {
                FrameY = 2;
                Projectile.frameCounter = 0;
            }
            else if (Projectile.velocity.X != 0f)
            {
                // 与Tenko相同：帧计数每帧增加 (int)|速度X| / 3 + 1。
                // helper会自行增加1，这里只补充速度带来的增量。
                Projectile.frameCounter += (int)(Math.Abs(Projectile.velocity.X) *0.5f);
                Projectile.UpdateFrameNormally(7, 7);
            }
            else
            {
                FrameY = 0;
                Projectile.frameCounter = 0;
            }
        }

        private void AI_Flying(Player player)
        {
            UpdateWalkingFrame(false);
            for (int i = 0; i < 2; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Bottom + Main.rand.NextVector2Circular(Projectile.width *0.7f, 3), DustID.Water, Helper.NextVec2Dir(0, 0.5f), Alpha: 100, Scale: Main.rand.NextFloat(1, 1.5f));
                d.noGravity = true;
            }

            if (FlyMovement(player))
                ChangeState(0);
        }

        private void ChangeState(int state)
        {
            State = state;
            Timer = 0;
            FrameX = state == 2 ? 1 : 0;
            FrameY = state == 1 ? 2 : 0;
            Projectile.frameCounter = 0;
            Projectile.tileCollide = state != 1;
            if (state != 1)
                Projectile.rotation = 0f;
            Projectile.netUpdate = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Rectangle rect = new Rectangle((int)FrameX, FrameY, 2, 18);

            Projectile.QuickFrameDraw(rect, lightColor, 0, Projectile.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);

            return false;
        }
    }
}
