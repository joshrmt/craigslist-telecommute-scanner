using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using CraigslistScanner.Data;
using CsQuery;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace ScannerRole {
    public class WorkerRole : RoleEntryPoint {

        public override void Run() {
            Stop = false;

            RoleEnvironment.Stopping += RoleEnvironmentStopping;

            List<IDomObject> locations;

            using (var client = new WebClient()) {
                var document = CQ.CreateDocument(client.DownloadString("http://www.craigslist.org/about/sites"));
                locations = document.Select(".colmask:first a").ToList();
            }

            while (!Stop) {
                using (var client = new WebClient()) {
                    Shuffle(locations);

                    foreach (var anchor in locations) {
                        if (Stop) {
                            break;
                        }

                        try {
                            var url = anchor.GetAttribute("href");

                            if (url == null || url.Contains("#")) {
                                continue;
                            }

                            var jobPage = CQ.CreateDocument(client.DownloadString(url + "/search/sof?zoomToPosting=&query=&srchType=A&addOne=telecommuting"));

                            var rows = jobPage.Select(".row");

                            foreach (var row in rows) {
                                var fragment = CQ.CreateFragment(new List<IDomObject> { row });

                                var item = fragment.Select("a");
                                var date = fragment.Select(".date").Text().Replace("\t", "").Replace("\n", "").Replace(" - ", "").Trim();

                                using (var database = new CraigslistDatabase()) {
                                    var jobUrl = item.Attr("href");
                                    var postId = jobUrl.Replace(".html", "").Split(new[] { "sof/" }, StringSplitOptions.None)[1];

                                    var job = new Job { Url = jobUrl, PostId = long.Parse(postId), Name = item.Text(), Date = DateTime.Parse(date), LastUpdated = DateTime.Now };

                                    var existing = database.Jobs.FirstOrDefault(each => each.PostId == job.PostId);

                                    if (existing == null) {
                                        var body = CQ.CreateDocument(client.DownloadString(job.Url));

                                        job.Body = body.Find("#postingbody").Html();

                                        database.Jobs.Add(job);
                                    }
                                    else {
                                        existing.Name = job.Name;
                                        existing.LastUpdated = DateTime.Now;
                                    }

                                    database.SaveChanges();
                                }
                            }
                        }
                        catch (Exception ex) {
                            Trace.WriteLine(ex + Environment.NewLine);
                            Thread.Sleep(TimeSpan.FromSeconds(5));
                        }
                    }
                }

                Thread.Sleep(TimeSpan.FromHours(1));
            }
        }

        private void RoleEnvironmentStopping(object sender, RoleEnvironmentStoppingEventArgs e) {
            Stop = true;
        }

        private static bool Stop { get; set; }

        public static void Shuffle<T>(IList<T> list) {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public override bool OnStart() {
            ServicePointManager.DefaultConnectionLimit = 1000;

            return base.OnStart();
        }
    }
}
