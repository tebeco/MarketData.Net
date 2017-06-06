import * as React from 'react';
import './App.css';
import { Workspace } from './Workspace';

const logo = require('./logo.svg');
const hubUrl: string = 'http://localhost:5000/chatRoom';

class App extends React.Component<{}, null> {

  render() {
    return (
      <div className="App">
        <div className="App-header">
          <img src={logo} className="App-logo" alt="logo" />
          <h2>Welcome to React</h2>
        </div>
        <Workspace hubUrl={hubUrl} />
      </div>
    );
  }
}

export { App };
