namespace RepoDbDemo
{
    public interface IPersonRepository
    {
        Task<long> Create(Person person);
        Task Update(Person person);
        Task Delete(long id);
        Task<List<Person>> FindAll();
        Task<Person?> FindById(long id);
        Task<Person> GetWithAddress(long id);
        Task CreatePersonWithAddress(Person person);
        Task BulkInsert(List<Person> persons);
    }
}
