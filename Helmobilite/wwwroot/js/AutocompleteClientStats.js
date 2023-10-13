$("#SearchClient").autocomplete({
    source: function (request, response) {
        $.ajax({
            headers: { RequestVerificationToken: $("#RequestCsrfToken").val() },
            datatype: 'json',
            url: 'Stats/SearchClient',
            data: { searchClient: request.term },
            success: function (data) { response(data) }
        })
    }
})