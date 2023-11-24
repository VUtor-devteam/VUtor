using Azure.Storage.Blobs;
using DataAccessLibrary.Data;
using DataAccessLibrary.FileRepo;
using DataAccessLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace VUtor.UnitTests
{
    public class FileRepoTest
    {
        [Fact]
        public async Task GetFilesForFolderAsync_ShouldReturnListOfUserFiles()
        {
            var mockConfig = new Mock<IConfiguration>();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=tcp:vutor-server.database.windows.net,1433;Initial Catalog=vutor_db;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication= Active Directory Default")
                .Options;
            // Arrange
            using (var context = new ApplicationDbContext(options, mockConfig.Object))
            {

                var mockBlobServiceClient = new Mock<BlobServiceClient>();
                var fileRepository = new FileRepository(context, mockBlobServiceClient.Object);

                // Act
                var result = await fileRepository.GetFilesForFolderAsync(9);

                // Assert
                Assert.IsType<List<UserFile>>(result);
            }
        }

        [Fact]
        public async Task GetFilesForTopicAsync_ShouldReturnListOfUserFiles()
        {
            var mockConfig = new Mock<IConfiguration>();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=tcp:vutor-server.database.windows.net,1433;Initial Catalog=vutor_db;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication= Active Directory Default")
                .Options;
            // Arrange
            using (var context = new ApplicationDbContext(options, mockConfig.Object))
            {

                var mockBlobServiceClient = new Mock<BlobServiceClient>();
                var fileRepository = new FileRepository(context, mockBlobServiceClient.Object);

                // Act
                var result = await fileRepository.GetFilesForTopicAsync(1);

                // Assert
                Assert.IsType<List<UserFile>>(result);
            }
        }

        [Fact]
        public async Task DeleteFileAsync_ShouldNotThrowException()
        {
            var mockConfig = new Mock<IConfiguration>();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=tcp:vutor-server.database.windows.net,1433;Initial Catalog=vutor_db;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication= Active Directory Default")
                .Options;
            // Arrange

            using (var context = new ApplicationDbContext(options, mockConfig.Object))
            {
                var mockBlobServiceClient = new Mock<BlobServiceClient>();
                var fileRepository = new FileRepository(context, mockBlobServiceClient.Object);

                // Act
                var exception = await Record.ExceptionAsync(() => fileRepository.DeleteFileAsync(1));

                // Assert
                Assert.Null(exception);
            }
        }
    }
}
