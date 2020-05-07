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
using System.Net;
using Image = System.Drawing.Image;

namespace BibliotekaMultimediow
{
    /// <summary>
    /// Logika interakcji dla klasy SzukajYT.xaml
    /// </summary>
    public partial class SzukajYT : Window
    {
        private BazaDanych db = new BazaDanych();
        private readonly string APIKEY = "AIzaSyCWlMH7Y1bGSh1efU2Pef8dhWo7_WjpZuY";
        public List<Wynik> Videos;
        public SzukajYT()
        {
            InitializeComponent();
        }

        public struct Wynik
        {
            public DateTime publishedAt { get; set; }
            public string date { get; set; }
            public string Title { get; set; }
            public string ChannelTitle { get; set; }
            public string Description { get; set; }
            public string Url { get; set; }
             
            public Image Thumbnail { get; set; }

            public Wynik(SearchResultSnippet snip, ResourceId id)
            {
                Title = snip.Title;
                publishedAt = snip.PublishedAt.Value.Date;
                date = snip.PublishedAt.Value.Date.ToString("d");
                ChannelTitle = snip.ChannelTitle;
                Description = snip.Description;
                Url = "https://www.youtube.com/watch?v=" + id.VideoId;
                Thumbnail = GetImageFromUrl(snip.Thumbnails.Standard.Url);
            }

        }
        /// <summary>
        /// Searching Api Class
        /// </summary>



        private async Task Run(string ObjectName, int MaxResults)
        {
            
            try
            {
                YouTubeService youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = APIKEY,
                    ApplicationName = this.GetType().ToString()
                });

                var searchListRequest = youtubeService.Search.List("snippet");
                searchListRequest.Type = "video";
                searchListRequest.Q = ObjectName; /// Replace with your search term.
                searchListRequest.MaxResults = MaxResults;

                /// Call the search.list method to retrieve results matching the specified query term.
                var searchListResponse = await searchListRequest.ExecuteAsync();

                /// Add each result to the appropriate list
                foreach (var searchResult in searchListResponse.Items)
                    this.Videos.Add(new Wynik(searchResult.Snippet, searchResult.Id));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            

        }

        public static BitmapImage GetFileFromUrl(string url)
        {
            System.Net.WebRequest request =
                  System.Net.WebRequest.Create(
                  url);
            System.Net.WebResponse response = request.GetResponse();
            Stream responseStream =
                response.GetResponseStream();
            string filename = "tmp.bmp";  // System.IO.Path.GetTempFileName();
            Bitmap bitmap = new Bitmap(responseStream);
            bitmap.Save(filename);
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(filename);
            bitmapImage.EndInit();
            return bitmapImage;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }


        public async Task<bool> Funko()
        {
            try
            {
                string k = SzukajTextBox.Text;
                return await Task.Run(() =>
                {
                    Run(k, 25).Wait();
                    return true;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return true;
        }

        private void PokazOpis(object sender, RoutedEventArgs e)
        {
            int i = Wyniki.SelectedIndex;
            MessageBox.Show(Videos[i].Description);
        }

        /// <summary>
        ///  Add to library button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Dodaj(object sender, RoutedEventArgs e)
        {
            int i = Wyniki.SelectedIndex;
            string NazwaWykonawcy = Videos[i].ChannelTitle;
            int wykonawcaId = 1;
            string pusto = "";
            if (NazwaWykonawcy != pusto)
            {
                Wykonawca wykonawca = (from w in db.Wykonawcy
                                       where w.Nazwa == NazwaWykonawcy
                                       select w).FirstOrDefault();
                if (wykonawca == null) //Wykonawcy nie ma w bazie
                {
                    Wykonawca newWykonawca = new Wykonawca { Nazwa = NazwaWykonawcy };
                    db.Add(newWykonawca);
                    db.SaveChanges();
                    wykonawcaId = newWykonawca.WykonawcaId;
                }
                else // wykonawca jest w bazie
                    wykonawcaId = wykonawca.WykonawcaId;


                Utwor u = new Utwor
                {
                    Nazwa = Videos[i].Title,
                    WykonawcaId = wykonawcaId,
                    AlbumId = 1,
                    Rok = Videos[i].publishedAt.Year.ToString(),
                    CzyUlubione = false,
                    DataDodania = DateTime.Now,
                    UrlPath = Videos[i].Url
                };

                db.Add(u);
                db.SaveChanges();
                MessageBox.Show("Utwór dodany do biblioteki");
            }
        }

        private async void SzukajButton_Click(object sender, RoutedEventArgs e)
        {
            Videos = new List<Wynik>();
            string k = SzukajTextBox.Text;
            await Run(k, 25);

            Wyniki.ItemsSource = Videos; 
        }


        public static Image GetImageFromUrl(string url)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            // if you have proxy server, you may need to set proxy details like below 
            //httpWebRequest.Proxy = new WebProxy("proxyserver",port){ Credentials = new NetworkCredential(){ UserName ="uname", Password = "pw"}};

            using (HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (Stream stream = httpWebReponse.GetResponseStream())
                {
                    return Image.FromStream(stream);
                }
            }
        }


    }

}
