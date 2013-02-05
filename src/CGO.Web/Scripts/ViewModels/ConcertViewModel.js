var CGO = CGO || {};

CGO.makeConcertViewModel = function(concert) {
    var self = {
        id: ko.observable(concert.id),
        href: ko.observable(concert.href),
        title: ko.observable(concert.title),
        location: ko.observable(concert.location),
        _dateAndStartTime: concert.dateAndStartTime,
        isPublished: ko.observable(concert.isPublished)
    };

    self.dateAndStartTime = ko.computed(function() {
        return $.format.date(new Date(this._dateAndStartTime).toString(), 'd MMM yyyy, HH:mm');
    }, self);

    return self;
};

CGO.makeConcertModel = function(concertViewModel) {
    return {
        id: concertViewModel.id(),
        href: concertViewModel.href(),
        title: concertViewModel.title(),
        location: concertViewModel.location(),
        dateAndStartTime: concertViewModel._dateAndStartTime,
        isPublished: concertViewModel.isPublished
    };
}