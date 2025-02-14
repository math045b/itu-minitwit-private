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
    private const string LatestFilePath = "../minitwit.web/SimulatorAPI/latest_processed_sim_action_id.txt";

    
    [Fact]
    public async Task GetLatestTest_FileIsEmpty_Minius1()
    {
        var response = await client.GetAsync("/api/Latest");
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var latestValue = doc.RootElement.GetProperty("latest").GetInt32();
        
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        Assert.Equal(-1, latestValue);
    }

    [Fact]
    public async Task GetMessages_Returns_Messages()
    {
        var context = fixture.GetDbContext();
        var user = new User{Username = "test2", Email = "test@test.com", PwHash = "23456"};
        var msg = new Message{AuthorId = 1, Text = "Hello from test", PubDate = (int) DateTimeOffset.Now.ToUnixTimeSeconds()};
        
        await context.Users.AddAsync(user);
        await context.Messages.AddAsync(msg);
        await context.SaveChangesAsync();
        
        
        var response = await client.GetAsync("/msgs");
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var messages = JsonConvert.DeserializeObject<List<Message>>(json);
        
        Assert.Equal(messages.First().Text, msg.Text);
    }
}