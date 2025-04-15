using Xunit;
using Service_Contracts;
using Service_Classes;
using Entities_DTO;
using Entities_Core;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCoreMock;
using Moq;
using AutoFixture;

namespace ContactManagementTest
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;

        private readonly IFixture _fixture;

        public CountriesServiceTest()
        {
            List<Country> InitialData = new List<Country>();

            // Mock the DbContext
            DbContextMock<ApplicationDbContext> dbContextMock =  new DbContextMock<ApplicationDbContext>(
                    new DbContextOptionsBuilder<ApplicationDbContext>().Options
                );
            // Mock the DbSet
            dbContextMock.CreateDbSetMock(x => x.Countries, InitialData);

            // Get the mocked DbContext Object
            ApplicationDbContext _dbContext = dbContextMock.Object; 

            _countriesService = new CountriesService(_dbContext);

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
                await _countriesService.AddCountry(null);
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

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _countriesService.AddCountry(countryRequest);
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

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _countriesService.AddCountry(countryRequest);
                await _countriesService.AddCountry(countryRequest);
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

            // Act
            CountryResponse countryResponse = await _countriesService.AddCountry(countryRequest);

            // Assert
            Assert.NotNull(countryResponse);
            Assert.True(countryResponse.CountryId != Guid.Empty);
            
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
            List<CountryResponse>? countryResponse = await _countriesService.GetAllCountries();
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
            CountryRequest countryRequest1 = _fixture.Create<CountryRequest>();
            CountryRequest countryRequest2 = _fixture.Create<CountryRequest>();

            List<CountryResponse> countryResponses_Expected = new List<CountryResponse>();
            countryResponses_Expected.Add(await _countriesService.AddCountry(countryRequest1));
            countryResponses_Expected.Add(await _countriesService.AddCountry(countryRequest2));

            // Act
            List<CountryResponse>? countryResponses_Actual = await _countriesService.GetAllCountries();

            // Assert
            Assert.NotNull(countryResponses_Actual);
            Assert.NotEmpty(countryResponses_Actual);
            Assert.Equal(countryResponses_Expected, countryResponses_Actual);
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
                await _countriesService.GetCountryById(Guid.Empty);
            });
        }

        /// <summary>
        /// Get Country By Id when Country Id is not found
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetCountryById_NotFound_ToBeNull()
        {
            // Act
            CountryResponse? countryResponse = await _countriesService.GetCountryById(Guid.NewGuid());

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
            CountryRequest countryRequest = _fixture.Create<CountryRequest>();

            CountryResponse countryResponse_Expected = await _countriesService.AddCountry(countryRequest);
            // Act
            CountryResponse? countryResponse_Actual = await _countriesService.GetCountryById(countryResponse_Expected.CountryId);

            // Assert
            Assert.NotNull(countryResponse_Actual);
            Assert.Equal(countryResponse_Expected, countryResponse_Actual);
        }

        #endregion


    }
}
