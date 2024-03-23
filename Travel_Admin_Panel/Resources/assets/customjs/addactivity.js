$('#activitycreateform').submit(function (event) {

    var formData = new FormData(this);
    if (validateAddActivity()) {
        $('#tblLoader').show();
        $.ajax({
            type: "POST",
            url: '/Activity/Add',
            data: formData,
            dataType: "json",
            contentType: false,
            processData: false,
            success: function (data) {
                activityAddSuccess(data);
                $('#tblLoader').hide();
            },
            error: function () {
                activityAddFailure();
                $('#tblLoader').hide();
            }
        });
    }
    event.preventDefault();

});

function activityAddSuccess(response) {
    if (response[0] == "True") {
        setSuccess($('#errLbl'), response[1]);
        clearTransactionFields();
    }
    else {
        setError($('#errLbl'), response[1]);
    }
}

function activityAddFailure() {
    setError($('#errLbl'), "Unknown error occured!");
}

function validateAddActivity() {
    clearError($('#errLbl'));
    var activityLocation = $('#Actvty_LocationID');
    var activityName = $('#Actvty_Title');
    var activityImage = $('#Actvty_ActivityImage');
    var activityDescription = $("#Actvty_Description");

    if ($('#Actvty_LocationID option:selected').index() == 0) {
        setError($('#errLbl'), "Please select any location");
        activityLocation.focus();
        return false;
    }
    else if (activityName.val().trim() == "") {
        setError($('#errLbl'), "Please enter any name for Activity");
        activityName.focus();
        return false;
    }
    else if (activityImage.val() == "") {
        setError($('#errLbl'), "Please select any image");
        activityImage.focus();
        return false;
    }
    else if (activityDescription.val().trim() == "") {
        setError($('#errLbl'), "Please enter activity description");
        activityDescription.focus();
        return false;
    }

    else if (activityImage.val() != "") {
        return fileValidation(activityImage, "jpg", "200", $('#errLbl'));
    }
    else {
        return true;
    }

}


//File parameters
var _URL = window.URL || window.webkitURL;
var imgWidth = 0;
var imgHeight = 0;
$('#Actvty_ActivityImage').change(function () {
    var file = $(this)[0].files[0];

    img = new Image();
    img.src = _URL.createObjectURL(file);
    img.onload = function () {
        imgWidth = this.width;
        imgHeight = this.height;
    }

});
function fileValidation(control, extension, size, errorControl) {

    if (control.val() != "" && size != "") {
        var fileExtension = control.val().split('.').pop();
        var fileSize = (control[0].files[0].size / 1024);

        if (fileExtension.toUpperCase() != extension.toUpperCase()) {
            setError(errorControl, "Invalid file selected (" + fileExtension + "), only " + extension + " files allowed");
            return false;
        }
        else if (imgWidth != 350 || imgHeight != 250) {
            setError(errorControl, "Please select a file with width 350px and height 250px");
            return false;
        }
        else if (fileSize > size) {
            setError(errorControl, "Please select a file less than " + size + " KB");
            return false;
        }
        else {
            return true;
        }
    }
    else {
        setError(errorControl, "Please select a file");
        return false;
    }




}