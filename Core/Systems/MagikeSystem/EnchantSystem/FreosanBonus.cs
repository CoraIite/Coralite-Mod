//using Coralite.Content.ModPlayers;
//using Terraria;
//using Terraria.Audio;
//using Terraria.DataStructures;
//using Terraria.ID;
//using Terraria.Localization;

//namespace Coralite.Core.Systems.MagikeSystem.EnchantSystem
//{
//    public class SpecialEnchant_FreosanBonus : SpecialEnchant
//    {
//        public SpecialEnchant_FreosanBonus(MagikeEnchant.Level level, float bonus0, float bonus1, float bonus2) : base(level, bonus0, bonus1, bonus2)
//        {
//        }

//        public override void Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
//        {
//            if (player.TryGetModPlayer(out MagikePlayer mp) && mp.SpecialEnchantCD <= 0)
//            {
//                float rot = Main.rand.NextFloat(3.141f, 6.282f);
//                Projectile.NewProjectile(source, player.Center + (rot.ToRotationVector2() * 48), Vector2.Zero, ModContent.ProjectileType<FreosanProj>()
//                    , (int)(damage * 0.2f), 2, player.whoAmI);
//                mp.SpecialEnchantCD = 30;
//            }
//        }

//        public override string Description => Language.GetOrRegister($"Mods.Coralite.Systems.MagikeSystem.FreosanBonus", () => "冻：攻击时生成冰块").Value;
//    }

//    public class FreosanProj : ModProjectile
//    {
//        public override string Texture => AssetDirectory.MagikeProjectiles + Name;

//        public override void SetStaticDefaults()
//        {
//            Main.projFrames[Type] = 3;
//        }

//        public override void SetDefaults()
//        {
//            Projectile.alpha = 255;
//            Projectile.width = 30;
//            Projectile.height = 28;
//            Projectile.friendly = true;
//            Projectile.timeLeft = 120;

//            Projectile.usesIDStaticNPCImmunity = true;
//            Projectile.idStaticNPCHitCooldown = 20;
//        }

//        public override bool? CanDamage()
//        {
//            if (Projectile.ai[0] == 0)
//                return false;

//            return null;
//        }

//        public override void OnSpawn(IEntitySource source)
//        {
//            Projectile.frame = Main.rand.Next(3);
//            Projectile.alpha = 255;
//        }

//        public override void AI()
//        {
//            if (Projectile.ai[0] == 0)
//            {
//                Projectile.alpha -= 25;
//                if (Projectile.alpha < 0)
//                {
//                    Projectile.alpha = 0;
//                    Projectile.ai[0] = 1;
//                }
//            }
//            else
//            {
//                Projectile.velocity.Y += 0.2f;
//                if (Projectile.velocity.Y > 16)
//                    Projectile.velocity.Y = 16;
//            }

//            if (Main.rand.NextBool())
//            {
//                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height,
//                    DustID.Ice);
//                d.noGravity = true;
//            }
//        }

//        public override void OnKill(int timeLeft)
//        {
//            for (int i = 0; i < 8; i++)
//            {
//                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height,
//                    DustID.Ice);
//            }

//            SoundEngine.PlaySound(CoraliteSoundID.DigIce, Projectile.Center);
//        }
//    }
//}
