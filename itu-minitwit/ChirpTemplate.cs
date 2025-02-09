namespace itu_minitwit;

public static class ChirpTemplate
{
    private const string top = @"<!DOCTYPE html>
<html>
  <head>
    <title>Chirp!</title>
    <link rel=""stylesheet"" type=""text/css"" href=""/css/style.css"">
    <link rel=""icon"" type=""image/x-icon"" href=""/icon/favicon.ico"">
  </head>
  <body>
  <div class=""page"">
  <div>
    <h1><img src=""images/icon1.png""/>Chirp!</h1>
  </div>
  <div class=""navigation"">
    {{ if isLoggedIn }}
    <a href=""/"">my timeline</a> |
    <a href=""/public"">public timeline</a> |
    <a href=""/logout"">sign out [{{-profileUsername-}}]</a>
    {{ else }}
    <a href=""/public"">public timeline</a> |
    <a href=""/register"">sign up</a> |
    <a href=""/login"">sign in</a>
    {{ end }}
  </div>
  {{ if message != """" }}
  <div class=message>{{ message }}</div>
  {{ end }}
  {{ if errorMessage != """" }}
  <div class=error><strong>Error:</strong> {{ errorMessage }}</div>
  {{ end }}
  <div class=""body"">
";
}