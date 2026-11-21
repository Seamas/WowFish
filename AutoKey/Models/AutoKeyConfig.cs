using System.Text.Json;
using System.Text.Json.Serialization;

namespace AutoKey.Models;

/// <summary>
/// 自动按键配置，单例模式并通过 JSON 持久化
/// </summary>
public class AutoKeyConfig
{
    private static readonly Lazy<AutoKeyConfig> _instance = new(Load);
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    private static string ConfigPath => Path.Combine(AppContext.BaseDirectory, "autokeyconfig.json");

    public static AutoKeyConfig Instance => _instance.Value;

    [JsonConstructor]
    private AutoKeyConfig()
    {
    }

    /// <summary>
    /// 要按下的按键，如 1、F3、A
    /// </summary>
    public string Key { get; set; } = "1";

    /// <summary>
    /// 按键间隔（毫秒），如 1500 表示 1.5 秒
    /// </summary>
    public int IntervalMilliseconds { get; set; } = 5000;

    /// <summary>
    /// 按键重复次数
    /// </summary>
    public int RepeatCount { get; set; } = 10;

    private static AutoKeyConfig Load()
    {
        try
        {
            if (File.Exists(ConfigPath))
            {
                var json = File.ReadAllText(ConfigPath);
                return JsonSerializer.Deserialize<AutoKeyConfig>(json, JsonOptions) ?? new AutoKeyConfig();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"加载配置文件失败: {ex.Message}");
        }

        return new AutoKeyConfig();
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
