namespace I4_QM_app.Models
{
    /// <summary>
    /// User class to represent logged in user.
    /// </summary>
    // TODO extend
    public class User
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="id">unique id.</param>
        public User(string id)
        {
            UID = id;
        }

        /// <summary>
        /// Gets or sets unique id.
        /// </summary>
        public string UID { get; set; }
    }
}
