using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.Donator
{
    public class SurviveAndPerish : ModItem
    {
        public override string Texture => AssetDirectory.Donator + Name;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 146;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.knockBack = 6;
            Item.shootSpeed = 7.5f;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Ranged;
            Item.value = Item.sellPrice(0, 8);
            Item.rare = ModContent.RarityType<SurviveAndPerishRarity>();
            Item.shoot = ModContent.ProjectileType<SurviveHeldProj>();
            Item.useAmmo = ItemID.Dynamite;

            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
        }

        public override void HoldItem(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<PerishProj>()] < 1)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<PerishProj>()
                    , 0, Item.knockBack, player.whoAmI, 60, 4);
            }
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                if (!player.PickAmmo(Item, out _, out _, out _, out _, out _, true))
                    return false;
                //检测弹幕状态
                Projectile p = Main.projectile.FirstOrDefault(proj => proj.active && proj.friendly
                    && proj.owner == player.whoAmI && proj.type == ModContent.ProjectileType<PerishProj>());

                if (p != null)
                {
                    if (p.ai[0] == 0)
                    {
                        (p.ModProjectile as PerishProj).StartAttack();
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }

            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer != player.whoAmI)
                return false;

            if (player.altFunctionUse == 2)
            {
                return false;
            }

            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center
                , Vector2.Zero, ModContent.ProjectileType<SurviveHeldProj>(), 0, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position
                , velocity, ModContent.ProjectileType<MiniDynamite>(), damage, knockback, player.whoAmI);

            Vector2 dir = velocity.SafeNormalize(Vector2.Zero);
            Vector2 pos = player.Center + (dir * 66);

            Dust d2 = Dust.NewDustPerfect(pos,
                Main.rand.NextFromList(
                    ModContent.DustType<PerishMissileFog1>(),
                    ModContent.DustType<PerishMissileFog2>(),
                    ModContent.DustType<PerishMissileFog3>(),
                    ModContent.DustType<PerishMissileFog4>())
                , Vector2.Zero);
            d2.rotation = dir.ToRotation();
            for (int i = 0; i < 4; i++)
            {
                Dust d = Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(6, 6), ModContent.DustType<StarFireSmokeDust>()
                     , dir.RotateByRandom(-0.3f, 0.3f) * Main.rand.NextFloat(2, 7), Scale: Main.rand.NextFloat(0.7f, 1.2f));
                d.noGravity = true;
                d = Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(6, 6), ModContent.DustType<StarFireDust>()
                    , dir.RotateByRandom(-0.3f, 0.3f) * Main.rand.NextFloat(2, 7), Scale: Main.rand.NextFloat(0.7f, 1.2f));
                d.noGravity = true;
            }

            Helper.PlayPitched(CoraliteSoundID.Shotgun2_Item38, player.Center, pitch: -0.4f, volumeAdjust: -0.4f);
            Helper.PlayPitched(CoraliteSoundID.Gun3_Item41, player.Center, pitch: -0.8f);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GrenadeLauncher)
                .AddIngredient(ItemID.MeteoriteBar, 20)
                .AddIngredient(ItemID.Dynamite, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class SurviveAndPerishRarity : ModRarity
    {
        public override Color RarityColor
        {
            get
            {
                float factor = Math.Abs(MathF.Sin(Main.GlobalTimeWrappedHourly / 2)) * 2;
                if (factor < 1)
                    return Color.Lerp(new Color(186, 30, 30), new Color(255, 103, 65), factor);
                return Color.Lerp(new Color(255, 103, 65), new Color(255, 168, 115), factor - 1);
            }
        }
    }

    public class SurviveHeldProj : BaseGunHeldProj
    {
        public override string Texture => AssetDirectory.Donator + "SurviveAndPerishHeldProj";

        public SurviveHeldProj() : base(0.15f, 18, -8, AssetDirectory.Donator) { }

        public override void Initialize()
        {
            int time = Owner.itemTimeMax;
            if (time < 6)
                time = 6;

            Projectile.timeLeft = time;
            MaxTime = time;
            if (Main.myPlayer == Projectile.owner)
            {
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                TargetRot = (Main.MouseWorld - Owner.Center).ToRotation() + (DirSign > 0 ? 0f : MathHelper.Pi);
            }

            Projectile.netUpdate = true;
            HeldPositionY = 10;
        }

        public override float Ease()
        {
            float x = 1.465f * Projectile.timeLeft / MaxTime;
            return x * MathF.Sin(x * x * x) / 1.186f;
        }

        public override void ModifyAI(float factor)
        {
            Projectile.frame = (int)((1 - (Projectile.timeLeft / MaxTime)) * 6);
        }

        public override void GetFrame(Texture2D mainTex, out Rectangle? frame, out Vector2 origin)
        {
            frame = mainTex.Frame(1, 6, 0, Projectile.frame);
            origin = frame.Value.Size() / 2;
            origin.X -= DirSign * frame.Value.Width / 6;
        }
    }

    public class PerishProj : BaseHeldProj
    {
        public override string Texture => AssetDirectory.Donator + Name;

        public int frameX;

        public ref float Timer => ref Projectile.ai[0];
        public ref float State => ref Projectile.ai[1];

        public static Asset<Texture2D> ShoulderTex;

        private bool drawed;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            ShoulderTex = ModContent.Request<Texture2D>(AssetDirectory.Donator + "PerishShoulder");
            GoreLoader.AddGoreFromTexture<SimpleModGore>(Mod, AssetDirectory.Donator + "PerishMetrorBucket");
            GoreLoader.AddGoreFromTexture<SimpleModGore>(Mod, AssetDirectory.Donator + "PerishMissileCompartment");
        }

        public override void Unload()
        {
            ShoulderTex = null;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.timeLeft = 200;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (Owner.HeldItem.type != ModContent.ItemType<SurviveAndPerish>())
                return;

            Projectile.timeLeft = 2;

            switch (State)
            {
                default:
                case 0://可以发射导弹
                    frameX = 1;
                    break;
                case 1://发射期间
                    ShootMissile();
                    break;
                case 2:
                    Timer--;
                    if (Timer < 0)
                    {
                        Timer = 0;
                        State++;
                    }
                    break;
                case 3://发射完毕，进入休整期的动画
                    {
                        frameX = 1;
                        int preFrame = Projectile.frame;
                        Projectile.UpdateFrameNormally(4, 4);

                        if (Projectile.frame == 1 && preFrame == 0)
                        {
                            Vector2 dir = (Projectile.rotation + MathHelper.Pi).ToRotationVector2();
                            Gore.NewGoreDirect(Projectile.GetSource_FromAI()
                                , Projectile.Top + (dir * 12)
                                , dir * 4, Mod.Find<ModGore>("PerishMetrorBucket").Type);

                            SoundEngine.PlaySound(CoraliteSoundID.NoUse_BlowgunPlus_Item65, Projectile.Center);
                        }

                        Vector2 pos = Projectile.Center + ((Projectile.rotation + (DirSign > 0 ? 0 : 3.141f)).ToRotationVector2() * 10) - (Vector2.UnitY * 30);

                        for (int i = 0; i < 2; i++)
                        {
                            Dust d = Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(30, 12), ModContent.DustType<StarFireSmokeDust>()
                                 , -Vector2.UnitY.RotateByRandom(-0.3f, 0.3f) * Main.rand.NextFloat(2, 6), Scale: Main.rand.NextFloat(0.7f, 1.4f));
                            d.noGravity = true;
                        }

                        if (Projectile.frame == 3)
                        {
                            //生成gore
                            Gore.NewGoreDirect(Projectile.GetSource_FromAI(), pos + new Vector2(DirSign > 0 ? 0 : -30, 0)
                                , new Vector2(DirSign, -2), Mod.Find<ModGore>("PerishMissileCompartment").Type);
                            SoundEngine.PlaySound(CoraliteSoundID.MartianDrone_HitMetal_NPCHit42, Projectile.Center);
                            State++;
                            Timer = 60 * 2;
                        }
                    }
                    break;
                case 4://休整期
                    {
                        frameX = 1;
                        Projectile.frame = 3;
                        Timer--;
                        if (Timer < 1)
                        {
                            State++;
                            frameX = 0;
                            Projectile.frame = 0;
                            SoundEngine.PlaySound(CoraliteSoundID.StarFalling_Item105, Projectile.Center);
                            //SoundEngine.PlaySound(CoraliteSoundID.Pixie, Projectile.Center);
                        }
                    }
                    break;
                case 5://重新装填的动画
                    {
                        Lighting.AddLight(Projectile.Center, new Vector3(0.7f, 0.35f, 0.2f));
                        Projectile.UpdateFrameNormally(4, 12);
                        if (Projectile.frame > 10)
                        {
                            State = 0;
                            Projectile.frame = 0;
                            frameX = 1;
                            SoundEngine.PlaySound(CoraliteSoundID.Ding_Item4, Projectile.Center);
                        }
                    }
                    break;
            }

            Projectile.Center = Owner.Center + new Vector2(-DirSign * 22, -8);
            float targetRot = (MathF.Sign(Main.MouseWorld.X - Projectile.Center.X) == DirSign ? 1 : -1)
                * Math.Clamp((Main.MouseWorld.Y - Projectile.Center.Y) / 300, -0.2f, 0.2f);
            Projectile.rotation = Projectile.rotation.AngleLerp(targetRot, 0.2f);
        }

        public override bool? CanDamage() => false;

        public void ShootMissile()
        {
            if (Timer % 6 == 0)
            {
                Vector2 pos = Projectile.Center;
                float rot = (DirSign * Projectile.rotation) + (DirSign > 0 ? 0 : MathHelper.Pi);
                Vector2 dir = rot.ToRotationVector2();
                pos -= Vector2.UnitY * 14;
                pos += dir * 40;
                pos += Main.rand.NextVector2Circular(16, 16);
                Projectile.NewProjectileFromThis<PerishMissile>(pos, dir.RotateByRandom(-0.2f, 0.2f) * 4, (int)(Owner.GetWeaponDamage(Owner.HeldItem) * 0.75f),
                    Projectile.knockBack);

                Dust d = Dust.NewDustPerfect(pos + (dir * 30), ModContent.DustType<MissileShootDust>(), Vector2.Zero, Scale: Main.rand.NextFloat(1.5f, 2f));
                d.rotation = rot + 1.57f;

                for (int i = 0; i < 2; i++)
                {
                    Dust d2 = Dust.NewDustPerfect(pos + (dir * 30) + Main.rand.NextVector2Circular(30, 12), ModContent.DustType<StarFireSmokeDust>()
                         , dir.RotateByRandom(-0.3f, 0.3f) * Main.rand.NextFloat(2, 6), Scale: Main.rand.NextFloat(0.7f, 1.4f));
                    d2.noGravity = true;
                }

                SoundEngine.PlaySound(CoraliteSoundID.Blowpipe_Item63, Projectile.Center);
            }

            Timer--;

            if (Timer < 0)
            {
                State++;
                Timer = 30;
            }
        }

        public void StartAttack()
        {
            if (State != 0)
                return;

            State = 1;
            Timer = 60;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            drawed = false;
            overPlayers.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects eff = DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            var pos = Projectile.Center - Main.screenPosition;

            if (drawed)
            {
                Texture2D mainTex2 = ShoulderTex.Value;
                Main.spriteBatch.Draw(mainTex2, pos + new Vector2(DirSign * 10, 12), null, lightColor, 0, mainTex2.Size() / 2, Projectile.scale, eff, 0);

                return false;
            }

            Texture2D mainTex = Projectile.GetTexture();

            var frame = mainTex.Frame(2, 11, frameX, Projectile.frame);
            var origin = new Vector2((frame.Width / 2) - (DirSign * frame.Width * 2 / 5), frame.Height * 3 / 4);

            Main.spriteBatch.Draw(mainTex, pos, frame, lightColor, Projectile.rotation * DirSign, origin, Projectile.scale, eff, 0);
            drawed = true;

            return false;
        }
    }

    public class PerishMissile : ModProjectile, IDrawPrimitive, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.Donator + Name;

        public ref float Target => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];

        private static BasicEffect effect;
        public Trail trail;

        public int chaseFactor = Main.rand.Next(160, 260);

        public PerishMissile()
        {
            if (Main.dedServ)
            {
                return;
            }

            Main.QueueMainThreadAction(() =>
            {
                effect = new BasicEffect(Main.instance.GraphicsDevice);
                effect.VertexColorEnabled = true;
            });
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                trail = new Trail(Main.instance.GraphicsDevice, 14, new NoTip()
                    , factor => 2, ColorFunc);
                Projectile.InitOldPosCache(14);
                Target = -1;
                FindTarget();
            }

            Chase();

            if (Main.rand.NextBool())
                Projectile.SpawnTrailDust(ModContent.DustType<StarFireDust>(), Main.rand.NextFloat(0.2f, 0.6f)
                    , Scale: Main.rand.NextFloat(0.5f, 0.8f));

            if (Projectile.frame < 2)
                Projectile.UpdateFrameNormally(7, 3);

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.UpdateOldPosCache(addVelocity: true);
            trail.Positions = Projectile.oldPos;
        }

        public static Color ColorFunc(Vector2 factor)
        {
            Color c;
            if (factor.X < 0.5f)
                c = Color.Lerp(new Color(186, 30, 30), new Color(255, 103, 65), factor.X / 0.5f);
            else
                c = Color.Lerp(new Color(255, 103, 65), new Color(255, 168, 115), (factor.X - 0.5f) / 0.5f);

            return c * factor.X;
        }

        public void FindTarget()
        {
            int type = ModContent.ProjectileType<MiniDynamite>();

            Dictionary<int, int> targetRecord = new();

            bool hasTarget = false;

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (!p.active || !p.friendly || p.owner != Projectile.owner || p.type != type)
                    continue;

                NPC n = Main.npc[(int)p.ai[0]];
                if (!n.CanBeChasedBy() || !Collision.CanHit(Projectile, n))
                    continue;

                hasTarget = true;

                if (targetRecord.TryGetValue((int)p.ai[0], out _))
                {
                    targetRecord[(int)p.ai[0]]++;
                }
                else
                    targetRecord.Add((int)p.ai[0], 1);
            }

            if (!hasTarget)
            {
                if (Helper.TryFindClosestEnemy(Projectile.Center, 800, n => n.CanBeChasedBy() && Collision.CanHit(Projectile, n), out NPC tar))
                    Target = tar.whoAmI;

                return;
            }

            Target = targetRecord.Keys.Aggregate((i, j) => targetRecord[i] >= targetRecord[j] ? i : j);
        }

        public void Chase()
        {
            if (!Target.GetNPCOwner(out NPC target, () => Target = -1))
                return;

            float slowTime = Timer < 60 ? Coralite.Instance.X2Smoother.Smoother(Timer / 60) : 1;
            float speed = Timer < 20 ? Coralite.Instance.X2Smoother.Smoother(Timer / 20) : 1;
            slowTime = Helper.Lerp(chaseFactor, 20, slowTime);
            speed = Helper.Lerp(6, 24, speed);

            Vector2 center = Projectile.Center;
            Vector2 dir = target.Center - center;
            float length = dir.Length();
            if (length < 100f)
                speed -= 4f;

            length = speed / length;
            dir *= length;

            if (Timer < 10)
            {
                Projectile.velocity *= 1.06f;
            }

            Projectile.velocity.X = ((Projectile.velocity.X * slowTime) + dir.X) / (slowTime + 1);
            Projectile.velocity.Y = ((Projectile.velocity.Y * slowTime) + dir.Y) / (slowTime + 1);

            Timer++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile p = Main.projectile.FirstOrDefault(p => p.active && p.friendly
                && p.owner == Projectile.owner && p.type == ModContent.ProjectileType<MiniDynamite>() && p.ai[0] == target.whoAmI);
            if (p != null)
            {
                (p.ModProjectile as MiniDynamite).BigBoom();
            }
        }

        public override void OnKill(int timeLeft)
        {
            Helper.PlayPitched(CoraliteSoundID.Fairy_NPCHit5, Projectile.Center, volumeAdjust: -0.6f);

            int dustType = Main.rand.NextFromList(
                ModContent.DustType<PerishMissileFog1>(),
                ModContent.DustType<PerishMissileFog2>(),
                ModContent.DustType<PerishMissileFog3>(),
                ModContent.DustType<PerishMissileFog4>(),
                ModContent.DustType<PerishMissileFog5>(),
                ModContent.DustType<PerishMissileFog6>()
                );

            Dust d = Dust.NewDustPerfect(Projectile.Center + (Projectile.velocity * Main.rand.NextFloat(1f, 3)), dustType, Vector2.Zero, Scale: Main.rand.NextFloat(0.8f, 1.2f));
            d.rotation = Projectile.rotation;
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawPrimitives()
        {
            if (trail == null)
                return;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.World = world;
            effect.View = view;
            effect.Projection = projection;

            trail?.Render(effect);
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            Projectile.QuickDraw(Projectile.GetTexture().Frame(1, 3, 0, Projectile.frame), Color.White, 1.57f);
        }
    }

    /// <summary>
    /// 使用ai0记录命中的目标
    /// </summary>
    public class MiniDynamite : ModProjectile
    {
        public override string Texture => AssetDirectory.Donator + Name;

        public ref float Target => ref Projectile.ai[0];
        public ref float State => ref Projectile.ai[1];

        public bool canDamage = true;

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 1800;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.extraUpdates = 1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void AI()
        {
            switch (State)
            {
                default:
                case 0:
                    {
                        //旋转
                        //Projectile.rotation -= Projectile.velocity.X / 100;
                        if (Projectile.velocity.Y < 8)
                            Projectile.velocity.Y += 0.01f;

                        if (Projectile.timeLeft < 1730 && Projectile.velocity.Length() > 4)
                        {
                            Projectile.velocity *= 0.97f;
                        }

                        Projectile.rotation = Projectile.velocity.ToRotation();

                        //生成火星
                        SpawnFlameDust();
                    }
                    break;
                case 1://黏在敌怪身上
                    {
                        if (!Target.GetNPCOwner(out NPC owner, Projectile.Kill))
                            return;

                        Projectile.Center += owner.position - owner.oldPosition;
                        Projectile.velocity = Vector2.Zero;
                        SpawnFlameDust(false);
                        if (Projectile.timeLeft == 2)
                            SmallBoom();
                    }
                    break;
                case 2://爆炸
                    break;
            }
        }

        public void SpawnFlameDust(bool noGravity = true)
        {
            Vector2 dir = (Projectile.rotation + MathHelper.Pi).ToRotationVector2();

            float speed;
            float scale;
            if (noGravity)
            {
                scale = Main.rand.NextFloat(0.8f, 1.1f);
                speed = Main.rand.NextFloat(2, 4);
            }
            else
            {
                if (Main.rand.NextBool(3, 4))
                    return;
                scale = Main.rand.NextFloat(0.9f, 1.1f);
                speed = Main.rand.NextFloat(1, 3);
            }

            Dust d = Dust.NewDustPerfect(Projectile.Center + (dir * 10), ModContent.DustType<StarFireDust>()
                , dir.RotateByRandom(-0.2f, 0.2f) * speed, Scale: scale);
            d.noGravity = noGravity;
        }

        public void SmallBoom()
        {
            SoundEngine.PlaySound(CoraliteSoundID.Boom_Item14, Projectile.Center);
            Projectile.Resize(40, 40);
            BoomBase();
            Projectile.damage /= 2;

            for (int i = 0; i < 2; i++)
            {
                int num911 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, ModContent.DustType<StarFireSmokeDust>(), 0f, 0f, 100, default, 1f);
                Dust dust2 = Main.dust[num911];
                dust2.velocity *= 1.2f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num911].scale = 0.5f;
                    Main.dust[num911].fadeIn = 1f + (Main.rand.Next(10) * 0.1f);
                }
            }

            int type = ModContent.DustType<StarFireDust>();

            for (int i = 0; i < 6; i++)
            {
                int num913 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, type, 0f, 0f, 100, default, 1f);
                Main.dust[num913].noGravity = true;
                Dust dust2 = Main.dust[num913];
                dust2.velocity *= 3f;
                num913 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, type, 0f, 0f, 100, default, 0.6f);
                dust2 = Main.dust[num913];
                dust2.velocity *= 2f;
            }

        }

        public void BigBoom()
        {
            SoundEngine.PlaySound(CoraliteSoundID.Boom_NPCDeath14, Projectile.Center);
            BoomBase();
            Projectile.Resize(140, 140);

            for (int i = 0; i < 5; i++)
            {
                int num911 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, ModContent.DustType<StarFireSmokeDust>(), 0f, 0f, 100, default, 1.2f);
                Dust dust2 = Main.dust[num911];
                dust2.velocity *= 1.5f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num911].scale = 0.5f;
                    Main.dust[num911].fadeIn = 1f + (Main.rand.Next(10) * 0.1f);
                }
            }

            int type = ModContent.DustType<StarFireDust>();

            for (int i = 0; i < 10; i++)
            {
                int num913 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, type, 0f, 0f, 100, default, 1.5f);
                Main.dust[num913].noGravity = true;
                Dust dust2 = Main.dust[num913];
                dust2.velocity *= 3f;
                num913 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, type, 0f, 0f, 100, default, 1f);
                dust2 = Main.dust[num913];
                dust2.velocity *= 2f;
            }

            int dustType = Main.rand.NextFromList(
                ModContent.DustType<PerishMissileBigFog1>(),
                ModContent.DustType<PerishMissileBigFog2>(),
                ModContent.DustType<PerishMissileBigFog3>(),
                ModContent.DustType<PerishMissileBigFog4>(),
                ModContent.DustType<PerishMissileBigFog5>(),
                ModContent.DustType<PerishMissileBigFog6>()
                );
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            Dust d = Dust.NewDustPerfect(Projectile.Center + (dir * 12), dustType, Vector2.Zero);
            d.rotation = Projectile.rotation;
        }

        public void BoomBase()
        {
            canDamage = true;
            State = 2;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
            Projectile.StartAttack();
            Projectile.velocity = Vector2.Zero;

        }

        public override bool? CanDamage()
        {
            if (canDamage)
                return null;

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (State == 0)
            {
                Projectile.rotation = Main.rand.NextFloat(6.282f);
                int i = Main.projectile.Count(p => p.active && p.friendly
                    && p.owner == Projectile.owner && p.type == Projectile.type && p.ai[0] == target.whoAmI);

                Projectile.extraUpdates = 0;

                if (i > 8)
                {
                    SmallBoom();
                    return;
                }

                Projectile.timeLeft = 1200;
                State = 1;
                Projectile.velocity *= 0;
                canDamage = false;
                Target = target.whoAmI;
                Projectile.tileCollide = false;
            }
            else if (State == 2)
            {
                Projectile.damage = (int)(Projectile.damage * 0.8f);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SmallBoom();

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.QuickDraw(lightColor, 0);

            return false;
        }
    }

    public abstract class BasePerishMissileFog : ModDust
    {
        public override string Texture => AssetDirectory.Donator + Name;

        public abstract int FrameCount { get; }

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle();
        }

        public override bool Update(Dust dust)
        {
            dust.fadeIn++;

            if (dust.fadeIn % 2 == 0)
            {
                dust.frame.Y++;
                if (dust.frame.Y > FrameCount)
                    dust.active = false;
            }

            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Texture2D mainTex = Texture2D.Value;
            var frame = mainTex.Frame(1, FrameCount, 0, dust.frame.Y);
            Main.spriteBatch.Draw(mainTex, dust.position - Main.screenPosition, frame, Lighting.GetColor(dust.position.ToTileCoordinates())
                , dust.rotation, new Vector2(frame.Width / 4, frame.Height / 2), dust.scale, SpriteEffects.None, 0f);

            return false;
        }
    }

    public class PerishMissileFog1 : BasePerishMissileFog
    {
        public override int FrameCount => 14;
    }
    public class PerishMissileFog2 : BasePerishMissileFog
    {
        public override int FrameCount => 13;
    }
    public class PerishMissileFog3 : BasePerishMissileFog
    {
        public override int FrameCount => 14;
    }
    public class PerishMissileFog4 : BasePerishMissileFog
    {
        public override int FrameCount => 16;
    }
    public class PerishMissileFog5 : BasePerishMissileFog
    {
        public override int FrameCount => 14;
    }
    public class PerishMissileFog6 : BasePerishMissileFog
    {
        public override int FrameCount => 14;
    }

    public class PerishMissileBigFog1 : BasePerishMissileFog
    {
        public override int FrameCount => 14;
    }
    public class PerishMissileBigFog2 : BasePerishMissileFog
    {
        public override int FrameCount => 13;
    }
    public class PerishMissileBigFog3 : BasePerishMissileFog
    {
        public override int FrameCount => 14;
    }
    public class PerishMissileBigFog4 : BasePerishMissileFog
    {
        public override int FrameCount => 16;
    }
    public class PerishMissileBigFog5 : BasePerishMissileFog
    {
        public override int FrameCount => 14;
    }
    public class PerishMissileBigFog6 : BasePerishMissileFog
    {
        public override int FrameCount => 14;
    }

    public class StarFireDust : ModDust
    {
        public override string Texture => AssetDirectory.Donator + Name;

        public override void OnSpawn(Dust dust)
        {
            UpdateType = DustID.Torch;
            dust.frame = new Rectangle(0, 22 * Main.rand.Next(6), 22, 22);
            dust.rotation = Main.rand.NextFloat(6.282f);
            dust.scale *= 0.8f;
        }

        public override bool PreDraw(Dust dust)
        {
            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, Lighting.GetColor(dust.position.ToTileCoordinates())
                , dust.rotation, dust.frame.Size() / 2, dust.scale, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class StarFireSmokeDust : ModDust
    {
        public override string Texture => AssetDirectory.Donator + Name;

        public override void OnSpawn(Dust dust)
        {
            UpdateType = DustID.Smoke;
            dust.frame = new Rectangle(0, 22 * Main.rand.Next(3), 22, 22);
            dust.rotation = Main.rand.NextFloat(6.282f);
            dust.scale *= 0.8f;
        }

        public override bool PreDraw(Dust dust)
        {
            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, Lighting.GetColor(dust.position.ToTileCoordinates())
                , dust.rotation, dust.frame.Size() / 2, dust.scale, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class MissileShootDust : ModDust
    {
        public override string Texture => AssetDirectory.Donator + Name;

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, 22 * Main.rand.Next(3), 22, 22);
        }

        public override bool Update(Dust dust)
        {
            dust.fadeIn++;
            if (dust.fadeIn > 5)
            {
                dust.active = false;
            }

            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, Lighting.GetColor(dust.position.ToTileCoordinates())
                , dust.rotation, dust.frame.Size() / 2, dust.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
