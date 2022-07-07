using System.Collections.Generic;

namespace I4_QM_app.Models
{
    /// <summary>
    /// Recipe class to store certain additive mixes.
    /// </summary>
    public class Recipe
    {
        /// <summary>
        /// Gets or sets Unique Id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets creatorId from logged in user.
        /// </summary>
        public string CreatorId { get; set; }

        /// <summary>
        /// Gets or sets recipe name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets recipe description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets list of additives (ingredients).
        /// </summary>
        public List<Additive> Additives { get; set; }

        /// <summary>
        /// Gets or sets times used.
        /// </summary>
        public int Used { get; set; }

    }
}
