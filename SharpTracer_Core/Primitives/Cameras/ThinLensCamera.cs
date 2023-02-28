using System.Numerics;
using SharpTracer_Core.Math;

namespace SharpTracer_Core.Primitives.Cameras;

public class ThinLensCamera : ICamera
{
    public ThinLensCamera(Vector3 p_origin, Vector3 p_target, Vector3 p_upVector, float p_verticalFieldOfView, 
                          float p_aspectRatio, float p_aperture, float p_focalLength)
    {
        var theta          = Utilities.DegreesToRadians(p_verticalFieldOfView);
        var h              = MathF.Tan(theta / 2.0f);
        var viewportHeight = 2.0f          * h;
        var viewportWidth  = p_aspectRatio * viewportHeight;

        W = Vector3.Normalize(p_origin - p_target);
        U = Vector3.Normalize(Vector3.Cross(p_upVector, W));
        V = Vector3.Cross(W, U);

        Origin = p_origin;

        Horizontal      = p_focalLength * viewportWidth  * U;
        Vertical        = p_focalLength * viewportHeight * V;
        LowerLeftCorner = Origin - Horizontal / 2.0f - Vertical / 2.0f - p_focalLength * W;

        LensRadius = p_aperture / 2.0f;
    }
    
    private Vector3 Origin          { get; }
    private Vector3 LowerLeftCorner { get; }
    private Vector3 Horizontal      { get; }
    private Vector3 Vertical        { get; }
    private Vector3 U               { get; }
    private Vector3 V               { get; }
    private Vector3 W               { get; }
    private float   LensRadius      { get; }
    
    public Ray GetRayAt(float p_s, float p_t)
    {
        var rd     = LensRadius * Utilities.GetRandomVectorInUnitDisc();
        var offset = U * rd.X + V * rd.Y;

        return new Ray(Origin + offset, LowerLeftCorner + p_s * Horizontal + p_t * Vertical - Origin - offset);
    }
}