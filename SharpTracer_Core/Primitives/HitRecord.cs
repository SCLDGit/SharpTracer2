using System.Numerics;

namespace SharpTracer_Core.Primitives;

public struct HitRecord
{
    public  Vector3 Point         { get; set; }
    public  Vector3 Normal        { get; set; }
    public  float   T             { get; set; }
    private bool    IsFrontFacing { get; set; }

    public void SetFaceNormal(Ray p_ray, Vector3 p_outwardNormal)
    {
        IsFrontFacing = Vector3.Dot(p_ray.Direction, p_outwardNormal) < 0;
        Normal        = IsFrontFacing ? p_outwardNormal : -p_outwardNormal;
    }
}