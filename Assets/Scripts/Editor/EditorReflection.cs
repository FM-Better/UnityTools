#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EditorTool
{
    internal static class Reflection
    {
        public const BindingFlags Static = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        
        private static Assembly _type;
        private static Assembly Type => _type ??= Assembly.Load("UnityEditor");

        private static Assembly GetAssembly(string name)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            return assemblies.FirstOrDefault(assembly => assembly.GetName().Name == name);
        }

        public static Type GetEditorAssembly(string name, string assemblyName)
        {
            var assembly = GetAssembly(assemblyName);
            return assembly != null ? assembly.GetType("UnityEditor." + name) : null;
        }

        public static Type GetEditorType(string name) => Type.GetType("UnityEditor." + name);

        public static MethodInfo GetMethod(Type type, BindingFlags bindingFlags, string name, Type[] types)
        {
            return type.GetMethod(name, bindingFlags, null, types, null);
        }
    }


    internal static class UtilityRef
    {
        private static Type Type => typeof(EditorUtility);

        private static MethodInfo _displayObjectContextMenuMethod;

        private static MethodInfo DisplayObjectContextMenuMethod => _displayObjectContextMenuMethod ??=
            Reflection.GetMethod(Type, Reflection.Static, "DisplayObjectContextMenu",
                new[] { typeof(Rect), typeof(Object), typeof(int) });

        public static void DisplayObjectContextMenu(Rect position, Object context, int contextUserData)
        {
            DisplayObjectContextMenuMethod.Invoke(null, new object[] { position, context, contextUserData });
        }
    }
}
#endif