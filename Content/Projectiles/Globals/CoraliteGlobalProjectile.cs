using Coralite.Content.ModPlayers;
using Coralite.Content.WorldGeneration;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Projectiles.Globals
{
    public partial class CoraliteGlobalProjectile : GlobalProjectile
    {
        public bool isBossProjectile;

        //private Vector2 rand;

        public override bool InstancePerEntity => true;

        public override void Load()
        {
            base.Load();
        }

        public override void Unload()
        {
            base.Unload();
        }

        public override void SetDefaults(Projectile entity)
        {

        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_Parent parent)
            {
                if (parent.Entity is NPC npc && npc.boss)
                    isBossProjectile = true;
            }

            if (source is EntitySource_ItemUse_WithAmmo itemUse
                && itemUse.Item.DamageType == DamageClass.Ranged
                && projectile.type is not (ProjectileID.VortexBeater or ProjectileID.Phantasm))
            {
                if (projectile.ModProjectile is not null && projectile.ModProjectile.Mod.Name != "Coralite" && projectile.ModProjectile.Mod is not ICoralite)
                    return;

                if (itemUse.Player.TryGetModPlayer(out CoralitePlayer cp))
                {
                    if (cp.split == 2)
                    {
                        projectile.damage = (int)(projectile.damage * 0.65f);
                        projectile.NewProjectileFromThis(projectile.Center, projectile.velocity.RotatedBy(0.1f), projectile.type,
                           (int)(projectile.damage * 0.65f), projectile.knockBack / 2, projectile.ai[0], projectile.ai[1], projectile.ai[2]);
                    }
                    else if (cp.split > 2)
                    {
                        projectile.damage = (int)(projectile.damage * 0.55f);
                        projectile.NewProjectileFromThis(projectile.Center, projectile.velocity.RotatedBy(0.1f), projectile.type,
                           (int)(projectile.damage * 0.5f), projectile.knockBack / 2, projectile.ai[0], projectile.ai[1], projectile.ai[2]);
                        projectile.NewProjectileFromThis(projectile.Center, projectile.velocity.RotatedBy(-0.1f), projectile.type,
                           (int)(projectile.damage * 0.5f), projectile.knockBack / 2, projectile.ai[0], projectile.ai[1], projectile.ai[2]);
                    }
                }
            }

            //rand = Main.rand.NextVector2Circular(128, 128);
        }

        public override void PostAI(Projectile projectile)
        {
        }

        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (CoraliteWorld.coralCatWorld)
            {
                switch (projectile.type)
                {
                    default:
                        break;
                    case ProjectileID.Boulder:
                        {
                            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "CatBoulder").Value;
                            Main.EntitySpriteDraw(mainTex, projectile.Center - Main.screenPosition, null, lightColor, projectile.rotation, mainTex.Size() / 2, projectile.scale, 0, 0);
                        }
                        return false;
                    case ProjectileID.BouncyBoulder:
                        {
                            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "RainbowCatBoulder").Value;
                            Main.EntitySpriteDraw(mainTex, projectile.Center - Main.screenPosition, null, lightColor, projectile.rotation, mainTex.Size() / 2, projectile.scale, 0, 0);
                        }
                        return false;
                }
            }
            //rand.Y -= 0.1f;

            //Effect effect = Filters.Scene["Crystal"].GetShader().Shader;

            //Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            //Matrix view = Main.GameViewMatrix.TransformationMatrix;
            //Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            //Texture2D noiseTex = GemTextures.CrystalNoises[(int)(Main.timeForVisualEffects / 7) % 20].Value;

            //effect.Parameters["transformMatrix"].SetValue(view * projection);
            //effect.Parameters["basePos"].SetValue((projectile.Center - Main.screenPosition + rand) * Main.GameZoomTarget);
            //effect.Parameters["scale"].SetValue(new Vector2(0.7f / Main.GameZoomTarget));
            //effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * (Main.gamePaused ? 0.02f : 0.01f));
            //effect.Parameters["lightRange"].SetValue(0.2f);
            //effect.Parameters["lightLimit"].SetValue(0.25f);
            //effect.Parameters["addC"].SetValue(0.65f);
            //effect.Parameters["highlightC"].SetValue(Color.White.ToVector4());
            //effect.Parameters["brightC"].SetValue(Color.Pink.ToVector4());
            //effect.Parameters["darkC"].SetValue(new Color(120, 20, 70).ToVector4());

            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, effect, Main.GameViewMatrix.ZoomMatrix);
            //Main.graphics.GraphicsDevice.Textures[1] = noiseTex;

            return base.PreDraw(projectile, ref lightColor);
        }

        public override void PostDraw(Projectile projectile, Color lightColor)
        {
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}
