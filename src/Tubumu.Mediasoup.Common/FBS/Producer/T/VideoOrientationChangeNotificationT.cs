namespace FBS.Producer
{
    public class VideoOrientationChangeNotificationT
    {
        public bool Camera { get; set; }

        public bool Flip { get; set; }

        public ushort Rotation { get; set; }
    }
}
