namespace Botreon.Models.Patreon
{
    public class PatronStatus
    {
        public bool IsPatron { get; set; }
        public int Tier { get; set; }
        public string Error { get; set; }
    }
}