namespace I4_QM_app.Models
{
    /// <summary>
    /// Status enum.
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// open, order not mixed
        /// </summary>
        Open,

        /// <summary>
        /// mixed, ready for molding
        /// </summary>
        Mixed,

        /// <summary>
        /// rated with feedback after quality inspection
        /// </summary>
        Rated,
    }
}
