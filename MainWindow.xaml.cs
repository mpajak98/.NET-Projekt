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
    //public enum CurrentView { utwory, albumy, wykonawcy}

    public partial class MainWindow : Window
    {
        private BazaDanych db = new BazaDanych();
        //private CurrentView cv;
        public MainWindow()
        {
            InitializeComponent(); // Inicjalizacja komponentu
            UtworyColumns_Create(); // Stworzenie kolumn siatki dla utworów 
            Utwory_Refresh(); // Wyswietlenie utworów z bazy danych
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            this.db.Dispose();
        }

        //*****************************************************************************************
        // Funkcje wywoływane przez przyciski wyświetlające różne widoki
        private void UtworyButton_Click(object sender, RoutedEventArgs e)
        {
            UtworyColumns_Create(); // Stworzenie kolumn siatki dla utworów 
            Utwory_Refresh(); // Wyswietlenie utworów z bazy danych
            //cv = CurrentView.utwory;
        }

        private void AlbumyButton_Click(object sender, RoutedEventArgs e)
        {
            AlbumyColumns_Create(); // Stworzenie kolumn siatki dla albumów
            Albumy_Refresh(); // Wyswietlenie albumów z bazy danych
            //cv = CurrentView.albumy;
        }

        private void WykonawcyButton_Click(object sender, RoutedEventArgs e)
        {
            WykonawcyColumns_Create(); // Stworzenie kolumn siatki dla wykonawców
            Wykonawcy_Refresh(); // Wyswietlenie wykonawców z bazy danych
            //cv = CurrentView.wykonawcy;
        }

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

        //******************************************************************************************



        // Funkcja dodająca jedną z instancji w zależności w jakim widoku się znajdujemy
        private void DodajUtworButton_Click(object sender, RoutedEventArgs e)
        {
            /*switch (cv)
            {
                case CurrentView.utwory:
                    {*/
                        DodajUtworWindow win = new DodajUtworWindow();
                        win.ShowDialog();
                        Utwory_Refresh();
         /*               break;
                    }
                case CurrentView.albumy:
                    {
                        DodajAlbumWindow win = new DodajAlbumWindow();
                        win.ShowDialog();
                        Albumy_Refresh();
                        break;
                    }
                case CurrentView.wykonawcy:
                    {
                        DodajWykonawceWindow win = new DodajWykonawceWindow();
                        win.ShowDialog();
                        Wykonawcy_Refresh();
                        break;
                    }

            }
        */
        }

        // Funkcja dodająca jedną z utwór po połączeniu z YT
        private void YouTubeSearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SzukajYT win = new SzukajYT();
                win.ShowDialog();
                Utwory_Refresh();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            

        }

        // Funkcja edytująca jedną z instancji w zależności w jakim widoku się znajdujemy
        private void EdytujButton_Click(object sender, RoutedEventArgs e)
        {
            dynamic idToEdit = MainWindowGrid.SelectedItem;
            int i = idToEdit.UtworId;
            EdytujUtworWindow win = new EdytujUtworWindow(i);
            win.ShowDialog();
            Utwory_Refresh();
        }

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


        // Funkcja usuwająca jedną z instancji w zależności w jakim widoku się znajdujemy
        private void UsunButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                IList rows = MainWindowGrid.SelectedItems;
                List<int> idToDelete = new List<int>();
                foreach (dynamic m in rows)
                {
                    idToDelete.Add(m.UtworId);
                    MessageBox.Show(m.UtworId.ToString());
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


        //******************************************************************************************
        // Funkcje tworzące odpowiednie kolumny w zależności od instancji
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
                Header = "Wykonawca",
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
                Header = "Rok",
                Binding = new Binding("Rok")
            };
            //c1.Width = 110;
            MainWindowGrid.Columns.Add(c4);
            DataGridTextColumn c5 = new DataGridTextColumn
            {
                Header = "Data",
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

            EdytujButtonColumn_Create();
            UsunButtonColumn_Create();
        }

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
                Header = "Rok",
                //c3.Width = 110;
                Binding = new Binding("AlbumRok")
            };
            MainWindowGrid.Columns.Add(c3);
            DataGridTextColumn c4 = new DataGridTextColumn
            {
                Header = "Ulubione",
                //c2.Width = 110;
                Binding = new Binding("AlbumUlubione")
            };
            MainWindowGrid.Columns.Add(c4);

            PokazUtworyAlbumuButtonColumn_Create();
            //Create_Column_Usun();
        }

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
            DataGridTextColumn c3 = new DataGridTextColumn
            {
                Header = "Ulubione",
                //c3.Width = 110;
                Binding = new Binding("WykonawcaUlubione")
            };
            MainWindowGrid.Columns.Add(c3);

            PokazUtworyWykonawcyButtonColumn_Create();
            //Create_Column_Usun();
        }

        // Funkcja tworząca kolumnę przycisków do edycji
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

        // Funkcja tworząca kolumnę przycisków do usuwania
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


        //******************************************************************************************

        //******************************************************************************************
        // Funkcje aktualizujące listy instancji
        private void Utwory_Refresh()
        {
            var query =
            from plik in db.Utwory
            join wyk in db.Wykonawcy on plik.WykonawcaId equals wyk.WykonawcaId
            join alb in db.Albumy on plik.AlbumId equals alb.AlbumId
            orderby plik.Nazwa
            select new { UtworId = plik.UtworId, Tytul = plik.Nazwa, Wykonawca = wyk.Nazwa, Album = alb.Nazwa, Rok = plik.Rok, Data = plik.DataDodania, Ulubione = plik.CzyUlubione, URL = plik.UrlPath };

            MainWindowGrid.ItemsSource = query.ToList();

        }
        private void Albumy_Refresh()
        {
            try
            {
                var query =
                from plik in db.Albumy
                join wyk in db.Wykonawcy on plik.WykonawcaId equals wyk.WykonawcaId
                orderby plik.Nazwa
                select new { AlbumNazwa = plik.Nazwa, AlbumWykonawca = wyk.Nazwa, AlbumRok = plik.Rok , AlbumUlubione = plik.CzyUlubione };

                MainWindowGrid.ItemsSource = query.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Wykonawcy_Refresh()
        {
            try
            {
                var query =
                from plik in db.Wykonawcy
                orderby plik.Nazwa
                select new { WykonawcaNazwa = plik.Nazwa, WykonawcaUlubione = plik.CzyUlubione };

                MainWindowGrid.ItemsSource = query.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        //******************************************************************************************

    }
}
