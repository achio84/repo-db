namespace RepoDbDemo
{
    public class PersonViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public List<AddressViewModel> Addresses { get; set; } = new List<AddressViewModel>();
    }

    public class AddressViewModel
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string PostCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
    }
}
