using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Projectiles.Projectiles_Shoot
{
    public class HyacinthBullet2 : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles_Shoot + Name;

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 2;
            Projectile.light = 0.5f;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 7;
            Projectile.scale = 1.18f;
            Projectile.timeLeft = 600;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.alpha > 160)
                return Color.Transparent;

            Color color = Color.Lerp(Color.Red, GetColor(), Projectile.alpha / 160f);

            color.A = 100;
            return color;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
        }

        public override void AI()
        {
            float velLength = Projectile.velocity.Length();
            if (Projectile.alpha > 0)
                Projectile.alpha -= (byte)(velLength * 0.5f);

            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.damage = (int)(Projectile.damage * 0.85f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

            return true;
        }


        public Color GetColor()
        {
            switch (-Projectile.ai[0])
            {
                default: break;
                case 1:     //彩弹枪
                    return Color.Silver;
                case 2:     //超级星星炮
                    return Color.LightYellow;
                case 3:     //星星炮
                    return Color.Yellow;
                case 4:     //玛瑙爆破枪
                    return Color.Purple;
                case 5:     //维纳斯万能枪
                    return new Color(140, 255, 102);
                case 6:     //链式机枪
                    return new Color(196, 17, 18);
                case 7:     //外星泡泡枪
                    return new Color(233, 148, 248);
                case 8:     //星旋机枪
                    return new Color(0, 242, 170);
                case 9:     //太空海豚机枪
                    return new Color(147, 227, 236);
                case 10:    //邓氏鲨
                    return new Color(57, 140, 125);
                case 11:    //巨兽鲨
                    return new Color(147, 98, 103);
                case 12:    //迷你鲨
                    return new Color(195, 131, 49);
                case 13:    //满天星
                    return Color.White;
                case 14:    //雪花莲
                    return new Color(152, 192, 70);
                case 15:    //迷迭香
                    return new Color(235, 141, 207);
                case 16:    //迷迭香2
                    return new Color(235, 141, 207);
                case 17:    //幽兰
                    return new Color(95, 120, 233);
                case 18:    //木蜡
                    return new Color(125, 165, 79);
                case 19:    //火枪
                    return new Color(165, 165, 165);
                case 20:    //夺命枪
                    return new Color(237, 28, 36);
                case 21:    //凤凰爆破枪
                    return new Color(252, 145, 28);
                case 22:    //手枪
                    return new Color(127, 127, 127);
            }

            return Color.White;
        }
    }
}