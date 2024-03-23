$('#locationaddform').submit(function (event) {

    var formData = new FormData(this);
    if (validateAddLocation()) {
        $('#tblLoader').show();
        $.ajax({
            type: "POST",
            url: '/Location/Add',
            data: formData,
            dataType: "json",
            contentType: false,
            processData: false,
            success: function (data) {
                destinationAddSuccess(data);
                $('#tblLoader').hide();
            },
            error: function () {
                destinationAddFailure();
                $('#tblLoader').hide();
            }
        });
    }
    event.preventDefault();

});

function destinationAddSuccess(response) {
    if (response[0] == "True") {
        setSuccess($('#errLbl'), response[1]);
        clearTransactionFields();
    }
    else {
        setError($('#errLbl'), response[1]);
    }
}

function destinationAddFailure() {
    setError($('#errLbl'), "Unknown error occured!");
}

function validateAddLocation() {
    clearError($('#errLbl'));
    var locationName = $('#Name');
    var locationImage = $('#LocationImage');
    var locationDescription = $("#Description");

    if (locationName.val().trim() == "") {
        setError($('#errLbl'), "Please enter any name for location");
        locationName.focus();
        return false;
    }
    else if (locationImage.val() == "") {
        setError($('#errLbl'), "Please select any image");
        locationImage.focus();
        return false;
    }

    else if (locationDescription.val().trim() == "") {
        setError($('#errLbl'), "Please enter location description");
        locationDescription.focus();
        return false;
    }
    else if (locationImage.val() != "") {
        return fileValidation(locationImage, "jpg", "200", $('#errLbl'));
    }
    else {
        return true;
    }

}


//File parameters
var _URL = window.URL || window.webkitURL;
var imgWidth = 0;
var imgHeight = 0;
$('#LocationImage').change(function () {
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