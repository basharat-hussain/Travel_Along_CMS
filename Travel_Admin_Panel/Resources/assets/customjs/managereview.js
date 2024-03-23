


//View review
$('#reviewlist').on('click', '.btnviewreview', function () {
    $('#modal-view-review .rev-name').html($(this).attr('data-name') == "" ? "NA" : $(this).attr('data-name'));
    $('#modal-view-review .rev-title').html($(this).attr('data-title') == "" ? "NA" : $(this).attr('data-title'));
    $('#modal-view-review .rev-address').html($(this).attr('data-address') == "" ? "NA" : $(this).attr('data-address'));


    var Rating = $(this).attr('data-rating');
    var htm = "";
    for (i = 1; i <= Rating; i++) {
        htm += "<i class='fa fa-star'></i> ";
    }
    for (i = Rating; i < 5; i++) {
        htm += "<i class='fa fa-star-o'></i> ";
    }
    
    $('#modal-view-review .rev-rating').html(htm);
    $('#modal-view-review .rev-description').html($(this).attr('data-description') == "" ? "NA" : $(this).attr('data-description'));

    $('#modal-view-review').modal('show');
});



