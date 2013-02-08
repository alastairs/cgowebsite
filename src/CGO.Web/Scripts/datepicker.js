$('input[type=date]').each(function() {
    var $input = $(this);
    $input.datepicker({
        minDate: $input.attr('min'),
        maxDate: $input.attr('max'),
        dateFormat: 'dd/mm/yy'
    });
});

$('input[type=time]').each(function() {
    var $input = $(this);
    $input.timepickr({
        minDate: $input.attr('min'),
        maxDate: $input.attr('max'),
        dateFormat: 'dd-mm-yy'
    });
});