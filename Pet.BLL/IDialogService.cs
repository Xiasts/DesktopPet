using System.Threading.Tasks;

namespace Pet.BLL
{
    /// <summary>
    /// 对话服务接口 - 适配器模式的基础接口
    /// 为未来接入不同的AI服务提供统一的接口
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// 异步获取AI回复
        /// </summary>
        /// <param name="userInput">用户输入的文本</param>
        /// <returns>AI的回复文本</returns>
        Task<string> GetResponseAsync(string userInput);

        /// <summary>
        /// 服务名称
        /// </summary>
        string ServiceName { get; }

        /// <summary>
        /// 服务是否可用
        /// </summary>
        bool IsAvailable { get; }
    }
}
