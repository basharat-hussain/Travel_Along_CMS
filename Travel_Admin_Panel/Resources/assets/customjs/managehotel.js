//View Image
$('#hotellist').on('click', '.btnimage', function () {
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
$('#hotellist').on('click', '.btnviewhotel', function () {
    $('#modal-view-hotel .location').html($(this).attr('data-location') == "" ? "NA" : $(this).attr('data-location'));
    $('#modal-view-hotel .htl-name').html($(this).attr('data-name') == "" ? "NA" : $(this).attr('data-name'));
    $('#modal-view-hotel .htl-address').html($(this).attr('data-address') == "" ? "NA" : $(this).attr('data-address'));
    $('#modal-view-hotel .htl-category').html($(this).attr('data-category') == "" ? "NA" : $(this).attr('data-category'));
    $('#modal-view-hotel .htl-description').html($(this).attr('data-hotel-desc') == "" ? "NA" : $(this).attr('data-hotel-desc'));
    $('#modal-view-hotel .htl-features').html($(this).attr('data-features') == "" ? "NA" : $(this).attr('data-features'));
   
    $('#modal-view-hotel').modal('show');
});

//View for Edit
$('#hotellist').on('click', '.btnviewhoted', function () {
    clearError($('#errLbl'));
    $('#modal-edit-hotel #Htl_ID').val($(this).attr('data-id'));
    $('#modal-edit-hotel #Htl_LocationID').val($(this).attr('data-locationid'));    
    $('#modal-edit-hotel #Htl_Name').val($(this).attr('data-name') == "" ? "NA" : $(this).attr('data-name'));
    $('#modal-edit-hotel #Htl_Address').val($(this).attr('data-address') == "" ? "NA" : $(this).attr('data-address'));
    $('#modal-edit-hotel #Htl_HotelCategory').val($(this).attr('data-category'));
    $('#modal-edit-hotel #Htl_Description').val($(this).attr('data-hotel-desc') == "" ? "NA" : $(this).attr('data-hotel-desc'));
    $('#modal-edit-hotel #Htl_ImageLink, #modal-edit-hotel #uploader ').val($(this).attr('data-image') == "" ? "no file selected" : $(this).attr('data-image'));
    $('#modal-edit-hotel #Htl_ThumbLink').val($(this).attr('data-thumb') == "" ? "no file selected" : $(this).attr('data-thumb'));
    $('#modal-edit-hotel #Htl_Features').val($(this).attr('data-features') == "" ? "NA" : $(this).attr('data-features'));
    $('#modal-edit-hotel').modal('show');
});


$('#hoteleditform').submit(function (event) {

    var formData = new FormData(this);
    if (validateEditHotel()) {
        $('#tblLoader').show();
        $.ajax({
            type: "POST",
            url: '/Hotel/Edit',
            data: formData,
            dataType: "json",
            contentType: false,
            processData: false,
            success: function (data) {
                hotelEditSuccess(data);
                $('#tblLoader').hide();
            },
            error: function () {
                hotelEditFailure();
                $('#tblLoader').hide();
            }
        });
    }
    event.preventDefault();

});

function validateEditHotel() {
    clearError($('#errLbl'));
    var hotelName = $('#Htl_Name');
    var hotelLocation = $('#Htl_LocationID');
    var hotelImage = $('#Htl_HotelImage');
    var hotelDescription = $("#Htl_Description");

    if (hotelName.val().trim() == "") {
        setError($('#errLbl'), "Please enter any name for hotel");
        hotelName.focus();
        return false;
    }
    else if (hotelLocation.val().trim() == "") {
        setError($('#errLbl'), "Please select any location");
        hotelLocation.focus();
        return false;
    }
    else if (hotelDescription.val().trim() == "") {
        setError($('#errLbl'), "Please enter hotel description");
        hotelDescription.focus();
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
function hotelEditSuccess(response) {
    if (response[0] == "True") {
        setSuccess($('#errLbl'), response[1]);
        clearTransactionFields();
        setTimeout(function () { $('#modal-edit-hotel').modal('hide') }, 2000);
        $('#btnupdatehtllist').trigger('click');

    }
    else {
        setError($('#errLbl'), response[1]);
    }
}

function hotelEditFailure() {
    setError($('#errLbl'), "Unknown error occured!");
}
