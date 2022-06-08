using System;
using System.Collections.Generic;

namespace I4_QM_app.Models
{
    public class Order
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public int Amount { get; set; }

        public int Weight { get; set; }

        public List<Additive> Additives { get; set; }

        public Status Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Due { get; set; }

        public DateTime Done { get; set; }
    }

    public enum Status
    {
        open,
        done
        //maybe ready for qm review?
    }


}