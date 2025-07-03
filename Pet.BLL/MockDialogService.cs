using System;
using System.Threading.Tasks;
using Pet.Common;

namespace Pet.BLL
{
    /// <summary>
    /// 模拟对话服务 - 用于演示适配器模式
    /// 在真实项目中，这里会被替换为真正的AI服务适配器
    /// </summary>
    public class MockDialogService : IDialogService
    {
        public string ServiceName => "模拟AI助手";

        public bool IsAvailable => true;

        /// <summary>
        /// 模拟AI回复
        /// </summary>
        /// <param name="userInput">用户输入</param>
        /// <returns>模拟的AI回复</returns>
        public async Task<string> GetResponseAsync(string userInput)
        {
            // 模拟网络延迟
            await Task.Delay(SharedRandom.Next(500, 1500));

            // 根据用户输入生成不同的回复
            string response = GenerateResponse(userInput);
            
            return response;
        }

        /// <summary>
        /// 根据用户输入生成模拟回复
        /// </summary>
        /// <param name="input">用户输入</param>
        /// <returns>模拟回复</returns>
        private string GenerateResponse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return "皮卡皮卡？你想说什么呢？";
            }

            string lowerInput = input.ToLower();

            // 问候相关
            if (lowerInput.Contains("你好") || lowerInput.Contains("hello") || lowerInput.Contains("hi"))
            {
                string[] greetings = {
                    "皮卡皮卡！你好呀！",
                    "皮卡丘很高兴见到你！⚡",
                    "你好！今天过得怎么样？",
                    "皮卡～欢迎和我聊天！"
                };
                return greetings[SharedRandom.Next(greetings.Length)];
            }

            // 询问名字
            if (lowerInput.Contains("名字") || lowerInput.Contains("叫什么"))
            {
                return "我是皮卡丘！皮卡皮卡～⚡\n你可以叫我小皮！";
            }

            // 询问能力
            if (lowerInput.Contains("能做什么") || lowerInput.Contains("功能"))
            {
                return "我可以：\n• 陪你聊天 💬\n• 提醒你的日程 ⏰\n• 在桌面上卖萌 🎭\n• 释放十万伏特！⚡";
            }

            // 夸奖相关
            if (lowerInput.Contains("可爱") || lowerInput.Contains("萌"))
            {
                return "皮卡皮卡～谢谢夸奖！(〃∀〃)";
            }

            // 询问心情
            if (lowerInput.Contains("心情") || lowerInput.Contains("怎么样"))
            {
                string[] moods = {
                    "皮卡丘今天心情很好！⚡",
                    "有点想吃番茄酱了...",
                    "皮卡皮卡～充满活力！",
                    "和你聊天让我很开心！"
                };
                return moods[SharedRandom.Next(moods.Length)];
            }

            // 告别相关
            if (lowerInput.Contains("再见") || lowerInput.Contains("拜拜") || lowerInput.Contains("bye"))
            {
                return "皮卡皮卡～再见！记得常来找我玩哦！⚡";
            }

            // 默认回复
            string[] defaultResponses = {
                "皮卡皮卡？我不太明白，但是很高兴和你聊天！",
                "虽然我听不太懂，但是皮卡丘会努力理解的！⚡",
                "皮卡～你说的很有趣呢！",
                "皮卡皮卡！能再说详细一点吗？",
                "我是皮卡丘，虽然不太懂，但我很想帮助你！"
            };

            return defaultResponses[SharedRandom.Next(defaultResponses.Length)];
        }
    }
}
