import * as React from 'react';
// import { HubConnection, TransportType } from 'signalr-client';
// const signalR = require('../node_modules/signalr-client/dist/browser/signalr-client.js');

let hubUrl: string = 'http://localhost:5000/myHub';
// let hubUrl: string = 'http://localhost:2512/myHub';

// let connection = new HubConnection(hubUrl);

function connectToSignalR() {
    // connection = new HubConnection(hubUrl);
    
    console.log('conncting to', hubUrl);

    // connection.onDataReceived = (data: any) => {
    //     console.log(data);
    // };

    // connection
    //     .start(TransportType.WebSockets)
    //     .then(() => {
    //         console.log('conncted to', hubUrl);
    //     })
    //     .catch((err: any) => {
    //         console.log('could not connect', err);
    //     });
}

const SignalR = () => {
    return (
        <div>
            <button onClick={() => connectToSignalR()}>connect</button>
        </div>
    );
};

export { SignalR };