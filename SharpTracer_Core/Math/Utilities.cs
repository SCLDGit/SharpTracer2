using System.Numerics;

namespace SharpTracer_Core.Math;

public static class Utilities
{
    public static float DegreesToRadians(float p_degrees)
    {
        return p_degrees * MathF.PI / 180.0f;
    }

    public static float GetRandomFloat()
    {
        return Random.Shared.NextSingle();
    }

    public static float GetRandomFloat(float p_min, float p_max)
    {
        return p_min + (p_max - p_min) * GetRandomFloat();
    }

    public static Vector3 GetRandomVec3()
    {
        return new Vector3(GetRandomFloat(), GetRandomFloat(), GetRandomFloat());
    }

    public static Vector3 GetRandomVec3(float p_min, float p_max)
    {
        return new Vector3(GetRandomFloat(p_min, p_max), GetRandomFloat(p_min, p_max), GetRandomFloat(p_min, p_max));
    }

    public static Vector3 GetRandomVectorInUnitSphere()
    {
        while (true)
        {
            var point = GetRandomVec3(-1, 1);
            if (point.LengthSquared() >= 1) continue;
            return point;
        }
    }

    public static Vector3 GetRandomVectorInUnitDisc()
    {
        while (true)
        {
            var point = new Vector3(GetRandomFloat(-1, 1), GetRandomFloat(-1, 1), 0.0f);
            if (point.LengthSquared() >= 1) continue;
            return point;
        }
    }
}