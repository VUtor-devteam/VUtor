using Bunit;
using DataAccessLibrary.Models;
using DataAccessLibrary.FolderRepo;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using VUtor.Components;
using System.Configuration;
using Syncfusion.Blazor.DropDowns;

public class FolderCompTests : TestContext
{
    [Fact]
    public void FolderCompRendersCorrectly()
    {
        // Arrange
        var mockFolderRepo = new Mock<IFolderRepository>();
        mockFolderRepo.Setup(repo => repo.GetFolder(It.IsAny<int>()))
                      .ReturnsAsync(new Folder { Id = 6 });

        Services.AddSingleton(mockFolderRepo.Object);
        var folder = new Folder { Id = 5 };
        var subFolder = new Folder { Id = 7, Name = "SubFolder1" };

        // Act
        var cut = RenderComponent<FolderComp>(parameters => parameters
            .Add(p => p.CurrentFolder, subFolder));

        // Assert
        // Make assertions on the 'cut' object which represents the rendered component
        // For example, to ensure that a certain text is present in the rendered markup:
        Assert.True(cut.Markup.Contains("New sub-folder name"));
        Assert.True(cut.Markup.Contains("Create"));
        Assert.True(cut.Markup.Contains("Go back one folder"));
    }


    [Fact]
    public async Task UpdateSubFolderList_UpdatesCurrentFolder()
    {
        // Arrange
        var mockFolderRepo = new Mock<IFolderRepository>();
        var initialFolder = new Folder { Id = 1, Name = "Initial Folder" };
        var updatedFolder = new Folder { Id = 1, Name = "Updated Folder" };

        mockFolderRepo.Setup(repo => repo.GetFolder(It.IsAny<int>()))
            .ReturnsAsync(updatedFolder);

        using var ctx = new TestContext();
        ctx.Services.AddSingleton(mockFolderRepo.Object);

        var cut = ctx.RenderComponent<FolderComp>(parameters => parameters
            .Add(p => p.CurrentFolder, initialFolder));

        // Act
        await cut.Instance.UpdateSubFolderList();

        // Assert
        Assert.Equal(updatedFolder, cut.Instance.CurrentFolder);
        mockFolderRepo.Verify(repo => repo.GetFolder(It.IsAny<int>()), Times.Once());
    }

    [Fact]
    public async Task GoBackAFolder_UpdatesSelectedFolder()
    {
        // Arrange
        var mockFolderRepo = new Mock<IFolderRepository>();
        var initialFolder = new Folder { Id = 1, Name = "Initial Folder", ParentFolderId = 2 };
        var parentFolder = new Folder { Id = 2, Name = "Parent Folder" };

        mockFolderRepo.Setup(repo => repo.GetFolder(It.Is<int>(id => id == initialFolder.ParentFolderId.Value)))
            .ReturnsAsync(parentFolder);

        using var ctx = new TestContext();
        ctx.Services.AddSingleton(mockFolderRepo.Object);

        var cut = ctx.RenderComponent<FolderComp>(parameters => parameters
            .Add(p => p.CurrentFolder, initialFolder));

        // Act
        await cut.Instance.GoBackAFolder();

        // Assert
        Assert.Equal(parentFolder, cut.Instance.SelectedFolder);
        mockFolderRepo.Verify(repo => repo.GetFolder(It.Is<int>(id => id == initialFolder.ParentFolderId.Value)), Times.Once());
    }

    [Fact]
    public async Task SubFolderSelected_UpdatesSelectedFolder()
    {
        // Arrange
        var mockFolderRepo = new Mock<IFolderRepository>();
        var initialFolder = new Folder { Id = 1, Name = "Initial Folder" };
        var selectedSubFolder = new Folder { Id = 2, Name = "Selected Subfolder" };

        mockFolderRepo.Setup(repo => repo.GetFolder(It.Is<int>(id => id == selectedSubFolder.Id)))
            .ReturnsAsync(selectedSubFolder);

        using var ctx = new TestContext();
        ctx.Services.AddSingleton(mockFolderRepo.Object);

        var cut = ctx.RenderComponent<FolderComp>(parameters => parameters
            .Add(p => p.CurrentFolder, initialFolder));

        // Act
        await cut.Instance.SubFolderSelected(new Microsoft.AspNetCore.Components.ChangeEventArgs { Value = selectedSubFolder.Id });

        // Assert
        Assert.Equal(selectedSubFolder, cut.Instance.SelectedFolder);
        mockFolderRepo.Verify(repo => repo.GetFolder(It.Is<int>(id => id == selectedSubFolder.Id)), Times.Once());
    }

    [Fact]
    public async Task GoBackAFolder_AddsError_WhenExceptionIsThrown()
    {
        // Arrange
        var mockFolderRepo = new Mock<IFolderRepository>();
        mockFolderRepo.Setup(repo => repo.GetFolder(It.IsAny<int>()))
            .ThrowsAsync(new Exception("Test exception"));

        using var ctx = new TestContext();
        ctx.Services.AddSingleton(mockFolderRepo.Object);

        var cut = ctx.RenderComponent<FolderComp>(parameters => parameters
            .Add(p => p.CurrentFolder, new Folder { Id = 1, Name = "Initial Folder", ParentFolderId = 2 }));

        // Act
        await cut.Instance.GoBackAFolder();

        // Assert
        Assert.Contains("Error selecting folder.", cut.Instance.errors);
    }

}