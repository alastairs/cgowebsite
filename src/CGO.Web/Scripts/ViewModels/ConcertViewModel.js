var CGO = CGO || {};

CGO.makeConcertViewModel = function(concert) {
    var self = {
        id: concert.Id,
        title: concert.Title,
        dateAndStartTime: $.format.date(concert.DateAndStartTime, "d MMM yyyy, HH:mm"),
        location: concert.Location
    };

    return self;
};