using System.Diagnostics;

namespace Utility
{
    /// <summary>
    /// 封装只有在UnityEditor下起作用的Debug类 避免打包的处理
    /// </summary>
    public class DebugUtil
    {
        [Conditional("UNITY_EDITOR")]
        public static void Log(object message) => UnityEngine.Debug.Log(message);
        
        [Conditional("UNITY_EDITOR")]
        public static void Log(object message, string colorName) => UnityEngine.Debug.Log($"<color={colorName}>{message}</color>");
        
        [Conditional("UNITY_EDITOR")]
        public static void LogWarning(object message) => UnityEngine.Debug.LogWarning(message);
        
        [Conditional("UNITY_EDITOR")]
        public static void LogError(object message) => UnityEngine.Debug.LogError(message);
    }
}