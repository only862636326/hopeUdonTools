# HopeShell Unity 组件子成员支持一览

> 列出每个组件在 Inspector 中的所有原生属性，标记 HopeShell 是否支持读写。

---

## Transform — `.tf` `.transform`

| Inspector 属性 | 序列化字段 | 是否支持 | HopeShell Key | 类型 | 备注 |
|---|---|---|---|---|---|
| Position | `m_LocalPosition` | 支持 | `lp` | Vector3 | |
| Rotation | `m_LocalRotation` + `m_LocalEulerAnglesHint` | 支持 | `lr` / `lel` | Euler(Vector3) | Quaternion → Euler 转换 |
| Scale | `m_LocalScale` | 支持 | `ls` | Vector3 | |

---

## GameObject — `.en` `.active`

| Inspector 属性 | 序列化字段 | 是否支持 | HopeShell Key | 类型 | 备注 |
|---|---|---|---|---|---|
| Name | `m_Name` | 不支持 | — | string | |
| Active Self (勾选框) | `m_IsActive` | 支持 | `en` | bool | |
| Tag | `m_TagString` | 不支持 | — | string | |
| Layer | `m_Layer` | 不支持 | — | int | |
| Static | `m_StaticEditorFlags` | 不支持 | — | flags | |

---

## Text (Legacy) — `.t` `.text`

| Inspector 属性 | 序列化字段 | 是否支持 | HopeShell Key | 类型 | 备注 |
|---|---|---|---|---|---|
| Text | `m_Text` | 支持 | `t` / ``(空)`` | string | |
| Font | `m_FontData` | 不支持 | — | FontData | 嵌套对象 |
| Font Style | — | 不支持 | — | — | FontData 子字段 |
| Font Size | `m_FontData.m_FontSize` / `m_FontSize` | 支持 | `fts` | int | |
| Line Spacing | `m_FontData.m_LineSpacing` | 不支持 | — | float | |
| Rich Text | `m_FontData.m_RichText` | 不支持 | — | bool | |
| Alignment | `m_FontData.m_Alignment` | 不支持 | — | enum | |
| Align By Geometry | `m_FontData.m_AlignByGeometry` | 不支持 | — | bool | |
| Horizontal Overflow | `m_FontData.m_HorizontalOverflow` | 不支持 | — | enum | |
| Vertical Overflow | `m_FontData.m_VerticalOverflow` | 不支持 | — | enum | |
| Best Fit | `m_FontData.m_BestFit` | 不支持 | — | bool | |
| Color | `m_Color` | 支持 | `c` | Color | |
| Material | `m_Material` | 不支持 | — | Material | 资源引用 |
| Raycast Target | `m_RaycastTarget` | 不支持 | — | bool | |
| Enabled | `m_Enabled` | 支持 | `en` | bool | |

---

## TMP_Text / TextMeshPro — `.t` `.text`

| Inspector 属性 | 序列化字段 | 是否支持 | HopeShell Key | 类型 | 备注 |
|---|---|---|---|---|---|
| Text | `m_text` | 支持 | `t` / ``(空)`` | string | |
| Font Asset | `m_fontAsset` | 不支持 | — | TMP_FontAsset | 资源引用 |
| Font Style | `m_fontStyle` | 不支持 | — | int/bitmask | |
| Font Size | `m_fontSize` | 支持 | `fts` | float | |
| Auto Size | `m_enableAutoSizing` | 不支持 | — | bool | |
| Font Size Min/Max | — | 不支持 | — | — | Auto Size 子字段 |
| Vertex Color | `m_faceColor` | 不支持 | — | Color | |
| Outline Color | `m_outlineColor` | 不支持 | — | Color | |
| Outline Width | `m_outlineWidth` | 不支持 | — | float | |
| Color | `m_fontColor` / 面板 Color | 支持 | `c` | Color | |
| Spacing Options | — | 不支持 | — | — | 多个子字段 |
| Alignment | `m_textAlignment` | 不支持 | — | enum | |
| Wrapping | `m_enableWordWrapping` | 不支持 | — | bool | |
| Overflow | `m_overflowMode` | 不支持 | — | enum | |
| Raycast Target | `m_raycastTarget` | 不支持 | — | bool | |
| Material | `m_fontSharedMaterial` / `m_fontMaterial` | 不支持 | — | Material | |
| Enabled | `m_Enabled` | 支持 | `en` | bool | |

