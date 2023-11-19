namespace LyricMeter.Models
{
    public class Relation
    {
        public string? Type { get; set; }
        public string? TypeId { get; set; }
        public string? Direction { get; set; }
        public Artist? Artist { get; set; }
        public Recording? Recording { get; set; }
    }
}
