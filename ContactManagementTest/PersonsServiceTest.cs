using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service_Contracts;
using Service_Classes;
using Entities_DTO;
using Entities_Core;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCoreMock;
using AutoFixture;
using Repository_Contracts;
using Moq;
using System.Linq.Expressions;
using Serilog;
using Microsoft.Extensions.Logging;
using Exceptions;

namespace ContactManagementTest
{
    public class PersonsServiceTest
    {
        private readonly ICountriesGetterService _countriesGetterService;

        //private readonly ApplicationDbContext _dbContext;

        private readonly IFixture _fixture;

        private readonly Mock<IPersonsRepository> _personsRepositoryMock;

        private readonly IPersonsRepository _personsRepository;

        private readonly Mock<ICountriesRepository> _countriesRepositoryMock;

        private readonly ICountriesRepository _countriesRepository;

        private readonly Mock<IDiagnosticContext> _diagnosticContextMock;

        private readonly IDiagnosticContext _diagnosticContext;

        private readonly IPersonsAdderService _personsAdderService;

        private readonly IPersonsDeleterService _personsDeleterService;

        private readonly IPersonsGetterService _personsGetterService;

        private readonly IPersonsSorterService _personsSorterService;

        private readonly IPersonsUpdaterService _personsUpdaterService;

    
        public PersonsServiceTest()
        {
            //// Mock the DbContext
            //DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
            //        new DbContextOptionsBuilder<ApplicationDbContext>().Options
            //    );

            //// seed data
            //List<Person> people = new List<Person>();
            //List<Country> countries = new List<Country>();

            //// Mock the DbSet
            //dbContextMock.CreateDbSetMock(x => x.Persons, people);
            //dbContextMock.CreateDbSetMock(x => x.Countries, countries);

            //// get the mocked DbContext Object
            //_dbContext = dbContextMock.Object;

            // Mock the Countries repository
            _countriesRepositoryMock = new Mock<ICountriesRepository>();
            _countriesRepository = _countriesRepositoryMock.Object;

            // Mock the Persons repository
            _personsRepositoryMock = new Mock<IPersonsRepository>();
            _personsRepository = _personsRepositoryMock.Object;


            // Mock the DiagnosticContext
            _diagnosticContextMock = new Mock<IDiagnosticContext>();
            _diagnosticContext = _diagnosticContextMock.Object;

            // Mock the Logger
            var _loggerMock = new Mock<ILogger<IPersonsGetterService>>();
            var _logger = _loggerMock.Object;

            var _loggerSorterMock = new Mock<ILogger<IPersonsSorterService>>();
            var _loggerSorter = _loggerSorterMock.Object;

            _countriesGetterService = new CountriesGetterService(_countriesRepository);

            _personsAdderService = new PersonsAdderService(_personsRepository, _countriesGetterService);
            _personsDeleterService = new PersonsDeleteService(_personsRepository);
            _personsGetterService = new PersonsGetterService(_personsRepository, _logger, _diagnosticContext);
            _personsSorterService = new PersonsSorterService(_loggerSorter);
            _personsUpdaterService = new PersonsUpdaterService(_personsRepository, _countriesGetterService);

            _fixture = new Fixture();
        }

        #region Add Person

        /// <summary>
        /// Add a new Person When PersonRequest is Null
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddPerson_NullRequest_ToBeArgumentNullException()
        {
            // Act + Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => 
                await _personsAdderService.AddPerson(null)
            );
        }

