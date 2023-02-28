using SharpTracer_Core.Primitives.HittablePrimitives;

namespace SharpTracer_Core.Primitives;

public class Scene : IHittablePrimitive
{
    public List<IHittablePrimitive> Entities { get; } = new ();

    public bool WasHit(Ray p_ray, float p_tMin, float p_tMax, ref HitRecord p_hitRecord)
    {
        var tempHitRecord = new HitRecord();
        var       hitAnything = false;
        var       closestHit  = p_tMax;

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var t in Entities)
        {
            if (!t.WasHit(p_ray, p_tMin, closestHit, ref tempHitRecord)) continue;
            
            hitAnything = true;
            closestHit  = tempHitRecord.T;
            p_hitRecord = tempHitRecord;
        }

        // LINQ here is ~3x as slow as above loop. - Comment by Matt Heimlich on 02/16/2023@11:26:26
        // foreach (var entity in Entities.Where(p_entity => p_entity.WasHit(p_ray, p_tMin, closestHit, ref tempHitRecord)))
        // {
        //     hitAnything = true;
        //     closestHit  = tempHitRecord.T;
        //     p_hitRecord = tempHitRecord;
        // }

        return hitAnything;
    }
}