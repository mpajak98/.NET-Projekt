using Microsoft.EntityFrameworkCore;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

/* Do zrobienia:
 *  - dodaj wpis - mechanika i okienko
 *  - dodaj utwór z YT - mech i okno
 *  - edycja wpisu
 *  - usuwanie wpisów
 *  
 *  
 *  - dokumentacja
*/

namespace BibliotekaMultimediow
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public enum CurrentView { utwory, albumy, wykonawcy}

    public partial class MainWindow : Window
    {
        private BazaDanych db = new BazaDanych();
        private CurrentView cv;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Show_Utwory();
            cv = CurrentView.utwory;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            this.db.Dispose();
        }

        private void Button_Click_Utwory(object sender, RoutedEventArgs e)
        {
            Show_Utwory();

            cv = CurrentView.utwory;
        }

        private void Button_Click_Album(object sender, RoutedEventArgs e)
        {
           Show_Albumy();
           cv = CurrentView.albumy;
        }

        private void Button_Click_Wykonawca(object sender, RoutedEventArgs e)
        {
            Show_Wykonawcy();

            cv = CurrentView.wykonawcy;
        }

        private void Button_Click_Dodaj(object sender, RoutedEventArgs e)
        {
            switch (cv)
            {
                case CurrentView.utwory:
                    {
                        DodajUtworWindow win = new DodajUtworWindow();
                        win.ShowDialog();
                        Show_Utwory();
                        break;
                    }
                case CurrentView.albumy:
                    {
                        DodajAlbumWindow win = new DodajAlbumWindow();
                        win.ShowDialog();
                        Show_Albumy();
                        break;
                    }
                case CurrentView.wykonawcy:
                    {
                        DodajWykonawceWindow win = new DodajWykonawceWindow();
                        win.ShowDialog();
                        Show_Wykonawcy();
                        break;
                    }

            }
            
        }

        private void Button_Click_DodajYT(object sender, RoutedEventArgs e)
        {
            DodajUtworYTWindow win = new DodajUtworYTWindow();
            win.ShowDialog();
            Show_Utwory();
                   
        }


        private void Button_Click_Edytuj(object sender, RoutedEventArgs e)
        {
            
        }

        private void Show_Utwory()
        {
           var query =
           from plik in db.Muzyka
           orderby plik.Nazwa
           select new { Tytul = plik.Nazwa, wykonawca = plik.WykonawcaId, plik.CzasTrwania, album = plik.AlbumId, plik.DataDodania };

           dataGrid1.ItemsSource = query.ToList();

        }

        private void Show_Albumy()
        {
            var query =
            from plik in db.Albumy
            orderby plik.Nazwa
            select new { plik.Nazwa };

            dataGrid1.ItemsSource = query.ToList();

        }

        private void Show_Wykonawcy()
        {
            var query =
            from plik in db.Wykonawcy
            orderby plik.Nazwa
            select new { plik.Nazwa };

            dataGrid1.ItemsSource = query.ToList();

        }

    }
}
