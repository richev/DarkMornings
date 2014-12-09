var locationMap = {
    gmap: null,

    gmarker: null,

    init: function() {
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

    placeMarker: function(lat, lng) {
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

    storePosition: function (position) {
        var latitude = position.coords.latitude;
        var longitude = position.coords.longitude;

        $("#x").val(longitude.toFixed(2));
        $("#y").val(latitude.toFixed(2));

        locationMap.placeMarker(latitude, longitude);
    },

    failPosition: function (e) {
        if ($("#x").val() == "" || $("#y").val() == "") {
            // The user won't let their browser tell us their location, and we couldn't get it by IP address...
            showWarning("<p>Please select your location by dragging the marker on the map below.</p>");
            locationMap.placeDefaultMarker();
        }
    }
};

var hiddenFields = {
    setToWorkTime: function() {
        $("#h").val($("#ToWorkHours").val() + $("#ToWorkMinutes").val());
    },

    setFromWorkTime: function() {
        $("#w").val($("#FromWorkHours").val() + $("#FromWorkMinutes").val());
    },

    setWorkingDays: function () {
        var buttons = $(".working-days .btn-group button");

        var workingDays = 0;

        buttons.each(function (i, elem) {
            if ($(elem).hasClass("checked")) {
                workingDays += Math.pow(2, i); // workingDays is a per-day bit-field
            }
        });

        $("#d").val(workingDays);
    }
};

var setWorkingDaysButtons = function () {
    var buttons = $(".working-days .btn-group button");
    var workingDays = parseInt($("#d").val());

    buttons.each(function (i, elem) {
        $(elem).removeClass("checked");

        // Slightly cryptic - workingDays is a bit-field so we do a per-day little calculation
        // to figure out which days are selected
        if (Math.pow(2, i) & workingDays) {
            $(buttons[i]).addClass("checked");
        }
    });
};

var showWarning = function (msgHtml) {
    $("#warning").append(msgHtml);
    $("#warning").show();
};

$(document).ready(function () {
    locationMap.init();

    if ($("a#Results").length == 0) {
        // No results yet, so let's locate!
        locate.init();
    }

    setWorkingDaysButtons();

    $("#ToWorkHours").click(hiddenFields.setToWorkTime);
    $("#ToWorkMinutes").click(hiddenFields.setToWorkTime);

    $("#FromWorkHours").click(hiddenFields.setFromWorkTime);
    $("#FromWorkMinutes").click(hiddenFields.setFromWorkTime);

    $(".working-days button").click(function () {
        $(this).toggleClass("checked");

        hiddenFields.setWorkingDays();

        return false;
    });

    if ($("a#Results").length > 0) {
        // We have results, so scroll to them
        $('html,body').delay(250).animate({
            scrollTop: $("a#Results").offset().top - $(".navbar").height()
        }, 500);
    }
});