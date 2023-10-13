$("#SearchDriver").autocomplete({
    source: function (request, response) {
        $.ajax({
            headers: { RequestVerificationToken: $("#RequestCsrfToken").val() },
            datatype: 'json',
            url: 'Stats/SearchDriver',
            data: { searchDriver: request.term },
            success: function (data) { response(data) }
        })
    }
})