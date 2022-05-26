using System;
using System.Collections.Generic;

namespace I4_QM_app.Models
{
    public class Item
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
    }

    public class Order
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public int Amount { get; set; }

        public List<Additive> Additives { get; set; }

        public Status Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime Due { get; set; }
    }

    public class Additive
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int Amount { get; set; }
    }

    public enum Status
    {
        test,
        waiting,
        ready
    }


}