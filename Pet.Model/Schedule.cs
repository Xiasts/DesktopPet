using System;

namespace Pet.Model
{
    /// <summary>
    /// 日程实体类 - 表示一个日程提醒项
    /// </summary>
    public class Schedule
    {
        /// <summary>
        /// 唯一标识符
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 提醒时间
        /// </summary>
        public DateTime ReminderTime { get; set; }

        /// <summary>
        /// 日程内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 是否启用此提醒
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public Schedule()
        {
            Id = Guid.NewGuid();
            IsEnabled = true;
            CreatedTime = DateTime.Now;
            Content = string.Empty;
        }

        /// <summary>
        /// 带参数的构造函数
        /// </summary>
        /// <param name="content">日程内容</param>
        /// <param name="reminderTime">提醒时间</param>
        public Schedule(string content, DateTime reminderTime) : this()
        {
            Content = content;
            ReminderTime = reminderTime;
        }

        /// <summary>
        /// 检查是否到了提醒时间
        /// </summary>
        /// <returns>如果到了提醒时间返回true</returns>
        public bool IsTimeToRemind()
        {
            return IsEnabled && DateTime.Now >= ReminderTime;
        }

        /// <summary>
        /// 重写ToString方法，便于显示
        /// </summary>
        /// <returns>格式化的字符串</returns>
        public override string ToString()
        {
            return $"{ReminderTime:yyyy-MM-dd HH:mm} - {Content}";
        }
    }
}
