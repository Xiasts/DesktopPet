using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Pet.BLL
{
    /// <summary>
    /// 天气播报策略 - 双击时查询当地天气
    /// </summary>
    public class WeatherReporterStrategy : IDoubleClickActionStrategy
    {
        public string Name => "查询当地天气";

        public event Action<string, int> OnActionMessage;

        // 和风天气API Key - 
        private const string ApiKey = "6058f1ce22984edd85f41ce40c58d427";

        // 和风天气API Host - 
        private const string ApiHost = "nk5n8kntw9.re.qweatherapi.com";

        // 默认城市ID - 
        private const string LocationId = "101120201"; // 青岛

        public async void Execute()
        {
            try
            {
                // 显示加载中提示
                OnActionMessage?.Invoke("皮卡皮卡...正在查询天气...", 3000);

                string weatherInfo = await GetWeatherInfoAsync();

                // 显示天气结果
                OnActionMessage?.Invoke(weatherInfo, 10000);
            }
            catch (Exception ex)
            {
                OnActionMessage?.Invoke($"皮卡...天气查询失败了！\n错误: {ex.Message}\n\n调试信息:\nAPI Host: {ApiHost}\nLocation ID: {LocationId}", 8000);
            }
        }

        private async Task<string> GetWeatherInfoAsync()
        {
            // 创建支持自动解压缩的HttpClientHandler
            using (var handler = new HttpClientHandler())
            {
                handler.AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate;

                using (HttpClient client = new HttpClient(handler))
                {
                    try
                    {
                        // 添加必要的请求头
                        client.DefaultRequestHeaders.Add("User-Agent", "DesktopPet/1.0");

                        // 构建API请求URL - 使用专用Host
                        string url = $"https://{ApiHost}/v7/weather/now?location={LocationId}&key={ApiKey}";

                        // 添加调试信息
                        System.Diagnostics.Debug.WriteLine($"请求URL: {url}");

                        HttpResponseMessage response = await client.GetAsync(url);
                    string responseBody = await response.Content.ReadAsStringAsync();

                    // 添加调试信息
                    System.Diagnostics.Debug.WriteLine($"响应状态码: {response.StatusCode}");
                    System.Diagnostics.Debug.WriteLine($"响应内容: {responseBody}");

                    // 检查HTTP状态码
                    if (!response.IsSuccessStatusCode)
                    {
                        return $"皮卡...API请求失败\n状态码: {response.StatusCode}\n响应: {responseBody}";
                    }

                    // 检查响应是否为空或不是JSON格式
                    if (string.IsNullOrWhiteSpace(responseBody))
                    {
                        return "皮卡...API返回了空响应";
                    }

                    // 检查响应是否以{开头（JSON格式）
                    if (!responseBody.TrimStart().StartsWith("{"))
                    {
                        return $"皮卡...API返回了非JSON格式的响应:\n{responseBody.Substring(0, Math.Min(200, responseBody.Length))}";
                    }

                    // 解析JSON数据
                    var weatherData = JsonConvert.DeserializeObject<WeatherResponse>(responseBody);

                    if (weatherData?.code == "200" && weatherData.now != null)
                    {
                        var now = weatherData.now;
                        return FormatWeatherMessage(now);
                    }
                    else
                    {
                        return $"皮卡...获取天气数据失败\n错误代码: {weatherData?.code ?? "未知"}\n原始响应: {responseBody}";
                    }
                }
                catch (JsonException jsonEx)
                {
                    return $"皮卡...JSON解析失败: {jsonEx.Message}";
                }
                catch (HttpRequestException httpEx)
                {
                    return $"皮卡...网络请求失败: {httpEx.Message}";
                }
                }
            }
        }

        private string FormatWeatherMessage(Now weather)
        {
            string message = "⚡ 皮卡丘天气播报 ⚡\n\n";
            message += $"🌡️ 当前温度: {weather.temp}°C\n";
            message += $"🤔 体感温度: {weather.feelsLike}°C\n";
            message += $"☁️ 天气状况: {weather.text}\n";
            message += $"💨 风向风力: {weather.windDir} {weather.windScale}级\n";
            message += $"💧 湿度: {weather.humidity}%\n";
            
            // 根据天气状况添加皮卡丘的评论
            string comment = GetPikachuComment(weather.text, int.Parse(weather.temp));
            message += $"\n{comment}";

            return message;
        }

        private string GetPikachuComment(string weatherText, int temperature)
        {
            if (weatherText.Contains("晴"))
            {
                return "皮卡皮卡！今天天气真不错，适合出去玩耍！⚡";
            }
            else if (weatherText.Contains("雨"))
            {
                return "皮卡...下雨天要小心漏电哦！记得带伞！☔";
            }
            else if (weatherText.Contains("雪"))
            {
                return "皮卡！雪天路滑，小心别摔倒了！❄️";
            }
            else if (weatherText.Contains("雾") || weatherText.Contains("霾"))
            {
                return "皮卡...能见度不好，出门要注意安全！😷";
            }
            else if (temperature > 30)
            {
                return "皮卡皮卡...好热啊！记得多喝水降温！🔥";
            }
            else if (temperature < 0)
            {
                return "皮卡...好冷！记得多穿衣服保暖！🧥";
            }
            else
            {
                return "皮卡！天气还不错，心情也要保持愉快哦！😊";
            }
        }
    }
}
