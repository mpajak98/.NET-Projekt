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

namespace BibliotekaMultimediow
{
    /// <summary>
    /// Logika interakcji dla klasy DodajUtworWindow.xaml
    /// </summary>
    public partial class DodajUtworWindow : Window
    {
        private BazaDanych db = new BazaDanych();

        private List<string> Roczniki  = new List<string> { "" };

        /// <summary>
        /// Konstruktor okna DodajUtworWindow
        /// </summary>
        /// <remarks>
        /// Inicjalizuje okno oraz inicjalizuje pola z rozwijanymi listami do wpisania informacji o utworze
        /// </remarks>
        public DodajUtworWindow()
        {

            InitializeComponent();

            for (int i = 2020; i > 1900; i--)
                Roczniki.Add(i.ToString());

            WykonawcaComboBox_Init();
            AlbumComboBox_Init();
            RokComboBox_Init();
        }


        /// <summary>
        /// Inicjalizuje listę rozwijaną do wyboru wykonawcy
        /// </summary>
        public void WykonawcaComboBox_Init()
        {
            var result = from w in db.Wykonawcy
                         where w.WykonawcaId != 1
                         orderby w.Nazwa
                         select w.Nazwa;
            
            WykonawcaComboBox.ItemsSource = result.ToArray();
        }
        /// <summary>
        /// Inicjalizuje listę rozwijaną do wyboru albumu
        /// </summary>
        public void AlbumComboBox_Init()
        {
            var result = from a in db.Albumy
                         where a.AlbumId != 1
                         orderby a.Nazwa
                         select a.Nazwa;
            AlbumComboBox.ItemsSource = result.ToArray();
        }

        /// <summary>
        /// Inicjalizuje listę rozwijaną do wyboru roku
        /// </summary>
        public void RokComboBox_Init()
        {
            RokComboBox.ItemsSource = Roczniki;
            RokComboBox.Text = Roczniki.First();
        }

        /// <summary>
        /// Działanie przycisku Cancel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Dialog box canceled
            DialogResult = false;
        }

