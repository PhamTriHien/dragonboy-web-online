using System;

namespace UnityEngine
{
    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp Mathf: Đảm nhiệm phân hệ chuyên trách Math ().
    /// </summary>
    public static class Mathf
    {
        public const float PI = MathF.PI;
        public const float Deg2Rad = MathF.PI / 180f;
        public const float Rad2Deg = 180f / MathF.PI;

        public static float Abs(float f) => MathF.Abs(f);
        public static int Abs(int value) => System.Math.Abs(value);
        public static float Min(float a, float b) => MathF.Min(a, b);
        public static int Min(int a, int b) => System.Math.Min(a, b);
        public static float Max(float a, float b) => MathF.Max(a, b);
        public static int Max(int a, int b) => System.Math.Max(a, b);
        public static float Sqrt(float f) => MathF.Sqrt(f);
        public static float Sin(float f) => MathF.Sin(f);
        public static float Cos(float f) => MathF.Cos(f);
        public static float Atan(float f) => MathF.Atan(f);
        public static float Atan2(float y, float x) => MathF.Atan2(y, x);
        public static float Clamp(float value, float min, float max) => System.Math.Clamp(value, min, max);
        public static int Clamp(int value, int min, int max) => System.Math.Clamp(value, min, max);
        public static float Floor(float f) => MathF.Floor(f);
        public static int FloorToInt(float f) => (int)MathF.Floor(f);
        public static float Ceil(float f) => MathF.Ceiling(f);
        public static int CeilToInt(float f) => (int)MathF.Ceiling(f);
        public static float Round(float f) => MathF.Round(f);
        public static int RoundToInt(float f) => (int)MathF.Round(f);
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp Vector2: Đảm nhiệm phân hệ chuyên trách Math ().
    /// </summary>
    public struct Vector2
    {
        public float x;
        public float y;

        /// <summary>
        /// Thực thi phương thức nghiệp vụ Vector2.
        /// </summary>
        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2 zero => new Vector2(0f, 0f);
        public static Vector2 one => new Vector2(1f, 1f);
        public float magnitude => MathF.Sqrt(x * x + y * y);

        /// <summary>
        /// Thực thi phương thức nghiệp vụ Distance.
        /// </summary>
        public static float Distance(Vector2 a, Vector2 b)
        {
            float dx = a.x - b.x;
            float dy = a.y - b.y;
            return MathF.Sqrt(dx * dx + dy * dy);
        }

        public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.x + b.x, a.y + b.y);
        public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.x - b.x, a.y - b.y);
        public static Vector2 operator *(Vector2 a, float d) => new Vector2(a.x * d, a.y * d);
        public static Vector2 operator /(Vector2 a, float d) => new Vector2(a.x / d, a.y / d);
        public static bool operator ==(Vector2 a, Vector2 b) => a.x == b.x && a.y == b.y;
        public static bool operator !=(Vector2 a, Vector2 b) => !(a == b);

        public override bool Equals(object obj) => obj is Vector2 v && this == v;
        public override int GetHashCode() => HashCode.Combine(x, y);
        public override string ToString() => $"({x}, {y})";
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp Vector3: Đảm nhiệm phân hệ chuyên trách Math ().
    /// </summary>
    public struct Vector3
    {
        public float x;
        public float y;
        public float z;

        /// <summary>
        /// Thực thi phương thức nghiệp vụ Vector3.
        /// </summary>
        public Vector3(float x, float y, float z = 0f)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Vector3 zero => new Vector3(0f, 0f, 0f);
        public static Vector3 one => new Vector3(1f, 1f, 1f);

        public static Vector3 operator +(Vector3 a, Vector3 b) => new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        public static Vector3 operator -(Vector3 a, Vector3 b) => new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        public static Vector3 operator *(Vector3 a, float d) => new Vector3(a.x * d, a.y * d, a.z * d);
        public static Vector3 operator /(Vector3 a, float d) => new Vector3(a.x / d, a.y / d, a.z / d);
        public static bool operator ==(Vector3 a, Vector3 b) => a.x == b.x && a.y == b.y && a.z == b.z;
        public static bool operator !=(Vector3 a, Vector3 b) => !(a == b);

        public override bool Equals(object obj) => obj is Vector3 v && this == v;
        public override int GetHashCode() => HashCode.Combine(x, y, z);
        public override string ToString() => $"({x}, {y}, {z})";
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp Rect: Đảm nhiệm phân hệ chuyên trách Math ().
    /// </summary>
    public struct Rect
    {
        public float x;
        public float y;
        public float width;
        public float height;

