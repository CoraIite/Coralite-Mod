using Coralite.Content.UI;
using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.Misc
{
    public class BisketyHead : ModItem
    {
        public override string Texture => AssetDirectory.MiscItems + Name;

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Pink;
            Item.useTime = Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.HoldUp;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && npc.type == ModContent.NPCType<Biskety>())
                        npc.Kill();
                }

                return true;
            }

            NPC.NewNPC(new EntitySource_ItemUse(player, Item), (int)player.Center.X, (int)player.Bottom.Y,
                ModContent.NPCType<Biskety>(), Target: player.whoAmI);
            return base.CanUseItem(player);
        }

        public override bool CanRightClick() => true;
        public override bool ConsumeItem(Player player) => false;

        public override void RightClick(Player player)
        {
            BisketyController.visible = true;
            UILoader.GetUIState<BisketyController>().Recalculate();
        }
    }

    public class Biskety : ModNPC
    {
        public override string Texture => AssetDirectory.MiscItems + Name;

        ref float Timer => ref NPC.ai[0];
        ref float TotalTime => ref NPC.ai[1];

        int itemDamageRecorder;
        int projectileDamageRecorder;

        public override void SetDefaults()
        {
            NPC.lifeMax = 10_0000_0000;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;

            NPC.width = 38;
            NPC.height = 48;
        }

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            Timer = 0;
            itemDamageRecorder += damageDone;
            SoundStyle st = Main.rand.Next(3) switch
            {
                0 => CoraliteSoundID.TargetDummy_NPCHit15,
                1 => CoraliteSoundID.TargetDummy2_NPCHit16,
                _ => CoraliteSoundID.TargetDummy3_NPCHit17
            };
            SoundEngine.PlaySound(st, NPC.Center);
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            Timer = 0;
            projectileDamageRecorder += damageDone;
            SoundStyle st = Main.rand.Next(3) switch
            {
                0 => CoraliteSoundID.TargetDummy_NPCHit15,
                1 => CoraliteSoundID.TargetDummy2_NPCHit16,
                _ => CoraliteSoundID.TargetDummy3_NPCHit17
            };
            SoundEngine.PlaySound(st, NPC.Center);
        }

        public override void AI()
        {
            if (Timer >= 0)//计时器大于某个值时才会记录
            {
                TotalTime++;
                Timer++;
                if (Timer > 60 * 5)
                {
                    TotalTime = 0;
                    Timer = -1;
                    itemDamageRecorder = 0;
                    projectileDamageRecorder = 0;
                    NPC.life = NPC.lifeMax;
                }
            }

            NPC.spriteDirection = Main.LocalPlayer.Center.X > NPC.Center.X ? -1 : 1;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = NPC.GetTexture();
            Vector2 pos = NPC.Center - Main.screenPosition;
            var origin = mainTex.Size() / 2;

            if (Timer >= 0)
            {
                int howMany = 0;
                for (int i = 0; i < BisketyController.ShowFlags.Length; i++)
                    if (BisketyController.ShowFlags[i])
                        howMany++;

                Vector2 position = pos - new Vector2(0, 40 + (howMany * 20));
                int totalDamage = NPC.lifeMax - NPC.life;
                if (BisketyController.ShowFlags[(int)BisketyController.ShowType.ShowTotalDamage])
                    Utils.DrawBorderString(spriteBatch, $"总伤害：{totalDamage}", position, Color.White, anchorx: 0.5f, anchory: 0.5f);
                if (BisketyController.ShowFlags[(int)BisketyController.ShowType.ShowProjectileDamage])
                {
                    position += new Vector2(0, 20);
                    Utils.DrawBorderString(spriteBatch, $"弹幕伤害：{projectileDamageRecorder}", position, Color.Pink, anchorx: 0.5f, anchory: 0.5f);
                }
                if (BisketyController.ShowFlags[(int)BisketyController.ShowType.ShowItemDamage])
                {
                    position += new Vector2(0, 20);
                    Utils.DrawBorderString(spriteBatch, $"玩家近战伤害：{itemDamageRecorder}", position, Color.LightBlue, anchorx: 0.5f, anchory: 0.5f);
                }
                if (BisketyController.ShowFlags[(int)BisketyController.ShowType.ShowOtherDamage])
                {
                    position += new Vector2(0, 20);
                    Utils.DrawBorderString(spriteBatch, $"其他来源伤害：{totalDamage - projectileDamageRecorder - itemDamageRecorder}", position, Color.LightCoral, anchorx: 0.5f, anchory: 0.5f);
                }
                if (BisketyController.ShowFlags[(int)BisketyController.ShowType.ShowTotalDPS])
                {
                    position += new Vector2(0, 20);
                    Utils.DrawBorderString(spriteBatch, $"长期DPS：{MathF.Round(60 * totalDamage / TotalTime, 2)}", position, Color.LightGoldenrodYellow, anchorx: 0.5f, anchory: 0.5f);
                }
            }

            Main.spriteBatch.Draw(mainTex, pos, null, Color.White, 0, origin, NPC.scale
                , NPC.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

            return false;
        }
    }
}
