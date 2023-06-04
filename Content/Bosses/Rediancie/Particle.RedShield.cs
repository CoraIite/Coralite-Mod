using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.Rediancie
{
    public class RedShield : ModParticle
    {
        public override string Texture => AssetDirectory.Rediancie+"RedShield";
        public static Asset<Texture2D> flowTex;

        public override void Load()
        {
            flowTex = ModContent.Request<Texture2D>(AssetDirectory.Rediancie + "RedShield_Flow");
        }

        public override bool ShouldUpdateCenter(Particle particle) => false;

        public override void OnSpawn(Particle particle)
        {
            particle.color = Coralite.Instance.RedJadeRed;
            particle.color.A = 0;
            particle.rotation = Main.rand.NextFloat(6.282f);
            particle.oldRot = new float[2];
            particle.scale = 0f;
            particle.shouldKilledOutScreen = false;
        }

        public override void Update(Particle particle)
        {
            particle.rotation += 0.06f;

            if (particle.oldRot[0] == 0f)
            {
                particle.scale += 0.05f;
                particle.color.A += 255 / 16;
                if (particle.scale > 0.8f)
                {
                    particle.scale = 0.8f;
                    particle.color.A = 255;
                    particle.oldRot[0] = 1;
                }
            }

            if (GetDatas(particle,out NPC rediancie))
            {
                particle.center = rediancie.Center;
            }

            particle.fadeIn--;

            if (particle.fadeIn < 0)
                particle.oldRot[1] = 1;

            if (particle.oldRot[1] == 1)
            {
                particle.color.A -= 255 / 16;
                if (particle.color.A < 10)
                    particle.active = false;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Particle particle)
        {
            Vector2 center = particle.center - Main.screenPosition;
            spriteBatch.Draw(Texture2D.Value, center, null, particle.color, particle.rotation, Texture2D.Size() / 2, particle.scale, SpriteEffects.None, 0);

            float extraRot1 = particle.rotation + particle.fadeIn * 0.1f;
            float extraRot2 = particle.rotation + particle.fadeIn * 0.05f;
            Vector2 flowOrigin = flowTex.Size() / 2;

            spriteBatch.Draw(flowTex.Value,center, null, particle.color, extraRot1, flowOrigin, particle.scale - 0.1f, SpriteEffects.None, 0);
            spriteBatch.Draw(flowTex.Value, center, null, new Color(255,255,255,particle.color.A*3/4), extraRot1+extraRot2, flowOrigin, particle.scale - 0.2f, SpriteEffects.FlipHorizontally, 0);
            spriteBatch.Draw(flowTex.Value, center, null, particle.color, extraRot2 + 3.141f, flowOrigin, particle.scale - 0.2f, SpriteEffects.FlipHorizontally, 0);
        }



        public static void Spawn(NPC rediancie,int maxTime)
        {
            Particle particle = Particle.NewParticleDirect(rediancie.Center, Vector2.Zero, CoraliteContent.ParticleType<RedShield>());
            particle.datas=new object[1]
            {
                rediancie
            };
            particle.fadeIn = maxTime;
        }

        public static void Kill()
        {
            int type = CoraliteContent.ParticleType<RedShield>();
            foreach (var particle in ParticleSystem.Particles.Where(p=>p.active&&p.type==type))
            {
                particle.fadeIn = -1;
            }
        }

        private bool GetDatas(Particle particle,out NPC rediancie)
        {
            if (particle.datas == null || particle.datas[0]is not NPC)
            {
                rediancie = null;
                return false;
            }

            rediancie = (NPC)particle.datas[0];
            return true;
        }
    }
}
