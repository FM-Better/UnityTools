using UnityEditor;

namespace Utility
{
    public class AssetUtil
    {
#if UNITY_EDITOR
        public static bool AssetExists(string assetPath) => AssetDatabase.AssetPathToGUID(assetPath).Length > 0;
#endif
    }
}
