using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

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
        var response = await client.GetAsync("/msgs");
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var messages = doc.RootElement.GetProperty("messages");
    }
}