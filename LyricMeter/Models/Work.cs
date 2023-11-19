namespace LyricMeter.Models
{
    public class Work
    {
        public string? Id { get; set; }
        public string? Type { get; set; }
        public int Score { get; set; }
        public string? Title { get; set; }
        public string? Language { get; set; }
        public List<string>? Iswcs { get; set; }
        public List<Relation>? Relations { get; set; }
        public List<string>? Languages { get; set; }
    }
}
