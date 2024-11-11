using api_coqon.Models;
using api_coqon.Services;
using Microsoft.AspNetCore.Mvc;

namespace api_coqon.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CameraController : ControllerBase
    {
        private readonly ICameraService _cameraService;

        public CameraController(ICameraService cameraService)
        {
            _cameraService = cameraService;
        }

        [HttpGet("stream/{cameraId}")]
        public async Task<IActionResult> GetStream(string cameraId)
        {
            var stream = await _cameraService.GetVideoStreamAsync(cameraId);
            return File(stream, "video/MP2T");
        }

        [HttpPost("start/{cameraId}")]
        public async Task<IActionResult> StartStreaming(string cameraId)
        {
            await _cameraService.StartStreamingAsync(cameraId);
            return Ok(new { success = true });
        }

        [HttpPost("stop/{cameraId}")]
        public async Task<IActionResult> StopStreaming(string cameraId)
        {
            await _cameraService.StopStreamingAsync(cameraId);
            return Ok(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> AddCamera([FromBody] Camera camera)
        {
            await _cameraService.AddCameraAsync(camera);
            return Ok(new { success = true });
        }

        [HttpPut("{cameraId}/settings")]
        public async Task<IActionResult> UpdateSettings(string cameraId, [FromBody] CameraSettings settings)
        {
            await _cameraService.UpdateCameraSettingsAsync(cameraId, settings);
            return Ok(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCameras()
        {
            var cameras = await _cameraService.GetAllCamerasAsync();
            return Ok(cameras);
        }

        [HttpDelete("{cameraId}")]
        public async Task<IActionResult> DeleteCamera(string cameraId)
        {
            await _cameraService.DeleteCameraAsync(cameraId);
            return Ok(new { success = true });
        }

        [HttpPost("saveToCloud/{cameraId}")]
        public async Task<IActionResult> SaveToCloud(string cameraId)
        {
            var videoStream = await _cameraService.GetVideoStreamAsync(cameraId);
            var result = await _cameraService.SaveStreamToCloudAsync(cameraId, videoStream);
            return Ok(new { success = result });
        }
    }
}
