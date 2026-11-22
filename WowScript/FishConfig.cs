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
    /// 是否在每次钓鱼前自动上鱼饵
    /// </summary>
    public bool UseBait { get; set; } = false;

    /// <summary>
    /// 上鱼饵按键（游戏内宏按键），如 3、F3、A 等
    /// </summary>
    public string BaitKey { get; set; } = "3";

    /// <summary>
    /// 鱼饵持续时间（分钟），未超过该时间不会重复上鱼饵
    /// </summary>
    public int BaitDurationMinutes { get; set; } = 10;

    /// <summary>
    /// 是否在每次钓鱼前使用填充键（用于消耗蚌壳等物品，清理背包空格）
    /// </summary>
    public bool UseFillKey { get; set; } = false;

    /// <summary>
    /// 填充键按键（游戏内宏按键），如 1、F4、B 等
    /// </summary>
    public string FillKey { get; set; } = "1";

    /// <summary>
    /// 收竿时按下的游戏内交互键，用于直接与鱼漂互动并拾取，无需鼠标点击
    /// </summary>
    public string InteractKey { get; set; } = "F";

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
