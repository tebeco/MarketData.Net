import { HubConnection } from 'signalr-client';
import { connect } from './signalr';

export interface User {
    name: string;
}

export interface Message {
    user: User;
    id: string;
    messageContent: string;
}

export interface UserJoinedChannel {
    user: User;
}

export interface UserLeftChannel {
    user: User;
}

export interface UserSentMessage {
    user: User;
    message: Message;
}

export class ChatConnection {
    public isConnected: boolean = false;

    private hubConnection: HubConnection;
    private onConnection: (succes: boolean) => void;
    private onDisconnection: () => void;
    private onUserJoin: (data: string) => void;
    private onUserLeft: (data: string) => void;
    private onMessageSent: (data: string) => void;

    constructor(
        onConnection: (succes: boolean) => void,
        onDisconnection: () => void,
        onUserJoin: (data: string) => void,
        onUserLeft: (data: string) => void,
        onMessageSent: (data: string) => void,
    ) {
        this.onConnection = onConnection;
        this.onDisconnection = onDisconnection;
        this.onUserJoin = onUserJoin;
        this.onUserLeft = onUserLeft;
        this.onMessageSent = onMessageSent;

    }

    public connect(hubUrl: string): void {
        this.isConnected = true;
        connect(
            hubUrl,
            (hub) => {
                this.onConnection(true);

                this.hubConnection.on('userJoinedChannel', (data) => this.onUserJoin(data));
                this.hubConnection.on('userLeftChannel', (data) => this.onUserLeft(data));
                this.hubConnection.on('userSendMessage', (data) => this.onMessageSent(data));
            },
            (err) => {
                this.onConnection(false);
            });
    }

    public disconnect(): void {
        this.isConnected = false;
        this.hubConnection.stop();

        this.onDisconnection();
    }
}