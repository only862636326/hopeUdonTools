
using SGS;
using System;
using System.Runtime.InteropServices;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;


namespace HopeTools
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class HopeUdonIoc : UdonSharpBehaviour
    {
        [Header("服务配置")]
        public int maxServices = 50;  // 最大服务数量

        // 服务存储数组 - 替代字典
        [SerializeField] private string[] serviceNames;
        private UdonSharpBehaviour[] serviceInstancesUdon;
        private object[] serviceInstancesObj;
        private bool[] isSingleton;
        private int serviceCount = 0;
        private HopeUdonFramework hufw;
        private void Start()
        {
            //Init();
        }
        private bool _is_init = false;
        // 初始化
        public void Init()
        {
            if (this._is_init)
                return;

            _is_init = true;
            InitializeArrays();

            hufw = this.GetComponentInParent<HopeUdonFramework>();
            hufw.udondebug.LogUdonMsg(this, "[HopeUdonIoc] IOC容器初始化完成");
        }



        /// <summary>
        /// 初始化数组
        /// </summary>
        private void InitializeArrays()
        {
            maxServices = 50;
            serviceNames = new string[maxServices];
            serviceInstancesUdon = new UdonSharpBehaviour[maxServices];
            serviceInstancesObj = new object[maxServices];
            isSingleton = new bool[maxServices];
            serviceCount = 0;
        }

        /// <summary>
        /// 自动扩容数组（当容量不足时）
        /// </summary>
        private void ExpandArrays()
        {
            int oldCapacity = maxServices;
            int newCapacity = oldCapacity + 25; // 每次扩容25个
            
            hufw.udondebug.LogUdonMsg(this, "[HopeUdonIoc] 开始扩容，从 " + oldCapacity + " 到 " + newCapacity);
            
            // 创建新数组
            string[] newServiceNames = new string[newCapacity];
            UdonSharpBehaviour[] newServiceInstances = new UdonSharpBehaviour[newCapacity];
            object[] newServiceInstancesobj = new object[newCapacity];
            bool[] newIsSingleton = new bool[newCapacity];
            
            // 复制现有数据
            for (int i = 0; i < serviceCount; i++)
            {
                newServiceNames[i] = serviceNames[i];
                newServiceInstances[i] = serviceInstancesUdon[i];
                newServiceInstancesobj[i] = serviceInstancesObj[i];
                newIsSingleton[i] = isSingleton[i];
            }
            
            // 更新引用
            serviceNames = newServiceNames;
            serviceInstancesUdon = newServiceInstances;
            serviceInstancesObj = newServiceInstancesobj;
            isSingleton = newIsSingleton;
            maxServices = newCapacity;
            
            hufw.udondebug.LogUdonMsg(this, "[HopeUdonIoc] 扩容完成，新容量: " + newCapacity);
        }



        /// <summary>
        /// 查找服务索引
        /// </summary>
        private int FindServiceIndex(string serviceName)
        {
            for (int i = 0; i < serviceCount; i++)
            {
                if (serviceNames[i] == serviceName)
                {
                    return i;
                }
            }
            return -1;
        }



        /// <summary>
        /// 注册单例服务（同时提供UdonSharpBehaviour和Object两个参数）
        /// </summary>
        public void RegisterSingleton(string serviceName, UdonSharpBehaviour udonInstance, object objInstance)
        {
            int existingIndex = FindServiceIndex(serviceName);            

            if (existingIndex >= 0)
            {
                // 替换现有服务
                serviceInstancesUdon[serviceCount] = udonInstance;
                serviceInstancesObj[serviceCount] = objInstance;
                isSingleton[existingIndex] = true;

                hufw.udondebug.LogUdonMsg(this, "[HopeUdonIoc] 替换双参数单例服务: " + serviceName);
            }
            else if (serviceCount < maxServices)
            {
                // 添加新服务
                serviceNames[serviceCount] = serviceName;
                serviceInstancesUdon[serviceCount] = udonInstance;
                serviceInstancesObj[serviceCount] = objInstance;
                isSingleton[serviceCount] = true;
                serviceCount++;

                hufw.udondebug.LogUdonMsg(this, "[HopeUdonIoc] 注册双参数单例服务: " + serviceName);
            }
            else
            {
                // 数组已满，自动扩容
                ExpandArrays();
                
                // 重新尝试注册
                if (serviceCount < maxServices)
                {
                    serviceNames[serviceCount] = serviceName;
                    serviceInstancesUdon[serviceCount] = udonInstance;
                    serviceInstancesObj[serviceCount] = objInstance;
                    isSingleton[serviceCount] = true;
                    serviceCount++;

                    hufw.udondebug.LogUdonMsg(this, "[HopeUdonIoc] 扩容后注册双参数单例服务: " + serviceName);
                }
                else
                {
                    hufw.udondebug.LogUdonMsg(this, "[HopeUdonIoc] 扩容失败，服务数量已达上限: " + maxServices);
                }
            }
        }

        /// <summary>
        /// 获取服务实例
        /// </summary>
        public UdonSharpBehaviour GetServiceUdon(string serviceName)
        {
            int index = FindServiceIndex(serviceName);

            if (index >= 0)
            {
                return serviceInstancesUdon[index];
            }

            hufw.udondebug.LogUdonMsg(this, "[HopeUdonIoc] 服务未找到: " + serviceName);
            return null;
        }

        /// <summary>
        /// 获取服务实例（Object版本）
        /// </summary>
        public object GetServiceObj(string serviceName)
        {
            int index = FindServiceIndex(serviceName);

            if (index >= 0)
            {
                return serviceInstancesObj[index];
            }

            hufw.udondebug.LogUdonMsg(this, "[HopeUdonIoc] 服务未找到(Object): " + serviceName);
            return null;
        }



        /// <summary>
        /// 检查服务是否存在
        /// </summary>
        public bool HasService(string serviceName)
        {
            return FindServiceIndex(serviceName) >= 0;
        }

        /// <summary>
        /// 移除服务
        /// </summary>
        public void RemoveService(string serviceName)
        {
            int index = FindServiceIndex(serviceName);

            if (index >= 0)
            {
                // 将最后一个服务移到被删除的位置
                int lastIndex = serviceCount - 1;
                if (index < lastIndex)
                {
                    serviceNames[index] = serviceNames[lastIndex];
                    serviceInstancesUdon[index] = serviceInstancesUdon[lastIndex];
                    serviceInstancesObj[index] = serviceInstancesObj[lastIndex];
                    isSingleton[index] = isSingleton[lastIndex];
                }

                // 清理最后一个位置
                serviceNames[lastIndex] = null;
                serviceInstancesUdon[lastIndex] = null;
                serviceInstancesObj[lastIndex] = null;
                serviceCount--;

                hufw.udondebug.LogUdonMsg(this, "[HopeUdonIoc] 移除服务: " + serviceName);
            }
        }

        ///// <summary>
        ///// 手动注入服务到指定字段（Udon不支持反射，需要手动调用）
        ///// </summary>
        //public void InjectService(UdonSharpBehaviour target, string fieldName, string serviceName)
        //{
        //    if (!HasService(serviceName))
        //    {
        //        hufw.udondebug.LogUdonMsg(this, "[HopeUdonIoc] 服务未找到: " + serviceName);
        //        return;
        //    }

        //    var service = GetServiceUdon(serviceName);
        //    if (service == null)
        //    {
        //        hufw.udondebug.LogUdonMsg(this, "[HopeUdonIoc] 服务实例为null: " + serviceName);
        //        return;
        //    }

        //    // 由于Udon不支持反射，这里只是记录日志
        //    // 实际注入需要在目标脚本中手动实现
        //    hufw.udondebug.LogUdonMsg(this, "[HopeUdonIoc] 手动注入服务 " + serviceName + " 到字段 " + fieldName);
        //}

        /// <summary>
        /// 清除所有服务
        /// </summary>
        public void ClearAllServices()
        {
            InitializeArrays();
            hufw.udondebug.LogUdonMsg(this, "[HopeUdonIoc] 所有服务已清除并重新初始化");
        }

        /// <summary>
        /// 获取服务统计信息
        /// </summary>
        public string GetServiceStats()
        {
            string stats = "服务统计:\n";
            stats += "已注册服务: " + serviceCount + "\n";
            stats += "当前容量: " + maxServices + "\n";
            stats += "容量使用率: " + ((serviceCount * 100) / maxServices) + "%\n";
            stats += "单例服务: " + GetSingletonCount() + "\n";
            stats += "调试系统: HopeUdonFramework";

            if (serviceCount > 0)
            {
                stats += "\n服务列表:\n";
                for (int i = 0; i < serviceCount; i++)
                {
                    string serviceType = isSingleton[i] ? "[单例]" : "[瞬态]";
                    string serviceName = serviceNames[i];
                    string instanceInfo = "null";
                    
                    if (serviceInstancesObj[i] != null)
                    {
                        instanceInfo = serviceInstancesObj[i].GetType().Name;
                    }
                    else if (serviceInstancesUdon[i] != null)
                    {
                        instanceInfo = "UdonSharpBehaviour";
                    }
                    
                    stats += "  - " + serviceName + ": " + instanceInfo + " " + serviceType + "\n";
                }
            }

            return stats;
        }

        /// <summary>
        /// 获取单例服务数量
        /// </summary>
        private int GetSingletonCount()
        {
            int count = 0;
            for (int i = 0; i < serviceCount; i++)
            {
                if (isSingleton[i])
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// 调试日志输出
        /// </summary>
        private void DebugLog(string message)
        {
            hufw.udondebug.LogUdonMsg(this, "[HopeUdonIoc] " + message);
        }

        /// <summary>
        /// 获取IOC实例（需要在场景中手动获取）
        /// </summary>
        public HopeUdonIoc GetIocInstance()
        {
            return this;
        }
    }
}