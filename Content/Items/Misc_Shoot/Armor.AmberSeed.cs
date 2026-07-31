using Coralite.Content.Items.Donator;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.Items.Misc_Shoot
{
    [PlayerEffect]
    [AutoloadEquip(EquipType.Head)]
    public class AmberSeed : ModItem
    {
        public override string Texture => AssetDirectory.Misc_Shoot + Name;

        public static LocalizedText bonus;

        public override void Load()
        {
            if (!Main.dedServ)
                bonus = this.GetLocalization("ArmorBonus");
        }

        public override void Unload()
        {
            bonus = null;
        }

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(silver: 30);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 1;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemID.FossilShirt && legs.type == ItemID.FossilPants;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = bonus.Value;
            
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.AddEffect(nameof(AmberSeed));
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Ranged) += 3;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FossilOre, 10)
                .AddIngredient(ItemID.Amber, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class AmberBullet : ModProjectile
    {
        public override string Texture => AssetDirectory.Misc_Shoot + Name;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 60 * 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (Main.rand.NextBool())
            {
                Projectile.SpawnTrailDust(DustID.GemAmber, Main.rand.NextFloat(-0.3f, -0.1f));
            }

            if (Projectile.alpha < 255)
            {
                Projectile.alpha += 20;
                if (Projectile.alpha > 255)
                    Projectile.alpha = 255;
            }

            Lighting.AddLight(Projectile.Center, new Vector3(0.25f, 0.21f, 0.05f));

            Projectile.rotation += Projectile.velocity.Length() / 60f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);

            return true;
        }

        public override void OnKill(int timeLeft)
        {
            Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.Zero);
            for (int i = 0; i < 12; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GemAmber, dir.RotateByRandom(-0.3f, 0.3f) * Main.rand.NextFloat(1.5f, 3f));
                d.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<AmberDebuff>(), 60*2);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.QuickDraw(Color.White * (Projectile.alpha / 255f), 0);

            return false;
        }
    }
}
