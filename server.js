
const WebSocket = require('ws')
const fs = require('fs');


const wss = new WebSocket.Server({ port: 8081 })

var editor = null;
var clients = [];


let rawdata = fs.readFileSync('data.json');
var data = JSON.parse(rawdata);
console.log(data);

wss.on('connection', ws => {
    if(ws._protocol == 'editor'){
        editor = ws;

        ws.on('message', function incoming(d, isBinary) {
            console.log(JSON.parse(d));
            clients.forEach(function each(client) {
                if (client.readyState === WebSocket.OPEN) {
                    client.send(d, { binary: isBinary });
                }
            });
        });
    }else{
        console.log("client + 1");
        clients.push(ws);
    }

    ws.send(JSON.stringify(data))
});