using Coralite.Content.Items.Donator;
using Coralite.Content.Items.FlyingShields;
using Coralite.Content.ModPlayers;
using Coralite.Content.Particles;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Helpers;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.MagikeSeries1
{
    [AutoloadEquip(EquipType.Head)]
    public class BasaltHelmet : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries1Item + Name;

        public override void SetDefaults()
        {
            Item.SetShopValues(ItemRarityColor.White0, Item.sellPrice(0, 0, 10));
            Item.rare = RarityType<MagicCrystalRarity>();
            Item.defense = 4;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class BasaltBreastplate : ModItem, IFlyingShieldAccessory
    {
        public override string Texture => AssetDirectory.MagikeSeries1Item + Name;

        public static LocalizedText bonus;

        public override void Load()
        {
            bonus = this.GetLocalization("ArmorBonus");
        }

        public override void Unload()
        {
            bonus = null;
        }

        public override void SetDefaults()
        {
            Item.SetShopValues(ItemRarityColor.White0, Item.sellPrice(0, 0, 10));
            Item.rare = RarityType<MagicCrystalRarity>();
            Item.defense = 5;
        }

        public override void UpdateEquip(Player player)
        {
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<BasaltBreastplate>() && legs.type == ItemType<BasaltLegs>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = bonus.Value;
            player.GetDamage(DamageClass.Melee).Flat += 4;
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.FlyingShieldAccessories?.Add(this);
            }
        }

        public void OnGuardInitialize(BaseFlyingShieldGuard projectile)
        {
            projectile.parryTime = 6;
        }

        public bool OnParry(BaseFlyingShieldGuard projectile)
        {
            Player Owner = projectile.Owner;
            Projectile Projectile = projectile.Projectile;

            if (Owner.TryGetModPlayer(out CoralitePlayer cp))
            {
                if (cp.parryTime < 100)
                {
                    Owner.AddImmuneTime(ImmunityCooldownID.General, 15);
                    Owner.immune = true;
                }

                if (cp.parryTime < 250)
                    cp.parryTime += 80;
            }

            float projSpeed = 7;
            float particleBaseSpeed = 2f;
            float particleSpeedAdder = 0.7f;
            int damage = 2;

            Helper.PlayPitched(CoraliteSoundID.IceMagic_Item28, Projectile.Center, pitch: -0.5f);
            Helper.PlayPitched(CoraliteSoundID.NoUse_SuperMagicShoot_Item68, Projectile.Center, pitch: 0.7f);

            LightCiecleParticle.Spawn(Projectile.Center, Coralite.MagicCrystalPink * 0.5f, 0.2f, Projectile.rotation
                , new Vector2(0.25f + Main.rand.NextFloat(-0.1f, 0.1f), 0.6f), new Color(108, 19, 58, 0));

            float rot;

            Color c = Coralite.MagicCrystalPink;
            for (int i = 0; i < 8; i++)
            {
                float angle = 0.45f - i * 0.4f / 8;
                rot = Main.rand.NextFloat(-angle, angle);

                Color color = i == 7 ? c : Main.rand.NextFromList(c, new Color(211, 103, 156), Color.Pink);
                MagikeFlowLine.Spawn(Projectile.Center, (Projectile.rotation + rot).ToRotationVector2() * (particleBaseSpeed + i * particleSpeedAdder)
                    , Main.rand.Next(1, 3), Main.rand.Next(14, 25), Main.rand.Next(8, 16), Main.rand.NextFloat(0.4f, 0.6f)
                    , color);
            }

            for (int i = -1; i < 2; i++)
            {
                rot = i * 0.4f + Main.rand.NextFloat(-0.2f, 0.2f);
                LightShotParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(8, 8), Color.DeepPink, rot + Projectile.rotation
                    , new Vector2(Main.rand.NextFloat(0.4f, 0.7f) * Helper.EllipticalEase(rot, 1, 0.4f)
                    , 0.03f));
            }

            if (Projectile.IsOwnedByLocalPlayer())
                Projectile.NewProjectileFromThis<PlatinumMagshieldParry>(Projectile.Center, Projectile.rotation.ToRotationVector2() * projSpeed
                    , Projectile.damage * damage, 6);
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(24)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class BasaltLegs : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries1Item + Name;

        public override void SetDefaults()
        {
            Item.SetShopValues(ItemRarityColor.White0, Item.sellPrice(0, 0, 10));
            Item.rare = RarityType<MagicCrystalRarity>();
            Item.defense = 4;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.05f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
