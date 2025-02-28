using System.Net;
using System.Text.Json;
using Api.DataAccess.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;

namespace Api.UnitTest;

public class UnitTest(InMemoryWebApplicationFactory fixture) : IClassFixture<InMemoryWebApplicationFactory>
{
    private readonly InMemoryWebApplicationFactory fixture = fixture;

    private readonly HttpClient client = fixture.CreateClient(new WebApplicationFactoryClientOptions
        { AllowAutoRedirect = true, HandleCookies = true });

    [Fact]
    public async Task GetLatest_FileIsEmpty_Minius1()
    {
        fixture.ResetDB();
        var response = await client.GetAsync("api/Latest");
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var latestValue = doc.RootElement.GetProperty("latest").GetInt32();
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        Assert.Equal(-1, latestValue);
    }

    [Fact]
    public async Task GetLatest_ThereIsAValue_TheValue()
    {
        fixture.ResetDB();
        var dbContext = fixture.GetDbContext();
        var lastAction = new LatestProcessedSimAction { Id = 230 };
        await dbContext.AddAsync(lastAction);
        await dbContext.SaveChangesAsync();

        var response = await client.GetAsync("api/Latest");
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var latestValue = doc.RootElement.GetProperty("latest").GetInt32();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        Assert.Equal(lastAction.Id, latestValue);
    }
    
    [Fact]
    public async Task FollowUser_FollowsItself_BadRequest()
    {
        fixture.ResetDB();
        var dbContext = fixture.GetDbContext();
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("follow", "test"),
        });
        
        var response = await client.PostAsync("/fllws/test", content);
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var user = dbContext.Users.FirstOrDefault(user => user.Username == "test");
        Assert.Null(user);
    }

    [Fact]
    public async Task FollowUser_FollowsUser_NoContent()
    {
        // Arrange
        fixture.ResetDB();
        
        var dbContext = fixture.GetDbContext();
        var user1 = new User { Username = "test", Email = "", PwHash = "" };
        var user2 = new User { Username = "test2", Email = "", PwHash = "" };
        
        dbContext.Users.Add(user1);
        dbContext.Users.Add(user2);
        await dbContext.SaveChangesAsync();
        
        // Act
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("follow", "test2"),
        });

        var response = await client.PostAsync("/fllws/test", content);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var user = dbContext.Users.FirstOrDefault(user => user.Username == "test2");

        // Assert
        Assert.NotNull(user);
    }

    [Fact]
    public async Task UnfollowUser_UnfollowsItself_BadRequest()
    {
        fixture.ResetDB();
        
        var dbContext = fixture.GetDbContext();
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("unfollow", "test"),
        });
        
        var response = await client.PostAsync("/fllws/test", content);
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var user = dbContext.Users.FirstOrDefault(user => user.Username == "test");
        Assert.Null(user);
    }
    
    [Fact]
    public async Task UnfollowUser_UnfollowsUser_NoContent()
    {
        // Arrange
        fixture.ResetDB();
        
        var dbContext = fixture.GetDbContext();
        var user1 = new User { UserId = 1, Username = "test", Email = "", PwHash = "" };
        var user2 = new User { UserId = 2, Username = "test2", Email = "", PwHash = "" };
        
        var followRelation = new Follower
        {
            WhoId = user1.UserId,
            WhomId = user2.UserId
        };

        dbContext.Users.Add(user1);
        dbContext.Users.Add(user2);
        dbContext.Followers.Add(followRelation);

        await dbContext.SaveChangesAsync();
        
        // Act
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("unfollow", "test2"),
        });

        var response = await client.PostAsync("/fllws/test", content);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var user = dbContext.Users.FirstOrDefault(user => user.Username == "test2");

        // Assert
        Assert.NotNull(user);
    }
    
    [Fact]
    public async Task Getfollows_ReturnsFollows()
    {
        // Arrange
        fixture.ResetDB();
        
        var dbContext = fixture.GetDbContext();
        var user1 = new User { UserId = 1, Username = "test", Email = "", PwHash = "" };
        var user2 = new User { UserId = 2, Username = "test2", Email = "", PwHash = "" };

        var followRelation = new Follower
        {
            WhoId = user1.UserId,
            WhomId = user2.UserId
        };

        await dbContext.Users.AddAsync(user1);
        await dbContext.Users.AddAsync(user2);
        await dbContext.Followers.AddAsync(followRelation);

        await dbContext.SaveChangesAsync();
        
        // Act
        var response = await client.GetAsync("/fllws/test");
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var follows = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        Assert.Equal(user2.Username, follows!["follows"].First());
    }

    #region Register
    
    [Fact]
    public async Task Register_UsernameValidation_StatusCode400()
    {
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("username", ""),
            new KeyValuePair<string, string>("email", "test@test.com"),
            new KeyValuePair<string, string>("psw", "test123!"),
        });

        var response = await client.PostAsync("/register", content);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var errorMessage = doc.RootElement.GetProperty("error_msg").GetString();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        Assert.Equal("You have to enter a username", errorMessage);
    }

    [Fact]
    public async Task Register_EmailValidation_StatusCode400()
    {
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("username", "test"),
            new KeyValuePair<string, string>("email", "test.com"),
            new KeyValuePair<string, string>("psw", "test123!"),
        });

        var response = await client.PostAsync("/register", content);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var errorMessage = doc.RootElement.GetProperty("error_msg").GetString();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        Assert.Equal("You have to enter a valid email address", errorMessage);
    }

    [Fact]
    public async Task Register_PasswordValidation_StatusCode400()
    {
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("username", "test"),
            new KeyValuePair<string, string>("email", "test@test.com"),
            new KeyValuePair<string, string>("psw", ""),
        });

        var response = await client.PostAsync("/register", content);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var errorMessage = doc.RootElement.GetProperty("error_msg").GetString();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        Assert.Equal("You have to enter a password", errorMessage);
    }

    [Fact]
    public async Task Register_UsernameTaken_StatusCode400()
    {
        fixture.ResetDB();
        var dbContext = fixture.GetDbContext();
        User user = new User
        {
            Username = "test",
            Email = "test@test.com",
            PwHash = "test123!",
        };
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("username", "test"),
            new KeyValuePair<string, string>("email", "test@test.com"),
            new KeyValuePair<string, string>("psw", "test123!"),
        });

        var response = await client.PostAsync("/register", content);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var errorMessage = doc.RootElement.GetProperty("error_msg").GetString();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        Assert.Equal("The username is already taken", errorMessage);
    }

    [Fact]
    public async Task Register_RegistersUser_StatusCode200()
    {
        var dbContext = fixture.GetDbContext();
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("username", "test"),
            new KeyValuePair<string, string>("email", "test@test.com"),
            new KeyValuePair<string, string>("psw", "test123!"),
        });

        var response = await client.PostAsync("/register", content);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var user = dbContext.Users.FirstOrDefault(user => user.Username == "test");
        Assert.NotNull(user);
    }

    #endregion
}