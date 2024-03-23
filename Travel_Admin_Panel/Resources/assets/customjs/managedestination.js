
//View Image
$('#destinationlist').on('click', '.btnimage', function () {
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
$('#destinationlist').on('click', '.btnviewdest', function () {
    $('#modal-view-destination .dest-name').html($(this).attr('data-title') == "" ? "NA" : $(this).attr('data-title'));
    $('#modal-view-destination .location').html($(this).attr('data-location') == "" ? "NA" : $(this).attr('data-location'));
    $('#modal-view-destination .dest-description').html($(this).attr('data-description') == "" ? "NA" : $(this).attr('data-description'));

    $('#modal-view-destination').modal('show');
});

//View for Edit
$('#destinationlist').on('click', '.btnviewdested', function () {
    clearError($('#errLbl'));
    $('#modal-edit-destination #Dest_Name').val($(this).attr('data-title'));
    $('#modal-edit-destination #Dest_LocationID').val($(this).attr('data-locationid'));
    $('#modal-edit-destination #Dest_Description').val($(this).attr('data-description') == "" ? "NA" : $(this).attr('data-description'));
    $('#modal-edit-destination #Dest_ID').val($(this).attr('data-ID') == "" ? "NA" : $(this).attr('data-ID'));
    $('#modal-edit-destination #Dest_ImageLink, #modal-edit-destination #uploader ').val($(this).attr('data-image') == "" ? "no file selected" : $(this).attr('data-image'));
    $('#modal-edit-destination #Dest_ThumbLink').val($(this).attr('data-thumb') == "" ? "no file selected" : $(this).attr('data-thumb'));
    $('#modal-edit-destination').modal('show');
});


//Update Code
$('#destinationeditform').submit(function (event) {

    var formData = new FormData(this);
    if (validateEditDestination()) {
        $('#tblLoader').show();
        $.ajax({
            type: "POST",
            url: '/Destination/Edit',
            data: formData,
            dataType: "json",
            contentType: false,
            processData: false,
            success: function (data) {
                destinationEditSuccess(data);
                $('#tblLoader').hide();
            },
            error: function () {
                destinationEditFailure();
                $('#tblLoader').hide();
            }
        });
    }
    event.preventDefault();

});

function validateEditDestination() {
    clearError($('#errLbl'));
    var destinationName = $('#Dest_Name');
    var destinationLocation = $('#Dest_LocationID');
    var destinationImage = $('#Dest_DestinationImage');
    var destinationDescription = $("#Dest_Description");
    
    
     if (destinationName.val().trim() == "") {
        setError($('#errLbl'), "Please enter any Name for destination");
        destinationName.focus();
        return false;
     }
     else if (destinationLocation.val().trim() == "") {
        setError($('#errLbl'), "Please enter any Name for Location");
        destinationName.focus();
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
function destinationEditSuccess(response) {
    if (response[0] == "True") {
        setSuccess($('#errLbl'), response[1]);
        clearTransactionFields();
        setTimeout(function () { $('#modal-edit-destination').modal('hide') }, 2000);
        $('#btnupdatedestlist').trigger('click');

    }
    else {
        setError($('#errLbl'), response[1]);
    }
}

function destinationEditFailure() {
    setError($('#errLbl'), "Unknown error occured!");
}
