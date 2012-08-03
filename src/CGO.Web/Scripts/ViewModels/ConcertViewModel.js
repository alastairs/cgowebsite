var CGO = CGO || {};

CGO.makeConcertViewModel = function(concert) {
    var self = {
        id: concert.Id,
        title: concert.Title,
        dateAndStartTime: $.format.date(new Date(concert.StartTime).toString(), "d MMM yyyy, HH:mm"),
        location: concert.Location,
        isPublished: concert.IsPublished ? "Yes" : "No"
    };

    return self;
};