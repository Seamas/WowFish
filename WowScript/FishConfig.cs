using System.Text.Json;
using System.Text.Json.Serialization;

namespace WowScript;

public class FishConfig
{
    private static readonly Lazy<FishConfig> _instance = new(Load);
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    private static string ConfigPath => Path.Combine(AppContext.BaseDirectory, "fishconfig.json");

    public static FishConfig Instance => _instance.Value;

    [JsonConstructor]
    private FishConfig()
    {
    }

    public bool Debug { get; set; } = false;

    public double Threshold { get; set; } = 1.4;

    public double Distance { get; set; } = 1000;

    public int BobberPoints { get; set; } = 150;

    /// <summary>
    /// 是否启用后台模式（WGC 截图 + PostMessage 模拟输入）
    /// </summary>
    public bool UseBackgroundMode { get; set; } = false;

    private static FishConfig Load()
    {
        try
        {
            if (File.Exists(ConfigPath))
            {
                var json = File.ReadAllText(ConfigPath);
                return JsonSerializer.Deserialize<FishConfig>(json, JsonOptions) ?? new FishConfig();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"加载配置文件失败: {ex.Message}");
        }

        return new FishConfig();
    }

    public void Save()
    {
        try
        {
            var json = JsonSerializer.Serialize(this, JsonOptions);
            File.WriteAllText(ConfigPath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"保存配置文件失败: {ex.Message}");
        }
    }
}
