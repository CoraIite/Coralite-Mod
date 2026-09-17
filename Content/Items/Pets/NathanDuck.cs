using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using System;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace Coralite.Content.Items.Pets
{
    public class NathanDuck : ModItem
    {
        public override string Texture => AssetDirectory.PetItems + Name;

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ModContent.ProjectileType<NathanDuckProj>(), ModContent.BuffType<NathanDuckBuff>());
            Item.width = 28;
            Item.height = 20;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(0, 5);
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 15, true, false);
            }
        }
    }

    public class NathanDuckBuff : ModBuff
    {
        public override string Texture => AssetDirectory.PetBuffs + Name;

        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;

            int projType = ModContent.ProjectileType<NathanDuckProj>();
            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[projType] <= 0)
            {
                var entitySource = player.GetSource_Buff(buffIndex);
                Projectile.NewProjectile(entitySource, player.Center, Vector2.Zero, projType, 0, 0f, player.whoAmI);
            }
        }
    }

    public class NathanDuckProj : BasePetProj
    {
        public override string Texture => AssetDirectory.PetItems + Name;
        protected override int PetBuffType => ModContent.BuffType<NathanDuckBuff>();

        /// <summary> 0：走地，1：飞行，2：特殊待机动画 </summary>
        public ref float State => ref Projectile.ai[0];

        protected override void SetPetStaticDefaults()
        {
            // 暂用单帧，待贴图确定后补充动画帧数。
            Main.projFrames[Type] = 1;
        }

        protected override void SetPetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 34;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.timeLeft = 100;
            Projectile.friendly = false;
            Projectile.tileCollide = true;
            Projectile.decidesManualFallThrough = true;
        }

        public override bool? CanDamage() => false;

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active)
            {
                Projectile.active = false;
                return;
            }

            CheckActive(player);

            if (TeleportToOwner(player))
                Projectile.netUpdate = true;

            Projectile.shouldFallThrough = player.Bottom.Y - 12f > Projectile.Bottom.Y;

            switch (State)
            {
                case 0: // 走地
                    AI_Walking(player);
                    break;
                case 1: // 飞行
                    AI_Flying(player);
                    break;
                case 2: // 特殊待机动画，暂留空
                    break;
            }
        }

        private void AI_Walking(Player player)
        {
            Vector2 toPlayer = player.Center - Projectile.Center;
            if (toPlayer.Length() > 500f || Math.Abs(toPlayer.Y) > 200f)
            {
                State = 1;
                Projectile.tileCollide = false;
                Projectile.netUpdate = true;
                return;
            }

            GroundMovement(player.Center, maxSpeed: 6f,
                followOffset: new Vector2(-40f * player.direction, 0f));
        }

        private void AI_Flying(Player player)
        {
            if (FlyMovement(player))
            {
                State = 0;
                Projectile.tileCollide = true;
                Projectile.rotation = 0f;
                Projectile.netUpdate = true;
            }
        }
    }
}
