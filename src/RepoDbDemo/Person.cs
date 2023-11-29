namespace RepoDbDemo
{
    public class Person
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime CreatedDateUtc { get; set; }
        public List<Address> Addresses { get; set; }
    }
}
