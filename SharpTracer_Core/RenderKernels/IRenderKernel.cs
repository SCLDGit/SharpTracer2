using SharpTracer_Core.RenderKernels.Results;
using SharpTracer_Core.RenderKernels.Settings;

namespace SharpTracer_Core.RenderKernels;

public interface IRenderKernel
{
    public event EventHandler RenderShouldUpdate;
    public RenderSettings     Settings     { get; }
    public RenderResults?     RenderResult { get; }
    
    public Task Render();
}