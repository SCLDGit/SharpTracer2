using SharpTracer_Core.Primitives;
using SharpTracer_Core.RenderKernels.Settings;

namespace SharpTracer_Core.Threading;

public static class SceneUtilities
{
    public static Queue<RenderBucket> GetRenderBuckets(RenderSettings p_settings)
    {
        var queue = new Queue<RenderBucket>();
        
        for (var i = 0; i < p_settings.Width; i += p_settings.XBucketSize)
        {
            for (var j = 0; j < p_settings.Height; j += p_settings.YBucketSize)
            {
                queue.Enqueue(new RenderBucket(i, j));
            }
        }

        return queue;
    }
}