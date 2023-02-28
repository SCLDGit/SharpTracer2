using System.Numerics;

namespace SharpTracer_Core.RenderKernels.Settings;

public class RenderSettings
{
    public RenderSettings(Vector3 p_cameraOrigin,
                          Vector3 p_cameraTarget,
                          Vector3 p_cameraUpVector,
                          float p_verticalFieldOfView,
                          float p_aperture,
                          float p_focalLength,
                          int p_height      = 768,
                          int p_width       = 1366,
                          int p_samples     = 16,
                          int p_xBucketSize = 32,
                          int p_yBucketSize = 32)
    {
        CameraOrigin        = p_cameraOrigin;
        CameraTarget        = p_cameraTarget;
        CameraUpVector      = p_cameraUpVector;
        VerticalFieldOfView = p_verticalFieldOfView;
        Aperture            = p_aperture;
        FocalLength         = p_focalLength;
        Height              = p_height;
        Width               = p_width;
        Samples             = p_samples;
        XBucketSize         = p_xBucketSize;
        YBucketSize         = p_yBucketSize;
    }
    public int     Height              { get; }
    public int     Width               { get; }
    public int     Samples             { get; }
    public int     XBucketSize         { get; }
    public int     YBucketSize         { get; }
    public Vector3 CameraOrigin        { get; }
    public Vector3 CameraTarget        { get; }
    public Vector3 CameraUpVector      { get; }
    public float   VerticalFieldOfView { get; }
    public float   Aperture            { get; }
    public float   FocalLength         { get; }
}