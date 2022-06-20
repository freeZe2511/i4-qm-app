using System.Text.Json.Serialization;

namespace I4_QM_app.Models
{
    public class Additive
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public float Portion { get; set; }

        public float ActualPortion { get; set; }

        public int Amount { get; set; }

        [JsonIgnore]
        public bool Checked { get; set; }

        [JsonIgnore]
        public string ImageId { get; set; }

    }
}
