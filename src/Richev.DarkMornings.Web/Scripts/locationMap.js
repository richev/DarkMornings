var locationMap = {
    gmap: null,

    gmarker: null,

    init: function () {
        var mapOptions = {
            center: { lat: 30, lng: 0 },
            zoom: 2,
            minZoom: 2,
            maxZoom: 6,
            mapTypeControl: false,
            streetViewControl: false
        };
        locationMap.gmap = new google.maps.Map(document.getElementById('map-canvas'), mapOptions);

        locationMap.gmarker = new google.maps.Marker({
            map: locationMap.gmap,
            title: 'Drag to change!',
            draggable: true,
            animation: google.maps.Animation.DROP
        });

        if ($("#x").val() != "" && $("#y").val() != "") {
            var latitude = parseInt($("#y").val());
            var longitude = parseInt($("#x").val());
            locationMap.placeMarker(latitude, longitude);
        }

        google.maps.event.addListener(locationMap.gmarker, 'dragend', function (e) {
            $("#x").val(e.latLng.lng().toFixed(2));
            $("#y").val(e.latLng.lat().toFixed(2));
        });
    },

    placeMarker: function (lat, lng) {
        var mapPosition = new google.maps.LatLng(lat, lng);

        locationMap.gmarker.setPosition(mapPosition);

        locationMap.gmap.setCenter(mapPosition);
        locationMap.gmap.setZoom(4);
    },

    placeDefaultMarker: function () {
        // Sticks a marker on London, for when we don't know where the user is located.

        $("#x").val(0.12);
        $("#y").val(51.51);

        locationMap.placeMarker(51.5072, 0.1205);

        locationMap.gmap.setZoom(2);
    }
};