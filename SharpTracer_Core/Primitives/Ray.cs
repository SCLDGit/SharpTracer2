using System.Numerics;

namespace SharpTracer_Core.Primitives;

public readonly struct Ray
{
    public Ray(Vector3 p_origin, Vector3 p_direction)
    {
        Origin    = p_origin;
        Direction = p_direction;
    }
    
    public Vector3 Origin    { get; }
    public Vector3 Direction { get; }

    public Vector3 CoordinateAt(float p_t)
    {
        return Origin + p_t * Direction;
    }
}