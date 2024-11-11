namespace api_coqon.Models
{
    public class Camera
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string IpAddress { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Resolution { get; set; }
        public int? FrameRate { get; set; }
        public bool IsEnabled { get; set; }
    }

    public class CameraSettings
    {
        public string Name { get; set; }
        public string IpAddress { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Resolution { get; set; }
        public int? FrameRate { get; set; }
        public bool? IsEnabled { get; set; }
    }
}