using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;

namespace AzDoGitPermissions
{

    public class Status
    {
        public string id { get; set; }
        public string reportName { get; set; }
        public string requestor { get; set; }
        public DateTime requestedTime { get; set; }
        public string reportStatus { get; set; }
        public DateTime reportStatusLastUpdatedTime { get; set; }
        public string error { get; set; }
    }


    public class PermissionResult
    {
        public _Link _link { get; set; }
        public _Downloadlink _downloadLink { get; set; }
    }

    public class _Link
    {
        public string href { get; set; }
    }

    public class _Downloadlink
    {
        public string href { get; set; }
    }

    public class Permissions
    {
        public object[] descriptors { get; set; }
        public string reportName { get; set; }
        public Resource[] Resources { get; set; }
    }

    public class Resource
    {
        public string resourceId { get; set; }
        public string resourceName { get; set; }
        public string resourceType { get; set; }
    }


    public class Repos
    {
        public Repo[] value { get; set; }
        public int count { get; set; }
    }

    public class Repo
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public Project project { get; set; }
        public string defaultBranch { get; set; }
        public int size { get; set; }
        public string remoteUrl { get; set; }
        public string sshUrl { get; set; }
        public string webUrl { get; set; }
        public bool isDisabled { get; set; }
    }






    public class ProjectList
    {
        public int count { get; set; }
        public Project[] value { get; set; }
    }

    public class Project
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string state { get; set; }
        public int revision { get; set; }
        public string visibility { get; set; }
        public DateTime lastUpdateTime { get; set; }
        public string description { get; set; }
    }

    internal class Program
    {
        public static string? orgname;
        public static string? PAT;
        static void Main(string[] args)
        {
            Console.WriteLine("Getting Permissions!");
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", true, true);

            var config = builder.Build();
            orgname = config["orgname"];
            PAT = config["PAT"];
          //  Console.WriteLine(orgname);
         //   Console.WriteLine(PAT);
            GetProjects();
        }


        public static async void GetProjects()
        {
            ProjectList projectList = new ProjectList();
            try
            {
                var personalaccesstoken = PAT;

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(
                            System.Text.ASCIIEncoding.ASCII.GetBytes(
                                string.Format("{0}:{1}", "", personalaccesstoken))));

                    string url = String.Format("https://dev.azure.com/{0}/_apis/projects", orgname);

                    using (HttpResponseMessage response = client.GetAsync(url).Result)
                    {
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();
                        projectList = JsonSerializer.Deserialize<ProjectList>(responseBody);
                        foreach (var item in projectList.value)
                        {
                            Thread.Sleep(1000);
                            Console.WriteLine("Project:{0}", item.name);
                            GetRepos(item.name);
                        }
                        // Console.WriteLine(responseBody);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        

        public static async Task<string> GetPermissionStatus(string id)
        {
             Status  status = new Status();
            string result = "error";
            try
            {
                var personalaccesstoken = PAT;

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(
                            System.Text.ASCIIEncoding.ASCII.GetBytes(
                                string.Format("{0}:{1}", "", personalaccesstoken))));
                    string url = String.Format("https://dev.azure.com/{0}/_apis/permissionsreport/{1}?api-version=6.1-preview.1", "ahmeds", id);

                    using (HttpResponseMessage response = client.GetAsync(url).Result)
                    {
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();
                         status = JsonSerializer.Deserialize<Status>(responseBody);
                        //  foreach (var item in projectList.value)
                        //  {
                        //      Console.WriteLine("Project:{0}", item.name);
                        //       GetRepos(item.name);
                        //   }
                        result = status.reportStatus;
                       // Console.WriteLine(responseBody);
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return result;
            }
        }


        public static async void DownloadReport(string id)
        {
            PermissionsInfo permissionsInfo = new PermissionsInfo();
          //  string result = "error";
            try
            {
                var personalaccesstoken = PAT;

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(
                            System.Text.ASCIIEncoding.ASCII.GetBytes(
                                string.Format("{0}:{1}", "", personalaccesstoken))));
                    string url = String.Format("https://dev.azure.com/{0}/_apis/permissionsreport/{1}/download?api-version=6.1-preview.1", orgname, id);

                    using (HttpResponseMessage response = client.GetAsync(url).Result)
                    {
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();
                       // Console.WriteLine(responseBody);
                        string filename = string.Format("{0}.json", id);
                        Console.WriteLine("writing file : {0}", filename);
                        await File.WriteAllTextAsync(filename, responseBody);
                     //   permissionsInfo = JsonSerializer.Deserialize<PermissionsInfo>(responseBody);
                       //  foreach (var item in permissionsInfo.Property1)
                       // {
                        //     Console.WriteLine("Project:{0}", item.AccountName);
                        //       GetRepos(item.name);
                          }
                        //result = status.reportStatus;
                       // Console.WriteLine("----report --- ");
                       // Console.WriteLine(responseBody);
                        //return result;
                    }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
               // return result;
            }
        }


        public static async void GetRepos(string projectName)
        {
            Repos repoList = new Repos();
            try
            {
                var personalaccesstoken = PAT;

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(
                            System.Text.ASCIIEncoding.ASCII.GetBytes(
                                string.Format("{0}:{1}", "", personalaccesstoken))));

                    string url = String.Format("https://dev.azure.com/{0}/{1}/_apis/git/repositories?api-version=4.1", orgname, projectName);

                    using (HttpResponseMessage response = client.GetAsync(url).Result)
                    {
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();
                        repoList = JsonSerializer.Deserialize<Repos>(responseBody);
                        foreach (var item in repoList.value)
                        {
                            Thread.Sleep(1000);
                            Console.WriteLine("--Repo: {0}", item.name);
                            GetPermissins(projectName, item.name, item.id);
                        }
                        //  Console.WriteLine(responseBody);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        public static async void GetPermissins(string projectName, string repoName, string repoId)
        {
            Repos repoList = new Repos();
            try
            {
                var personalaccesstoken = PAT;

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(
                            System.Text.ASCIIEncoding.ASCII.GetBytes(
                                string.Format("{0}:{1}", "", personalaccesstoken))));

                    string url = String.Format("https://dev.azure.com/{0}/_apis/permissionsreport?api-version=6.1-preview.1", orgname);
                    Permissions permissions = new Permissions();
                    Resource[] resources = new Resource[1];
                    resources[0] = new Resource();
                    resources[0].resourceId = repoId;
                    resources[0].resourceName = repoName;
                    resources[0].resourceType = "Repo";
                    permissions.descriptors = new object[0];

                    // Resource resource = new Resource();
                    // resource.resourceName = repoName;
                    // resource.resourceId = repoId;
                    permissions.reportName = "Permissions-" + repoId;
                    permissions.Resources = resources;

                    string content = JsonSerializer.Serialize(permissions);
                    //Console.WriteLine(content);
                    StringContent queryString = new StringContent(content);
                    queryString.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    // HttpContent httpContent = new HttpContent();
                    // httpContent.

                 HttpResponseMessage response = client.PostAsync(url, queryString).Result;
                    
                       // if ()
                        while (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                        {
                        Console.WriteLine("going to sleep for 10 seconds due to many requests");
                        Thread.Sleep(10000);
                            response = client.PostAsync(url, queryString).Result;
                        }
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();
                        PermissionResult permissionResult = JsonSerializer.Deserialize<PermissionResult>(responseBody);
                        Uri uri = new UriBuilder(permissionResult._link.href).Uri;
                        string permissionId = uri.Segments[4]; // v1/
                        Thread.Sleep(3000);
                        string status= await GetPermissionStatus(permissionId);
                        bool c1 = (status.CompareTo("completedSuccessfully") != 0);
                        bool c2 = (status.CompareTo("created") != 0);
                        bool c = (c1 | c2);

                        while (status.CompareTo("completedSuccessfully") != 0)
                        {
                            Console.WriteLine("Status -----" + status);
                            System.Threading.Thread.Sleep(1000);
                            status = await GetPermissionStatus(permissionId);
                            if (status.CompareTo("created") == 0) break;
                            c1 = (status.CompareTo("completedSuccessfully") != 0);
                            c2 = (status.CompareTo("created") != 0);
                            c = (c1 | c2);
                        }
                        
                        if (status.CompareTo("completedSuccessfully")==0) { DownloadReport(permissionId); } else { Console.WriteLine("need to review"); }
                        
                        // permissionResult._link.href
                        // foreach (var item in repoList.value)
                        // {
                        //     Console.WriteLine("--Repo: {0}", item.name);
                        // }
                       // Console.WriteLine("-------Permissins-------");
                     //   Console.WriteLine(responseBody);
                    response.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


    }
}