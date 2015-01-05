var locate = {
    init: function() {
        if (navigator.geolocation) {
            if ($("#x").val() == "" || $("#y").val() == "") {
                locationMap.placeDefaultMarker();
            }

            navigator.geolocation.getCurrentPosition(locate.storePosition, locate.failPosition);
        } else {
            if ($("#x").val() == "" || $("#y").val() == "") {
                // Problematic...we couldn't figure out the user's location by IP address, and their browser won't tell us either...
                showWarning("<p>Please select your location by dragging the marker on the map below.</p>");
                locationMap.placeDefaultMarker();
            }
        }
    },

    storePosition: function(position) {
        var latitude = position.coords.latitude;
        var longitude = position.coords.longitude;

        $("#x").val(longitude.toFixed(2));
        $("#y").val(latitude.toFixed(2));

        locationMap.placeMarker(latitude, longitude);
    },

    failPosition: function(e) {
        if ($("#x").val() == "" || $("#y").val() == "") {
            // The user won't let their browser tell us their location, and we couldn't get it by IP address...
            showWarning("<p>Please select your location by dragging the marker on the map below.</p>");
            locationMap.placeDefaultMarker();
        }
    },

    showWarning: function(msgHtml) {
        $("#warning").append(msgHtml);
        $("#warning").show();
    }
};