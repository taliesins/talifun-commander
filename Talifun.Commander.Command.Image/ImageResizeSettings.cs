namespace Talifun.Commander.Command.Image
{
    public class ImageResizeSettings : IImageResizeSettings
    {
        /// <summary>
        /// The out image type to use
        /// </summary>
        public ResizeImageType ResizeImageType { get; set; }
        public ResizeMode ResizeMode { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public string BackgroundColour { get; set; }
        public Gravity Gravity { get; set; }
        public int? Quality { get; set; }
		public string WatermarkPath { get; set; }
		public int WatermarkDissolveLevels { get; set; }
		public Gravity WatermarkGravity { get; set; }
    }
}