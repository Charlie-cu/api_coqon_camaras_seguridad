using api_coqon.Models;

namespace api_coqon.Services
{
    public interface ICameraService
    {
        Task<MemoryStream> GetVideoStreamAsync(string cameraId);
        Task<bool> SaveStreamToCloudAsync(string cameraId, Stream stream);
        Task StartStreamingAsync(string cameraId);
        Task StopStreamingAsync(string cameraId);
        Task AddCameraAsync(Camera camera);
        Task UpdateCameraSettingsAsync(string cameraId, CameraSettings settings);
        Task<IEnumerable<Camera>> GetAllCamerasAsync();
        Task DeleteCameraAsync(string cameraId);
    }
}