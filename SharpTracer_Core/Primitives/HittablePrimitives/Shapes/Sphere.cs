using System.Numerics;

namespace SharpTracer_Core.Primitives.HittablePrimitives.Shapes;

public record Sphere(Vector3 Center, float Radius) : IHittablePrimitive
{
    public bool WasHit(Ray p_ray, float p_tMin, float p_tMax, ref HitRecord p_hitRecord)
    {
        var oc = p_ray.Origin - Center;

        var a     = p_ray.Direction.LengthSquared();
        var halfB = Vector3.Dot(oc, p_ray.Direction);
        var c     = oc.LengthSquared() - Radius * Radius;

        var discriminant = halfB * halfB - a * c;
        
        if (discriminant < 0) return false;

        var discriminantSquareRoot = MathF.Sqrt(discriminant);

        var root = (-halfB - discriminantSquareRoot) / a;

        if (root < p_tMin || root > p_tMax)
        {
            root = (-halfB + discriminantSquareRoot) / a;

            if (root < p_tMin || root > p_tMax)
            {
                return false;
            }
        }

        p_hitRecord.T      = root;
        p_hitRecord.Point  = p_ray.CoordinateAt(p_hitRecord.T);

        var outwardNormal = (p_hitRecord.Point - Center) / Radius;
        
        p_hitRecord.SetFaceNormal(p_ray, outwardNormal);

        return true;
    }
}