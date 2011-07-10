using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Video
{
    /// <summary>
    /// Represents a configuration element within a configuration file that configures options for custom string formatting providers.
    /// </summary>
    public sealed class VideoConversionSettingElement : CommandConfigurationBase
    {
        private static readonly ConfigurationProperty videoConversionType = new ConfigurationProperty("videoConversionType", typeof(VideoConversionType), VideoConversionType.NotSpecified, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty audioBitRate = new ConfigurationProperty("audioBitRate", typeof(int), 64000, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty audioFrequency = new ConfigurationProperty("audioFrequency", typeof(int), 44100, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty audioChannels = new ConfigurationProperty("audioChannels", typeof(int), 2, ConfigurationPropertyOptions.None);

        private static readonly ConfigurationProperty deinterlace = new ConfigurationProperty("deinterlace", typeof(bool), false, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty width = new ConfigurationProperty("width", typeof(int), null, ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty height = new ConfigurationProperty("height", typeof(int), null, ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty videoBitRate = new ConfigurationProperty("videoBitRate", typeof(int), null, ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty frameRate = new ConfigurationProperty("frameRate", typeof(int), null, ConfigurationPropertyOptions.IsRequired);

        private static readonly ConfigurationProperty aspectRatio = new ConfigurationProperty("aspectRatio", typeof(AspectRatio), AspectRatio.NotSpecified, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty maxVideoBitRate = new ConfigurationProperty("maxVideoBitRate", typeof(int?), null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty bufferSize = new ConfigurationProperty("bufferSize", typeof(int?), null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty keyframeInterval = new ConfigurationProperty("keyframeInterval", typeof(int?), null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty minKeyframeInterval = new ConfigurationProperty("minKeyframeInterval", typeof(int?), null, ConfigurationPropertyOptions.None);

        /// <summary>
        /// Initializes the <see cref="VideoConversionSettingElement"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static VideoConversionSettingElement()
        {
            properties.Add(videoConversionType);
            properties.Add(audioBitRate);
            properties.Add(audioFrequency);
            properties.Add(audioChannels);

            properties.Add(deinterlace);
            properties.Add(width);
            properties.Add(height);
            properties.Add(videoBitRate);
            properties.Add(frameRate);
            properties.Add(aspectRatio);
            properties.Add(maxVideoBitRate);
            properties.Add(bufferSize);
            properties.Add(keyframeInterval);
            properties.Add(minKeyframeInterval);
        }

        /// <summary>
        /// Gets or sets the video conversion type to use.
        /// </summary>
        [ConfigurationProperty("videoConversionType", DefaultValue = VideoConversionType.NotSpecified)]
        public VideoConversionType VideoConversionType
        {
            get { return ((VideoConversionType)base[videoConversionType]); }
            set
            {
                if (value == VideoConversionType) return;

                base[videoConversionType] = value;
                OnPropertyChanged("VideoConversionType");
            }
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
        public int BitRate
        {
            get { return ((int)base[audioBitRate]); }
            set
            {
                if (value == BitRate) return;

                base[audioBitRate] = value;
                OnPropertyChanged("BitRate");
            }
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
        public int Frequency
        {
            get { return ((int)base[audioFrequency]); }
            set
            {
                if (value == Frequency) return;

                base[audioFrequency] = value;
                OnPropertyChanged("Frequency");
            }
        }

        /// <summary>
        /// Gets or sets the number of audio channels to use the audio.
        /// Popular values are
        /// 1 (Mono)
        /// 2 (Stero)
        /// </summary>
        [ConfigurationProperty("audioChannels", DefaultValue = 2, IsRequired = false)]
        public int Channel
        {
            get { return ((int)base[audioChannels]); }
            set
            {
                if (value == Channel) return;

                base[audioChannels] = value;
                OnPropertyChanged("Channel");
            }
        }

        /// <summary>
        /// Gets or sets if deinterlace should be used when encoding video.
        /// </summary>
        [ConfigurationProperty("deinterlace", DefaultValue = false, IsRequired = false)]
        public bool Deinterlace
        {
            get { return ((bool)base[deinterlace]); }
            set
            {
                if (value == Deinterlace) return;

                base[deinterlace] = value;
                OnPropertyChanged("Deinterlace");
            }
        }

        /// <summary>
        /// Gets or sets the width of the video output.
        /// </summary>
        [ConfigurationProperty("width", DefaultValue = null, IsRequired = true)]
        public int Width
        {
            get { return ((int)base[width]); }
            set
            {
                if (value == Width) return;

                base[width] = value;
                OnPropertyChanged("Width");
            }
        }

        /// <summary>
        /// Gets or sets the height of the video output.
        /// </summary>
        [ConfigurationProperty("height", DefaultValue = null, IsRequired = true)]
        public int Height
        {
            get { return ((int)base[height]); }
            set
            {
                if (value == Height) return;

                base[height] = value;
                OnPropertyChanged("Height");
            }
        }

        /// <summary>
        /// Gets or sets the video bit rate of the video output.
        /// </summary>
        [ConfigurationProperty("videoBitRate", DefaultValue = null, IsRequired = true)]
        public int VideoBitRate
        {
            get { return ((int)base[videoBitRate]); }
            set
            {
                if (value == VideoBitRate) return;

                base[videoBitRate] = value;
                OnPropertyChanged("VideoBitRate");
            }
        }

        /// <summary>
        /// Gets or sets the frame rate of the video output.
        /// </summary>
        [ConfigurationProperty("frameRate", DefaultValue = null, IsRequired = true)]
        public int FrameRate
        {
            get { return ((int)base[frameRate]); }
            set
            {
                if (value == FrameRate) return;

                base[frameRate] = value;
                OnPropertyChanged("FrameRate");
            }
        }

        /// <summary>
        /// Gets or sets the video bit rate of the video output.
        /// </summary>
        [ConfigurationProperty("aspectRatio", DefaultValue = AspectRatio.NotSpecified, IsRequired = false)]
        public AspectRatio AspectRatio
        {
            get { return ((AspectRatio)base[aspectRatio]); }
            set
            {
                if (value == AspectRatio) return;

                base[aspectRatio] = value;
                OnPropertyChanged("AspectRatio");
            }
        }

        /// <summary>
        /// Gets or sets the max video bit rate of the video output.
        /// Recommend setting MaxVideoBitRate > VideoBitRate
        /// </summary>
        [ConfigurationProperty("maxVideoBitRate", DefaultValue = null, IsRequired = false)]
        public int? MaxVideoBitRate
        {
            get { return ((int?)base[maxVideoBitRate]); }
            set
            {
                if (value == MaxVideoBitRate) return;

                base[maxVideoBitRate] = value;
                OnPropertyChanged("MaxVideoBitRate");
            }
        }

        /// <summary>
        /// Gets or sets the buffer size of the video output.
        /// Recommend setting BufferSize > MaxVideoBitRate > VideoBitRate
        /// </summary>
        [ConfigurationProperty("bufferSize", DefaultValue = null, IsRequired = false)]
        public int? BufferSize
        {
            get { return ((int?)base[bufferSize]); }
            set
            {
                if (value == BufferSize) return;

                base[bufferSize] = value;
                OnPropertyChanged("BufferSize");
            }
        }

        /// <summary>
        /// Gets or sets the buffer size of the video output.
        /// Recommend setting KeyframeInterval = 10 * FrameRate
        /// </summary>
        [ConfigurationProperty("keyframeInterval", DefaultValue = null, IsRequired = false)]
        public int? KeyframeInterval
        {
            get { return ((int?)base[keyframeInterval]); }
            set
            {
                if (value == KeyframeInterval) return;

                base[keyframeInterval] = value;
                OnPropertyChanged("KeyframeInterval");
            }
        }

        /// <summary>
        /// Gets or sets the buffer size of the video output.
        /// Recommend setting MinKeyframeInterval = FrameRate
        /// </summary>
        [ConfigurationProperty("minKeyframeInterval", DefaultValue = null, IsRequired = false)]
        public int? MinKeyframeInterval
        {
            get { return ((int?)base[minKeyframeInterval]); }
            set
            {
                if (value == MinKeyframeInterval) return;

                base[minKeyframeInterval] = value;
                OnPropertyChanged("MinKeyframeInterval");
            }
        }
    }
}
