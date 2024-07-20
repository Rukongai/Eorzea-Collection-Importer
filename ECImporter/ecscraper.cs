using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System.IO.Compression;
using ImGuiNET;

namespace ECImporter;
internal class Scrape
{
    private static readonly string RequiredPrefix = "https://ffxiv.eorzeacollection.com/glamour/";
    public static async Task GetEC(string ecurl)
    {

        if (!ecurl.StartsWith(RequiredPrefix, StringComparison.OrdinalIgnoreCase))
        {
            Service.PluginLog.Info("Error: The URL is invalid");
        }
        else
        {

        string url = ecurl;
        var items = await GetItemsWithDetails(url);
        var itemSheet = Service.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Item>()!;
        var stainSheet = Service.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Stain>()!;

        string jsonCompressed = """
                Bh+LCAAAAAAAAArEV01T4zgQ/S++LqFsYzsfNyAEspWwTJKFw9QeOk47UaFYXkmGylD8923ZngwBRwlMufamSK/7dbc6z+oXZ8A43qNUTKROzztxhgtMNUsYSqfnxBD60G1jCyDqtIIgwtZ8ftZteTHE864fYhD5zolzKRE0OeiDRrLyXT9oue2W15m5Qc9ze2fuqed1/LAd/uG6PdclkxEofbVg2gqPtvBbWBvPfUwg55o2+qhiyTLDSvu0MRAyxsUEFxKenV4CXCEFJriQ5fm3nMWPZMWWZKBlTqczWCqn9/2fE+dBMo13UmiMNS625lf/5ixbUz2c3oszBpbeQEqnL85Q43pIKy9yvSJ9pbdG51nGNz8pih9TTaY7O7sW1blbrXxavp44fyXJe7rA7wbdKOq0g98gLXfqOW8QagjbXtQU4YVYbGoJz5rKkEqqahn9pi5yhMt6RrcpxgGirmMMu00xXoGszTHsNMV4i/FjLWO7KUYSCaXrs4ya4pwMWLo0UlxDGjbWsP8H6Q0ULTtdiec634S4Z0qUQamZWC75G6HeBT4gZOazYPFWSE+aF7d5zUEpLJbFnsnX/QC/zJUWa/YDi2+BWCCvcBOIi7174DktvK1lQUuW15guympWkHfOSxWcbbJ6NxXmBtlypd8gwg9uLjmkNheDfZH+pAAmld5wO4jC4CYUZcto+sjS6uO739XVBgvM5F1mtYG99+buD+wgMRWCAR/QoyWX6Nmc7iCpU49Enh2NDI5Ghkcjo6ORbRuSvlsQb2agtRA2XIk45rbnUjyrYzpihIk+gJuuwP6Hma6B8yFpti32W6GsTv40j8n9x2OR65UNMGKZ0vTwtNaZZQeLN85VzHFMQmVVgBkwfrAwF6Rk00rI9noyWnFH6my9hi1ogk80QhhBtjZeCT6Y7APqFHcy3VXwUlgJeAeS5gJN1Aa7Jfj7aUxDAst4McT8iujU/aDNb2zova3KZ5MFX97DTKRF9e6QZo5Uw3IP2rTxiK3nwIepxlQxvTnGrNDDL9gZ0e2zJMnLnqZZqMJdS8S0Wl/szc2o7G+abzX4Kw5MteiP/RXTomJftK2kcNuVnw6bZZ+qGs9WUO+pjzHwT8Sxz5PpU5rEJem8+WMUOmWGHjPpjlj6WD51MBGSXJnN84TQZvn6+h8AAAD//w==
                """;
        JObject defaultDesign = JsonDecompressor.DeserializeDecompressFromBase64(jsonCompressed);

        foreach (var match in items)
        {
            var itemRow = itemSheet.FirstOrDefault(item => item.Name.RawString.Equals(
                                      match.Name, StringComparison.InvariantCultureIgnoreCase));
            Tinker.UpdateEquipmentSlot(defaultDesign, itemRow);
            if (match.DyeColors.Count > 0)
            {
                foreach (var dye in match.DyeColors)
                {
                    var stainRow = stainSheet.FirstOrDefault(stain => stain.Name.RawString.Equals(
                                    dye, StringComparison.InvariantCultureIgnoreCase));
                    Tinker.UpdateDyeSlot(defaultDesign, itemRow, stainRow, match.DyeColors.IndexOf(dye));
                    /*Service.PluginLog.Info($"{dye}: {stainRow.RowId} index: {match.DyeColors.IndexOf(dye)}");*/
                }
            }
        }

        string jsonString = defaultDesign.ToString();

        // Convert the JSON string to a byte array
        byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);

        // Compress the byte array using GZip
        byte[] compressedBytes;
        using (var memoryStream = new MemoryStream())
        {
            using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
            {
                gzipStream.Write(jsonBytes, 0, jsonBytes.Length);
            }
            compressedBytes = memoryStream.ToArray();
        }

        // Prepend the byte 0x06 to the compressed data
        byte[] resultBytes = new byte[compressedBytes.Length + 1];
        resultBytes[0] = 0x06;
        Array.Copy(compressedBytes, 0, resultBytes, 1, compressedBytes.Length);

        // Convert the final byte array to a Base64 string
        string base64String = Convert.ToBase64String(resultBytes);
        ImGui.SetClipboardText(base64String);
            Service.Chat.Print(new Dalamud.Game.Text.XivChatEntry
            {
                Message = "[Eorzea Collection Importer] Import string copied to clipboard"
            });
    }
    static async Task<List<ItemDetails>> GetItemsWithDetails(string url)
    {
        List<ItemDetails> itemList = new List<ItemDetails>();

        // Initialize HttpClient
        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");

        try
        {
            // Get the HTML
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string htmlContent = await response.Content.ReadAsStringAsync();

            // Load HTML
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(htmlContent);

            // Select all divs with the class "c-gear-slot-item"
            var itemNodes = document.DocumentNode.SelectNodes("//div[contains(@class, 'c-gear-slot-item')]");

            if (itemNodes != null)
            {
                foreach (var itemNode in itemNodes)
                {
                    var linkNode = itemNode.SelectSingleNode(".//a[contains(@class, 'eorzeadb_link')]");
                    var nameNode = itemNode.SelectSingleNode(".//span[contains(@class, 'c-gear-slot-item-name')]");
                    var colorNodes = itemNode.SelectNodes(".//span[contains(@class, 'c-gear-slot-item-info-color u-inset')]");

                    string link = linkNode?.GetAttributeValue("href", string.Empty);
                    string name = HtmlEntity.DeEntitize(nameNode?.InnerText.Trim());

                    List<string> colors = new List<string>();
                    if (colorNodes != null)
                    {
                        foreach (var colorNode in colorNodes)
                        {
                            string color = HtmlEntity.DeEntitize(colorNode?.InnerText.Trim());
                            if (!string.IsNullOrEmpty(color))
                            {
                                // Remove the leading character to get rid of dye icons
                                color = color.TrimStart('⬤', ' ', '◯');
                                    if (color != "Undyed" && color != "Optional piece")
                                    {
                                        colors.Add(color);
                                    }

                            }
                        }
                    }

                    itemList.Add(new ItemDetails
                    {
                        Link = link,
                        Name = name,
                        DyeColors = colors
                    });
                }
            }
        }
        catch (Exception e)
        {
            Service.PluginLog.Info("An error occurred: " + e.Message);
                Service.Chat.Print(new Dalamud.Game.Text.XivChatEntry
                {
                    Message = "[Eorzea Collection Importer] An error occurred: " + e.Message
                });
            }

        return itemList;
    }
    }
}

class ItemDetails
{
    public string Link { get; set; }
    public string Name { get; set; }
    public List<string> DyeColors { get; set; } = new List<string>();
}

public class JsonDecompressor
{
    public static JObject DeserializeDecompressFromBase64(string base64String)
    {

        byte[] base64Bytes = Convert.FromBase64String(base64String);
        if (base64Bytes[0] != 0x06)
        {
            throw new InvalidOperationException("Invalid data: Expected the first byte to be 0x06.");
        }
        byte[] compressedBytes = new byte[base64Bytes.Length - 1];
        Array.Copy(base64Bytes, 1, compressedBytes, 0, compressedBytes.Length);


        byte[] decompressedBytes;
        using (var compressedStream = new MemoryStream(compressedBytes))
        using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
        using (var decompressedStream = new MemoryStream())
        {
            gzipStream.CopyTo(decompressedStream);
            decompressedBytes = decompressedStream.ToArray();
        }

        string jsonString = Encoding.UTF8.GetString(decompressedBytes);
        JObject jsonObject = JObject.Parse(jsonString);

        return jsonObject;
    }
}
