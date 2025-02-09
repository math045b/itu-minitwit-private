// Program.cs
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scriban;

public class Program
{
    // Configuration (mirroring the Python "config" variables)
    private const string DATABASE = "../minitwit.db";
    private const int PER_PAGE = 30;
    private const bool DEBUG = true;
    private const string SECRET_KEY = "development key";

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Session + Memory cache for sessions
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options =>
        {
            // You can configure session timeouts here
            options.Cookie.Name = "MiniTwit.Session";
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        var app = builder.Build();

        // Use sessions
        app.UseSession();

        // In a more complex project, you could enable developer exception pages, static files, etc.
        if (DEBUG)
        {
            app.UseDeveloperExceptionPage();
        }

        // Before-request logic (like @app.before_request in Python)
        app.Use(async (context, next) =>
        {
            // Make sure db connection is available
            context.Items["db"] = ConnectDb();
            // Load current user if session user_id is present
            var userId = context.Session.GetInt32("user_id");
            if (userId.HasValue)
            {
                var user = QueryDb(context, 
                    "SELECT * FROM user WHERE user_id = @p1",
                    new object[] { userId.Value })
                    .FirstOrDefault();
                context.Items["user"] = user; 
            }
            else
            {
                context.Items["user"] = null;
            }
            await next.Invoke();
        });

        // After-request logic (like @app.after_request)
        // We'll do this in a "finally" block to ensure disposal.
        app.Use(async (context, next) =>
        {
            try
            {
                await next.Invoke();
            }
            finally
            {
                // Dispose of the connection if present
                if (context.Items["db"] is SQLiteConnection dbConn)
                {
                    dbConn.Close();
                    dbConn.Dispose();
                }
            }
        });

        // Route: '/'
        app.MapGet("/", async context =>
        {
            // Python: if not g.user => redirect(url_for('public_timeline'))
            if (!IsLoggedIn(context))
            {
                context.Response.Redirect("/public");
                return;
            }

            // print "We got a visitor from: " + str(request.remote_addr)
            Console.WriteLine("We got a visitor from: " + context.Connection.RemoteIpAddress);

            // Grab messages
            var userId = context.Session.GetInt32("user_id") ?? 0;
            var messages = QueryDb(context,
                @"SELECT message.*, user.* 
                  FROM message, user
                  WHERE message.flagged = 0 
                    AND message.author_id = user.user_id
                    AND (
                        user.user_id = @p1 OR
                        user.user_id IN (
                            SELECT whom_id 
                            FROM follower
                            WHERE who_id = @p2
                        )
                    )
                  ORDER BY message.pub_date DESC
                  LIMIT @p3",
                new object[] { userId, userId, PER_PAGE });

            var body = RenderTimelineTemplate(messages, context, followed:false, profileUser:null);
            await context.Response.WriteAsync(body);
        });

        // Route: '/public'
        app.MapGet("/public", async context =>
        {
            var messages = QueryDb(context,
                @"SELECT message.*, user.* 
                  FROM message, user
                  WHERE message.flagged = 0 
                    AND message.author_id = user.user_id
                  ORDER BY message.pub_date DESC
                  LIMIT @p1",
                new object[] { PER_PAGE });

            var body = RenderTimelineTemplate(messages, context, followed:false, profileUser:null);
            await context.Response.WriteAsync(body);
        });

        // Route: '/{username}'
        app.MapGet("/{username}", async context =>
        {
            var username = context.Request.RouteValues["username"]?.ToString();
            if (string.IsNullOrEmpty(username))
            {
                context.Response.StatusCode = 404;
                return;
            }

            // get user from DB
            var profileUser = QueryDb(context,
                "SELECT * FROM user WHERE username = @p1",
                new object[] { username }).FirstOrDefault();

            if (profileUser == null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            // check if g.user follows profileUser
            bool followed = false;
            if (IsLoggedIn(context))
            {
                var whoId = context.Session.GetInt32("user_id") ?? 0;
                var check = QueryDb(context,
                    "SELECT 1 FROM follower WHERE who_id=@p1 AND whom_id=@p2",
                    new object[] { whoId, (long)profileUser["user_id"] })
                    .FirstOrDefault();
                followed = check != null;
            }

            // get user messages
            var messages = QueryDb(context,
                @"SELECT message.*, user.*
                  FROM message, user
                  WHERE user.user_id = message.author_id
                    AND user.user_id = @p1
                  ORDER BY message.pub_date DESC
                  LIMIT @p2",
                new object[] { (long)profileUser["user_id"], PER_PAGE });

            var body = RenderTimelineTemplate(messages, context, followed, profileUser);
            await context.Response.WriteAsync(body);
        });

        // Route: '/{username}/follow'
        app.MapGet("/{username}/follow", async context =>
        {
            if (!IsLoggedIn(context))
            {
                context.Response.StatusCode = 401; // Unauthorized
                return;
            }

            var username = context.Request.RouteValues["username"]?.ToString();
            var whomId = GetUserId(context, username);
            if (whomId == null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            var whoId = context.Session.GetInt32("user_id") ?? 0;
            ExecuteNonQuery(context,
                "INSERT INTO follower (who_id, whom_id) VALUES (@p1, @p2)",
                new object[] { whoId, whomId.Value });

            Flash(context, $"You are now following \"{username}\"");
            context.Response.Redirect($"/{username}");
        });

        // Route: '/{username}/unfollow'
        app.MapGet("/{username}/unfollow", async context =>
        {
            if (!IsLoggedIn(context))
            {
                context.Response.StatusCode = 401;
                return;
            }

            var username = context.Request.RouteValues["username"]?.ToString();
            var whomId = GetUserId(context, username);
            if (whomId == null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            var whoId = context.Session.GetInt32("user_id") ?? 0;
            ExecuteNonQuery(context,
                "DELETE FROM follower WHERE who_id=@p1 AND whom_id=@p2",
                new object[] { whoId, whomId.Value });

            Flash(context, $"You are no longer following \"{username}\"");
            context.Response.Redirect($"/{username}");
        });

        // Route: '/add_message' [POST]
        app.MapPost("/add_message", async context =>
        {
            if (!IsLoggedIn(context))
            {
                context.Response.StatusCode = 401;
                return;
            }

            var form = await context.Request.ReadFormAsync();
            var text = form["text"].ToString();
            if (!string.IsNullOrEmpty(text))
            {
                var userId = context.Session.GetInt32("user_id") ?? 0;
                var pubDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                ExecuteNonQuery(context,
                    @"INSERT INTO message (author_id, text, pub_date, flagged)
                      VALUES (@p1, @p2, @p3, 0)",
                    new object[] { userId, text, pubDate });

                Flash(context, "Your message was recorded");
            }

            context.Response.Redirect("/");
        });

        // Route: '/login' [GET, POST]
        app.MapMethods("/login", new[] { "GET", "POST" }, async ctx =>
        {
            if (IsLoggedIn(ctx))
            {
                ctx.Response.Redirect("/");
                return;
            }

            if (ctx.Request.Method == "POST")
            {
                var form = await ctx.Request.ReadFormAsync();
                var username = form["username"].ToString();
                var password = form["password"].ToString();

                var user = QueryDb(ctx,
                    "SELECT * FROM user WHERE username = @p1",
                    new object[] { username }).FirstOrDefault();

                if (user == null)
                {
                    var bodyError = RenderLoginTemplate("Invalid username");
                    await ctx.Response.WriteAsync(bodyError);
                    return;
                }
                else
                {
                    var pwHash = (string)user["pw_hash"];
                    if (!CheckPasswordHash(password, pwHash))
                    {
                        var bodyError = RenderLoginTemplate("Invalid password");
                        await ctx.Response.WriteAsync(bodyError);
                        return;
                    }
                    else
                    {
                        Flash(ctx, "You were logged in");
                        ctx.Session.SetInt32("user_id", (int)(long)user["user_id"]);
                        ctx.Response.Redirect("/");
                        return;
                    }
                }
            }
            else
            {
                // GET
                var body = RenderLoginTemplate(null);
                await ctx.Response.WriteAsync(body);
            }
        });

        // Route: '/register' [GET, POST]
        app.MapMethods("/register", new[] { "GET", "POST" }, async ctx =>
        {
            if (IsLoggedIn(ctx))
            {
                ctx.Response.Redirect("/");
                return;
            }

            if (ctx.Request.Method == "POST")
            {
                var form = await ctx.Request.ReadFormAsync();
                var username = form["username"].ToString();
                var email = form["email"].ToString();
                var password = form["password"].ToString();
                var password2 = form["password2"].ToString();
                string error = null;

                if (string.IsNullOrEmpty(username))
                {
                    error = "You have to enter a username";
                }
                else if (string.IsNullOrEmpty(email) || !email.Contains("@"))
                {
                    error = "You have to enter a valid email address";
                }
                else if (string.IsNullOrEmpty(password))
                {
                    error = "You have to enter a password";
                }
                else if (password != password2)
                {
                    error = "The two passwords do not match";
                }
                else if (GetUserId(ctx, username) != null)
                {
                    error = "The username is already taken";
                }

                if (error != null)
                {
                    var bodyError = RenderRegisterTemplate(error);
                    await ctx.Response.WriteAsync(bodyError);
                    return;
                }
                else
                {
                    var pwHash = GeneratePasswordHash(password);
                    ExecuteNonQuery(ctx,
                        @"INSERT INTO user (username, email, pw_hash) 
                          VALUES (@p1, @p2, @p3)",
                        new object[] { username, email, pwHash });

                    Flash(ctx, "You were successfully registered and can login now");
                    ctx.Response.Redirect("/login");
                    return;
                }
            }
            else
            {
                // GET
                var body = RenderRegisterTemplate(null);
                await ctx.Response.WriteAsync(body);
            }
        });

        // Route: '/logout'
        app.MapGet("/logout", async context =>
        {
            Flash(context, "You were logged out");
            context.Session.Remove("user_id");
            context.Response.Redirect("/public");
        });

        // If you want to handle DB init:
        // app.MapGet("/init_db", context => { InitDb(context); return Results.Ok("DB Init done."); });

        // Run the app
        app.Run();
    }

    // --------------------------------------
    // Helper methods
    // --------------------------------------

    private static SQLiteConnection ConnectDb()
    {
        var conn = new SQLiteConnection($"Data Source={DATABASE};Version=3;");
        conn.Open();
        return conn;
    }

    private static void InitDb(HttpContext context)
    {
        // Example: in Python, we read 'schema.sql' from file
        // Here, you can embed the schema or read from a file
        string schemaSql = @"
        DROP TABLE IF EXISTS user;
        DROP TABLE IF EXISTS message;
        DROP TABLE IF EXISTS follower;

        CREATE TABLE user (
            user_id INTEGER PRIMARY KEY AUTOINCREMENT,
            username TEXT NOT NULL,
            email TEXT NOT NULL,
            pw_hash TEXT NOT NULL
        );

        CREATE TABLE message (
            message_id INTEGER PRIMARY KEY AUTOINCREMENT,
            author_id INTEGER NOT NULL,
            text TEXT NOT NULL,
            pub_date INTEGER NOT NULL,
            flagged INTEGER NOT NULL
        );

        CREATE TABLE follower (
            who_id INTEGER NOT NULL,
            whom_id INTEGER NOT NULL
        );
        ";

        var db = (SQLiteConnection)context.Items["db"];
        using var cmd = new SQLiteCommand(schemaSql, db);
        cmd.ExecuteNonQuery();
    }

    private static List<Dictionary<string, object>> QueryDb(HttpContext context, string query, object[] args)
    {
        var db = (SQLiteConnection)context.Items["db"];
        using var cmd = new SQLiteCommand(query, db);

        for (int i = 0; i < args.Length; i++)
        {
            cmd.Parameters.AddWithValue($"@p{i+1}", args[i]);
        }

        var list = new List<Dictionary<string, object>>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var row = new Dictionary<string, object>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                row[reader.GetName(i)] = reader.GetValue(i);
            }
            list.Add(row);
        }
        return list;
    }

    private static void ExecuteNonQuery(HttpContext context, string sql, object[] args)
    {
        var db = (SQLiteConnection)context.Items["db"];
        using var cmd = new SQLiteCommand(sql, db);
        for (int i = 0; i < args.Length; i++)
        {
            cmd.Parameters.AddWithValue($"@p{i+1}", args[i]);
        }
        cmd.ExecuteNonQuery();
    }

    private static long? GetUserId(HttpContext context, string username)
    {
        var row = QueryDb(context, 
            "SELECT user_id FROM user WHERE username = @p1", 
            new object[] { username })
            .FirstOrDefault();
        if (row != null)
        {
            return (long)row["user_id"];
        }
        return null;
    }

    private static bool IsLoggedIn(HttpContext context)
    {
        return context.Session.GetInt32("user_id").HasValue;
    }

    private static void Flash(HttpContext context, string message)
    {
        // Minimal "flash" storage in session
        // We'll store it under "flash_message" and let the template pick it up
        context.Session.SetString("flash_message", message);
    }

    private static string GetFlash(HttpContext context)
    {
        var msg = context.Session.GetString("flash_message");
        if (!string.IsNullOrEmpty(msg))
        {
            context.Session.Remove("flash_message");
        }
        return msg;
    }

    // Equivalent to the old "generate_password_hash" in Python
    private static string GeneratePasswordHash(string password)
    {
        // Minimal example using SHA256 (not recommended for real production use).
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password + SECRET_KEY));
        return Convert.ToBase64String(bytes);
    }

    // Equivalent to "check_password_hash"
    private static bool CheckPasswordHash(string password, string storedHash)
    {
        var testHash = GeneratePasswordHash(password);
        return storedHash == testHash;
    }

    // Equivalent to the old Jinja filter: format_datetime(timestamp)
    private static string FormatDateTime(long timestamp)
    {
        var dt = DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;
        return dt.ToString("yyyy-MM-dd @ HH:mm");
    }

    // Equivalent to jinja filter: gravatar_url
    private static string GravatarUrl(string email, int size = 80)
    {
        // Minimal approach: MD5 of email
        using var md5 = MD5.Create();
        var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(email.Trim().ToLower()));
        var sb = new StringBuilder();
        foreach (var b in hashBytes) sb.Append(b.ToString("x2"));
        var hashedEmail = sb.ToString();
        return $"http://www.gravatar.com/avatar/{hashedEmail}?d=identicon&s={size}";
    }

    // --------------------------------------
    // Scriban Templates (inlined)
    // --------------------------------------

    private static string RenderTimelineTemplate(
        List<Dictionary<string, object>> messages,
        HttpContext context,
        bool followed,
        Dictionary<string, object> profileUser
    )
    {
        // This replicates the "timeline.html" style: 
        // we just show messages, a quick nav, etc.
        // For simplicity, we combine 'public_timeline', 'user_timeline', 'timeline' in one template
        var templateSrc = @"
<!DOCTYPE html>
<html>
<head>
    <title>MiniTwit Timeline</title>
</head>
<body>
    <h1>MiniTwit</h1>
    <p style='color:green;'>{{ flash_message }}</p>
    {{ if user != null }}
        <p>Logged in as {{ user.username }}</p>
        <p>
            <a href='/logout'>Logout</a> | 
            <a href='/'>My Timeline</a> | 
            <a href='/public'>Public</a>
        </p>
        <form action='/add_message' method='post'>
            <textarea name='text'></textarea>
            <br/>
            <button type='submit'>Add message</button>
        </form>
    {{ else }}
        <p>
            <a href='/login'>Login</a> | <a href='/register'>Register</a> | <a href='/public'>Public Timeline</a>
        </p>
    {{ end }}

    {{ if profileUser != null }}
        <h2>Timeline for {{ profileUser.username }}</h2>
        {{ if user != null and user.user_id != profileUser.user_id }}
            {{ if followed }}
                <p><a href='/{{ profileUser.username }}/unfollow'>Unfollow {{ profileUser.username }}</a></p>
            {{ else }}
                <p><a href='/{{ profileUser.username }}/follow'>Follow {{ profileUser.username }}</a></p>
            {{ end }}
        {{ end }}
    {{ else if user == null }}
        <h2>Public Timeline</h2>
    {{ else }}
        <h2>Your Timeline</h2>
    {{ end }}

    <ul>
    {{ for msg in messages }}
        <li>
            <img src='{{ gravatar(msg.email) }}' width='48' height='48' alt='gravatar'/>
            <strong>{{ msg.username }}</strong> 
            on {{ datetimeformat(msg.pub_date) }} <br/>
            {{ msg.text }}
        </li>
    {{ end }}
    </ul>
</body>
</html>
";

        var template = Template.Parse(templateSrc);

        var flash = GetFlash(context);
        var user = (Dictionary<string, object>)context.Items["user"];
        
        // Render
        var output = template.Render(new
        {
            flash_message = flash,
            user = user,
            messages = messages,
            followed = followed,
            profileUser = profileUser
        }, memberRenamer: member => member.Name);

        return output;
    }

    private static string RenderLoginTemplate(string error)
    {
        // Equivalent to "login.html"
        var templateSrc = @"
<!DOCTYPE html>
<html>
<head>
    <title>Login</title>
</head>
<body>
<h1>Login</h1>
{{ if error }}
<p style='color:red;'>{{ error }}</p>
{{ end }}
<form method='post'>
    <p><input type='text' name='username' placeholder='Username'/></p>
    <p><input type='password' name='password' placeholder='Password'/></p>
    <p><button type='submit'>Login</button></p>
</form>
</body>
</html>
";
        var template = Template.Parse(templateSrc);
        return template.Render(new { error = error });
    }

    private static string RenderRegisterTemplate(string error)
    {
        // Equivalent to "register.html"
        var templateSrc = @"
<!DOCTYPE html>
<html>
<head>
    <title>Register</title>
</head>
<body>
<h1>Register</h1>
{{ if error }}
<p style='color:red;'>{{ error }}</p>
{{ end }}
<form method='post'>
    <p><input type='text' name='username' placeholder='Username'/></p>
    <p><input type='text' name='email' placeholder='Email'/></p>
    <p><input type='password' name='password' placeholder='Password'/></p>
    <p><input type='password' name='password2' placeholder='Retype Password'/></p>
    <p><button type='submit'>Register</button></p>
</form>
</body>
</html>
";
        var template = Template.Parse(templateSrc);
        return template.Render(new { error = error });
    }
}
