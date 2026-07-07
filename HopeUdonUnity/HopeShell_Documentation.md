# HopeShell 文档

> **HopeUdonUnity** 是一套在 VRChat 运行时通过文本命令操控场景中 GameObject 和组件的 UdonSharp 工具集。
> **HopeShell** 是其核心 — 一个类 Unix Shell 的命令解析引擎。

***

## 目录

1. [项目概述](#1-项目概述)
2. [系统架构](#2-系统架构)
3. [核心技术设计](#3-核心技术设计)
4. [Shell 命令体系](#4-shell-命令体系)
5. [组件成员访问详解](#5-组件成员访问详解)
6. [输入方式](#6-输入方式)
7. [屏幕 UI 与键盘导航](#7-屏幕-ui-与键盘导航)
8. [编辑器工具集成](#8-编辑器工具集成)
9. [预制体与场景搭建](#9-预制体与场景搭建)
10. [完整命令速查表](#10-完整命令速查表)
11. [使用示例](#11-使用示例)
12. [UdonSharp 兼容性说明](#12-udonsharp-兼容性说明)

***

## 1. 项目概述

HopeUdonUnity 解决的核心问题：**在 VRChat 世界中，如何像操作终端一样实时操控场景中的任何对象和组件属性。**

无需重新上传世界，无需在 Unity Editor 中反复调整，只需在游戏内输入几行命令，即可：

- 修改任意 GameObject 的位置、旋转、缩放
- 切换 UI 元素的文本、颜色、开关状态
- 读写 UdonBehaviour 的 public 变量
- 触发自定义 Udon 事件
- 通过 URL 从远程服务器批量注入命令

### 文件结构总览

```
HopeUdonUnity/
├── HopeShell.cs                  ← 核心：Shell 命令解析引擎
├── HopeShellScreen.cs            ← 命令行 UI（显示 + 输入框 + 键盘导航）
├── HopeShellWebInput.cs          ← 通过 VRCUrl 下载文本注入 Shell
├── HopeUnityGameObjCompnet.cs    ← 组件面板初始化分发器
├── HopeUnityGameObjList.cs       ← 树形 GameObject 列表 UI
│
├── CompnetScript/                ← 各类组件详情编辑面板
│   ├── huComponetSimple.cs       ← 通用组件面板
│   ├── huComponetTf.cs           ← Transform 面板
│   ├── huComponetText.cs         ← Text 面板
│   ├── huComponetObj.cs          ← GameObject 面板
│   └── huComponetImage.cs        ← Image 面板（空壳）
│
├── Editor/                       ← 纯编辑器工具（不进入 VRChat 运行时）
│   ├── HtoolSimpleWebSever.cs    ← 本地 HTTP Server（端口 5004）
│   ├── PropertyContextMenuExtender.cs ← Inspector 右键菜单集成
│   ├── UdonExportTool.cs         ← 导出 .po / 层级路径 / Udon 变量
│   ├── HopeShell_Commands.md     ← 已有命令参考
│   └── HopeShell_ComponentSupport.md ← 组件支持一览
│
└── Perfeb/                       ← 预制体
    ├── ShellWithInputCmdLine.prefab     ← 命令行输入模式
    └── ShellWithWebDown.prefab          ← Web 下载模式
```

***

## 2. 系统架构

```
┌─────────────────────────────────────────────────────────────┐
│                    Unity Editor（编辑器侧）                    │
│                                                               │
│  HtoolSimpleWebSever (端口5004)  ←── 浏览器 / 外部工具        │
│  PropertyContextMenuExtender    ←── Inspector 右键            │
│  UdonExportTool                 ←── 文件导出                   │
└──────────────────────┬──────────────────────────────────────┘
                       │ VRCUrl 下载 (VRCStringDownloader)
                       ▼
┌──────────────────────────────────────────────────────────────┐
│                    VRChat Runtime（运行时）                     │
│                                                                │
│  ┌─────────────────┐                                           │
│  │ HopeShellWebInput │── VRCStringDownloader 下载文本           │
│  └────────┬────────┘                                           │
│           │ SetProgramVariable("eventData", text)               │
│           │ SendCustomEvent("ex")                               │
│           ▼                                                    │
│  ┌─────────────────┐     Udon 事件通信     ┌──────────────────┐ │
│  │    HopeShell      │◄─────────────────────│  HopeShellScreen  │ │
│  │  （命令解析引擎）   │                      │  （UI 显示+输入）  │ │
│  └────────┬────────┘                       └──────────────────┘ │
│           │                                                     │
│           │ 直接读写 Unity 组件                                  │
│           ▼                                                     │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │  Transform / Text / Toggle / Slider / Image / RawImage    │   │
│  │  AudioSource / BoxCollider / UdonBehaviour               │   │
│  └─────────────────────────────────────────────────────────┘   │
│                                                                │
│  ┌──────────────────────┐     ┌─────────────────────────────┐ │
│  │ HopeUnityGameObjList  │────▶│ HopeUnityGameObjCompnet      │ │
│  │ （树形目录选择）       │     │ └→ huComponetTf / Text /     │ │
│  └──────────────────────┘     │    Obj / Simple / Image      │ │
│                               │   （详情面板编辑）            │ │
│                               └─────────────────────────────┘ │
└──────────────────────────────────────────────────────────────┘
```

**核心数据流：**

```text
文本输入 → InputCommand / InputCommandText → ProcessCommand
    ├── 内置命令 (help/ls/cd/...) → PrintLine 输出
    ├── $变量操作 → ProcessVariableAccess → SetVariable/GetVariable
    └── .成员访问 → MemberVariableAccess → MemberVariableEq
        ├── .tf → GetSetTransformPropertyValue
        ├── .en → GetSetActiveState
        ├── .t  → GetSetText
        ├── .tg → GetSetToggle
        ├── .u  → GetSetUdon
        ├── .ut → GetSetUdonEvn
        ├── .img→ GetSetImage
        ├── .sl → GetSetSlider
        ├── .as → GetSetAudioSource
        └── .bc → GetSetBoxCollider
```

***

## 3. 核心技术设计

### 3.1 变量系统

HopeShell 使用**固定大小数组**（默认容量 20）实现键值对存储，完全兼容 Udon VM 的限制。

```csharp
// 三个平行数组模拟 Dictionary<string, string>
private string[] variableNames;   // 变量名
private string[] variableValues;  // 变量值
private string[] variableType;    // 变量类型（预留字段）
private int variableCount = 0;    // 当前变量数量
```

**特点：**

| 特性   | 说明                                 |
| ---- | ---------------------------------- |
| 变量设置 | `$var = value`，已存在则覆盖，不存在则新增       |
| 变量读取 | `$var` 直接回显值                       |
| 变量引用 | `$a = $b` 将其他变量的值赋给新变量             |
| 只读变量 | `$pwd` 由 `cd` 命令自动维护，不可手动赋值        |
| 模板替换 | `{$var}` 语法在字符串中自动展开               |
| 系统变量 | `$__VERSION__` = `"1.0.0"`，启动时自动设置 |

**字符串模板替换** (`string_placeholder`)：

```text
# 输入
$name = "World"
.t = "Hello {$name}"

# 结果
Text 内容变为: Hello World
```

### 3.2 路径系统

HopeShell 有一个"当前工作目录"概念，对应场景中的 `Transform` 层级。

- **`cd ObjectName`** — 相对路径，进入当前目录下的子物体
- **`cd /Root/Child/GrandChild`** — 绝对路径（以 `/` 开头）
- **`cd ..`** — 返回父物体
- **`cd .`** — 停留在当前目录（刷新 `$pwd`）
- **`cd "Name With Spaces"`** — 含有空格的名称用引号包围
- **`cd $myPath`** — 使用变量中的路径

**路径查找逻辑：**

1. 绝对路径 → 从 `_managered_transform` 管理器数组中查找根物体，或通过 `GameObject.Find()` 查找
2. 相对路径 → 从当前 `active_transform` 的子物体中按名称精确匹配
3. 找到的根物体会自动加入管理器（`_managered_transform`）

**管理器（`_managered_transform`）：**

- 一个固定大小的 `Transform[]` 数组，存储允许访问的根级 Transform
- 通过 `addroot PATH` 命令手动将场景中的 GameObject 添加进去
- 场景中没有管理器时，`cd` 绝对路径会自动加入

### 3.3 值解析系统

`MyParseValue()` 方法自动推断输入值的类型：

| 输入格式                                        | 解析类型        | 示例              |
| ------------------------------------------- | ----------- | --------------- |
| `"text"` 或 `'text'`                         | string      | `"Hello World"` |
| `(x, y, z)`                                 | Vector3     | `(1, 2, 3)`     |
| `(x, y)`                                    | Vector2     | `(0.5, 0.5)`    |
| `(x, y, z, w)`                              | Quaternion  | `(0, 0, 0, 1)`  |
| `true` / `false` / `on` / `off` / `1` / `0` | bool        | `true`          |
| 纯数字                                         | float 或 int | `3.14` 或 `42`   |
| `$var`                                      | 变量引用        | `$myValue`      |
| 其他                                          | string（原样）  | `SomeText`      |

### 3.4 命令路由

`ProcessCommand()` 根据命令前缀自动分流：

```text
以 "." 开头 → MemberVariableAccess()  → 组件成员访问
以 "$" 开头 → ProcessVariableAccess() → 变量操作
其他       → 内置命令匹配 (help/clear/history/time/ls/cd/addroot)
```

### 3.5 UdonSharp 兼容性保障

为适配 Udon VM 的严格限制，HopeShell 采用了以下策略：

- **禁用泛型集合**：使用固定数组 `string[]` 代替 `List<T>` 和 `Dictionary<K,V>`
- **禁用 LINQ**：所有查询使用 `for` 循环手动实现
- **禁用 async/await**：所有逻辑同步执行
- **无动态内存分配**：数组在 `Start()` 中预分配，`Update()` 中无分配
- **限定类型判断**：只判断 `typeof(bool/int/float/string/Vector2/Vector3/...)` 等已知类型
- **BehaviourSyncMode.None**：Shell 本身不同步网络，保持轻量

***

## 4. Shell 命令体系

### 4.1 内置命令

| 命令        | 别名    | 功能                 | 用法             |
| --------- | ----- | ------------------ | -------------- |
| `help`    | —     | 显示帮助信息             | `help`         |
| `clear`   | `cls` | 清屏                 | `clear`        |
| `history` | —     | 显示所有历史命令           | `history`      |
| `time`    | —     | 显示当前时间             | `time`         |
| `ls`      | —     | 列出当前目录下的子物体        | `ls`           |
| `cd`      | —     | 切换当前目录             | `cd DIR`       |
| `addroot` | —     | 添加 GameObject 到管理器 | `addroot PATH` |

### 4.2 变量命令

```text
# 设置变量
$myVar = "hello world"
$count = 42
$pos = (1, 2, 3)

# 读取变量
$myVar         → 输出: myVar = hello world
$count         → 输出: count = 42

# 变量引用赋值
$backup = $myVar

# 读取系统变量
$pwd           → 输出: pwd = /Root/Canvas/Panel
$__VERSION__   → 输出: __VERSION__ = 1.0.0

# 变量模板替换
$name = "VRChat"
.t = "Hello {$name}!"
```

### 4.3 命令处理默认行为

- 命令不区分大小写（命令名 `ToLower()` 处理）
- 空白命令自动忽略
- 未识别的命令返回 `Command not found` 提示
- 每个命令执行后自动加入历史缓存

***

## 5. 组件成员访问详解

**通用语法：`.<组件缩写>.<子成员> [= 值]`**

- 不写 `= 值` → 读取属性当前值
- 写 `= 值` → 设置属性为新值

### 5.1 Transform（`.tf` / `.transform`）

```text
.tf              → 读取摘要（名称/active/position/localPosition/localScale）
.tf.lp           → 读取 localPosition
.tf.lp = (1, 2, 3)  → 设置 localPosition
.tf.lr           → 读取 localRotation (Euler角度)
.tf.lr = (0, 90, 0) → 设置 localRotation
.tf.lel          → 读取 localEulerAngles
.tf.ls           → 读取 localScale
.tf.ls = (2, 2, 2)  → 设置 localScale
.tf.p            → 读取世界坐标 position
.tf.p = (0, 5, 0)   → 设置世界坐标 position
.tf.r            → 读取世界旋转 (Euler角度)
.tf.r = (0, 180, 0) → 设置世界旋转
```

| 子成员 Key | 完整名              | 别名                           | 类型             |
| ------- | ---------------- | ---------------------------- | -------------- |
| `lp`    | localPosition    | `localPosition`              | Vector3        |
| `lr`    | localRotation    | `localRotation`              | Euler(Vector3) |
| `lel`   | localEulerAngles | `localEulerAngles`, `leuler` | Vector3        |
| `ls`    | localScale       | `localScale`                 | Vector3        |
| `p`     | position         | `position`                   | Vector3        |
| `r`     | rotation(world)  | `rotation`, `euler`, `el`    | Euler(Vector3) |

### 5.2 GameObject Active（`.en` / `.active`）

```text
.en              → 读取 activeSelf
.en = true       → 激活物体
.en = false      → 禁用物体
```

### 5.3 Text / TMP\_Text（`.t` / `.text`）

```text
.t               → 读取文本内容
.t = "Hello"     → 设置文本
.t.fts = 36      → 设置字号
.t.c = (1, 0, 0) → 设置颜色为红色
.t.c = (0, 0, 1, 0.5) → 设置颜色为半透明蓝色
.t.en = false    → 禁用文本组件
```

> 自动适配 `UnityEngine.UI.Text` 和 `TMPro.TMP_Text`，`fontSize` 类型自适应（Text 用 int，TMP 用 float）

### 5.4 Toggle（`.tg` / `.toggle`）

```text
.tg              → 读取 isOn 状态
.tg = true       → 打开开关
.tg.en = false   → 禁用 Toggle 组件
.tg.isOn = false → 关闭开关
```

### 5.5 Slider（`.sl` / `.slider`）

```text
.sl              → 读取摘要（value/min/max/enabled/interactable）
.sl.val = 0.5    → 设置当前值
.sl.max = 1.0    → 设置最大值
.sl.min = 0.0    → 设置最小值
.sl.en = false   → 禁用 Slider
.sl.int = false  → 设置不可交互
```

### 5.6 UdonBehaviour（`.u` / `.udon`）

```text
.u               → 读取 enabled 状态
.u.myVar         → 读取 public 变量 myVar
.u.myInt = 42    → 设置 int 变量
.u.myStr = "hi"  → 设置 string 变量
.u.myVec = (1,2,3) → 设置 Vector3 变量
.u.en = false    → 禁用 UdonBehaviour
```

**支持的变量类型：** `bool` / `int` / `float` / `string` / `Vector2` / `Vector3` / `Vector2Int` / `Vector3Int` / `int[]` / `float[]`

**数组格式化输出：** 10 个一组，用 `|` 分隔。例如：

```text
[0:1, 1:2, 2:3, 3:4, 4:5, 5:6, 6:7, 7:8, 8:9, 9:10 | 10:11, 11:12]
```

### 5.7 Udon 自定义事件（`.ut` / `.udonevent`）

```text
.ut.OnReset              → 发送无参数事件 OnReset()
.ut.OnSet = 42           → 设置 eventData=42 后发送 OnSet()
.ut.OnSet = "hello"      → 设置 eventData="hello" 后发送 OnSet()
.ut.OnMulti = 1, hello   → 设置 eventData=1, eventData2="hello" 后发送 OnMulti()
```

> 逗号分隔双参数：`val1, val2` → eventData / eventData2

### 5.8 Image / RawImage（`.img` / `.image` / `.ri` / `.rawimg`）

```text
.img              → 读取摘要（enabled + color）
.img.en = false   → 禁用 Image
.img.c = (1,0,0)  → 设置颜色为红色
.ri.c = (0,1,0,0.5) → 设置 RawImage 为半透明绿色
```

### 5.9 AudioSource（`.as` / `.aud` / `.audio`）

```text
.as               → 读取摘要（mute/loop/enabled/volume/pitch/minDist/maxDist）
.as.mute = true   → 静音
.as.loop = false  → 取消循环
.as.vol = 0.8     → 设置音量为 80%
.as.pitch = 1.2   → 设置音高
.as.min = 5       → 设置最小距离
.as.max = 100     → 设置最大距离
```

### 5.10 BoxCollider（`.bc` / `.box`）

```text
.bc               → 读取摘要（enabled + isTrigger）
.bc.en = false    → 禁用碰撞体
.bc.trig = true   → 设为触发器
```

***

## 6. 输入方式

### 6.1 命令行单行输入

通过 `HopeShellScreen` 的 `InputField` 提交，底层调用：

```csharp
hope_shell.InputCommand(text)
```

- 自动去除换行符
- 以 `>` 开头的命令自动去除前缀
- 空命令忽略

### 6.2 多行文本输入（批量命令）

```csharp
hope_shell.InputCommandText(multiline_text)
```

按 `\n` 分割为多行，每行的特殊前缀处理：

| 前缀         | 转换行为           | 示例                                     |
| ---------- | -------------- | -------------------------------------- |
| `#cmd`     | 去除前缀后执行        | `#cmd .tf.lp = (0, 0, 0)`              |
| `#` 或 `//` | 注释行（跳过）        | `# 这是一条注释`                             |
| `msgid`    | 转为 `cd` 命令     | `msgid /Root/Panel` → `cd /Root/Panel` |
| `msgstr`   | 转为 `.t = `  命令 | `msgstr 你好` → `.t = 你好`                |

**`msgid`/`msgstr`** **前缀的由来：** 这是 `.po` 翻译文件的标准字段名。UdonExportTool 导出的 `.po` 文件可以直接用此格式注入 HopeShell，实现场景文字的批量设置。

**批量命令示例：**

```text
#cmd cd /Root/Canvas/Title
msgstr 欢迎来到VRChat
#cmd cd /Root/Canvas/SubTitle
msgstr 请选择一个房间
#cmd cd ../SettingsToggle
.tg = true
```

### 6.3 Web 远程下载输入

`HopeShellWebInput` 组件通过 `VRCStringDownloader` 定期从指定 URL 下载文本内容。

**工作流程：**

```text
1. HopeShellWebInput 每帧检查 auto_down 标志
2. 如果 auto_down=true，调用 VRCStringDownloader.LoadUrl(targetUrl)
3. 下载完成后触发 OnStringLoadSuccess()
4. 通过 SetProgramVariable + SendCustomEvent("ex") 将文本注入 HopeShell
5. HopeShell.ex() 调用 InputCommandText(text) 逐行执行

┌──────────────────────────────────────────────────────────┐
│  外部工具/浏览器                                          │
│  POST http://localhost:5004/ → 写入命令缓冲区              │
│  或 GET  http://localhost:5004/ → 读取并清空缓冲区          │
└──────────────────┬───────────────────────────────────────┘
                   │ HTTP
                   ▼
┌──────────────────────────────────────────────────────────┐
│  Editor: HtoolSimpleWebSever (端口5004)                   │
│  - AppendToBuffer(cmd) 积累命令                           │
│  - GET 时返回缓冲区 + 清空                                 │
└──────────────────┬───────────────────────────────────────┘
                   │ VRCUrl("http://localhost:5004/")
                   ▼
┌──────────────────────────────────────────────────────────┐
│  Runtime: HopeShellWebInput                               │
│  - VRCStringDownloader.LoadUrl() 周期下载                  │
│  - 注入 HopeShell                                         │
└──────────────────────────────────────────────────────────┘
```

**注意：** VRChat 客户端和编辑器必须运行在同一台机器上，或 URL 指向一个可公网访问的服务器。使用 `localhost` 仅适用于本地调试。

***

## 7. 屏幕 UI 与键盘导航

### 7.1 HopeShellScreen

负责 Shell 的视觉呈现和用户交互：

- **`_screen_text`** — 输出显示 Text 组件
- **`cmd_line`** — 命令输入框 InputField
- **向上箭头** — 浏览上一条历史命令
- **向下箭头** — 浏览下一条历史命令

### 7.2 历史命令系统

HopeShell 使用**环形缓冲区**管理命令历史：

```csharp
private string[] historyCmd;        // 固定大小数组（默认50）
private int historyCurrentIndex;    // 当前写入位置
private int historySize;            // 实际数量
private int historyNavigateIndex;   // 浏览导航位置
private bool isNavigatingHistory;   // 是否正在浏览
```

**特性：**

- 默认容量 50 条（通过 `historyMaxLines` 可配置）
- 缓冲区满后自动覆盖最早记录
- 上箭头从最新记录开始回溯
- 下箭头向较新记录前进
- 浏览到底部后回到空白输入框

### 7.3 Forbig 模式

`Ctrl + Alt + T` 快捷键切换防乱跑模式：

- **开启：** `Networking.LocalPlayer.Immobilize(true)` — 冻结玩家
- **关闭：** `Networking.LocalPlayer.SetJumpImpulse(3.0)` — 恢复跳跃
- 也可通过 `extern_forbid_udon` 外挂自定义规则

### 7.4 输出双通道

`PrintLine()` 同时写入两个输出通道：

1. **Udon 事件通道：** 通过 `_udon_api.SendCustomEvent("Evn_PrintLine")` 通知屏幕脚本
2. **直接写入通道：** 如果配置了 `_text1`，直接追加到 Text 组件的 text 属性

***

## 8. 编辑器工具集成

### 8.1 HtoolSimpleWebSever

本地 HTTP 服务器，在 Unity Editor 运行时启动：

- **端口：** 5004
- **POST：** 向缓冲区追加命令
- **GET：** 返回整个缓冲区内容并清空
- **用途：** 作为外部工具（如 PropertyContextMenuExtender）和运行时 HopeShellWebInput 之间的桥接

### 8.2 PropertyContextMenuExtender

在 Inspector 面板中右键点击属性时，生成对应的 HopeShell 命令。

**支持的操作：**

- **"发送属性值 -> HopeShell 命令"** — 生成如 `.tf.lp = (x, y, z)` 的命令文本
- **"发送到 Web Buffer"** — 将生成的命令发送到 HtoolSimpleWebSever 的缓冲区，供运行时下载执行

### 8.3 UdonExportTool

批量导出工具：

| 导出格式     | 内容                           | 用途                 |
| -------- | ---------------------------- | ------------------ |
| `.po` 文件 | `msgid PATH` + `msgstr TEXT` | 批量设置 UI 文字         |
| 层级路径     | `cd /Root/Child/...`         | 场景结构快照             |
| Udon 变量  | `.u.xxx = value`             | UdonBehaviour 变量配置 |

***

## 9. 预制体与场景搭建

### 9.1 ShellWithInputCmdLine.prefab

**用途：** 本地命令行交互模式

**包含组件：**

- `HopeShell` — 命令引擎
- `HopeShellScreen` — UI 显示 + 输入框

**适用场景：**

- 开发者调试
- 需要在场景内直接输入命令

**配置要点：**

1. 将 `HopeShellScreen.hope_shell` 拖拽引用到 `HopeShell` 对象
2. 将 `HopeShell._udon_api` 引用到 `HopeShellScreen` 的 UdonBehaviour
3. 将 `HopeShellScreen._screen_text` 引用到显示用的 Text 组件
4. 将 `HopeShellScreen.cmd_line` 引用到 InputField 组件

### 9.2 ShellWithWebDown.prefab

**用途：** 远程命令注入模式

**包含组件：**

- `HopeShell` — 命令引擎
- `HopeShellWebInput` — URL 下载器

**适用场景：**

- 编辑器远程操控运行时
- 批量命令自动化执行
- 外部工具集成

**配置要点：**

1. 将 `HopeShellWebInput._tar_duon` 拖拽引用到 `HopeShell` 的 UdonBehaviour
2. 设置 `targetUrl` 为命令服务器地址（本地调试用 `http://localhost:5004/`）
3. 可选：配置 `urlInputField` 和 `atuto_down_toggle` 做运行时 URL 切换

***

## 10. 完整命令速查表

### 内置命令

| 命令              | 说明     | 示例               |
| --------------- | ------ | ---------------- |
| `help`          | 帮助信息   | `help`           |
| `clear` / `cls` | 清屏     | `clear`          |
| `history`       | 历史命令   | `history`        |
| `time`          | 当前时间   | `time`           |
| `ls`            | 列出子物体  | `ls`             |
| `cd PATH`       | 切换目录   | `cd /Root/Panel` |
| `cd ..`         | 返回上级   | `cd ..`          |
| `addroot PATH`  | 添加管理物体 | `addroot /Root`  |

### 变量操作

| 命令              | 说明   | 示例                     |
| --------------- | ---- | ---------------------- |
| `$var = value`  | 设置变量 | `$x = 100`             |
| `$var = $other` | 引用赋值 | `$b = $a`              |
| `$var`          | 读取变量 | `$pwd`                 |
| `{$var}`        | 模板替换 | `.t = "Hello {$name}"` |

### 组件访问速查

| 缩写     | 组件            | 常用子成员                                        | 示例                   |
| ------ | ------------- | -------------------------------------------- | -------------------- |
| `.tf`  | Transform     | `lp` `lr` `ls` `p` `r`                       | `.tf.lp = (0, 0, 0)` |
| `.en`  | GameObject    | （无子成员）                                       | `.en = false`        |
| `.t`   | Text/TMP      | `t` `fts` `c` `en`                           | `.t.c = (1, 0, 0)`   |
| `.tg`  | Toggle        | `isOn` `en`                                  | `.tg = true`         |
| `.sl`  | Slider        | `val` `max` `min` `en` `int`                 | `.sl.val = 0.5`      |
| `.u`   | UdonBehaviour | 任意 public 变量 `en`                            | `.u.myVar = 42`      |
| `.ut`  | Udon Event    | 任意自定义事件名                                     | `.ut.OnReset`        |
| `.img` | Image         | `en` `c`                                     | `.img.c = (1, 0, 0)` |
| `.ri`  | RawImage      | `en` `c`                                     | `.ri.en = false`     |
| `.as`  | AudioSource   | `mute` `loop` `vol` `pitch` `min` `max` `en` | `.as.vol = 0.8`      |
| `.bc`  | BoxCollider   | `en` `trig`                                  | `.bc.trig = true`    |

### 值类型格式

| 类型         | 格式                                | 示例               |
| ---------- | --------------------------------- | ---------------- |
| string     | `"..."` 或 `'...'`                 | `"Hello"`        |
| bool       | `true`/`false`/`on`/`off`/`1`/`0` | `true`           |
| int        | 整数                                | `42`             |
| float      | 小数                                | `3.14`           |
| Vector3    | `(x, y, z)`                       | `(1, 2, 3)`      |
| Vector2    | `(x, y)`                          | `(0.5, 0.5)`     |
| Color      | `(r, g, b)` 或 `(r, g, b, a)`      | `(1, 0, 0, 0.5)` |
| Quaternion | `(x, y, z, w)`                    | `(0, 0, 0, 1)`   |
| 数组         | `(v1, v2, v3, ...)`               | `(1, 2, 3, 4)`   |

### 批量命令前缀

| 前缀            | 效果              |
| ------------- | --------------- |
| `#cmd`        | 执行命令            |
| `#` / `//`    | 注释（跳过）          |
| `msgid PATH`  | 等价于 `cd PATH`   |
| `msgstr TEXT` | 等价于 `.t = TEXT` |

***

## 11. 使用示例

### 示例 1：基础物体操控

```text
# 列出根物体（假设管理器中已有 Root）
addroot /Root
ls

# 切换到 Canvas 子物体
cd /Root/Canvas

# 列出 Canvas 下的所有子物体
ls

# 移动一个物体
cd Title
.tf.lp = (0, 100, 0)

# 缩放一个物体
.tf.ls = (2, 2, 2)

# 返回上级
cd ..
```

### 示例 2：UI 文字批量更新

```text
# 切换到标题文本
cd /Root/Canvas/TitleText

# 设置文字内容和颜色
.t = "欢迎来到我的世界"
.t.c = (1, 0.8, 0)
.t.fts = 48

# 切换到副标题
cd ../SubTitleText
.t = "选择一个传送点"
.t.c = (0.7, 0.7, 0.7)
.t.fts = 24
```

### 示例 3：Toggle 和 Slider 控制

```text
cd /Root/Canvas/SettingsPanel

# 操作 Toggle
cd MusicToggle
.tg = true           # 打开音乐
.tg.en = false       # 禁用开关

# 操作 Slider
cd ../VolumeSlider
.sl.val = 0.75      # 音量设为 75%
.sl.max = 2.0       # 最大值为 2.0
.sl.int = false     # 禁用交互
```

### 示例 4：UdonBehaviour 变量与事件

```text
cd /Root/GameManager

# 读取 Udon 变量
.u.myInt             → 输出: .u.myInt : typ = System.Int32 , val = 0

# 设置 Udon 变量
.u.myInt = 42
.u.myStr = "active"
.u.myVec = (1, 2, 3)

# 发送无参数事件
.ut.OnGameStart

# 发送带参数事件
.ut.OnPlayerJoin = VRChatUser

# 发送双参数事件
.ut.OnScoreChange = 100, bonus
```

### 示例 5：变量系统

```text
# 保存当前位置
$savedPos = .tf.lp

# 操作后再恢复
.tf.lp = (100, 200, 300)
.tf.lp = $savedPos

# 利用模板替换
$playerName = "小明"
.t = "你好, {$playerName}!"
```

### 示例 6：批量命令（通过 .po 文件或远程下载）

```text
#cmd cd /Root/Canvas
#cmd ls

# 以下使用 msgid/msgstr 格式（.po 兼容）
msgid Title
msgstr 欢迎光临
msgid SubTitle
msgstr 版本 v2.0

# 注释行会被跳过
# 这是注释，不会执行

msgid ../TipText
msgstr 按 Tab 打开菜单
```

### 示例 7：AudioSource 控制

```text
cd /Root/BGM

# 播放背景音乐
.as.vol = 0.5      # 音量 50%
.as.loop = true    # 循环播放
.as.mute = false   # 取消静音

# 3D 音效设置
.as.min = 5        # 最近衰减距离
.as.max = 50       # 最远衰减距离
```

***

## 12. UdonSharp 兼容性说明

HopeShell 严格遵循 UdonSharp 的 VM 限制进行开发。

### 禁止使用的特性

| 特性                                                         | 原因                                     |
| ---------------------------------------------------------- | -------------------------------------- |
| `List<T>`, `Dictionary<K,V>`, `Queue<T>`, `Stack<T>` 等泛型集合 | Udon VM 不支持                            |
| LINQ 方法（`.Where()`, `.Select()`, `.First()`, `.Any()` 等）   | Udon VM 不支持                            |
| `async`/`await` / `Task` / 协程                              | Udon VM 不支持                            |
| 动态数组分配（`Update()` 中 `new` 数组）                              | 性能问题，可能导致崩溃                            |
| 本地文件 IO、第三方网络请求                                            | VRChat 沙箱禁止                            |
| 多线程 / `unsafe` 代码                                          | VRChat 沙箱禁止                            |
| NuGet 外部包                                                  | 仅允许 VRC SDK + UdonSharp + Unity 原生 API |

### 采用的替代方案

| 需求     | 方案                                                                  |
| ------ | ------------------------------------------------------------------- |
| 键值存储   | 三个固定数组 `variableNames[]` + `variableValues[]` + `variableType[]` 模拟 |
| 动态列表   | 固定数组 + `variableCount` 计数器                                          |
| 字符串分割  | Unity 原生 `string.Split()`                                           |
| 类型判断   | `typeof()` 逐类型对比                                                    |
| 跨脚本通信  | `UdonBehaviour.SetProgramVariable()` + `SendCustomEvent()`          |
| 外部命令注入 | `VRCStringDownloader.LoadUrl()` 下载，`ex()` 事件转发                      |

### 最大容量限制

| 项目     | 默认值     | 可配置                                    |
| ------ | ------- | -------------------------------------- |
| 变量数量   | 20      | `maxVariables`                         |
| 历史命令   | 50      | `historyMaxLines`                      |
| 管理器根物体 | 取决于数组大小 | Inspector 设定 `_managered_transform` 长度 |

***

> **版本：** HopeShell v1.0.0
> **命名空间：** `HopeTools`
> **核心类：** `HopeShell : UdonSharpBehaviour`

