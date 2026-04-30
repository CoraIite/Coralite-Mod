using Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor;
using Coralite.Content.CoraliteNotes.Readfragment;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.SlimeChapter1
{
    public class Slime1DangerousPage : DangerousPage<Slime1Knowledge>
    {
        public override int NPCID => ModContent.NPCType<SlimeEmperor>();

        public static LocalizedText Title { get; private set; }
        public static LocalizedText Description { get; private set; }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Title = this.GetLocalization(nameof(Title));
            Description = this.GetLocalization(nameof(Description));
        }

        public override void AddNodes()
        {
            float x = -200;
            float y = -160;

            Color red = Color.Red * 0.8f;

            DangerousButton b1 = NewButton(new Vector2(x, y), (int)Slime1Knowledge.Dangerous.LifeMaxBonus_1, KnowledgeButtonType.Rune).SetColor(red);
            x += 80;
            DangerousButton b2 = NewButton(new Vector2(x, y+10), (int)Slime1Knowledge.Dangerous.LifeMaxBonus_2, KnowledgeButtonType.Rune).SetColor(red);
            x += 80;
            DangerousButton b3 = NewButton(new Vector2(x, y+20), (int)Slime1Knowledge.Dangerous.LifeMaxBonus_3, KnowledgeButtonType.Rune).SetColor(red);

            b1.AddSameLevelNode(b2);
            b1.AddSameLevelNode(b3);
            b2.AddSameLevelNode(b3);

            x = -180;
            y += 80;

            DangerousButton b4 = NewButton(new Vector2(x, y), (int)Slime1Knowledge.Dangerous.DefenceBonus_1, KnowledgeButtonType.Metal).SetColor(Color.Gray);
            x += 80;
            DangerousButton b5 = NewButton(new Vector2(x, y+15), (int)Slime1Knowledge.Dangerous.DefenceBonus_2, KnowledgeButtonType.Metal).SetColor(Color.Gray);

            b4.AddSameLevelNode(b5);

            x = -200;
            y += 80;

            DangerousButton b6 = NewButton(new Vector2(x, y), (int)Slime1Knowledge.Dangerous.SpeedBonus1_1, KnowledgeButtonType.Ball).SetColor(Color.DarkSeaGreen);
            x += 80;
            DangerousButton b7 = NewButton(new Vector2(x, y+10), (int)Slime1Knowledge.Dangerous.SpeedBonus2_1, KnowledgeButtonType.Ball).SetColor(Color.ForestGreen);
            x += 80;
            DangerousButton b8 = NewButton(new Vector2(x, y+20), (int)Slime1Knowledge.Dangerous.SpeedBonus3_1, KnowledgeButtonType.Ball).SetColor(Color.LightSeaGreen);

            b6.AddPostNode(b7);
            b7.AddPostNode(b8);

            x = -200 + 80 * 4;
            y = -180 + 80;

            DangerousButton b9 = NewButton(new Vector2(x, y), (int)Slime1Knowledge.Dangerous.WeaponLimit_4, KnowledgeButtonType.Reel);
            x += 80;
            DangerousButton b10 = NewButton(new Vector2(x, y), (int)Slime1Knowledge.Dangerous.ArmorLimit_4, KnowledgeButtonType.Reel);
            x -= 80;
            y += 100;
            DangerousButton b11 = NewButton(new Vector2(x, y), (int)Slime1Knowledge.Dangerous.HitLimit_3, KnowledgeButtonType.Reel).SetColor(red);
            x += 80;
            DangerousButton b12 = NewButton(new Vector2(x, y), (int)Slime1Knowledge.Dangerous.HitLimit_S_5, KnowledgeButtonType.Reel).SetColor(red);
            b11.AddSameLevelNode(b12);

            x = -220;
            y = -160 + 80 * 3 + 30;

            DangerousButton b13 = NewButton(new Vector2(x-10, y), (int)Slime1Knowledge.Dangerous.FlippyBonus_1, KnowledgeButtonType.Wild).SetColor(Color.CornflowerBlue);
            y += 80;
            DangerousButton b14 = NewButton(new Vector2(x, y), (int)Slime1Knowledge.Dangerous.FlippyBonus_S_2, KnowledgeButtonType.Wild).SetColor(Color.CornflowerBlue);
            b13.AddPostNode(b14);

            x += 80;
            y -= 70;

            DangerousButton b15 = NewButton(new Vector2(x-10, y), (int)Slime1Knowledge.Dangerous.AvatarBonus_1, KnowledgeButtonType.Wild).SetColor(Color.CornflowerBlue);
            y += 80;
            DangerousButton b16 = NewButton(new Vector2(x, y), (int)Slime1Knowledge.Dangerous.AvatarBonus_S_2, KnowledgeButtonType.Wild).SetColor(Color.CornflowerBlue);
            b15.AddPostNode(b16);

            x += 100;
            y -= 80;

            DangerousButton b17 = NewButton(new Vector2(x+10, y), (int)Slime1Knowledge.Dangerous.GelBallBonus_P1_2, KnowledgeButtonType.Wild).SetColor(Color.CornflowerBlue);
            y += 80;
            DangerousButton b18 = NewButton(new Vector2(x, y), (int)Slime1Knowledge.Dangerous.GelBallBonus_P2_2, KnowledgeButtonType.Wild).SetColor(Color.CornflowerBlue);
            b17.AddSameLevelNode(b18);

            x += 80;
            y -= 90;

            DangerousButton b19 = NewButton(new Vector2(x+10, y), (int)Slime1Knowledge.Dangerous.SpilkeBallBonus_P1_2, KnowledgeButtonType.Wild).SetColor(Color.CornflowerBlue);
            y += 80;
            DangerousButton b20 = NewButton(new Vector2(x, y), (int)Slime1Knowledge.Dangerous.SpilkeBallBonus_P2_2, KnowledgeButtonType.Wild).SetColor(Color.CornflowerBlue);
            b19.AddSameLevelNode(b20);

            x += 100;
            y -= 70;

            DangerousButton b21 = NewButton(new Vector2(x, y), (int)Slime1Knowledge.Dangerous.CrownBonus_1, KnowledgeButtonType.Wild).SetColor(Color.CornflowerBlue);
            y += 80;
            DangerousButton b22 = NewButton(new Vector2(x, y), (int)Slime1Knowledge.Dangerous.CrownBonus_S_2, KnowledgeButtonType.Wild).SetColor(Color.CornflowerBlue);
            b21.AddSameLevelNode(b22);

            x += 100;
            y -= 70;

            DangerousButton b23 = NewButton(new Vector2(x, y), (int)Slime1Knowledge.Dangerous.ElasticBonus_1, KnowledgeButtonType.Wild).SetColor(Color.CornflowerBlue);
            y += 80;
            DangerousButton b24 = NewButton(new Vector2(x, y), (int)Slime1Knowledge.Dangerous.StickyBonus_1, KnowledgeButtonType.Wild).SetColor(Color.CornflowerBlue);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            DrawTitleH2(spriteBatch, Title, Color.SkyBlue);
            DrawParaNormal(spriteBatch, Description, Position.Y + TitleHeight, out _);
        }
    }
}
