using MQTT.wasm.Models;

namespace MQTT.wasm.Builder;

public class MqttApplicationMessageBuilder
{
    private readonly MqttApplicationMessage _message = new();

    public MqttApplicationMessageBuilder WithTopic(string topic)
    {
        _message.Topic = topic;
        return this;
    }

    public MqttApplicationMessageBuilder WithPayload(byte[] payload)
    {
        _message.Payload = payload;
        return this;
    }

    public MqttApplicationMessageBuilder WithRetainFlag(bool retain = true)
    {
        _message.Retain = retain;
        return this;
    }

    public MqttApplicationMessageBuilder WithUserProperty(string key, string value)
    {
        _message.UserProperties[key] = value;
        return this;
    }

    public MqttApplicationMessage Build() => _message;
}
