using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.VideoThumbnailer;

namespace Talifun.Commander.Command.VideoThumbNailer
{
    /// <summary>
    /// Represents a configuration element within a configuration file that configures options for custom string formatting providers.
    /// </summary>
    public class VideoThumbnailerSettingElement : CommandConfigurationBase
    {
        private static readonly ConfigurationProperty imageType = new ConfigurationProperty("imageType", typeof(ImageType), ImageType.JPG, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty width = new ConfigurationProperty("width", typeof(int), null, ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty height = new ConfigurationProperty("height", typeof(int), null, ConfigurationPropertyOptions.IsRequired);

        private static readonly ConfigurationProperty time = new ConfigurationProperty("time", typeof(TimeSpan), TimeSpan.Zero, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty timePercentage = new ConfigurationProperty("timePercentage", typeof(int), int.MinValue, ConfigurationPropertyOptions.None);

        /// <summary>
        /// Initializes the <see cref="VideoThumbnailerSettingElement"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static VideoThumbnailerSettingElement()
        {
            properties.Add(imageType);
            properties.Add(width);
            properties.Add(height);

            properties.Add(time);
            properties.Add(timePercentage);
        }

        /// <summary>
        /// Gets or sets the image output type to use for the thumbnail.
        /// </summary>
        [ConfigurationProperty("imageType", DefaultValue = ImageType.JPG, IsRequired = false)]
        public ImageType ImageType
        {
            get { return ((ImageType)base[imageType]); }
            set
            {
                if (value == ImageType) return;

                base[imageType] = value;
                OnPropertyChanged("ImageType");
            }
        }

        /// <summary>
        /// Gets or sets the image width for the thumbnail.
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
        /// Gets or sets the image height for the thumbnail.
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
        /// Gets or sets the time into the video the thumbnail should be taken. Rather use 
        /// percentage where possible.
        /// </summary>
        [ConfigurationProperty("time", IsRequired = false)]
        public TimeSpan Time
        {
            get { return ((TimeSpan)base[time]); }
            set
            {
                if (value == Time) return;

                base[time] = value;
                OnPropertyChanged("Time");
            }
        }

        /// <summary>
        /// Gets or sets the percentage into the video the thumbnail should be taken.
        /// </summary>
        [ConfigurationProperty("timePercentage", IsRequired = false)]
        public int TimePercentage
        {
            get { return ((int)base[timePercentage]); }
            set
            {
                if (value == TimePercentage) return;

                base[timePercentage] = value;
                OnPropertyChanged("TimePercentage");
            }
        }
    }
}
