# HopeShell 视频教程文案

> 预计总时长：12-15 分钟
> 目标观众：VRChat 世界制作者、UdonSharp 开发者
> 前置知识：了解 Unity 基础、VRChat SDK

---

## 视频结构总览

| 段落 | 内容 | 预计时长 |
|------|------|----------|
| 片头 | 效果展示 + 一句话介绍 | 30秒 |
| 第一部分 | 这是什么？能干什么？ | 1分钟 |
| 第二部分 | 环境搭建 - 预制体拖入即用 | 1.5分钟 |
| 第三部分 | 路径导航 - cd / ls / pwd | 2分钟 |
| 第四部分 | 操控物体 - Transform 的读与写 | 2分钟 |
| 第五部分 | 操控 UI - Text / Toggle / Slider | 2分钟 |
| 第六部分 | 变量系统 - 临时存储与模板替换 | 1.5分钟 |
| 第七部分 | UdonBehaviour - 读写变量 / 触发事件 | 2分钟 |
| 第八部分 | 远程注入 - Web 下载 + 编辑器联动 | 1.5分钟 |
| 片尾 | 总结 + 资源链接 | 30秒 |

---

## 片头（0:00 - 0:30）

**【画面】**
- 黑底，HopeShell Logo / 文字标题渐入
- 背景是 VRChat 场景一角，模糊处理
- 快速闪过几组命令输入+实时效果的分屏画面（文字变色、物体移动、开关切换）

**【旁白】**
> 在 VRChat 里，你不再需要反复打包上传来调整场景。
> 今天给大家介绍 HopeShell —— 一个运行在 VRChat 世界中的命令行工具。
> 输入几行命令，就能操控场景里任何物体的位置、UI、甚至是 Udon 脚本变量。

**【画面】**
- 标题大字：「HopeShell - VRChat 世界里的终端」

---

## 第一部分：这是什么？（0:30 - 1:30）

**【画面】**
- Unity Editor 中反复调整 → 打包 → 上传 → 进 VRChat 测试 → 不满意 → 回 Editor 修改 → 再打包上传……（用加速镜头表现这个痛苦循环）
- 一个红色大叉覆盖这个循环

**【旁白】**
> 做 VRChat 世界的朋友都经历过这种痛苦：调一个 UI 位置，要反复打包上传十几次。
>
> HopeShell 就是来解决这个问题的。它是一个类 Unix Shell 的命令解析引擎，你在游戏里直接打字，它就能实时操控场景中的任何 GameObject。

**【画面】**
- 列出支持的组件列表（表格形式，逐行出现）

| 组件 | 支持的操作 |
|------|-----------|
| Transform | 读写位置/旋转/缩放 |
| Text / TMP | 读写文字/字号/颜色 |
| Toggle | 开关状态 |
| Slider | 值/范围/交互状态 |
| Image / RawImage | 颜色/启用状态 |
| AudioSource | 音量/音高/静音/循环/距离 |
| BoxCollider | 触发器/启用 |
| UdonBehaviour | 读写 public 变量、发送自定义事件 |

**【旁白】**
> 支持 Transform、Text、Toggle、Slider、Image、AudioSource、BoxCollider，以及最重要的 —— 直接读写 UdonBehaviour 的 public 变量和触发自定义事件。
>
> 而且它是纯 UdonSharp 实现，完全兼容 VRChat 的 Udon VM 沙箱限制，不会崩、不会卡。

---

## 第二部分：环境搭建（1:30 - 3:00）

**【画面】**
- 打开 Unity，展示 HopeUdonUnity 文件夹结构
- 高亮 `Perfeb/ShellWithInputCmdLine.prefab`

**【旁白】**
> 搭建非常简单。在 HopeUdonUnity 的 Perfeb 目录下，有两个预制体。
>
> ShellWithInputCmdLine —— 本地命令行交互模式，适合自己在场景里直接敲命令调试。
>
> ShellWithWebDown —— 远程命令注入模式，适合用外部工具或脚本批量下发命令。

**【画面】**
- 操作：将 `ShellWithInputCmdLine.prefab` 拖入场景
- 选中预制体，Inspector 面板展示各字段
- 逐一指示拖拽引用关系