        /// <summary>
        /// Działanie przeycisku OK
        /// </summary>
        /// <remarks>
        /// Sprawdza poprawność danych i jeśli są poprawne to dodaje utwór do bazy danych
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DodanieUtworu();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            
            // Dialog box accepted
            DialogResult = true;
        }

        /// <summary>
        /// Sprawdza poprawność danych oraz dodaje utwór do bazy danych, jeśli dane poprawne
        /// </summary>
         private void DodanieUtworu()
        {

            string rok = RokComboBox.Text;
            string NazwaWykonawcy = WykonawcaComboBox.Text;
            string NazwaAlbumu = AlbumComboBox.Text;
            string nazwa = NazwaTextBox.Text;
            string pusto = "";
            int wykonawcaID = 1, albumID = 1;
            // nir podano nazwy utworu
            if (nazwa == pusto)
                throw new Exception("Nie podano nazwy");
            else //podano nazwe
            {
                //nazwa jest zajęta
                if (db.Utwory.Any(tmp => tmp.Nazwa == nazwa))
                    throw new Exception("Podana nazwa już istnieje");
                else //podana nazwa jest wolna
                {
                    // podano wszystko
                    if (NazwaWykonawcy != pusto && NazwaAlbumu != pusto && rok != pusto)
                    {
                        //istnieje wykonawca oraz istnieje album
                        if (db.Wykonawcy.Any(tmp => tmp.Nazwa == NazwaWykonawcy) && db.Albumy.Any(tmp => tmp.Nazwa == NazwaAlbumu))
                        {
                            Wykonawca w = (from wyk in db.Wykonawcy
                                           where wyk.Nazwa == WykonawcaComboBox.Text
                                           select wyk).FirstOrDefault();
                            Album a = (from alb in db.Albumy
                                       where alb.Nazwa == AlbumComboBox.Text
                                       select alb).FirstOrDefault();

                            if (w.WykonawcaId == 1 && a.AlbumId == 1)
                            {
                                albumID = a.AlbumId;
                                wykonawcaID = w.WykonawcaId;
                            }
                            if (w.WykonawcaId != 1 && a.AlbumId == 1)
                            {
                                albumID = a.AlbumId;
                                wykonawcaID = w.WykonawcaId;
                            }
                            if (w.WykonawcaId == 1 && a.AlbumId != 1)
                            {
                                foreach (Utwor ut in db.Utwory)
                                {
                                    if (ut.AlbumId == a.AlbumId)
                                    {
                                        ut.WykonawcaId = a.WykonawcaId;
                                        db.Utwory.Update(ut);
                                        db.SaveChanges();
                                    }
                                }

                                albumID = a.AlbumId;
                                wykonawcaID = a.WykonawcaId;
                            }
                            if (w.WykonawcaId != 1 && a.AlbumId != 1)
                            {
                                if (a.WykonawcaId != w.WykonawcaId)
                                    throw new Exception("Album nie zgadza się z wykonawcą");
                                else
                                {
                                    a.Rok = rok;
                                    db.Albumy.Update(a);
                                    db.SaveChanges();

                                    foreach (Utwor ut in db.Utwory)
                                    {
                                        if(ut.AlbumId == a.AlbumId)
                                        {
                                            ut.Rok = rok;
                                            db.Utwory.Update(ut);
                                            db.SaveChanges();
                                        }
                                    }


                                    albumID = a.AlbumId;
                                    wykonawcaID = w.WykonawcaId;



                                }
                            }

                        }
                        // NIE istnieje wykonawca oraz istnieje album
                        if (!db.Wykonawcy.Any(tmp => tmp.Nazwa == NazwaWykonawcy) && db.Albumy.Any(tmp => tmp.Nazwa == NazwaAlbumu))
                        {

                            Wykonawca newWykonawca = new Wykonawca { Nazwa = NazwaWykonawcy };
                            db.Add(newWykonawca);
                            db.SaveChanges();

                            Album a = (from alb in db.Albumy
                                       where alb.Nazwa == AlbumComboBox.Text
                                       select alb).FirstOrDefault();
                            a.WykonawcaId = newWykonawca.WykonawcaId;
                            a.Rok = rok;
                            db.Albumy.Update(a);
                            db.SaveChanges();
                            foreach (Utwor ut in db.Utwory)
                            {
                                if (ut.AlbumId == a.AlbumId)
                                {
                                    ut.WykonawcaId = newWykonawca.WykonawcaId;
                                    ut.Rok = rok;
                                    db.Utwory.Update(ut);
                                    db.SaveChanges();
                                }
                            }

                            albumID = a.AlbumId;
                            wykonawcaID = newWykonawca.WykonawcaId;

                        }
                        // istnieje wykonawca oraz NIE istnieje album
                        if (db.Wykonawcy.Any(tmp => tmp.Nazwa == NazwaWykonawcy) && !db.Albumy.Any(tmp => tmp.Nazwa == NazwaAlbumu))
                        {
                            // utworz album o podanym tytule i przypisz do niego tego wykonawce
                            Wykonawca w = (from wyk in db.Wykonawcy
                                           where wyk.Nazwa == WykonawcaComboBox.Text
                                           select wyk).FirstOrDefault();

                            Album newAlbum = new Album { Nazwa = NazwaAlbumu, Rok = RokComboBox.Text, WykonawcaId = w.WykonawcaId };
                            db.Add(newAlbum);
                            db.SaveChanges();

                            albumID = newAlbum.AlbumId;
                            wykonawcaID = w.WykonawcaId;

                        }
                        // NIE istnieje wykonawca oraz NIE istnieje album
                        if (!db.Wykonawcy.Any(tmp => tmp.Nazwa == NazwaWykonawcy) && !db.Albumy.Any(tmp => tmp.Nazwa == NazwaAlbumu))
                        {
                            Wykonawca newWykonawca = new Wykonawca { Nazwa = NazwaWykonawcy };
                            db.Add(newWykonawca);
                            db.SaveChanges();

                            Album newAlbum = new Album { Nazwa = NazwaAlbumu, Rok = RokComboBox.Text, WykonawcaId = newWykonawca.WykonawcaId };
                            db.Add(newAlbum);
                            db.SaveChanges();

                            albumID = newAlbum.AlbumId;
                            wykonawcaID = newWykonawca.WykonawcaId;
                        }
                    }
                    // NIE podano wykonacy  - podano album  - podano rok
                    if (NazwaWykonawcy == pusto && NazwaAlbumu != pusto && rok != pusto)
                    {
                        // album istnieje
                        if (db.Albumy.Any(tmp => tmp.Nazwa == NazwaAlbumu))
                        {
                            Album a = (from alb in db.Albumy
                                       where alb.Nazwa == AlbumComboBox.Text
                                       select alb).FirstOrDefault();
                            a.Rok = rok;
                            db.Albumy.Update(a);
                            db.SaveChanges();
                            foreach (Utwor ut in db.Utwory)
                            {
                                if (ut.AlbumId == a.AlbumId)
                                {
                                    ut.Rok = rok;
                                    db.Utwory.Update(ut);
                                    db.SaveChanges();
                                }
                            }


                            albumID = a.AlbumId;
                            wykonawcaID = a.WykonawcaId;
                        }
                        //album NIE istnieje
                        if (!db.Albumy.Any(tmp => tmp.Nazwa == NazwaAlbumu))
                        {
                            Album newAlbum = new Album { Nazwa = NazwaAlbumu, Rok = rok, WykonawcaId = 1 };
                            db.Add(newAlbum);
                            db.SaveChanges();

                            albumID = newAlbum.AlbumId;
                            wykonawcaID = 1;
                        }

                    }
                    // podano wykonawce - NIE podano albumu - podano rok
                    if (NazwaWykonawcy != pusto && NazwaAlbumu == pusto && rok != pusto)
                    {
                        // wykonawca istnieje
                        if (db.Wykonawcy.Any(tmp => tmp.Nazwa == NazwaWykonawcy))
                        {
                            Wykonawca w = (from wyk in db.Wykonawcy
                                           where wyk.Nazwa == WykonawcaComboBox.Text
                                           select wyk).FirstOrDefault();

                            albumID = 1;
                            wykonawcaID = w.WykonawcaId;
                        }
                        //wykonawca NIE istnieje
                        if (!db.Wykonawcy.Any(tmp => tmp.Nazwa == NazwaWykonawcy))
                        {
                            Wykonawca newWykonawca = new Wykonawca { Nazwa = NazwaWykonawcy };
                            db.Add(newWykonawca);
                            db.SaveChanges();

                            albumID = 1;
                            wykonawcaID = newWykonawca.WykonawcaId;
                        }
                    }
                    // podano wykonawce - podano album - NIE podano roku
                    if (NazwaWykonawcy != pusto && NazwaAlbumu != pusto && rok == pusto)
                    {
                        //istnieje wykonawca oraz istnieje album
                        if (db.Wykonawcy.Any(tmp => tmp.Nazwa == NazwaWykonawcy) && db.Albumy.Any(tmp => tmp.Nazwa == NazwaAlbumu))
                        {
                            Wykonawca w = (from wyk in db.Wykonawcy
                                           where wyk.Nazwa == WykonawcaComboBox.Text
                                           select wyk).FirstOrDefault();
                            Album a = (from alb in db.Albumy
                                       where alb.Nazwa == AlbumComboBox.Text
                                       select alb).FirstOrDefault();
                            if (w.WykonawcaId == 1 && a.AlbumId == 1)
                            {
                                albumID = a.AlbumId;
                                wykonawcaID = w.WykonawcaId;
                            }
                            if (w.WykonawcaId != 1 && a.AlbumId == 1)
                            {
                                albumID = a.AlbumId;
                                wykonawcaID = w.WykonawcaId;
                            }
                            if (w.WykonawcaId == 1 && a.AlbumId != 1)
                            {
                                albumID = a.AlbumId;
                                wykonawcaID = a.WykonawcaId;
                            }
                            if (w.WykonawcaId != 1 && a.AlbumId != 1)
                            {
                                if (a.WykonawcaId != w.WykonawcaId)
                                    throw new Exception("Album nie zgadza się z wykonawcą");
                                else
                                {
                                    albumID = a.AlbumId;
                                    wykonawcaID = w.WykonawcaId;
                                }
                            }
                        }
                        // NIE istnieje wykonawca oraz istnieje album
                        if (!db.Wykonawcy.Any(tmp => tmp.Nazwa == NazwaWykonawcy) && db.Albumy.Any(tmp => tmp.Nazwa == NazwaAlbumu))
                        {
                            Wykonawca newWykonawca = new Wykonawca { Nazwa = NazwaWykonawcy };
                            db.Add(newWykonawca);
                            db.SaveChanges();

                            Album a = (from alb in db.Albumy
                                       where alb.Nazwa == AlbumComboBox.Text
                                       select alb).FirstOrDefault();
                            a.WykonawcaId = newWykonawca.WykonawcaId;
                            db.Albumy.Update(a);
                            db.SaveChanges();
                            foreach (Utwor ut in db.Utwory)
                            {
                                if (ut.AlbumId == a.AlbumId)
                                {
                                    ut.WykonawcaId = newWykonawca.WykonawcaId;
                                    db.Utwory.Update(ut);
                                    db.SaveChanges();
                                }
                            }

                            albumID = a.AlbumId;
                            wykonawcaID = newWykonawca.WykonawcaId;

                        }
                        // istnieje wykonawca oraz NIE istnieje album
                        if (db.Wykonawcy.Any(tmp => tmp.Nazwa == NazwaWykonawcy) && !db.Albumy.Any(tmp => tmp.Nazwa == NazwaAlbumu))
                        {
                            // utworz album o podanym tytule i przypisz do niego tego wykonawce
                            Wykonawca w = (from wyk in db.Wykonawcy
                                           where wyk.Nazwa == WykonawcaComboBox.Text
                                           select wyk).FirstOrDefault();

                            Album newAlbum = new Album { Nazwa = NazwaAlbumu, Rok = "nieznany", WykonawcaId = w.WykonawcaId };
                            db.Add(newAlbum);
                            db.SaveChanges();

                            albumID = newAlbum.AlbumId;
                            wykonawcaID = w.WykonawcaId;

                        }
                        // NIE istnieje wykonawca oraz NIE istnieje album
                        if (!db.Wykonawcy.Any(tmp => tmp.Nazwa == NazwaWykonawcy) && !db.Albumy.Any(tmp => tmp.Nazwa == NazwaAlbumu))
                        {
                            Wykonawca newWykonawca = new Wykonawca { Nazwa = NazwaWykonawcy };
                            db.Add(newWykonawca);
                            db.SaveChanges();

                            Album newAlbum = new Album { Nazwa = NazwaAlbumu, Rok = "nieznany", WykonawcaId = newWykonawca.WykonawcaId };
                            db.Add(newAlbum);
                            db.SaveChanges();

                            albumID = newAlbum.AlbumId;
                            wykonawcaID = newWykonawca.WykonawcaId;
                        }
                    }
                    // NIE podano wykonawcy - NIE podano albumu - podano rok
                    if (NazwaWykonawcy == pusto && NazwaAlbumu == pusto && rok != pusto)
                    {
                        albumID = 1;
                        wykonawcaID = 1;
                    }
                    // podano wykonawce - NIE podano albumu - NIE podano roku
                    if (NazwaWykonawcy != pusto && NazwaAlbumu == pusto && rok == pusto)
                    {
                        if (db.Wykonawcy.Any(tmp => tmp.Nazwa == NazwaWykonawcy))
                        {
                            Wykonawca w = (from wyk in db.Wykonawcy
                                           where wyk.Nazwa == WykonawcaComboBox.Text
                                           select wyk).FirstOrDefault();
                            albumID = 1;
                            wykonawcaID = w.WykonawcaId;
                        }
                        if (!db.Wykonawcy.Any(tmp => tmp.Nazwa == NazwaWykonawcy))
                        {
                            Wykonawca newWykonawca = new Wykonawca { Nazwa = NazwaWykonawcy };
                            db.Add(newWykonawca);
                            db.SaveChanges();

                            albumID = 1;
                            wykonawcaID = newWykonawca.WykonawcaId;
                        }
                    }
                    if (NazwaWykonawcy == pusto && NazwaAlbumu != pusto && rok == pusto)
                    {
                        if (db.Albumy.Any(tmp => tmp.Nazwa == NazwaAlbumu))
                        {
                            Album a = (from alb in db.Albumy
                                       where alb.Nazwa == AlbumComboBox.Text
                                       select alb).FirstOrDefault();
                            albumID = a.AlbumId;
                            wykonawcaID = a.WykonawcaId;
                        }
                        if (!db.Albumy.Any(tmp => tmp.Nazwa == NazwaAlbumu))
                        {
                            Album newAlbum = new Album { Nazwa = NazwaAlbumu, Rok = "nieznany", WykonawcaId = 1 };
                            db.Add(newAlbum);
                            db.SaveChanges();

                            albumID = newAlbum.AlbumId;
                            wykonawcaID = 1;
                        }
                    }
                    if (NazwaWykonawcy == pusto && NazwaAlbumu == pusto && rok == pusto)
                    {
                        albumID = 1;
                        wykonawcaID = 1;
                    }


                }

            }

            Utwor u;
            if (rok == pusto)
                u = new Utwor { Nazwa = NazwaTextBox.Text, WykonawcaId = wykonawcaID, AlbumId = albumID, Rok = "nieznany", CzyUlubione = UlubioneCheckBox.IsChecked.Value, DataDodania = DateTime.Now, UrlPath = UrlTextBox.Text };
            else
                u = new Utwor { Nazwa = NazwaTextBox.Text, WykonawcaId = wykonawcaID, AlbumId = albumID, Rok = RokComboBox.Text, CzyUlubione = UlubioneCheckBox.IsChecked.Value, DataDodania = DateTime.Now, UrlPath = UrlTextBox.Text };


            db.Add(u);
            db.SaveChanges();


            }

        /// <summary>
        /// Reakcja na zmianę tekstu w polu Wykonawca
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WykonawcaComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Wykonawca w = (from wyk in db.Wykonawcy
                           where wyk.Nazwa == WykonawcaComboBox.Text
                           select wyk).FirstOrDefault();
            if (w != null)
            {

                var result = from a in db.Albumy
                             where a.WykonawcaId == w.WykonawcaId
                             where a.WykonawcaId != 1
                             orderby a.Nazwa
                             select a.Nazwa;
                AlbumComboBox.ItemsSource = result.ToArray();
                AlbumComboBox.Text = result.FirstOrDefault();

            }
            else
            {
                var result = from a in db.Albumy
                             where a.WykonawcaId != 1
                             orderby a.Nazwa
                             select a.Nazwa;
                AlbumComboBox.ItemsSource = result.ToArray();
            }
        }

        /// <summary>
        /// Reakcja na zmianę tekstu w polu Album
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AlbumComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Album a = (from alb in db.Albumy
                       where alb.Nazwa == AlbumComboBox.Text
                       select alb).FirstOrDefault();
            if (a != null)
            {
                if(a.WykonawcaId != 1)
                {
                    var result = from w in db.Wykonawcy
                                 where w.WykonawcaId != 1
                                 where w.WykonawcaId == a.WykonawcaId
                                 select w.Nazwa;

                    WykonawcaComboBox.ItemsSource = result.ToList();
                    WykonawcaComboBox.Text = result.FirstOrDefault();
                }

                if(a.Rok == "nieznany")
                {
                    RokComboBox.ItemsSource = Roczniki;
                }
                else 
                {
                    RokComboBox.ItemsSource = new List<string> { a.Rok };
                    RokComboBox.Text = a.Rok;
                }
               
            }
            else
            {
                var result = from w in db.Wykonawcy
                             where w.WykonawcaId != 1
                             orderby w.Nazwa
                             select w.Nazwa;

                WykonawcaComboBox.ItemsSource = result.ToList();
                


                RokComboBox.ItemsSource = Roczniki.ToList();
                RokComboBox.Text = Roczniki.First();
            }
        }
    }
}
