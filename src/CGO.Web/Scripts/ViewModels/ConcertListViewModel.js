var CGO = CGO || { };

CGO.makeConcertListViewModel = function ConcertListViewModel() {
    var self = this;
    self.concerts = ko.observableArray([]);
    self.newConcertTitle = ko.observable();
    self.newConcertDate = ko.observable();
    self.newConcertStartTime = ko.observable();
    self.newConcertLocation = ko.observable();

    self.quickAdd = function() {
        if (!$("#quickAdd").valid()) {
            return;
        }

        var makeDate = function(date, startTime) {
            return Date.parse(date + "T" + startTime);
        };

        var concert = CGO.makeConcertViewModel({
            title: this.newConcertTitle(),
            dateAndStartTime: makeDate(this.newConcertDate(),
                this.newConcertStartTime()),
            location: this.newConcertLocation()
        });

        $.ajax({
            url: "/api/concerts",
            type: "post",
            data: JSON.stringify(CGO.makeConcertModel(concert)),
            dataType: "json",
            contentType: "application/json",
        }).done(function(data) {
            concert.id(data.id);
            concert.href(data.href);

            self.concerts.push(concert);

            self.newConcertTitle("");
            self.newConcertDate("");
            self.newConcertStartTime("");
            self.newConcertLocation("");
        });
    };

    self.deleteConcert = function(concert) {
        // TODO: show a modal warning if the concert is published.
        // Should we show it whether or not the concert is published?

        $.ajax({
            type: "DELETE",
            url: "/api/concerts/" + concert.id()
        }).done(function() {
            self.concerts.remove(concert);
        });
    };

    self.publishConcert = function(viewModel) {
        var concert = CGO.makeConcertModel(viewModel);
        concert.isPublished = true;

        $.ajax({
            type: "PUT",
            url: concert.href,
            data: JSON.stringify(concert),
            dataType: "json",
            contentType: "application/json"
        }).success(function() {
            viewModel.isPublished(true);
        }).error(function() {
            viewModel.isPublished(false);
        });
    };

    $.getJSON("/api/concerts", function(concertsJson) {
        var mappedConcerts = $.map(concertsJson, function(concert) {
            return CGO.makeConcertViewModel(concert);
        });
        self.concerts(mappedConcerts);
    });

    return self;
};