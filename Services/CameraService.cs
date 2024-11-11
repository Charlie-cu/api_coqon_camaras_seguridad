using api_coqon.Models;
using FFMpegCore;
using FFMpegCore.Pipes;
using System.Diagnostics;

namespace api_coqon.Services
{
    public class CameraService : ICameraService
    {
        private readonly Dictionary<string, Camera> _cameras = new();
        private readonly pCloudService _pCloudService;
        private readonly Dictionary<string, Process> _activeStreams = new();

        public CameraService(pCloudService pCloudService)
        {
            _pCloudService = pCloudService;
        }

        public async Task<MemoryStream> GetVideoStreamAsync(string cameraId)
        {
            try
            {
                if (!_cameras.TryGetValue(cameraId, out var camera))
                    throw new KeyNotFoundException("Cámara no encontrada");

                var rtspUrl = GetSafeRtspUrl(camera);
                var outputStream = new MemoryStream();

                await FFMpegArguments
                    .FromUrlInput(new Uri(rtspUrl))
                    .OutputToPipe(new StreamPipeSink(outputStream), options => options
                        .WithVideoCodec("libx264")
                        .ForceFormat("mpegts")
                        .WithAudioCodec("aac")
                        .WithCustomArgument("-preset ultrafast -tune zerolatency"))
                    .ProcessAsynchronously();

                return outputStream;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error while streaming video: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> SaveStreamToCloudAsync(string cameraId, Stream stream)
        {
            try
            {
                if (!_cameras.TryGetValue(cameraId, out var camera))
                    throw new KeyNotFoundException("Cámara no encontrada");

                var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
                var fileName = $"camera_{cameraId}_{timestamp}.mp4";

                await _pCloudService.UploadStreamAsync(fileName, stream);
                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error saving stream to cloud: {ex.Message}");
                return false;
            }
        }


        public async Task StartStreamingAsync(string cameraId)
        {
            try
            {
                if (!_cameras.TryGetValue(cameraId, out var camera))
                    throw new KeyNotFoundException("Cámara no encontrada");

                if (_activeStreams.ContainsKey(cameraId))
                    throw new InvalidOperationException("La transmisión ya está activa para esta cámara");

                var rtspUrl = GetSafeRtspUrl(camera);

                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = $"-i {rtspUrl} -c:v libx264 -preset ultrafast -tune zerolatency -c:a aac -f mpegts pipe:1",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                var process = new Process { StartInfo = processStartInfo };
                process.Start();

                _activeStreams.Add(cameraId, process);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error starting stream: {ex.Message}");
                throw;
            }
        }

        public async Task StopStreamingAsync(string cameraId)
        {
            try
            {
                if (!_activeStreams.TryGetValue(cameraId, out var process))
                    throw new KeyNotFoundException("No hay transmisión activa para esta cámara");

                if (!process.HasExited)
                {
                    process.Kill();
                    process.Dispose();
                }

                _activeStreams.Remove(cameraId);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error stopping stream: {ex.Message}");
                throw;
            }
        }

        private string GetSafeRtspUrl(Camera camera)
        {
            return $"rtsp://{camera.Username}:{new string('*', camera.Password.Length)}@{camera.IpAddress}/stream1";
        }
        public async Task AddCameraAsync(Camera camera)
        {
            if (string.IsNullOrEmpty(camera.Id))
                camera.Id = Guid.NewGuid().ToString();

            if (_cameras.ContainsKey(camera.Id))
                throw new InvalidOperationException("Ya existe una cámara con este ID");

            if (string.IsNullOrEmpty(camera.IpAddress))
                throw new ArgumentException("La dirección IP es requerida");

            _cameras.Add(camera.Id, camera);
            await Task.CompletedTask;
        }
        public async Task UpdateCameraSettingsAsync(string cameraId, CameraSettings settings)
        {
            if (!_cameras.TryGetValue(cameraId, out var camera))
                throw new KeyNotFoundException("Cámara no encontrada");

            camera.Username = settings.Username ?? camera.Username;
            camera.Password = settings.Password ?? camera.Password;
            camera.IpAddress = settings.IpAddress ?? camera.IpAddress;
            camera.Name = settings.Name ?? camera.Name;
            camera.Resolution = settings.Resolution ?? camera.Resolution;
            camera.FrameRate = settings.FrameRate ?? camera.FrameRate;
            camera.IsEnabled = settings.IsEnabled ?? camera.IsEnabled;

            await Task.CompletedTask;
        }
        public async Task<IEnumerable<Camera>> GetAllCamerasAsync()
        {
            return await Task.FromResult(_cameras.Values);
        }
        public async Task DeleteCameraAsync(string cameraId)
        {
            if (!_cameras.TryGetValue(cameraId, out _))
                throw new KeyNotFoundException("Cámara no encontrada");

            if (_activeStreams.ContainsKey(cameraId))
                await StopStreamingAsync(cameraId);

            _cameras.Remove(cameraId);
            await Task.CompletedTask;
        }

    }
}
