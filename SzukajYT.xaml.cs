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
        private readonly string APIKEY = "AIzaSyA0RPp6XRV_WcHJQn3gtUyZVdIJ7Qpk74A";

        public List<Wynik> Videos;

        /// <summary>
        /// Konstruktor okna SzukajYT
        /// </summary>
        public SzukajYT()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Klasa reprezentująca pojedynczy wynik wyszukiwania
        /// </summary>
        public class Wynik
        {
            /// <summary>
            /// Pełna data i godzina dodania video do serwisu YouTube
            /// </summary>
            public DateTime publishedAt { get; set; }
            /// <summary>
            /// Sama data dodania video do serwisu YouTube
            /// </summary>
            public string date { get; set; }
            /// <summary>
            /// Tytuł video
            /// </summary>
            public string Title { get; set; }
            /// <summary>
            /// Nazwa kanału
            /// </summary>
            public string ChannelTitle { get; set; }
            /// <summary>
            /// Opis video
            /// </summary>
            public string Description { get; set; }
            /// <summary>
            /// Link do video
            /// </summary>
            public string Url { get; set; }
             
            /// <summary>
            /// Konstruktor
            /// </summary>
            /// <param name="snip">Objekt przechowujący podstawowe dane o video</param>
            /// <param name="id">ID video</param>
            public Wynik(SearchResultSnippet snip, ResourceId id)
            {
                Title = snip.Title;
                publishedAt = snip.PublishedAt.Value.Date;
                date = snip.PublishedAt.Value.Date.ToString("d");
                ChannelTitle = snip.ChannelTitle;
                Description = snip.Description;
                Url = "https://www.youtube.com/watch?v=" + id.VideoId;
            }
        }

        /// <summary>
        /// Wyszukiwarka w serwisie YouTube
        /// </summary>
        /// <remarks>Wyniki wyszukiwania są dodawane do listy Videos </remarks>
        /// <param name="ObjectName">Nazwa do wyszukania</param>
        /// <param name="MaxResults">Maksymalna liczba wyników wyszukiwania</param>
        /// <returns></returns>
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
                searchListRequest.Q = ObjectName; // Replace with your search term.
                searchListRequest.MaxResults = MaxResults;

                // Call the search.list method to retrieve results matching the specified query term.
                var searchListResponse = await searchListRequest.ExecuteAsync();

                // Add each result to the appropriate list
                foreach (var searchResult in searchListResponse.Items)
                    this.Videos.Add(new Wynik(searchResult.Snippet, searchResult.Id));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Działanie przycisku Dodaj
        /// </summary>
        /// <remarks>Dodaje wybrany utwór do bazy danych</remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DodajButton_Click(object sender, RoutedEventArgs e)
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

        /// <summary>
        /// Działanie przycisku Szukaj
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
