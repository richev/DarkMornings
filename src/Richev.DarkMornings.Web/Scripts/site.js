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
    },

    placeMarker: function(lat, lng) {
        var mapPosition = new google.maps.LatLng(lat, lng);

        locationMap.gmap.setCenter(mapPosition);
        locationMap.gmap.setZoom(4);

        locationMap.gmarker = new google.maps.Marker({
            position: mapPosition,
            map: locationMap.gmap,
            title: 'Drag to change!',
            draggable: true,
            animation: google.maps.Animation.DROP
        });

        google.maps.event.addListener(locationMap.gmarker, 'dragend', function (e) {
            $("#x").val(e.latLng.lng().toFixed(2));
            $("#y").val(e.latLng.lat().toFixed(2));
        });
    }
};

var locate = {
    init: function() {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(locate.storePosition, locate.failPosition);
        } else {
            showWarning("<p>You're using a browser that won't tell us your location (or it may just be set this way).</p>Instead, we'll try to find this out using your IP address (less accurate, might not work).</p>");
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
        if (e.code == 1) {
            showWarning("<p>Your browser is set to not tell us your location.</p><p>Instead, we'll try to figure this out using your IP address (less accurate, might not work).</p>");
        } else {
            showWarning("<p>Your location could not be figured out (" + e.message + ").</p><p>Instead, we'll try to find this out using your IP address (less accurate, might not work).</p>");
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

    if ($("#x").val() == "" || $("#y").val() == "") {
        locate.init();
    } else {
        locationMap.placeMarker(parseInt($("#y").val()), parseInt($("#x").val()));
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