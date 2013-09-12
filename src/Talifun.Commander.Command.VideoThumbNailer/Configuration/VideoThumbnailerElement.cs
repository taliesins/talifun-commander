using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.VideoThumbnailer.Configuration
{
    /// <summary>
    /// Represents a configuration element within a configuration file that configures options for custom string formatting providers.
    /// </summary>
	[JsonObject(MemberSerialization.OptIn)]
    public sealed partial class VideoThumbnailerElement : CommandConfigurationBase
    {
        private static readonly ConfigurationProperty imageType = new ConfigurationProperty("imageType", typeof(ImageType), ImageType.JPG, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty width = new ConfigurationProperty("width", typeof(int), null, ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty height = new ConfigurationProperty("height", typeof(int), null, ConfigurationPropertyOptions.IsRequired);

        private static readonly ConfigurationProperty time = new ConfigurationProperty("time", typeof(TimeSpan), TimeSpan.Zero, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty timePercentage = new ConfigurationProperty("timePercentage", typeof(int), int.MinValue, ConfigurationPropertyOptions.None);

        /// <summary>
        /// Initializes the <see cref="VideoThumbnailerElement"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static VideoThumbnailerElement()
        {
            properties.Add(imageType);
            properties.Add(width);
            properties.Add(height);

            properties.Add(time);
            properties.Add(timePercentage);
        }

		public VideoThumbnailerElement()
		{
			Setting = VideoThumbnailerConfiguration.Instance;	
		}

        /// <summary>
        /// Gets or sets the image output type to use for the thumbnail.
        /// </summary>
        [ConfigurationProperty("imageType", DefaultValue = ImageType.JPG, IsRequired = false)]
		[JsonProperty]
        public ImageType ImageType
        {
            get { return ((ImageType)base[imageType]); }
			set { SetPropertyValue(value, imageType, "ImageType"); }
        }

        /// <summary>
        /// Gets or sets the image width for the thumbnail.
        /// </summary>
        [ConfigurationProperty("width", DefaultValue = null, IsRequired = true)]
		[JsonProperty]
        public int Width
        {
            get { return ((int)base[width]); }
			set { SetPropertyValue(value, width, "Width"); }
        }

        /// <summary>
        /// Gets or sets the image height for the thumbnail.
        /// </summary>
        [ConfigurationProperty("height", DefaultValue = null, IsRequired = true)]
		[JsonProperty]
        public int Height
        {
            get { return ((int)base[height]); }
			set { SetPropertyValue(value, height, "Height"); }
        }

        /// <summary>
        /// Gets or sets the time into the video the thumbnail should be taken. Rather use 
        /// percentage where possible.
        /// </summary>
        [ConfigurationProperty("time", IsRequired = false)]
		[JsonProperty]
        public TimeSpan Time
        {
            get { return ((TimeSpan)base[time]); }
			set { SetPropertyValue(value, time, "TimeSpan"); }
        }

        /// <summary>
        /// Gets or sets the percentage into the video the thumbnail should be taken.
        /// </summary>
        [ConfigurationProperty("timePercentage", IsRequired = false)]
		[JsonProperty]
        public int TimePercentage
        {
            get { return ((int)base[timePercentage]); }
			set { SetPropertyValue(value, timePercentage, "TimePercentage"); }
        }
    }
}
