using Coralite.Content.Bosses.BabyIceDragon;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Icicle
{
    [AutoloadEquip(EquipType.Head)]
    public class IcicleHelmet : ModItem, IControllableArmorBonus
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

        public static LocalizedText bonus;
        private int attType;

        public override void Load()
        {
            bonus = this.GetLocalization("ArmorBonus", () => "按下套装奖励键使用冰晶吐息或冰刺\n套装奖励键可在模组配置中更改");
        }

        public override void Unload()
        {
            bonus = null;
        }

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 6;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<IcicleBreastplate>() && legs.type == ItemType<IcicleLegs>();
        }

        public override void UpdateEquip(Player player)
        {
            player.GetAttackSpeed(DamageClass.Generic) += 0.04f;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = bonus.Value;
            player.resistCold = true;
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowLokis = true;
            player.armorEffectDrawOutlinesForbidden = true;
        }

        public void UseArmorBonus(Player player)
        {
            if (!player.HasBuff<IcicleArmorBonus>())
            {
                if (attType == 0)
                {
                    SpawnIceBreath(player);
                }
                else
                {
                    attType = 0;
                    SpawnIceThorns(player);
                }
            }
        }

        public void SpawnIceBreath(Player player)
        {
            attType = 1;
            SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath, player.Center);
            Vector2 targetDir = (Main.MouseWorld + Main.rand.NextVector2CircularEdge(30, 30) - player.Center).SafeNormalize(Vector2.Zero);
            Vector2 pos = player.MountedCenter;
            int j = 0;
            if (Main.netMode != NetmodeID.MultiplayerClient)
                for (int i = -1; i < 1; i++)
                {
                    int damage = 65;
                    int index = Projectile.NewProjectile(player.GetSource_FromThis(), pos, targetDir.RotatedBy(i * 0.05f) * (6f + (j * 2)), ProjectileType<IceBreath>(), damage, 5f);
                    Projectile p = Main.projectile[index];
                    p.DamageType = DamageClass.Magic;
                    p.hostile = false;
                    p.friendly = true;
                    p.penetrate = 3;
                    p.usesIDStaticNPCImmunity = true;
                    p.idStaticNPCHitCooldown = 10;
                    p.timeLeft = 60;
                    j++;
                }

            player.AddBuff(BuffType<IcicleArmorBonus>(), 60 * 10);

        }

        public void SpawnIceThorns(Player player)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            bool spawn = false;
            Point sourceTileCoords = player.Bottom.ToTileCoordinates();
            for (int i = 0; i < 5; i++)
            {
                bool temp = TryMakingSpike(player, ref sourceTileCoords, player.direction, 20, i * 6, 1, i * 0.12f);
                if (temp)
                    spawn = true;
                sourceTileCoords.X += player.direction * 2;
            }

            if (spawn)
            {
                PunchCameraModifier modifier = new(player.Center, new Vector2(0f, 1f), 10f, 6f, 15, 1000f, "BabyIceDragon");
                Main.instance.CameraModifiers.Add(modifier);
                player.AddBuff(BuffType<IcicleArmorBonus>(), 60 * 10);
            }
            else
                SpawnIceBreath(player);
        }

        private bool TryMakingSpike(Player player, ref Point sourceTileCoords, int dir, int howMany, int whichOne, int xOffset, float scaleOffset)
        {
            int position_X = sourceTileCoords.X + (xOffset * dir);
            int position_Y = TryMakingSpike_FindBestY(player.Bottom, ref sourceTileCoords, position_X);
            if (WorldGen.ActiveAndWalkableTile(position_X, position_Y))
            {
                Vector2 position = new((position_X * 16) + 8, (position_Y * 16) - 8);
                Vector2 velocity = new Vector2(0f, -1f).RotatedBy(whichOne * dir * 0.7f * ((float)Math.PI / 4f / howMany));
                int damage = 80;
                int index = Projectile.NewProjectile(player.GetSource_FromAI(), position, velocity, ProjectileID.DeerclopsIceSpike, damage, 0f, player.whoAmI, 0f, 0.4f + scaleOffset + (xOffset * 1.1f / howMany));
                Projectile p = Main.projectile[index];
                p.DamageType = DamageClass.Melee;
                p.hostile = false;
                p.friendly = true;
                p.penetrate = -1;
                p.usesIDStaticNPCImmunity = true;
                p.idStaticNPCHitCooldown = 10;
                return true;
            }

            return false;
        }

        private static int TryMakingSpike_FindBestY(Vector2 vector, ref Point sourceTileCoords, int x)
        {
            int position_Y = sourceTileCoords.Y;
            int num2 = (int)(vector.Y / 16f);
            int num3 = Math.Sign(num2 - position_Y);
            int num4 = num2 + (num3 * 15);
            int? num5 = null;
            float num6 = float.PositiveInfinity;
            for (int i = position_Y; i != num4; i += num3)
            {
                if (WorldGen.ActiveAndWalkableTile(x, i))
                {
                    float num7 = new Point(x, i).ToWorldCoordinates().Distance(vector);
                    if (!num5.HasValue || !(num7 >= num6))
                    {
                        num5 = i;
                        num6 = num7;
                    }
                }
            }

            if (num5.HasValue)
                position_Y = num5.Value;

            for (int j = 0; j < 8; j++)
            {
                if (position_Y < 10)
                    break;

                if (!WorldGen.SolidTile(x, position_Y))
                    break;

                position_Y--;
            }

            for (int k = 0; k < 8; k++)
            {
                if (position_Y > Main.maxTilesY - 10)
                    break;

                if (WorldGen.ActiveAndWalkableTile(x, position_Y))
                    break;

                position_Y++;
            }

            return position_Y;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IcicleCrystal>()
                .AddIngredient<IcicleBreath>(12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class IcicleBreastplate : ModItem
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 7;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetAttackSpeed(DamageClass.Generic) += 0.06f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IcicleCrystal>()
                .AddIngredient<IcicleScale>(12)
                .AddTile(TileID.Anvils)
                .Register();
        }

    }

    [AutoloadEquip(EquipType.Legs)]
    public class IcicleLegs : ModItem
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 6;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetAttackSpeed(DamageClass.Generic) += 0.04f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IcicleCrystal>()
                .AddIngredient<IcicleScale>(8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class IcicleArmorBonus : ModBuff
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.coldDamageBonus += 0.15f;
            }
        }
    }
}
