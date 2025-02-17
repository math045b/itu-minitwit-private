using System.Net;
using System.Net.Http.Json;
using System.Runtime.Serialization;
using System.Text.Json;
using FluentAssertions;
using itu_minitwit.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;

namespace itu_minitwit.test;

public class API_Tests(InMemoryWebApplicationFactory fixture) : IClassFixture<InMemoryWebApplicationFactory>
{
    private readonly InMemoryWebApplicationFactory fixture = fixture;

    private readonly HttpClient client = fixture.CreateClient(new WebApplicationFactoryClientOptions
        { AllowAutoRedirect = true, HandleCookies = true });

    [Fact]
    public async Task GetLatest_FileIsEmpty_Minius1()
    {
        var response = await client.GetAsync("/Latest");
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var latestValue = doc.RootElement.GetProperty("latest").GetInt32();


        response.StatusCode.Should().Be(HttpStatusCode.OK);
        Assert.Equal(-1, latestValue);
    }

    [Fact]
    public async Task GetLatest_ThereIsAValue_TheValue()
    {
        var dbContext = fixture.GetDbContext();
        var lastAction = new LatestProcessedSimAction { Id = 230 };
        await dbContext.AddAsync(lastAction);
        await dbContext.SaveChangesAsync();

        var response = await client.GetAsync("/Latest");
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var latestValue = doc.RootElement.GetProperty("latest").GetInt32();


        response.StatusCode.Should().Be(HttpStatusCode.OK);
        Assert.Equal(lastAction.Id, latestValue);
    }

    [Fact]
    public async Task GetMessages_Returns_Messages()
    {
        var context = fixture.GetDbContext();
        var user = new User { Username = "test2", Email = "test@test.com", PwHash = "23456" };
        var msg = new Message
            { AuthorId = 1, Text = "Hello from test", PubDate = (int)DateTimeOffset.Now.ToUnixTimeSeconds() };

        await context.Users.AddAsync(user);
        await context.Messages.AddAsync(msg);
        await context.SaveChangesAsync();


        var response = await client.GetAsync("/msgs");
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var messages = JsonConvert.DeserializeObject<List<Message>>(json);

        Assert.Equal(messages.First().Text, msg.Text);
    }

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
            new KeyValuePair<string, string>("username", ""),
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
}