$('#destinationcreateform').submit(function (event) {

    var formData = new FormData(this);
    if (validateAddDestination()) {
        $('#tblLoader').show();
        $.ajax({
            type: "POST",
            url: '/Destination/Add',
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

function validateAddDestination() {
    clearError($('#errLbl'));
    var destinationLocation = $('#Dest_LocationID');
    var destinationName = $('#Dest_Name');
    var destinationImage = $('#Dest_DestinationImage');
    var destinationDescription = $("#Dest_Description");

    if ($('#Dest_LocationID option:selected').index() == 0) {
        setError($('#errLbl'), "Please select any location");
        destinationLocation.focus();
        return false;
    }
    else if (destinationName.val().trim() == "") {
        setError($('#errLbl'), "Please enter any name for destination");
        destinationName.focus();
        return false;
    }
    else if (destinationImage.val() == "") {
        setError($('#errLbl'), "Please select any image");
        destinationImage.focus();
        return false;
    }
    else if (destinationDescription.val().trim() == "") {
        setError($('#errLbl'), "Please enter destination description");
        destinationDescription.focus();
        return false;
    }
   
    else if (destinationImage.val() != "") {
        return fileValidation(destinationImage, "jpg", "200", $('#errLbl'));
    }
    else {
        return true;
    }

}


//File parameters
var _URL = window.URL || window.webkitURL;
var imgWidth = 0;
var imgHeight = 0;
$('#Dest_DestinationImage').change(function () {
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
        else if (imgWidth != 850 || imgHeight != 550) {
            setError(errorControl, "Please select a file with width 850px and height 550px");
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