using System;

namespace Pet.Common
{
    /// <summary>
    /// 提供一个全局共享的Random实例，避免因短时内创建多个实例导致随机数重复的问题。
    /// 这是一个简化的单例应用。
    /// </summary>
    public static class SharedRandom
    {
        // 创建一个线程安全的、全局唯一的Random实例
        private static readonly Random _instance = new Random();

        /// <summary>
        /// 获取一个随机整数。
        /// </summary>
        public static int Next(int minValue, int maxValue)
        {
            return _instance.Next(minValue, maxValue);
        }

        /// <summary>
        /// 获取一个0到指定最大值之间的随机整数。
        /// </summary>
        public static int Next(int maxValue)
        {
            return _instance.Next(maxValue);
        }

        /// <summary>
        /// 获取一个0到1之间的随机双精度浮点数。
        /// </summary>
        public static double NextDouble()
        {
            return _instance.NextDouble();
        }
    }
}
