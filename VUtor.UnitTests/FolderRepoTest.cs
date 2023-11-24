using DataAccessLibrary.Data;
using DataAccessLibrary.FolderRepo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace VUtor.UnitTests
{
    public class FolderRepoTest
    {

        [Fact]
        public async Task CreateFolder_AddsAndSavesFolder()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=tcp:vutor-server.database.windows.net,1433;Initial Catalog=vutor_db;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication= Active Directory Default")
                .Options;

            using (var context = new ApplicationDbContext(options, mockConfig.Object))
            {
                var repository = new FolderRepository(context);

                // Act
                var folder = await repository.CreateFolder("TestFolder", 1);

                // Assert
                Assert.Equal("TestFolder", context.Folders.Where(x => x.Name == "TestFolder").Single().Name);
            }
        }
    }
}
