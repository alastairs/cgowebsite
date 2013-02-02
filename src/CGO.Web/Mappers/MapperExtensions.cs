using System;

using CGO.Web.Models;
using CGO.Web.ViewModels;

namespace CGO.Web.Mappers
{
    public static class MapperExtensions
    {
        public static TViewModel ToViewModel<TModel, TViewModel>(this TModel model) where TViewModel: ConcertViewModel where TModel: Concert
        {
            if (typeof(TModel) == typeof(Concert))
            {
                return (TViewModel) new ConcertViewModel
                {
                    Id = model.Id,
                    Title = model.Title,
                    Date = model.DateAndStartTime,
                    StartTime = model.DateAndStartTime,
                    Location = model.Location,
                    Description = model.Description,
                    IsPublished = model.IsPublished
                };
            }

            return null;
        }

        public static TModel ToModel<TModel, TViewModel>(this TViewModel viewModel) where TViewModel: ConcertViewModel where TModel: Concert
        {
            if (typeof(TViewModel) == typeof(ConcertViewModel))
            {
                var dateAndStartTime = new DateTime(viewModel.Date.Year, viewModel.Date.Month, viewModel.Date.Day,
                                                    viewModel.StartTime.Hour, viewModel.StartTime.Minute, 00);
                var model = (TModel) new Concert(viewModel.Id, viewModel.Title, dateAndStartTime, viewModel.Location)
                                         {
                                             Description = viewModel.Description
                                         };

                if (viewModel.IsPublished)
                {
                    model.Publish();
                }

                return model;
            }

            return null;
        }
    }
}