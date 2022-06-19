﻿using System.Collections.Generic;

namespace I4_QM_app.Models
{
    public class Recipe
    {
        public string Id { get; set; }

        public string CreatorId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public List<Additive> Additives { get; set; }

        public int Used { get; set; }

    }
}
