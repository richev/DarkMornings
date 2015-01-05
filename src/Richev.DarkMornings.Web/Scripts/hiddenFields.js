var hiddenFields = {
    setToWorkTime: function () {
        $("#h").val($("#ToWorkHours").val() + $("#ToWorkMinutes").val());
    },

    setFromWorkTime: function () {
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