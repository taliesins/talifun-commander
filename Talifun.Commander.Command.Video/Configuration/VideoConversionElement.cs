using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Video.Configuration
{
    /// <summary>
    /// Represents a configuration element within a configuration file that configures options for custom string formatting providers.
    /// </summary>
    public sealed partial class VideoConversionElement : CommandConfigurationBase
    {
		private static readonly ConfigurationProperty audioConversionType = new ConfigurationProperty("audioConversionType", typeof(AudioConversionType), AudioConversionType.NotSpecified, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty audioBitRate = new ConfigurationProperty("audioBitRate", typeof(int), 64000, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty audioFrequency = new ConfigurationProperty("audioFrequency", typeof(int), 44100, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty audioChannels = new ConfigurationProperty("audioChannels", typeof(int), 2, ConfigurationPropertyOptions.None);

		private static readonly ConfigurationProperty videoConversionType = new ConfigurationProperty("videoConversionType", typeof(VideoConversionType), VideoConversionType.NotSpecified, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty deinterlace = new ConfigurationProperty("deinterlace", typeof(bool), false, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty width = new ConfigurationProperty("width", typeof(int), null, ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty height = new ConfigurationProperty("height", typeof(int), null, ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty videoBitRate = new ConfigurationProperty("videoBitRate", typeof(int), null, ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty frameRate = new ConfigurationProperty("frameRate", typeof(int), null, ConfigurationPropertyOptions.IsRequired);

        private static readonly ConfigurationProperty aspectRatio = new ConfigurationProperty("aspectRatio", typeof(AspectRatio), AspectRatio.NotSpecified, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty maxVideoBitRate = new ConfigurationProperty("maxVideoBitRate", typeof(int), 0, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty bufferSize = new ConfigurationProperty("bufferSize", typeof(int), 0, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty keyFrameInterval = new ConfigurationProperty("keyFrameInterval", typeof(int), 0, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty minKeyFrameInterval = new ConfigurationProperty("minKeyFrameInterval", typeof(int), 0, ConfigurationPropertyOptions.None);

        /// <summary>
        /// Initializes the <see cref="VideoConversionElement"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static VideoConversionElement()
        {
			properties.Add(audioConversionType);
            properties.Add(audioBitRate);
            properties.Add(audioFrequency);
            properties.Add(audioChannels);

			properties.Add(videoConversionType);
            properties.Add(deinterlace);
            properties.Add(width);
            properties.Add(height);
            properties.Add(videoBitRate);
            properties.Add(frameRate);
            properties.Add(aspectRatio);
            properties.Add(maxVideoBitRate);
            properties.Add(bufferSize);
            properties.Add(keyFrameInterval);
            properties.Add(minKeyFrameInterval);
        }

		/// <summary>
		/// Gets or sets the audio conversion type to use.
		/// </summary>
		[ConfigurationProperty("audioConversionType", DefaultValue = AudioConversionType.NotSpecified)]
		public AudioConversionType AudioConversionType
		{
			get { return ((AudioConversionType)base[audioConversionType]); }
			set { SetPropertyValue(value, audioConversionType, "AudioConversionType"); }
		}

        /// <summary>
        /// Gets or sets the bitrate to use for the audio.
        /// Popular rates are:
        /// 192000
        /// 128000
        /// 96000
        /// 64000
        /// 32000
        /// </summary>
        [ConfigurationProperty("audioBitRate", DefaultValue = 64000, IsRequired = false)]
        public int AudioBitRate
        {
            get { return ((int)base[audioBitRate]); }
			set { SetPropertyValue(value, audioBitRate, "AudioBitRate"); }
        }

        /// <summary>
        /// Gets or sets the audio frequency to use for audio.
        /// Popular frequencies are:
        /// 96000
        /// 48000
        /// 44100
        /// 22050
        /// </summary>
        [ConfigurationProperty("audioFrequency", DefaultValue = 44100, IsRequired = false)]
        public int AudioFrequency
        {
            get { return ((int)base[audioFrequency]); }
			set { SetPropertyValue(value, audioFrequency, "AudioFrequency"); }
        }

        /// <summary>
        /// Gets or sets the number of audio channels to use the audio.
        /// Popular values are
        /// 1 (Mono)
        /// 2 (Stero)
        /// </summary>
        [ConfigurationProperty("audioChannels", DefaultValue = 2, IsRequired = false)]
        public int AudioChannel
        {
            get { return ((int)base[audioChannels]); }
			set { SetPropertyValue(value, audioChannels, "AudioChannel"); }
        }

		/// <summary>
		/// Gets or sets the video conversion type to use.
		/// </summary>
		[ConfigurationProperty("videoConversionType", DefaultValue = VideoConversionType.NotSpecified)]
		public VideoConversionType VideoConversionType
		{
			get { return ((VideoConversionType)base[videoConversionType]); }
			set { SetPropertyValue(value, videoConversionType, "VideoConversionType"); }
		}

        /// <summary>
        /// Gets or sets if deinterlace should be used when encoding video.
        /// </summary>
        [ConfigurationProperty("deinterlace", DefaultValue = false, IsRequired = false)]
        public bool Deinterlace
        {
            get { return ((bool)base[deinterlace]); }
			set { SetPropertyValue(value, deinterlace, "Deinterlace"); }
        }

        /// <summary>
        /// Gets or sets the width of the video output.
        /// </summary>
        [ConfigurationProperty("width", DefaultValue = null, IsRequired = true)]
        public int Width
        {
            get { return ((int)base[width]); }
			set { SetPropertyValue(value, width, "Width"); }
        }

        /// <summary>
        /// Gets or sets the height of the video output.
        /// </summary>
        [ConfigurationProperty("height", DefaultValue = null, IsRequired = true)]
        public int Height
        {
            get { return ((int)base[height]); }
			set { SetPropertyValue(value, height, "Height"); }
        }

        /// <summary>
        /// Gets or sets the video bit rate of the video output.
        /// </summary>
        [ConfigurationProperty("videoBitRate", DefaultValue = null, IsRequired = true)]
        public int VideoBitRate
        {
            get { return ((int)base[videoBitRate]); }
			set { SetPropertyValue(value, videoBitRate, "VideoBitRate"); }
        }

        /// <summary>
        /// Gets or sets the frame rate of the video output.
        /// </summary>
        [ConfigurationProperty("frameRate", DefaultValue = null, IsRequired = true)]
        public int FrameRate
        {
            get { return ((int)base[frameRate]); }
			set { SetPropertyValue(value, frameRate, "FrameRate"); }
        }

        /// <summary>
        /// Gets or sets the video bit rate of the video output.
        /// </summary>
        [ConfigurationProperty("aspectRatio", DefaultValue = AspectRatio.NotSpecified, IsRequired = false)]
        public AspectRatio AspectRatio
        {
            get { return ((AspectRatio)base[aspectRatio]); }
			set { SetPropertyValue(value, aspectRatio, "AspectRatio"); }
        }

        /// <summary>
        /// Gets or sets the max video bit rate of the video output.
        /// Recommend setting MaxVideoBitRate > VideoBitRate
        /// </summary>
        [ConfigurationProperty("maxVideoBitRate", DefaultValue = null, IsRequired = false)]
        public int MaxVideoBitRate
        {
            get { return ((int)base[maxVideoBitRate]); }
			set { SetPropertyValue(value, maxVideoBitRate, "MaxVideoBitRate"); }
        }

        /// <summary>
        /// Gets or sets the buffer size of the video output.
        /// Recommend setting BufferSize > MaxVideoBitRate > VideoBitRate
        /// </summary>
        [ConfigurationProperty("bufferSize", DefaultValue = null, IsRequired = false)]
        public int BufferSize
        {
            get { return ((int)base[bufferSize]); }
			set { SetPropertyValue(value, bufferSize, "BufferSize"); }
        }

        /// <summary>
        /// Gets or sets the buffer size of the video output.
        /// Recommend setting KeyFrameInterval = 10 * FrameRate
        /// </summary>
        [ConfigurationProperty("keyFrameInterval", DefaultValue = null, IsRequired = false)]
        public int KeyFrameInterval
        {
            get { return ((int)base[keyFrameInterval]); }
			set { SetPropertyValue(value, keyFrameInterval, "KeyFrameInterval"); }
        }

        /// <summary>
        /// Gets or sets the buffer size of the video output.
        /// Recommend setting MinKeyFrameInterval = FrameRate
        /// </summary>
        [ConfigurationProperty("minKeyframeInterval", DefaultValue = null, IsRequired = false)]
        public int MinKeyFrameInterval
        {
            get { return ((int)base[minKeyFrameInterval]); }
			set { SetPropertyValue(value, minKeyFrameInterval, "MinKeyFrameInterval"); }
        }
    }
}
