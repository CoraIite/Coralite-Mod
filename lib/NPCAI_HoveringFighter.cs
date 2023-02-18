using Terraria;

namespace Coralite.lib
{
    public class NPCAI_HoveringFighter
    {
        private void VanillaAI_Inner(NPC NPC)
        {
            bool flag17 = false;

            if (NPC.justHit)
                NPC.ai[2] = 0f;

            if (NPC.ai[2] >= 0f)
            {
                int num297 = 16;
                bool flag19 = false;
                bool flag20 = false;
                if (NPC.position.X > NPC.ai[0] - num297 && NPC.position.X < NPC.ai[0] + num297)
                    flag19 = true;
                else if ((NPC.velocity.X < 0f && NPC.direction > 0) || (NPC.velocity.X > 0f && NPC.direction < 0))
                    flag19 = true;

                num297 += 24;
                if (NPC.position.Y > NPC.ai[1] - num297 && NPC.position.Y < NPC.ai[1] + num297)
                    flag20 = true;

                if (flag19 && flag20)
                {
                    NPC.ai[2] += 1f;
                    if (NPC.ai[2] >= 30f && num297 == 16)
                        flag17 = true;

                    if (NPC.ai[2] >= 60f)
                    {
                        NPC.ai[2] = -200f;
                        NPC.direction *= -1;
                        NPC.velocity.X *= -1f;
                        NPC.collideX = false;
                    }
                }
                else
                {
                    NPC.ai[0] = NPC.position.X;
                    NPC.ai[1] = NPC.position.Y;
                    NPC.ai[2] = 0f;
                }

                NPC.TargetClosest();
            }
            else
            {
                NPC.ai[2] += 1f;

                if (Main.player[NPC.target].position.X + (Main.player[NPC.target].width / 2) > NPC.position.X + (NPC.width / 2))
                    NPC.direction = -1;
                else
                    NPC.direction = 1;
            }

            int num298 = (int)(NPC.Center.X / 16f) + NPC.direction * 2;
            int num299 = (int)((NPC.position.Y + NPC.height) / 16f);
            bool flag21 = true;
            int num300 = 3;

            if (NPC.position.Y + NPC.height > Main.player[NPC.target].position.Y)
            {
                for (int num329 = num299; num329 < num299 + num300; num329++)
                {
                    //if (Main.tile[num298, num329] == null)
                    //    Main.tile[num298, num329] = new Tile();

                    if ((/*Main.tile[num298, num329].nactive() &&*/ Main.tileSolid[Main.tile[num298, num329].TileType]) || Main.tile[num298, num329].LiquidType > 0)
                    {
                        flag21 = false;
                        break;
                    }
                }
            }

            if (Main.player[NPC.target].npcTypeNoAggro[NPC.type])
            {
                bool flag23 = false;
                for (int num330 = num299; num330 < num299 + num300 - 2; num330++)
                {
                    if ((/*Main.tile[num298, num330].nactive() &&*/ Main.tileSolid[Main.tile[num298, num330].TileType]) || Main.tile[num298, num330].LiquidType > 0)
                    {
                        flag23 = true;
                        break;
                    }
                }

                NPC.directionY = (!flag23).ToDirectionInt();
            }

            if (flag17)
                flag21 = true;

            if (flag21)
            {
                NPC.velocity.Y += 0.1f;

                if (NPC.velocity.Y > 3f)
                    NPC.velocity.Y = 3f;
            }
            else
            {
                if (NPC.directionY < 0 && NPC.velocity.Y > 0f)
                    NPC.velocity.Y -= 0.1f;

                if (NPC.velocity.Y < -4f)
                    NPC.velocity.Y = -4f;
            }


            if (NPC.collideX)
            {
                NPC.velocity.X = NPC.oldVelocity.X * -0.4f;
                if (NPC.direction == -1 && NPC.velocity.X > 0f && NPC.velocity.X < 1f)
                    NPC.velocity.X = 1f;

                if (NPC.direction == 1 && NPC.velocity.X < 0f && NPC.velocity.X > -1f)
                    NPC.velocity.X = -1f;
            }

            if (NPC.collideY)
            {
                NPC.velocity.Y = NPC.oldVelocity.Y * -0.25f;
                if (NPC.velocity.Y > 0f && NPC.velocity.Y < 1f)
                    NPC.velocity.Y = 1f;

                if (NPC.velocity.Y < 0f && NPC.velocity.Y > -1f)
                    NPC.velocity.Y = -1f;
            }

            float num332 = 2f;

            if (NPC.direction == -1 && NPC.velocity.X > 0f - num332)
            {
                NPC.velocity.X -= 0.1f;
                if (NPC.velocity.X > num332)
                    NPC.velocity.X -= 0.1f;
                else if (NPC.velocity.X > 0f)
                    NPC.velocity.X += 0.05f;

                if (NPC.velocity.X < 0f - num332)
                    NPC.velocity.X = 0f - num332;
            }
            else if (NPC.direction == 1 && NPC.velocity.X < num332)
            {
                NPC.velocity.X += 0.1f;
                if (NPC.velocity.X < 0f - num332)
                    NPC.velocity.X += 0.1f;
                else if (NPC.velocity.X < 0f)
                    NPC.velocity.X -= 0.05f;

                if (NPC.velocity.X > num332)
                    NPC.velocity.X = num332;
            }

            num332 = 1.5f;
            if (NPC.directionY == -1 && NPC.velocity.Y > 0f - num332)
            {
                NPC.velocity.Y -= 0.04f;
                if (NPC.velocity.Y > num332)
                    NPC.velocity.Y -= 0.05f;
                else if (NPC.velocity.Y > 0f)
                    NPC.velocity.Y += 0.03f;

                if (NPC.velocity.Y < 0f - num332)
                    NPC.velocity.Y = 0f - num332;
            }
            else if (NPC.directionY == 1 && NPC.velocity.Y < num332)
            {
                NPC.velocity.Y += 0.04f;
                if (NPC.velocity.Y < 0f - num332)
                    NPC.velocity.Y += 0.05f;
                else if (NPC.velocity.Y < 0f)
                    NPC.velocity.Y -= 0.03f;

                if (NPC.velocity.Y > num332)
                    NPC.velocity.Y = num332;
            }

        }
    }
}
