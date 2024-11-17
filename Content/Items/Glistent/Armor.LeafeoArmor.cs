using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Glistent
{
    [AutoloadEquip(EquipType.Head)]
    public class LeafeoHelmet : ModItem
    {
        public override string Texture => AssetDirectory.GlistentItems + Name;

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(silver: 5);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 1;
        }

        public override void UpdateEquip(Player player)
        {
            player.maxMinions += 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafStone>(8)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class LeafeoLightArmor : ModItem
    {
        public override string Texture => AssetDirectory.GlistentItems + Name;

        public static LocalizedText bonus;

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(silver: 10);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 3;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return head.type == ItemType<LeafeoHelmet>() && legs.type == ItemType<LeafeoBoots>();
        }

        public override void Load()
        {
            bonus = this.GetLocalization("ArmorBonus");
        }

        public override void Unload()
        {
            bonus = null;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = bonus.Value;
            if (player.HasBuff<LeafeoShieldCD>())
                return;

            if (player.ownedProjectileCounts[ProjectileType<LeafeoShield>()] < 1)
                Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ProjectileType<LeafeoShield>()
                    , 0, 0, player.whoAmI);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafStone>(14)
                .AddIngredient(ItemID.IronBar, 8)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class LeafeoBoots : ModItem
    {
        public override string Texture => AssetDirectory.GlistentItems + Name;

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(silver: 5);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 1;
        }

        public override void UpdateEquip(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.fallDamageModifyer -= 0.25f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafStone>(8)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class LeafeoShield : BaseHeldProj
    {
        public override string Texture => AssetDirectory.Halos + "Circle";

        private ParticleGroup particles = [];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            if (Owner.armor[1].type != ItemType<LeafeoLightArmor>()
                || !Owner.armor[1].ModItem.IsArmorSet(Owner.armor[0], Owner.armor[1], Owner.armor[2]))
            {
                Projectile.Kill();
                return;
            }

            Projectile.timeLeft = 2;
            Projectile.Center = Owner.MountedCenter + new Vector2(0, -8 + Owner.gfxOffY);
            Lighting.AddLight(Projectile.Center, (new Color(119, 133, 34) * 0.5f).ToVector3());

            if (Projectile.localAI[0] < 1)
                Projectile.localAI[0] += 0.1f;

            Projectile.ai[0]++;
            if (Projectile.ai[0] > 30)
            {
                Projectile.ai[0] = 0;
                for (int i = 0; i < Owner.buffType.Length; i++)
                {
                    int type = Owner.buffType[i];
                    int time = Owner.buffTime[i];
                    if (type == 0 && time == 0)
                        continue;

                    if (Main.debuff[type] && !BuffID.Sets.NurseCannotRemoveDebuff[type])
                    {
                        Owner.DelBuff(i);
                        Projectile.Kill();
                        break;
                    }
                }
            }

            SpawnParticles();
            particles.UpdateParticles();
        }

        public void SpawnParticles()
        {
            Projectile.ai[1]++;

            if (Projectile.ai[1] > 25)
            {
                Projectile.ai[1] = 0;
                if (Main.rand.NextBool())
                    return;

                var particle = Particle.NewPawticleInstance<LeafeoShieldParticle>(Projectile.Center, Vector2.Zero
                    , Color.White, Scale: Main.rand.NextFloat(0.6f, 1));
                particle.OwnerIndex = Projectile.whoAmI;
                particles.Add(particle);
            }
        }

        public override void OnKill(int timeLeft)
        {
            foreach (var p in particles)
            {
                (p as LeafeoShieldParticle).Fade();
                ParticleSystem.Particles.Add(p);
            }

            if (Owner.armor[1].type == ItemType<LeafeoLightArmor>()
                && Owner.armor[1].ModItem.IsArmorSet(Owner.armor[0], Owner.armor[1], Owner.armor[2]))
            {
                SoundEngine.PlaySound(CoraliteSoundID.Grass, Projectile.Center);
                Helper.PlayPitched(CoraliteSoundID.Derpling_NPCDeath25, Projectile.Center, pitch: -0.5f);
                Owner.AddBuff(BuffType<LeafeoShieldCD>(), 60 * 30);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Color c = new Color(119, 133, 34, 0) * 0.75f * Projectile.localAI[0];
            Color c2 = new Color(87, 74, 36, 0) * 0.75f * Projectile.localAI[0];

            Projectile.QuickDraw(c, 0.3f, 0f);
            Projectile.QuickDraw(c2, 0.25f, 0f);

            particles?.DrawParticles(Main.spriteBatch);

            return false;
        }
    }

    public class LeafeoShieldParticle : LeafParticle
    {
        private float start;
        private float length;
        private float speed;
        public int OwnerIndex;

        public int State;

        public override void SetProperty()
        {
            frameCounterMax = 6;
            start = Main.rand.NextFloat(6.282f);
            fadeIn = Main.rand.NextFloat(12, 20);
            length = Main.rand.NextFloat(16, 30);
            speed = Main.rand.NextFloat(0.02f, 0.06f);
            LeafType = Main.rand.Next(2);
            shouldKilledOutScreen = false;
        }

        public override void AI()
        {
            fadeIn -= speed;
            length += speed;

            UpdateFrame();

            switch (State)
            {
                default:
                case 0://在弹幕周围环绕
                    {
                        if (!OwnerIndex.GetProjectileOwner(out Projectile owner, () => active = false))
                            return;

                        if (alpha < 1)
                            alpha += 0.05f;

                        Position = owner.Center + ((fadeIn + start).ToRotationVector2() * length);
                        Rotation = fadeIn + start - 1.57f + (LeafType == 0 ? 1.57f : 0);

                        if (fadeIn < 0.5f)
                            color *= 0.93f;
                    }
                    break;
                case 1://树叶掉落
                    {
                        Rotation = Rotation.AngleLerp(LeafType == 0 ? 1.57f : 0, 0.1f);

                        Velocity.X *= 0.97f;
                        if (Velocity.Y < 8)
                            Velocity.Y += 0.25f;

                        if (Collision.SolidCollision(Position - (Vector2.One * 5f), 10, 10))
                        {
                            if (fadeIn > 0.5f)
                                fadeIn = 0.5f;
                            Velocity *= 0.25f;
                        }

                        if (fadeIn < 0.5f)
                            color.A = (byte)(color.A * 0.9f);
                    }
                    break;
            }

            if (fadeIn < 0)
                active = false;
        }

        public void Fade()
        {
            State = 1;
            Velocity = (fadeIn + 1.57f).ToRotationVector2() * Main.rand.NextFloat(2, 5);
            color = new Color(119, 133, 34, 255);
            alpha = 1;
        }
    }

    public class LeafeoShieldCD : ModBuff
    {
        public override string Texture => AssetDirectory.Debuffs + Name;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
    }
}
