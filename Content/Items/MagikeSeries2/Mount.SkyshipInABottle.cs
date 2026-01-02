using Coralite.Content.Raritys;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class SkyshipInABottle : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public override void SetDefaults()
        {
            Item.DefaultToMount(ModContent.MountType<ChalcedonySkyship>());
            Item.rare = ModContent.RarityType<CrystallineMagikeRarity>();
            Item.value = Item.sellPrice(0, 2);
            Item.UseSound = CoraliteSoundID.RabbitMount_Item79 with { Pitch = 0.5f };
        }
    }

    public class ChalcedonySkyship : ModMount
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public override void SetStaticDefaults()
        {
            MountData.spawnDust = DustID.Cloud;

            MountData.heightBoost = 8;
            MountData.flightTimeMax = 320;
            MountData.fatigueMax = 320;
            MountData.fallDamage = 0f;
            MountData.usesHover = false;
            MountData.buff = ModContent.BuffType<ChalcedonySkyshipBuff>();

            MountData.runSpeed = 6f;
            MountData.dashSpeed = 6f;
            MountData.acceleration = 0.35f;
            MountData.jumpHeight = 4;
            MountData.jumpSpeed = 6f;
            MountData.swimSpeed = 6f;
            MountData.blockExtraJumps = true;

            MountData.totalFrames = 7;

            int[] array = new int[MountData.totalFrames];
            for (int num16 = 0; num16 < array.Length; num16++)
                array[num16] = 9;

            MountData.playerYOffsets = array;
            MountData.xOffset = 16;
            MountData.yOffset = -10;
            MountData.bodyFrame = 3;
            MountData.playerHeadOffset = 16;

            MountData.flyingFrameCount = 7;
            MountData.flyingFrameDelay = 6;
            MountData.flyingFrameStart = 0;
            // In-air
            MountData.inAirFrameCount = 7;
            MountData.inAirFrameDelay = 12;
            MountData.inAirFrameStart = 0;
        }

        public override bool UpdateFrame(Player mountedPlayer, int state, Vector2 velocity)
        {
            if (++mountedPlayer.mount._frameCounter > 10 - velocity.Length())
            {
                mountedPlayer.mount._frameCounter = 0;
                if (++mountedPlayer.mount._frame > 6)
                    mountedPlayer.mount._frame = 0;
            }
            return false;
        }

        public override void UpdateEffects(Player player)
        {
            float maxFallSpeed = 5f;
            float accDown = 0.35f;
            float maxUpSpeed = 5f;
            float accUp = 0.35f;

            player.gravity = 0.000001f; //玩家没有重力
            player.maxFallSpeed = maxFallSpeed; //控制下落最大速度
                                                //当按下“下”方向键时，如果Y轴速度没有达到最大则持续增加Y轴速度
            if (player.controlDown && player.velocity.Y <= maxFallSpeed)
            {
                player.velocity.Y += accDown;
            }
            else if ((player.controlUp || player.controlJump) && player.velocity.Y >= -maxUpSpeed)
            {
                player.velocity.Y -= accUp; //同理
            }
            else
            {
                player.velocity.Y *= 0.98f;
                if (MathF.Abs(player.velocity.Y) < 0.02f)
                    player.velocity.Y = 0;
            }

            float value = -player.velocity.Y / 5;
            value = MathHelper.Clamp(value, -1f, 1f);
            float value2 = player.velocity.X / MountData.dashSpeed;
            value2 = MathHelper.Clamp(value2, -1f, 1f);
            float num11 = -(float)Math.PI / 18f * value * player.direction;
            float num12 = (float)Math.PI / 18f * value2;
            float fullRotation3 = num11 + num12;
            if (MathF.Abs(fullRotation3) < 0.01f)
                fullRotation3 = 0;
            player.fullRotation = fullRotation3;
            player.fullRotationOrigin = new Vector2(player.width / 2, player.height * 3 / 4);

            player.fallStart = (int)(player.position.Y / 16f);//让你的坐骑拥有一个合理的坠落判定

            if (player.velocity.Length() > 2)
            {
                Dust d = Dust.NewDustPerfect(player.MountedCenter + new Vector2(0, 30) + Main.rand.NextVector2Circular(48, 12)
                       , DustID.Cloud, -player.velocity * Main.rand.NextFloat(0.1f, 0.3f), Scale: Main.rand.NextFloat(1f, 1.5f));
                d.noGravity = true;

                if (player.mount._frame == 0 && player.mount._frameCounter == 1)
                    Helpers.Helper.PlayPitched(CoraliteSoundID.Swing2_Item7, player.Center);
            }
        }

        public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
        {
            if (drawType == 0)
            {
                frame = new Rectangle(0, 0, 128, 96);
                rotation = -drawPlayer.fullRotation;
            }
            else if (drawType == 1)
            {
                //rotation = -drawPlayer.direction*drawPlayer.velocity.Length() / 10;
                frame = new Rectangle(0, 96 * drawPlayer.mount._frame, 128, 96);
            }
            else if (drawType == 2)
            {
                //rotation = -drawPlayer.direction*drawPlayer.velocity.Length() / 10;
                int frame2 = (int)Math.Clamp((Math.Abs(drawPlayer.velocity.X / 5f)) * 6, 0, 6);
                //Main.NewText(frame2);
                frame = new Rectangle(0, frame2 * 96, 128, 96);
            }

            drawOrigin = frame.Size() / 2;

            return true;
        }
    }

    public class ChalcedonySkyshipBuff : ModBuff
    {
        public override string Texture => AssetDirectory.MountBuffs + Name;

        public override void SetStaticDefaults()
        {
            BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(ModContent.MountType<ChalcedonySkyship>(), player);
            player.mount._flyTime = 320;
            player.mount._fatigue = 0;
        }
    }
}
