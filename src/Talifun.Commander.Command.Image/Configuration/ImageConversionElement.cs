﻿using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Image.Configuration
{
    /// <summary>
    /// Represents a configuration element within a configuration file that configures options for custom string formatting providers.
    /// </summary>
	[JsonObject(MemberSerialization.OptIn)]
    public sealed partial class ImageConversionElement : CommandConfigurationBase
    {
        private static readonly ConfigurationProperty width = new ConfigurationProperty("width", typeof(int), 0, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty height = new ConfigurationProperty("height", typeof(int), 0, ConfigurationPropertyOptions.None);

        private static readonly ConfigurationProperty resizeMode = new ConfigurationProperty("resizeMode", typeof(ResizeMode), ResizeMode.None, ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty gravity = new ConfigurationProperty("gravity", typeof(Gravity), Gravity.Center, ConfigurationPropertyOptions.None);
		private static readonly ConfigurationProperty backgroundColor = new ConfigurationProperty("backgroundColor", typeof(string), "#00FFFFFF", ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty quality = new ConfigurationProperty("quality", typeof(int), 0, ConfigurationPropertyOptions.None);
        
        private static readonly ConfigurationProperty resizeImageType = new ConfigurationProperty("resizeImageType", typeof(ResizeImageType), ResizeImageType.Original, ConfigurationPropertyOptions.None);

		private static readonly ConfigurationProperty watermarkPath = new ConfigurationProperty("watermarkPath", typeof(string), "", ConfigurationPropertyOptions.None);
		private static readonly ConfigurationProperty watermarkGravity = new ConfigurationProperty("watermarkGravity", typeof(Gravity), Gravity.SouthEast, ConfigurationPropertyOptions.None);
		private static readonly ConfigurationProperty watermarkDissolveLevels = new ConfigurationProperty("watermarkDissolveLevels", typeof(int), 15, ConfigurationPropertyOptions.None);

        /// <summary>
        /// Initializes the <see cref="ImageConversionElement"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static ImageConversionElement()
        {
            properties.Add(width);
            properties.Add(height);

            properties.Add(resizeMode);
            properties.Add(gravity);
            properties.Add(backgroundColor);
            properties.Add(quality);

            properties.Add(resizeImageType);

			properties.Add(watermarkPath);
			properties.Add(watermarkGravity);
			properties.Add(watermarkDissolveLevels);
        }

		public ImageConversionElement()
		{
			Setting = ImageConversionConfiguration.Instance;	
		}

        /// <summary>
        /// Gets or sets the image width, in pixels, to generate when using this photo style.
        /// Images 
        /// </summary>
        [ConfigurationProperty("width", DefaultValue = null, IsRequired = false)]
		[JsonProperty]
        public int Width
        {
            get { return ((int)base[width]); }
			set { SetPropertyValue(value, width, "Width"); }
        }

        /// <summary>
        /// Gets or sets the image height, in pixels, to generate when using this photo style.
        /// </summary>
        [ConfigurationProperty("height", DefaultValue = null, IsRequired = false)]
		[JsonProperty]
        public int Height
        {
            get { return ((int)base[height]); }
			set { SetPropertyValue(value, height, "Height"); }
        }

        /// <summary>
        /// Gets or sets the ResizeMode.
        /// </summary>
        /// <value>The name.</value>
        [ConfigurationProperty("resizeMode", DefaultValue = ResizeMode.None, IsRequired = true)]
		[JsonProperty]
        public ResizeMode ResizeMode
        {
            get { return ((ResizeMode)base[resizeMode]); }
			set { SetPropertyValue(value, resizeMode, "ResizeMode"); }
        }

        /// <summary>
        /// Gets or sets the Gravity.
        /// </summary>
        [ConfigurationProperty("gravity", DefaultValue = Gravity.Center, IsRequired = false)]
		[JsonProperty]
        public Gravity Gravity
        {
            get { return ((Gravity)base[gravity]); }
			set { SetPropertyValue(value, gravity, "Gravity"); }
        }

        /// <summary>
        /// Gets or sets a string specifying the background color in hexadecimal for padding images
        /// that do not completely fill the canvas when using this photo style. The empty string will
        /// result in images with a transparent background if this is supported by the output format.
        /// </summary>
		[ConfigurationProperty("backgroundColor", DefaultValue = "#00FFFFFF", IsRequired = false)]
		[JsonProperty]
        public string BackgroundColor
        {
            get { return ((string)base[backgroundColor]); }
			set { SetPropertyValue(value, backgroundColor, "BackgroundColor"); }
        }

        /// <summary>
        /// Gets or sets the image quality, as a percentage (0-100), of images returned in this photo style when 
        /// using an image format that supports lossy compression (such as JPG or PNG)
        /// </summary>
        [ConfigurationProperty("quality", DefaultValue = 0, IsRequired = false)]
		[JsonProperty]
        public int Quality
        {
            get { return ((int)base[quality]); }
			set { SetPropertyValue(value, quality, "Quality"); }
        }

        /// <summary>
        /// Gets or sets the image format to convert to.
        /// </summary>
        /// <remarks>
        /// Original uses the source format for its output format
        /// </remarks>
        [ConfigurationProperty("resizeImageType", DefaultValue = ResizeImageType.Original, IsRequired = false)]
		[JsonProperty]
        public ResizeImageType ResizeImageType
        {
            get { return ((ResizeImageType)base[resizeImageType]); }
			set { SetPropertyValue(value, resizeImageType, "ResizeImageType"); }
        }

		/// <summary>
		/// Gets or sets the path where the watermark image to use is. If none is specified then image will not be
		/// watermarked.
		/// </summary>       
		[ConfigurationProperty("watermarkPath", DefaultValue = "", IsRequired = false)]
		[JsonProperty]
		public string WatermarkPath
		{
			get { return ((string)base[watermarkPath]); }
			set { SetPropertyValue(value, watermarkPath, "WatermarkPath"); }
		}

		/// <summary>
		/// Gets or sets the gravity to use when placing watermark overlay.
		/// </summary>
		[ConfigurationProperty("watermarkGravity", DefaultValue = Gravity.SouthEast, IsRequired = false)]
		[JsonProperty]
		public Gravity WatermarkGravity
		{
			get { return ((Gravity)base[watermarkGravity]); }
			set { SetPropertyValue(value, watermarkGravity, "WatermarkGravity"); }
		}

		/// <summary>
		/// Gets or sets the dissolve levels to use when placing watermark overlay.
		/// </summary>
		[ConfigurationProperty("watermarkDissolveLevels", DefaultValue = 15, IsRequired = false)]
		[JsonProperty]
		public int WatermarkDissolveLevels
		{
			get { return ((int)base[watermarkDissolveLevels]); }
			set { SetPropertyValue(value, watermarkDissolveLevels, "WatermarkDissolveLevels"); }
		}
    }
}