"use strict";
(function(){
    console.log('Hello world!');

    var endpointConfig = {
        url: ' http://localhost:5000/api/crs',
        responseReceivers: [
            new crsClientSignalr.SignalrResponseReceiver(
                ' http://localhost:5000/hubs/crs'
            )
        ]
    };

    var buttonContainer = document.getElementById('button-container');
    var metadataContainer = document.getElementById('pre-metadata-info');

    var endpoint = new crsClient.CrsEndpoint(endpointConfig);
    window.endpoint = endpoint;

    window.initializeCrs = function() {
        endpoint.initialize().then(function (metadata) {
            metadataContainer.innerHTML = JSON.stringify(metadata, undefined, 4);
        }).catch(function(err) {
            alert(err);
        });
    }
    window.sendCrsCommand = function(commandName) {
        var CommandType = function() {};
        crsClient.Command(commandName)(CommandType);

        var commandInstance = new CommandType();

        endpoint.send(commandInstance).then( (result) => {
            alert(result);
        }).catch(function(err) {
            alert(err);
        });
    }
    window.reloadMetadata = function() {
        endpoint.reloadMetadata().then(function (metadata) {
            metadataContainer.innerHTML = JSON.stringify(metadata, undefined, 4);
        }).catch(function(err) {
            alert(err);
        });
    }

})();
