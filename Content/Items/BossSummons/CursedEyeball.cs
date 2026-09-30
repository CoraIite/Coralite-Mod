using Coralite.Content.Bosses.VanillaReinforce.EoC;
using Coralite.Core;
using Coralite.Core.Systems.ItemTransform;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace Coralite.Content.Items.BossSummons
{
    public class CursedEyeball : ModItem
    {
        public override string Texture => AssetDirectory.Vanilla + "Item_43";

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;

            NPCID.Sets.MPAllowedEnemies[ModContent.NPCType<EyeOfCthulhu>()] = true;
            ItemTransformSystem.RegisterToTransformGroup(Type, ItemID.SuspiciousLookingEye);
        }

        public override void SetDefaults()
        {
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 0, 1, 0);
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ItemRarityID.Green;
            Item.consumable = true;
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.dayTime && !NPC.AnyNPCs(ModContent.NPCType<EyeOfCthulhu>());
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(SoundID.Roar, player.position);

                int type = ModContent.NPCType<EyeOfCthulhu>();

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.SpawnOnPlayer(player.whoAmI, type);
                else
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
            }

            return true;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Helpers.Helper.GetItemTexAndFrame(Item.type, out Texture2D tex, out Rectangle frameBox);
            Vector2 pos = Item.Bottom + new Vector2(0, -frameBox.Height / 2) - Main.screenPosition;

            for (int i = 0; i < 5; i++)
            {
                Main.spriteBatch.Draw(tex, pos + (Main.GlobalTimeWrappedHourly + i * MathHelper.TwoPi / 5).ToRotationVector2() * 3, frameBox, new Color(200, 90, 255, 0), rotation, frameBox.Size() / 2, scale, 0, 0);
            }

            return base.PreDrawInWorld(spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Helpers.Helper.GetItemTexAndFrame(Item.type, out Texture2D tex, out Rectangle frameBox);
            Vector2 pos = position + new Vector2(0, 0);

            for (int i = 0; i < 5; i++)
            {
                Main.spriteBatch.Draw(tex, pos + (Main.GlobalTimeWrappedHourly + i * MathHelper.TwoPi / 5).ToRotationVector2() * 3, frameBox, new Color(200, 90, 255, 0), 0, frameBox.Size() / 2, scale, 0, 0);
            }

            return base.PreDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }
    }
}
