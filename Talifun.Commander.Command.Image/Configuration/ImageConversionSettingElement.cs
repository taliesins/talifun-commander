using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace Talifun.Commander.Command.Image
{
    /// <summary>
    /// Represents a configuration element within a configuration file that configures options for custom string formatting providers.
    /// </summary>
    public sealed class ImageConversionSettingElement : CommandConfigurationBase
    {
        private static readonly ConfigurationProperty width = new ConfigurationProperty("width", typeof(int?), null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty height = new ConfigurationProperty("height", typeof(int?), null, ConfigurationPropertyOptions.None);

        private static readonly ConfigurationProperty resizeMode = new ConfigurationProperty("resizeMode", typeof(ResizeMode), ResizeMode.None, ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty gravity = new ConfigurationProperty("gravity", typeof(Gravity), Gravity.Center, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty backgroundColor = new ConfigurationProperty("backgroundColor", typeof(string), "", ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty quality = new ConfigurationProperty("quality", typeof(int?), null, ConfigurationPropertyOptions.None);
        
        private static readonly ConfigurationProperty resizeImageType = new ConfigurationProperty("resizeImageType", typeof(ResizeImageType), ResizeImageType.Orginal, ConfigurationPropertyOptions.None);

        /// <summary>
        /// Initializes the <see cref="ImageConversionSettingElement"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static ImageConversionSettingElement()
        {
            properties.Add(width);
            properties.Add(height);

            properties.Add(resizeMode);
            properties.Add(gravity);
            properties.Add(backgroundColor);
            properties.Add(quality);

            properties.Add(resizeImageType);
        }

        /// <summary>
        /// Gets or sets the image width, in pixels, to generate when using this photo style.
        /// Images 
        /// </summary>
        [ConfigurationProperty("width", DefaultValue = null, IsRequired = false)]
        public int? Width
        {
            get { return ((int?)base[width]); }
            set { base[width] = value; }
        }

        /// <summary>
        /// Gets or sets the image height, in pixels, to generate when using this photo style.
        /// </summary>
        [ConfigurationProperty("height", DefaultValue = null, IsRequired = false)]
        public int? Height
        {
            get { return ((int?)base[height]); }
            set { base[height] = value; }
        }

        /// <summary>
        /// Gets or sets the ResizeMode.
        /// </summary>
        /// <value>The name.</value>
        [ConfigurationProperty("resizeMode", DefaultValue = ResizeMode.None, IsRequired = true)]
        public ResizeMode ResizeMode
        {
            get { return ((ResizeMode)base[resizeMode]); }
            set { base[resizeMode] = value; }
        }

        /// <summary>
        /// Gets or sets the Gravity.
        /// </summary>
        [ConfigurationProperty("gravity", DefaultValue = Gravity.Center, IsRequired = false)]
        public Gravity Gravity
        {
            get { return ((Gravity)base[gravity]); }
            set { base[gravity] = value; }
        }

        /// <summary>
        /// Gets or sets a string specifying the background color in hexadecimal for padding images
        /// that do not completely fill the canvas when using this photo style. The empty string will
        /// result in images with a transparent background if this is supported by the output format.
        /// </summary>
        [ConfigurationProperty("backgroundColor", DefaultValue = "", IsRequired = false)]
        public string BackgroundColor
        {
            get { return ((string)base[backgroundColor]); }
            set { base[backgroundColor] = value; }
        }

        /// <summary>
        /// Gets or sets the image quality, as a percentage (0-100), of images returned in this photo style when 
        /// using an image format that supports lossy compression (such as JPG or PNG)
        /// </summary>
        [ConfigurationProperty("quality", DefaultValue = null, IsRequired = false)]
        public int? Quality
        {
            get { return ((int?)base[quality]); }
            set { base[quality] = value; }
        }

        /// <summary>
        /// Gets or sets the image format to convert to.
        /// </summary>
        /// <remarks>
        /// Orginal uses the source format for its output format
        /// </remarks>
        [ConfigurationProperty("resizeImageType", DefaultValue = ResizeImageType.Orginal, IsRequired = false)]
        public ResizeImageType ResizeImageType
        {
            get { return ((ResizeImageType)base[resizeImageType]); }
            set { base[resizeImageType] = value; }
        }
    }
}