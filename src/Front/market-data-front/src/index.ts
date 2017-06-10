import './index.css';
import * as rx from 'rxjs/Rx';
import { fromWebSocket } from './rx-websocket';
// import LineChart from './LineChart';
import * as stock from './Stock';
// import { getColor } from './colors';

console.log('NO REACT');

let openObserver: rx.Observer<string> = {
    next: data => {
        // console.log('openObserver ticked');
    },
    error: _err => {
        // console.log('openObserver error');
    },
    complete: () => {
        // console.log('openObserver completed');
    }
};

const activeStocksEventUrl = 'ws://localhost:8096';
const activeStocksEventSource = fromWebSocket(activeStocksEventUrl, openObserver);
const stockStaticDataObservable
    = stock.parseStaticDataRawStream(
        activeStocksEventSource
            .map((event: any) => event)
    );

stockStaticDataObservable.subscribe(
    data => console.log(data),
    err => console.log('err :', err),
    () => console.log('completed')
);
