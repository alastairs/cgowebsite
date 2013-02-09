function formatDate(date, format) {
    return $.format.date(new Date(date).toString(), format);
}