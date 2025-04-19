using AutoFixture;
using Moq;
using Service_Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContactManagement.Controllers;
using Entities_DTO;
using Microsoft.AspNetCore.Mvc;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;

namespace ContactManagementTest
{
    public class PersonsControllerTest
    {
        private readonly Mock<IPersonsAdderService> _personsAdderServiceMock;

        private readonly Mock<IPersonsDeleterService> _personsDeleterServiceMock;

        private readonly Mock<IPersonsGetterService> _personsGetterServiceMock;

        private readonly Mock<IPersonsSorterService> _personsSorterServiceMock;

        private readonly Mock<IPersonsUpdaterService> _personsUpdaterServiceMock;

        private readonly Mock<ICountriesService> _countriesServiceMock;

        private readonly ICountriesService _countriesService;

        private readonly IFixture _fixture;

        private readonly PersonsController _personsController;


        public PersonsControllerTest()
        {
            _fixture = new Fixture();
            _countriesServiceMock = new Mock<ICountriesService>();
            _countriesService = _countriesServiceMock.Object;
            var loggerMock = new Mock<ILogger<PersonsController>>();

            _personsAdderServiceMock = new Mock<IPersonsAdderService>();
            _personsDeleterServiceMock = new Mock<IPersonsDeleterService>();
            _personsGetterServiceMock = new Mock<IPersonsGetterService>();
            _personsSorterServiceMock = new Mock<IPersonsSorterService>();
            _personsUpdaterServiceMock = new Mock<IPersonsUpdaterService>();

            _personsController = new PersonsController(_countriesService, loggerMock.Object, _personsDeleterServiceMock.Object, _personsAdderServiceMock.Object, _personsGetterServiceMock.Object, _personsSorterServiceMock.Object, _personsUpdaterServiceMock.Object);
        }

        #region Index Method

        /// <summary>
        /// Test Index Method
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Index_ValidView_ToBeSuccess()
        {
            List<PersonResponse> personResponses = new List<PersonResponse>()
            {
                _fixture.Build<PersonResponse>()
                .With(p => p.Email, "s@example.com")
                .With(p => p.PhoneNumber, "1234567890")
                .Create(),

                _fixture.Build<PersonResponse>()
                .With(p => p.Email, "sk@example.com")
                .With(p => p.PhoneNumber, "1234667890")
                .Create(),
            };

            List<PersonResponse> personResponses1 = personResponses.OrderBy(p => p.FirstName).ToList();

            _personsGetterServiceMock.Setup(x => x.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(personResponses);

            _personsSorterServiceMock.Setup(x => x.SortPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SortOrderEnum>())).ReturnsAsync(personResponses1);

            // Act
            IActionResult actionResult =  await _personsController.Index(nameof(PersonResponse.FirstName), "", nameof(PersonResponse.FirstName), SortOrderEnum.ASCENDING);

            // Assert
            ViewResult viewResult =  Assert.IsType<ViewResult>(actionResult);
            Assert.NotNull(viewResult);
            Assert.Equal("Index", viewResult.ViewName); 
            Assert.IsType<List<PersonResponse>>(viewResult.Model);
            Assert.Equal(personResponses1, viewResult.Model);
        }

        [Fact]
        public async Task Create_InvalidModelState_ReturnsViewResult()
        {
            // Arrange
            PersonRequest personRequest = _fixture.Build<PersonRequest>()
            .With(p => p.FirstName, null as string)
            .With(p => p.LastName, null as string)
            .With(p => p.Email, null as string)
            .With(p => p.PhoneNumber, null as string)
            .Create();

            // Simulate invalid model state
            _personsController.ModelState.AddModelError("FirstName", "First name is required.");
            _personsController.ModelState.AddModelError("LastName", "Last name is required.");
            _personsController.ModelState.AddModelError("Email", "Email is required.");
            _personsController.ModelState.AddModelError("PhoneNumber", "Phone number is required.");

            List<CountryResponse> countryResponses = new List<CountryResponse>()
            {
                _fixture.Build<CountryResponse>()
                .With(c => c.CountryName, "India")
                .With(c => c.CountryId, Guid.NewGuid())
                .Create(),
                _fixture.Build<CountryResponse>()
                .With(c => c.CountryName, "USA")
                .With(c => c.CountryId, Guid.NewGuid())
                .Create(),
            };

            PersonResponse personResponse = _fixture.Build<PersonResponse>()
             .With(p => p.Email, "sky@test.com")
             .With(p => p.PhoneNumber, "1234567890")
             .Create();

            _countriesServiceMock.Setup(c => c.GetAllCountries())
                .ReturnsAsync(countryResponses);

            _personsAdderServiceMock.Setup(p => p.AddPerson(personRequest))
            .ReturnsAsync(personResponse);

            // Act
            IActionResult actionResult = await _personsController.Create(personRequest);

            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(actionResult);
            Assert.NotNull(viewResult);
            Assert.Equal("Create", viewResult.ViewName);
        }

        [Fact]
        public async Task Create_ValidModelState_ToBeSuccess()
        {
            // Arrange
            PersonRequest personRequest = _fixture.Build<PersonRequest>()
            .With(p => p.Email, "sky@test.com")
            .With(p => p.PhoneNumber, "1234567890")
            .Create();

            PersonResponse personResponse = _fixture.Build<PersonResponse>()
            .With(p => p.Email, "sky@test.com")
            .With(p => p.PhoneNumber, "1234567890")
            .Create();

            List<CountryResponse> countryResponses = new List<CountryResponse>()
            {
                _fixture.Build<CountryResponse>()
                .With(c => c.CountryName, "India")
                .With(c => c.CountryId, Guid.NewGuid())
                .Create(),
                _fixture.Build<CountryResponse>()
                .With(c => c.CountryName, "USA")
                .With(c => c.CountryId, Guid.NewGuid())
                .Create(),
            };

            _personsAdderServiceMock.Setup(p => p.AddPerson(It.IsAny<PersonRequest>()))
            .ReturnsAsync(personResponse);

            _countriesServiceMock.Setup(c => c.GetAllCountries())
            .ReturnsAsync(countryResponses);

            // Act
            IActionResult actionResult = await _personsController.Create(personRequest);

            // Assert
            RedirectToActionResult viewResult = Assert.IsType<RedirectToActionResult>(actionResult);
            Assert.NotNull(viewResult);
            Assert.Equal("Index", viewResult.ActionName);
        }

        #endregion

    }
}
