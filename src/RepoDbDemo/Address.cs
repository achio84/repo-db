namespace RepoDbDemo
{
    public class Address
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public string Address1  { get; set; }
        public string Address2 { get; set; }
        public string PostCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public long PersonId { get; set; }
    }
}
