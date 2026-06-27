using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.Items.Misc_Magic
{
    [AutoloadEquip(EquipType.Head)]
    [PlayerEffect]
    public class SpectreBoulder : ModItem
    {
        public override string Texture => AssetDirectory.Misc_Magic + Name;
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
            Item.value = Item.sellPrice(0, 7, 50);
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 12;
        }

        public override void UpdateEquip(Player player)
        {
            player.statManaMax2 += 20;
            player.GetDamage(DamageClass.Magic) += 0.05f;
            player.GetCritChance(DamageClass.Magic) += 5f;
            if (!player.isDisplayDollOrInanimate)
                player.socialGhost = true;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemID.SpectreRobe && legs.type == ItemID.SpectrePants;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = bonus.Value;
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.AddEffect(nameof(SpectreBoulder));
        }

        public override void PreUpdateVanitySet(Player player)
        {
            if (!player.isDisplayDollOrInanimate)
            {
                if (player.TryGetModPlayer(out CoralitePlayer cp) && cp.HasEffect(nameof(SpectreBoulder)))
                    player.socialGhost = true;

                player.SetArmorEffectVisuals(player);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SpectreBar, 12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class SpectreBoulderProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Misc_Magic + Name;

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];
        public ref float TargetIndex => ref Projectile.ai[2];
        public Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 10);
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 32;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
        }

        public override bool? CanDamage()
        {
            if (State == 1)
                return null;

            return false;
        }

        public override void AI()
        {
            switch (State)
            {
                default:
                case 0:
                    Timer++;
                    Projectile.velocity *= 0.9f;
                    TargetIndex = -1;
                    Projectile.alpha = 255;

                    if (Timer > 25)
                    {
                        Timer = 0;
                        Projectile.extraUpdates = 1;
                        if (Helper.TryFindClosestEnemy(Projectile.Center, 1000, n => n.CanBeChasedBy(), out NPC target))
                        {
                            TargetIndex = target.whoAmI;
                            State = 1;
                        }
                        else
                        {
                            State = 2;
                        }
                    }

                    break;
                case 1://追踪目标敌怪
                    Timer++;
                    if (!TargetIndex.TryGetNPC(out NPC npc))
                    {
                        State = 2;
                        Timer = 0;
                        return;
                    }

                    float speed2 = 10;
                    if (Timer > 45)
                        speed2 += (Timer - 45) / 10;

                    Projectile.ChaseGradually(npc.Center, speed2, 19, 20);

                    break;
                case 2:
                    //追踪玩家
                    Timer++;

                    float speed = 12;
                    if (Timer > 30)
                    {
                        speed += (Timer - 30) / 10;
                    }

                    Projectile.ChaseGradually(Owner.Center, speed, 19, 20);
                    if (Vector2.Distance(Projectile.Center, Owner.Center) < speed + 10)
                    {
                        //回血只在 owner 端结算，避免各端到达时机/次数不一致导致的重复回血
                        float healValue = Projectile.damage * 0.12f;
                        if (Projectile.IsOwnedByLocalPlayer() && Owner.lifeSteal > 0f && !Owner.moonLeech)
                        {
                            Owner.lifeSteal -= healValue;
                            Owner.Heal((int)healValue);
                        }

                        Projectile.Kill();

                        foreach (var pos in Projectile.oldPos)
                        {
                            Vector2 p = pos + Projectile.Size / 2;

                            for (int i = 0; i < 2; i++)
                            {
                                Dust d = Dust.NewDustPerfect(p, DustID.Stone, Helper.NextVec2Dir(0.5f, 2), 100, Color.White with { A = 50 }, Main.rand.NextFloat(1, 1.5f));
                                d.noGravity = true;
                            }
                        }
                    }

                    break;
            }

            if (State != 0 || Timer > 15)
            {
                Projectile.UpdateFrameNormally(5 * Projectile.MaxUpdates, 9, resetTo: 4);
                if (Projectile.alpha > 120)
                    Projectile.alpha -= 15;
            }

            if (Math.Abs(Projectile.velocity.X) > 1)
            {
                Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
                //Projectile.rotation = Projectile.velocity.ToRotation();
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            State = 2;
            Timer = 0;

            Projectile.velocity *= -0.6f;
            Projectile.velocity = Projectile.velocity.RotateByRandom(-0.3f, 0.3f);

            Helper.PlayPitched(CoraliteSoundID.StoneBurst_Item70, Projectile.Center, volumeAdjust: -0.3f, pitchAdjust: 0.5f);

            for (int i = 0; i < 20; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Stone, (i * MathHelper.TwoPi / 20).ToRotationVector2() * Main.rand.NextFloat(2, 5), 100, Color.White with { A = 50 }, Main.rand.NextFloat(1, 1.5f));
                d.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects effect = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Color c = new Color(255, 255, 255, Projectile.alpha) * (0.6f + 0.4f * Projectile.alpha / 255f);
            Rectangle frameBox = new(0, Projectile.frame, 1, 10);

            Projectile.DrawShadowTrailsSpriteEffect(c, 0.4f, 0.4f / 10, 1, 10, 2, frameBox: frameBox, effect: effect);
            Projectile.QuickFrameDraw(frameBox, c, 0, effect);

            return false;
        }
    }
}
