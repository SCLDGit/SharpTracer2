using System.Diagnostics;
using System.Numerics;
using SharpTracer_Core.Primitives;
using SharpTracer_Core.RenderKernels.Results;
using SharpTracer_Core.RenderKernels.Settings;

namespace SharpTracer_Core.RenderKernels;

public class SkyKernel : IRenderKernel
{
    public SkyKernel(RenderSettings p_settings)
    {
        Settings = p_settings;
    }
    
    public event EventHandler RenderShouldUpdate;
    public RenderSettings     Settings     { get; }
    public RenderResults?     RenderResult { get; private set; }
    
    public async Task Render()
    {
        RenderResult = new RenderResults
                       {
                           RenderData = new byte[Settings.Height * Settings.Width * 4]
                       };
        
        var aspectRatio = (float)Settings.Width / Settings.Height;

        var viewportHeight = 2.0f;
        var viewportWidth  = aspectRatio * viewportHeight;
        var focalLength    = 1.0f;

        var origin          = new Vector3(0.0f);
        var horizontal      = new Vector3(viewportWidth, 0, 0);
        var vertical        = new Vector3(0, viewportHeight, 0);
        var lowerLeftCorner = origin - horizontal / 2 - vertical / 2 - new Vector3(0, 0, focalLength);

        var sw = Stopwatch.StartNew();
        
        for (var row = 0; row < Settings.Height; ++row)
        {
            for (var column = 0; column < Settings.Width; ++column)
            {
                var index = ((Settings.Height - 1 - row) * Settings.Width + column) * 4;
                
                var u     = (float)column / (Settings.Width  - 1);
                var v     = (float)row    / (Settings.Height - 1);
                var ray   = new Ray(origin, lowerLeftCorner + u * horizontal + v * vertical - origin);
                var color = GetRayColor(ray);
                
                RenderResult.RenderData[index]     = (byte) (int)(255 * color.X);
                RenderResult.RenderData[index + 1] = (byte) (int)(255 * color.Y);
                RenderResult.RenderData[index + 2] = (byte) (int)(255 * color.Z);
                RenderResult.RenderData[index + 3] = 255;
            }
        }
        
        sw.Stop();

        RenderResult.ElapsedMilliseconds = sw.ElapsedMilliseconds;

        await Task.CompletedTask;
    }

    private Vector3 GetRayColor(Ray p_ray)
    {
        var unitDirection = Vector3.Normalize(p_ray.Direction);
        var t             = 0.5f * (unitDirection.Y + 1.0f);
        return (1.0f - t) * new Vector3(1.0f) + t * new Vector3(0.5f, 0.7f, 1.0f);
    }
}