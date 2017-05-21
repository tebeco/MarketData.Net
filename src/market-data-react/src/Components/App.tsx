import * as React from 'react';
import './App.css';
// import { HubConnection, TransportType } from 'signalr-client';

const logo = require('./logo.svg');

function connectToSignalR() {
  // const connection = new HubConnection('http://localhost:2512/myHub');
  // connection.start(TransportType.WebSockets);
  // connection.on('send', (data) => {
    // console.log('data', data);
  // });
}

class App extends React.Component<{}, null> {
  render() {
    return (
      <div className="App">
        <div className="App-header">
          <img src={logo} className="App-logo" alt="logo" />
          <h2>Welcome to React</h2>
        </div>
        <p className="App-intro">
          <input type="button" value="x" onClick={() => connectToSignalR()} />
        </p>
      </div>
    );
  }
}

export default App;
