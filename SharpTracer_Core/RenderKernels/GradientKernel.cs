using System.Diagnostics;
using SharpTracer_Core.RenderKernels.Results;
using SharpTracer_Core.RenderKernels.Settings;

namespace SharpTracer_Core.RenderKernels;

public class GradientKernel : IRenderKernel
{
    public GradientKernel(RenderSettings p_settings)
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

        var sw = Stopwatch.StartNew();

        for (var row = 0; row < Settings.Height; ++row)
        {
            for (var column = 0; column < Settings.Width; ++column)
            {
                var index = ((Settings.Height - 1 - row) * Settings.Width + column) * 4;

                var         red   = (float)column / Settings.Width;
                var         green = (float)row    / Settings.Height;
                const float blue  = 0.25f;

                RenderResult.RenderData[index]     = (byte) (int)(255 * red);
                RenderResult.RenderData[index + 1] = (byte) (int)(255 * green);
                RenderResult.RenderData[index + 2] = (int)(255 * blue);
                RenderResult.RenderData[index + 3] = 255;
            }
        }
        
        sw.Stop();

        RenderResult.ElapsedMilliseconds = sw.ElapsedMilliseconds;

        await Task.CompletedTask;
    }
}