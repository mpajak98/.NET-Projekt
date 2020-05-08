using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections;
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
using System.Data;
using System.Windows.Controls.Primitives;
using System.Net;


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
    public partial class MainWindow : Window
    {
        private BazaDanych db = new BazaDanych();


        /// <summary>
        /// Konstruktor okna MainWindow
        /// </summary>
        /// <remarks>
        /// Inicjalizuje okno oraz wyświetla domyślną widok tabeli utworów
        /// </remarks>
        public MainWindow()
        {
            InitializeComponent(); // Inicjalizacja komponentu
            UtworyColumns_Create(); // Stworzenie kolumn siatki dla utworów 
            Utwory_Refresh(); // Wyswietlenie utworów z bazy danych
        }

        /// <summary>
        /// Funkcja wywoływana przez kliknięcie przycisku Utwory
        /// </summary>
        /// <remarks>
        /// Tworzy domyślne kolumny do wyświetlenia utworów oraz je wyświetla
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UtworyButton_Click(object sender, RoutedEventArgs e)
        {
            UtworyColumns_Create(); // Stworzenie kolumn siatki dla utworów 
            Utwory_Refresh(); // Wyswietlenie utworów z bazy danych
        }

        /// <summary>
        /// Funkcja wywoływana przez kliknięcie przycisku Albumy
        /// </summary>
        /// <remarks>
        /// Tworzy domyślne kolumny do wyświetlenia albumów oraz je wyświetla
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AlbumyButton_Click(object sender, RoutedEventArgs e)
        {
            AlbumyColumns_Create(); // Stworzenie kolumn siatki dla albumów
            Albumy_Refresh(); // Wyswietlenie albumów z bazy danych
            //cv = CurrentView.albumy;
        }

        /// <summary>
        /// Funkcja wywoływana przez kliknięcie przycisku Wykonawcy
        /// </summary>
        /// <remarks>
        /// Tworzy domyślne kolumny do wyświetlenia wykonawców oraz je wyświetla
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WykonawcyButton_Click(object sender, RoutedEventArgs e)
        {
            WykonawcyColumns_Create(); // Stworzenie kolumn siatki dla wykonawców
            Wykonawcy_Refresh(); // Wyswietlenie wykonawców z bazy danych
            //cv = CurrentView.wykonawcy;
        }

        /// <summary>
        /// Funkcja wywoływana przez kliknięcie przycisku Ulubione
        /// </summary>
        /// <remarks>
        /// Tworzy domyślne kolumny do wyświetlenia utworów oraz wyświetla te które są zaznaczone jako ulubione
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UlubioneButton_Click(object sender, RoutedEventArgs e)
        {
            UtworyColumns_Create(); // Stworzenie kolumn siatki dla wykonawców
            var query =
            from plik in db.Utwory
            join wyk in db.Wykonawcy on plik.WykonawcaId equals wyk.WykonawcaId
            join alb in db.Albumy on plik.AlbumId equals alb.AlbumId
            where plik.CzyUlubione == true
            orderby plik.Nazwa
            select new { UtworId = plik.UtworId, Tytul = plik.Nazwa, Wykonawca = wyk.Nazwa, Album = alb.Nazwa, Rok = plik.Rok, Data = plik.DataDodania, Ulubione = plik.CzyUlubione, URL = plik.UrlPath };

            MainWindowGrid.ItemsSource = query.ToList();
        }

        /// <summary>
        /// Funkcja wywoływana przez kliknięcie przycisku Dodaj Utwór
        /// </summary>
        /// <remarks>
        /// Wyświetla nowe okienko do wpisania danych nowego utworu, a następnie odświeża widok utworów
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DodajUtworButton_Click(object sender, RoutedEventArgs e)
        {
            DodajUtworWindow win = new DodajUtworWindow();
            win.ShowDialog();
            UtworyColumns_Create();
            Utwory_Refresh();
        }

        /// <summary>
        /// Funkcja wywoływana przez kliknięcie przycisku YouTube Search
        /// </summary>
        /// <remarks>
        /// Wyświetla nowe okienko do wyszukiwania utworów poprzez YouTube
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void YouTubeSearchButton_Click(object sender, RoutedEventArgs e)
        {

            if (CheckForInternetConnection())
            {
                SzukajYT win = new SzukajYT();
                win.ShowDialog();
                UtworyColumns_Create();
                Utwory_Refresh();
            }
            else
            {
                MessageBox.Show("Brak połączenia internetowego", "Komunikat");
            }

        }

        /// <summary>
        /// Funkcja wywoływana przez kliknięcie przycisku Edytuj
        /// </summary>
        /// <remarks>
        /// Wyświetla nowe okienko do edycji informacji o utworze
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EdytujButton_Click(object sender, RoutedEventArgs e)
        {
            dynamic idToEdit = MainWindowGrid.SelectedItem;
            int i = idToEdit.UtworId;
            EdytujUtworWindow win = new EdytujUtworWindow(i);
            win.ShowDialog();
            Utwory_Refresh();
        }

        /// <summary>
        /// Funkcja wywoływana przez kliknięcie przycisku Pokaż w widoku Wykonawcy
        /// </summary>
        /// <remarks>
        /// Wyświetla utwory wybranego wykonawcy
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PokazUtworyWykonawcyButton_Click(object sender, RoutedEventArgs e)
        {
            dynamic row = MainWindowGrid.SelectedItem;
            string nazwa = row.WykonawcaNazwa;
            UtworyColumns_Create(); // Stworzenie kolumn siatki dla utworów 
            var query =
            from plik in db.Utwory
            join wyk in db.Wykonawcy on plik.WykonawcaId equals wyk.WykonawcaId
            join alb in db.Albumy on plik.AlbumId equals alb.AlbumId
            where wyk.Nazwa == nazwa
            orderby plik.Nazwa
            select new { UtworId = plik.UtworId, Tytul = plik.Nazwa, Wykonawca = wyk.Nazwa, Album = alb.Nazwa, Rok = plik.Rok, Data = plik.DataDodania, Ulubione = plik.CzyUlubione, URL = plik.UrlPath };

            MainWindowGrid.ItemsSource = query.ToList();
        }

        /// <summary>
        /// Funkcja wywoływana przez kliknięcie przycisku Pokaż w widoku Albumy
        /// </summary>
        /// <remarks>
        /// Wyświetla utwory wybranego albumu
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PokazUtworyAlbumuButton_Click(object sender, RoutedEventArgs e)
        {
            dynamic row = MainWindowGrid.SelectedItem;
            string nazwa = row.AlbumNazwa;
            UtworyColumns_Create(); // Stworzenie kolumn siatki dla utworów 
            var query =
            from plik in db.Utwory
            join wyk in db.Wykonawcy on plik.WykonawcaId equals wyk.WykonawcaId
            join alb in db.Albumy on plik.AlbumId equals alb.AlbumId
            where alb.Nazwa == nazwa
            orderby plik.Nazwa
            select new { UtworId = plik.UtworId, Tytul = plik.Nazwa, Wykonawca = wyk.Nazwa, Album = alb.Nazwa, Rok = plik.Rok, Data = plik.DataDodania, Ulubione = plik.CzyUlubione, URL = plik.UrlPath };

            MainWindowGrid.ItemsSource = query.ToList();
        }

        /// <summary>
        /// Funkcja wywoływana przez kliknięcie przycisku Usuń w widoku Utwory
        /// </summary>
        /// <remarks>
        /// Usuwa wybrany utwór
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UsunButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                IList rows = MainWindowGrid.SelectedItems;
                List<int> idToDelete = new List<int>();
                foreach (dynamic m in rows)
                {
                    idToDelete.Add(m.UtworId);
                }

                foreach (int id in idToDelete)
                {
                    db.Utwory.Remove(db.Utwory.Find(id));
                    db.SaveChanges();
                }


                Utwory_Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// Funkcja tworząca kolumny do wyświetlenia listy utworów
        /// </summary>
        private void UtworyColumns_Create()
        {
            MainWindowGrid.Columns.Clear();
            DataGridTextColumn c1 = new DataGridTextColumn
            {
                Header = "Tytuł",
                Binding = new Binding("Tytul"),
                Width = 100
            };
            //c1.Width = 110;
            MainWindowGrid.Columns.Add(c1);
            DataGridTextColumn c2 = new DataGridTextColumn
            {
                Header = "Wykonawca/ Kanał",
                //c2.Width = 110;
                Binding = new Binding("Wykonawca")
            };
            MainWindowGrid.Columns.Add(c2);
            DataGridTextColumn c3 = new DataGridTextColumn
            {
                Header = "Album",
                //c3.Width = 110;
                Binding = new Binding("Album")
            };
            MainWindowGrid.Columns.Add(c3);
            DataGridTextColumn c4 = new DataGridTextColumn
            {
                Header = "Rok wydania",
                Binding = new Binding("Rok")
            };
            //c1.Width = 110;
            MainWindowGrid.Columns.Add(c4);
            DataGridTextColumn c5 = new DataGridTextColumn
            {
                Header = "Data dodania",
                //c2.Width = 110;
                Binding = new Binding("Data")
            };
            MainWindowGrid.Columns.Add(c5);
            DataGridTextColumn c7 = new DataGridTextColumn
            {
                Header = "Ulubione",
                //c2.Width = 110;
                Binding = new Binding("Ulubione")
            };
            MainWindowGrid.Columns.Add(c7);
            DataGridHyperlinkColumn c8 = new DataGridHyperlinkColumn
            {
                Header = "URL",
                Width = 100,
                Binding = new Binding("URL")
            };
            MainWindowGrid.Columns.Add(c8);
            DataGridTemplateColumn col1 = new DataGridTemplateColumn();
            col1.Header = "Icon";
            FrameworkElementFactory factory1 = new FrameworkElementFactory(typeof(Image));
            Binding b1 = new Binding("Star");
            factory1.SetValue(Image.SourceProperty, b1);
            DataTemplate cellTemplate1 = new DataTemplate();
            cellTemplate1.VisualTree = factory1;
            col1.CellTemplate = cellTemplate1;
            MainWindowGrid.Columns.Add(col1);

            EdytujButtonColumn_Create();
            UsunButtonColumn_Create();
        }

        /// <summary>
        /// Funkcja tworząca kolumny do wyświetlenia listy albumów
        /// </summary>
        private void AlbumyColumns_Create()
        {
            MainWindowGrid.Columns.Clear();

            DataGridTextColumn c1 = new DataGridTextColumn
            {
                Header = "Album",
                Binding = new Binding("AlbumNazwa")
            };
            MainWindowGrid.Columns.Add(c1);
            DataGridTextColumn c2 = new DataGridTextColumn
            {
                Header = "Wykonawca",
                //c2.Width = 110;
                Binding = new Binding("AlbumWykonawca")
            };
            MainWindowGrid.Columns.Add(c2);
            DataGridTextColumn c3 = new DataGridTextColumn
            {
                Header = "Rok wydania",
                //c3.Width = 110;
                Binding = new Binding("AlbumRok")
            };
            MainWindowGrid.Columns.Add(c3);
            DataGridTextColumn c4 = new DataGridTextColumn
            {
                Header = "Liczba utwórów",
                //c2.Width = 110;
                Binding = new Binding("LiczbaUtworowWAlbumie")
            };
            MainWindowGrid.Columns.Add(c4);
            

            PokazUtworyAlbumuButtonColumn_Create();
            //Create_Column_Usun();
        }

        /// <summary>
        /// Funkcja tworząca kolumny do wyświetlenia listy wykonawców
        /// </summary>
        private void WykonawcyColumns_Create()
        {
            MainWindowGrid.Columns.Clear();
            DataGridTextColumn c1 = new DataGridTextColumn
            {
                Header = "Wykonawca",
                Binding = new Binding("WykonawcaNazwa")
            };
            //c1.Width = 110;
            MainWindowGrid.Columns.Add(c1);
            DataGridTextColumn c2 = new DataGridTextColumn
            {
                Header = "LIczba utworów",
                //c3.Width = 110;
                Binding = new Binding("LiczbaUtworowWykonawcy")
            };
            MainWindowGrid.Columns.Add(c2);

            PokazUtworyWykonawcyButtonColumn_Create();
            //Create_Column_Usun();
        }


        /// <summary>
        /// Funkcja tworząca kolumnę z przyciskami Edytuj
        /// </summary>
        private void EdytujButtonColumn_Create()
        {
            DataGridTemplateColumn buttonEdytujColumn = new DataGridTemplateColumn();
            buttonEdytujColumn.Header = "Edycja";
            DataTemplate buttonTemplate = new DataTemplate();
            FrameworkElementFactory buttonFactory = new FrameworkElementFactory(typeof(Button));
            buttonFactory.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(EdytujButton_Click));
            buttonFactory.SetValue(ContentProperty, "Edytuj");
            buttonTemplate.VisualTree = buttonFactory;
            buttonEdytujColumn.CellTemplate = buttonTemplate;
            MainWindowGrid.Columns.Add(buttonEdytujColumn);
        }

        /// <summary>
        /// Funkcja tworząca kolumnę z przyciskami Usuń
        /// </summary>
        private void UsunButtonColumn_Create()
        {
            DataGridTemplateColumn buttonUsunColumn = new DataGridTemplateColumn();
            buttonUsunColumn.Header = "Usuwanie";
            DataTemplate buttonTemplate = new DataTemplate();
            FrameworkElementFactory buttonFactory = new FrameworkElementFactory(typeof(Button));
            buttonFactory.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(UsunButton_Click));
            buttonFactory.SetValue(ContentProperty, "Usuń");
            buttonTemplate.VisualTree = buttonFactory;
            buttonUsunColumn.CellTemplate = buttonTemplate;
            MainWindowGrid.Columns.Add(buttonUsunColumn);
        }

        /// <summary>
        /// Funkcja tworząca kolumnę z przyciskami Pokaż dla widoku Albumy
        /// </summary>
        private void PokazUtworyAlbumuButtonColumn_Create()
        {
            DataGridTemplateColumn buttonPokazColumn = new DataGridTemplateColumn();
            DataTemplate buttonTemplate = new DataTemplate();
            FrameworkElementFactory buttonFactory = new FrameworkElementFactory(typeof(Button));
            buttonFactory.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(PokazUtworyAlbumuButton_Click));
            buttonFactory.SetValue(ContentProperty, "Pokaż");
            buttonTemplate.VisualTree = buttonFactory;
            buttonPokazColumn.CellTemplate = buttonTemplate;
            MainWindowGrid.Columns.Add(buttonPokazColumn);
        }

        /// <summary>
        /// Funkcja tworząca kolumnę z przyciskami Pokaż dla widoku Wykonawcy
        /// </summary>
        private void PokazUtworyWykonawcyButtonColumn_Create()
        {
            DataGridTemplateColumn buttonPokazColumn = new DataGridTemplateColumn();
            DataTemplate buttonTemplate = new DataTemplate();
            FrameworkElementFactory buttonFactory = new FrameworkElementFactory(typeof(Button));
            buttonFactory.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(PokazUtworyWykonawcyButton_Click));
            buttonFactory.SetValue(ContentProperty, "Pokaż");
            buttonTemplate.VisualTree = buttonFactory;
            buttonPokazColumn.CellTemplate = buttonTemplate;
            MainWindowGrid.Columns.Add(buttonPokazColumn);
        }

        /// <summary>
        /// Funkcja wyświetlająca aktualną zawartość tabeli Utwory
        /// </summary>
        private void Utwory_Refresh()
        {
            var query =
            from plik in db.Utwory
            join wyk in db.Wykonawcy on plik.WykonawcaId equals wyk.WykonawcaId
            join alb in db.Albumy on plik.AlbumId equals alb.AlbumId
            orderby plik.Nazwa
            select new { UtworId = plik.UtworId, Tytul = plik.Nazwa, Wykonawca = wyk.Nazwa, Album = alb.Nazwa, Rok = plik.Rok, Data = plik.DataDodania, Ulubione = plik.CzyUlubione, URL = plik.UrlPath};


            MainWindowGrid.ItemsSource = query.ToList();

        }

        /// <summary>
        /// Funkcja wyświetlająca aktualną zawartość tabeli Albumy
        /// </summary>
        private void Albumy_Refresh()
        {
            try
            {
                

                LiczbaUtworowWAlbumie_Update();

                var query =
                from plik in db.Albumy
                join wyk in db.Wykonawcy on plik.WykonawcaId equals wyk.WykonawcaId
                orderby plik.Nazwa
                select new { AlbumNazwa = plik.Nazwa, AlbumWykonawca = wyk.Nazwa, AlbumRok = plik.Rok, LiczbaUtworowWAlbumie = plik.LiczbaUtworowWAlumie};

                MainWindowGrid.ItemsSource = query.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Funkcja wyświetlająca aktualną zawartość tabeli Wykonawcy
        /// </summary>
        private void Wykonawcy_Refresh()
        {
            try
            {
                LiczbaUtworowWykonawcy_Update();

                var query =
                from plik in db.Wykonawcy
                orderby plik.Nazwa
                select new { WykonawcaNazwa = plik.Nazwa, LiczbaUtworowWykonawcy = plik.LiczbaUtworowWykonawcy };

                MainWindowGrid.ItemsSource = query.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// Funkcja sprawdzająca połaczenie internetowe
        /// </summary>
        /// <returns></returns>
        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://google.com/generate_204"))
                    return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Funkcja aktualizująca liczbę utworów w albumie
        /// </summary>
        public void LiczbaUtworowWAlbumie_Update()
        {
            foreach (Album a in db.Albumy)
            {
                var tmp = from u in db.Utwory
                          where u.AlbumId == a.AlbumId
                          select new { u.UtworId };

                int liczba = tmp.Count();
                
                if(liczba == 0 && a.AlbumId != 1)
                {
                    db.Remove(a);
                }
                else
                {
                    a.LiczbaUtworowWAlumie = liczba;
                    db.Update(a);
                }
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Funkcja aktualizująca liczbę utworów wykonawcy
        /// </summary>
        public void LiczbaUtworowWykonawcy_Update()
        {
            foreach (Wykonawca w in db.Wykonawcy)
            {
                var tmp = from u in db.Utwory
                          where u.WykonawcaId == w.WykonawcaId
                          select new { u.UtworId };

                int liczba = tmp.Count();
                if (liczba == 0 && w.WykonawcaId != 1)
                {
                    db.Remove(w);
                }
                else
                {
                    w.LiczbaUtworowWykonawcy = liczba;
                    db.Update(w);
                }
                db.SaveChanges();
            }
        }

    }
}
