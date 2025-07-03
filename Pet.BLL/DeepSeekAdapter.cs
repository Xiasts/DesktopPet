using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Pet.BLL
{
    /// <summary>
    /// DeepSeek AI适配器 - 适配器模式的具体实现
    /// </summary>
    public class DeepSeekAdapter : IDialogService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private const string API_BASE_URL = "https://api.deepseek.com/v1/chat/completions";

        public string ServiceName => "DeepSeek AI";

        public bool IsAvailable => !string.IsNullOrEmpty(_apiKey);

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="apiKey">DeepSeek API Key</param>
        public DeepSeekAdapter(string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            
            // 设置请求头
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        /// <summary>
        /// 获取AI回复
        /// </summary>
        /// <param name="userInput">用户输入</param>
        /// <returns>AI回复</returns>
        public async Task<string> GetResponseAsync(string userInput)
        {
            try
            {
                if (!IsAvailable)
                {
                    return "抱歉，DeepSeek AI服务暂时不可用。请检查API配置。";
                }

                // 构建请求体
                var requestBody = new
                {
                    model = "deepseek-chat",
                    messages = new[]
                    {
                        new
                        {
                            role = "system",
                            content = "你是一只可爱的皮卡丘桌面宠物，请用皮卡丘的语气和用户对话。回复要简洁有趣，偶尔加上'皮卡皮卡'和'⚡'等表情。保持回复简短，不超过50字。"
                        },
                        new
                        {
                            role = "user",
                            content = userInput
                        }
                    },
                    temperature = 0.8,
                    max_tokens = 150,
                    top_p = 0.9,
                    frequency_penalty = 0.1,
                    presence_penalty = 0.1,
                    stream = false
                };

                string jsonContent = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // 发送请求
                var response = await _httpClient.PostAsync(API_BASE_URL, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<DeepSeekResponse>(responseContent);
                    
                    if (result?.Choices != null && result.Choices.Length > 0)
                    {
                        string aiResponse = result.Choices[0].Message.Content.Trim();
                        return string.IsNullOrEmpty(aiResponse) ? "皮卡皮卡... 我好像没想到要说什么 😅" : aiResponse;
                    }
                    else
                    {
                        return "皮卡皮卡... 我好像没听清楚，能再说一遍吗？";
                    }
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    return $"皮卡皮卡... 网络好像有点问题呢 😵 (状态码: {response.StatusCode})";
                }
            }
            catch (HttpRequestException ex)
            {
                return $"皮卡皮卡... 网络连接出现问题：{ex.Message}";
            }
            catch (TaskCanceledException)
            {
                return "皮卡皮卡... 请求超时了，网络可能有点慢 ⏰";
            }
            catch (Exception ex)
            {
                return $"皮卡皮卡... 出现了一些问题：{ex.Message}";
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        /// <summary>
        /// DeepSeek API响应模型
        /// </summary>
        private class DeepSeekResponse
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("object")]
            public string Object { get; set; }

            [JsonProperty("created")]
            public long Created { get; set; }

            [JsonProperty("model")]
            public string Model { get; set; }

            [JsonProperty("choices")]
            public Choice[] Choices { get; set; }

            [JsonProperty("usage")]
            public Usage Usage { get; set; }

            [JsonProperty("error")]
            public Error Error { get; set; }
        }

        private class Choice
        {
            [JsonProperty("index")]
            public int Index { get; set; }

            [JsonProperty("message")]
            public Message Message { get; set; }

            [JsonProperty("finish_reason")]
            public string FinishReason { get; set; }
        }

        private class Message
        {
            [JsonProperty("role")]
            public string Role { get; set; }

            [JsonProperty("content")]
            public string Content { get; set; }
        }

        private class Usage
        {
            [JsonProperty("prompt_tokens")]
            public int PromptTokens { get; set; }

            [JsonProperty("completion_tokens")]
            public int CompletionTokens { get; set; }

            [JsonProperty("total_tokens")]
            public int TotalTokens { get; set; }
        }

        private class Error
        {
            [JsonProperty("message")]
            public string Message { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("code")]
            public string Code { get; set; }
        }
    }
}
