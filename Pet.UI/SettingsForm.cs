using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Pet.BLL;
using Pet.Model;

namespace Pet.UI
{
    public partial class SettingsForm : Form
    {
        private ScheduleManager _scheduleManager;

        public SettingsForm()
        {
            InitializeComponent();
            _scheduleManager = ScheduleManager.Instance;
            LoadSchedules();

            // 添加DataGridView的单元格点击事件
            dataGridViewSchedules.CellClick += DataGridViewSchedules_CellClick;
        }

        /// <summary>
        /// 加载日程到DataGridView
        /// </summary>
        private void LoadSchedules()
        {
            var schedules = _scheduleManager.Schedules;
            var dataSource = schedules
                .OrderBy(s => s.ReminderTime) // 按提醒时间排序
                .Select(s => new
                {
                    Id = s.Id,
                    Content = s.Content,
                    ReminderTime = s.ReminderTime,
                    IsEnabled = s.IsEnabled ? "✅ 启用" : "❌ 禁用",
                    CreatedTime = s.CreatedTime
                }).ToList();

            dataGridViewSchedules.DataSource = dataSource;

            // 隐藏ID列
            if (dataGridViewSchedules.Columns["Id"] != null)
            {
                dataGridViewSchedules.Columns["Id"].Visible = false;
            }

            // 设置列标题和样式
            if (dataGridViewSchedules.Columns["Content"] != null)
            {
                dataGridViewSchedules.Columns["Content"].HeaderText = "📝 日程内容";
                dataGridViewSchedules.Columns["Content"].Width = 300;
            }
            if (dataGridViewSchedules.Columns["ReminderTime"] != null)
            {
                dataGridViewSchedules.Columns["ReminderTime"].HeaderText = "⏰ 提醒时间";
                dataGridViewSchedules.Columns["ReminderTime"].Width = 150;
            }
            if (dataGridViewSchedules.Columns["IsEnabled"] != null)
            {
                dataGridViewSchedules.Columns["IsEnabled"].HeaderText = "🔔 状态 ";
                dataGridViewSchedules.Columns["IsEnabled"].Width = 120;
                // 设置状态列的样式，让它看起来可点击
                dataGridViewSchedules.Columns["IsEnabled"].DefaultCellStyle.BackColor = Color.FromArgb(240, 248, 255);
                dataGridViewSchedules.Columns["IsEnabled"].DefaultCellStyle.SelectionBackColor = Color.FromArgb(173, 216, 230);
                dataGridViewSchedules.Columns["IsEnabled"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            if (dataGridViewSchedules.Columns["CreatedTime"] != null)
            {
                dataGridViewSchedules.Columns["CreatedTime"].HeaderText = "📅 创建时间";
                dataGridViewSchedules.Columns["CreatedTime"].Width = 150;
            }

            // 设置行样式
            foreach (DataGridViewRow row in dataGridViewSchedules.Rows)
            {
                if (row.Index % 2 == 0)
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                }
            }
        }

        /// <summary>
        /// 添加按钮点击事件
        /// </summary>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            string content = txtContent.Text.Trim();
            if (string.IsNullOrEmpty(content))
            {
                MessageBox.Show("请输入日程内容！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime reminderTime = dateTimePickerReminder.Value;
            if (reminderTime <= DateTime.Now)
            {
                MessageBox.Show("提醒时间必须是未来时间！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_scheduleManager.AddSchedule(content, reminderTime))
            {
                MessageBox.Show("日程添加成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtContent.Clear();
                dateTimePickerReminder.Value = DateTime.Now.AddHours(1);
                LoadSchedules();
            }
            else
            {
                MessageBox.Show("日程添加失败！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 删除按钮点击事件
        /// </summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewSchedules.SelectedRows.Count == 0)
            {
                MessageBox.Show("请选择要删除的日程！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show("确定要删除选中的日程吗？", "确认删除", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                var selectedRow = dataGridViewSchedules.SelectedRows[0];
                var scheduleId = (Guid)selectedRow.Cells["Id"].Value;

                if (_scheduleManager.DeleteSchedule(scheduleId))
                {
                    MessageBox.Show("日程删除成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadSchedules();
                }
                else
                {
                    MessageBox.Show("日程删除失败！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// 保存按钮点击事件
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            MessageBox.Show("日程已自动保存！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 关闭按钮点击事件
        /// </summary>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 测试提醒按钮点击事件
        /// </summary>
        private void btnTestReminder_Click(object sender, EventArgs e)
        {
            // 添加一个1分钟后的测试提醒
            var testTime = DateTime.Now.AddMinutes(1);
            if (_scheduleManager.AddSchedule("测试提醒", testTime))
            {
                MessageBox.Show($"测试提醒已添加，将在 {testTime:HH:mm:ss} 提醒！",
                    "测试提醒", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadSchedules();
            }
        }

        /// <summary>
        /// DataGridView单元格点击事件 - 处理状态列的点击切换
        /// </summary>
        private void DataGridViewSchedules_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // 检查是否点击了有效的行和列
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && e.RowIndex < dataGridViewSchedules.Rows.Count)
            {
                try
                {
                    var columnName = dataGridViewSchedules.Columns[e.ColumnIndex].Name;

                    // 检查是否点击了状态列
                    if (columnName == "IsEnabled")
                    {
                        var selectedRow = dataGridViewSchedules.Rows[e.RowIndex];

                        // 安全地获取ID
                        if (selectedRow.Cells["Id"].Value != null)
                        {
                            var scheduleId = (Guid)selectedRow.Cells["Id"].Value;

                            // 获取当前状态（切换前）
                            var currentStatusText = selectedRow.Cells["IsEnabled"].Value?.ToString() ?? "";
                            bool wasEnabled = currentStatusText.Contains("启用");

                            // 切换状态
                            if (_scheduleManager.ToggleScheduleStatus(scheduleId))
                            {
                                // 刷新显示
                                LoadSchedules();

                                // 显示提示信息
                                string message = wasEnabled ?
                                    "日程已禁用，将不会收到提醒" :
                                    "日程已启用，将按时提醒";

                                MessageBox.Show(message, "状态切换",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                // 刷新显示以确保状态正确
                                LoadSchedules();

                                // 如果是尝试启用过期的日程，给出特殊提示
                                if (!wasEnabled)
                                {
                                    MessageBox.Show("无法启用已过期的日程！\n请修改提醒时间为未来时间后再启用。",
                                        "提醒时间已过期", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                                else
                                {
                                    MessageBox.Show("状态切换失败！", "错误",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"操作失败：{ex.Message}", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
