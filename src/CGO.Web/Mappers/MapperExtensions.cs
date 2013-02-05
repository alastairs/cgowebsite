using System;
using AutoMapper;
using CGO.Web.Models;

namespace CGO.Web.Mappers
{
    public static class MapperExtensions
    {
        static MapperExtensions()
        {
            Mapper.CreateMap<Concert, ViewModels.ConcertViewModel>()
                  .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.DateAndStartTime))
                  .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.DateAndStartTime));
            Mapper.CreateMap<ViewModels.ConcertViewModel, Concert>()
                  .ConstructUsing(vm => new Concert(vm.Id, vm.Title, GetDateAndStartTimeFromViewModel(vm.Date, vm.StartTime), vm.Location));

            Mapper.CreateMap<Concert, ViewModels.Api.ConcertViewModel>()
                  .ForMember(dest => dest.Href, opt => opt.MapFrom(src => "/api/concerts/" + src.Id));
            Mapper.CreateMap<ViewModels.Api.ConcertViewModel, Concert>()
                  .ConstructUsing(vm => new Concert(vm.Id, vm.Title, vm.DateAndStartTime, vm.Location));
        }

        public static TViewModel ToViewModel<TModel, TViewModel>(this TModel model)
        {
            return Mapper.Map<TViewModel>(model);
        }

        public static TModel ToModel<TModel, TViewModel>(this TViewModel viewModel)
        {
            return Mapper.Map<TModel>(viewModel);
        }

        private static DateTime GetDateAndStartTimeFromViewModel(DateTime date, DateTime time)
        {
            return new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, 00);
        }
    }
}