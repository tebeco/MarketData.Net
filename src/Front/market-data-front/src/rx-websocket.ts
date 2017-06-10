import * as Rx from 'rxjs/Rx';
import { Observer } from 'rxjs/Rx';

export function fromWebSocket(address: string, openObserver: Observer<string>) {
    var ws = new WebSocket(address);

    let observer: Observer<string> = {
        next: data => {
            if (ws.readyState === WebSocket.OPEN) { ws.send(data); }
        },
        error: _err => { /*No Empty*/ },
        complete: () => { /*No Empty*/ }
    };

    // Handle the data
    var observable = Rx.Observable.create(function (obs: Observer<string>) {
        // Handle open
        if (openObserver) {
            ws.onopen = function (e: Event) {
                openObserver.next('opened');
                openObserver.complete();
            };
        }

        // Handle messages  
        ws.onmessage = (data) => obs.next(data.data);
        ws.onerror = (err) => obs.error(err);
        ws.onclose = () => obs.complete();

        // Return way to unsubscribe
        return ws.close.bind(ws);
    });

    return Rx.Subject.create(observer, observable);
}
