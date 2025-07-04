namespace Pet.BLL
{
    /// <summary>
    /// 和风天气API返回的数据模型
    /// </summary>
    public class WeatherResponse
    {
        public string code { get; set; }
        public string updateTime { get; set; }
        public string fxLink { get; set; }
        public Now now { get; set; }
        public Refer refer { get; set; }
    }

    /// <summary>
    /// 当前天气实况数据
    /// </summary>
    public class Now
    {
        public string obsTime { get; set; }
        public string temp { get; set; }
        public string feelsLike { get; set; }
        public string icon { get; set; }
        public string text { get; set; }
        public string wind360 { get; set; }
        public string windDir { get; set; }
        public string windScale { get; set; }
        public string windSpeed { get; set; }
        public string humidity { get; set; }
        public string precip { get; set; }
        public string pressure { get; set; }
        public string vis { get; set; }
        public string cloud { get; set; }
        public string dew { get; set; }
    }

    /// <summary>
    /// 数据来源信息
    /// </summary>
    public class Refer
    {
        public string[] sources { get; set; }
        public string[] license { get; set; }
    }
}
