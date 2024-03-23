$('#hotelcreateform').submit(function (event) {

    var formData = new FormData(this);
    if (validateAddHotel()) {
        $('#tblLoader').show();
        $.ajax({
            type: "POST",
            url: '/Hotel/Add',
            data: formData,
            dataType: "json",
            contentType: false,
            processData: false,
            success: function (data) {
                hotelAddSuccess(data);
                $('#tblLoader').hide();
            },
            error: function () {
                hotelAddFailure();
                $('#tblLoader').hide();
            }
        });
    }
    event.preventDefault();

});

function hotelAddSuccess(response) {
    if (response[0] == "True") {
        setSuccess($('#errLbl'), response[1]);
        clearTransactionFields();
    }
    else {
        setError($('#errLbl'), response[1]);
    }
}

function hotelAddFailure() {
    setError($('#errLbl'), "Unknown error occured!");
}

function validateAddHotel() {
    clearError($('#errLbl'));
    var hotelLocation = $('#Htl_LocationID');
    var hotelName = $('#Htl_Name');
    var hotelImage = $('#Htl_HotelImage');
    var hotelAddress = $("#Htl_Address");
    var hotelCategory = $("#Htl_HotelCategory");
    var hotelDescription = $("#Htl_Description");
    var hotelFeatures = $("#Htl_Features");

    if ($('#Htl_LocationID option:selected').index() == 0) {
        setError($('#errLbl'), "Please select any location");
        hotelLocation.focus();
        return false;
    }
    else if (hotelName.val().trim() == "") {
        setError($('#errLbl'), "Please enter any name for Hotel");
        hotelName.focus();
        return false;
    }
    else if (hotelImage.val() == "") {
        setError($('#errLbl'), "Please select any image");
        hotelImage.focus();
        return false;
    }
    else if (hotelAddress.val() == "") {
        setError($('#errLbl'), "Please enter hotel address");
        hotelAddress.focus();
        return false;
    }
    else if ($('#Htl_HotelCategory option:selected').index() == 0) {
        setError($('#errLbl'), "Please select any category");
        hotelCategory.focus();
        return false;
    }

    else if (hotelDescription.val().trim() == "") {
        setError($('#errLbl'), "Please enter hotel description");
        hotelDescription.focus();
        return false;
    }
    else if (hotelFeatures.val().trim() == "") {
        setError($('#errLbl'), "Please enter hotel features");
        hotelFeatures.focus();
        return false;
    }

    else if (hotelImage.val() != "") {
        return fileValidation(hotelImage, "jpg", "200", $('#errLbl'));
    }
    else {
        return true;
    }

}


//File parameters
var _URL = window.URL || window.webkitURL;
var imgWidth = 0;
var imgHeight = 0;
$('#Htl_HotelImage').change(function () {
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