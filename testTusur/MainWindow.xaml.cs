using Microsoft.Win32;
using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml.Serialization;

namespace testTusur
{
    public partial class MainWindow : Window
    {
        private string currentFilePath;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void loadButton_Click(object sender, RoutedEventArgs e)
        {
            await OpenFile("Загрузить файл", LoadData);
        }


        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFile("Сохранить файл", SaveData);
        }

        private void printButton_Click(object sender, RoutedEventArgs e)
        {
            PrintDocument(MainTextArea.Text);
        }
        // Загрузка или открытие файла
        private async Task OpenFile(string title, Action<string> loadAction)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "JSON Files|*.json|XML Files|*.xml",
                Title = title
            };

            if (openFileDialog.ShowDialog() == true)
            {
                loadAction(openFileDialog.FileName);
            }
        }

        // Сохронение файла
        private void SaveFile(string title, Action<string> saveAction)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON Files|*.json|XML Files|*.xml",
                Title = title
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                saveAction(saveFileDialog.FileName);
            }
        }
        /* Метод отвечает за загрузку данных из файла.
         * Он определяет, как именно данные будут загружены в зависимости от расширения файла*/
        private async void LoadData(string filePath)
        {
            try
            {
                string fileExtension = Path.GetExtension(filePath).ToLower();
                FormData data = null;

                switch (fileExtension)
                {
                    case ".json":
                        data = await DataLoader.LoadFromJsonAsync(filePath);
                        break;
                    case ".xml":
                        data = await DataLoader.LoadFromXmlAsync(filePath);
                        break;
                    default:
                        MessageBox.Show("Unsupported file format", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                }

                MainTextArea.Text = data.TextArea;
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error loading data: {ex.Message}");
            }
        }

        /* Метод отвечает за сохронение данных из файла.
         * Он определяет, как именно данные будут сохронены в зависимости от расширения файла*/
        private async void SaveData(string filePath)
        {
            string fileExtension = Path.GetExtension(filePath).ToLower();
            var data = new FormData { TextArea = MainTextArea.Text };

            try
            {
                switch (fileExtension)
                {
                    case ".json":
                        await DataLoader.SaveToJsonAsync(filePath, data);
                        break;
                    case ".xml":
                        await DataLoader.SaveToXmlAsync(filePath, data);
                        break;
                    default:
                        MessageBox.Show("Unsupported file format", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error saving data: {ex.Message}");
            }
        }

        // Печать
        private void PrintDocument(string text)
        {
            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                var document = new FlowDocument(new Paragraph(new Run(text)))
                {
                    Name = "Document"
                };
                IDocumentPaginatorSource idpSource = document;
                printDialog.PrintDocument(idpSource.DocumentPaginator, "Printing Flow Document");
            }
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public class FormData
    {
        public string TextArea { get; set; }
    }
}
