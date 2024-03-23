function setError(control, message) {
    control.addClass('alert-danger').removeClass('alert-success').removeClass('display-hide');
    control.find('span').html(message);
}

function setSuccess(control, message) {
    control.addClass('alert-success').removeClass('alert-danger').removeClass('display-hide');
    control.find('span').html(message);
}

function clearError(control) {
    control.addClass('display-hide');
    control.find('span').html('');
}

function clearTransactionFields() {
    $('input').val('');
    $('input').removeAttr('data-val');
    $('input').removeAttr('disabled');
    $('select').prop('selectedIndex', 0);
    $('textarea').val('');
}
function changeActive(child) {
    $('.nav-sidebar li').removeClass('active nav-active');
    $('.nav-sidebar> li:nth-child(' + child + ')').addClass('active nav-active');
}