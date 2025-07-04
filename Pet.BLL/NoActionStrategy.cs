namespace Pet.BLL
{
    /// <summary>
    /// 无操作策略 - 双击时不执行任何操作
    /// </summary>
    public class NoActionStrategy : IDoubleClickActionStrategy
    {
        public string Name => "无操作";

        public void Execute()
        {
            // 什么都不做
        }
    }
}
