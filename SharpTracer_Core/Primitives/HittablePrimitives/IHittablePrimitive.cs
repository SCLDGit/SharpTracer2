namespace SharpTracer_Core.Primitives.HittablePrimitives;

public interface IHittablePrimitive
{
    public bool WasHit(Ray p_ray, float p_tMin, float p_tMax, ref HitRecord p_hitRecord);
}