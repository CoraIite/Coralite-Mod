using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;

namespace Coralite.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Shoes)]
    public class GigantesShoes : BaseAccessory
    {
        public GigantesShoes() : base(ItemRarityID.Blue, Item.sellPrice(0, 2)) { }

        public Vector2 oldVelocity;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.defense = 2;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.fallDamageModifyer -= 0.5f;
            }

            if (!player.mount.Active && player.controlDown)
            {
                Vector2 oldVelocity = player.GetModPlayer<CoralitePlayer>().oldOldVelocity;
                if (oldVelocity.Y != 0 && player.velocity.Y == 0)
                {
                    Collision.HitTiles(player.BottomLeft + new Vector2(-64, 0), -Vector2.UnitY * 16, player.width + 64 * 2, 32);
                    SoundEngine.PlaySound(CoraliteSoundID.StaffOfEarth_Item69, player.Center);
                    var modifyer = new PunchCameraModifier(player.Center, -Vector2.UnitY, 4, 6, 6, 1000);
                    Main.instance.CameraModifiers.Add(modifyer);

                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC n = Main.npc[i];
                        if (n.CanBeChasedBy() && n.Distance(player.Center) < 120 && Collision.CanHit(player, n))
                        {
                            int dir = Math.Sign(n.Center.X - player.Center.X);
                            n.velocity += new Vector2(dir * 8, -3) * n.knockBackResist;
                        }
                    }
                }

                if (player.velocity.Y != 0)
                {
                    player.maxFallSpeed = 17;
                    player.velocity.Y += player.gravDir * 1.7f;
                }
            }
        }
    }
}
