import * as React from 'react';
import { UserSentMessage } from '../chat';

export interface ChatChannelProps {
    messagesSent: UserSentMessage[];
}

const ChatChannel = ({ messagesSent }: ChatChannelProps) => {
    return (
        <div id="chatChannel">
            <ul id="messages">
                {
                    messagesSent.map(msgSent => {
                        return (
                            <li key={msgSent.message.id}>{msgSent.user + ' : ' + msgSent.message.messageContent}</li>
                        );
                    })
                }
            </ul>
        </div>
    );
};

export { ChatChannel };
