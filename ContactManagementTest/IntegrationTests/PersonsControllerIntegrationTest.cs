using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Fizzler;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

namespace ContactManagementTest.IntegrationTests
{
    public class PersonsControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public PersonsControllerIntegrationTest(CustomWebApplicationFactory factory)
        {
            // Create a new instance of the HttpClient using the custom factory
            _client = factory.CreateClient();

        }

        #region  Index

        /// <summary>
        /// Test to check if the Index action returns the correct view
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Index_ToReturnView()
        {
            // Arrange
            string url = "/Persons/Index";

            // Act
            HttpResponseMessage httpResponseMessage = await _client.GetAsync(url);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, httpResponseMessage.StatusCode);
            string responseBody = await httpResponseMessage.Content.ReadAsStringAsync();

           HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(responseBody);

            var document = htmlDocument.DocumentNode;

            var personTable = document.QuerySelector("table.personsTableForTest");
            Assert.NotNull(personTable);
        }


        #endregion

    }
}
