using Coralite.Content.ModPlayers;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class Luminward() : BaseAccessory(ModContent.RarityType<CrystallineMagikeRarity>(), Item.sellPrice(0, 2))
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp) && cp.HurtTimer >= 60 * 12)
            {
                player.invis = true;
                player.GetCritChance(DamageClass.Generic) += 6;
                cp.AddEffect(nameof(Luminward));

                if (cp.HurtTimer == 60 * 12)
                {
                    Helper.PlayPitched(CoraliteSoundID.WindyBalloon_NPCDeath63, player.Center);
                    Helper.PlayPitched(CoraliteSoundID.ShimmerContract, player.Center);

                    var p = PRTLoader.NewParticle<LuminwardParticle>(player.Center, Vector2.Zero, Color.White , 1);
                    p.player = player;
                    p.Rotation = Main.rand.NextFloat(-0.4f,0.4f);
                    p.effect = Main.rand.NextFromList(SpriteEffects.None, SpriteEffects.FlipHorizontally);
                }
            }
        }
    }

    public class LuminwardParticle : Particle
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public Player player;
        public SpriteEffects effect;

        public override void AI()
        {
            Lighting.AddLight(Position, new Vector3(0.4f, 0.4f, 0.4f));
            if (player != null)
                Position = player.Center+new Vector2(0,-8);

            if (Time % 3 == 0)
            {
                if (++Frame.Y > 16)
                    active = false;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            var frameBox = TexValue.Frame(1, 17, 0, Frame.Y);
            spriteBatch.Draw(TexValue, Position - Main.screenPosition, frameBox, Color, Rotation, frameBox.Size() / 2, Scale, effect, 0);
            return false;
        }
    }

    public class LuminwardParticleExplosion:Particle
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public Player player;
        public SpriteEffects effect;

        public override void AI()
        {
            Lighting.AddLight(Position, new Vector3(0.4f, 0.4f, 0.4f));
            if (player != null)
                Position = player.Center + new Vector2(0, -8);

            if (Time % 3 == 0)
            {
                if (++Frame.Y > 11)
                    active = false;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            var frameBox = TexValue.Frame(1, 12, 0, Frame.Y);
            spriteBatch.Draw(TexValue, Position - Main.screenPosition, frameBox, Color, Rotation, frameBox.Size() / 2, Scale, effect, 0);
            return false;
        }
    }
}
