namespace Coralite
{
    /// <summary>
    /// 继承这个就代表你有对Coralite的扩展内容
    /// </summary>
    public interface ICoralite
    {
        /// <summary>
        /// 各种数据文件的路径
        /// </summary>
        string DataPath { get; }
    }
}
