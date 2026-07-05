# HopeShell 命令参考

## 内置命令

| 命令 | 别名 | 说明 | 用法 |
|---|---|---|---|
| `help` | — | 显示帮助信息 | `help` |
| `clear` | `cls` | 清屏 | `clear` |
| `history` | — | 显示历史命令 | `history` |
| `time` | — | 显示当前时间 | `time` |
| `ls` | — | 列出当前 Transform 的子物体，无 cd 时列出管理器根物体 | `ls` |
| `cd` | — | 切换当前工作目录（Transform） | `cd PATH` |
| `addroot` | — | 将 GameObject 添加到管理器 | `addroot PATH` |

---

## 变量系统 `$`

| 命令 | 说明 | 示例 |
|---|---|---|
| `$var` | 读取变量值 | `$pwd` → `pwd = /Root` |
| `$var = value` | 设置变量 | `$msg = "hello"` |
| `$var = $other` | 用其他变量赋值 | `$a = $b` |
| **`$pwd`** | 系统只读变量，自动跟随 cd 更新当前路径 | `$pwd` |

---

## 组件成员访问 `.`

通用格式：`.<组件缩写>.<子成员> = <值>`

**不写 `= 值` → 读取**
**写 `= 值` → 设置**

---

### `.tf` / `.transform` — Transform

| 子成员 | 别名 | 类型 | 示例 |
|---|---|---|---|
| `lp` | `localPosition` | Vector3 | `.tf.lp = (1, 2, 3)` |
| `lr` | `localRotation` | Euler(Vector3) | `.tf.lr = (0, 90, 0)` |
| `lel` | `localEulerAngles` | Vector3 | `.tf.lel = (0, 0, 0)` |
| `ls` | `localScale` | Vector3 | `.tf.ls = (1, 1, 1)` |
| `p` | `position` | Vector3 | `.tf.p = (0, 5, 0)` |
| `r` | `rotation/euler` | Euler(Vector3) | `.tf.r = (0, 180, 0)` |

> 无子成员 → 返回摘要：`name / active / position / localPosition / localScale`

---

### `.en` / `.active` — GameObject Active

| 命令 | 说明 | 示例 |
|---|---|---|
| `.en` | 读取 activeSelf | `.en` → `True` |
| `.en = true` | 设置 activeSelf | `.en = false` |

---

### `.t` / `.text` — Text / TMP_Text

| 子成员 | 别名 | 类型 | 示例 |
|---|---|---|---|
| `t` / `text` / ``(空)`` | — | string | `.t = "Hello"` |
| `fts` | `fontSize` | int/float | `.t.fts = 24` |
| `c` | `color` | (r,g,b,a) | `.t.c = (1, 0, 0)` |
| `en` | `enable` | bool | `.t.en = false` |

---

### `.tg` / `.toggle` — Toggle

| 子成员 | 别名 | 类型 | 示例 |
|---|---|---|---|
| `isOn` / `on` / ``(空)`` | — | bool | `.tg.isOn = true` |
| `en` | `enable` | bool | `.tg.en = false` |

---

### `.u` / `.udon` — UdonBehaviour

| 命令 | 说明 | 示例 |
|---|---|---|
| `.u` | 读取 enabled | `.u` → `True` |
| `.u.变量名` | 读取 public 变量 | `.u.myVar` |
| `.u.变量名 = 值` | 设置 public 变量 | `.u.myInt = 42` |
| `.u.en = true` | 设置 enabled | `.u.en = false` |

> 支持类型：`bool` `int` `float` `string` `Vector2` `Vector3` `Vector2Int` `Vector3Int` `int[]` `float[]`

---

### `.ut` / `.udonevent` — Udon 自定义事件

| 命令 | 说明 | 示例 |
|---|---|---|
| `.ut.EventName` | 发送无参数事件 | `.ut.OnReset` |
| `.ut.EventName = "val"` | 设置 eventData 并发送 | `.ut.OnSet = 42` |
| `.ut.EventName = "a, b"` | 设置 eventData / eventData2 并发送 | `.ut.OnSet = 1, hello` |

---

### `.img` / `.image` — Image / RawImage

| 子成员 | 别名 | 类型 | 示例 |
|---|---|---|---|
| `en` | `enable` | bool | `.img.en = false` |
| `c` | `color` | (r,g,b,a) | `.img.c = (1, 0, 0, 0.5)` |

> `.ri` / `.rawimg` 等同效；读 `.img` 返回摘要

---

### `.sl` / `.slider` — Slider

| 子成员 | 别名 | 类型 | 示例 |
|---|---|---|---|
| `val` | `value` | float | `.sl.val = 0.5` |
| `max` | `maxval` | float | `.sl.max = 1.0` |
| `min` | `minval` | float | `.sl.min = 0.0` |
| `en` | `enable` | bool | `.sl.en = false` |
| `int` | `interactable` | bool | `.sl.int = true` |

---

### `.as` / `.aud` / `.audio` — AudioSource

| 子成员 | 别名 | 类型 | 示例 |
|---|---|---|---|
| `mute` | — | bool | `.as.mute = true` |
| `loop` | — | bool | `.as.loop = false` |
| `en` | `enable` | bool | `.as.en = true` |
| `vol` | `volume` | float (0~1) | `.as.vol = 0.8` |
| `pitch` | — | float (-3~3) | `.as.pitch = 1.2` |
| `min` | `mindist` | float | `.as.min = 1` |
| `max` | `maxdist` | float | `.as.max = 100` |

---

### `.bc` / `.box` — BoxCollider

| 子成员 | 别名 | 类型 | 示例 |
|---|---|---|---|
| `en` | `enable` | bool | `.bc.en = false` |
| `trig` | `istrigger` | bool | `.bc.trig = true` |

---

## 路径规则 `cd`

| 路径 | 说明 |
|---|---|
| `cd ObjectName` | 相对路径，进入当前目录下子物体 |
| `cd /Root/Child` | 绝对路径，从管理器根或场景根开始 |
| `cd ..` | 返回上级目录 |
| `cd .` | 当前目录（刷新 pwd） |
| `cd "Name With Spaces"` | 名称含空格的用引号 |
| `cd $varname` | 使用变量中的路径 |
| `cd msgid` | 缩写，`msgid /Path` ≡ `cd /Path` |

---

## 值格式参考

| 类型 | 格式 | 示例 |
|---|---|---|
| string | 双引号或单引号 | `"hello"` `'world'` |
| bool | `true` / `false` / `on` / `off` / `1` / `0` | `true` |
| int | 数字 | `42` |
| float | 数字 | `3.14` |
| Vector3 | `(x, y, z)` | `(1， 2， 3)` |
| Vector2 | `(x, y)` | `(0.5， 0.5)` |
| Color | `(r, g, b, a)` a可选默认1 | `(1， 0， 0， 0.5)` |
| Quaternion | `(x, y, z, w)` | `(0， 0， 0， 1)` |
| 数组 | `(1, 2, 3, 4)` | 同 Vector 格式，长度决定类型 |

---

## 批量命令 / Text Input

`InputCommandText` 方法支持多行文本输入，特殊前缀：

| 前缀 | 转换 | 示例 |
|---|---|---|
| `#cmd` | 去除前缀直接执行 | `#cmd cd /Root` |
| `#` / `//` | 注释行（跳过） | `# 这是注释` |
| `msgid` | 转为 `cd ` | `msgid Path` → `cd Path` |
| `msgstr` | 转为 `.t = ` | `msgstr Hello` → `.t = Hello` |
