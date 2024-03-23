$('#transportaddform').submit(function (event) {

    var formData = new FormData(this);
    if (validateAddTransport()) {
        $('#tblLoader').show();
        $.ajax({
            type: "POST",
            url: '/Transport/Add',
            data: formData,
            dataType: "json",
            contentType: false,
            processData: false,
            success: function (data) {
                TransportAddSuccess(data);
                $('#tblLoader').hide();
            },
            error: function () {
                TransportAddFailure();
                $('#tblLoader').hide();
            }
        });
    }
    event.preventDefault();

});

function TransportAddSuccess(response) {
    if (response[0] == "True") {
        setSuccess($('#errLbl'), response[1]);
        clearTransactionFields();
    }
    else {
        setError($('#errLbl'), response[1]);
    }
}

function TransportAddFailure() {
    setError($('#errLbl'), "Unknown error occured!");
}

function validateAddTransport() {
    debugger;
    clearError($('#errLbl'));
    var transportName = $('#Name');
    var transportImage = $('#TransportImage');
    var transportFeatures = $("#Features");
    var transportRate = $("#Rate");

    if (transportName.val().trim() == "") {
        setError($('#errLbl'), "Please enter any name for Transport");
        transportName.focus();
        return false;
    }
    else if (transportImage.val() == "") {
        setError($('#errLbl'), "Please select any image");
        transportImage.focus();
        return false;
    }

    else if (transportRate.val().trim() == "") {
        setError($('#errLbl'), "Please enter Transport Rate");
        transportRate.focus();
        return false;
    }
    else if (transportFeatures.val().trim() == "") {
        setError($('#errLbl'), "Please enter Transport Features");
        transportFeatures.focus();
        return false;
    }
    else if (transportImage.val() != "") {
        return fileValidation(transportImage, "jpg", "200", $('#errLbl'));
    }

    else {
        return true;
    }

}


//File parameters
var _URL = window.URL || window.webkitURL;
var imgWidth = 0;
var imgHeight = 0;
$('#TransportImage').change(function () {
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
        else if (imgWidth != 300 || imgHeight != 190) {
            setError(errorControl, "Please select a file with width 300px and height 190px");
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