using Bunit;
using Moq;
using Microsoft.Extensions.DependencyInjection;
using DataAccessLibrary.FileRepo;
using DataAccessLibrary.FolderRepo;
using Microsoft.AspNetCore.Components;
using DataAccessLibrary.Data;
using Microsoft.Extensions.Configuration;
using VUtor.Pages;
using Microsoft.AspNetCore.Http;
using ServiceStack;
using DataAccessLibrary.Models;
using System.Security.Claims;
using DataAccessLibrary.WebSearch;
using Blazored.Modal.Services;

public class LibraryTests
{
    private readonly TestContext _ctx;

    public LibraryTests()
    {
        _ctx = new TestContext();

        // Mock the dependencies
        var mockContext = new Mock<ApplicationDbContext>();
        var mockConfig = new Mock<IConfiguration>();
        var mockHttpContext = new Mock<IHttpContextAccessor>();
        var mockFileRepo = new Mock<IFileRepository>();
        var mockFolderRepo = new Mock<IFolderRepository>();
        var mockSearch = new Mock<ISearch>();
        var mockModalService = new Mock<IModalService>(); // Add this line

        var testUserId = "test-user-id";


        // Create a UserFile with a ProfileId that matches the testUserId
        var testFile = new UserFile { ProfileId = testUserId };
        var testFiles = new List<UserFile> { testFile };

        // Mock GetFilesForFolderAsync to return the testFiles
        mockFileRepo.Setup(repo => repo.GetFilesForFolderAsync(It.IsAny<int>()))
            .ReturnsAsync(testFiles);

        // Mock HttpContext.User.FindFirst to return a Claim with the testUserId
        mockHttpContext.Setup(accessor => accessor.HttpContext.User.FindFirst(It.IsAny<string>()))
            .Returns(new Claim(ClaimTypes.NameIdentifier, testUserId));

        // Mock GetFilesForBlobUrls to return the testFiles
        mockFileRepo.Setup(repo => repo.GetFilesForBlobUrls(It.IsAny<List<string>>()))
            .ReturnsAsync(testFiles);

        var subfolderList = new List<Folder> { new Folder() }; // Initialize subfolderList
        mockFolderRepo.Setup(repo => repo.GetSubFolders(It.IsAny<int>()))
            .ReturnsAsync(subfolderList);

        // Register the dependencies with the TestContext
        _ctx.Services.AddSingleton(mockContext.Object);
        _ctx.Services.AddSingleton(mockConfig.Object);
        _ctx.Services.AddSingleton<NavigationManager, TestNavigationManager>();
        _ctx.Services.AddSingleton(mockFileRepo.Object);
        _ctx.Services.AddSingleton(mockHttpContext.Object);
        _ctx.Services.AddSingleton(mockFolderRepo.Object);
        _ctx.Services.AddSingleton(mockSearch.Object);
        _ctx.Services.AddSingleton(mockModalService.Object); // Add this line

        var cut = _ctx.RenderComponent<Library>();
        cut.Instance.SelectedFolder = new Folder();
    }

    [Fact]
    public void LibraryComponentRendersWithoutThrowing()
    {
        var cut = _ctx.RenderComponent<Library>();
        cut.Instance.SelectedFolder = new Folder(); // Initialize SelectedFolder
        cut.Render(); // This will throw an exception if the component throws an exception when rendered
    }

    [Fact]
    public void IsCurrentUserUploader_ReturnsTrue_WhenCurrentUserIsUploader()
    {
        // Arrange
        var cut = _ctx.RenderComponent<Library>();
        cut.Instance.SelectedFolder = new Folder(); // Initialize SelectedFolder
        cut.Render();

        var testFile = new UserFile { ProfileId = "test-user-id" };

        // Act
        var result = cut.Instance.IsCurrentUserUploader(testFile);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void LibraryComponentRendersErrorMessage()
    {
        // Arrange
        var cut = _ctx.RenderComponent<Library>();
        cut.Instance.SelectedFolder = new Folder(); // Initialize SelectedFolder
        cut.Instance.ErrorMessage = "Error fetching folders.";

        // Act
        cut.Render();

        // Assert
        cut.Find("div.alert.alert-danger").TextContent.MarkupMatches("Error fetching folders.");
    }


}


public class TestNavigationManager : NavigationManager
{
    public string LastNavigationUri { get; private set; }

    protected override void NavigateToCore(string uri, bool forceLoad)
    {
        LastNavigationUri = uri;
    }

    protected override void EnsureInitialized()
    {
        Uri uri = new Uri("http://localhost/");
        Uri = uri.AbsoluteUri;
        BaseUri = uri.AbsoluteUri;
    }
}