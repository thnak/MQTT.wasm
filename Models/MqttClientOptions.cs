namespace MQTT.wasm.Models;

public class MqttClientOptions
{
    public string ClientId { get; set; } = Guid.NewGuid().ToString();
    public string WebSocketServer { get; set; } = string.Empty;
    public string TcpServer { get; set; } = string.Empty;
    public int Port { get; set; } = 1883;
    public bool UseTls { get; set; } = false; // NEW: TLS flag
    public bool CleanSession { get; set; } = true;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Dictionary<string, string> UserProperties { get; set; } = new();
}
