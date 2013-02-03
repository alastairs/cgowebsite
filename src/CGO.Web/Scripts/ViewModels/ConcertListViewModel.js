var CGO = CGO || { };

CGO.makeConcertListViewModel = function ConcertListViewModel(concerts) {
    var makeConcertViewModels = function(concertsJson) {
        return $.map(concertsJson, function(concert) {
            return CGO.makeConcertViewModel(concert);
        });
    };

    var self = {
        concerts: ko.observableArray(makeConcertViewModels(concerts))
    };

    self.quickAdd = function() {
        var makeDate = function(date, startTime) {
            return Date.parse(date + "T" + startTime);
        };

        var concert = {
            id: 0,
            title: $("#ConcertViewModel_Title").val(),
            dateAndStartTime: makeDate($("#ConcertViewModel_Date").val(), $("#ConcertViewModel_StartTime").val()),
            location: $("#ConcertViewModel_Location").val()
        };

        $.ajax({
            url: "/api/concerts",
            type: "post",
            data: JSON.stringify(concert),
            dataType: "json",
            contentType: "application/json",
        }).done(function (data) {
            concert.id = data.id;
            concert.href = data.href;
            self.concerts.push(CGO.makeConcertViewModel(concert));
            $("#quickAdd input").each(function() {
                $(this).val('');
            });
        });
    };

    self.deleteConcert = function (concert) {
        // TODO: show a modal warning if the concert is published.
        // Should we show it whether or not the concert is published?

        $.ajax({
            type: "DELETE",
            url: "/api/concerts/" + concert.id
        }).done(function() {
            self.concerts.remove(concert);
        });
    };

    self.publishConcert = function (viewModel) {
        var concert = CGO.makeConcertModel(viewModel);
        concert.isPublished = true;
        
        $.ajax({
            type: "PUT",
            url: concert.href,
            data: JSON.stringify(concert),
            dataType: "json",
            contentType: "application/json"
        }).success(function() {
            viewModel._isPublished = true;
        }).error(function() {
            viewModel._isPublished = false;
        });
    };

    return self;
};