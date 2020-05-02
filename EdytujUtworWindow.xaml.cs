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
        public EdytujUtworWindow(int id)
        {
            InitializeComponent();
            for (int i = 1900; i < 2020; i++)
                Roczniki.Add(i.ToString());

            idEdytowanegoUtworu = id;
            WykonawcyComboBox();
            AlbumyComboBox();
            SetStartValues();
            RocznikiLista.ItemsSource = Roczniki;
        }
           

        private List<string> Roczniki = new List<string> { "nieznany" };

        public void SetStartValues()
        {
            Utwor u = db.Utwory.Find(idEdytowanegoUtworu);
            Wykonawca w = db.Wykonawcy.Find(u.WykonawcaId);
            Album a= db.Albumy.Find(u.AlbumId);
            NazwaTextBox.Text = u.Nazwa;
            WykonawcyLista.Text = w.Nazwa;
            AlbumyLista.Text = a.Nazwa;
            RocznikiLista.Text = u.Rok;

        }
        public void WykonawcyComboBox()
        {
            var result = from w in db.Wykonawcy
                         orderby w.Nazwa
                         select w.Nazwa;
            WykonawcyLista.ItemsSource = result.ToArray();
        }

        public void AlbumyComboBox()
        {
            var result = from a in db.Albumy
                         orderby a.Nazwa
                         select a.Nazwa;
            AlbumyLista.ItemsSource = result.ToArray();
        }


        public Thickness DocumentMargin
        {
            get { return (Thickness)DataContext; }
            set { DataContext = value; }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Dialog box canceled
            DialogResult = false;
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            if (NazwaTextBox.Text != null)
            {
                int WykonawcaId = 0, AlbumId = 0;

                if (WykonawcyLista.Text != null)
                {
                    if (!db.Wykonawcy.Any(w => w.Nazwa == WykonawcyLista.Text))
                    {
                        Wykonawca wykonawca = new Wykonawca { Nazwa = WykonawcyLista.Text };
                        db.Add(wykonawca);
                        db.SaveChanges();
                        WykonawcaId = wykonawca.WykonawcaId;
                    }
                    else
                    {
                        var query =
                            (from wyk in db.Wykonawcy
                             where wyk.Nazwa == WykonawcyLista.Text
                             select wyk).FirstOrDefault();

                        WykonawcaId = query.WykonawcaId;
                    }
                }
                else
                {
                    Wykonawca wykonawca = new Wykonawca { Nazwa = "niezanany" };
                    db.Add(wykonawca);
                    db.SaveChanges();
                    WykonawcaId = wykonawca.WykonawcaId;
                }

                if (AlbumyLista.Text != null)
                {
                    if (!db.Albumy.Any(w => w.Nazwa == AlbumyLista.Text))
                    {
                        Album album;
                        if (RocznikiLista.Text != null)
                            album = new Album { Nazwa = AlbumyLista.Text, Rok = RocznikiLista.Text };
                        else
                            album = new Album { Nazwa = AlbumyLista.Text, Rok = "nieznany" };
                        db.Add(album);
                        db.SaveChanges();
                        AlbumId = album.AlbumId;
                    }
                    else
                    {
                        var query =
                            (from alb in db.Albumy
                             where alb.Nazwa == AlbumyLista.Text
                             select alb).FirstOrDefault();

                        AlbumId = query.AlbumId;
                    }

                }
                else
                {
                    Album album = new Album { Nazwa = "nieznany", Rok = "nieznany" };
                    db.Add(album);
                    db.SaveChanges();
                    AlbumId = album.AlbumId;
                }

                string rocznik;
                if (RocznikiLista.Text != null)
                    rocznik = RocznikiLista.Text;
                else
                    rocznik = "nieznany";

                Utwor u = db.Utwory.Find(idEdytowanegoUtworu);
                u.Nazwa = NazwaTextBox.Text;
                u.WykonawcaId = WykonawcaId;
                u.AlbumId = AlbumId;
                u.Rok = rocznik;
                db.Utwory.Update(u);
                db.SaveChanges();
            }
            else
            {
                // window is not valid
            }
            // Don't accept the dialog box if there is invalid data
            if (!IsValid(this)) return;

            // Dialog box accepted
            DialogResult = true;
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
