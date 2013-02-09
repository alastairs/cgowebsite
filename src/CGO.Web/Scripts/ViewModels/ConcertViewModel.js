var CGO = CGO || {};

CGO.makeConcertViewModel = function(concert) {
    var self = {
        id: ko.observable(concert.id),
        href: ko.observable(concert.href),
        title: ko.observable(concert.title),
        location: ko.observable(concert.location),
        dateAndStartTime: concert.dateAndStartTime,
        isPublished: ko.observable(concert.isPublished)
    };

    return self;
};

CGO.makeConcertModel = function(concertViewModel) {
    return {
        id: concertViewModel.id(),
        href: concertViewModel.href(),
        title: concertViewModel.title(),
        location: concertViewModel.location(),
        dateAndStartTime: concertViewModel.dateAndStartTime,
        isPublished: concertViewModel.isPublished
    };
}