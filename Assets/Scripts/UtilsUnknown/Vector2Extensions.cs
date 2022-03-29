using UnityEngine;

namespace UtilsUnknown.Extensions
{
    public static class Vector2Extensions
    {
        public static float RandomInRange(this Vector2 vector2)
        {
            if (vector2.x <= vector2.y) return Random.Range(vector2.x, vector2.y);
            return Random.Range(vector2.y, vector2.x);
        }

        public static float MinValue(this Vector2 vector2)
        {
            return Mathf.Min(vector2.x, vector2.y);
        }

        public static float MaxValue(this Vector2 vector2)
        {
            return Mathf.Max(vector2.x, vector2.y);
        }

        public static float Sum(this Vector2 vector2)
        {
            return vector2.x + vector2.y;
        }

        public static float Diff(this Vector2 vector2)
        {
            return Mathf.Abs(vector2.x - vector2.y);
        }
    }

    public static class Vector2IntExtensions
    {
        public static int RandomInRange(this Vector2Int vector2)
        {
            if (vector2.x <= vector2.y) return Random.Range(vector2.x, vector2.y);
            return Random.Range(vector2.y, vector2.x);
        }

        public static int MinValue(this Vector2Int vector2)
        {
            return Mathf.Min(vector2.x, vector2.y);
        }

        public static int MaxValue(this Vector2Int vector2)
        {
            return Mathf.Max(vector2.x, vector2.y);
        }

        public static int Sum(this Vector2Int vector2)
        {
            return vector2.x + vector2.y;
        }

        public static int Diff(this Vector2Int vector2)
        {
            return Mathf.Abs(vector2.x - vector2.y);
        }
    }
}
