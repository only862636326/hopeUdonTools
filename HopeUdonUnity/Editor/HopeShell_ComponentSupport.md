# HopeShell Unity 组件支持一览

## 已支持组件详情

### Transform — `.tf` `.transform`

| 子成员 Key | 完整名 | 别名 | 类型 | 读写 |
|---|---|---|---|---|
| `lp` | localPosition | `localPosition` | Vector3 | R/W |
| `lr` | localRotation | `localRotation` | Euler(Vector3) | R/W |
| `lel` | localEulerAngles | `localEulerAngles`, `leuler` | Vector3 | R/W |
| `ls` | localScale | `localScale` | Vector3 | R/W |
| `p` | position | `position` | Vector3 | R/W |
| `r` | rotation (world euler) | `rotation`, `euler`, `el` | Euler(Vector3) | R/W |

### GameObject — `.en` `.active` `.isenabled`

| 子成员 Key | 完整名 | 别名 | 类型 | 读写 |
|---|---|---|---|---|
| `en` | activeSelf | `active`, `isenabled` | bool | R/W |

### Text / TMP_Text — `.t` `.text`

| 子成员 Key | 完整名 | 别名 | 类型 | 读写 |
|---|---|---|---|---|
| `t` / ``(空)`` | text | `text` | string | R/W |
| `fts` | fontSize | `fontSize` | int / float | R/W |
| `c` | color | `color` | Color(r,g,b,a) | R/W |
| `en` | enabled | `enable`, `active`, `isenabled` | bool | R/W |

### Image — `.img` `.image`

| 子成员 Key | 完整名 | 别名 | 类型 | 读写 |
|---|---|---|---|---|
| `en` | enabled | `enable`, `active`, `isenabled` | bool | R/W |
| `c` | color | `color` | Color(r,g,b,a) | R/W |

### RawImage — `.ri` `.rawimg` `.rawimage`

| 子成员 Key | 完整名 | 别名 | 类型 | 读写 |
|---|---|---|---|---|
| `en` | enabled | `enable`, `active`, `isenabled` | bool | R/W |
| `c` | color | `color` | Color(r,g,b,a) | R/W |

### Toggle — `.tg` `.toggle` `.tog`

| 子成员 Key | 完整名 | 别名 | 类型 | 读写 |
|---|---|---|---|---|
| `isOn` / ``(空)`` | isOn | `is_on`, `on` | bool | R/W |
| `en` | enabled | `enable`, `active`, `isenabled` | bool | R/W |

### Slider — `.sl` `.slider`

| 子成员 Key | 完整名 | 别名 | 类型 | 读写 |
|---|---|---|---|---|
| `val` / ``(空)`` | value | `value` | float | R/W |
| `max` | maxValue | `maxval`, `maxvalue` | float | R/W |
| `min` | minValue | `minval`, `minvalue` | float | R/W |
| `en` | enabled | `enable`, `active`, `isenabled` | bool | R/W |
| `int` | interactable | `interactable` | bool | R/W |

### AudioSource — `.as` `.aud` `.audio` `.audiosource`

| 子成员 Key | 完整名 | 别名 | 类型 | 读写 |
|---|---|---|---|---|
| `mute` | mute | — | bool | R/W |
| `loop` | loop | — | bool | R/W |
| `en` | enabled | `enable`, `active`, `isenabled` | bool | R/W |
| `vol` | volume | `volume` | float (0~1) | R/W |
| `pitch` | pitch | — | float (-3~3) | R/W |
| `min` | minDistance | `mindist`, `mindistance` | float | R/W |
| `max` | maxDistance | `maxdist`, `maxdistance` | float | R/W |

### BoxCollider — `.bc` `.box` `.boxcollider`

| 子成员 Key | 完整名 | 别名 | 类型 | 读写 |
|---|---|---|---|---|
| `en` | enabled | `enable`, `active`, `isenabled` | bool | R/W |
| `trig` | isTrigger | `istrigger`, `trigger` | bool | R/W |

### UdonBehaviour — `.u` `.udon` `.udonsharp`

| 子成员 Key | 完整名 | 别名 | 类型 | 读写 |
|---|---|---|---|---|
| `en` | enabled | `enable`, `active`, `isenabled` | bool | R/W |
| `<变量名>` | 任意 public 变量 | — | bool / int / float / string / Vector2 / Vector3 / Vector2Int / Vector3Int / int[] / float[] | R/W |

### UdonBehaviour 事件 — `.ut` `.udonevent` `.uevent` `.uevn`

| 子成员 Key | 完整名 | 别名 | 类型 | 读写 |
|---|---|---|---|---|
| `<事件名>` | SendCustomEvent | — | void | 写 |
| `<事件名> = val` | 设置 eventData 后发送 | — | void | 写 |
| `<事件名> = a, b` | 设置 eventData / eventData2 后发送 | — | void | 写 |

---

## 未支持组件

| 组件 | 说明 |
|---|---|
| Button | 不支持 |
| Canvas | 不支持 |
| CanvasGroup | 不支持 |
| InputField | 不支持 |
| Dropdown | 不支持 |
| ScrollRect | 不支持 |
| Scrollbar | 不支持 |
| Camera | 不支持 |
| Light | 不支持 |
| MeshRenderer | 不支持 |
| SkinnedMeshRenderer | 不支持 |
| SphereCollider | 不支持 |
| CapsuleCollider | 不支持 |
| MeshCollider | 不支持 |
| Rigidbody | 不支持 |
| Animator | 不支持 |
| Animation | 不支持 |
| ParticleSystem | 不支持 |
| LineRenderer | 不支持 |
| NavMeshAgent | 不支持 |
| LayoutGroup (Vertical/Horizontal/Grid) | 不支持 |
| ContentSizeFitter | 不支持 |
| Outline / Shadow | 不支持 |
| VideoPlayer | 不支持 |
