namespace SharpTracer_Core.Primitives.Cameras;

public interface ICamera
{
    public Ray GetRayAt(float p_u, float p_v);
}