function storePosition(position) {
    var latitude = position.coords.latitude;
    var longitude = position.coords.longitude;

    $("#x").val(longitude.toFixed(2));
    $("#y").val(latitude.toFixed(2));
}

function failPosition(e) {
    if (e.code == 1) {
        showWarning("<p>Your browser is set to not tell us your location.</p><p>Instead, we'll try to figure this out using your IP address (less accurate, might not work).</p>");
    } else {
        showWarning("<p>Your location could not be figured out (" + e.message + ").</p><p>Instead, we'll try to find this out using your IP address (less accurate, might not work).</p>");
    }
}

var setToWorkTime = function() {
    $("#t").val($("#ToWorkHours").val() + $("#ToWorkMinutes").val());
};

var setFromWorkTime = function() {
    $("#f").val($("#FromWorkHours").val() + $("#FromWorkMinutes").val());
};

var setWorkingDaysValue = function () {
    var buttons = $(".working-days .btn-group button");

    var workingDays = 0;

    buttons.each(function (i, elem) {
        if ($(elem).hasClass("checked")) {
            workingDays += Math.pow(2, i); // workingDays is a per-day bit-field
        }
    });

    $("#d").val(workingDays);
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

// From http://michaelapproved.com/articles/timezone-detect-and-ignore-daylight-saving-time-dst
var timezoneDetect = function () {
    var dtDate = new Date('1/1/' + (new Date()).getUTCFullYear());
    var intOffset = 10000; //set initial offset high so it is adjusted on the first attempt
    var intMonth;

    //go through each month to find the lowest offset to account for DST
    for (intMonth = 0; intMonth < 12; intMonth++) {
        //go to the next month
        dtDate.setUTCMonth(dtDate.getUTCMonth() + 1);

        //To ignore daylight saving time look for the lowest offset.
        //Since, during DST, the clock moves forward, it'll be a bigger number.
        if (intOffset > (dtDate.getTimezoneOffset() * (-1))) {
            intOffset = (dtDate.getTimezoneOffset() * (-1));
        }
    }

    return intOffset / 60; // to adjust minutes to hours
};

var showWarning = function (msgHtml) {
    $("#warning").append(msgHtml);
    $("#warning").show();
};

$(document).ready(function () {
    if ($("#x").val() == "" || $("#y").val() == "") {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(storePosition, failPosition);
        } else {
            showWarning("<p>You're using a browser that won't tell us your location (or it may just be set this way).</p>Instead, we'll try to find this out using your IP address (less accurate, might not work).</p>");
        }
    }

    setWorkingDaysButtons();

    if ($("#z").val() == "") {
        $("#z").val(timezoneDetect());
    }

    $("#ToWorkHours").click(setToWorkTime);
    $("#ToWorkMinutes").click(setToWorkTime);

    $("#FromWorkHours").click(setFromWorkTime);
    $("#FromWorkMinutes").click(setFromWorkTime);

    $(".working-days button").click(function () {
        $(this).toggleClass("checked");

        setWorkingDaysValue();

        return false;
    });

    if ($("a#Results").length > 0) {
        // We have results, so scroll to them
        $('html,body').delay(250).animate({
            scrollTop: $("a#Results").offset().top - $(".navbar").height()
        }, 500);
    }
});