var CGO = CGO || {};

CGO.makeConcertViewModel = function(concert) {
    var self = {
        id: concert.id,
        href: concert.href,
        title: ko.observable(concert.title),
        location: ko.observable(concert.location),
        _dateAndStartTime: concert.dateAndStartTime,
        _isPublished: ko.observable(concert.isPublished)
    };

    var dateAndStartTime = ko.computed(function() {
        return $.format.date(new Date(this._dateAndStartTime).toString(), 'd MMM yyyy, HH:mm');
    }, self);
    self.dateAndStartTime = dateAndStartTime;

    var isPublished = ko.computed(function() {
        return this._isPublished() ? 'Yes' : 'No';
    }, self);
    self.isPublished = isPublished;

    return self;
};

CGO.makeConcertModel = function(concertViewModel) {
    return {
        id: concertViewModel.id,
        href: concertViewModel.href,
        title: concertViewModel.title(),
        location: concertViewModel.location(),
        dateAndStartTime: concertViewModel._dateAndStartTime,
        isPublished: concertViewModel._isPublished
    };
}