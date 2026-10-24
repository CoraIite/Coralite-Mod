using Coralite.Content.CoraliteNotes.FlyingShieldChapter;
using Coralite.Content.ModPlayers;
using Coralite.Core.Systems.FlyingShieldSystem;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields.Accessories
{
    public class FlyingShieldTerminalChip : BaseFlyingShieldAccessory<FlyingShieldAccessoryPage1>, IFlyingShieldAccessory
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 21));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        }

        public FlyingShieldTerminalChip() : base(ItemRarityID.Cyan, Item.sellPrice(0, 5, 50))
        { }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.defense = 4;
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return !((equippedItem.type == ModContent.ItemType<HeavyWedges>()//下位
                || equippedItem.type == ModContent.ItemType<FlyingShieldCore>())//下位
                && incomingItem.type == ModContent.ItemType<FlyingShieldTerminalChip>());
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Melee) += 0.15f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.07f;
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.AddEffect(FlyingShieldEffects.FlyingShieldLRMeantime);
                cp.AddEffectHandler(this);
            }
        }

        public void OnInitialize(BaseFlyingShield projectile)
        {
            //projectile.shootSpeed *= 0.8f;
            //projectile.backSpeed *= 0.65f;

            Projectile p = projectile.Projectile;
            projectile.maxPuncture += 5;

            p.scale *= 1.25f;
            p.Resize((int)(p.width * p.scale), (int)(p.height * p.scale));

            projectile.trailWidth = (int)(projectile.trailWidth * p.scale);
        }

        public void PostInitialize(BaseFlyingShield projectile)
        {
            projectile.backTime = (int)(projectile.backTime * 0.7f);
            if (projectile.backTime < 1)
                projectile.backTime = 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<FlyingShieldCore>()
                .AddIngredient(ItemID.MartianConduitPlating, 30)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
