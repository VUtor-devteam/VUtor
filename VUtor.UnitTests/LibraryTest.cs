using Bunit;
using Moq;
using Microsoft.Extensions.DependencyInjection;
using DataAccessLibrary.FileRepo;
using DataAccessLibrary.FolderRepo;
using Microsoft.AspNetCore.Components;
using DataAccessLibrary.Data;
using Microsoft.Extensions.Configuration;
using DataAccessLibrary.Search;
using VUtor.Pages;
using Microsoft.AspNetCore.Http;
using ServiceStack;
using DataAccessLibrary.Models;
using System.Security.Claims;
using ServiceStack.Testing;

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
        var mockSearch = new Mock<Search>();

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



        // Register the dependencies with the TestContext
        _ctx.Services.AddSingleton(mockContext.Object);
        _ctx.Services.AddSingleton(mockConfig.Object);
        _ctx.Services.AddSingleton<NavigationManager, TestNavigationManager>();
        _ctx.Services.AddSingleton(mockFileRepo.Object);
        _ctx.Services.AddSingleton(mockHttpContext.Object);
        _ctx.Services.AddSingleton(mockFolderRepo.Object);
        _ctx.Services.AddSingleton(mockSearch.Object);

    }

    [Fact]
    public void LibraryComponentRendersCorrectly()
    {
        // Arrange
        var cut = _ctx.RenderComponent<Library>();

        // Assert
        cut.MarkupMatches(@"
            <h1>Library</h1>
            <div>
              <div>
                <div></div>
                <h2 class=""congrats_h2_left""></h2>
                <div id=""HASH"">
                  <button id=""previousFolder""  type=""button"" class=""round_corners small_arrow"">
                    <img src=""css/Images/angle-small-right.png"" alt=""buttonpng"" class=""angle_img right"">
                  </button>
                  <button id=""upload""  type=""button"" class=""round_corners btn btn-outline-secondary"">Upload Files</button>
                </div>
              </div>
              <br>
              <div>
                <div class=""form-floating mb-2"">
                  <input  type=""text"" name=""filter"" class=""round_corners"" placeholder=""Search"" >
                </div>
              </div>
            </div>");

    }

    [Fact]
    public void IsCurrentUserUploader_ReturnsTrue_WhenCurrentUserIsUploader()
    {
        // Arrange
        var cut = _ctx.RenderComponent<Library>();
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
        IRenderedComponent<Library> cut = _ctx.RenderComponent<Library>();
        cut.Instance.ErrorMessage = "Error fetching folders.";

        // Act
        cut.Render();

        // Assert
        cut.Find("div.alert.alert-danger").TextContent.MarkupMatches("Error fetching folders.");
    }

    [Fact]
    public async Task PreviousFolder_NavigatesToParentFolder()
    {
        // Arrange
        var mockFolderRepo = new Mock<IFolderRepository>();
        var parentFolder = new Folder { Id = 1 };
        var childFolder = new Folder { Id = 2, ParentFolderId = parentFolder.Id };
        mockFolderRepo.Setup(repo => repo.GetFolder(parentFolder.Id)).ReturnsAsync(parentFolder);

        using var ctx = new TestContext();
        ctx.Services.AddSingleton(mockFolderRepo.Object);
        var cut = _ctx.RenderComponent<Library>();
        cut.Instance.SelectedFolder = childFolder;

        // Act
        await cut.Instance.PreviousFolder();

        // Assert
        var actualFolder = cut.Instance.SelectedFolder;
        Assert.Equal(childFolder, actualFolder);
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