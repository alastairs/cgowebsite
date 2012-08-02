var CGO = CGO || { };

CGO.makeConcertListViewModel = function ConcertListViewModel(concerts) {
    var makeConcertViewModels = function (concertsJson) {
        return $.map(concertsJson, function (concert) {
            return CGO.makeConcertViewModel(concert);
        });
    };

    var self = {
        concerts: ko.observableArray(makeConcertViewModels(concerts))
    };

    self.quickAdd = function () {
        var makeDateAndStartTime = function(date, startTime) {
            var concertDate = new Date(date);
            concertDate.setTime(startTime);

            return concertDate;
        };

        var concert = {
            Id: 0,
            Title: $("#ConcertViewModel_Title").val(),
            DateAndStartTime: makeDateAndStartTime($("#ConcertViewModel_Date").val(), $("#ConcertViewModel_StartTime").val()),
            Location: $("#ConcertViewModel_Location").val()
        };

        $.post("/api/concerts", concert, function () {
            self.concerts.push(CGO.makeConcertViewModel(concert));
            $("#quickAdd input").each(function() {
                $(this).val('');
            });
        }, "json");
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

    return self;
};