using Coralite.Content.Items.BotanicalItems.Seeds;
using Coralite.Content.Items.RedJadeItems;
using Coralite.Content.Projectiles.RedJadeProjectiles;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.ModPlayers
{
    public class CoralitePlayer : ModPlayer
    {
        public bool ownedYujianProj;
        public float Nianli;
        public float NianliMax = 100f;
        public float NianliRegain = 1f;

        public short RightClickReuseDelay = 0;

        public bool RedJadePendant;

        #region 各种更新

        public override void PreUpdate()
        {
            NianliRegain = 1f;
            NianliMax = 100f;
        }

        public override void PostUpdateEquips()
        {
            Nianli += NianliRegain;
            Nianli = Math.Clamp(Nianli, 0f, NianliMax);
        }

        public override void PostUpdate()
        {
            RedJadePendant = false;
            if (RightClickReuseDelay > 0)
                RightClickReuseDelay--;

            Nianli = Math.Clamp(Nianli, 0f, NianliMax);  //只是防止意外发生
        }

        #endregion

        #region 受击与攻击

        public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
        {
            if (RedJadePendant && damage > 5 && Main.rand.NextBool(3))
            {
                Projectile.NewProjectile(Player.GetSource_Accessory(Player.armor.First((item) => item.type == ItemType<RedJadePendant>())),
                    Player.Center + (proj.Center - Player.Center).SafeNormalize(Vector2.One) * 16, Vector2.Zero, ProjectileType<RedJadeBoom>(), 80, 8f, Player.whoAmI);
            }
        }

        public override void OnHitByNPC(NPC npc, int damage, bool crit)
        {
            if (RedJadePendant && damage > 5 && Main.rand.NextBool(3))
            {
                Projectile.NewProjectile(Player.GetSource_Accessory(Player.armor.First((item) => item.type == ItemType<RedJadePendant>())),
                    Player.Center + (npc.Center - Player.Center).SafeNormalize(Vector2.One) * 16, Vector2.Zero, ProjectileType<RedJadeBoom>(), 80, 8f, Player.whoAmI);
            }
        }

        #endregion

        #region 钓鱼系统

        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            bool inWater = !attempt.inLava && !attempt.inHoney;

            if (inWater && Player.ZoneBeach && attempt.common && !attempt.crate)
            {
                if (Main.rand.NextBool(15))
                    itemDrop = ModContent.ItemType<NacliteSeedling>();
            }
        }

        #endregion
    }
}
