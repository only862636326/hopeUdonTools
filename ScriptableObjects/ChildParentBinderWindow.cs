using UnityEngine;
using System.Collections.Generic;

namespace ChildParentBinding
{
    // 创建配置文件的菜单
    [CreateAssetMenu(fileName = "ChildParentBindingConfig", menuName = "Custom/Child Parent Binding Config", order = 51)]
    public class ChildParentBindingConfig : ScriptableObject
    {
        // 根物体配置
        [Header("根物体配置")]
        [Tooltip("作为父物体来源的根物体")]
        public Transform rootA;
        [Tooltip("作为子物体来源的根物体")]
        public Transform rootB;

        // 匹配规则
        [Header("匹配规则")]
        public MatchMode matchMode = MatchMode.ByName;
        [Tooltip("是否包含非激活的子物体")]
        public bool includeInactive = true;

        // 绑定模式
        [Header("绑定模式")]
        public BindMode bindMode = BindMode.ChangeHierarchy;
        [Tooltip("绑定后保持子物体世界坐标不变")]
        public bool keepWorldPosition = true;

        // 约束配置（仅BindMode.Constraint生效）
        [Header("约束配置（仅约束模式生效）")]
        public bool constrainPosition = true;
        public bool constrainRotation = true;
        [Range(0, 1)] public float constraintWeight = 1f;

        // 匹配结果（序列化保存）
        [Header("匹配结果（自动生成）")]
        [SerializeField] private List<ChildMatchPair> matchPairs = new List<ChildMatchPair>();

        // 枚举定义
        public enum MatchMode
        {
            ByName,    // 按名称匹配
            ByIndex,   // 按索引匹配
            Manual     // 手动指定
        }

        public enum BindMode
        {
            ChangeHierarchy, // 修改父子层级
            Constraint       // 仅添加约束组件（不修改层级）
        }

        // 子物体匹配对（序列化）
        [System.Serializable]
        public class ChildMatchPair
        {
            public Transform childA; // RootA的子物体（父）
            public Transform childB; // RootB的子物体（子）
            [HideInInspector] public Transform originalParentB; // 记录原父物体（用于重置）
            [HideInInspector] public bool hasConstraint; // 记录是否添加约束
        }

        // 对外暴露匹配对列表（供编辑器访问）
        public List<ChildMatchPair> MatchPairs => matchPairs;

        // 清空匹配对
        public void ClearMatchPairs() => matchPairs.Clear();

        // 添加匹配对
        public void AddMatchPair(Transform a, Transform b)
        {
            if (a == null || b == null) return;
            matchPairs.Add(new ChildMatchPair
            {
                childA = a,
                childB = b,
                originalParentB = b.parent,
                hasConstraint = false
            });
        }

        // 移除指定索引的匹配对
        public void RemoveMatchPair(int index)
        {
            if (index >= 0 && index < matchPairs.Count)
            {
                matchPairs.RemoveAt(index);
            }
        }

        // 获取所有子物体（辅助方法）
        public List<Transform> GetAllChildren(Transform root)
        {
            List<Transform> children = new List<Transform>();
            foreach (Transform child in root)
            {
                if (includeInactive || child.gameObject.activeInHierarchy)
                {
                    children.Add(child);
                    children.AddRange(GetAllChildren(child));
                }
            }
            return children;
        }
    }
}


