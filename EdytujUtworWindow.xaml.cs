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
    /// Logika interakcji dla klasy EdytujUtworWindow.xaml
    /// </summary>
    public partial class EdytujUtworWindow : Window
    {
        private int idEdytowanegoUtworu;
        private BazaDanych db = new BazaDanych();
        private List<string> Roczniki = new List<string>();
        public EdytujUtworWindow(int id)
        {
            InitializeComponent();
            WykonawcaComboBox_Init();
            AlbumComboBox_Init();
            RokComboBox_Init();

            idEdytowanegoUtworu = id;
            SetStartValues();
        }
           
        private void RokComboBox_Init()
        {
            Roczniki.Add("nieznany");
            for (int i = 2020; i > 1900; i--)
                Roczniki.Add(i.ToString());
            RokComboBox.ItemsSource = Roczniki;
        }

        public void SetStartValues()
        {
            Utwor u = db.Utwory.Find(idEdytowanegoUtworu);
            Wykonawca w = db.Wykonawcy.Find(u.WykonawcaId);
            Album a= db.Albumy.Find(u.AlbumId);
            NazwaTextBox.Text = u.Nazwa;
            NazwaTextBox.TextChanged += NazwaTextBox_TextChanged;

            WykonawcaComboBox.Text = w.Nazwa;
            WykonawcaComboBox.SelectionChanged += WykonawcaComboBox_SelectionChanged;

            AlbumComboBox.Text = a.Nazwa;
            AlbumComboBox.SelectionChanged += AlbumComboBox_SelectionChanged;

            RokComboBox.Text = a.Rok;
            RokComboBox.SelectionChanged += RokComboBox_SelectionChanged;

            UrlTextBox.Text = u.UrlPath;
            UrlTextBox.TextChanged += UrlTextBox_TextChanged;

            UlubioneCheckBox.IsChecked = u.CzyUlubione;
        }


        public void WykonawcaComboBox_Init()
        {
            var result = from w in db.Wykonawcy
                         orderby w.Nazwa
                         select w.Nazwa;
            WykonawcaComboBox.ItemsSource = result.ToArray();
        }

        public void AlbumComboBox_Init()
        {
            var result = from a in db.Albumy
                         orderby a.Nazwa
                         select a.Nazwa;
            AlbumComboBox.ItemsSource = result.ToArray();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Dialog box canceled
            DialogResult = false;
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SprawdzeniePoprawnosci();
                AktualizacjaUtworu();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }


            // Dialog box accepted
            DialogResult = true;
        }

        private void AktualizacjaUtworu()
        {
            int WykonawcaId = 1, AlbumId = 1;
            if (NazwaTextBox.Text != "")
            {

                WykonawcaId = IsWykonawcaValid(WykonawcaComboBox.Text);
                AlbumId = IsAlbumValid(WykonawcaComboBox.Text, AlbumComboBox.Text);

                Utwor u = db.Utwory.Find(idEdytowanegoUtworu);
                u.Nazwa = NazwaTextBox.Text;
                u.WykonawcaId = WykonawcaId;
                u.AlbumId = AlbumId;
                u.Rok = RokComboBox.Text;
                u.UrlPath = UrlTextBox.Text;
                u.CzyUlubione = UlubioneCheckBox.IsChecked.Value;
                
                db.Utwory.Update(u);
                db.SaveChanges();
            }

        }

        public int IsAlbumValid(string NazwaWykonawcy, string NazwaAlbumu)
        {
            string pusto = "";
            if (NazwaAlbumu != pusto)
            {
                Album album = (from a in db.Albumy
                               where a.Nazwa == NazwaAlbumu
                               select a).FirstOrDefault();
                if (album == null) // albumu nie ma w bazie
                {
                    if (NazwaWykonawcy == pusto) // nie podano wykonawcy
                    {
                        Album newAlbum = new Album { Nazwa = NazwaAlbumu, Rok = RokComboBox.Text, WykonawcaId = 1 };
                        db.Add(newAlbum);
                        db.SaveChanges();
                        return newAlbum.AlbumId;
                    }
                    else // podano wykonawce
                    {
                        Wykonawca wykonawca =
                        (from wyk in db.Wykonawcy
                         where wyk.Nazwa == NazwaWykonawcy
                         select wyk).FirstOrDefault();

                        Album newAlbum = new Album { Nazwa = NazwaAlbumu, Rok = RokComboBox.Text, WykonawcaId = wykonawca.WykonawcaId };
                        db.Add(newAlbum);
                        db.SaveChanges();
                        return newAlbum.AlbumId;
                    }
                }
                else // album jest w bazie
                {
                    if (NazwaWykonawcy == pusto) // nie podano wykonawcy
                    {
                        return album.AlbumId;
                    }
                    else // podano wykonawce
                    {
                        Wykonawca wykonawca =
                        (from wyk in db.Wykonawcy
                         where wyk.Nazwa == NazwaWykonawcy
                         select wyk).FirstOrDefault();

                        if (wykonawca.WykonawcaId == 1)
                            album.WykonawcaId = wykonawca.WykonawcaId;
                        return album.AlbumId;
                    }
                }
            }
            return 1;
        }

        public int IsWykonawcaValid(string NazwaWykonawcy)
        {
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
                    return newWykonawca.WykonawcaId;
                }
                else // wykonawca jest w bazie
                    return wykonawca.WykonawcaId;
            }
            return 1;
        }

        private void SprawdzeniePoprawnosci()
        {
            Wykonawca w = (from wyk in db.Wykonawcy
                           where wyk.Nazwa == WykonawcaComboBox.Text
                           select wyk).FirstOrDefault();
            Album a = (from alb in db.Albumy
                       where alb.Nazwa == AlbumComboBox.Text
                       select alb).FirstOrDefault();
            Utwor u = (from utw in db.Utwory
                       where utw.Nazwa == NazwaTextBox.Text
                       select utw).FirstOrDefault();
            string rok = RokComboBox.Text;
            string wykonawca = WykonawcaComboBox.Text;
            string album = AlbumComboBox.Text;
            string nazwa = NazwaTextBox.Text;

            if (nazwa == "")
                throw new Exception("Nie podano nazwy");
            if (a != null && rok != "")
                if (a.Rok != rok)
                    throw new Exception("Album już istnieje i ma inny rok wydania");
            if (a != null && w.Nazwa != "")
                if (a.WykonawcaId != w.WykonawcaId)
                    throw new Exception("Album nie zgadza się z wykonawcą");
            if (a != null && rok != "" && w != null)
            {
                if (a.Rok != rok)
                    throw new Exception("Album już istnieje i ma inny rok wydania");
                if (a.WykonawcaId != w.WykonawcaId)
                    throw new Exception("Album nie zgadza się z wykonawcą");
            }

        }

        private void NazwaTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            NazwaTextBox.Background = Brushes.Yellow;

            NazwaLabel.Content = "Nazwa:*";
        }

        private void RokComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RokLabel.Content = "Rok:*";
        }

        private void AlbumComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            


            Album a = (from alb in db.Albumy
                       where alb.Nazwa == AlbumComboBox.Text
                       select alb).FirstOrDefault();
            if (a != null)
            {
                RokComboBox.ItemsSource = new List<string> { a.Rok };
                RokComboBox.Text = a.Rok;
            }
            else
            {
                RokComboBox.ItemsSource = Roczniki.ToList();
                RokComboBox.Text = Roczniki.First();
            }


            AlbumComboBox.Background = Brushes.Yellow;
            AlbumComboBox.BorderBrush = Brushes.Yellow;

            AlbumLabel.Content = "Album:*";
        }

        private void WykonawcaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Wykonawca w = (from wyk in db.Wykonawcy
                           where wyk.Nazwa == WykonawcaComboBox.Text
                           select wyk).FirstOrDefault();
            if (w != null)
            {
                var result = from a in db.Albumy
                             where a.WykonawcaId == w.WykonawcaId
                             orderby a.Nazwa
                             select a.Nazwa;
                AlbumComboBox.ItemsSource = result.ToArray();
                AlbumComboBox.Text = result.FirstOrDefault();

            }
            else
            {
                var result = from a in db.Albumy
                             orderby a.Nazwa
                             select a.Nazwa;
                AlbumComboBox.ItemsSource = result.ToArray();
                AlbumComboBox.Text = result.FirstOrDefault();
            }

            WykonawcaComboBox.Background = Brushes.Yellow;
            WykonawcaComboBox.BorderBrush = Brushes.Yellow;

            WykonawcaLabel.Content = "Wykonawcy:*";

        }

        private void UrlTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UrlTextBox.Background = Brushes.Yellow;
            UrlLabel.Content = "Link:*";
        }
      
    }

}
