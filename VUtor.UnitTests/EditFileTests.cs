using Bunit;
using DataAccessLibrary.Models;
using DataAccessLibrary.Data;
using DataAccessLibrary.FileRepo;
using DataAccessLibrary.FolderRepo;
using DataAccessLibrary.GenericRepo;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using VUtor.Pages;
using Microsoft.Extensions.Logging;

public class EditFileTests
{
    [Fact]
    public void OnInitializedAsync_LoadsFileAndTopicsAndFolders()
    {
        // Arrange
        var mockFileRepo = new Mock<IFileRepository>();
        var mockTopicRepo = new Mock<IGenericRepository<TopicEntity>>();
        var mockFolderRepo = new Mock<IFolderRepository>();
        var testFile = new UserFile { Id = 1, Title = "Test File", Description = "Test Description", FolderId = 1, Folder = new Folder(), Topics = new List<TopicEntity>() };
        var testTopics = new List<TopicEntity> { new TopicEntity { Id = 1, Title = "Test Topic" } };
        var testFolders = new List<Folder> { new Folder { Id = 1, Name = "Test Folder" } };
        mockFileRepo.Setup(repo => repo.GetFileAsync(It.IsAny<int>())).ReturnsAsync(testFile);
        mockTopicRepo.Setup(repo => repo.LoadData()).ReturnsAsync(testTopics);
        mockFolderRepo.Setup(repo => repo.GetParentFolders()).ReturnsAsync(testFolders);
        var ctx = new TestContext();
        ctx.Services.AddSingleton(mockFileRepo.Object);
        ctx.Services.AddSingleton(mockTopicRepo.Object);
        ctx.Services.AddSingleton(mockFolderRepo.Object);

        // Act
        var cut = ctx.RenderComponent<EditFile>(parameters => parameters.Add(p => p.fileId, "1"));

        // Assert
        mockFileRepo.Verify(repo => repo.GetFileAsync(It.IsAny<int>()), Times.Once());
        mockTopicRepo.Verify(repo => repo.LoadData(), Times.Once());
        mockFolderRepo.Verify(repo => repo.GetParentFolders(), Times.Once());
    }
}