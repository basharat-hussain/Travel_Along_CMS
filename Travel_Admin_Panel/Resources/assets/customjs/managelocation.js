
//View Image
$('#locationlist').on('click', '.btnimage', function () {
    if ($(this).attr('data-image') == "") {
        alert("Sorry no image found");
    }
    else {
        $('#modal-image .modal-body img').attr('src', $(this).attr('data-image'));
        $('#modal-image .modal-footer p').html($(this).attr('data-title'));
        $('#modal-image').modal('show');
    }
});

//View Details
$('#locationlist').on('click', '.btnviewlocation', function () {
    $('#modal-view-location .location').html($(this).attr('data-location') == "" ? "NA" : $(this).attr('data-location'));
    $('#modal-view-location .loc-description').html($(this).attr('data-description') == "" ? "NA" : $(this).attr('data-description'));

    $('#modal-view-location').modal('show');
});

//View for Edit
$('#locationlist').on('click', '.btnviewlocationed ', function () {
    clearError($('#errLbl'));
    $('#modal-edit-location #Loc_Name').val($(this).attr('data-name'));
    $('#modal-edit-location #Loc_Description').val($(this).attr('data-description') == "" ? "NA" : $(this).attr('data-description'));
    $('#modal-edit-location #Loc_ID').val($(this).attr('data-ID') == "" ? "NA" : $(this).attr('data-ID'));
    $('#modal-edit-location #Loc_ImageLink, #modal-edit-location #uploader ').val($(this).attr('data-image') == "" ? "no file selected" : $(this).attr('data-image'));
    $('#modal-edit-location').modal('show');
});


//Update Code
$('#locationeditform').submit(function (event) {
    debugger;
    var formData = new FormData(this);
    if (validateEditLocation()) {
        $('#tblLoader').show();
        $.ajax({
            type: "POST",
            url: '/Location/Edit',
            data: formData,
            dataType: "json",
            contentType: false,
            processData: false,
            success: function (data) {
                locationEditSuccess(data);
                $('#tblLoader').hide();
            },
            error: function () {
                locationEditFailure();
                $('#tblLoader').hide();
            }
        });
    }
    event.preventDefault();

});

function validateEditLocation() {
    clearError($('#errLbl'));
    var locationName = $('#Loc_Name');
    var locationDescription = $('#Loc_Description');
    var locationImage = $('#Loc_LocationImage');

    if (locationName.val().trim() == "") {
        setError($('#errLbl'), "Please enter any Description for Location");
        locationName.focus();
        return false;
    }
    else if (locationDescription.val().trim() == "") {
        setError($('#errLbl'), "Please enter any Description for Location");
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
$('#Loc_LocationImage').change(function () {
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
        var fileSize = (control[0].files[0].size)/1024;

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
function locationEditSuccess(response) {
    if (response[0] == "True") {
        setSuccess($('#errLbl'), response[1]);
        clearTransactionFields();
        setTimeout(function () { $('#modal-edit-location').modal('hide') }, 2000);
        $('#btnupdatelocationlist').trigger('click');

    }
    else {
        setError($('#errLbl'), response[1]);
    }
}

function locationEditFailure() {
    setError($('#errLbl'), "Unknown error occured!");
}
