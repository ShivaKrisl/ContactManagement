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

namespace ContactManagementTest
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personsService;

        private readonly ICountriesService _countriesService;

        private readonly ApplicationDbContext _dbContext;

        private readonly IFixture _fixture;

        public PersonsServiceTest()
        {
            // Mock the DbContext
            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
                    new DbContextOptionsBuilder<ApplicationDbContext>().Options
                );

            // seed data
            List<Person> people = new List<Person>();
            List<Country> countries = new List<Country>();

            // Mock the DbSet
            dbContextMock.CreateDbSetMock(x => x.Persons, people);
            dbContextMock.CreateDbSetMock(x => x.Countries, countries);

            // get the mocked DbContext Object
            _dbContext = dbContextMock.Object;

            _countriesService = new CountriesService(_dbContext);
            _personsService = new PersonsService(_countriesService, _dbContext);

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
                await _personsService.AddPerson(null)
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

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await _personsService.AddPerson(personRequest)
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

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _personsService.AddPerson(personRequest1);
                await _personsService.AddPerson(personRequest2);
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

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _personsService.AddPerson(personRequest1);
                await _personsService.AddPerson(personRequest2);
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

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await _personsService.AddPerson(personRequest)
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
            CountryRequest countryRequest = _fixture.Create<CountryRequest>();

            CountryResponse countryResponse = await _countriesService.AddCountry(countryRequest);

            PersonRequest personRequest = _fixture.Build<PersonRequest>()
            .With(p => p.Email, "test@example.com")
            .With(p => p.PhoneNumber, "1234567890")
            .With(p => p.CountryId, countryResponse.CountryId)
            .Create();

            // Act
            PersonResponse personResponse = await _personsService.AddPerson(personRequest);

            // Assert
            Assert.NotNull(personResponse);
            Assert.True(personResponse.PersonId != Guid.Empty);
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
            List<PersonResponse>? personResponses = await _personsService.GetAllPersons();

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
            CountryRequest countryRequest = _fixture.Create<CountryRequest>();

            CountryResponse countryResponse = await _countriesService.AddCountry(countryRequest);

            PersonRequest personRequest1 = _fixture.Build<PersonRequest>()
            .With(p => p.Email, "test@example.com")
            .With(p => p.PhoneNumber, "1234567890")
            .With(p => p.CountryId, countryResponse.CountryId)
            .Create();

            PersonRequest personRequest2 = _fixture.Build<PersonRequest>()
            .With(p => p.Email, "test2@example.com")
            .With(p => p.PhoneNumber, "1234567800")
            .With(p => p.CountryId, countryResponse.CountryId)
            .Create();

            List<PersonResponse> personResponses_Expected = new List<PersonResponse>()
            {
                await _personsService.AddPerson(personRequest1),
                await _personsService.AddPerson(personRequest2)
            };

            // Act
            List<PersonResponse>? personResponses_Actual = await _personsService.GetAllPersons();

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
                await _personsService.GetPersonById(Guid.Empty);
            });
        }

        /// <summary>
        /// Get a Person By Id When Person is not found
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetPersonId_NotFound_ToBeNull()
        {
            // Act
            PersonResponse? personResponse = await _personsService.GetPersonById(Guid.NewGuid());

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
            CountryRequest countryRequest = _fixture.Create<CountryRequest>();

            CountryResponse countryResponse = await _countriesService.AddCountry(countryRequest);

            PersonRequest personRequest = _fixture.Build<PersonRequest>()
                .With(x => x.Email, "test@example.com")
                .With(x => x.PhoneNumber, "1234567890")
                .With(x => x.CountryId, countryResponse.CountryId)
                .Create();

            PersonResponse personResponse_Expected = await _personsService.AddPerson(personRequest);

            // Act
            PersonResponse? personResponse_Actual = await _personsService.GetPersonById(personResponse_Expected.PersonId);

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
                await _personsService.DeletePerson(Guid.Empty);
            });
        }

        /// <summary>
        /// Delete Person By Id when Person is not found
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DeletePerson_NotFound_ToBeFalse()
        {
            // Act
            bool isDeleted = await _personsService.DeletePerson(Guid.NewGuid());
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
            CountryRequest countryRequest = _fixture.Create<CountryRequest>();

            CountryResponse countryResponse = await _countriesService.AddCountry(countryRequest);

            PersonRequest personRequest = _fixture.Build<PersonRequest>()
            .With(p => p.Email, "test@example.com")
            .With(p => p.PhoneNumber, "1234567890")
            .With(p => p.CountryId, countryResponse.CountryId)
            .Create();

            PersonResponse personResponse = await _personsService.AddPerson(personRequest);

            // Act
            bool isDeleted = await _personsService.DeletePerson(personResponse.PersonId);
            // Assert
            Assert.True(isDeleted);
            PersonResponse? personResponse_Actual = await _personsService.GetPersonById(personResponse.PersonId);
            Assert.Null(personResponse_Actual);
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
                await _personsService.UpdatePerson(Guid.Empty, new PersonRequest());
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
            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await _personsService.UpdatePerson(Guid.NewGuid(), personRequest)
            );
        }

        /// <summary>
        /// Update Person By Id when Person is not found
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task UpdatePerson_PersonNotFound_ToBeNull()
        {
            // Arrange
            CountryRequest countryRequest = new CountryRequest()
            {
                CountryName = "USA"
            };

            CountryResponse countryResponse = await _countriesService.AddCountry(countryRequest);

            PersonRequest personRequest = new PersonRequest()
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateTime.Today,
                Gender = GenderOptions.MALE,
                Email = "sk@gmail.com",
                PhoneNumber = "1223456789",
                CountryId = countryResponse.CountryId
            };

            PersonResponse personResponse = await _personsService.AddPerson(personRequest);

            // Act
            PersonResponse? personResponse_Actual = await _personsService.UpdatePerson(Guid.NewGuid(), personRequest);

            // Assert
            Assert.Null(personResponse_Actual);

        }

        /// <summary>
        /// Update Person By Id when Person is found but has duplicate email
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task UpdatePerson_DuplicateEmail_ToBeArgumentException()
        {
            CountryRequest countryRequest = new CountryRequest()
            {
                CountryName = "USA"
            };

            CountryResponse countryResponse = await _countriesService.AddCountry(countryRequest);

            PersonRequest personRequest1 = new PersonRequest()
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateTime.Today,
                Gender = GenderOptions.MALE,
                Email = "sk@gmail.com",
                PhoneNumber = "1223456789",
                CountryId = countryResponse.CountryId
            };

            PersonRequest personRequest2 = new PersonRequest()
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateTime.Today,
                Gender = GenderOptions.FEMALE,
                Email = "s@gmail.com",
                PhoneNumber = "1234567890",
                CountryId = countryResponse.CountryId
            };

            PersonResponse personResponse1 = await _personsService.AddPerson(personRequest1);
            PersonResponse personResponse2 = await _personsService.AddPerson(personRequest2);

            PersonRequest personRequest_ToBeUpdated = new PersonRequest()
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateTime.Today,
                Gender = GenderOptions.MALE,
                Email = "sk@gmail.com",
                PhoneNumber = "1334567890",
                CountryId = countryResponse.CountryId
            };

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _personsService.UpdatePerson(personResponse2.PersonId, personRequest_ToBeUpdated);
            });

        }

        /// <summary>
        /// Update Person By Id when Person is found but has duplicate phone number
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task UpdatePerson_DuplicatePhone_ToBeArgumentException()
        {
            CountryRequest countryRequest = new CountryRequest()
            {
                CountryName = "USA"
            };

            CountryResponse countryResponse = await _countriesService.AddCountry(countryRequest);

            PersonRequest personRequest1 = new PersonRequest()
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateTime.Today,
                Gender = GenderOptions.MALE,
                Email = "sk@gmail.com",
                PhoneNumber = "1223456789",
                CountryId = countryResponse.CountryId
            };

            PersonRequest personRequest2 = new PersonRequest()
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateTime.Today,
                Gender = GenderOptions.FEMALE,
                Email = "s@gmail.com",
                PhoneNumber = "1234567890",
                CountryId = countryResponse.CountryId
            };

            PersonResponse personResponse1 = await _personsService.AddPerson(personRequest1);
            PersonResponse personResponse2 = await _personsService.AddPerson(personRequest2);

            PersonRequest personRequest_ToBeUpdated = new PersonRequest()
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateTime.Today,
                Gender = GenderOptions.MALE,
                Email = "sky@gmail.com",
                PhoneNumber = "1223456789",
                CountryId = countryResponse.CountryId
            };

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _personsService.UpdatePerson(personResponse2.PersonId, personRequest_ToBeUpdated);
            });
        }

        /// <summary>
        /// Update Person By Id when Person is found but CountryId is not found
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task UpdatePerson_CountryNotFound_ToBeArgumentException()
        {
            CountryRequest countryRequest = new CountryRequest()
            {
                CountryName = "USA"
            };

            CountryResponse countryResponse = await _countriesService.AddCountry(countryRequest);

            PersonRequest personRequest1 = new PersonRequest()
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateTime.Today,
                Gender = GenderOptions.MALE,
                Email = "sk@gmail.com",
                PhoneNumber = "1223456789",
                CountryId = countryResponse.CountryId
            };

            PersonRequest personRequest2 = new PersonRequest()
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateTime.Today,
                Gender = GenderOptions.FEMALE,
                Email = "s@gmail.com",
                PhoneNumber = "1234567890",
                CountryId = countryResponse.CountryId
            };

            PersonResponse personResponse1 = await _personsService.AddPerson(personRequest1);
            PersonResponse personResponse2 = await _personsService.AddPerson(personRequest2);

            PersonRequest personRequest_ToBeUpdated = new PersonRequest()
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateTime.Today,
                Gender = GenderOptions.MALE,
                Email = "sky@gmail.com",
                PhoneNumber = "1234456789",
                CountryId = Guid.NewGuid()
            };

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _personsService.UpdatePerson(personResponse2.PersonId, personRequest_ToBeUpdated);
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
            CountryRequest countryRequest = new CountryRequest()
            {
                CountryName = "USA"
            };

            CountryResponse countryResponse = await _countriesService.AddCountry(countryRequest);

            PersonRequest personRequest = new PersonRequest()
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateTime.Today,
                Gender = GenderOptions.FEMALE,
                Email = "s@gmail.com",
                PhoneNumber = "1234567890",
                CountryId = countryResponse.CountryId
            };

            PersonResponse personResponse = await _personsService.AddPerson(personRequest);

            // Act
            PersonRequest personRequest_ToBeUpdate = new PersonRequest()
            {
                FirstName = "John1",
                LastName = "Doe1",
                DateOfBirth = DateTime.Today,
                Gender = GenderOptions.MALE,
                Email = "sk@gmail.com",
                PhoneNumber = "1224567890",
                CountryId = countryResponse.CountryId
            };

            PersonResponse? personResponse_Actual = await _personsService.UpdatePerson(personResponse.PersonId, personRequest_ToBeUpdate);

            PersonResponse? personResponse_Expected = await _personsService.GetPersonById(personResponse.PersonId);

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
            CountryRequest countryRequest = _fixture.Create<CountryRequest>();

            CountryResponse countryResponse = await _countriesService.AddCountry(countryRequest);

            PersonRequest personRequest1 = _fixture.Build<PersonRequest>()
            .With(p => p.Email, "test@example.com")
            .With(p => p.PhoneNumber, "1234567890")
            .With(p => p.CountryId, countryResponse.CountryId)
            .Create();

            PersonRequest personRequest2 = _fixture.Build<PersonRequest>()
            .With(p => p.Email, "test2@example.com")
            .With(p => p.PhoneNumber, "1234557890")
            .With(p => p.CountryId, countryResponse.CountryId)
            .Create();

            List<PersonResponse> personResponses_Expected = new List<PersonResponse>()
            {
                await _personsService.AddPerson(personRequest1),
                await _personsService.AddPerson(personRequest2)
            };

            // Act
            List<PersonResponse>? personResponses_Actual = await _personsService.GetFilteredPersons(nameof(PersonRequest.FirstName), "");

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
            CountryRequest countryRequest = _fixture.Create<CountryRequest>();

            CountryResponse countryResponse = await _countriesService.AddCountry(countryRequest);

            PersonRequest personRequest1 = _fixture.Build<PersonRequest>()
            .With(p => p.FirstName, "Shiva")
            .With(p => p.Email, "test@example.com")
            .With(p => p.PhoneNumber, "1234567890")
            .With(p => p.CountryId, countryResponse.CountryId)
            .Create();

            PersonRequest personRequest2 = _fixture.Build<PersonRequest>()
            .With(p => p.FirstName, "krishna")
            .With(p => p.Email, "test2@example.com")
            .With(p => p.PhoneNumber, "1234557890")
            .With(p => p.CountryId, countryResponse.CountryId)
            .Create();

            //await _personsService.AddPerson(personRequest1);
            

            List<PersonResponse> personResponses_Expected = new List<PersonResponse>()
            {
                await _personsService.AddPerson(personRequest1),
                await _personsService.AddPerson(personRequest2)
            };

            // Act
            List<PersonResponse>? personResponses_Actual = await _personsService.GetFilteredPersons(nameof(PersonRequest.FirstName), "Sh");

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
            CountryRequest countryRequest = _fixture.Create<CountryRequest>();

            CountryResponse countryResponse = await _countriesService.AddCountry(countryRequest);

            PersonRequest personRequest1 = _fixture.Build<PersonRequest>()
            .With(p => p.Email, "test@example.com")
            .With(p => p.PhoneNumber, "1234567890")
            .With(p => p.CountryId, countryResponse.CountryId)
            .Create();

            PersonRequest personRequest2 = _fixture.Build<PersonRequest>()
            .With(p => p.Email, "test2@example.com")
            .With(p => p.PhoneNumber, "1234557890")
            .With(p => p.CountryId, countryResponse.CountryId)
            .Create();

            PersonResponse personResponse1 =  await _personsService.AddPerson(personRequest1);
            PersonResponse personResponse2 =  await _personsService.AddPerson(personRequest2);

            List<PersonResponse> personResponses = new List<PersonResponse>()
            {
                personResponse1,
                personResponse2
            };

            List<PersonResponse> personResponses_Expected = personResponses.OrderBy(p => p.FirstName).ToList();

            // Act
            List<PersonResponse>? personResponses_Actual = await _personsService.SortPersons(personResponses, nameof(PersonResponse.FirstName), SortOrderEnum.ASCENDING);

            // Assert
            Assert.NotNull(personResponses_Actual);
            Assert.Equal(personResponses_Expected, personResponses_Actual);
        }
#endregion


    }
}
