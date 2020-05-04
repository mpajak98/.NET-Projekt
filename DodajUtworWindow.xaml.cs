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

        private List<string> Roczniki  = new List<string> { "nieznany" };
        private List<int> Ocena = new List<int> { 0, 1, 2, 3, 4, 5 };
        public DodajUtworWindow()
        {
            InitializeComponent();

            for (int i = 2020; i > 1900; i--)
                Roczniki.Add(i.ToString());

            WykonawcyComboBox();
            AlbumyComboBox();
            RocznikiLista.ItemsSource = Roczniki;
            RocznikiLista.Text = Roczniki.First();
        }

        // Utworzenie rozwijanej listy wykonawców
        public void WykonawcyComboBox()
        {
            var result = from w in db.Wykonawcy
                         orderby w.Nazwa
                         select w.Nazwa;
            
            WykonawcyLista.ItemsSource = result.ToArray();
        }
        // Utworzenie rozwijanej listy albumów
        public void AlbumyComboBox()
        {
            var result = from a in db.Albumy
                         orderby a.Nazwa
                         select a.Nazwa;
            AlbumyLista.ItemsSource = result.ToArray();
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

        private void DodanieUtworu()
        {
            int WykonawcaId = 1, AlbumId = 1;
            if (NazwaTextBox.Text != "")
            {

                WykonawcaId = IsWykonawcaValid();
                AlbumId = IsAlbumValid();
               
                Utwor u = new Utwor { Nazwa = NazwaTextBox.Text, WykonawcaId = WykonawcaId, AlbumId = AlbumId, Rok = RocznikiLista.Text, CzyUlubione = false, DataDodania = DateTime.Now };

                db.Add(u);
                db.SaveChanges();
            }

        }

        private int IsAlbumValid()
        {
            string pusto = "";
            if (AlbumyLista.Text != pusto)
            {
                Album album = (from a in db.Albumy
                               where a.Nazwa == AlbumyLista.Text
                               select a).FirstOrDefault();
                if (album == null) // albumu nie ma w bazie
                {
                    if (WykonawcyLista.Text == pusto) // nie podano wykonawcy
                    {
                        Album newAlbum = new Album { Nazwa = AlbumyLista.Text, Rok = RocznikiLista.Text, WykonawcaId = 1 };
                        db.Add(newAlbum);
                        db.SaveChanges();
                        return newAlbum.AlbumId;
                    }
                    else // podano wykonawce
                    {
                        Wykonawca wykonawca =
                        (from wyk in db.Wykonawcy
                         where wyk.Nazwa == WykonawcyLista.Text
                         select wyk).FirstOrDefault();

                        Album newAlbum = new Album { Nazwa = AlbumyLista.Text, Rok = RocznikiLista.Text, WykonawcaId = wykonawca.WykonawcaId };
                        db.Add(newAlbum);
                        db.SaveChanges();
                        return newAlbum.AlbumId;
                    }
                }
                else // album jest w bazie
                {
                    if (WykonawcyLista.Text == pusto) // nie podano wykonawcy
                    {
                        return album.AlbumId;
                    }
                    else // podano wykonawce
                    {
                        Wykonawca wykonawca =
                        (from wyk in db.Wykonawcy
                         where wyk.Nazwa == WykonawcyLista.Text
                         select wyk).FirstOrDefault();

                        if (wykonawca.WykonawcaId == 1)
                            album.WykonawcaId = wykonawca.WykonawcaId;
                        return album.AlbumId;
                    }
                }
            }
            return 1;
        }

        private int IsWykonawcaValid()
        {
            string pusto = "";
            if (WykonawcyLista.Text != pusto)
            {
                Wykonawca wykonawca = (from w in db.Wykonawcy
                                       where w.Nazwa == WykonawcyLista.Text
                                       select w).FirstOrDefault();
                if (wykonawca == null) //Wykonawcy nie ma w bazie
                {
                    Wykonawca newWykonawca = new Wykonawca { Nazwa = WykonawcyLista.Text };
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
                           where wyk.Nazwa == WykonawcyLista.Text
                           select wyk).FirstOrDefault();
            Album a = (from alb in db.Albumy
                           where alb.Nazwa == AlbumyLista.Text
                           select alb).FirstOrDefault();
            Utwor u = (from utw in db.Utwory
                       where utw.Nazwa == NazwaTextBox.Text
                       select utw).FirstOrDefault();
            string rok = RocznikiLista.Text;
            string wykonawca = WykonawcyLista.Text;
            string album = AlbumyLista.Text;
            string nazwa = NazwaTextBox.Text;

            if (nazwa == "") 
                throw new Exception("Nie podano nazwy");
            if(u != null) 
                throw new Exception("Podana nazwa już istnieje");
            if(a != null && rok != "")
                if(a.Rok != rok)
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

        public void AlbumyCheck()
        {
            Wykonawca w = (from wyk in db.Wykonawcy
                           where wyk.Nazwa == WykonawcyLista.Text
                           select wyk).FirstOrDefault();
            if(w != null)
            {
                var result = from a in db.Albumy
                             where a.WykonawcaId == w.WykonawcaId
                             orderby a.Nazwa
                             select a.Nazwa;
                AlbumyLista.ItemsSource = result.ToArray();
            }
        }

        public void AlbumyWykonawcy(object sender, EventArgs e)
        {
            Wykonawca w = (from wyk in db.Wykonawcy
                           where wyk.Nazwa == WykonawcyLista.Text
                           select wyk).FirstOrDefault();
            if (w != null)
            {
                var result = from a in db.Albumy
                             where a.WykonawcaId == w.WykonawcaId
                             orderby a.Nazwa
                             select a.Nazwa;
                AlbumyLista.ItemsSource = result.ToArray();
                AlbumyLista.Text = result.FirstOrDefault();

            }
            else
            {
                var result = from a in db.Albumy
                             orderby a.Nazwa
                             select a.Nazwa;
                AlbumyLista.ItemsSource = result.ToArray();
                AlbumyLista.Text = result.FirstOrDefault();
            }
           
        }

        public void RokAlbumu(object sender, EventArgs e)
        {
            Album a = (from alb in db.Albumy
                           where alb.Nazwa == AlbumyLista.Text
                           select alb).FirstOrDefault();
            if (a != null)
            {
                RocznikiLista.ItemsSource = new List<string>{a.Rok};
                RocznikiLista.Text = a.Rok;
            }
            else
            {
                RocznikiLista.ItemsSource = Roczniki.ToList();
                RocznikiLista.Text = Roczniki.First();
            }
                
        }

        // Validate all dependency objects in a window
        private bool IsValid(DependencyObject node)
        {
            // Check if dependency object was passed
            if (node != null)
            {
                // Check if dependency object is valid.
                // NOTE: Validation.GetHasError works for controls that have validation rules attached 
                var isValid = !Validation.GetHasError(node);
                if (!isValid)
                {
                    // If the dependency object is invalid, and it can receive the focus,
                    // set the focus
                    if (node is IInputElement) Keyboard.Focus((IInputElement)node);
                    return false;
                }
            }

            // If this dependency object is valid, check all child dependency objects
            return LogicalTreeHelper.GetChildren(node).OfType<DependencyObject>().All(IsValid);

            // All dependency objects are valid
        }
    

    }
}
