using Coralite.Core.Loaders;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;

namespace Coralite.Core.Systems.ParticleSystem
{
    //[Autoload(Side = ModSide.Client)]
    public abstract class ModParticle : ModTexturedType
    {
        public int Type { get; internal set; }
        //(GetType().Namespace + "." + Name).Replace('.', '/');    GetType().FullName.Replace('.', '/');
        public override string Texture => AssetDirectory.Particles + Name;

        public Asset<Texture2D> Texture2D { get; private set; }

        #region 子类可用方法

        public virtual void OnSpawn(Particle particle) { }

        public virtual void Update(Particle particle) { }

        public virtual bool ShouldUpdateCenter(Particle particle) => true;

        public virtual void Draw(SpriteBatch spriteBatch, Particle particle)
        {
            ModParticle modParticle = ParticleLoader.GetParticle(particle.type);
            Rectangle frame = particle.frame;
            Vector2 origin = new Vector2(frame.Width / 2, frame.Height / 2);

            spriteBatch.Draw(modParticle.Texture2D.Value, particle.center - Main.screenPosition, frame, particle.color, particle.rotation, origin, particle.scale, SpriteEffects.None, 0f);
        }

        public virtual void DrawInUI(SpriteBatch spriteBatch, Particle particle)
        {
            ModParticle modParticle = ParticleLoader.GetParticle(particle.type);
            Rectangle frame = particle.frame;
            Vector2 origin = new Vector2(frame.Width / 2, frame.Height / 2);

            spriteBatch.Draw(modParticle.Texture2D.Value, particle.center, frame, particle.color, particle.rotation, origin, particle.scale, SpriteEffects.None, 0f);
        }

        #endregion

        protected sealed override void Register()
        {
            ModTypeLookup<ModParticle>.Register(this);

            ParticleLoader.modParticles ??= new List<ModParticle>();
            ParticleLoader.modParticles.Add(this);

            Type = ParticleLoader.ReserveParticleID();

            Texture2D = !string.IsNullOrEmpty(Texture) ? ModContent.Request<Texture2D>(Texture) : TextureAssets.Dust;
        }
    }
}
