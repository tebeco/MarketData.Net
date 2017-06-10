import * as Rx from 'rxjs/Rx';
import { Observer } from 'rxjs/Rx';


export function fromWebSocket(address: string, openObserver: Observer<string>) {
    var ws = new WebSocket(address);

    let observer: Observer<string> = {
        next: data => {
            if (ws.readyState === WebSocket.OPEN) { ws.send(data); }
        },
        error: _err => {},
        complete: () => {}
    };

    // Handle the data
    var observable = Rx.Observable.create(function (obs) {
        // Handle open
        if (openObserver) {
            ws.onopen = function (e) {
                openObserver.next(e.toString());
                openObserver.complete();
            };
        }

        // Handle messages  
        ws.onmessage = obs.onNext.bind(obs);
        ws.onerror = obs.onError.bind(obs);
        ws.onclose = obs.onCompleted.bind(obs);

        // Return way to unsubscribe
        return ws.close.bind(ws);
    });

    return Rx.Subject.create(observer, observable);
}
