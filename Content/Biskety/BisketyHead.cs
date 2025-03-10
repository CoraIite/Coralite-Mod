using Coralite.Content.Biskety.UI;
using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.Biskety
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

        public static void SendKillBiskety(Mod mod)
        {
            if (VaultUtils.isSinglePlayer)
            {
                KillBiskety();
            }
            else
            {
                var netMessage = mod.GetPacket();
                netMessage.Write((byte)CoraliteNetWorkEnum.KillBiskety);
                netMessage.Send();
            }
        }

        public static void KillBiskety()
        {
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.type != ModContent.NPCType<Biskety>())
                {
                    continue;
                }

                npc.Kill();
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                }
            }
        }

        public static void SendSpawnBiskety(Player player, Item item, Mod mod)
        {
            Vector2 spanPos = new Vector2((int)Main.MouseWorld.X - Biskety.Width / 2, (int)Main.MouseWorld.Y - Biskety.Height / 2);
            if (VaultUtils.isSinglePlayer)
            {
                NPC.NewNPC(new EntitySource_ItemUse(player, item), (int)spanPos.X, (int)spanPos.Y, ModContent.NPCType<Biskety>(), Target: player.whoAmI);
            }
            else
            {
                var netMessage = mod.GetPacket();
                netMessage.Write((byte)CoraliteNetWorkEnum.SpawnBiskety);
                netMessage.WriteVector2(spanPos);
                netMessage.Send();
            }
        }

        public static void SpawnBiskety(BinaryReader reader, int whoAmI)
        {
            if (!VaultUtils.isServer)
            {
                return;
            }

            Vector2 spanPos = reader.ReadVector2();
            NPC.NewNPC(new EntitySource_WorldEvent(), (int)spanPos.X, (int)spanPos.Y, ModContent.NPCType<Biskety>(), Target: whoAmI);
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                SendKillBiskety(Mod);
                return true;
            }
            SendSpawnBiskety(player, Item, Mod);
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

        public float Timer { get; set; } = -1;
        public float TotalTime { get; set; }

        private int itemDamageRecorder;
        private int projectileDamageRecorder;
        internal const int Height = 38;
        internal const int Width = 48;

        public static LocalizedText[] DamageShowTexts { get; private set; }

        public override void Load()
        {
            if (Main.dedServ)
                return;

            DamageShowTexts = [
                this.GetLocalization("TotalDamage"),
                this.GetLocalization("ProjectileDamage"),
                this.GetLocalization("MeleeDamage"),
                this.GetLocalization("OtherDamage"),
                this.GetLocalization("LongTimeDPS"),

                ];
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            DamageShowTexts = null;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 10_0000_0000;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;

            NPC.width = Width;
            NPC.height = Width;

            NPC.defense = BisketyDefenceController.Defence;
        }

        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            modifiers.DamageVariationScale *= 0;
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
                if (Timer > 60 * 3)
                {
                    TotalTime = 0;
                    Timer = -1;
                    itemDamageRecorder = 0;
                    projectileDamageRecorder = 0;
                    NPC.life = NPC.lifeMax;
                }
            }

            if (NPC.target >= 0 && NPC.target < 255)
            {
                NPC.spriteDirection = Main.npc[NPC.target].Center.X > NPC.Center.X ? -1 : 1;
            }
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

                Vector2 position = pos + new Vector2(0, 40 + howMany * 20);
                int totalDamage = NPC.lifeMax - NPC.life;

                if (BisketyController.ShowFlags[(int)BisketyController.ShowType.ShowTotalDamage])
                    Utils.DrawBorderString(spriteBatch, $"{DamageShowTexts[0].Value}{totalDamage}", position, Color.White, anchorx: 0.5f, anchory: 0.5f);

                if (BisketyController.ShowFlags[(int)BisketyController.ShowType.ShowProjectileDamage])
                {
                    position += new Vector2(0, 20);
                    Utils.DrawBorderString(spriteBatch, $"{DamageShowTexts[1].Value}{projectileDamageRecorder}", position, Color.Pink, anchorx: 0.5f, anchory: 0.5f);
                }
                if (BisketyController.ShowFlags[(int)BisketyController.ShowType.ShowItemDamage])
                {
                    position += new Vector2(0, 20);
                    Utils.DrawBorderString(spriteBatch, $"{DamageShowTexts[2].Value}{itemDamageRecorder}", position, Color.LightBlue, anchorx: 0.5f, anchory: 0.5f);
                }
                if (BisketyController.ShowFlags[(int)BisketyController.ShowType.ShowOtherDamage])
                {
                    position += new Vector2(0, 20);
                    Utils.DrawBorderString(spriteBatch, $"{DamageShowTexts[3].Value}{totalDamage - projectileDamageRecorder - itemDamageRecorder}", position, Color.LightCoral, anchorx: 0.5f, anchory: 0.5f);
                }
                if (BisketyController.ShowFlags[(int)BisketyController.ShowType.ShowTotalDPS])
                {
                    position += new Vector2(0, 20);
                    Utils.DrawBorderString(spriteBatch, $"{DamageShowTexts[4].Value}{MathF.Round(60 * totalDamage / TotalTime, 2)}", position, Color.LightGoldenrodYellow, anchorx: 0.5f, anchory: 0.5f);
                }
            }

            Main.spriteBatch.Draw(mainTex, pos, null, Color.White, 0, origin, NPC.scale
                , NPC.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

            return false;
        }
    }
}
