using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Reflection;
using System.Threading;
using MessageBox = System.Windows.MessageBox;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.Drawing;
using Brushes = System.Drawing.Brushes;

namespace BibliotekaMultimediow
{
    /// <summary>
    /// Logika interakcji dla klasy SzukajYT.xaml
    /// </summary>
    public partial class SzukajYT : Window
    {
        public SzukajYT()
        {
            InitializeComponent();
        }

        public struct Output
        {
            public SearchResultSnippet Info;
            public ResourceId Id;

            public Output(SearchResultSnippet snip, ResourceId id)
            {
                Info = snip;
                Id = id;
            }

            public string Name() { return Info.Title; }
        }
        /// <summary>
        /// Searching Api Class
        /// </summary>
        public class Searcher
        {
            public List<Output> Videos { get; set; }
            public List<Output> Playlists { get; set; }
            public List<Output> Channels { get; set; }
            protected bool IsGood { get; set; }
            protected YouTubeService youtubeService;
            public bool Good() { return IsGood; }
            public void Search(string ObjectName, int MaxResults)
            {
                this.Videos = new List<Output>();
                this.Channels = new List<Output>();
                this.Playlists = new List<Output>();
                try
                {
                    if (Good() == true) { this.Run(ObjectName, MaxResults).Wait(); }
                }
                catch (AggregateException ex)
                {
                    IsGood = false;
                    foreach (var e in ex.InnerExceptions)
                    {
                        Console.WriteLine("API Error: " + e.Message);
                    }
                }
            }
            public Searcher(string APIKEY)
            {
                IsGood = true;
                try
                {
                    youtubeService = new YouTubeService(new BaseClientService.Initializer()
                    {
                        ApiKey = APIKEY,
                        ApplicationName = this.GetType().ToString()
                    });
                }
                catch (AggregateException ex)
                {
                    IsGood = false;
                    foreach (var e in ex.InnerExceptions)
                    {
                        Console.WriteLine("API Error: " + e.Message);
                    }
                }
            }

            private async Task Run(string ObjectName, int MaxResults)
            {
                var searchListRequest = youtubeService.Search.List("snippet");
                searchListRequest.Q = ObjectName; /// Replace with your search term.
                searchListRequest.MaxResults = MaxResults;

                /// Call the search.list method to retrieve results matching the specified query term.
                var searchListResponse = await searchListRequest.ExecuteAsync();

                /// Add each result to the appropriate list
                foreach (var searchResult in searchListResponse.Items)
                {
                    switch (searchResult.Id.Kind)
                    {
                        case "youtube#video":
                            this.Videos.Add(new Output(searchResult.Snippet, searchResult.Id));
                            break;

                        case "youtube#channel":
                            this.Channels.Add(new Output(searchResult.Snippet, searchResult.Id));
                            break;

                        case "youtube#playlist":
                            this.Playlists.Add(new Output(searchResult.Snippet, searchResult.Id));
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// Searching from Textbox task

        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public async Task<List<string>> Funko(int a)
        {
            string k = SzukajkaTB.Text;
            return await Task.Run(() =>
            {

                Searcher tmp = new Searcher("AIzaSyCWlMH7Y1bGSh1efU2Pef8dhWo7_WjpZuY");
                tmp.Search(k, 10);

                //tmp.Videos[0].Info.Title
                // string url = tmp.Videos[a].Info.Thumbnails.High.Url +"\n" + tmp.Videos[a].Info.Title + "\n" + tmp.Videos[a].Info.Description + "\n" + "https://www.youtube.com/watch?v=" + tmp.Videos[a].Id.VideoId + "\n";
                List<string> url = new List<string>();
                ///List making an access to informations from api.
                url.Add(tmp.Videos[a].Info.Thumbnails.High.Url);
                url.Add(tmp.Videos[a].Info.Title);
                url.Add(tmp.Videos[a].Info.Description);
                url.Add("https://www.youtube.com/watch?v=" + tmp.Videos[a].Id.VideoId);
                return url;
            });


        }
        /// <summary>
        ///  Function saving thumbnails from API.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Bitmap GetFileFromUrl(string url, string filename)
        {
            System.Net.WebRequest request =
                  System.Net.WebRequest.Create(
                  url);
            System.Net.WebResponse response = request.GetResponse();
            System.IO.Stream responseStream =
                response.GetResponseStream();
            Bitmap bitmap2 = new Bitmap(responseStream);
            bitmap2.Save(filename);

            return bitmap2;
        }
        /// <summary>
        /// Future function to save our best search results in library
        /// </summary>
        /// <param name="Song1"></param>
        
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

        /// <summary>
        ///  Search button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SzukajkaClick(object sender, RoutedEventArgs e)
        {
            ZnajdzkaTB.Background = System.Windows.Media.Brushes.White;
            ZnajdzkaTB2.Background = System.Windows.Media.Brushes.White;
            ZnajdzkaTB3.Background = System.Windows.Media.Brushes.White;
            Baddon1.Background = System.Windows.Media.Brushes.Yellow;
            Baddon2.Background = System.Windows.Media.Brushes.Yellow;
            Baddon3.Background = System.Windows.Media.Brushes.Yellow;
            List<string> Dzialaj1 = await Funko(0);
            List<string> Dzialaj2 = await Funko(1);
            List<string> Dzialaj3 = await Funko(2);
            ZnajdzkaTB.Text = Dzialaj1[1] + "\n" + Dzialaj1[2] + "\n" + Dzialaj1[3];
            ZnajdzkaTB2.Text = Dzialaj2[1] + "\n" + Dzialaj2[2] + "\n" + Dzialaj3[3];
            ZnajdzkaTB3.Text = Dzialaj3[1] + "\n" + Dzialaj2[2] + "\n" + Dzialaj3[3];
        }
        /// <summary>
        /// Open Library button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BibliotekaClick(object sender, RoutedEventArgs e)
        {

        }
        /// <summary>
        ///  Add to library button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BAdd(object sender, RoutedEventArgs e)
        {
            
        }
    
    }
}
