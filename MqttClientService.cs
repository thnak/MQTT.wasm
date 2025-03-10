using System.Text.Json;
using Microsoft.JSInterop;
using MQTT.wasm.Builder;
using MQTT.wasm.Models;

namespace MQTT.wasm;

public class MqttClientService
{
    private readonly IJSRuntime _js;
    private IJSObjectReference? _mqttJs;
    private DotNetObjectReference<MqttClientService>? _dotNetRef;

    public event Action? OnConnected;
    public event Func<MqttApplicationMessage, Task>? ApplicationMessageReceivedAsync;
    public event Action<string>? OnError;
    public event Action? OnDisconnected;

    public MqttClientService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task ConnectAsync(MqttClientOptions options)
    {
        _mqttJs ??= await _js.InvokeAsync<IJSObjectReference>("import", "./_content/MQTT.wasm/MQTT.wasm.js");
        _dotNetRef = DotNetObjectReference.Create(this);

        var optionsJson = JsonSerializer.Serialize(options);
        await _mqttJs.InvokeVoidAsync("mqttService.connect", optionsJson, _dotNetRef);
    }

    public async Task SubscribeAsync(string topic)
    {
        if (_mqttJs is not null)
        {
            await _mqttJs.InvokeVoidAsync("mqttService.subscribe", topic);
        }
    }

    public async Task PublishAsync(MqttApplicationMessage message)
    {
        if (_mqttJs is not null)
        {
            var messageJson = JsonSerializer.Serialize(new
            {
                message.Topic,
                Payload = Convert.ToBase64String(message.Payload),
                message.Retain,
                message.UserProperties
            });

            await _mqttJs.InvokeVoidAsync("mqttService.publish", messageJson);
        }
    }

    public async Task DisconnectAsync()
    {
        if (_mqttJs is not null)
        {
            await _mqttJs.InvokeVoidAsync("mqttService.disconnect");
        }
    }

    [JSInvokable]
    public async Task OnMessageReceivedEvent(string topic, string payloadBase64)
    {
        if (ApplicationMessageReceivedAsync is not null)
        {
            var payload = Convert.FromBase64String(payloadBase64); // Decode Base64 to byte[]
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .Build();

            await ApplicationMessageReceivedAsync.Invoke(message);
        }
    }


    [JSInvokable]
    public void OnConnectedEvent() => OnConnected?.Invoke();

    [JSInvokable]
    public void OnErrorEvent(string error) => OnError?.Invoke(error);

    [JSInvokable]
    public void OnDisconnectedEvent() => OnDisconnected?.Invoke();

    public async ValueTask DisposeAsync()
    {
        _dotNetRef?.Dispose();
        if (_mqttJs is not null)
        {
            await _mqttJs.DisposeAsync();
        }
    }
}