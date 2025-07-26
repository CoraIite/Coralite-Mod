using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.ID;

namespace Coralite.Content.Items.Glistent
{
    public class GlistentJar : BaseFairyCatcher
    {
        public override string Texture => AssetDirectory.GlistentItems + Name;

        public override int CatchPower => 8;

        public override void SetOtherDefaults()
        {
            Item.shoot = ModContent.ProjectileType<GlistentJarProj>();
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = Item.useAnimation = 20;
            Item.shootSpeed = 12;
            Item.SetWeaponValues(20, 3);
            Item.SetShopValues(ItemRarityColor.Green2, Item.sellPrice(0, 1));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GlistentBar>(6)
                .AddIngredient(ItemID.ClayBlock, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class GlistentJarProj : BaseJarProj
    {
        public override string Texture => AssetDirectory.GlistentItems + "GlistentJar";

        public override void InitFields()
        {
            MaxFlyTime = 15;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 32;
        }

        public override void Load()
        {
            for (int i = 0; i < 3; i++)
                GoreLoader.AddGoreFromTexture<SimpleModGore>(Mod, Texture + "Proj_Gore" + i);
        }

        public override void SpawnDustOnFlying(bool outofTime)
        {
            Projectile.SpawnTrailDust(DustID.GemEmerald, Main.rand.NextFloat(0.1f, 0.3f));
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (int i = 0; i < 12; i++)
            {
                Dust d = Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(Projectile.getRect())
                    , DustID.GemEmerald, Helper.NextVec2Dir(1f, 2.5f), 150, Scale: Main.rand.NextFloat(1, 1.5f));
                d.noGravity = true;
            }

            this.SpawnGore(3);
            Helper.PlayPitched(CoraliteSoundID.GlassBroken_Shatter, Projectile.Center, pitchAdjust: -0.2f);
        }
    }

    public class GlistentJarDebuff:ModBuff
    {
        public override string Texture => AssetDirectory.Buffs+ "Debuff";

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {

        }
    }

    public class GlistentJarExplode : ModProjectile
    {
        public override string Texture => AssetDirectory.DefaultItem;

        /// <summary>
        /// 未1时命中敌怪造成debuff
        /// </summary>
        public ref float HasDebuff => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 128;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = CoraliteAssets.Halo.CircleSPA.Value;

            return false;
        }
    }
}
