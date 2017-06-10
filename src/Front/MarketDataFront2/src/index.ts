import * as rx from 'rxjs/Rx';
import { Observer, Observable } from 'rxjs/Rx';
import { fromWebSocket } from './rx-websocket';
import LineChart from './LineChart';
import * as stock from './Stock';
import { getColor } from './colors';

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

window["toggle"] = function (codes: string[]): void {
  if (window["cleanUp"]) {
    window["cleanUp"]();
  }

  document.getElementById('quotesTable').style.display = 'table';
  document.getElementById('quotesBody').innerHTML = codes.map((code, index) =>
    `<tr>
      <td style="color: ${getColor(index)}">${code}</td>
      <td id="last-${code}" class="text-xs-right"></td>
      <td id="min-${code}" class="text-xs-right"></td>
      <td id="max-${code}" class="text-xs-right"></td>
      <td id="vwap-${code}" class="text-xs-right"></td>
    </tr>`).join('');

  const spotLineChart = new LineChart("#spotGraph", 'Spot', codes.length);
  const vwapLineChart = new LineChart("#vwapGraph", 'Vwap', codes.length);

  const subscriptions = [];
  const eventSources = [];

  codes.forEach((code, index) => {
    const quoteEventSourceUrl = `ws://localhost:8081?code=${code}`;
    const quoteEventSourceObs:Observable<any> = fromWebSocket(quoteEventSourceUrl, openObserver)

    const quoteObservable = stock.parseRawStream(
      quoteEventSourceObs
      .map(event => event.data)
    );

    subscriptions.push(quoteObservable.pluck('quote').subscribe(spotLineChart.getObserver(index)));
    subscriptions.push(stock.detectTrends(quoteObservable).subscribe(q => {
      const color = q.color === 'green' ? '#5cb85c' : '#d9534f';
      document.getElementById(`last-${code}`).innerHTML = `<span style="background: ${color}">${q.quote.quote.toFixed(4)}</span> EUR`
    }));

    subscriptions.push(stock.minFromPrevious(quoteObservable, 10).subscribe(q => {
      document.getElementById(`min-${code}`).innerHTML = `${q.toFixed(4)} EUR`
    }));
    subscriptions.push(stock.maxFromPrevious(quoteObservable, 10).subscribe(q => {
      document.getElementById(`max-${code}`).innerHTML = `${q.toFixed(4)} EUR`
    }));

    const vwapEventSourceUrl = `ws://localhost:8082?code=${code}`;
    const vwapEventSourceObs: Observable<any> = fromWebSocket(vwapEventSourceUrl, openObserver) 
    const vwapObservable
      = stock.parseRawVwapStream(
        vwapEventSourceObs
          .map(event => event)
      ).pluck<number>('vwap');

    subscriptions.push(vwapObservable.subscribe(vwapLineChart.getObserver(index)));
    subscriptions.push(vwapObservable.subscribe(v => {
      document.getElementById(`vwap-${code}`).innerHTML = `${v.toFixed(4)}$`
    }));

    eventSources.push(quoteEventSourceObs, vwapEventSourceObs);
  });


  window["cleanUp"] = () => {
    subscriptions.forEach(_ => _.unsubscribe());
    eventSources.forEach(_ => _.close());
    document.getElementById("spotGraph").innerHTML = "";
    document.getElementById("vwapGraph").innerHTML = "";
    document.getElementById('quotesTable').style.display = 'none';
  }
};

const activeStocksEventUrl = 'ws://localhost:8083';
const stockStaticDataObservable
  = stock.parseStaticDataRawStream(
    fromWebSocket(activeStocksEventUrl, openObserver)
      .map(event => event.data)
  );

stockStaticDataObservable.scan((stockNames, stock) => stockNames.concat([stock]), [])
  .subscribe(stocks => {
    const stockLinks = stocks.map(st =>
      `<div class="col-md-2">
        <a href="#" onclick="toggle(['${st.code}'])">
          ${st.code}
        </a><br/> <small>${st.companyName} - ${st.market}</small>
      </div>`);

    const stocksArray = stocks.map(stock => `'${stock.code}'`).join(', ');
    stockLinks.push(`
      <div class="col-md-2">
        <a href="#" onclick="toggle([${stocksArray}])">All stocks</a><br/>
      </div>
    `);

    document.getElementById("activeStocks").innerHTML = `
    <div class="row">
      ${stockLinks.join('')}
    </div>`;
  }, error => console.log("Stocks connection stopped"));
