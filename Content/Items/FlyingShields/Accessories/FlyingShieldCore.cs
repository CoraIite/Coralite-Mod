using Coralite.Content.CoraliteNotes.FlyingShieldChapter;
using Coralite.Content.ModPlayers;
using Coralite.Core.Systems.FlyingShieldSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields.Accessories
{
    public class FlyingShieldCore : BaseFlyingShieldAccessory<FlyingShieldAccessoryPage1>, IFlyingShieldAccessory
    {
        public FlyingShieldCore() : base(ItemRarityID.Pink, Item.sellPrice(0, 2, 50))
        { }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.defense = 4;
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return !((equippedItem.type == ModContent.ItemType<HeavyWedges>()//下位
                || equippedItem.type == ModContent.ItemType<FlyingShieldTerminalChip>())//上位
                && incomingItem.type == ModContent.ItemType<FlyingShieldCore>());
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Melee) += 0.15f;
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.AddEffect(FlyingShieldEffects.FlyingShieldLRMeantime);
                cp.AddEffectHandler(this);
            }
        }

        public void OnInitialize(BaseFlyingShield projectile)
        {
            //projectile.shootSpeed *= 0.6f;
            //projectile.backSpeed *= 0.65f;

            Projectile p = projectile.Projectile;
            projectile.maxPuncture += 3;

            p.scale *= 1.2f;
            p.Resize((int)(p.width * p.scale), (int)(p.height * p.scale));

            projectile.trailWidth = (int)(projectile.trailWidth * p.scale);
        }

        public void PostInitialize(BaseFlyingShield projectile)
        {
            projectile.backTime = (int)(projectile.backTime * 0.8f);
            if (projectile.backTime < 1)
                projectile.backTime = 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<HeavyWedges>()
                .AddIngredient(ItemID.WarriorEmblem)
                .AddIngredient(ItemID.SoulofFright)
                .AddIngredient(ItemID.SoulofSight)
                .AddIngredient(ItemID.SoulofMight)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
