using System;
using Newtonsoft.Json;
namespace OLinqTests
{
    public class Entry
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Synopsis { get; set; }
        public string ShortSynopsis { get; set; }
        public double? AverageRating { get; set; }
        public DateTime DateModified { get; set; }
        public int? ReleaseYear { get; set; }
    }
}
