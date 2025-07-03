using System;
using System.Collections.Generic;
using System.Linq;
using Pet.Model;
using Pet.DAL;

namespace Pet.BLL
{
    /// <summary>
    /// 日程管理器 - 应用单例模式和观察者模式
    /// 负责日程的业务逻辑处理和提醒事件的触发
    /// </summary>
    public class ScheduleManager
    {
        #region 单例模式实现
        
        private static readonly ScheduleManager _instance = new ScheduleManager();
        private static readonly object _lock = new object();

        /// <summary>
        /// 获取单例实例
        /// </summary>
        public static ScheduleManager Instance
        {
            get
            {
                return _instance;
            }
        }

        #endregion

        #region 观察者模式实现

        /// <summary>
        /// 提醒事件 - 当有日程到期时触发
        /// </summary>
        public event Action<Schedule> OnReminderDue;

        #endregion

        #region 私有字段

        private readonly ScheduleDAL _dal;
        private List<Schedule> _schedules;
        private readonly object _schedulesLock = new object();

        #endregion

        #region 构造函数

        /// <summary>
        /// 私有构造函数，确保单例
        /// </summary>
        private ScheduleManager()
        {
            _dal = new ScheduleDAL();
            LoadSchedules();
        }

        #endregion

        #region 公共属性

        /// <summary>
        /// 获取所有日程的只读副本
        /// </summary>
        public List<Schedule> Schedules
        {
            get
            {
                lock (_schedulesLock)
                {
                    return new List<Schedule>(_schedules);
                }
            }
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 检查提醒 - 在主循环中调用
        /// </summary>
        public void CheckReminders()
        {
            lock (_schedulesLock)
            {
                var dueSchedules = _schedules.Where(s => s.IsTimeToRemind()).ToList();
                
                foreach (var schedule in dueSchedules)
                {
                    // 触发提醒事件
                    OnReminderDue?.Invoke(schedule);
                    
                    // 提醒后禁用，防止重复提醒
                    schedule.IsEnabled = false;
                }

                // 如果有日程被禁用，保存到文件
                if (dueSchedules.Count > 0)
                {
                    SaveSchedules();
                }
            }
        }

        /// <summary>
        /// 添加新日程
        /// </summary>
        /// <param name="content">日程内容</param>
        /// <param name="reminderTime">提醒时间</param>
        /// <returns>添加是否成功</returns>
        public bool AddSchedule(string content, DateTime reminderTime)
        {
            if (string.IsNullOrWhiteSpace(content))
                return false;

            var schedule = new Schedule(content, reminderTime);
            return AddSchedule(schedule);
        }

        /// <summary>
        /// 添加日程
        /// </summary>
        /// <param name="schedule">要添加的日程</param>
        /// <returns>添加是否成功</returns>
        public bool AddSchedule(Schedule schedule)
        {
            if (schedule == null)
                return false;

            lock (_schedulesLock)
            {
                _schedules.Add(schedule);
                return SaveSchedules();
            }
        }

        /// <summary>
        /// 删除日程
        /// </summary>
        /// <param name="scheduleId">要删除的日程ID</param>
        /// <returns>删除是否成功</returns>
        public bool DeleteSchedule(Guid scheduleId)
        {
            lock (_schedulesLock)
            {
                var schedule = _schedules.FirstOrDefault(s => s.Id == scheduleId);
                if (schedule != null)
                {
                    _schedules.Remove(schedule);
                    return SaveSchedules();
                }
                return false;
            }
        }

        /// <summary>
        /// 更新日程
        /// </summary>
        /// <param name="schedule">要更新的日程</param>
        /// <returns>更新是否成功</returns>
        public bool UpdateSchedule(Schedule schedule)
        {
            if (schedule == null)
                return false;

            lock (_schedulesLock)
            {
                var existingSchedule = _schedules.FirstOrDefault(s => s.Id == schedule.Id);
                if (existingSchedule != null)
                {
                    existingSchedule.Content = schedule.Content;
                    existingSchedule.ReminderTime = schedule.ReminderTime;
                    existingSchedule.IsEnabled = schedule.IsEnabled;
                    return SaveSchedules();
                }
                return false;
            }
        }

        /// <summary>
        /// 切换日程的启用/禁用状态
        /// </summary>
        /// <param name="scheduleId">日程ID</param>
        /// <returns>切换是否成功</returns>
        public bool ToggleScheduleStatus(Guid scheduleId)
        {
            lock (_schedulesLock)
            {
                var schedule = _schedules.FirstOrDefault(s => s.Id == scheduleId);
                if (schedule != null)
                {
                    bool wasEnabled = schedule.IsEnabled;
                    schedule.IsEnabled = !schedule.IsEnabled;

                    // 如果是从禁用变为启用，且提醒时间已过期，则自动禁用并提示用户
                    if (!wasEnabled && schedule.IsEnabled && schedule.ReminderTime <= DateTime.Now)
                    {
                        schedule.IsEnabled = false;
                        return false; // 返回false表示切换失败，需要在UI层处理
                    }

                    return SaveSchedules();
                }
                return false;
            }
        }

        /// <summary>
        /// 获取即将到期的日程（未来1小时内）
        /// </summary>
        /// <returns>即将到期的日程列表</returns>
        public List<Schedule> GetUpcomingSchedules()
        {
            lock (_schedulesLock)
            {
                var oneHourLater = DateTime.Now.AddHours(1);
                return _schedules.Where(s => s.IsEnabled && 
                                           s.ReminderTime > DateTime.Now && 
                                           s.ReminderTime <= oneHourLater)
                                .OrderBy(s => s.ReminderTime)
                                .ToList();
            }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 从文件加载日程
        /// </summary>
        private void LoadSchedules()
        {
            lock (_schedulesLock)
            {
                _schedules = _dal.LoadSchedules();
            }
        }

        /// <summary>
        /// 保存日程到文件
        /// </summary>
        /// <returns>保存是否成功</returns>
        private bool SaveSchedules()
        {
            return _dal.SaveSchedules(_schedules);
        }

        #endregion
    }
}
