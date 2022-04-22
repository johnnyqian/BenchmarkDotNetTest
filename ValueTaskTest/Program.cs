using System.Net.Http.Headers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;

BenchmarkRunner.Run<TestValueTask>();

/// <summary>
/// Task vs ValueTask: When Should I use ValueTask?
/// https://www.youtube.com/watch?v=dCj7-KvaIJ0
/// https://www.youtube.com/watch?v=mEhkelf0K6g
/// </summary>
[MemoryDiagnoser]
public class TestValueTask
{
    private static readonly GitHubService Service = new();

    [Benchmark]
    public async Task<object> RunTask()
    {
        return await Service.GetReposAsyncTask("johnnyqian");
    }

    [Benchmark]
    public async Task<object> RunValueTask()
    {
        return await Service.GetReposAsyncValueTask("johnnyqian");
    }
}

public class GitHubService
{
    private readonly IMemoryCache cacheRepos = new MemoryCache(new MemoryCacheOptions());
    private readonly HttpClient httpClient = new HttpClient();

    public GitHubService()
    {
        // 403 (Forbidden) error if not add this line
        httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("TaskVsValueTask", "1.0"));
    }

    public async Task<List<JObject>> GetReposAsyncTask(string username)
    {
        var repos = cacheRepos.Get<List<JObject>>(username);
        if (repos is null)
        {
            var response = await httpClient.GetAsync($"https://api.github.com/users/{username}/repos");
            response.EnsureSuccessStatusCode();
            repos = await response.Content.ReadAsAsync<List<JObject>>();
            cacheRepos.Set(username, repos, TimeSpan.FromHours(1));
            Console.WriteLine("Get repos from GitHub with AsyncTask");
        }
        return repos;
    }

    public async ValueTask<List<JObject>> GetReposAsyncValueTask(string username)
    {
        var repos = cacheRepos.Get<List<JObject>>(username);
        if (repos is null)
        {
            var response = await httpClient.GetAsync($"https://api.github.com/users/{username}/repos");
            response.EnsureSuccessStatusCode();
            repos = await response.Content.ReadAsAsync<List<JObject>>();
            cacheRepos.Set(username, repos, TimeSpan.FromHours(1));
            Console.WriteLine("Get repos from GitHub with AsyncValueTask");
        }
        return repos;
    }
}
