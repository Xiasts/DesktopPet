using System;
using System.Drawing;
using System.Windows.Forms;

namespace Pet.UI
{
    /// <summary>
    /// 对话输入窗体 - 提供更好的用户体验
    /// </summary>
    public partial class ChatForm : Form
    {
        private TextBox txtInput;
        private Button btnSend;
        private Button btnCancel;
        private Label lblPrompt;

        public string UserInput { get; private set; }

        public ChatForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 初始化组件
        /// </summary>
        private void InitializeComponent()
        {
            this.lblPrompt = new Label();
            this.txtInput = new TextBox();
            this.btnSend = new Button();
            this.btnCancel = new Button();
            this.SuspendLayout();

            // 
            // lblPrompt
            // 
            this.lblPrompt.AutoSize = true;
            this.lblPrompt.Font = new Font("微软雅黑", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            this.lblPrompt.Location = new Point(12, 15);
            this.lblPrompt.Name = "lblPrompt";
            this.lblPrompt.Size = new Size(200, 20);
            this.lblPrompt.TabIndex = 0;
            this.lblPrompt.Text = "想和皮卡丘说什么呢？⚡";

            // 
            // txtInput
            // 
            this.txtInput.Font = new Font("微软雅黑", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            this.txtInput.Location = new Point(12, 45);
            this.txtInput.Multiline = true;
            this.txtInput.Name = "txtInput";
            this.txtInput.ScrollBars = ScrollBars.Vertical;
            this.txtInput.Size = new Size(360, 80);
            this.txtInput.TabIndex = 1;
            this.txtInput.Text = "你好！";
            this.txtInput.KeyDown += TxtInput_KeyDown;

            // 
            // btnSend
            // 
            this.btnSend.BackColor = Color.FromArgb(255, 193, 7);
            this.btnSend.FlatStyle = FlatStyle.Flat;
            this.btnSend.Font = new Font("微软雅黑", 9F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(134)));
            this.btnSend.ForeColor = Color.White;
            this.btnSend.Location = new Point(217, 140);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new Size(75, 30);
            this.btnSend.TabIndex = 2;
            this.btnSend.Text = "发送 ⚡";
            this.btnSend.UseVisualStyleBackColor = false;
            this.btnSend.Click += BtnSend_Click;

            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = Color.FromArgb(108, 117, 125);
            this.btnCancel.FlatStyle = FlatStyle.Flat;
            this.btnCancel.Font = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            this.btnCancel.ForeColor = Color.White;
            this.btnCancel.Location = new Point(297, 140);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(75, 30);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += BtnCancel_Click;

            // 
            // ChatForm
            // 
            this.AutoScaleDimensions = new SizeF(6F, 12F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.ClientSize = new Size(384, 185);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.lblPrompt);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChatForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "和皮卡丘聊天";
            this.TopMost = true;

            this.ResumeLayout(false);
            this.PerformLayout();

            // 设置焦点到输入框并选中默认文本
            this.Load += (s, e) => {
                txtInput.Focus();
                txtInput.SelectAll();
            };
        }

        /// <summary>
        /// 处理输入框的键盘事件
        /// </summary>
        private void TxtInput_KeyDown(object sender, KeyEventArgs e)
        {
            // Ctrl+Enter 发送消息
            if (e.Control && e.KeyCode == Keys.Enter)
            {
                BtnSend_Click(sender, e);
                e.Handled = true;
            }
            // Escape 取消
            else if (e.KeyCode == Keys.Escape)
            {
                BtnCancel_Click(sender, e);
                e.Handled = true;
            }
        }

        /// <summary>
        /// 发送按钮点击事件
        /// </summary>
        private void BtnSend_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtInput.Text))
            {
                UserInput = txtInput.Text.Trim();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("请输入一些内容再发送哦！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtInput.Focus();
            }
        }

        /// <summary>
        /// 取消按钮点击事件
        /// </summary>
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
