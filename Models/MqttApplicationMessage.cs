namespace MQTT.wasm.Models;

public class MqttApplicationMessage
{
    public string Topic { get; set; } = string.Empty;
    public byte[] Payload { get; set; } = [];
    public bool Retain { get; set; }
    public Dictionary<string, string> UserProperties { get; set; } = new();
}