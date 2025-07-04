using System;

namespace Pet.BLL
{
    /// <summary>
    /// 聊天策略 - 双击时打开聊天对话框
    /// </summary>
    public class ChatStrategy : IDoubleClickActionStrategy
    {
        public string Name => "聊天对话";

        public event Action<string, int> OnActionMessage;

        /// <summary>
        /// 触发聊天功能的事件
        /// </summary>
        public event Action OnChatRequested;

        public void Execute()
        {
            try
            {
                // 触发聊天请求事件，让UI层处理具体的聊天逻辑
                OnChatRequested?.Invoke();
                OnActionMessage?.Invoke("皮卡！准备开始聊天...", 2000);
            }
            catch (Exception ex)
            {
                OnActionMessage?.Invoke($"皮卡...聊天功能出现问题：{ex.Message}", 4000);
            }
        }
    }
}
