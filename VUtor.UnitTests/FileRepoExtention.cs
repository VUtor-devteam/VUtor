using DataAccessLibrary.Data;
using DataAccessLibrary.FileRepo;
using DataAccessLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUtor.UnitTests
{
    public class FileRepoExtention
    {
        [Fact]
        public async Task EditFile_UpdatesFilePropertiesAndRelations()
        {
            var mockConfig = new Mock<IConfiguration>();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=tcp:vutor-server.database.windows.net,1433;Initial Catalog=vutor_db;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication= Active Directory Default")
                .Options;

            using (var context = new ApplicationDbContext(options, mockConfig.Object))
            {
                var fileRepository = new FileRepository(context);

                // Seed the database with test data
                var testFile = new UserFile { /* Initialize properties */ };
                context.UserFiles.Add(testFile);
                context.Topics.AddRange(/* Add some topics */);
                context.Folders.Add(new Folder { /* Initialize properties */ });
                await context.SaveChangesAsync();

                string newTitle = "New Title";
                string newDescription = "New Description";
                List<int> newTopics = new List<int> { /* New topic IDs */ };
                int newFolderId = /* ID of the new folder */123456;

                await context.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
                {
                    using (var transaction = await context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            // Act
                            await fileRepository.EditFile(testFile, newTitle, newDescription, newTopics, newFolderId);

                            // Assert
                            var updatedFile = await context.UserFiles.FindAsync(testFile.Id);
                            Assert.Equal(newTitle, updatedFile.Title);
                            Assert.Equal(newDescription, updatedFile.Description);
                            // Assert that the topics and folder of the file have been updated correctly

                            await transaction.RollbackAsync();
                        }
                        catch
                        {
                            await transaction.RollbackAsync();
                            throw;
                        }
                    }
                });
            }
        }
    }
}
