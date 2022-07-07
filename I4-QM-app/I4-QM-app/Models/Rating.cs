namespace I4_QM_app.Models
{
    /// <summary>
    /// Rating class to rate an order.
    /// </summary>
    public class Rating
    {
        /// <summary>
        /// Gets or sets Unique Rating Id.
        /// </summary>
        public string RatingId { get; set; }

        /// <summary>
        /// Gets or sets Feedback.
        /// </summary>
        public string Feedback { get; set; }

        /// <summary>
        /// Gets or sets a Form rating.
        /// </summary>
        public int Form { get; set; }

        /// <summary>
        /// Gets or sets a Color rating.
        /// </summary>
        public int Color { get; set; }

        /// <summary>
        /// Gets or sets a Ridge rating.
        /// </summary>
        public int Ridge { get; set; }

        /// <summary>
        /// Gets or sets a Surface rating.
        /// </summary>
        public int Surface { get; set; }

        /// <summary>
        /// Gets or sets a Bindings rating.
        /// </summary>
        public int Bindings { get; set; }

        /// <summary>
        /// Gets or sets a Sprue rating.
        /// </summary>
        public int Sprue { get; set; }

        /// <summary>
        /// Gets or sets a Drop Ins rating.
        /// </summary>
        public int DropIn { get; set; }

        /// <summary>
        /// Gets or sets a Demolding rating.
        /// </summary>
        public int Demolding { get; set; }

        /// <summary>
        /// Gets or sets a Air Inclusions rating.
        /// </summary>
        public int AirInclusion { get; set; }

        /// <summary>
        /// Gets or sets an Overall rating.
        /// </summary>
        public int Overall { get; set; }

    }
}