---

## Image — `.img` `.image`

| Inspector 属性 | 序列化字段 | 是否支持 | HopeShell Key | 类型 | 备注 |
|---|---|---|---|---|---|
| Source Image | `m_Sprite` | 不支持 | — | Sprite | 资源引用 |
| Color | `m_Color` | 支持 | `c` | Color | |
| Material | `m_Material` | 不支持 | — | Material | 资源引用 |
| Raycast Target | `m_RaycastTarget` | 不支持 | — | bool | |
| Maskable | `m_Maskable` | 不支持 | — | bool | |
| Image Type | `m_Type` | 不支持 | — | enum | |
| Use Sprite Mesh | `m_UseSpriteMesh` | 不支持 | — | bool | |
| Preserve Aspect | `m_PreserveAspect` | 不支持 | — | bool | |
| Fill Center (Simple) | `m_FillCenter` | 不支持 | — | bool | |
| Fill Method | `m_FillMethod` | 不支持 | — | enum | |
| Fill Origin | `m_FillOrigin` | 不支持 | — | int | |
| Fill Amount | `m_FillAmount` | 不支持 | — | float | |
| Fill Clockwise | `m_FillClockwise` | 不支持 | — | bool | |
| Pixels Per Unit Multiplier | `m_PixelsPerUnitMultiplier` | 不支持 | — | float | |
| Enabled | `m_Enabled` | 支持 | `en` | bool | |

---

## RawImage — `.ri` `.rawimg`

| Inspector 属性 | 序列化字段 | 是否支持 | HopeShell Key | 类型 | 备注 |
|---|---|---|---|---|---|
| Texture | `m_Texture` | 不支持 | — | Texture | 资源引用 |
| Color | `m_Color` | 支持 | `c` | Color | |
| Material | `m_Material` | 不支持 | — | Material | 资源引用 |
| Raycast Target | `m_RaycastTarget` | 不支持 | — | bool | |
| Maskable | `m_Maskable` | 不支持 | — | bool | |
| UV Rect | `m_UvRect` | 不支持 | — | Rect | |
| Enabled | `m_Enabled` | 支持 | `en` | bool | |

---

## Toggle — `.tg` `.toggle`

| Inspector 属性 | 序列化字段 | 是否支持 | HopeShell Key | 类型 | 备注 |
|---|---|---|---|---|---|
| Is On | `m_IsOn` | 支持 | `isOn` / `on` / ``(空)`` | bool | |
| Toggle Transition | `m_ToggleTransition` | 不支持 | — | enum | |
| Graphic | `m_Graphic` | 不支持 | — | Graphic | 对象引用 |
| Group | `m_Group` | 不支持 | — | ToggleGroup | 对象引用 |
| Interactable | `m_Interactable` | 不支持 | — | bool | |
| Navigation | `m_Navigation` | 不支持 | — | Navigation | 嵌套结构 |
| Enabled | `m_Enabled` | 支持 | `en` | bool | |

---

## Slider — `.sl` `.slider`

| Inspector 属性 | 序列化字段 | 是否支持 | HopeShell Key | 类型 | 备注 |
|---|---|---|---|---|---|
| Fill Rect | `m_FillRect` | 不支持 | — | RectTransform | 对象引用 |
| Handle Rect | `m_HandleRect` | 不支持 | — | RectTransform | 对象引用 |
| Direction | `m_Direction` | 不支持 | — | enum | |
| Min Value | `m_MinValue` | 支持 | `min` | float | |
| Max Value | `m_MaxValue` | 支持 | `max` | float | |
| Whole Numbers | `m_WholeNumbers` | 不支持 | — | bool | |
| Value | `m_Value` | 支持 | `val` / ``(空)`` | float | |
| On Value Changed | `m_OnValueChanged` | 不支持 | — | UnityEvent | |
| Interactable | `m_Interactable` | 支持 | `int` | bool | |
| Navigation | `m_Navigation` | 不支持 | — | Navigation | |
| Enabled | `m_Enabled` | 支持 | `en` | bool | |

