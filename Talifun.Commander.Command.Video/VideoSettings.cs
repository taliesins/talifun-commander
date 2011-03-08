namespace Talifun.Commander.Command.Video
{
    public class VideoSettings
    {
        public int AudioBitRate { get; set; } //-ab {AudioBitRate}
        public int AudioFrequency { get; set; } //-ar {AudioFrequency}
        public int AudioChannels { get; set; }//-ac {AudioChannels - 1, 2}

        public bool Deinterlace { get; set; } //-deinterlace
        public int Width { get; set; } //-s {Width}x{Height}
        public int Height { get; set; } //-s {Width}x{Height}
        public AspectRatio AspectRatio { get; set; } //-aspect {AspectRatio - 4:3, 16:9, 16:10}
        public int FrameRate { get; set; } //-r {framerate}
        public int VideoBitRate { get; set; } //-b {VideoBitRate}

        public int? MaxVideoBitRate { get; set; } //-maxrate {MaxVideoBitRate}
        public int? BufferSize { get; set; } //-bufsize {BufferSize}
        public int? KeyframeInterval { get; set; }//-g {KeyframeInterval}
        public int? MinKeyframeInterval { get; set; }//-keyint_min {KeyframeInterval}
    }
}