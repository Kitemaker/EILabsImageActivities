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
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Test_ImageActivites
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string imagePath = string.Empty;
        System.Drawing.Image img;
        string folderPath = string.Empty;
        public MainWindow()
        {
            InitializeComponent();
            cmbBoxExt.ItemsSource = new string[] { "jpg", "png", "bmp","gif" };

        }

        private void browseButton_Click(object sender, RoutedEventArgs e)
        {
            if (radioButtonFile.IsChecked == true)
            {

                OpenFileDialog _dlg = new OpenFileDialog();
                if (_dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (System.IO.File.Exists(_dlg.FileName))
                    {
                        ImageSource src;
                      
                        imagePath = _dlg.FileName;
                        txtBoxPath.Text = _dlg.FileName;
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(imagePath);
                        bitmap.EndInit();
                        imgBox.Source = bitmap;

                    }
                }
                else
                {
                    FolderBrowserDialog _fld = new FolderBrowserDialog();
                    if (_fld.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        if (System.IO.Directory.Exists(_fld.SelectedPath))
                        {
                            folderPath = _fld.SelectedPath;
                            txtBoxPath.Text = _fld.SelectedPath;
                        }
                    }

                }
            }
        }

        private void btnConvert_Click(object sender, RoutedEventArgs e)
        {
            if (radioButtonFile.IsChecked == true)
            {
                img = System.Drawing.Image.FromFile(imagePath);
                
                string selctedformat = cmbBoxExt.SelectedValue.ToString();
                string destPath = System.IO.Path.GetFileNameWithoutExtension(imagePath) + "." + selctedformat;
                switch (selctedformat)
                {
                    case "jpg":
                        img.Save(destPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case "png":
                        img.Save(destPath, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    case "bmp":
                        img.Save(destPath, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    case "gif":
                        img.Save(destPath, System.Drawing.Imaging.ImageFormat.Gif);
                        break;
                    default:
                        img.Save(System.IO.Path.GetFileName(imagePath));
                        break;
                }
                

              

            }
        }

        private void btnResize_Click(object sender, RoutedEventArgs e)
        {
            System.Drawing.Image newImage = ResizeImage(img, Convert.ToInt32(xvalue.Text), Convert.ToInt32(yvalue.Text));
            newImage.Save(System.IO.Path.GetFileName(imagePath));
        }

        public  Bitmap ResizeImage( System.Drawing.Image image, int width, int height)
        {
            var destRect = new System.Drawing.Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return destImage;
        }

        private void btnThumb_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                img = System.Drawing.Image.FromFile(imagePath);
                System.Drawing.Image.GetThumbnailImageAbort myCallback = new System.Drawing.Image.GetThumbnailImageAbort(ThumbNailCallback);
                System.Drawing.Image thumbImage = img.GetThumbnailImage(40, 60, myCallback, IntPtr.Zero);
                thumbImage.Save(System.IO.Path.GetFileName(imagePath));
            }
            catch (Exception ex)
            { System.Windows.MessageBox.Show(ex.Message); }
        }

        private bool ThumbNailCallback()
        { return true; }
    }
}