---

## AudioSource — `.as` `.aud` `.audio`

| Inspector 属性 | 序列化字段 | 是否支持 | HopeShell Key | 类型 | 备注 |
|---|---|---|---|---|---|
| Audio Clip | `m_audioClip` | 不支持 | — | AudioClip | 资源引用 |
| Output | `m_OutputAudioMixerGroup` | 不支持 | — | AudioMixerGroup | 资源引用 |
| Mute | `m_Mute` | 支持 | `mute` | bool | |
| Bypass Effects | `m_BypassEffects` | 不支持 | — | bool | |
| Bypass Listener Effects | `m_BypassListenerEffects` | 不支持 | — | bool | |
| Bypass Reverb Zones | `m_BypassReverbZones` | 不支持 | — | bool | |
| Play On Awake | `m_PlayOnAwake` | 不支持 | — | bool | |
| Loop | `m_Loop` | 支持 | `loop` | bool | |
| Priority | `m_Priority` | 不支持 | — | int | |
| Volume | `m_Volume` | 支持 | `vol` | float | |
| Pitch | `m_Pitch` | 支持 | `pitch` | float | |
| Stereo Pan | `m_StereoPan` | 不支持 | — | float | |
| Spatial Blend | `m_SpatialBlend` | 不支持 | — | float | |
| Reverb Zone Mix | `m_ReverbZoneMix` | 不支持 | — | float | |
| 3D Sound Settings (Doppler) | `m_DopplerLevel` | 不支持 | — | float | |
| 3D Sound Settings (Spread) | `m_Spread` | 不支持 | — | float | |
| 3D Sound Settings (Volume Rolloff) | `m_VolumeRolloff` | 不支持 | — | enum | |
| 3D Sound Settings (Min Distance) | `m_MinDistance` | 支持 | `min` | float | |
| 3D Sound Settings (Max Distance) | `m_MaxDistance` | 支持 | `max` | float | |
| Enabled | `m_Enabled` | 支持 | `en` | bool | |

---

## BoxCollider — `.bc` `.box`

| Inspector 属性 | 序列化字段 | 是否支持 | HopeShell Key | 类型 | 备注 |
|---|---|---|---|---|---|
| Is Trigger | `m_IsTrigger` | 支持 | `trig` | bool | |
| Material | `m_Material` | 不支持 | — | PhysicMaterial | 资源引用 |
| Center | `m_Center` | 不支持 | — | Vector3 | |
| Size | `m_Size` | 不支持 | — | Vector3 | |
| Enabled | `m_Enabled` | 支持 | `en` | bool | |

---

## UdonBehaviour — `.u` `.udon`

| Inspector 属性 | 序列化字段 | 是否支持 | HopeShell Key | 类型 | 备注 |
|---|---|---|---|---|---|
| Program Source | `programSource` | 不支持 | — | AbstractUdonProgramSource | 资源引用 |
| Serialized Program Asset | `serializedProgramAsset` | 不支持 | — | byte[] | |
| Interactable | `interact` | 不支持 | — | bool | |
| Sync Mode | `syncMode` | 不支持 | — | enum | |
| Allow Collision Ownership Transfer | `allowCollisionOwnershipTransfer` | 不支持 | — | bool | |
| `<public 变量>` | 对应变量名 | 支持 | `<变量名>` | bool/int/float/string/V2/V3/V2I/V3I/数组 | |
| Enabled | `m_Enabled` | 支持 | `en` | bool | |

---

## UdonBehaviour 事件 — `.ut` `.udonevent`

| Inspector 属性 | 是否支持 | HopeShell Key | 类型 | 备注 |
|---|---|---|---|---|
| EventName | 支持 | `<事件名>` | void | 无参调用 |
| EventName + eventData | 支持 | `<事件名> = val` | void | 单参数 |
| EventName + eventData/eventData2 | 支持 | `<事件名> = a, b` | void | 双参数 |

---

## 未支持组件汇总

| 组件 | 说明 |
|---|---|
| Button | 不支持任何属性 |
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
