using DataAccessLibrary.Data;
using DataAccessLibrary.Models;
using DataAccessLibrary.RatingRepo;
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
    public class RatingRepo
    {
        [Fact]
        public async Task GetAverageRatingAsync_ShouldReturnCorrectAverage()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=tcp:vutor-server.database.windows.net,1433;Initial Catalog=vutor_db;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication= Active Directory Default")
                .Options;

            using (var context = new ApplicationDbContext(options, mockConfig.Object))
            {
                var ratingRepository = new RatingRepository(context);
                var recipientId = "testRecipientId";

                // Seed data for testing
                context.Ratings.Add(new Rating { RecipientId = recipientId, Score = 4, ReviewerId = "testReviewer1" });
                context.Ratings.Add(new Rating { RecipientId = recipientId, Score = 5, ReviewerId = "testReviewer2" });
                await context.SaveChangesAsync();

                // Act
                var averageRating = await ratingRepository.GetAverageRatingAsync(recipientId);

                // Assert
                Assert.Equal(4.5, averageRating); // Expect average of 4.5 from scores 4 and 5
            }
        }

        [Fact]
        public async Task AddOrUpdateRatingAsync_AddsNewRating_WhenRatingDoesNotExist()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("your_connection_string")
                .Options;

            var mockConfig = new Mock<IConfiguration>();

            using (var context = new ApplicationDbContext(options, mockConfig.Object))
            {
                var ratingRepository = new RatingRepository(context);

                var newRating = new Rating { RecipientId = "newRecipient", ReviewerId = "newReviewer", Score = 5 };

                // Execute the test code within an execution strategy
                await context.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
                {
                    using (var transaction = await context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            // Act
                            await ratingRepository.AddOrUpdateRatingAsync(newRating);
                            await context.SaveChangesAsync();

                            var addedRating = await context.Ratings
                                .FirstOrDefaultAsync(r => r.RecipientId == "newRecipient" && r.ReviewerId == "newReviewer");

                            // Assert
                            Assert.NotNull(addedRating);
                            Assert.Equal(5, addedRating.Score);

                            // Rollback transaction to undo changes
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

        [Fact]
        public async Task AddOrUpdateRatingAsync_UpdatesRating_WhenRatingExists()
        {
            var mockConfig = new Mock<IConfiguration>();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=tcp:vutor-server.database.windows.net,1433;Initial Catalog=vutor_db;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication= Active Directory Default")
                .Options;

            using (var context = new ApplicationDbContext(options, mockConfig.Object))
            {
                var ratingRepository = new RatingRepository(context);

                var existingRating = new Rating { RecipientId = "existingRecipient", ReviewerId = "existingReviewer", Score = 3 };
                context.Ratings.Add(existingRating);
                await context.SaveChangesAsync();

                var updatedRating = new Rating { RecipientId = "existingRecipient", ReviewerId = "existingReviewer", Score = 4 };

                // Execute the test code within an execution strategy
                await context.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
                {
                    using (var transaction = await context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            // Act
                            await ratingRepository.AddOrUpdateRatingAsync(updatedRating);
                            await context.SaveChangesAsync();

                            var resultRating = await context.Ratings
                                .FirstOrDefaultAsync(r => r.RecipientId == "existingRecipient" && r.ReviewerId == "existingReviewer");

                            // Assert
                            Assert.NotNull(resultRating);
                            Assert.Equal(4, resultRating.Score);

                            // Rollback transaction to undo changes
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

        [Fact]
        public async Task GetRatingByReviewerAsync_ReturnsRating_WhenRatingExists()
        {
            var mockConfig = new Mock<IConfiguration>();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=tcp:vutor-server.database.windows.net,1433;Initial Catalog=vutor_db;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication= Active Directory Default")
                .Options;

            using (var context = new ApplicationDbContext(options, mockConfig.Object))
            {
                var ratingRepository = new RatingRepository(context);

                var testRating = new Rating { RecipientId = "testRecipient", ReviewerId = "testReviewer", Score = 5 };
                context.Ratings.Add(testRating);
                await context.SaveChangesAsync();

                // Execute the test code within an execution strategy
                await context.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
                {
                    using (var transaction = await context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            // Act
                            var rating = await ratingRepository.GetRatingByReviewerAsync("testRecipient", "testReviewer");

                            // Assert
                            Assert.NotNull(rating);
                            Assert.Equal(5, rating.Score);

                            // Rollback transaction to undo changes
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
