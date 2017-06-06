import { HubConnection, TransportType } from 'signalr-client';

export function connect(hubUrl: string, resolve: (hubConnection: HubConnection) => void, reject: (err: {}) => void) {
    const hubConnection = new HubConnection(hubUrl);

    hubConnection
        .start(TransportType.WebSockets)
        .then(() => {
            
            resolve(hubConnection);
        })
        .catch((err: {}) => {
            reject(err);
        });

    return hubConnection;
}

export function disconnect(hubConnection: HubConnection) {
    hubConnection.stop();
}