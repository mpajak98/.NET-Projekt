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
            Create_Columns_Utwory(); // Stworzenie kolumn siatki dla utworów 
            Refresh_Utwory(); // Wyswietlenie utworów z bazy danych
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //cv = CurrentView.utwory;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            this.db.Dispose();
        }

        //*****************************************************************************************
        // Funkcje wywoływane przez przyciski wyświetlające różne widoki
        private void Button_Click_Utwory(object sender, RoutedEventArgs e)
        {
            Create_Columns_Utwory(); // Stworzenie kolumn siatki dla utworów 
            Refresh_Utwory(); // Wyswietlenie utworów z bazy danych
            //cv = CurrentView.utwory;
        }

        private void Button_Click_Album(object sender, RoutedEventArgs e)
        {
            Create_Columns_Albumy(); // Stworzenie kolumn siatki dla albumów
            Refresh_Albumy(); // Wyswietlenie albumów z bazy danych
            //cv = CurrentView.albumy;
        }

        private void Button_Click_Wykonawca(object sender, RoutedEventArgs e)
        {
            Create_Columns_Wykonawcy(); // Stworzenie kolumn siatki dla wykonawców
            Refresh_Wykonawcy(); // Wyswietlenie wykonawców z bazy danych
            //cv = CurrentView.wykonawcy;
        }

        private void Button_Click_Ulubione(object sender, RoutedEventArgs e)
        {
            Create_Columns_Wykonawcy(); // Stworzenie kolumn siatki dla wykonawców
            var query =
            from plik in db.Utwory
            join wyk in db.Wykonawcy on plik.WykonawcaId equals wyk.WykonawcaId
            join alb in db.Albumy on plik.AlbumId equals alb.AlbumId
            where plik.CzyUlubione == true
            orderby plik.Nazwa
            select new { UtworId = plik.UtworId, Tytul = plik.Nazwa, Wykonawca = wyk.Nazwa, Album = alb.Nazwa, Rok = plik.Rok, Data = plik.DataDodania, Ocena = plik.Ocena, Ulubione = plik.CzyUlubione, URL = plik.UrlPath };

            dataGrid1.ItemsSource = query.ToList();
        }

        //******************************************************************************************



        // Funkcja dodająca jedną z instancji w zależności w jakim widoku się znajdujemy
        private void Button_Click_Dodaj(object sender, RoutedEventArgs e)
        {
            /*switch (cv)
            {
                case CurrentView.utwory:
                    {*/
                        DodajUtworWindow win = new DodajUtworWindow();
                        win.ShowDialog();
                        Refresh_Utwory();
         /*               break;
                    }
                case CurrentView.albumy:
                    {
                        DodajAlbumWindow win = new DodajAlbumWindow();
                        win.ShowDialog();
                        Refresh_Albumy();
                        break;
                    }
                case CurrentView.wykonawcy:
                    {
                        DodajWykonawceWindow win = new DodajWykonawceWindow();
                        win.ShowDialog();
                        Refresh_Wykonawcy();
                        break;
                    }

            }
        */
        }

        // Funkcja dodająca jedną z utwór po połączeniu z YT
        private void Button_Click_DodajYT(object sender, RoutedEventArgs e)
        {
            SzukajYT win = new SzukajYT();
            win.ShowDialog();
            Refresh_Utwory();

        }

        // Funkcja edytująca jedną z instancji w zależności w jakim widoku się znajdujemy
        private void Button_Click_Edytuj(object sender, RoutedEventArgs e)
        {
            dynamic idToEdit = dataGrid1.SelectedItem;
            int i = idToEdit.UtworId;
            EdytujUtworWindow win = new EdytujUtworWindow(i);
            win.ShowDialog();
            Refresh_Utwory();
        }

        private void Button_Click_Pokaz_Utwory_Wykonawcy(object sender, RoutedEventArgs e)
        {
            dynamic row = dataGrid1.SelectedItem;
            string nazwa = row.WykonawcaNazwa;
            Create_Columns_Utwory(); // Stworzenie kolumn siatki dla utworów 
            var query =
            from plik in db.Utwory
            join wyk in db.Wykonawcy on plik.WykonawcaId equals wyk.WykonawcaId
            join alb in db.Albumy on plik.AlbumId equals alb.AlbumId
            where wyk.Nazwa == nazwa
            orderby plik.Nazwa
            select new { UtworId = plik.UtworId, Tytul = plik.Nazwa, Wykonawca = wyk.Nazwa, Album = alb.Nazwa, Rok = plik.Rok, Data = plik.DataDodania, Ocena = plik.Ocena, Ulubione = plik.CzyUlubione, URL = plik.UrlPath };

            dataGrid1.ItemsSource = query.ToList();
        }

        private void Button_Click_Pokaz_Utwory_Albumu(object sender, RoutedEventArgs e)
        {
            dynamic row = dataGrid1.SelectedItem;
            string nazwa = row.AlbumNazwa;
            Create_Columns_Utwory(); // Stworzenie kolumn siatki dla utworów 
            var query =
            from plik in db.Utwory
            join wyk in db.Wykonawcy on plik.WykonawcaId equals wyk.WykonawcaId
            join alb in db.Albumy on plik.AlbumId equals alb.AlbumId
            where alb.Nazwa == nazwa
            orderby plik.Nazwa
            select new { UtworId = plik.UtworId, Tytul = plik.Nazwa, Wykonawca = wyk.Nazwa, Album = alb.Nazwa, Rok = plik.Rok, Data = plik.DataDodania, Ocena = plik.Ocena, Ulubione = plik.CzyUlubione, URL = plik.UrlPath };

            dataGrid1.ItemsSource = query.ToList();
        }


        // Funkcja usuwająca jedną z instancji w zależności w jakim widoku się znajdujemy
        private void Button_Click_Usun(object sender, RoutedEventArgs e)
        {
            try
            {
                
                IList rows = dataGrid1.SelectedItems;
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


                Refresh_Utwory();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }


        //******************************************************************************************
        // Funkcje tworzące odpowiednie kolumny w zależności od instancji
        private void Create_Columns_Utwory()
        {
            dataGrid1.Columns.Clear();
            DataGridTextColumn c1 = new DataGridTextColumn();
            c1.Header = "Tytuł";
            c1.Binding = new Binding("Tytul");
            //c1.Width = 110;
            dataGrid1.Columns.Add(c1);
            DataGridTextColumn c2 = new DataGridTextColumn();
            c2.Header = "Wykonawca";
            //c2.Width = 110;
            c2.Binding = new Binding("Wykonawca");
            dataGrid1.Columns.Add(c2);
            DataGridTextColumn c3 = new DataGridTextColumn();
            c3.Header = "Album";
            //c3.Width = 110;
            c3.Binding = new Binding("Album");
            dataGrid1.Columns.Add(c3);
            DataGridTextColumn c4 = new DataGridTextColumn();
            c4.Header = "Rok";
            c4.Binding = new Binding("Rok");
            //c1.Width = 110;
            dataGrid1.Columns.Add(c4);
            DataGridTextColumn c5 = new DataGridTextColumn();
            c5.Header = "Data";
            //c2.Width = 110;
            c5.Binding = new Binding("Data");
            dataGrid1.Columns.Add(c5);
            DataGridTextColumn c6 = new DataGridTextColumn();
            c6.Header = "ocena";
            //c3.Width = 110;
            c6.Binding = new Binding("Ocena");
            dataGrid1.Columns.Add(c6);
            DataGridTextColumn c7 = new DataGridTextColumn();
            c7.Header = "Ulubione";
            //c2.Width = 110;
            c7.Binding = new Binding("Ulubione");
            dataGrid1.Columns.Add(c7);
            DataGridTextColumn c8 = new DataGridTextColumn();
            c8.Header = "URL";
            //c3.Width = 110;
            c8.Binding = new Binding("URL");
            dataGrid1.Columns.Add(c8);

            Create_Column_Edytuj();
            Create_Column_Usun();
        }

        private void Create_Columns_Albumy()
        {
            dataGrid1.Columns.Clear();

            DataGridTextColumn c1 = new DataGridTextColumn
            {
                Header = "Album",
                Binding = new Binding("AlbumNazwa")
            };
            dataGrid1.Columns.Add(c1);
            DataGridTextColumn c2 = new DataGridTextColumn
            {
                Header = "Wykonawca",
                //c2.Width = 110;
                Binding = new Binding("AlbumWykonawca")
            };
            dataGrid1.Columns.Add(c2);
            DataGridTextColumn c3 = new DataGridTextColumn
            {
                Header = "Rok",
                //c3.Width = 110;
                Binding = new Binding("AlbumRok")
            };
            dataGrid1.Columns.Add(c3);
            DataGridTextColumn c4 = new DataGridTextColumn
            {
                Header = "Ulubione",
                //c2.Width = 110;
                Binding = new Binding("AlbumUlubione")
            };
            dataGrid1.Columns.Add(c4);
            DataGridTextColumn c5 = new DataGridTextColumn
            {
                Header = "Ocena",
                //c3.Width = 110;
                Binding = new Binding("AlbumOcena")
            };
            dataGrid1.Columns.Add(c5);

            Create_Column_Pokaz_Utwory_Albumu();
            //Create_Column_Usun();
        }

        private void Create_Columns_Wykonawcy()
        {
            dataGrid1.Columns.Clear();
            DataGridTextColumn c1 = new DataGridTextColumn
            {
                Header = "Wykonawca",
                Binding = new Binding("WykonawcaNazwa")
            };
            //c1.Width = 110;
            dataGrid1.Columns.Add(c1);
            DataGridTextColumn c2 = new DataGridTextColumn
            {
                Header = "Ocena",
                //c2.Width = 110;
                Binding = new Binding("WykonawcaOcena")
            };
            dataGrid1.Columns.Add(c2);
            DataGridTextColumn c3 = new DataGridTextColumn
            {
                Header = "Ulubione",
                //c3.Width = 110;
                Binding = new Binding("WykonawcaUlubione")
            };
            dataGrid1.Columns.Add(c3);

            Create_Column_Pokaz_Utwory_Wykonawcy();
            //Create_Column_Usun();
        }

        // Funkcja tworząca kolumnę przycisków do edycji
        private void Create_Column_Edytuj()
        {
            DataGridTemplateColumn buttonEdytujColumn = new DataGridTemplateColumn();
            buttonEdytujColumn.Header = "Edycja";
            DataTemplate buttonTemplate = new DataTemplate();
            FrameworkElementFactory buttonFactory = new FrameworkElementFactory(typeof(Button));
            buttonFactory.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(Button_Click_Edytuj));
            buttonFactory.SetValue(ContentProperty, "Edytuj");
            buttonTemplate.VisualTree = buttonFactory;
            buttonEdytujColumn.CellTemplate = buttonTemplate;
            dataGrid1.Columns.Add(buttonEdytujColumn);
        }

        // Funkcja tworząca kolumnę przycisków do usuwania
        private void Create_Column_Usun()
        {
            DataGridTemplateColumn buttonUsunColumn = new DataGridTemplateColumn();
            buttonUsunColumn.Header = "Usuwanie";
            DataTemplate buttonTemplate = new DataTemplate();
            FrameworkElementFactory buttonFactory = new FrameworkElementFactory(typeof(Button));
            buttonFactory.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(Button_Click_Usun));
            buttonFactory.SetValue(ContentProperty, "Usuń");
            buttonTemplate.VisualTree = buttonFactory;
            buttonUsunColumn.CellTemplate = buttonTemplate;
            dataGrid1.Columns.Add(buttonUsunColumn);
        }
        private void Create_Column_Pokaz_Utwory_Albumu()
        {
            DataGridTemplateColumn buttonPokazColumn = new DataGridTemplateColumn();
            DataTemplate buttonTemplate = new DataTemplate();
            FrameworkElementFactory buttonFactory = new FrameworkElementFactory(typeof(Button));
            buttonFactory.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(Button_Click_Pokaz_Utwory_Albumu));
            buttonFactory.SetValue(ContentProperty, "Pokaż");
            buttonTemplate.VisualTree = buttonFactory;
            buttonPokazColumn.CellTemplate = buttonTemplate;
            dataGrid1.Columns.Add(buttonPokazColumn);
        }

        private void Create_Column_Pokaz_Utwory_Wykonawcy()
        {
            DataGridTemplateColumn buttonPokazColumn = new DataGridTemplateColumn();
            DataTemplate buttonTemplate = new DataTemplate();
            FrameworkElementFactory buttonFactory = new FrameworkElementFactory(typeof(Button));
            buttonFactory.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(Button_Click_Pokaz_Utwory_Wykonawcy));
            buttonFactory.SetValue(ContentProperty, "Pokaż");
            buttonTemplate.VisualTree = buttonFactory;
            buttonPokazColumn.CellTemplate = buttonTemplate;
            dataGrid1.Columns.Add(buttonPokazColumn);
        }


        //******************************************************************************************

        //******************************************************************************************
        // Funkcje aktualizujące listy instancji
        private void Refresh_Utwory()
        {
            var query =
            from plik in db.Utwory
            join wyk in db.Wykonawcy on plik.WykonawcaId equals wyk.WykonawcaId
            join alb in db.Albumy on plik.AlbumId equals alb.AlbumId
            orderby plik.Nazwa
            select new { UtworId = plik.UtworId, Tytul = plik.Nazwa, Wykonawca = wyk.Nazwa, Album = alb.Nazwa, Rok = plik.Rok, Data = plik.DataDodania, Ocena = plik.Ocena, Ulubione = plik.CzyUlubione, URL = plik.UrlPath };

            dataGrid1.ItemsSource = query.ToList();

        }

        private void Refresh_Albumy()
        {
            try
            {
                var query =
                from plik in db.Albumy
                join wyk in db.Wykonawcy on plik.WykonawcaId equals wyk.WykonawcaId
                orderby plik.Nazwa
                select new { AlbumNazwa = plik.Nazwa, AlbumWykonawca = wyk.Nazwa, AlbumRok = plik.Rok , AlbumUlubione = plik.CzyUlubione, AlbumOcena = plik.Ocena };

                dataGrid1.ItemsSource = query.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Refresh_Wykonawcy()
        {
            try
            {
                var query =
                from plik in db.Wykonawcy
                orderby plik.Nazwa
                select new { WykonawcaNazwa = plik.Nazwa, WykonawcaOcena = plik.Ocena, WykonawcaUlubione = plik.CzyUlubione };

                dataGrid1.ItemsSource = query.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        //******************************************************************************************

    }
}
