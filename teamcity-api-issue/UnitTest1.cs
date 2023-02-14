namespace teamcity_api_issue;

public class Tests
{
    private HttpClient? _httpClient;
    private string? _buildId;
    private string? _apiDelay;

    [OneTimeSetUp]
    public void SetUpFixture()
    {
        var username = Environment.GetEnvironmentVariable("TeamcityUsername");
        var password = Environment.GetEnvironmentVariable("TeamcityPassword");
        _buildId = Environment.GetEnvironmentVariable("TeamcityBuildId");
        _apiDelay = Environment.GetEnvironmentVariable("TeamCityApiDelay");

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://build.appulate.dev/app/rest/"),
            DefaultRequestHeaders =
            {
                Authorization = new AuthenticationHeaderValue(
                    "Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"))),
                Accept = { MediaTypeWithQualityHeaderValue.Parse(MediaTypeNames.Application.Json) }
            }
        };
    }

    [SetUp]
    public void Setup()
    {
        Console.WriteLine($"{DateTime.Now:O}: Test {TestContext.CurrentContext.Test.Name} Setup");
    }

    [Test]
    [Order(0)]
    public void SuccessTest()
    {
        Assert.Pass();
    }

    [Test]
    [Order(1)]
    public async Task TeamCitySuccessTestCountIsOne()
    {
        await Task.Delay(TimeSpan.Parse(_apiDelay!));

        var result =
            await _httpClient!.GetFromJsonAsync<JsonElement>(
                $"testOccurrences?locator=build:{_buildId},status:SUCCESS");
        var numberOfTests = result.GetProperty("testOccurrence").EnumerateArray().Count();

        Assert.That(numberOfTests, Is.EqualTo(1));
    }

    [TearDown]
    public void TearDown()
    {
        Console.WriteLine($"{DateTime.Now:O}: Test {TestContext.CurrentContext.Test.Name} TearDown");
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _httpClient?.Dispose();
    }
}