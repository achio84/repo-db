using Microsoft.Data.SqlClient;
using RepoDb;
using RepoDb.Extensions;
using System.Data;

namespace RepoDbDemo
{
    public class PersonRepository : DbRepository<SqlConnection>, IPersonRepository
    {
        public PersonRepository(IConfiguration configuration) : base(configuration.GetConnectionString("DemoConnection"))
        {
        }

        #region Normal CRUD
        /// <summary>
        /// Utilizing base repository from RepoDb
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        public async Task<long> Create(Person person)
        {
            var result = await base.InsertAsync<Person, long>(person);
            return result;
        }

        public async Task Update(Person person)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var result = await connection.UpdateAsync(person);
            }
        }

        /// <summary>
        /// Raw SQL
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        public async Task Delete(long id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var sql = "DELETE Person WHERE Id = @Id;";
                var rowCount = await connection.ExecuteNonQueryAsync(sql, new { Id = id });
            }
        }

        /// <summary>
        /// NOLOCK hints
        /// </summary>
        /// <returns></returns>
        public async Task<List<Person>> FindAll()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var result = await connection.QueryAllAsync<Person>(hints: SqlServerTableHints.NoLock);
                return result.ToList();
            }
        }

        /// <summary>
        /// Stored procedure
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Person?> FindById(long id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var sql = "spGetUserById";
                var result = await connection.ExecuteQueryAsync<Person>(sql, new { Id = id }, commandType: CommandType.StoredProcedure);

                return result.FirstOrDefault();
            }
        }

        #endregion

        #region Multiple Resultset
        public async Task<Person> GetWithAddress(long id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var result = await connection.QueryMultipleAsync<Person, Address>(
                    p => p.Id == id, 
                    a => a.PersonId == id, 
                    hints1: SqlServerTableHints.NoLock, 
                    hints2: SqlServerTableHints.NoLock);

                var person = result.Item1.FirstOrDefault();
                var addresses = result.Item2.AsList();
                if (person is not null)
                {
                    person.Addresses = addresses;
                }

                return person;
            }
        }
        #endregion

        #region Atomic Transaction

        public async Task CreatePersonWithAddress(Person person)
        {
            using(var connection  =new SqlConnection(ConnectionString))
            {
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    var personId = await connection.InsertAsync<Person, long>(person, transaction: transaction);
                    person.Addresses.ForEach(a =>
                    {
                        a.PersonId = personId;
                    });
                    var rowCount = await connection.InsertAllAsync(person.Addresses, transaction: transaction);

                    transaction.Commit();
                }
            }
        }
        #endregion

        #region BulkInsert

        /// <summary>
        /// Bulk Insert with Batch Size
        /// </summary>
        /// <param name="persons"></param>
        /// <returns></returns>
        public async Task BulkInsert(List<Person> persons)
        {
            using(var connection = new SqlConnection(ConnectionString))
            {
                var rowsCount = await connection.BulkInsertAsync(persons, batchSize: 1000);
            }
        }
        #endregion
    }
}
