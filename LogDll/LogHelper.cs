namespace LogDll
{
    public class LogHelper
    {
        public static event Action<string>? LogReceived;

        public static void Info(string message)
        {
            var formatted = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}";
            Console.WriteLine(formatted);
            LogReceived?.Invoke(formatted);
        }
    }
}