**【旁白】**
> 把 ShellWithInputCmdLine 拖入场景后，需要配置几个引用。
>
> 把 HopeShellScreen 上的 hope_shell 指向 HopeShell 对象。
>
> HopeShell 上的 _udon_api 指向 HopeShellScreen 的 UdonBehaviour。
>
> _screen_text 指向场景中用来显示输出的 Text 组件。
>
> cmd_line 指向用来输入命令的 InputField 组件。
>
> 就这四个引用，配好就能用了。

**【画面】**
- 操作：点击 Play 进入运行模式
- 画面出现命令行界面，第一行显示 `HopeShell v1.0.0 - Type 'help' for available commands`
- 光标在 `>` 后面闪烁

**【旁白】**
> 进 Play 模式，看到这个欢迎信息，说明 Shell 已经跑起来了。

---

## 第三部分：路径导航（3:00 - 5:00）

**【画面】**
- 展示场景层级结构图（Hierarchy 截图 + 路径概念图的叠加）
- 例如：`/World/Canvas/Panel/TitleText`

**【旁白】**
> HopeShell 有一个核心概念：当前工作目录。它对应的是场景里某一个 Transform。
>
> 你可以用 cd 命令在不同的物体之间切换，就像在终端里切换文件夹一样。

**【画面】**
- 操作演示（实时录屏 + 打字效果）

```text
> addroot /World
> ls
Canvas,Env,BGM,...
> cd Canvas
> ls
Panel,Background,...
> cd Panel
> ls
TitleText,SubTitleText,SettingsToggle,VolumeSlider
> cd ..
> cd ../Env
```

**【旁白】**
> 首先用 addroot 把场景根物体加入管理器，这样 Shell 就知道从哪开始找了。
>
> ls 列出当前目录下所有子物体。
>
> cd 后面跟子物体名称，就进去了。cd ..回到上一层。
>
> 斜杠开头是绝对路径，不带斜杠是相对路径，和 Linux 终端完全一样的逻辑。

**【画面】**
- 突出显示提示符的变化：`/World>>>` → `/World/Canvas>>>` → `/World/Canvas/Panel>>>`
- 输入 `$pwd` 查看当前路径

**【旁白】**
> 注意看提示符，它会自动显示当前路径。你也可以用 $pwd 随时查看。
>
> pwd 是一个系统只读变量，每次 cd 都会自动更新它。

---

## 第四部分：操控物体（5:00 - 7:00）

**【画面】**
- 场景中有一个 Cube，选中后在 Inspector 上能看到 Transform 数值
- 镜头同时展示 Inspector 和命令行

**【旁白】**
> 有了路径导航，我们就可以对物体动手了。还记得那个点语法吗？
>
> 所有组件操作都以一个点开头，然后是组件缩写，再是属性名。

**【画面】**
- 操作演示（每一步在画面上标注语法格式）

```text
> cd /World/Env/MyCube

> .tf
输出: name: MyCube, active: True, position: (0, 0, 0), localPosition: (0, 0, 0), localScale: (1, 1, 1)

> .tf.lp
输出: (0, 0, 0)

> .tf.lp = (0, 2, 0)
输出: (0, 2, 0)
```
- **画面特写：Cube 在场景中向上移动了 2 个单位**

**【旁白】**
> .tf 是 Transform 的缩写。不写等号就是读取，写了等号就是设置。
>
> .tf.lp 是 localPosition，刚才 (0, 2, 0) 把它向上移了 2 个单位。
>
> 常用的还有 .tf.ls 缩放、.tf.lr 旋转、.tf.p 世界坐标。
>
> 在游戏里就能实时调整物体位置，不用回 Editor 重新打包。

**【画面】**
- 快速展示几个属性操作（表格化展示）

```text
.tf.ls = (2, 2, 2)     # 两倍放大
.tf.lr = (0, 90, 0)    # Y轴旋转90度
.en = false            # 禁用物体
.en = true             # 重新激活
```

**【旁白】**
> .en 直接控制物体的激活状态，true 显示，false 隐藏。所有这些变化都是即时生效的。

---

## 第五部分：操控 UI（7:00 - 9:00）

**【画面】**
- 场景中有一组 UI 面板：标题文字、开关、滑块
- 镜头先展示 UI 的初始状态

