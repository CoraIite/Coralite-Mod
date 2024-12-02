using System;

namespace Coralite.Helpers
{
    public static class CoraliteUtils
    {
        /// <summary>
        /// 一个额外的跳字方法，向游戏内打印对象的ToString内容
        /// </summary>
        /// <param name="obj"></param>
        public static void Dump(this object obj, Color color = default)
        {
            if (color == default)
            {
                color = Color.White;
            }
            if (obj == null)
            {
                VaultUtils.Text("ERROR Is Null", Color.Red);
                return;
            }
            VaultUtils.Text(obj.ToString(), color);
        }

        /// <summary>
        /// 一个额外的跳字方法，向控制台面板打印对象的ToString内容，并自带换行
        /// </summary>
        /// <param name="obj"></param>
        public static void DumpInConsole(this object obj, bool outputLogger = true)
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
    }
}
