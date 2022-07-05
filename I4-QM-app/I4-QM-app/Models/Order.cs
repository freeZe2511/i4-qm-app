using System;
using System.Collections.Generic;

namespace I4_QM_app.Models
{
    /// <summary>
    /// Order class.
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Gets or sets Unique Id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the associated userid who mixed it.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets total amount in grams.
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// Gets or sets weight of single item.
        /// </summary>
        public int Weight { get; set; }

        /// <summary>
        /// Gets or sets a list with the additives (ingredients).
        /// </summary>
        public List<Additive> Additives { get; set; }

        /// <summary>
        /// Gets or sets a status (enum).
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// Gets or sets a datetime when order is received.
        /// </summary>
        public DateTime Received { get; set; }

        /// <summary>
        /// Gets or sets a datetime when order is due.
        /// </summary>
        public DateTime Due { get; set; }

        /// <summary>
        /// Gets or sets a datetime when order is done.
        /// </summary>
        public DateTime Done { get; set; }

        /// <summary>
        /// Gets or sets a rating.
        /// </summary>
        public Rating Rating { get; set; }
    }

}