**【旁白】**
> 操控 UI 组件是 HopeShell 最实用的功能之一。不用在 Udon 脚本里写死文字和颜色，运行时随时可以改。

**【画面】**
- 实操：修改 Text 文字

```text
> cd /World/Canvas/Panel/TitleText
> .t
输出: "Hello World"

> .t = "欢迎来到我的世界"
> .t.c = (1, 0.8, 0)
> .t.fts = 48
```
- **画面特写：标题文字变大、变橙黄色**

**【旁白】**
> .t 是 Text 的缩写。.t 直接读写字面内容。
>
> .t.c 是颜色，括号里四个数字分别是红、绿、蓝、透明度，透明度可以不写，默认是 1。
>
> .t.fts 是字号，会自动适配普通 Text 和 TMP Text。

**【画面】**
- 实操：操控 Toggle 和 Slider

```text
> cd ../SettingsToggle
> .tg = true           ← 开关打开了
> .tg = false          ← 开关关闭了

> cd ../VolumeSlider
> .sl.val = 0.75       ← 滑块跳到 75%
> .sl.max = 2.0        ← 最大值改为 2
```
- **画面特写：Toggle 在 true/false 之间切换，Slider 滑块位置变化**

**【旁白】**
> .tg 控制 Toggle，.sl 控制 Slider。
>
> Slider 还能控制最大值、最小值和是否可交互。
>
> 比如你想在活动期间把某个滑条的范围从 0-1 临时改成 0-2，一行命令就搞定。

---

## 第六部分：变量系统（9:00 - 10:30）

**【画面】**
- 展示变量系统的概念图：三个数组 → 键值对

**【旁白】**
> HopeShell 有一个内置的变量系统，可以临时保存数据，还能在字符串里做模板替换。

**【画面】**
- 实操演示

```text
> $savedPos = .tf.lp
> .tf.lp = (100, 200, 300)
> .tf.lp = $savedPos        ← 恢复原位

> $playerName = "小明"
> .t = "你好, {$playerName}!"
> → 文字显示: 你好, 小明!

> $playerName = "小红"
> .t = "你好, {$playerName}!"
> → 文字显示: 你好, 小红!
```

**【旁白】_
> 美元符号开头的就是变量操作。$savedPos = .tf.lp 把当前位置存进变量。
>
> 然后一通操作之后，$savedPos 把位置恢复，非常方便。
>
> 花括号里面包变量名的写法，可以在字符串中做模板替换。切换玩家名字就改一个变量，所有用到的地方自动更新。
>
> 默认支持 20 个变量，够日常用了。

---

## 第七部分：UdonBehaviour（10:30 - 12:30）

**【画面】**
- 展示一个简单的 UdonBehaviour 脚本代码
- 高亮 public 变量：`public int score;` `public string status;`

```csharp
public class GameManager : UdonSharpBehaviour
{
    public int score;
    public string status;

    public void OnGameStart() { ... }
    public void OnPlayerJoin() { ... }
}
```

**【旁白】_
> 这一部分是 HopeShell 最强大的能力 —— 直接和 Udon 脚本交互。
>
> 场景里有一个 GameManager，上面有个 UdonBehaviour 挂载的脚本。
>
> 用 .u 就能读写它的 public 变量。

**【画面】**
- 实操演示

```text
> cd /World/GameManager

> .u.score
输出: .u.score : typ = System.Int32 , val = 0

> .u.score = 999
> .u.status = "active"

> .u.score
输出: .u.score : typ = System.Int32 , val = 999
```
- **画面特写：游戏内计分板或状态显示随之变化**

**【旁白】_
> 先读一下 score，当前是 0。设成 999，再读一下，已经变了。
>
> 支持的类型很多：bool、int、float、string、Vector2、Vector3、Vector2Int、Vector3Int，甚至是 int 数组和 float 数组。

**【画面】**
- 实操：触发自定义事件

```text
> .ut.OnGameStart
输出: Sent event OnGameStart

> .ut.OnPlayerJoin = VRChatUser
输出: Sent event OnPlayerJoin (eventData=VRChatUser)

> .ut.OnScoreChange = 100, bonus
输出: Sent event OnScoreChange (eventData=100 eventData2=bonus)
```
- **画面特写：事件触发后场景中的变化（游戏开始动画、玩家入场效果等）**

