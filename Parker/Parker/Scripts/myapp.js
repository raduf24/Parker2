(function () {

    function getSelectedSensor(sensors) {
        for (var i = 0, len = sensors.length; i < len; i++) {
            if (sensors[i].IsSelected) return sensors[i];
        }
    }

    function getSelectedSensorByUrl(sensors, url) {
        for (var i = 0, len = sensors.length; i < len; i++) {
            if (sensors[i].Url == url) {
                return sensors[i];
            }
        }
    }

    function getNewSensorData() {

        var sensorData = getSelectedSensor(window.sensorData);

        $.getJSON(sensorData.Url, {
            tags: "",
            tagmode: "any",
            format: "json"
        })
            .done(function (data) {
                if (data && data.items && data.items[0] && data.items[0].media)
                    var imageUrl = data.items[0].media.m;
                $("#sensorimg").attr("src", imageUrl);
            });
    }

    $("#sensorselector").change(function () {
        sensorReloadInterval = null;
        var url = $("#sensorselector option:selected").val();
        var selectedSensor = getSelectedSensor(window.sensorData);
        selectedSensor.IsSelected = false;
        var newSelectedSensor = getSelectedSensorByUrl(window.sensorData, url);
        newSelectedSensor.IsSelected = true;
        getNewSensorData();
        sensorReloadInterval = window.setInterval(function () { getNewSensorData(); }, 1000);
    });

    getNewSensorData();
    var sensorReloadInterval = window.setInterval(function () { getNewSensorData(); }, 1000);
})();