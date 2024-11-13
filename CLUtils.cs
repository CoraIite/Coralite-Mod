using System;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria;
using System.IO;
using Terraria.GameInput;

namespace Coralite
{
    internal static class CLUtils
    {
        /// <summary>
        /// 一个额外的跳字方法，向游戏内打印对象的ToString内容
        /// </summary>
        /// <param name="obj"></param>
        public static void Domp(this object obj, Color color = default)
        {
            if (color == default)
            {
                color = Color.White;
            }
            if (obj == null)
            {
                Text("ERROR Is Null", Color.Red);
                return;
            }
            Text(obj.ToString(), color);
        }

        /// <summary>
        /// 一个额外的跳字方法，向控制台面板打印对象的ToString内容，并自带换行
        /// </summary>
        /// <param name="obj"></param>
        public static void DompInConsole(this object obj, bool outputLogger = true)
        {
            if (obj == null)
            {
                Console.WriteLine("ERROR Is Null");
                return;
            }
            string value = obj.ToString();
            Console.WriteLine(value);
            if (outputLogger)
            {
                Coralite.Instance.Logger.Info(value);
            }
        }

        /// <summary>
        /// 在游戏中发送文本消息
        /// </summary>
        /// <param name="message">要发送的消息文本</param>
        /// <param name="colour">（可选）消息的颜色,默认为 null</param>
        public static void Text(string message, Color? colour = null)
        {
            Color newColor = (Color)(colour == null ? Color.White : colour);
            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(message), (Color)(colour == null ? Color.White : colour));
                return;
            }
            Main.NewText(message, newColor);
        }

        /// <summary>
        /// 一个根据语言选项返回字符的方法
        /// </summary>
        public static string Translation(string Chinese = null, string English = null, string Spanish = null, string Russian = null)
        {
            string text = default(string);

            if (English == null)
            {
                English = "Invalid Character";
            }

            switch (Language.ActiveCulture.LegacyId)
            {
                case (int)GameCulture.CultureName.Chinese:
                    text = Chinese;
                    break;
                case (int)GameCulture.CultureName.Russian:
                    text = Russian;
                    break;
                case (int)GameCulture.CultureName.Spanish:
                    text = Spanish;
                    break;
                case (int)GameCulture.CultureName.English:
                    text = English;
                    break;
                default:
                    text = English;
                    break;
            }

            return text;
        }

        /// <summary>
        /// 检测玩家是否有效且正常存活
        /// </summary>
        /// <returns>返回 true 表示活跃，返回 false 表示为空或者已经死亡的非活跃状态</returns>
        public static bool Alives(this Player player)
        {
            return player != null && player.active && !player.dead;
        }


        /// <summary>
        /// 检测弹幕是否有效且正常存活
        /// </summary>
        /// <returns>返回 true 表示活跃，返回 false 表示为空或者已经死亡的非活跃状态</returns>
        public static bool Alives(this Projectile projectile)
        {
            return projectile != null && projectile.active && projectile.timeLeft > 0;
        }

        /// <summary>
        /// 检测NPC是否有效且正常存活
        /// </summary>
        /// <returns>返回 true 表示活跃，返回 false 表示为空或者已经死亡的非活跃状态</returns>
        public static bool Alives(this NPC npc)
        {
            return npc != null && npc.active && npc.timeLeft > 0;
        }

        /// <summary>
        /// 简单安全的获取一个单位向量，如果出现非法情况则会返回 <see cref="Vector2.Zero"/>
        /// </summary>
        public static Vector2 UnitVector(this Vector2 vr)
        {
            return vr.SafeNormalize(Vector2.Zero);
        }

        /// <summary>
        /// 检查指定玩家是否按下了鼠标键
        /// </summary>
        /// <param name="player">要检查的玩家</param>
        /// <param name="leftCed">是否检查左鼠标键，否则检测右鼠标键</param>
        /// <param name="netCed">是否进行网络同步检查</param>
        /// <returns>如果按下了指定的鼠标键，则返回true，否则返回false</returns>
        public static bool PressKey(this Player player, bool leftCed = true, bool netCed = true)
        {
            return (!netCed || Main.myPlayer == player.whoAmI) && (leftCed ? PlayerInput.Triggers.Current.MouseLeft : PlayerInput.Triggers.Current.MouseRight);
        }

        public static Vector2 To(this Vector2 vr1, Vector2 vr2) => vr2 - vr1;

        /// <summary>
        /// 获取玩家对象一个稳定的中心位置，考虑斜坡矫正与坐骑矫正，适合用于处理手持弹幕的位置获取
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static Vector2 GetPlayerStabilityCenter(this Player player) => player.MountedCenter.Floor() + new Vector2(0, player.gfxOffY);

        public static Color MultiStepColorLerp(float percent, params Color[] colors)
        {
            if (colors == null)
            {
                Text("MultiLerpColor: 空的颜色数组!");
                return Color.White;
            }
            float per = 1f / (colors.Length - 1f);
            float total = per;
            int currentID = 0;
            while (percent / total > 1f && currentID < colors.Length - 2)
            {
                total += per;
                currentID++;
            }
            return Color.Lerp(colors[currentID], colors[currentID + 1], (percent - (per * currentID)) / per);
        }

        /// <summary>
        /// 判断是否处于客户端状态，如果是在单人或者服务端下将返回false
        /// </summary>
        public static bool isClient => Main.netMode == NetmodeID.MultiplayerClient;
        /// <summary>
        /// 判断是否处于服务端状态，如果是在单人或者客户端下将返回false
        /// </summary>
        public static bool isServer => Main.netMode == NetmodeID.Server;
        /// <summary>
        /// 仅判断是否处于单人状态，在单人模式下返回true
        /// </summary>
        public static bool isSinglePlayer => Main.netMode == NetmodeID.SinglePlayer;
        /// <summary>
        /// 检查一个 Projectile 对象是否属于当前客户端玩家拥有的，如果是，返回true
        /// </summary>
        public static bool IsOwnedByLocalPlayer(this Projectile projectile) => projectile.owner == Main.myPlayer;

        public static void WriteCLNetID(this ModPacket modPacket, CLNetWorkEnum id) => modPacket.Write((byte)id);
    }
}
