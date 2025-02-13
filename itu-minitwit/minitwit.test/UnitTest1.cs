namespace itu_minitwit.test;

using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class MiniTwitTests
{
    private readonly HttpClient _client;

    public MiniTwitTests()
    {
        _client = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };
    }

    private async Task<HttpResponseMessage> RegisterAsync(string username, string password, string password2 = null, string email = null)
    {
        if (password2 == null) password2 = password;
        if (email == null) email = username + "@example.com";
        
        var formData = new Dictionary<string, string>
        {
            { "username", username },
            { "password", password },
            { "password2", password2 },
            { "email", email }
        };

        return await _client.PostAsync("/register", new FormUrlEncodedContent(formData));
    }

    private async Task<HttpResponseMessage> LoginAsync(string username, string password)
    {
        var formData = new Dictionary<string, string>
        {
            { "username", username },
            { "password", password }
        };

        return await _client.PostAsync("/login", new FormUrlEncodedContent(formData));
    }
    
    private async Task<HttpResponseMessage> RegisterAndLogin(string username, string password)
    {
        await RegisterAsync(username, password);
        return await LoginAsync(username, password);
    }

    private async Task<HttpResponseMessage> LogoutAsync()
    {
        return await _client.GetAsync("/logout");
    }

    private async Task<HttpResponseMessage> AddMessageAsync(string text)
    {
        var formData = new Dictionary<string, string>
        {
            { "text", text }
        };
        return await _client.PostAsync("/add_message", new FormUrlEncodedContent(formData));
    }

    [Fact]
    public async Task TestRegister()
    {
        var response = await RegisterAsync("user1", "default");
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("You were successfully registered and can login now", content);

        response = await RegisterAsync("user1", "default");
        content = await response.Content.ReadAsStringAsync();
        Assert.Contains("The username is already taken", content);

        response = await RegisterAsync("", "default");
        content = await response.Content.ReadAsStringAsync();
        Assert.Contains("You have to enter a username", content);

        response = await RegisterAsync("meh", "");
        content = await response.Content.ReadAsStringAsync();
        Assert.Contains("You have to enter a password", content);

        response = await RegisterAsync("meh", "x", "y");
        content = await response.Content.ReadAsStringAsync();
        Assert.Contains("The two passwords do not match", content);

        response = await RegisterAsync("meh", "foo", email: "broken");
        content = await response.Content.ReadAsStringAsync();
        Assert.Contains("You have to enter a valid email address", content);
    }

    [Fact]
    public async Task TestLoginLogout()
    {
        var response = await RegisterAndLogin("user1", "default");
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("You were logged in", content);

        response = await LogoutAsync();
        content = await response.Content.ReadAsStringAsync();
        Assert.Contains("You were logged out", content);

        response = await LoginAsync("user1", "wrongpassword");
        content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Invalid password", content);

        response = await LoginAsync("user2", "wrongpassword");
        content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Invalid username", content);
    }


    [Fact]
    public async Task TestMessageRecording()
    {
        await RegisterAndLogin("foo", "default");
        await AddMessageAsync("test message 1");
        await AddMessageAsync("<test message 2>");

        var response = await _client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("test message 1", content);
        Assert.Contains("&lt;test message 2&gt;", content);
    }
    
    [Fact]
    public async Task TestTimelines()
    {
        await RegisterAndLogin("foo", "default");
        await AddMessageAsync("the message by foo");
        await LogoutAsync();
        
        await RegisterAndLogin("bar", "default");
        await AddMessageAsync("the message by bar");

        var response = await _client.GetAsync("/public");
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("the message by foo", content);
        Assert.Contains("the message by bar", content);

        response = await _client.GetAsync("/");
        content = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("the message by foo", content);
        Assert.Contains("the message by bar", content);

        response = await _client.GetAsync("/foo/follow");
        content = await response.Content.ReadAsStringAsync();
        Assert.Contains("You are now following", content);

        response = await _client.GetAsync("/");
        content = await response.Content.ReadAsStringAsync();
        Assert.Contains("the message by foo", content);
        Assert.Contains("the message by bar", content);

        response = await _client.GetAsync("/bar");
        content = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("the message by foo", content);
        Assert.Contains("the message by bar", content);

        response = await _client.GetAsync("/foo");
        content = await response.Content.ReadAsStringAsync();
        Assert.Contains("the message by foo", content);
        Assert.DoesNotContain("the message by bar", content);

        response = await _client.GetAsync("/foo/unfollow");
        content = await response.Content.ReadAsStringAsync();
        Assert.Contains("You are no longer following", content);

        response = await _client.GetAsync("/");
        content = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("the message by foo", content);
        Assert.Contains("the message by bar", content);
    }
}
