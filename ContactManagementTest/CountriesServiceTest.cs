using Xunit;
using Service_Contracts;
using Service_Classes;
using Entities_DTO;
using Entities_Core;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCoreMock;
using Moq;
using AutoFixture;
using Repository_Contracts;

namespace ContactManagementTest
{
    public class CountriesServiceTest
    {
        private readonly ICountriesAdderService _countriesAdderService;

        private readonly ICountriesGetterService _countriesGetterService;

        private readonly IFixture _fixture;

        private readonly ICountriesRepository _countriesRepository;

        private readonly Mock<ICountriesRepository> _countriesRepositoryMock;

        public CountriesServiceTest()
        {

            // Initialize the mock repository
            _countriesRepositoryMock = new Mock<ICountriesRepository>();
            _countriesRepository = _countriesRepositoryMock.Object;

            //List<Country> InitialData = new List<Country>();

            //// Mock the DbContext
            //DbContextMock<ApplicationDbContext> dbContextMock =  new DbContextMock<ApplicationDbContext>(
            //        new DbContextOptionsBuilder<ApplicationDbContext>().Options
            //    );
            //// Mock the DbSet
            //dbContextMock.CreateDbSetMock(x => x.Countries, InitialData);

            //// Get the mocked DbContext Object
            //ApplicationDbContext _dbContext = dbContextMock.Object; 

            _countriesAdderService = new CountriesAdderService(_countriesRepository);
            _countriesGetterService = new CountriesGetterService(_countriesRepository);
            _fixture = new Fixture();
        }

        #region Add Country

        /// <summary>
        /// Add a new Country when CountryRequest is null
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddCountry_NullRequest_ToBeArgumentNullException()
        {
            // Act + Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _countriesAdderService.AddCountry(null);
            });
        }

        /// <summary>
        /// Add a new Country when Country Name is Empty
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddCountry_EmptyCountryName_ToBeArgumentException()
        {
            // Arrange
            CountryRequest countryRequest = _fixture.Build<CountryRequest>()
            .With(x => x.CountryName, null as string)
            .Create();

            Country country = countryRequest.ToCountry();

            _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>()))
            .ReturnsAsync(country);

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _countriesAdderService.AddCountry(countryRequest);
            });
        }

        /// <summary>
        /// Add a new Country when Country Name already exists
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddCountry_DuplicateCountry_ToBeArgumentException()
        {
            // Arrange
           CountryRequest countryRequest = _fixture.Build<CountryRequest>()
            .With(x => x.CountryName, "India")
            .Create();

            Country country = countryRequest.ToCountry();

            // Mock the repository to return the country when searching by name
            _countriesRepositoryMock.Setup(temp => temp.GetCountryByName(It.IsAny<string>()))
            .ReturnsAsync(country);

            // Mock the repository to return the country when adding
            _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>()))
                .ReturnsAsync(country);

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _countriesAdderService.AddCountry(countryRequest);
                await _countriesAdderService.AddCountry(countryRequest);
            });
        }

        /// <summary>
        /// Add a new Country when Country Request is valid
        /// </summary>
        /// <returns></returns>
        [Fact]  
        public async Task AddCountry_ValidRequest_ToBeSuccess()
        {
            // Arrange
            CountryRequest countryRequest = _fixture.Create<CountryRequest>();
            Country country = countryRequest.ToCountry();

            CountryResponse countryResponse_Expected = country.ToCountryResponse();

            // Mock the repository to return null when searching by name
            _countriesRepositoryMock.Setup(temp => temp.GetCountryByName(It.IsAny<string>()))
            .ReturnsAsync(null as Country);

            // Mock the repository to return the country when adding
            _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>()))
            .ReturnsAsync(country);

            // Act
            CountryResponse countryResponse_Actual = await _countriesAdderService.AddCountry(countryRequest);
            countryResponse_Expected.CountryId = countryResponse_Actual.CountryId;

            // Assert
            Assert.NotNull(countryResponse_Actual);
            Assert.True(countryResponse_Actual.CountryId != Guid.Empty);
            Assert.Equal(countryResponse_Expected, countryResponse_Actual);

        }

        #endregion

        #region Get All Countries

        /// <summary>
        /// Get All Countries when Country List is Empty
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetAllCountries_EmptyList_ToBeEmpty()
        {
            // Act 
            _countriesRepositoryMock.Setup(temp => temp.GetAllCountries())
                .ReturnsAsync(null as List<Country>);

            List<CountryResponse>? countryResponse = await _countriesGetterService.GetAllCountries();
            // Assert
            Assert.Null(countryResponse);
        }

        /// <summary>
        /// Get All Countries when Country List is not Empty
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetAllCountries_ListExists_ToBeSuccess()
        {
            // Arrange
           Country country = _fixture.Build<Country>()
            .With(c => c.Persons, null as List<Person>)
                .Create();

            CountryResponse countryResponse = country.ToCountryResponse();

            List<CountryResponse> countrieResponses_Expected = new List<CountryResponse>() { countryResponse };

            // Mock
            _countriesRepositoryMock.Setup(temp => temp.GetAllCountries())
            .ReturnsAsync(new List<Country>() { country });

            // Act
            List<CountryResponse>? countryResponses_Actual = await _countriesGetterService.GetAllCountries();

            // Assert
            Assert.NotNull(countryResponses_Actual);
            Assert.Equal(countrieResponses_Expected, countryResponses_Actual);
        }

        #endregion

        #region Get Country By Id

        /// <summary>
        /// Get Country By Id when Country Id is empty
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetCountryById_EmptyId_ToBeArgumentException()
        {
            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _countriesGetterService.GetCountryById(Guid.Empty);
            });
        }

        /// <summary>
        /// Get Country By Id when Country Id is not found
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetCountryById_NotFound_ToBeNull()
        {
            _countriesRepositoryMock.Setup(temp => temp.GetCountryById(It.IsAny<Guid>()))
            .ReturnsAsync(null as Country);

            // Act
            CountryResponse? countryResponse = await _countriesGetterService.GetCountryById(Guid.NewGuid());

            // Assert
            Assert.Null(countryResponse);
        }

        /// <summary>
        /// Get Country By Id when Country Id is found
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetCountryById_CountryExists_ToBeSuccess()
        {
            // Arrange
           Country country = _fixture.Build<Country>()
            .With(c => c.Persons, null as List<Person>)
            .Create();

            CountryResponse countryResponse_Expected = country.ToCountryResponse();

            // Mock the repository to return the country when searching by Id
            _countriesRepositoryMock.Setup(temp => temp.GetCountryById(It.IsAny<Guid>()))
                .ReturnsAsync(country);

            // Act
            CountryResponse? countryResponse_Actual = await _countriesGetterService.GetCountryById(country.CountryId);

            countryResponse_Expected.CountryId = countryResponse_Actual.CountryId;


            // Assert
            Assert.NotNull(countryResponse_Actual);
            Assert.Equal(countryResponse_Expected, countryResponse_Actual);
        }

        #endregion


    }
}