**【旁白】_
> .ut 是 Udon Event 的缩写，可以直接触发脚本里的任何自定义事件。
>
> 不写等号就是无参数事件。写一个值就是单参数。逗号分隔两个值就是双参数。
>
> 这意味着你可以用 HopeShell 作为调试工具，手动触发游戏流程中的任何一个事件，验证逻辑是否正常。
>
> 相比写几十行测试代码，一行命令就能搞定。

---

## 第八部分：远程注入（12:30 - 14:00）

**【画面】**
- 展示编辑器到运行时的数据流示意图（文档中的架构图简化版）
- 同时打开 Unity Editor 和 VRChat 游戏窗口

**【旁白】_
> 还有一个进阶功能 —— 远程命令注入。你可以在 Unity Editor 里操作，命令自动在 VRChat 里执行。
>
> 这套流程依赖三个组件配合：

**【画面】**
- 分步展示三个组件及其作用

| 组件 | 位置 | 作用 |
|------|------|------|
| HtoolSimpleWebSever | Editor 目录 | 本地 HTTP 服务器，端口 5004 |
| PropertyContextMenuExtender | Editor 目录 | Inspector 右键菜单 |
| HopeShellWebInput | 运行时预制体 | VRCStringDownloader 下载命令 |

**【旁白】_
> HtoolSimpleWebSever 是一个在 Editor 里跑的 HTTP 服务器，端口 5004。它有一个命令缓冲区，外部工具往里写命令，VRChat 从它读取命令。
>
> PropertyContextMenuExtender 给你的 Inspector 右键菜单加了两个选项。选中任意属性，右键就能生成对应的 HopeShell 命令并发送到缓冲区。
>
> HopeShellWebInput 挂在场景里，通过 VRCStringDownloader 定期拉取缓冲区的命令并执行。

**【画面】**
- 实操演示：Inspector 中选中 Transform 的 Position，右键 → "发送到 Web Buffer"
- 切换到 VRChat 窗口，几秒后物体位置自动更新

**【旁白】_
> 举个例子。在 Editor 里把 Position 调整到你满意的位置，右键发送到 Web Buffer。
>
> 几秒后，VRChat 里的物体就同步过去了。你甚至不需要在 VRChat 里敲任何东西。
>
> 这对于需要精细调整的场景 —— 比如排布大量 UI 元素 —— 简直太方便了。
>
> 另外还可以用 UdonExportTool 批量导出场景结构或文字内容，一次性注入。

**【画面】**
- 简单展示一下 `#cmd` / `msgid` / `msgstr` 批量命令格式

---

## 片尾（14:00 - 14:30）

**【画面】**
- 回顾核心功能清单（快速轮播）
- GitHub / Booth 链接信息占位

**【旁白】_
> 回顾一下，HopeShell 能做什么：
>
> cd + ls 在场景层级中导航，
> .tf 操控物体的位置旋转缩放，
> .t .tg .sl 实时调整 UI，
> .u .ut 读写 Udon 变量和触发事件，
> 还有变量系统和远程注入。
>
> 所有功能都可以在 VRChat 运行时直接使用，告别反复打包上传的痛苦。

**【画面】**
- 工具下载链接（留白，由用户自行填入）
- 文档地址链接
- 结尾画面

**【旁白】_
> 工具链接和完整文档在视频简介里，有问题欢迎留言交流。
>
> 如果这个工具对你有帮助，别忘了三连支持一下。我们下期见！

---

## 附加：拍摄提示

1. **命令输入效果**：建议用屏幕录制的键鼠高亮插件，让观众看清每次按键
2. **特效切换**：每次属性值变化时加一个微弱的高亮闪烁，引导观众注意变化的部分
3. **语法高亮**：命令行的文字可以用后期叠加颜色标注（命令名一种颜色、参数另一种颜色、等号红色高亮）
4. **速度控制**：敲命令的过程用 1.2x 轻微加速，结果展示用正常速度
5. **分屏**：第七部分 UdonBehaviour 演示时，左侧放命令行，右侧放脚本代码，方便对照
6. **背景音乐**：轻量电子/科技感背景音乐，音量控制在 -25dB 以下避免干扰旁白
