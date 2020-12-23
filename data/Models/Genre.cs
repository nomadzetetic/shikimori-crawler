using System.Collections.Generic;

namespace Shikimori.Data.Models
{
    public class Genre
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public ICollection<Video> Videos { get; set; }
    }
}
