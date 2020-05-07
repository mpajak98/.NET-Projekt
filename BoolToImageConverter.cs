using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media.Imaging;
using System.Data;

namespace BibliotekaMultimediow
{
    public class BoolToImageConverter : IValueConverter
    {

        public object ConvertBack(object value,
                          Type targetType,
                          object parameter, CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }

        public object Convert(object value, Type targetType,
                 object parameter, CultureInfo culture)
        {
            if (value is DataRowView)
            {
                DataRowView row = value as DataRowView;
                if (row != null)
                {
                    if (row.DataView.Table.Columns.Contains("Ulubione"))
                    {
                        Type type = row["Ulubione"].GetType();
                        string status = (string)row["Ulubione"];
                        if (status == "False")
                        {
                            Uri uri =
                              new Uri("pack://application:,,,/Images/star_white.png");
                            BitmapImage source = new BitmapImage(uri);
                            return source;
                        }
                        if (status == "True")
                        {
                            Uri uri =
                              new Uri("pack://application:,,,/Images/star_yellow.png");
                            BitmapImage source = new BitmapImage(uri);
                            return source;
                        }
                    }
                }
            }
            return null;
        }

    }
}
