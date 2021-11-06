var data = null;

var cont = document.getElementById("cont");
var bt_update = document.getElementById("bt_update");

var ws = new WebSocket("ws://localhost:8081", "editor");

var items = [];

ws.onopen = function (event) {
};

ws.onmessage = function (event) {
    data = JSON.parse(event.data);
    console.log(data);

    data.forEach(item => {
        cont.innerHTML += 
            '<div class="input-group input-group-lg">'+
            '<span class="input-group-text" id="inputGroup-sizing-lg" style="width: 100px;">'+ item.name +'</span>'+
            '<input id="'+"_"+ item.name +'" type="number" class="form-control" aria-label="Sizing example input"'+
            '    aria-describedby="inputGroup-sizing-lg" value="'+ item.count +'">'+
            '</div>';
        
        items.push(document.getElementById("_"+item.name));
    });
};

function updateOnclick(){
    data.forEach(item => {
        item.count = Number(document.getElementById("_"+item.name).value);
    });

    console.log(data);

    ws.send(JSON.stringify(data));
}