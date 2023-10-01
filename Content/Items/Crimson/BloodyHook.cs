using Coralite.Content.Dusts;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Crimson
{
    public class BloodyHook : ModItem
    {
        public override string Texture => AssetDirectory.CrimsonItems + Name;

        public int combo;

        public override void SetDefaults()
        {
            Item.useAnimation = Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileType<BloodHookSlash>();
            Item.DamageType = DamageClass.Melee;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Orange3, Item.sellPrice(0, 1, 0, 0));
            Item.SetWeaponValues(26, 4);
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer != player.whoAmI)
                return false;

            if (player.altFunctionUse == 2)
            {
                foreach (var proj in Main.projectile.Where(p => p.active && p.owner == player.whoAmI && p.type == ProjectileType<BloodHookChain>()))
                {
                    if ((int)proj.ai[2] == 2f)
                    {
                        for (int i = 0; i < proj.localNPCImmunity.Length; i++)
                            proj.localNPCImmunity[i] = 0;

                        proj.ai[2] = 3f;
                        proj.netUpdate = true;
                    }
                    return false;
                }

                //生成弹幕
                Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<BloodHookChain>(), (int)(damage * 0.8f), knockback, player.whoAmI);
                return false;
            }

            //生成弹幕
            Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI, combo);

            combo++;
            if (combo > 2)
                combo = 0;
            return false;
        }
    }

    public class BloodHookSlash : BaseSwingProj
    {
        public override string Texture => AssetDirectory.CrimsonItems + "BloodyHookProj";

        public static Asset<Texture2D> ChainTex;

        public ref float Combo => ref Projectile.ai[0];

        public override void Load()
        {
            ChainTex = Request<Texture2D>(AssetDirectory.CrimsonItems + "BloodyChain");
        }

        public override void Unload()
        {
            ChainTex = null;
        }

        public BloodHookSlash() : base(1.57f) { }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 4;
        }

        public override void SetDefs()
        {
            Projectile.width = Projectile.height = 48;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.localNPCHitCooldown = 16;
            onHitFreeze = 0;
        }

        protected override void Initializer()
        {
            Projectile.extraUpdates = 1;

            Projectile.scale = 0.9f;

            SoundEngine.PlaySound(CoraliteSoundID.Swing2_Item7, Projectile.Center);
            switch ((int)Combo)
            {
                default:
                case 0:
                    maxTime = Owner.itemTimeMax*2;
                    startAngle = 2f;
                    totalAngle = 4.6f;
                    distanceToOwner = 10;
                    Smoother = Coralite.Instance.SqrtSmoother;
                    break;
                case 1:
                    maxTime = Owner.itemTimeMax * 2;
                    startAngle = -2f;
                    totalAngle = -4.6f;
                    distanceToOwner = 10;
                    Smoother = Coralite.Instance.SqrtSmoother;
                    break;
                case 2:
                    maxTime = Owner.itemTimeMax * 3;
                    startAngle = 3f;
                    totalAngle = 12;
                    distanceToOwner = 10;
                    Smoother = Coralite.Instance.NoSmootherInstance;
                    break;
            }
            base.Initializer();
        }

        protected override void OnSlash()
        {
            Vector2 dir = RotateVec2.RotatedBy(1.57f * Math.Sign(totalAngle));
            Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), DustID.Crimson,
                   dir * Main.rand.NextFloat(0.5f, 2f));
            dust.noGravity = true;

            if ((int)Combo == 2)
            {
                if ((int)Timer == 24)
                    SoundEngine.PlaySound(CoraliteSoundID.Swing2_Item7, Projectile.Center);
                if (Timer < 3 * (maxTime - minTime) / 4f)
                {
                    distanceToOwner += 1.4f;
                    Projectile.scale += 0.01f;
                }
                else
                {
                    distanceToOwner -= 2.2f;
                    Projectile.scale -= 0.03f;
                }
            }
            else
            {
                if (Timer < (maxTime - minTime) / 4f)
                {
                    distanceToOwner += 8f;
                    Projectile.scale += 0.02f;
                }
                else
                {
                    distanceToOwner -= 0.6f;
                    Projectile.scale -= 0.0075f;
                }
            }
            base.OnSlash();
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.HasBuff<BloodyHookDebuff>())
            {
                modifiers.SourceDamage += 0.35f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (VisualEffectSystem.HitEffect_Dusts)
            {
                Vector2 direction = RotateVec2.RotatedBy(-1.57f * Math.Sign(totalAngle));

                Helper.SpawnDirDustJet(target.Center, () => direction.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)), 2, 8,
                    (i) => i * 1f, DustID.Blood, Scale: Main.rand.NextFloat(1f, 2f), noGravity: false);

                for (int i = 0; i < 6; i++)
                    Dust.NewDustPerfect(target.Center, DustID.Blood, direction.RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f)) * Main.rand.NextFloat(2f, 4f),
                        Scale: Main.rand.NextFloat(1f, 2f));
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //绘制链条
            Texture2D chainTex = ChainTex.Value;

            int width = (int)(Projectile.Center - Owner.Center).Length();   //链条长度

            Vector2 startPos = Owner.Center - Main.screenPosition;//起始点
            Vector2 endPos = Projectile.Center - Main.screenPosition;//终止点

            var laserTarget = new Rectangle((int)startPos.X, (int)startPos.Y, width, chainTex.Height);  //目标矩形
            var laserSource = new Rectangle(0, 0, width, chainTex.Height);   //把自身拉伸到目标矩形
            var origin = new Vector2(0, chainTex.Height / 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(default, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(chainTex, laserTarget, laserSource, lightColor, Projectile.rotation, origin, 0, 0);

            Texture2D mainTex = TextureAssets.Projectile[Type].Value;

            //绘制影子拖尾
            Projectile.DrawShadowTrails(lightColor, 0.3f, 0.03f, 1, 8, 2, 2f);

            //绘制自己
            Main.spriteBatch.Draw(mainTex, endPos, null, lightColor, Projectile.rotation + 2f, mainTex.Size() / 2, Projectile.scale, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);

            return false;
        }

        public override void PostDraw(Color lightColor) { }
    }

    public class BloodHookChain : BaseHeldProj
    {
        public override string Texture => AssetDirectory.CrimsonItems + "BloodyHookProj";

        public static Asset<Texture2D> ChainTex;

        public ref float Target => ref Projectile.ai[1];
        public ref float HookState => ref Projectile.ai[2];

        public ref float Timer => ref Projectile.localAI[0];

        private Vector2 offset;

        public override void SetDefaults()
        {
            Projectile.usesLocalNPCImmunity = false;
            Projectile.localNPCHitCooldown = 20;
            Projectile.width = Projectile.height = 32;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
        }

        private enum AIStates
        {
            back = -1,
            rolling = 0,
            shoot = 1,
            onHit = 2,
            drag = 3
        }

        public override void Load()
        {
            ChainTex = Request<Texture2D>(AssetDirectory.CrimsonItems + "BloodyChain");
            //GoreLoader.AddGoreFromTexture<SimpleModGore>(Mod, AssetDirectory.CrimsonItems + "BloodyHookProj");
        }

        public override void Unload()
        {
            ChainTex = null;
        }

        public override void AI()
        {
            switch ((int)HookState)
            {
                default:
                case (int)AIStates.back:    //返回玩家手中的状态
                    if (Vector2.Distance(Owner.Center, Projectile.Center) < 48)
                        Projectile.Kill();

                    Projectile.velocity = (Owner.Center - Projectile.Center).SafeNormalize(Vector2.One) * 32;
                    Projectile.rotation = (Projectile.Center - Owner.Center).ToRotation();
                    break;
                case (int)AIStates.rolling: //拿在手里转转转的状态
                    if (Main.mouseRight)
                    {
                        Owner.heldProj = Projectile.whoAmI;
                        Owner.itemAnimation = Owner.itemTime = 2;
                        Projectile.rotation += 0.5f;
                        if (Projectile.rotation % 4 < 0.2f)
                            SoundEngine.PlaySound(CoraliteSoundID.Swing2_Item7, Projectile.Center);

                        Owner.itemRotation = Projectile.rotation + (OwnerDirection > 0 ? 0 : MathHelper.Pi);
                        Projectile.Center = Owner.Center + Projectile.rotation.ToRotationVector2() * 32;
                    }
                    else
                    {
                        SoundEngine.PlaySound(CoraliteSoundID.WhipSwing_Item152, Projectile.Center);
                        Vector2 dir = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.Zero);
                        Projectile.Center = Owner.Center + dir * 64;
                        Projectile.velocity = dir * 20;
                        Projectile.rotation = dir.ToRotation();
                        HookState = (int)AIStates.shoot;
                        Projectile.tileCollide = true;
                        Projectile.netUpdate = true;
                    }
                    break;
                case (int)AIStates.shoot://发射中的状态
                    if (Timer > 16)
                    {
                        Timer = 0;
                        HookState = (int)AIStates.back;
                        Projectile.tileCollide = false;
                        Projectile.netUpdate = true;
                    }

                    Projectile.rotation = (Projectile.Center - Owner.Center).ToRotation();
                    Timer++;
                    break;
                case (int)AIStates.onHit://钩在敌人身上的状态
                    if (Target < 0 || Target > Main.maxNPCs)
                        Projectile.Kill();

                    NPC npc = Main.npc[(int)Target];
                    if (!npc.active || npc.dontTakeDamage || npc.Distance(Owner.Center) > 16 * 30 || !Collision.CanHitLine(Owner.Center, 1, 1, npc.Center, 1, 1))
                        Projectile.Kill();

                    Projectile.rotation = (Projectile.Center - Owner.Center).ToRotation();
                    Projectile.Center = npc.Center + offset;
                    Timer = 0;
                    break;
                case (int)AIStates.drag://将玩家拖拽过去的状态

                    if ((int)Timer == 0)
                    {
                        Owner.immuneTime = 30;
                        Owner.immune = true;
                    }

                    if ((int)Timer < 6)
                    {
                        Owner.velocity *= 0;
                        Owner.Center = Vector2.Lerp(Owner.Center, Projectile.Center, 0.35f);
                    }
                    else
                    {
                        //将玩家回弹
                        SoundEngine.PlaySound(CoraliteSoundID.Bloody_NPCHit9, Projectile.Center);
                        Helpers.Helper.PlayPitched("Misc/BloodySlash2", 0.4f, -0.2f, Projectile.Center);
                        Vector2 dir = (Projectile.Center - Owner.Center).SafeNormalize(Vector2.Zero);
                        var modifier = new PunchCameraModifier(Owner.position, dir, 14, 8f, 6, 1000f);
                        Main.instance.CameraModifiers.Add(modifier);
                        Owner.velocity = new Vector2(Math.Sign(Owner.Center.X - Projectile.Center.X) * 4, -3);
                        Projectile.Kill();
                    }

                    Timer++;
                    break;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if ((int)HookState == (int)AIStates.shoot)
            {
                HookState = (int)AIStates.back;
                Projectile.tileCollide = false;
            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.Server && (int)HookState < 3 && (int)HookState > -1)
            {
                //生成血液粒子和钩子gore
                SoundEngine.PlaySound(CoraliteSoundID.BloodyDeath3_NPCDeath21, Projectile.Center);
                //Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, Mod.Find<ModGore>("BloodyHookProj").Type);
                Vector2 direction = (Owner.Center - Projectile.Center).SafeNormalize(Vector2.One);
                float length = (Owner.Center - Projectile.Center).Length();
                for (int i = 0; i < length; i += 4)
                {
                    Dust.NewDustPerfect(Projectile.Center + direction * i + Main.rand.NextVector2Circular(4, 4), DustID.Blood, Scale: Main.rand.NextFloat(1f, 2f));
                }
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if ((int)HookState == (int)AIStates.drag && (int)Timer == 6)
                return true;
            if ((int)HookState < 2 && Collision.CanHitLine(Owner.Center, 1, 1, target.Center, 1, 1))
                return null;

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Vector2 direction = (Owner.Center - target.Center).SafeNormalize(Vector2.One);

            if ((int)HookState == (int)AIStates.shoot)
            {
                Projectile.velocity *= 0;
                Target = target.whoAmI;
                offset = Projectile.Center - target.Center;
                HookState = (int)AIStates.onHit;
                Timer = 0;
                Projectile.netUpdate = true;

                if (VisualEffectSystem.HitEffect_Dusts)
                    for (int i = 0; i < 10; i++)
                        Dust.NewDustPerfect(target.Center, DustID.Blood, direction.RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f)) * Main.rand.NextFloat(2f, 4f),
                            Scale: Main.rand.NextFloat(1f, 2f));
            }
            else if ((int)HookState == (int)AIStates.drag)
            {
                target.AddBuff(BuffType<BloodyHookDebuff>(), 120);

                if (VisualEffectSystem.HitEffect_Dusts)
                {
                    Helper.SpawnDirDustJet(target.Center, () => direction.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)), 2, 12,
                        (i) => i * 1f, DustID.Blood, Scale: Main.rand.NextFloat(1f, 2f), noGravity: false, extraRandRot: 0.1f);

                    for (int i = 0; i < 10; i++)
                        Dust.NewDustPerfect(target.Center, DustID.Blood, direction.RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f)) * Main.rand.NextFloat(2f, 4f),
                            Scale: Main.rand.NextFloat(1f, 2f));
                }

                if (VisualEffectSystem.HitEffect_SpecialParticles)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustType<Slash>(), newColor: Color.Red, Scale: Main.rand.NextFloat(0.3f, 0.4f));
                    dust.rotation = Projectile.rotation + 1.57f + Main.rand.NextFloat(-0.2f, 0.2f);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //绘制链条
            Texture2D chainTex = ChainTex.Value;

            int width = (int)(Projectile.Center - Owner.Center).Length();   //链条长度

            Vector2 startPos = Owner.Center - Main.screenPosition;//起始点
            Vector2 endPos = Projectile.Center - Main.screenPosition;//终止点

            var laserTarget = new Rectangle((int)startPos.X, (int)startPos.Y, width, chainTex.Height);  //目标矩形
            var laserSource = new Rectangle(0, 0, width, chainTex.Height);   //把自身拉伸到目标矩形
            var origin2 = new Vector2(0, chainTex.Height / 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(default, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(chainTex, laserTarget, laserSource, lightColor, Projectile.rotation, origin2, 0, 0);

            Texture2D mainTex = TextureAssets.Projectile[Type].Value;

            //绘制自己
            Main.spriteBatch.Draw(mainTex, endPos, null, lightColor, Projectile.rotation + 2f, mainTex.Size() / 2, Projectile.scale, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);

            return false;
        }
    }

    public class BloodyHookDebuff : ModBuff
    {
        public override string Texture => AssetDirectory.Debuffs + Name;

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (Main.rand.NextBool(10))
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, Scale: Main.rand.NextFloat(1, 2));
            }
        }
    }
}
