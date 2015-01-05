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