using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [Collection("TestCollection")]
    public class CollectionCreationTests : AuthenticatedTestBase
    {
        public CollectionCreationTests(TestWebApplicationFactory factory) : base(factory) { }

        [Fact]
        public async Task Can_Create_Collection()
        {
            var command = new
            {
                collectionDTO = new
                {
                    title = "Test collection",
                    description = "Test description",
                    visibility = "Public",
                    walorTemplateId = Guid.NewGuid()  // albo poprawny ID z bazy
                }
            };

            var response = await Client.PostAsJsonAsync("/collection", command);

            response.EnsureSuccessStatusCode(); // lub Assert.Equal(HttpStatusCode.OK, response.StatusCode)
        }
    }


}
