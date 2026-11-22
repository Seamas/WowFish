using System.Drawing;

namespace LogDll
{
    public class LogHelper
    {
        /// <summary>
        /// 日志事件，参数为格式化后的日志文本与颜色
        /// </summary>
        public static event Action<string, Color>? LogReceived;

        /// <summary>
        /// 输出日志，可通过 color 指定显示颜色，默认为黑色
        /// </summary>
        public static void Info(string message, Color? color = null)
        {
            var formatted = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}";
            Console.WriteLine(formatted);
            LogReceived?.Invoke(formatted, color ?? Color.Black);
        }
    }
}
