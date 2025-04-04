using Terraria;

namespace Coralite.Core.Prefabs
{
    public abstract class BaseVanillaDust : ModDust
    {
        private readonly int FrameCount;
        private readonly int UpdateTypeInner;
        private readonly string TexturePath;
        private readonly bool PathHasName;

        public BaseVanillaDust(int updateType, string texturePath, bool pathHasName = false, int frameCount = 3)
        {
            UpdateTypeInner = updateType;
            TexturePath = texturePath;
            PathHasName = pathHasName;
            FrameCount = frameCount;
        }

        public override string Texture => string.IsNullOrEmpty(TexturePath) ? base.Texture : TexturePath + (PathHasName ? string.Empty : Name);

        public override void SetStaticDefaults()
        {
            UpdateType = UpdateTypeInner;
        }

        public override void OnSpawn(Dust dust)
        {
            dust.frame = Texture2D.Frame(1, FrameCount, 0, Main.rand.Next(FrameCount));
            dust.rotation = Main.rand.NextFloat(6.282f);
            dust.color = Color.White;
        }

        public override bool PreDraw(Dust dust)
        {
            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, Lighting.GetColor(dust.position.ToTileCoordinates(), dust.color)
                , dust.rotation, dust.frame.Size() / 2, dust.scale, 0, 0);
            return false;
        }

    }
}