        /// <summary>
        /// Add a new Person When PersonRequest is Invalid
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddPerson_InvalidRequest_ToBeArgumentException()
        {
            // Arrange
            
            PersonRequest personRequest = _fixture.Build<PersonRequest>()
            .With(x => x.FirstName, null as string)
            .With(x => x.LastName, null as string)
            .With(x => x.Email, null as string)
            .With(x => x.PhoneNumber, null as string)
            .With(x => x.CountryId, Guid.Empty)
            .Create();

            _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>()))
            .ReturnsAsync(null as Person);

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await _personsAdderService.AddPerson(personRequest)
            );
        }

        /// <summary>
        /// Add a new Person When Person Email already exists
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddPerson_DuplicatePersonWithSameEmailExists_ToBeArgumentException()
        {
            PersonRequest personRequest1 = _fixture.Build<PersonRequest>()
            .With(p => p.Email, "sky@gmail.com")
            .With(p => p.PhoneNumber, "1234567890")
            .Create();

            PersonRequest personRequest2 = _fixture.Build<PersonRequest>()
            .With(p => p.Email, "sky@gmail.com")
            .With(p => p.PhoneNumber, "1234567899")
            .Create();

            Person person = personRequest1.ToPerson();

            _personsRepositoryMock.Setup(temp => temp.GetPersonByEmail(It.IsAny<string>()))
                .ReturnsAsync(person);

            _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _personsAdderService.AddPerson(personRequest1);
                await _personsAdderService.AddPerson(personRequest2);
            });
        }

        /// <summary>
        /// Add a new Person When Person PhoneNumber already exists
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddPerson_DuplicatePersonWithSamePhoneExists_ToBeArgumentException()
        {
            PersonRequest personRequest1 = _fixture.Build<PersonRequest>()
            .With(p => p.Email, "test@example.com")
            .With(p => p.PhoneNumber, "1234567890")
            .Create();

            PersonRequest personRequest2 = _fixture.Build<PersonRequest>()
            .With(p => p.Email, "teste@example.com")
            .With(p => p.PhoneNumber, "1234567890")
            .Create();

            Person person = personRequest1.ToPerson();

            _personsRepositoryMock.Setup(temp => temp.GetPersonByMobile(It.IsAny<string>()))
                .ReturnsAsync(person);

            _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _personsAdderService.AddPerson(personRequest1);
                await _personsAdderService.AddPerson(personRequest2);
            });
        }


        /// <summary>
        /// Add a new Person When Person CountryId is not found
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddPerson_CountryNotFound_ToBeArgumentException()
        {
            // Arrange
            PersonRequest personRequest = _fixture.Build<PersonRequest>()
           .With(p => p.Email, "teste@example.com")
           .With(p => p.PhoneNumber, "1234567890")
           .Create();

            Person person = personRequest.ToPerson();

            _countriesRepositoryMock.Setup(temp => temp.GetCountryById(It.IsAny<Guid>()))
            .ReturnsAsync(null as Country);

            _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await _personsAdderService.AddPerson(personRequest)
            );
        }

        /// <summary>
        /// Add Person Valid Request
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddPerson_ValidRequest_ToBeSuccess()
        {
            // Arrange
            PersonRequest personRequest = _fixture.Build<PersonRequest>()
            .With(p => p.Email, "test@example.com")
            .With(p => p.PhoneNumber, "1234567890")
            
            .Create();

            Person person = personRequest.ToPerson();
            PersonResponse personResponse_Expected = person.ToPersonResponse();

            // Act
            _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);
            _personsRepositoryMock.Setup(temp => temp.GetPersonByEmail(It.IsAny<string>()))
                .ReturnsAsync(null as Person);
            _personsRepositoryMock.Setup(temp => temp.GetPersonByMobile(It.IsAny<string>()))
                .ReturnsAsync(null as Person);
            _countriesRepositoryMock.Setup(temp => temp.GetCountryById(It.IsAny<Guid>()))
                .ReturnsAsync(new Country()
                {
                    CountryId = personRequest.CountryId,
                    CountryName = "USA"
                });

            PersonResponse personResponse_Actual = await _personsAdderService.AddPerson(personRequest);

            personResponse_Expected.PersonId = personResponse_Actual.PersonId;
            personResponse_Expected.CountryId = personResponse_Actual.CountryId;
            personResponse_Expected.CountryName = personResponse_Actual.CountryName;


            // Assert
            Assert.NotNull(personResponse_Actual);
            Assert.True(personResponse_Actual.PersonId != Guid.Empty);
            Assert.Equal(personResponse_Expected, personResponse_Actual);
        }


        #endregion

        #region Get All Persons

        /// <summary>
        /// Get All Persons When List is Empty
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetAllPersons_ListEmpty_ToBeNull()
        {
            // Act
            List<PersonResponse>? personResponses = await _personsGetterService.GetAllPersons();

            // Assert
            Assert.Null(personResponses);
        }

        /// <summary>
        /// Get All Persons when List exists
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetAllPersons_ListExists_ToBeSuccess()
        {
            // Arrange
            
            Person person = _fixture.Build<Person>()
            .With(p => p.Email, "sky@example.com")
            .With(p => p.PhoneNumber, "1234567890")
            .With(p => p.Country, null as Country)
            .Create();

            List<PersonResponse> personResponses_Expected = new List<PersonResponse>()
            {
                person.ToPersonResponse()
            };

            _personsRepositoryMock.Setup(temp => temp.GetAllPersons())
                .ReturnsAsync(new List<Person>() { person });

            // Act
            List<PersonResponse>? personResponses_Actual = await _personsGetterService.GetAllPersons();

            // Assert
            Assert.NotNull(personResponses_Actual);
            Assert.Equal(personResponses_Expected, personResponses_Actual);
        }

        #endregion

        #region Get Person By Id

        /// <summary>
        /// Get Person By Id with Empty Id
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetPersonById_EmptyId_ToBeArgumentException()
        {
            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _personsGetterService.GetPersonById(Guid.Empty);
            });
        }

        /// <summary>
        /// Get a Person By Id When Person is not found
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetPersonId_NotFound_ToBeNull()
        {

            _personsRepositoryMock.Setup(temp => temp.GetPersonById(It.IsAny<Guid>()))
                .ReturnsAsync(null as Person);

            // Act
            PersonResponse? personResponse = await _personsGetterService.GetPersonById(Guid.NewGuid());

            // Assert
            Assert.Null(personResponse);
        }

        /// <summary>
        /// Get a Person By Id when person is found
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetPersonById_PersonFound_ToBeSuccess()
        {
            // Arrange
            Person person = _fixture.Build<Person>()
            .With(x => x.Email, "sky@example.com")
            .With(x => x.PhoneNumber, "1234567890")
            .With(x => x.Country, null as Country)
            .Create();

            PersonResponse personResponse_Expected = person.ToPersonResponse();

            _personsRepositoryMock.Setup(temp => temp.GetPersonById(It.IsAny<Guid>()))
            .ReturnsAsync(person);

            // Act
            PersonResponse? personResponse_Actual = await _personsGetterService.GetPersonById(person.PersonId);

            // Assert
            Assert.NotNull(personResponse_Actual);
            Assert.Equal(personResponse_Expected, personResponse_Actual);
        }

        #endregion

        #region Delete Person By Id

        /// <summary>
        /// Delete Person By Id with Empty Id
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DeletePerson_EmptyId_ToBeArgumentException()
        {
            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _personsDeleterService.DeletePerson(Guid.Empty);
            });
        }

        /// <summary>
        /// Delete Person By Id when Person is not found
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DeletePerson_NotFound_ToBeFalse()
        {
            _personsRepositoryMock.Setup(temp => temp.GetPersonById(It.IsAny<Guid>()))
                .ReturnsAsync(null as Person);

            _personsRepositoryMock.Setup(temp => temp.DeletePerson(It.IsAny<Guid>()))
                .ReturnsAsync(false);

            // Act
            bool isDeleted = await _personsDeleterService.DeletePerson(Guid.NewGuid());
            // Assert
            Assert.False(isDeleted);
        }

        /// <summary>
        /// Delete Person By Id when Person is found
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DeletePerson_PersonFound_ToBeTrue()
        {
            // Arrange
            Person person = _fixture.Build<Person>()
            .With(x => x.Email, "sky@gmail.com")
            .With(x => x.PhoneNumber, "1234567890")
            .With(x => x.Country, null as Country)
            .Create();

            _personsRepositoryMock.Setup(temp => temp.GetPersonById(It.IsAny<Guid>()))
            .ReturnsAsync(person);
            _personsRepositoryMock.Setup(temp => temp.DeletePerson(It.IsAny<Guid>()))
            .ReturnsAsync(true);

            // Act
            bool isDeleted = await _personsDeleterService.DeletePerson(person.PersonId);

            // Assert
            Assert.True(isDeleted);
        }

        #endregion

        #region Update Person By Id

        /// <summary>
        /// Update Person By Id with Empty Id
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task UpdatePerson_EmptyId_ToBeArgumentException()
        {
            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _personsUpdaterService.UpdatePerson(Guid.Empty, new PersonRequest());
            });
        }

        /// <summary>
        /// Update Person By Id with Invalid PersonRequest
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task UpdatePerson_InvalidRequest_ToBeArgumentException()
        {
            // Arrange
            PersonRequest personRequest = new PersonRequest()
            {
                FirstName = null,
                LastName = null,
                Email = null,
                PhoneNumber = null,
                CountryId = Guid.Empty,
            };

            _personsRepositoryMock.Setup(temp => temp.GetPersonById(It.IsAny<Guid>()))
                .ReturnsAsync(personRequest.ToPerson());

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await _personsUpdaterService.UpdatePerson(Guid.NewGuid(), personRequest)
            );
        }

        /// <summary>
        /// Update Person By Id when Person is not found
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task UpdatePerson_PersonNotFound_ToBePersonNotFoundException()
        {
            // Arrange
             PersonRequest personRequest = _fixture.Build<PersonRequest>()
            .With(p => p.Email, "sk@gmail.com")
            .With(p => p.PhoneNumber, "1234567890")
            .Create();

            _personsRepositoryMock.Setup(temp => temp.GetPersonById(It.IsAny<Guid>()))
                .ReturnsAsync(null as Person);
            _personsRepositoryMock.Setup(temp => temp.UpdatePerson(It.IsAny<Guid>(), It.IsAny<Person>()))
                .ReturnsAsync(null as Person);
            // Act
            await Assert.ThrowsAsync<PersonNotFoundException>(async () =>
            {
                await _personsUpdaterService.UpdatePerson(Guid.NewGuid(), personRequest);
            });

        }

        /// <summary>
        /// Update Person By Id when Person is found but has duplicate email
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task UpdatePerson_DuplicateEmail_ToBeArgumentException()
        {
            // Arrange
            PersonRequest personRequest = _fixture.Build<PersonRequest>()
            .With(p => p.Email, "sk@example.com")
            .With(p => p.PhoneNumber, "1234567890")
            .Create();

            Person person = personRequest.ToPerson();
            person.PersonId = Guid.NewGuid();

            _personsRepositoryMock.Setup(temp => temp.GetPersonById(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            _personsRepositoryMock.Setup(temp => temp.GetPersonByEmail(It.IsAny<string>()))
                .ReturnsAsync(person);
            _personsRepositoryMock.Setup(temp => temp.UpdatePerson(It.IsAny<Guid>(), It.IsAny<Person>()))
                .ReturnsAsync(person);

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _personsUpdaterService.UpdatePerson(person.PersonId, personRequest);
            });

        }

        /// <summary>
        /// Update Person By Id when Person is found but has duplicate phone number
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task UpdatePerson_DuplicatePhone_ToBeArgumentException()
        {
            PersonRequest personRequest = _fixture.Build<PersonRequest>()
            .With(p => p.Email, "sky@example.com")
            .With(p => p.PhoneNumber, "1234567890")
             .Create();
            Person person = personRequest.ToPerson();
            person.PersonId = Guid.NewGuid();

            _personsRepositoryMock.Setup(temp => temp.GetPersonById(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            _personsRepositoryMock.Setup(temp => temp.GetPersonByEmail(It.IsAny<string>()))
                .ReturnsAsync(null as Person);

            _personsRepositoryMock.Setup(temp => temp.GetPersonByMobile(It.IsAny<string>()))
                .ReturnsAsync(person);
            _personsRepositoryMock.Setup(temp => temp.UpdatePerson(It.IsAny<Guid>(), It.IsAny<Person>())).ReturnsAsync(person);

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _personsUpdaterService.UpdatePerson(person.PersonId, personRequest);
            });
        }

        /// <summary>
        /// Update Person By Id when Person is found but CountryId is not found
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task UpdatePerson_CountryNotFound_ToBeArgumentException()
        {
            PersonRequest personRequest = _fixture.Build<PersonRequest>()
            .With(p=>p.Email, "sky@example.com")
            .With(p => p.PhoneNumber, "1234567890")
            .Create();

            Person person = personRequest.ToPerson();
            person.PersonId = Guid.NewGuid();

            _personsRepositoryMock.Setup(temp => temp.GetPersonById(It.IsAny<Guid>()))
                .ReturnsAsync(person);
            _personsRepositoryMock.Setup(temp => temp.GetPersonByEmail(It.IsAny<string>()))
                .ReturnsAsync(null as Person);
            _personsRepositoryMock.Setup(temp => temp.GetPersonByMobile(It.IsAny<string>()))
                .ReturnsAsync(null as Person);

            _countriesRepositoryMock.Setup(temp => temp.GetCountryById(It.IsAny<Guid>()))
                .ReturnsAsync(null as Country);

            _personsRepositoryMock.Setup(temp => temp.UpdatePerson(It.IsAny<Guid>(), It.IsAny<Person>()))
                .ReturnsAsync(person);

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _personsUpdaterService.UpdatePerson(person.PersonId, personRequest);
            });
        }

        /// <summary>
        /// Update Person By Id when Person is found and updated successfully
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task UpdatePerson_ValidRequest_ToBeSuccess()
        {
            // Arrange
            PersonRequest personRequest = _fixture.Build<PersonRequest>()
            .With(p => p.Email, "sky@example.com")
            .With(p => p.PhoneNumber, "1234567890")
            .Create();

            Person person = personRequest.ToPerson();
            person.PersonId = Guid.NewGuid();

            PersonResponse personResponse_Expected = person.ToPersonResponse();

            _personsRepositoryMock.Setup(temp => temp.GetPersonById(It.IsAny<Guid>()))
                .ReturnsAsync(person);
            _personsRepositoryMock.Setup(temp => temp.GetPersonByEmail(It.IsAny<string>()))
                .ReturnsAsync(null as Person);
            _personsRepositoryMock.Setup(temp => temp.GetPersonByMobile(It.IsAny<string>()))
                .ReturnsAsync(null as Person);
            _countriesRepositoryMock.Setup(temp => temp.GetCountryById(It.IsAny<Guid>()))
                .ReturnsAsync(new Country()
                {
                    CountryId = personRequest.CountryId,
                    CountryName = "USA"
                });
            _personsRepositoryMock.Setup(temp => temp.UpdatePerson(It.IsAny<Guid>(), It.IsAny<Person>()))
                .ReturnsAsync(person);

            // Act
            PersonResponse? personResponse_Actual = await _personsUpdaterService.UpdatePerson(person.PersonId, personRequest);
            //personResponse_Expected.PersonId = personResponse_Actual.PersonId;
            personResponse_Expected.CountryId = personResponse_Actual.CountryId;
            personResponse_Expected.CountryName = personResponse_Actual.CountryName;

            // Assert
            Assert.NotNull(personResponse_Actual);
            Assert.Equal(personResponse_Expected, personResponse_Actual);

        }

        #endregion

        #region Get Filtered Persons

        /// <summary>
        /// Get Filtered Persons search by Person Name and text is empty 
        /// </summary>
        /// <returns>List of all persons</returns>

        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName_EmptyText()
        {
            // Arrange

            List<Person> people = new List<Person>() { 
                _fixture.Build<Person>()
                .With(p => p.Email, "sky@example.com")
                .With(p => p.PhoneNumber, "1234567890")
                .With(p => p.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(p => p.Email, "sky2@example.com")
                .With(p => p.PhoneNumber, "1224567890")
                .With(p => p.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(p => p.Email, "sky3@example.com")
                .With(p => p.PhoneNumber, "1334567890")
                .With(p => p.Country, null as Country)
                .Create()
            };

            List<PersonResponse> personResponses_Expected = people.Select(p => p.ToPersonResponse()).ToList();

            _personsRepositoryMock.Setup(p => p.GetAllPersons())
            .ReturnsAsync(people);

            _personsRepositoryMock.Setup(p =>p.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
            .ReturnsAsync(people);

            // Act
            List<PersonResponse>? personResponses_Actual = await _personsGetterService.GetFilteredPersons(nameof(PersonRequest.FirstName), string.Empty);

            // Assert
            Assert.NotNull(personResponses_Actual);
            Assert.Equal(personResponses_Expected, personResponses_Actual);

        }

        /// <summary>
        /// Get Filtered Persons search by Person Name and text is found
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName_ValidRequest()
        {
            // Arrange
            List<Person> people = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(p => p.FirstName, "shiva")
                .With(p => p.Email, "sky@example.com")
                .With(p => p.PhoneNumber, "1234567890")
                .With(p => p.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(p => p.FirstName, "shivam")
                .With(p => p.Email, "sky2@example.com")
                .With(p => p.PhoneNumber, "1224567890")
                .With(p => p.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(p => p.FirstName, "Krishna")
                .With(p => p.Email, "sky3@example.com")
                .With(p => p.PhoneNumber, "1334567890")
                .With(p => p.Country, null as Country)
                .Create()
            };

            List<Person> people1 = people.Where(p => p.FirstName.Contains("Shiva", StringComparison.OrdinalIgnoreCase)).ToList();

            List<PersonResponse> personResponses_Expected = people1.Select(p => p.ToPersonResponse()).ToList();

            _personsRepositoryMock.Setup(p => p.GetAllPersons())
            .ReturnsAsync(people);

            _personsRepositoryMock.Setup(p => p.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
            .ReturnsAsync(people1);

            // Act
            List<PersonResponse>? personResponses_Actual = await _personsGetterService.GetFilteredPersons(nameof(PersonResponse.FirstName), "Shiva");

            // Assert
            Assert.NotNull(personResponses_Actual);
            Assert.Equal(personResponses_Expected, personResponses_Actual);

        }
        #endregion

        #region Get Sorted Persons List

        /// <summary>
        /// Get Sorted Persons List based on First Name in Ascending Order
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetSortedPersons_SortByFirstName_AscedingOrder_ToBeSuccess()
        {
            List<Person> people = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(p => p.Email, "sky@example.com")
                .With(p => p.PhoneNumber, "1234567890")
                .With(p => p.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(p => p.Email, "sky2@example.com")
                .With(p => p.PhoneNumber, "1224567890")
                .With(p => p.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(p => p.Email, "sky3@example.com")
                .With(p => p.PhoneNumber, "1334567890")
                .With(p => p.Country, null as Country)
                .Create()
            };

            List<PersonResponse> personResponses = people.Select(p => p.ToPersonResponse()).ToList();
            people = people.OrderBy(p => p.FirstName).ToList();
            List<PersonResponse> personResponses_Expected = people.Select(p => p.ToPersonResponse()).ToList();

            // Act
            List<PersonResponse>? personResponses_Actual = await _personsSorterService.SortPersons(personResponses,nameof(PersonResponse.FirstName), SortOrderEnum.ASCENDING);

            // Assert
            Assert.NotNull(personResponses_Actual);
            Assert.Equal(personResponses_Expected, personResponses_Actual);
        }
#endregion


    }
}