        /// <summary>
        /// Thực thi phương thức nghiệp vụ Rect.
        /// </summary>
        public Rect(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// Thực thi phương thức nghiệp vụ Rect.
        /// </summary>
        public Rect(Rect source)
        {
            x = source.x;
            y = source.y;
            width = source.width;
            height = source.height;
        }

        public float xMin => x;
        public float xMax => x + width;
        public float yMin => y;
        public float yMax => y + height;

        /// <summary>
        /// Thực thi phương thức nghiệp vụ Contains.
        /// </summary>
        public bool Contains(Vector2 point)
        {
            return point.x >= x && point.x <= x + width && point.y >= y && point.y <= y + height;
        }

        public static bool operator ==(Rect a, Rect b) => a.x == b.x && a.y == b.y && a.width == b.width && a.height == b.height;
        public static bool operator !=(Rect a, Rect b) => !(a == b);

        public override bool Equals(object obj) => obj is Rect r && this == r;
        public override int GetHashCode() => HashCode.Combine(x, y, width, height);
        public override string ToString() => $"(x:{x}, y:{y}, width:{width}, height:{height})";
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp Matrix4x4: Đảm nhiệm phân hệ chuyên trách Math ().
    /// </summary>
    public struct Matrix4x4
    {
        public float m00, m10, m20, m30;
        public float m01, m11, m21, m31;
        public float m02, m12, m22, m32;
        public float m03, m13, m23, m33;

        public static Matrix4x4 identity
        {
            get
            {
                Matrix4x4 m = default;
                m.m00 = 1f;
                m.m11 = 1f;
                m.m22 = 1f;
                m.m33 = 1f;
                return m;
            }
        }

        /// <summary>
        /// Thiết lập giá trị hoặc trạng thái cho SetTRS.
        /// </summary>
        public void SetTRS(Vector3 pos, Quaternion q, Vector3 s)
        {
            this = identity;
            m00 = s.x;
            m11 = s.y;
            m22 = s.z;
            m03 = pos.x;
            m13 = pos.y;
            m23 = pos.z;
        }

        public static Matrix4x4 operator *(Matrix4x4 lhs, Matrix4x4 rhs)
        {
            Matrix4x4 res = default;
            res.m00 = lhs.m00 * rhs.m00 + lhs.m01 * rhs.m10 + lhs.m02 * rhs.m20 + lhs.m03 * rhs.m30;
            res.m01 = lhs.m00 * rhs.m01 + lhs.m01 * rhs.m11 + lhs.m02 * rhs.m21 + lhs.m03 * rhs.m31;
            res.m02 = lhs.m00 * rhs.m02 + lhs.m01 * rhs.m12 + lhs.m02 * rhs.m22 + lhs.m03 * rhs.m32;
            res.m03 = lhs.m00 * rhs.m03 + lhs.m01 * rhs.m13 + lhs.m02 * rhs.m23 + lhs.m03 * rhs.m33;

            res.m10 = lhs.m10 * rhs.m00 + lhs.m11 * rhs.m10 + lhs.m12 * rhs.m20 + lhs.m13 * rhs.m30;
            res.m11 = lhs.m10 * rhs.m01 + lhs.m11 * rhs.m11 + lhs.m12 * rhs.m21 + lhs.m13 * rhs.m31;
            res.m12 = lhs.m10 * rhs.m02 + lhs.m11 * rhs.m12 + lhs.m12 * rhs.m22 + lhs.m13 * rhs.m32;
            res.m13 = lhs.m10 * rhs.m03 + lhs.m11 * rhs.m13 + lhs.m12 * rhs.m23 + lhs.m13 * rhs.m33;

            res.m20 = lhs.m20 * rhs.m00 + lhs.m21 * rhs.m10 + lhs.m22 * rhs.m20 + lhs.m23 * rhs.m30;
            res.m21 = lhs.m20 * rhs.m01 + lhs.m21 * rhs.m11 + lhs.m22 * rhs.m21 + lhs.m23 * rhs.m31;
            res.m22 = lhs.m20 * rhs.m02 + lhs.m21 * rhs.m12 + lhs.m22 * rhs.m22 + lhs.m23 * rhs.m32;
            res.m23 = lhs.m20 * rhs.m03 + lhs.m21 * rhs.m13 + lhs.m22 * rhs.m23 + lhs.m23 * rhs.m33;

            res.m30 = lhs.m30 * rhs.m00 + lhs.m31 * rhs.m10 + lhs.m32 * rhs.m20 + lhs.m33 * rhs.m30;
            res.m31 = lhs.m30 * rhs.m01 + lhs.m31 * rhs.m11 + lhs.m32 * rhs.m21 + lhs.m33 * rhs.m31;
            res.m32 = lhs.m30 * rhs.m02 + lhs.m31 * rhs.m12 + lhs.m32 * rhs.m22 + lhs.m33 * rhs.m32;
            res.m33 = lhs.m30 * rhs.m03 + lhs.m31 * rhs.m13 + lhs.m32 * rhs.m23 + lhs.m33 * rhs.m33;
            return res;
        }
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp Quaternion: Đảm nhiệm phân hệ chuyên trách Math ().
    /// </summary>
    public struct Quaternion
    {
        public float x, y, z, w;
        /// <summary>
        /// Thực thi phương thức nghiệp vụ Quaternion.
        /// </summary>
        public Quaternion(float x, float y, float z, float w) { this.x = x; this.y = y; this.z = z; this.w = w; }
        public static Quaternion identity => new Quaternion(0f, 0f, 0f, 1f);
    }
}
