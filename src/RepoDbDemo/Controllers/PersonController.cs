using Microsoft.AspNetCore.Mvc;

namespace RepoDbDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonController : ControllerBase
    {

        private readonly ILogger<PersonController> _logger;
        private readonly IPersonRepository _personRepository;

        public PersonController(ILogger<PersonController> logger, IPersonRepository personRepository)
        {
            _logger = logger;
            _personRepository = personRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var results = await _personRepository.FindAll();

            return Ok(results);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var person = await _personRepository.FindById(id);

            return Ok(person);
        }

        [HttpGet("get-with-address/{id:long}")]
        public async Task<IActionResult> GetByIdWithAddress(long id)
        {
            var person = await _personRepository.GetWithAddress(id);

            return Ok(person);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PersonViewModel vm)
        {
            var person = new Person() { Name = vm.Name, Age = vm.Age, CreatedDateUtc = DateTime.UtcNow };
            await _personRepository.Create(person);
            return Ok();
        }

        [HttpPost("create-with-address")]
        public async Task<IActionResult> CreateWithAddress([FromBody] PersonViewModel vm)
        {
            var person = new Person() 
            { 
                Name = vm.Name, 
                Age = vm.Age, 
                CreatedDateUtc = DateTime.UtcNow,
                Addresses = vm.Addresses.Select(a => new Address
                {
                    Address1 = a.Address1,
                    Address2 = a.Address2,
                    PostCode = a.PostCode,
                    City = a.City,
                    State = a.State,
                    Type = a.Type
                }).ToList()
            };
            await _personRepository.CreatePersonWithAddress(person);
            return Ok();
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> BulkInsert([FromBody] List<PersonViewModel> vm)
        {
            var persons = vm.Select(p => new Person
            {
                Age = p.Age,
                Name = p.Name,
                CreatedDateUtc = DateTime.UtcNow,
                Addresses = p.Addresses.Select(a => new Address
                {
                    Address1 = a.Address1,
                    Address2 = a.Address2,
                    PostCode = a.PostCode,
                    City = a.City,
                    State = a.State,
                    Type = a.Type
                }).ToList()
            }).ToList();
            await _personRepository.BulkInsert(persons);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] PersonViewModel vm)
        {
            var person = await _personRepository.FindById(vm.Id);
            if (person is not null)
            {
                person.Name = vm.Name;
                person.Age = vm.Age;
                await _personRepository.Update(person);
                return Ok();
            }
            return NotFound();
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var person = await _personRepository.FindById(id);
            if (person is not null)
            {
                await _personRepository.Delete(id);

                return Ok();
            }
            return NotFound();
        }
    }
}