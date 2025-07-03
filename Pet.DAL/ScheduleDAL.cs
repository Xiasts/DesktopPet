using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Pet.Model;

namespace Pet.DAL
{
    /// <summary>
    /// 日程数据访问层 - 负责日程数据的持久化
    /// </summary>
    public class ScheduleDAL
    {
        private readonly string _filePath;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ScheduleDAL()
        {
            // 将数据文件保存在程序目录下
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "schedules.json");
        }

        /// <summary>
        /// 加载所有日程
        /// </summary>
        /// <returns>日程列表</returns>
        public List<Schedule> LoadSchedules()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    return new List<Schedule>();
                }

                var schedules = new List<Schedule>();
                string[] lines = File.ReadAllLines(_filePath);

                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var schedule = ParseScheduleFromLine(line);
                    if (schedule != null)
                    {
                        schedules.Add(schedule);
                    }
                }

                return schedules;
            }
            catch (Exception ex)
            {
                // 记录错误（这里简化处理）
                Console.WriteLine($"加载日程失败: {ex.Message}");
                return new List<Schedule>();
            }
        }

        /// <summary>
        /// 保存日程列表
        /// </summary>
        /// <param name="schedules">要保存的日程列表</param>
        /// <returns>是否保存成功</returns>
        public bool SaveSchedules(List<Schedule> schedules)
        {
            try
            {
                if (schedules == null)
                {
                    schedules = new List<Schedule>();
                }

                var lines = new List<string>();
                foreach (var schedule in schedules)
                {
                    lines.Add(FormatScheduleToLine(schedule));
                }

                File.WriteAllLines(_filePath, lines);
                return true;
            }
            catch (Exception ex)
            {
                // 记录错误（这里简化处理）
                Console.WriteLine($"保存日程失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 添加单个日程
        /// </summary>
        /// <param name="schedule">要添加的日程</param>
        /// <returns>是否添加成功</returns>
        public bool AddSchedule(Schedule schedule)
        {
            if (schedule == null) return false;

            var schedules = LoadSchedules();
            schedules.Add(schedule);
            return SaveSchedules(schedules);
        }

        /// <summary>
        /// 删除日程
        /// </summary>
        /// <param name="scheduleId">要删除的日程ID</param>
        /// <returns>是否删除成功</returns>
        public bool DeleteSchedule(Guid scheduleId)
        {
            var schedules = LoadSchedules();
            var scheduleToRemove = schedules.Find(s => s.Id == scheduleId);
            
            if (scheduleToRemove != null)
            {
                schedules.Remove(scheduleToRemove);
                return SaveSchedules(schedules);
            }
            
            return false;
        }

        /// <summary>
        /// 更新日程
        /// </summary>
        /// <param name="schedule">要更新的日程</param>
        /// <returns>是否更新成功</returns>
        public bool UpdateSchedule(Schedule schedule)
        {
            if (schedule == null) return false;

            var schedules = LoadSchedules();
            var existingSchedule = schedules.Find(s => s.Id == schedule.Id);
            
            if (existingSchedule != null)
            {
                existingSchedule.Content = schedule.Content;
                existingSchedule.ReminderTime = schedule.ReminderTime;
                existingSchedule.IsEnabled = schedule.IsEnabled;
                return SaveSchedules(schedules);
            }
            
            return false;
        }

        /// <summary>
        /// 获取数据文件路径（用于调试）
        /// </summary>
        /// <returns>数据文件的完整路径</returns>
        public string GetDataFilePath()
        {
            return _filePath;
        }

        /// <summary>
        /// 将日程格式化为文本行
        /// </summary>
        /// <param name="schedule">日程对象</param>
        /// <returns>格式化的文本行</returns>
        private string FormatScheduleToLine(Schedule schedule)
        {
            // 格式: ID|Content|ReminderTime|IsEnabled|CreatedTime
            return $"{schedule.Id}|{schedule.Content}|{schedule.ReminderTime:yyyy-MM-dd HH:mm:ss}|{schedule.IsEnabled}|{schedule.CreatedTime:yyyy-MM-dd HH:mm:ss}";
        }

        /// <summary>
        /// 从文本行解析日程
        /// </summary>
        /// <param name="line">文本行</param>
        /// <returns>日程对象，解析失败返回null</returns>
        private Schedule ParseScheduleFromLine(string line)
        {
            try
            {
                string[] parts = line.Split('|');
                if (parts.Length != 5) return null;

                var schedule = new Schedule
                {
                    Id = Guid.Parse(parts[0]),
                    Content = parts[1],
                    ReminderTime = DateTime.Parse(parts[2]),
                    IsEnabled = bool.Parse(parts[3]),
                    CreatedTime = DateTime.Parse(parts[4])
                };

                return schedule;
            }
            catch
            {
                return null;
            }
        }
    }
}
