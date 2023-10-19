﻿using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Nightmare
{
    public class GriefSeed : ModItem
    {
        public override string Texture => AssetDirectory.NightmareItems + Name;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[Type] = true;
            ItemID.Sets.OpenableBag[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.maxStack = 999;

            Item.consumable = true;
            Item.expert = true;

            Item.rare = ItemRarityID.Expert;
        }

        public override bool CanRightClick() => true;

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.Lerp(lightColor, Color.White, 0.4f);
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            //itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<EmperorSabre>()));
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<NightmarePlantera>()));
            //itemLoot.Add(ItemDropRule.Common(ItemID.Gel, 1, 45, 150));

            IItemDropRule[] weaponTypes = new IItemDropRule[] {
                ItemDropRule.Common(ModContent.ItemType<LostSevensideHook>(), 1, 1, 1),
                ItemDropRule.Common(ModContent.ItemType<DreamShears>(), 1, 1, 1),
                ItemDropRule.Common(ModContent.ItemType<EuphorbiaMilii>(), 1, 1, 1),

                ItemDropRule.Common(ModContent.ItemType<Lycoris>(), 1, 1, 1),
                //
                //

                ItemDropRule.Common(ModContent.ItemType<DevilsClaw>(), 1, 1, 1),
                ItemDropRule.Common(ModContent.ItemType<BarrenThornsStaff>(), 1, 1, 1),
                ItemDropRule.Common(ModContent.ItemType<Lullaby>(), 1, 1, 1),

                ItemDropRule.Common(ModContent.ItemType<PurpleToeStaff>(), 1, 1, 1),
                ItemDropRule.Common(ModContent.ItemType<Dreamcatcher>(), 1, 1, 1),
                ItemDropRule.Common(ModContent.ItemType<Eden>(), 1, 1, 1),
            };

            itemLoot.Add(new FewFromRulesRule(3, 1, weaponTypes));
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.White.ToVector3() * 0.4f);

            if (Item.timeSinceItemSpawned % 18 == 0)
            {
                Vector2 center = Item.Center + new Vector2(0f, Item.height * -0.1f);
                Vector2 direction = Main.rand.NextVector2CircularEdge(Item.width * 0.6f, Item.height * 0.6f);
                //float distance = 0.8f + Main.rand.NextFloat() * 0.2f;
                Vector2 velocity = new Vector2(0f, -Main.rand.NextFloat() * 0.3f - 1.5f);

                Dust dust = Dust.NewDustPerfect(center + direction, DustID.SilverFlame, velocity, newColor: new Color(150, 150, 150));
                dust.scale = 0.5f;
                dust.fadeIn = 1.1f;
                dust.noGravity = true;
                dust.noLight = true;
                dust.alpha = 0;
            }
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            Rectangle frame;

            if (Main.itemAnimations[Item.type] != null)
                frame = Main.itemAnimations[Item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]);
            else
                frame = texture.Frame();

            Vector2 frameOrigin = frame.Size() / 2f;
            Vector2 offset = new Vector2(Item.width / 2 - frameOrigin.X, Item.height - frame.Height);
            Vector2 drawPos = Item.position - Main.screenPosition + frameOrigin + offset;
            Vector2 effectDrawPos = drawPos + new Vector2(0, -4);
            float time = Main.GlobalTimeWrappedHourly;
            float timer = Item.timeSinceItemSpawned / 240f + time * 0.04f;

            time %= 4f;
            time /= 2f;

            if (time >= 1f)
                time = 2f - time;

            time = time * 0.5f + 0.5f;

            Vector2 mainSparkleScale = new Vector2(2f, 5f);
            //中心的闪光
            ProjectilesHelper.DrawPrettyStarSparkle(1, 0, effectDrawPos, NightmarePlantera.nightmareRed, NightmarePlantera.nightmareRed,
                0.5f + time * 0.1f, 0f, 0.5f, 0.5f, 1f, 0, mainSparkleScale, Vector2.One);

            //float rot2 = timer * 10f;
            //周围一圈小星星
            //for (int i = 0; i < 7; i++)
            //{
            //    Vector2 dir = (Main.GlobalTimeWrappedHourly * 1.5f + i * MathHelper.TwoPi / 7).ToRotationVector2();
            //    ProjectilesHelper.DrawPrettyStarSparkle(1, 0, effectDrawPos + dir * (32 - time * 8), Color.White * 0.7f, Color.Gray,
            //        0.5f + time * 0.2f, 0f, 0.5f, 0.5f, 1f, rot2, new Vector2(0.7f, 0.7f), Vector2.One*1.25f);
            //}

            //绘制额外旋转的星星，和上面叠起来变成一个
            ProjectilesHelper.DrawPrettyStarSparkle(1, 0, effectDrawPos, Color.White * 0.3f, NightmarePlantera.nightmareRed * 0.5f,
                0.5f - time * 0.3f, 0f, 0.5f, 0.5f, 1f, MathHelper.PiOver4, new Vector2(mainSparkleScale.Y * 0.75f), Vector2.One); ;

            //ProjectilesHelper.DrawPrettyStarSparkle(1, 0, drawPos, new Color(38, 104, 185) * 0.7f, new Color(158, 222, 255),
            //    time, 0, 0.3f, 0.7f, 1, timer * MathHelper.TwoPi, (timer * MathHelper.TwoPi).ToRotationVector2() * 4, Vector2.One);
            //ProjectilesHelper.DrawPrettyStarSparkle(1, 0, drawPos, new Color(38, 104, 185) * 0.7f, new Color(158, 222, 255),
            //    0.4f + time * 0.2f, 0, 0.3f, 0.7f, 1, -timer * MathHelper.Pi, new Vector2(2, 2), Vector2.One * 2);

            //ProjectilesHelper.DrawPrettyStarSparkle(1, 0, drawPos, new Color(38, 104, 185) * 0.7f, new Color(50, 152, 225),
            //    0.4f + time * 0.2f, 0, 0.5f, 0.5f, 1, 0, Vector2.One * 3, Vector2.One * 1.5f);

            float rot = -0.3f + MathF.Sin(timer * 3f) * 0.2f;
            for (float i = 0f; i < 1f; i += 0.25f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;
                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(50, 50, 50, 50), rot, frameOrigin, scale, SpriteEffects.None, 0);
            }
            //Main.NewText(time);
            spriteBatch.Draw(texture, drawPos, frame, lightColor, rot, frameOrigin, scale, SpriteEffects.None, 0);
            return false;
        }

    }
}
