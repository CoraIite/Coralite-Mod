using Coralite.Core;
using Coralite.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.ModPlayers
{
    public partial class CoralitePlayer
    {
        /// <summary> 皇帝凝胶鞋的CD </summary>
        public byte EmperorArmorCD;
        /// <summary> 皇帝凝胶鞋的粘液覆层 </summary>
        public short EmperorDefence;
        /// <summary> 绘制皇帝凝胶鞋的粘液覆层 </summary>
        public bool SlimeDraw;
        /// <summary> 皇帝凝胶鞋的粘液覆层上限 </summary>
        public const short EmperorDefenctMax = 30;

        private void UpdateEmerorSlimeBoots()
        {
            if (EmperorArmorCD > 0)
                EmperorArmorCD--;
        }

        private void ResetEmperorBoots()
        {
            EmperorDefence = 0;
            EmperorArmorCD = 0;
        }

        public void AddEmperorDefence()
        {
            EmperorDefence++;
            if (EmperorDefence > EmperorDefenctMax)
                EmperorDefence = EmperorDefenctMax;
        }

        private void EmperorBootsHurt()
        {
            EmperorDefence -= 5;//失去5层粘液覆层
            if (EmperorDefence < 0)
                EmperorDefence = 0;
        }

        private void EmperorSlimeBootsHitNPC(NPC target)
        {
            if (target.HasBuff(BuffID.Slimed) && EmperorArmorCD == 0)
            {
                if (HasEffect(Items.Gels.EmperorSlimeBoots.DefenceSet))//凝胶防御套
                {
                    EmperorArmorCD = 30;
                    AddEmperorDefence();
                }
                else if (HasEffect(Items.Gels.EmperorSlimeBoots.AttackSet))//凝胶攻击套
                {
                    EmperorArmorCD = 30;
                    Vector2 dir = Helper.NextVec2Dir();

                    int damage = Player.GetDamageByHeldItem(44);
                    Projectile.NewProjectile(Player.GetSource_FromThis(), target.Center + (dir * Main.rand.NextFloat(60, 80)),
                        dir * Main.rand.NextFloat(2, 4), ModContent.ProjectileType<Items.Gels.GelChaser>(), damage
                        , 2, -1, ai1: target.Center.X, ai2: target.Center.Y);
                }
            }
        }

        private void EmperorSlimeMove()
        {
            int hitX = -1;
            int hitY = -1;
            bool spawnDust = false;

            List<Point> xHits = null;

            #region X方向撞墙检测，只有冲刺时触发

            if (!Collision.IsClearSpotTest(Player.position + Player.velocity, 16f, Player.width, Player.height, fallThrough: false, fall2: false, (int)Player.gravDir, checkCardinals: true, checkSlopes: true))
                xHits = Collision.FindCollisionTile(Player.velocity.X > 0 ? 0 : 1, Player.position + Player.velocity, 16f, Player.width, Player.height, fallThrough: false, fall2: false, (int)Player.gravDir, checkCardinals: true);

            if ((Player.dashDelay < 0 || DashTimer != 0)
                && xHits != null && xHits.Count != 0)
                foreach (var tilePoint in xHits)
                {
                    Tile tile = Main.tile[tilePoint.X, tilePoint.Y];
                    if (!tile.HasUnactuatedTile || !Main.tileSolid[tile.TileType] || tile.BlockType != BlockType.Solid)
                        continue;

                    Vector2 center = tilePoint.ToWorldCoordinates();

                    if (center.Y > Player.position.Y && center.Y < Player.position.Y + Player.height
                        && (center.X < Player.position.X + 8 || center.X > Player.position.X + Player.width - 8))
                    {
                        hitX = tilePoint.X;
                        break;
                    }
                }

            #endregion

            #region Y方向撞墙检测

            if (Player.TouchedTiles.Count != 0)
                foreach (var tilePoint in Player.TouchedTiles)
                {
                    Tile tile = Main.tile[tilePoint.X, tilePoint.Y];
                    if (!tile.HasUnactuatedTile || !Main.tileSolid[tile.TileType] || tile.BlockType != BlockType.Solid)
                        continue;

                    Vector2 center = tilePoint.ToWorldCoordinates();
                    if (center.X > Player.position.X && center.X < Player.position.X + Player.width
                        && center.Y > Player.position.Y + Player.height)
                    {
                        hitY = tilePoint.Y;
                        break;
                    }
                }

            #endregion

            float bounceF = -0.7f;//弹力系数
            if (Player.controlDown)
                bounceF = -0.15f;//按住下键减少弹性
            else if (hitX != -1 && MathF.Abs(Player.velocity.X) > 4f)
            {
                Player.velocity.X *= bounceF;
                OnBouncy();
            }

            if (hitY != -1 && MathF.Abs(Player.velocity.Y) > 4f)
            {
                Player.velocity.Y *= bounceF;
                Player.RefreshMovementAbilities();
                OnBouncy();
            }

            if (spawnDust)
            {
                Vector2 dir = Player.velocity.SafeNormalize(Vector2.Zero);
                for (int i = 0; i < 16; i++)
                {
                    Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(Player.Hitbox)
                        , DustID.t_Slime, dir.RotateByRandom(-0.3f, 0.3f) * Main.rand.NextFloat(1, 5), 150, new Color(50, 150, 225, 50), Main.rand.NextFloat(1f, 1.5f));
                }

                Helper.PlayPitched(CoraliteSoundID.QueenSlime_Item154, Player.Center, pitch: 0.3f);
            }

            //弹起时触发，生成粒子，并触发套装效果
            void OnBouncy()
            {
                spawnDust = true;

                if (HasEffect(Items.Gels.EmperorSlimeBoots.DefenceSet) && Player.velocity.Length() > 6.5f)
                    AddEmperorDefence();

                if (HasEffect(Items.Gels.EmperorSlimeBoots.AttackSet))
                    Player.AddBuff(ModContent.BuffType<Items.Gels.EmperorSlimeBuff>(), 60 * 20);
            }
        }
    }
}
