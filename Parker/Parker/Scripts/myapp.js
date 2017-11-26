(function () {

    var availableParkingSpaces = 0;
    var allParkingSpaces = 0
    var intervalDuration = 1000;
    

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

    function getAvailableParkingSpaces(spaces) {
        var availableCount = 0;
        for (var i = 0, len = spaces.length; i < len; i++) {
            if (spaces[i].IsAvailable) {
                availableCount++;
            }
        }
        return availableCount;
    }

    function getNewSensorData() {

        var selectedSensor = getSelectedSensor(window.sensorData);

        $.getJSON(selectedSensor.Url, {})
            .done(function (data) {
                allParkingSpaces = data.ParkingSpaces.length;
                availableParkingSpaces = getAvailableParkingSpaces(data.ParkingSpaces);

                $("#vacant").text(availableParkingSpaces);
                $("#all").text(allParkingSpaces);

                if (data && data.ImageUrl)
                    var imageUrl = data.ImageUrl;
                    var newImage = new Image();
                    newImage.src = imageUrl + '&bla=' + Date.now();
                    newImage.onload = function () {
                        $("#sensorimg").attr("src", imageUrl + '&bla=' + Date.now());
                    }
            });
    }

    $("#sensorselector").change(function () {
        clearInterval(sensorReloadInterval);
        var url = $("#sensorselector option:selected").val();
        var selectedSensor = getSelectedSensor(window.sensorData);
        selectedSensor.IsSelected = false;
        var newSelectedSensor = getSelectedSensorByUrl(window.sensorData, url);
        newSelectedSensor.IsSelected = true;
        getNewSensorData();
        sensorReloadInterval = window.setInterval(function () { getNewSensorData(); }, intervalDuration);
    });

    getNewSensorData();
    var sensorReloadInterval = window.setInterval(function () { getNewSensorData(); }, intervalDuration);

    $("#vacant").text(availableParkingSpaces);
    $("#all").text(allParkingSpaces);

    function imagesPreload() {
        var imgArray = new Array("path/to/img1.jpg", "path/to/img2.jpg", "path/to/img3.jpg");
        for (var i = 0; i < imgArray.length; i++) {
            (new Image()).src = imgArray[i];
        }
    }
})();