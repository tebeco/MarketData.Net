import * as React from 'react';
import './Workspace.css';
import { UserSentMessage, User, UserJoinedChannel, UserLeftChannel, ChatConnection } from '../chat';
import { ChatChannel } from './ChatChannel';

interface WorkspaceProps {
    hubUrl: string;
}

interface WorkspaceState {
    chatConnection: ChatConnection;
    messages: UserSentMessage[];
    users: User[];
}

class Workspace extends React.Component<WorkspaceProps, WorkspaceState> {
    constructor(props: WorkspaceProps) {
        super(props);

        const chatConnection = new ChatConnection(
            success => this.onConnection(success),
            () => this.onDisconnection(),
            data => this.onUserJoinedChannel(data),
            data => this.onUserLeftChannel(data),
            data => this.onUserSentMessage(data));
        this.state = { chatConnection, messages: [], users: [] };
    }
    public render() {
        let action;
        if (!this.state.chatConnection.isConnected) {
            action = () => this.state.chatConnection.connect(this.props.hubUrl);
        } else {
            action = () => this.state.chatConnection.disconnect();
        }

        return (
            <div id="workspace">
                <button onClick={action}>Toggle Connection</button>
                <div>
                    <span>Current Users :</span>
                    {this.state.users.map(user => (<li key={user.name}>{user.name}</li>))}
                </div>
                <ChatChannel messagesSent={this.state.messages} />
            </div>
        );
    }

    private onConnection(success: boolean) {
        this.setState({
            messages: []
        });
    }
    private onDisconnection() {
        this.setState({
            messages: []
        });
    }

    private onUserSentMessage(data: string) {
        const messageData: UserSentMessage = JSON.parse(data) as UserSentMessage;

        const messages = [...this.state.messages, messageData];
        this.setState({
            ...this.state,
            messages
        });
    }

    private onUserJoinedChannel(data: string) {
        const userJoined: UserJoinedChannel = JSON.parse(data) as UserJoinedChannel;
        const currentUser: User = userJoined.user;
        const currentUsers = this.state.users;

        this.setState({
            ...this.state,
            users: [...currentUsers, currentUser]
        });
    }

    private onUserLeftChannel(data: string) {
        const userLeft: UserLeftChannel = JSON.parse(data) as UserLeftChannel;
        const currentUser: User = userLeft.user;
        const currentUsers = this.state.users;

        const existingIndex = currentUsers.findIndex(item => item.name === currentUser.name);
        if (existingIndex !== -1) {
            const users = currentUsers.splice(existingIndex, 1);
            this.setState({
                ...this.state,
                users
            });
        }
    }
};

export { Workspace, WorkspaceProps, WorkspaceState };