import "./mqtt.min.js";

window.mqttService = (function () {
    let client = null;
    let dotNetRef = null;

    return {
        connect: function (optionsJson, dotNetInstance) {
            if (client) return;
            dotNetRef = dotNetInstance;

            const options = JSON.parse(optionsJson);
            const mqttOptions = {
                clientId: options.ClientId,
                clean: options.CleanSession,
                username: options.Username || undefined,
                password: options.Password || undefined,
                rejectUnauthorized: true // Ensures proper TLS verification
            };

            if (options.WebSocketServer) {
                client = mqtt.connect(options.WebSocketServer, mqttOptions);
            } else {
                const protocol = options.UseTls ? "mqtts" : "mqtt";
                client = mqtt.connect(`${protocol}://${options.TcpServer}:${options.Port}`, mqttOptions);
            }

            client.on("message", function (topic, message) {
                const payloadBase64 = message.toString("base64"); // Convert binary to Base64
                dotNetRef.invokeMethodAsync("OnMessageReceivedEvent", topic, payloadBase64);
            });
            
            // client.on("message", function (topic, message) {
            //     const buffer = new Uint8Array(message);
            //     dotNetRef.invokeMethodAsync("OnMessageReceivedEvent", topic, buffer);
            // });

            client.on("connect", function () {
                console.log("Connected to MQTT Broker");
                dotNetRef.invokeMethodAsync("OnConnectedEvent");
            });
            
            client.on("error", function (error) {
                console.error("MQTT Error:", error);
                dotNetRef.invokeMethodAsync("OnErrorEvent", error.toString());
            });

            client.on("close", function () {
                console.log("Disconnected from MQTT Broker");
                dotNetRef.invokeMethodAsync("OnDisconnectedEvent");
                client = null;
            });
        },

        disconnect: function () {
            if (client) {
                client.end();
                client = null;
            }
        }
    };
})();
