using Coralite.Content.UI.UILib;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Coralite.Content.UI.MagikeGuideBook.Chapter2
{
    public class C2_1_CrystalCave7 : UIPage
    {
        public override string LocalizationCategory => "MagikeSystem";

        public static LocalizedText Date { get; private set; }
        public static LocalizedText Description { get; private set; }

        public static LocalizedText CrystalCave1 { get; private set; }

        public override void OnInitialize()
        {
            Date = this.GetLocalization("Date", () => "【泰拉历235年11月23日】");
            Description = this.GetLocalization("Description", () => "    这是我刚刚告知人们魔能的知识后发生的事。他们领导者对此很重视呢，明明人们因为龙和妖精的战争产生了很多的伤亡，却依旧在第一时间组建了探险队。“为了保护更多国民，不管是什么手段我都要试以试”，那位领导者是这么说的。顺便给你看个后续，我想你会用到这上面写的知识的。");
            CrystalCave1 = this.GetLocalization("CrystalCave1", () => "    真是难办，在水晶洞的研究笔记因为怪物的突然袭击没能带出来。还好我记性好，并且最重要的魔力晶体和玄武岩带够了，是个好消息。不想这些事了，今天的研究发现魔力晶体和玄武岩底座结合后发出的粉色光芒竟然有着转化物品的能力。具体点说，似乎是将物体的性质重构了，我得再多捣鼓捣鼓，顺便去找找蒂姆吧，是他的话应该能掌握这项技术。");
        }

        public override bool CanShowInBook => MagikeSystem.learnedMagikeBase;

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = Position + new Vector2(0, 60);
            Helper.DrawText(spriteBatch, Description.Value, PageWidth, new Vector2(Position.X, pos.Y), Vector2.Zero, Vector2.One
                , new Color(40, 40, 40), Color.White, out Vector2 textSize);

            pos += new Vector2(0, textSize.Y + 10);

            Utils.DrawBorderStringBig(spriteBatch, Date.Value, pos, Coralite.Instance.MagicCrystalPink
                , 0.8f, 0f, 0f);

            pos += new Vector2(0, 60);

            //文字段1
            Helper.DrawText(spriteBatch, CrystalCave1.Value, PageWidth, new Vector2(Position.X, pos.Y), Vector2.Zero, Vector2.One
                , new Color(40, 40, 40), Color.White, out textSize);

            pos += new Vector2(0, textSize.Y + 10);
        }
    }
}
