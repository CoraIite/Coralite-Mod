using Coralite.Core;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.ModPlayers
{
    public partial class  CoralitePlayer
    {
        /// <summary>
        /// 完美格挡时间，在格挡时增加，每帧递减
        /// </summary>
        public int parryTime;

        public void UpdateParry()
        {
            if (parryTime > 0)
            {
                parryTime--;
                if (parryTime <= 0)
                {
                    SoundEngine.PlaySound(CoraliteSoundID.MaxMana, Player.Center);
                    float rot = Main.rand.NextFloat(6.282f);
                    for (int i = 0; i < 8; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(Player.Center, DustID.Clentaminator_Red, (rot + (i * MathHelper.TwoPi / 8)).ToRotationVector2() * 3,
                            255, Scale: Main.rand.Next(20, 26) * 0.1f);
                        dust.noLight = true;
                        dust.noGravity = true;
                    }
                }
            }
        }

    }
}
