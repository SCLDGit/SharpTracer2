using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using SharpTracer_Core.Math;
using SharpTracer_Core.Primitives;
using SharpTracer_Core.Primitives.Cameras;
using SharpTracer_Core.Primitives.HittablePrimitives.Shapes;
using SharpTracer_Core.RenderKernels.Results;
using SharpTracer_Core.RenderKernels.Settings;

namespace SharpTracer_Core.RenderKernels;

public class MultiNormalsSphereKernel : IRenderKernel
{
    public MultiNormalsSphereKernel(RenderSettings p_settings)
    {
        Settings = p_settings;
        
        Scene = new Scene();
        
        Scene.Entities.Add(new Sphere(new Vector3(0, 0, -1), 0.5f));
        Scene.Entities.Add(new Sphere(new Vector3(0, -100.5f, -1), 100.0f));

        Camera = new ThinLensCamera(p_settings.CameraOrigin, p_settings.CameraTarget, p_settings.CameraUpVector,
                                    p_settings.VerticalFieldOfView, (float)p_settings.Width / p_settings.Height,
                                    p_settings.Aperture, p_settings.FocalLength);
    }

    public event EventHandler? RenderShouldUpdate;
    
    public  ICamera        Camera       { get; }
    public  RenderSettings Settings     { get; }
    public  RenderResults? RenderResult { get; private set; }
    private Scene          Scene        { get; }

    public async Task Render()
    {
        RenderResult = new RenderResults
                       {
                           RenderData = new byte[Settings.Height * Settings.Width * 4]
                       };
        
        var sw = Stopwatch.StartNew();

        var buckets = Threading.SceneUtilities.GetRenderBuckets(Settings);

        var semaphore = new SemaphoreSlim(Environment.ProcessorCount);

        var bucketTasks = new List<Task>();

        var timer = new System.Timers.Timer();

        timer.Elapsed   += OnRenderShouldUpdate;
        timer.Interval  =  1000;
        timer.AutoReset =  true;
        timer.Enabled   =  true;

        timer.Start();

        while (buckets.Count > 0)
        {
            await semaphore.WaitAsync();

            var bucket = buckets.Dequeue();
            
            bucketTasks.Add(Task.Run(() =>
                                     {
                                         try
                                         {
                                             ProcessRenderBucket(bucket);
                                         }
                                         finally
                                         {
                                             semaphore.Release();
                                         }
                                     }));
        }

        Task.WaitAll(bucketTasks.ToArray());

        sw.Stop();
        
        timer.Stop();

        RenderResult.ElapsedMilliseconds = sw.ElapsedMilliseconds;

        await Task.CompletedTask;
    }

    private void OnRenderShouldUpdate(object? p_sender, EventArgs p_e)
    {
        RenderShouldUpdate(this, EventArgs.Empty);
    }

    private void ProcessRenderBucket(RenderBucket p_bucket)
    {
        var endYIndex = p_bucket.StartYIndex + Settings.YBucketSize > Settings.Height
                            ? Settings.Height
                            : p_bucket.StartYIndex + Settings.YBucketSize;
        
        var endXIndex = p_bucket.StartXIndex + Settings.XBucketSize > Settings.Width
                            ? Settings.Width
                            : p_bucket.StartXIndex + Settings.XBucketSize;
        
        var scale = 1.0f / Settings.Samples;
        
        for (var row = p_bucket.StartYIndex; row < endYIndex; ++row)
        {
            for (var column = p_bucket.StartXIndex; column < endXIndex; ++column)
            {
                var index = ((Settings.Height - 1 - row) * Settings.Width + column) * 4;

                var color = Vector3.Zero;

                for (var sample = 0; sample < Settings.Samples; ++sample)
                {
                    var u   = (column + Utilities.GetRandomFloat()) / (Settings.Width  - 1);
                    var v   = (row    + Utilities.GetRandomFloat()) / (Settings.Height - 1);
                    var ray = Camera.GetRayAt(u, v);
                    color += GetRayColor(ray);
                }

                color *= scale;

                RenderResult!.RenderData![index]   = (byte)(int)(255 * color.X);
                RenderResult.RenderData[index + 1] = (byte)(int)(255 * color.Y);
                RenderResult.RenderData[index + 2] = (byte)(int)(255 * color.Z);
                RenderResult.RenderData[index + 3] = 255;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Vector3 GetRayColor(Ray p_ray)
    {
        var hitRecord = new HitRecord();

        if (Scene.WasHit(p_ray, 0.0f, float.PositiveInfinity, ref hitRecord))
        {
            return 0.5f * (hitRecord.Normal + new Vector3(1));
        }
        
        var unitDirection = Vector3.Normalize(p_ray.Direction);
        var t = 0.5f * (unitDirection.Y + 1.0f);

        return (1.0f - t) * new Vector3(1.0f) + t * new Vector3(0.5f, 0.7f, 1.0f);
    }
}