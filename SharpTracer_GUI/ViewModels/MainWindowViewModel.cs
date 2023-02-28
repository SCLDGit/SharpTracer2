using System;
using System.Numerics;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Metadata;
using Avalonia.Platform;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SharpTracer_Core.Math;
using SharpTracer_Core.RenderKernels;
using SharpTracer_Core.RenderKernels.Settings;
using Vector = Avalonia.Vector;

namespace SharpTracer_GUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            this.WhenAnyValue(p_vm => p_vm.SelectedKernelIndex)
                .Select(p_property => Observable.FromAsync(async () => await OnSelectedKernelChanged()))
                .Concat()
                .Subscribe();
        }

        [Reactive] public bool          IsRendering         { get; set; }
        [Reactive] public int           SelectedKernelIndex { get; set; }
        [Reactive] public IRenderKernel CurrentRenderKernel { get; set; }
        private           object        UpdateLock          { get; }      = new();
        [Reactive] public int           InputWidth          { get; set; } = 1366;
        [Reactive] public int           InputHeight         { get; set; } = 768;
        [Reactive] public int           InputSamples        { get; set; } = 16;
        [Reactive] public bool          RenderDataIsVisible { get; set; }
        [Reactive] public Bitmap?       RenderedImage       { get; set; }
        [Reactive] public string?       RenderTime          { get; set; }
        [Reactive] public int           RenderWidth         { get; set; }
        [Reactive] public int           RenderHeight        { get; set; }
        [Reactive] public int           RenderSamples       { get; set; }
        [Reactive] public int           XBucketSize         { get; set; } = 32;
        [Reactive] public int           YBucketSize         { get; set; } = 32;
        [Reactive] public float         XCameraOrigin       { get; set; }
        [Reactive] public float         YCameraOrigin       { get; set; } 
        [Reactive] public float         ZCameraOrigin       { get; set; } = 5.0f;
        [Reactive] public float         XCameraTarget       { get; set; }
        [Reactive] public float         YCameraTarget       { get; set; }
        [Reactive] public float         ZCameraTarget       { get; set; } = -1.0f;
        [Reactive] public float         XCameraUpVector     { get; set; }
        [Reactive] public float         YCameraUpVector     { get; set; } = 1.0f;
        [Reactive] public float         ZCameraUpVector     { get; set; }
        [Reactive] public float         CameraFieldOfView   { get; set; } = 20.0f;
        [Reactive] public float         CameraAperture      { get; set; } = 0.25f;
        [Reactive] public float         CameraFocalLength   { get; set; } = 5.0f;

        
        private async Task OnSelectedKernelChanged()
        {
            RenderDataIsVisible = false;
            RenderedImage       = null;

            await Task.CompletedTask;
        }

        [DependsOn(nameof(IsRendering))]
        public bool CanClickRender(object p_parameter)
        {
            return !IsRendering;
        }
        
        public async Task ClickRender()
        {
            IsRendering = true;
            
            var renderSettings = new RenderSettings( new Vector3(XCameraOrigin, YCameraOrigin, ZCameraOrigin), new Vector3(XCameraTarget, YCameraTarget, ZCameraTarget), new Vector3(XCameraUpVector, YCameraUpVector, ZCameraUpVector), CameraFieldOfView, CameraAperture, CameraFocalLength, InputHeight, InputWidth, InputSamples, XBucketSize, YBucketSize);

            CurrentRenderKernel = SelectedKernelIndex switch
                               {
                                   0 => new GradientKernel(renderSettings),
                                   1 => new SkyKernel(renderSettings),
                                   2 => new RedSphereKernel(renderSettings),
                                   3 => new NormalsSphereKernel(renderSettings),
                                   4 => new MultiNormalsSphereKernel(renderSettings),
                                   _ => throw new ArgumentOutOfRangeException()
                               };

            CurrentRenderKernel.RenderShouldUpdate += OnRenderShouldUpdate;

            await CurrentRenderKernel.Render();

            await Task.Delay(150);

            UpdateRenderPreview(CurrentRenderKernel.RenderResult!.RenderData!);

            RenderHeight  = renderSettings.Height;
            RenderWidth   = renderSettings.Width;
            RenderSamples = renderSettings.Samples;
            RenderTime    = $"{CurrentRenderKernel.RenderResult.ElapsedMilliseconds}ms";
            
            RenderDataIsVisible = true;

            IsRendering = false;
        }

        private void OnRenderShouldUpdate(object? p_sender, EventArgs p_e)
        {
            UpdateRenderPreview(CurrentRenderKernel!.RenderResult!.RenderData!);
        }

        private void UpdateRenderPreview(byte[] p_data)
        {
            lock (UpdateLock)
            {
                var newImage = new WriteableBitmap(new PixelSize(InputWidth, InputHeight),
                                                   new Vector(96, 96), PixelFormat.Rgba8888, AlphaFormat.Premul);
            
                using var lockedBitmap = newImage.Lock();
            
                Marshal.Copy(p_data, 0, new IntPtr(lockedBitmap.Address.ToInt64()), p_data.Length);

                RenderedImage = newImage;   
            }
        }
    }
}