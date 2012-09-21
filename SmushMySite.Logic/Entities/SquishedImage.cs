namespace SmushMySite.Logic.Entities
{
    /// <summary>
    /// The object that we use to deserialize 
    /// the Json response to.
    /// </summary>
    public class SquishedImage
    {
        public string src { get; set; }
        public string dest { get; set; }
        public string dest_size { get; set; }
        public string percent { get; set; }
        public string id { get; set; }
        public string src_size { get; set; }
        public string error { get; set; }
    }
}
