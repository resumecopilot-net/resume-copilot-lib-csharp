using System.Text;
using System.Text.Json;

namespace ResumeCopilotLib
{
    public class JobSearchRequest
    {
        public string Title { get; set; }
        public string CompanyName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public bool? IsRemote { get; set; }
        public int Size { get; set; }
    }

    public class JobLocation
    {
        public string city { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public GeographicLocation loc { get; set; }
    }

    public class GeographicLocation
    {
        public double lat { get; set; }
        public double lon { get; set; }
    }

    public class Job
    {
        public string name { get; set; }
        public string companyName { get; set; }
        public JobLocation location { get; set; }
        public string feed { get; set; }
        public string id { get; set; }
        public string created { get; set; }
    }

    public class JobSearchResult
    {
        public List<Job> jobs { get; set; }
    }

    public static class JobSearchHelper
    {
        private static readonly HttpClient HttpClient = new HttpClient { BaseAddress = new Uri("https://api.resumecopilot.net") };

        public static async Task<JobSearchResult> SearchJobsAsync(JobSearchRequest request)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(request);
                var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await HttpClient.PostAsync("/api/v1/search", contentString);

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var jobSearchResult = JsonSerializer.Deserialize<JobSearchResult>(responseBody);

                if (jobSearchResult == null)
                {
                    throw new JsonException("Failed to deserialize job search results.");
                }

                return jobSearchResult;
            }
            catch (HttpRequestException e)
            {
                throw new Exception("There was an error connecting to the job search service.", e);
            }
            catch (JsonException e)
            {
                throw new Exception("There was an error processing the data from the job search service.", e);
            }
            catch (Exception e)
            {
                throw new Exception("An unexpected error occurred.", e);
            }
        }
    }
}