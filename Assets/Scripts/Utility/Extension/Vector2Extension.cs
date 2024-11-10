using UnityEngine;

namespace Utility
{
    public static class VectorExtension
    {
        public static Vector2 ToVector2(this Vector3 vector) => new Vector2(vector.x, vector.y);

        public static Vector3 ToVector3(this Vector2 vector) => new Vector3(vector.x, vector.y, 0f);
    }
}
