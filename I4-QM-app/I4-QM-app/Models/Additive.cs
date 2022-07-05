using System.Text.Json.Serialization;
using Xamarin.Forms;

namespace I4_QM_app.Models
{
    /// <summary>
    /// Additive class.
    /// </summary>
    // TODO AdditiveData + AdditiveView class?
    public class Additive
    {
        /// <summary>
        /// Gets or sets Unique Id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Target percentage in relation to total.
        /// </summary>
        public double Portion { get; set; }

        /// <summary>
        /// Gets or sets Actual percentage after mixing.
        /// </summary>
        public double ActualPortion { get; set; }

        /// <summary>
        /// Gets or sets weighted amount.
        /// </summary>
        public double Amount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether additive is available (in store).
        /// </summary>
        [JsonIgnore]
        public bool Available { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether additive is checked in to-do list.
        /// </summary>
        [JsonIgnore]
        public bool Checked { get; set; }

        /// <summary>
        /// Gets or sets an image through base64 encoding.
        /// </summary>
        [JsonIgnore]
        public string ImageBase64 { get; set; }

        /// <summary>
        /// Gets or sets the actual presentable image source.
        /// </summary>
        [JsonIgnore]
        public ImageSource Image { get; set; }

    }
}
