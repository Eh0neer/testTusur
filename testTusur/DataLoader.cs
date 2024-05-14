using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;
using testTusur;

public static class DataLoader
{
    public static async Task<FormData> LoadFromJsonAsync(string filePath)
    {
        try
        {
            string json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<FormData>(json) ?? throw new InvalidOperationException("Failed to deserialize JSON data");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error reading JSON file: {ex.Message}", ex);
        }
    }

    public static async Task<FormData> LoadFromXmlAsync(string filePath)
    {
        try
        {
            var serializer = new XmlSerializer(typeof(FormData));
            using (var fs = new FileStream(filePath, FileMode.Open))
            {
                return await Task.Run(() => (FormData)serializer.Deserialize(fs)) ?? throw new InvalidOperationException("Failed to deserialize XML data");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error reading XML file: {ex.Message}", ex);
        }
    }

    public static async Task SaveToJsonAsync(string filePath, FormData data)
    {
        try
        {
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, json);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error writing JSON file: {ex.Message}", ex);
        }
    }

    public static async Task SaveToXmlAsync(string filePath, FormData data)
    {
        try
        {
            var serializer = new XmlSerializer(typeof(FormData));
            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                await Task.Run(() => serializer.Serialize(fs, data));
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error writing XML file: {ex.Message}", ex);
        }
    }
}
