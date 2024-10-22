using Coralite.Core;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.UI.Chat;

namespace Coralite.Content.Tiles.MagikeSeries2
{
    public class PremissionProj:ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];

        public static LocalizedText SoulOfLightText {  get; private set; }
        public static LocalizedText SoulOfNightText {  get; private set; }
        public static LocalizedText PremissionText {  get; private set; }

        private TextDrawer[] textDrawers;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            SoulOfLightText=this.GetLocalization(nameof(SoulOfLightText));
            SoulOfNightText = this.GetLocalization(nameof(SoulOfNightText));
            PremissionText = this.GetLocalization(nameof(PremissionText));
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            SoulOfLightText = null;
            SoulOfNightText = null;
            PremissionText = null;
        }

        private struct TextDrawer
        {
            public string text;
            public float centerX;
            public float alpha;


            public void Draw(float y,float factor,Color c,Color darkC)
            {
                Utils.DrawBorderString(Main.spriteBatch, text, new Vector2(centerX,y),c);
            }
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (Projectile.localAI[0]==0)
            {
                Projectile.localAI[0] = 1;

                string text;
                switch (State)
                {
                    default:
                        Projectile.Kill();
                        return;
                    case 0:
                        text = SoulOfLightText.Value;
                        break;
                    case 1:
                        text = SoulOfNightText.Value;
                        break;
                    case 2:
                        text = PremissionText.Value;
                        break;
                }

                string[] texts= text.Split(' ');
                float[] lengths= new float[texts.Length];
                float total = 0;
                for (int i = 0; i < lengths.Length; i++)
                {
                    lengths[i] = ChatManager.GetStringSize(FontAssets.MouseText.Value, texts[i], Vector2.One).X; 
                    total += lengths[i];
                }

                total += lengths.Length * 16;
                textDrawers =new TextDrawer[lengths.Length];

                //生成文字
                float x = Projectile.Center.X - total / 2;
                for (int i = 0; i < textDrawers.Length; i++)
                {
                    textDrawers[i] = new TextDrawer()
                    {
                        text = texts[i],
                        centerX = x + lengths[i] / 2,
                    };

                    x+= lengths[i]+16; 
                }
            }

            Timer++;
            if (Timer>60*4)
                Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {

            return false;
        }
    }
}
