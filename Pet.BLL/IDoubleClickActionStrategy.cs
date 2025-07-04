using System;

namespace Pet.BLL
{
    /// <summary>
    /// 定义双击行为的策略接口
    /// </summary>
    public interface IDoubleClickActionStrategy
    {
        /// <summary>
        /// 策略的名称，用于在UI上显示
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 执行具体操作
        /// </summary>
        void Execute();

        /// <summary>
        /// 向UI层传递消息的事件
        /// </summary>
        event Action<string, int> OnActionMessage;
    }
}
