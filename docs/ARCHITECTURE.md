# 🏗️ 项目架构文档

## 概述

皮卡丘桌面宠物采用经典的三层架构设计，结合多种设计模式，确保代码的可维护性、可扩展性和可测试性。

## 🏛️ 架构层次

### 1. 表示层 (Presentation Layer) - Pet.UI
负责用户界面和用户交互逻辑。

```
Pet.UI/
├── PetForm.cs          # 主窗体，宠物显示和交互
├── BubbleForm.cs       # 气泡对话框
├── ChatForm.cs         # AI对话输入界面
├── SettingsForm.cs     # 日程管理设置界面
└── Resources/          # 图片资源文件
```

**职责：**
- 用户界面渲染
- 用户输入处理
- 事件响应
- 数据展示

### 2. 业务逻辑层 (Business Logic Layer) - Pet.BLL
包含核心业务逻辑和状态管理。

```
Pet.BLL/
├── States/             # 状态模式实现
│   ├── IPetState.cs   # 状态接口
│   ├── IdleState.cs   # 待机状态
│   ├── DragState.cs   # 拖拽状态
│   ├── FallState.cs   # 下落状态
│   ├── AttachState.cs # 吸附状态
│   ├── CookieState.cs # 吃Cookie状态
│   └── PlayState.cs   # 玩耍状态
├── AI/                 # AI服务适配器
│   ├── IDialogService.cs    # AI服务接口
│   ├── DeepSeekAdapter.cs   # DeepSeek适配器
│   └── MockDialogService.cs # 模拟服务
├── PetCore.cs         # 宠物核心控制器
└── ScheduleManager.cs # 日程管理器（单例）
```

**职责：**
- 业务规则实现
- 状态管理
- 数据处理
- 外部服务集成

### 3. 数据访问层 (Data Access Layer) - Pet.DAL
处理数据持久化和存储。

```
Pet.DAL/
├── IScheduleRepository.cs    # 日程数据接口
├── JsonScheduleRepository.cs # JSON文件存储实现
└── Models/                   # 数据传输对象
```

**职责：**
- 数据持久化
- 文件I/O操作
- 数据格式转换

### 4. 数据模型层 (Model Layer) - Pet.Model
定义数据结构和实体。

```
Pet.Model/
└── Schedule.cs        # 日程实体模型
```

### 5. 公共工具层 (Common Layer) - Pet.Common
提供通用工具和扩展方法。

```
Pet.Common/
└── SharedRandom.cs    # 共享随机数生成器
```

## 🎨 设计模式应用

### 1. 状态模式 (State Pattern)
**位置**: Pet.BLL/States/
**目的**: 管理宠物的不同行为状态

```csharp
public interface IPetState
{
    Image GetImage();
    void Update(PetCore core);
}
```

**优势**:
- 状态切换逻辑清晰
- 易于添加新状态
- 避免复杂的条件判断

### 2. 单例模式 (Singleton Pattern)
**位置**: Pet.BLL/ScheduleManager.cs
**目的**: 确保日程管理器全局唯一

```csharp
public class ScheduleManager
{
    private static readonly Lazy<ScheduleManager> _instance = 
        new Lazy<ScheduleManager>(() => new ScheduleManager());
    
    public static ScheduleManager Instance => _instance.Value;
}
```

### 3. 观察者模式 (Observer Pattern)
**位置**: Pet.BLL/ScheduleManager.cs
**目的**: 日程提醒事件通知

```csharp
public event Action<Schedule> OnReminderDue;
```

### 4. 适配器模式 (Adapter Pattern)
**位置**: Pet.BLL/AI/
**目的**: 统一不同AI服务的接口

```csharp
public interface IDialogService
{
    Task<string> GetResponseAsync(string userInput);
    string ServiceName { get; }
    bool IsAvailable { get; }
}
```

## 🔄 数据流

### 1. 用户交互流程
```
用户操作 → PetForm → PetCore → 状态类 → 更新UI
```

### 2. AI对话流程
```
用户输入 → ChatForm → PetForm → DeepSeekAdapter → API调用 → 返回结果 → BubbleForm显示
```

### 3. 日程提醒流程
```
定时器触发 → ScheduleManager → 检查到期日程 → 触发事件 → PetForm → 显示气泡
```

## 🔧 核心组件

### PetCore
宠物的核心控制器，负责：
- 状态管理和切换
- 位置计算
- 屏幕边界检测
- 图片获取

### ScheduleManager
日程管理的核心，负责：
- 日程CRUD操作
- 定时检查和提醒
- 数据持久化
- 事件通知

### 状态类
每个状态类负责：
- 特定状态的动画
- 状态转换逻辑
- 用户交互处理

## 🎯 扩展点

### 1. 添加新状态
1. 实现`IPetState`接口
2. 添加动画资源
3. 在适当位置调用状态切换

### 2. 集成新AI服务
1. 实现`IDialogService`接口
2. 创建适配器类
3. 在配置中添加选择选项

### 3. 扩展数据存储
1. 实现`IScheduleRepository`接口
2. 创建新的存储实现（如数据库）
3. 在依赖注入中替换实现

## 📊 性能考虑

### 动画系统
- 33ms高频率更新
- 独立动画速度控制
- 避免不必要的重绘

### 内存管理
- 及时释放图片资源
- 避免内存泄漏
- 合理使用缓存

### 异步处理
- AI API调用异步化
- 避免UI线程阻塞
- 合理的超时设置

## 🔒 安全考虑

### API密钥管理
- 不在代码中硬编码密钥
- 支持配置文件设置
- 考虑加密存储

### 数据验证
- 输入数据验证
- 防止注入攻击
- 错误信息不泄露敏感信息

## 🧪 测试策略

### 单元测试
- 业务逻辑层测试
- 状态转换测试
- 数据访问层测试

### 集成测试
- AI服务集成测试
- 数据持久化测试
- 事件通知测试

### UI测试
- 用户交互测试
- 界面响应测试
- 错误处理测试
