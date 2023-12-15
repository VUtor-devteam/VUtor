using DataAccessLibrary.Data;
using DataAccessLibrary;
using Microsoft.EntityFrameworkCore;
using Moq;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLibrary.Models;

namespace VUtor.UnitTests
{
    public class StudyGroupRepo
    {
        [Fact]
        public async Task GetAllStudyGroupsAsync_ReturnsAllStudyGroups()
        {
            var mockConfig = new Mock<IConfiguration>();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=tcp:vutor-server.database.windows.net,1433;Initial Catalog=vutor_db;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication= Active Directory Default")
                .Options;

            using (var context = new ApplicationDbContext(options, mockConfig.Object))
            {
                var studyGroupRepository = new StudyGroupRepository(context);

                var studyGroupsBefore = await studyGroupRepository.GetAllStudyGroupsAsync();
                // Seed the database with test data
                context.StudyGroups.Add(new StudyGroup { /* Initialize properties */ });
                context.StudyGroups.Add(new StudyGroup { /* Initialize properties */ });
                await context.SaveChangesAsync();

                // Execute the test code within an execution strategy
                await context.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
                {
                    using (var transaction = await context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            // Act
                            var studyGroupsAfter = await studyGroupRepository.GetAllStudyGroupsAsync();

                            // Assert
                            Assert.NotNull(studyGroupsAfter);
                            Assert.Equal((studyGroupsBefore.Count+2), studyGroupsAfter.Count);

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
        public async Task GetStudyGroupByIdAsync_ReturnsStudyGroup_WhenExists()
        {
            var mockConfig = new Mock<IConfiguration>();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=tcp:vutor-server.database.windows.net,1433;Initial Catalog=vutor_db;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication= Active Directory Default")
                .Options;

            using (var context = new ApplicationDbContext(options, mockConfig.Object))
            {
                var studyGroupRepository = new StudyGroupRepository(context);

                var newGroup = new StudyGroup { /* Initialize properties */ };
                context.StudyGroups.Add(newGroup);
                await context.SaveChangesAsync();

                await context.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
                {
                    using (var transaction = await context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            // Act
                            var studyGroup = await studyGroupRepository.GetStudyGroupByIdAsync(newGroup.Id);
                            var studyGroupsAfter = await studyGroupRepository.GetAllStudyGroupsAsync();

                            // Assert
                            Assert.NotNull(studyGroup);
                            Assert.Equal(studyGroup.Id, studyGroup.Id);

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
        public async Task GetMemberCountAsync_ReturnsCorrectMemberCount()
        {
            var mockConfig = new Mock<IConfiguration>();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=tcp:vutor-server.database.windows.net,1433;Initial Catalog=vutor_db;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication= Active Directory Default")
                .Options;

            using (var context = new ApplicationDbContext(options, mockConfig.Object))
            {
                var studyGroupRepository = new StudyGroupRepository(context);

                var studyGroupId = 1; // Adjust as necessary
                var memberCountBefore = await studyGroupRepository.GetMemberCountAsync(studyGroupId);
                context.StudyGroupMembers.Add(new StudyGroupMember { StudyGroupId = studyGroupId, ParticipantId = "User1" });
                context.StudyGroupMembers.Add(new StudyGroupMember { StudyGroupId = studyGroupId, ParticipantId = "User2" });
                await context.SaveChangesAsync();

                await context.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
                {
                    using (var transaction = await context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            // Act
                            var memberCount = await studyGroupRepository.GetMemberCountAsync(studyGroupId);

                            // Assert
                            Assert.Equal(memberCountBefore+2, memberCount);

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
        public async Task DeleteStudyGroupAsync_RemovesStudyGroupAndMembers()
        {
            var mockConfig = new Mock<IConfiguration>();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=tcp:vutor-server.database.windows.net,1433;Initial Catalog=vutor_db;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication= Active Directory Default")
                .Options;

            using (var context = new ApplicationDbContext(options, mockConfig.Object))
            {
                var studyGroupRepository = new StudyGroupRepository(context);

                var studyGroup = new StudyGroup { /* Initialize properties */ };
                context.StudyGroups.Add(studyGroup);
                context.StudyGroupMembers.Add(new StudyGroupMember { StudyGroupId = studyGroup.Id, ParticipantId = "User1" });
                await context.SaveChangesAsync();

                await context.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
                {
                    using (var transaction = await context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            // Act
                            await studyGroupRepository.DeleteStudyGroupAsync(studyGroup.Id);

                            // Assert
                            var groupInDb = await context.StudyGroups.FindAsync(studyGroup.Id);
                            var memberInDb = await context.StudyGroupMembers.FirstOrDefaultAsync(m => m.StudyGroupId == studyGroup.Id);

                            Assert.Null(groupInDb);
                            Assert.Null(memberInDb);

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
        public async Task IsMember_ReturnsTrue_WhenUserIsMember()
        {
            var mockConfig = new Mock<IConfiguration>();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=tcp:vutor-server.database.windows.net,1433;Initial Catalog=vutor_db;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication= Active Directory Default")
                .Options;

            using (var context = new ApplicationDbContext(options, mockConfig.Object))
            {
                var studyGroupRepository = new StudyGroupRepository(context);

                var studyGroupId = 1; // Adjust as necessary
                string userId = "TestUser";
                context.StudyGroupMembers.Add(new StudyGroupMember { StudyGroupId = studyGroupId, ParticipantId = userId });
                await context.SaveChangesAsync();

                await context.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
                {
                    using (var transaction = await context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            // Act
                            var isMember = await studyGroupRepository.IsMember(studyGroupId, userId);

                            // Assert
                            Assert.True(isMember);

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
        public async Task AddMemberAsync_AddsNewMemberToStudyGroup()
        {
            var mockConfig = new Mock<IConfiguration>();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("your_connection_string")
                .Options;

            using (var context = new ApplicationDbContext(options, mockConfig.Object))
            {
                var studyGroupRepository = new StudyGroupRepository(context);

                var studyGroupId = 1; // Adjust as necessary
                string newUserId = "NewUser";

                await context.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
                {
                    using (var transaction = await context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            // Act
                            await studyGroupRepository.AddMemberAsync(studyGroupId, newUserId);

                            // Assert
                            var memberInDb = await context.StudyGroupMembers.FirstOrDefaultAsync(m => m.StudyGroupId == studyGroupId && m.ParticipantId == newUserId);

                            Assert.NotNull(memberInDb);

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
        public async Task RemoveMemberAsync_RemovesMemberFromStudyGroup()
        {
            var mockConfig = new Mock<IConfiguration>();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("your_connection_string")
                .Options;

            using (var context = new ApplicationDbContext(options, mockConfig.Object))
            {
                var studyGroupRepository = new StudyGroupRepository(context);

                var studyGroupId = 1; // Adjust as necessary
                string userId = "ExistingUser";
                context.StudyGroupMembers.Add(new StudyGroupMember { StudyGroupId = studyGroupId, ParticipantId = userId });
                await context.SaveChangesAsync();

                await context.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
                {
                    using (var transaction = await context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            // Act
                            await studyGroupRepository.RemoveMemberAsync(studyGroupId, userId);

                            // Assert
                            var memberInDb = await context.StudyGroupMembers.FirstOrDefaultAsync(m => m.StudyGroupId == studyGroupId && m.ParticipantId == userId);

                            Assert.Null(memberInDb);

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
