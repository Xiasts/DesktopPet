# 🎮 皮卡丘桌面宠物 (Pikachu Desktop Pet)

一个基于C# WinForms开发的智能桌面宠物应用，采用多种设计模式实现，具备AI对话、日程提醒、物理交互等功能。

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.7.2-purple.svg)
![Platform](https://img.shields.io/badge/platform-Windows-lightgrey.svg)

## ✨ 主要功能

### 🤖 AI智能对话
- 集成DeepSeek AI，支持自然语言对话
- 皮卡丘人格化回复，生动有趣
- 自定义气泡对话界面，5秒自动消失
- 适配器模式设计，易于扩展其他AI服务

### 📅 智能日程管理
- 可视化日程添加和管理界面
- 点击切换启用/禁用状态
- 自动过期检测，防止误触发
- 气泡提醒替代系统通知

### 🎭 丰富的交互动画
- **待机状态**：可爱的待机动画
- **拖拽交互**：流畅的拖拽动画
- **物理下落**：真实的重力效果
- **边缘吸附**：智能贴边显示
- **特殊动画**：吃Cookie、玩耍等行为

### 🎨 精美的用户界面
- 现代化的设置界面设计
- 圆角气泡对话框
- 响应式布局和动画效果
- 用户友好的交互体验

## 🏗️ 技术架构

### 三层架构设计
```
├── Pet.UI          # 用户界面层
├── Pet.BLL         # 业务逻辑层  
├── Pet.DAL         # 数据访问层
├── Pet.Model       # 数据模型层
└── Pet.Common      # 公共工具层
```

### 设计模式应用
- **状态模式 (State Pattern)**: 管理宠物的不同行为状态
- **单例模式 (Singleton Pattern)**: 日程管理器的全局访问
- **观察者模式 (Observer Pattern)**: 日程提醒事件通知
- **适配器模式 (Adapter Pattern)**: AI服务接口统一

## 🚀 快速开始

### 环境要求
- Windows 7/8/10/11
- .NET Framework 4.7.2 或更高版本
- Visual Studio 2019 或更高版本（开发）

### 安装步骤

1. **克隆项目**
```bash
git clone https://github.com/Xiasts/DesktopPet.git
cd DesktopPet
```

2. **还原NuGet包**
```bash
nuget restore
```

3. **编译项目**
```bash
dotnet build
```

4. **运行应用**
```bash
cd Pet.UI\bin\Debug
Pet.UI.exe
```

### 配置AI服务

1. 打开 `Pet.BLL/DeepSeekAdapter.cs`
2. 替换API密钥为你的DeepSeek API Key
3. 重新编译项目

## 📖 使用指南

### 基本操作
- **拖拽移动**: 左键拖拽皮卡丘到任意位置
- **右键菜单**: 访问所有功能选项
- **AI对话**: 右键选择"和我聊天"
- **日程管理**: 右键选择"日程设置"

### 日程管理
1. 点击"日程设置"打开管理界面
2. 输入日程内容和提醒时间
3. 点击"添加"创建新日程
4. 点击状态列切换启用/禁用
5. 选中行后点击"删除"移除日程

### AI对话
1. 右键选择"和我聊天"
2. 在对话框中输入你想说的话
3. 支持Ctrl+Enter快速发送
4. 皮卡丘会用可爱的语气回复你

## 🎯 项目特色

### 动画系统
- 独立的动画速度控制
- 33ms高频率更新，流畅体验
- 解耦动画速度与主循环频率

### 智能交互
- 物理引擎模拟真实下落
- 屏幕边缘智能吸附
- 防止超出屏幕边界

### 数据持久化
- JSON格式存储日程数据
- 自动保存用户设置
- 程序重启后状态保持

## 🛠️ 开发指南

### 项目结构
```
DesktopPet/
├── Pet.UI/                 # 用户界面
│   ├── PetForm.cs         # 主窗体
│   ├── BubbleForm.cs      # 气泡对话框
│   ├── ChatForm.cs        # 对话输入框
│   └── SettingsForm.cs    # 设置界面
├── Pet.BLL/               # 业务逻辑
│   ├── States/            # 状态类
│   ├── AI Services/       # AI服务适配器
│   └── ScheduleManager.cs # 日程管理
├── Pet.Model/             # 数据模型
└── Pet.Common/            # 公共工具
```

### 添加新状态
1. 实现`IPetState`接口
2. 添加到项目文件
3. 在适当位置调用`core.SetState(new YourState())`

### 扩展AI服务
1. 实现`IDialogService`接口
2. 创建新的适配器类
3. 在`PetForm`中替换服务实例

## 🤝 贡献指南

欢迎提交Issue和Pull Request！

### 开发流程
1. Fork本项目
2. 创建特性分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 创建Pull Request

### 代码规范
- 遵循C#命名约定
- 添加适当的注释
- 保持代码整洁和可读性
- 编写单元测试（推荐）

## 📄 许可证

本项目采用MIT许可证 - 查看 [LICENSE](LICENSE) 文件了解详情

## 🙏 致谢

- 皮卡丘角色版权归任天堂所有
- DeepSeek AI提供智能对话支持
- 感谢所有贡献者的支持

## 📞 联系方式

- 项目地址: [https://github.com/Xiasts/DesktopPet](https://github.com/Xiasts/DesktopPet)
- 问题反馈: [Issues](https://github.com/Xiasts/DesktopPet/issues)

---

⭐ 如果这个项目对你有帮助，请给个Star支持一下！
