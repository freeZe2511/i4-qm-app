using System;
using System.Collections.Generic;
using System.Text;

namespace I4_QM_app.Models
{
    public class User
    {
        public string UID { get; set; }

        public User(string id)
        {
            UID = id;
        }
    }
}
