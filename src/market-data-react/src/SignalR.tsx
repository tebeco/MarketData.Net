import * as React from 'react';

let hubUrl: string = 'http://localhost:5000/myHub';

// /*/
import { HubConnection, TransportType } from 'signalr-client';
let connection = new HubConnection(hubUrl);
function connectToSignalR() {
    connection = new HubConnection(hubUrl);
    
    console.log('conncting to', hubUrl);

    connection
        .start(TransportType.WebSockets)
        .then(() => {
            console.log('conncted to', hubUrl);
        })
        .catch((err: any) => {
            console.log('could not connect', err);
        });
}
/*/
const signalR = require('../node_modules/signalr-client/dist/browser/signalr-client.js');
let connection = new signalR.HubConnection(hubUrl);
function connectToSignalR() {
    connection = new signalR.HubConnection(hubUrl);
    
    console.log('conncting to', hubUrl);

    connection
        .start(signalR.TransportType.WebSockets)
        .then(() => {
            console.log('conncted to', hubUrl);
        })
        .catch((err: any) => {
            console.log('could not connect', err);
        });
}
// */
const SignalR = () => {
    return (
        <div>
            <button onClick={() => connectToSignalR()}>connect</button>
        </div>
    );
};

export { SignalR };