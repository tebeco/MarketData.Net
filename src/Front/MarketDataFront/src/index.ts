import { fromWebSocket } from './rx-websocket';
import { Observer } from 'rxjs/Rx';

console.log('start');

let openObserver: Observer<string> = {
    next: data => {
        console.log('openObserver ticked');
    },
    error: _err => {
        console.log('openObserver error');
    },
    complete: () => {
        console.log('openObserver completed');
    }
};

const url: string = 'ws://localhost:8096';
console.log(url);
var socket = fromWebSocket(url, openObserver);
console.log(url);

socket.subscribe(
    function (e) {
        console.log('subscribe', e);
        
        console.log(e.data);
    });