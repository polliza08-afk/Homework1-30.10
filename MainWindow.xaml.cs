using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WpfImageReader;

public partial class MainWindow : Window
{
    private static readonly string[] Extensions =
        { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };

    public MainWindow()
    {
        InitializeComponent();
    }

    private void btnDownload_Click(object sender, RoutedEventArgs e)
    {
        string imageUrl = txtUrl.Text;
        string dir = "images";
        Directory.CreateDirectory(dir);
        try
        {
            HttpClient client = new HttpClient();
            var buffer = client.GetByteArrayAsync(imageUrl).Result;
            string fileName = Guid.NewGuid().ToString() + ".jpg";
            var imageSave = Path.Combine(dir, fileName);
            File.WriteAllBytes(imageSave, buffer);
            MessageBox.Show("Файл успішно збережено! " + imageSave);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void ReadDataFolder(string path)
    {
        if (!Directory.Exists(path))
        {
            MessageBox.Show("Папка 'images' не знайдена!");
            return;
        }

        // Повертаємо усі файли із папки, які є фото
        var files = Directory.GetFiles(path)
            .Where(f => Extensions.Contains(Path.GetExtension(f).ToLower()));

        var imageItems = new List<ImageItem>();

        foreach (var file in files)
        {
            var fileInfo = new FileInfo(file);

            var dateCreation = fileInfo.CreationTime; //Дата створення
            long fileSizeInBytes = fileInfo.Length; //Розмір у байтах
            string fileSizeFormatted = $"{fileSizeInBytes / 1024} КБ";//Конвертуємо у кілобайти
            string fileName = fileInfo.Name; //Назва файлу

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(file);
            bitmap.DecodePixelWidth = 300; //Економія пам'яті
            bitmap.CacheOption = BitmapCacheOption.OnLoad;//Щоб не блокувати фото
            bitmap.EndInit();

            //Додаємо заповнений об'єкт у список
            imageItems.Add(new ImageItem
            {
                FileName = fileName,
                FileSizeFormatted = fileSizeFormatted,
                CreationTime = dateCreation,
                ImageSource = bitmap
            });
        }

        //Прив'язуємо список до ListView
        GalleryList.ItemsSource = imageItems;
    }

    private void btnReadImages_Click(object sender, RoutedEventArgs e)
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        string folder = Path.Combine(currentDirectory, "images");
        ReadDataFolder(folder);
    }
}

//Модель даних для рядка у ListView
public class ImageItem
{
    public string FileName { get; set; }
    public string FileSizeFormatted { get; set; }
    public DateTime CreationTime { get; set; }
    public BitmapImage ImageSource { get; set; }
}