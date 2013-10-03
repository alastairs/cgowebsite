using System;
using System.Web.Mvc;
using CGO.Domain;
using CGO.Domain.Entities;
using CGO.Domain.Services;
using CGO.Web.Areas.Admin.Controllers;
using CGO.Web.Tests.EqualityComparers;
using CGO.Web.ViewModels;
using MvcContrib.TestHelper;
using NSubstitute;
using NUnit.Framework;

namespace CGO.Web.Tests.Controllers.Admin
{
    public class ConcertsControllerFacts
    {
        [TestFixture]
        public class ConstructorShould
        {
            [Test]
            public void ThrowAnArgumentNullExceptionIfTheConcertDetailsServiceIsNull()
            {
                Assert.That(() => new ConcertsController(null),
                            Throws.InstanceOf<ArgumentNullException>());
            }
        }

        [TestFixture]
        public class CreateShould
        {
            [Test]
            public void ShowTheCreateViewWhenCalledViaAGetRequest()
            {
                var controller = new ConcertsController(Substitute.For<IConcertDetailsService>());

                var result = controller.Create(); // The parameterless overload is called on GET.

                result.AssertViewRendered().ForView("Create");
            }

            [Test]
            public void ShowTheCreateViewWithTheSuppliedModelWhenThereAreValidationErrors()
            {
                var controller = new ConcertsController(Substitute.For<IConcertDetailsService>());
                controller.ViewData.ModelState.AddModelError("Title", "Please enter a title");

                var result = controller.Create(new ConcertViewModel()); // The overload with ConcertViewModel parameter is called on POST.

                result.AssertViewRendered().ForView("Create").WithViewData<ConcertViewModel>();
            }

            [Test]
            public void ReturnToTheListOfConcertsWhenThereAreNoValidationErrors()
            {
                var controller = new ConcertsController(Substitute.For<IConcertDetailsService>());

                var result = controller.Create(new ConcertViewModel());

                result.AssertActionRedirect().ToAction("Index");
            }

            [Test]
            public void CallSaveOnTheConcertDetailsServiceWhenThereAreNoValidationErrors()
            {
                var concertDetailsService = Substitute.For<IConcertDetailsService>();
                var controller = new ConcertsController(concertDetailsService);

                controller.Create(new ConcertViewModel
                {
                    Date = new DateTime(2012, 07, 29, 20, 00, 00),
                    StartTime = new DateTime(2012, 07, 29, 20, 00, 00),
                    Location = "Bar",
                    Title = "Foo"
                });

                concertDetailsService.ReceivedWithAnyArgs(1).SaveConcert(null);
            }
        }
 
        [TestFixture]
        public class IndexShould
        {
            [Test]
            public void ShowTheIndexView()
            {
                var controller = new ConcertsController(Substitute.For<IConcertDetailsService>());

                var result = controller.Index();

                result.AssertViewRendered().ForView("Index");
            }
        }


        [TestFixture]
        public class EditShouldOnGet
        {
            [Test]
            public void ShowTheEditView()
            {
                var concertDetailsService = GetMockConcertDetailsService();
                var controller = new ConcertsController(concertDetailsService);

                var result = controller.Edit(1);

                result.AssertViewRendered().ForView("Edit");
            }

            [Test]
            public void ShowTheEditViewWithAConcertViewModel()
            {
                var concertDetailsService = GetMockConcertDetailsService();
                var controller = new ConcertsController(concertDetailsService);

                var result = controller.Edit(1);

                result.AssertViewRendered().WithViewData<ConcertViewModel>();
            }

            [Test]
            public void ShowTheEditViewForTheConcertSpecified()
            {
                var concertDetailsService = GetMockConcertDetailsService();
                var controller = new ConcertsController(concertDetailsService);
                var viewModel = new ConcertViewModel
                {
                    Id = 1,
                    Title = "Test Concert",
                    Date = DateTime.MinValue,
                    StartTime = DateTime.MinValue,
                    Location = "Venue",
                    IsPublished = false
                };

                var result = controller.Edit(1) as ViewResult;

                Assert.That(result.Model, Is.EqualTo(viewModel).Using(new ConcertViewModelEqualityComparer()));
            }

            [Test]
            public void CallGetConcertOnTheConcertDetailsServiceWithTheSpecifiedId()
            {
                var concertDetailsService = Substitute.For<IConcertDetailsService>();
                var controller = new ConcertsController(concertDetailsService);

                const int concertId = 1;
                controller.Edit(concertId);

                concertDetailsService.Received().GetConcert(concertId);
            }

            [Test]
            public void ThrowA404NotFoundIfTheRequestedConcertIdIsUnknown()
            {
                var controller = new ConcertsController(Substitute.For<IConcertDetailsService>());

                var result = controller.Edit(23);

                result.AssertResultIs<HttpNotFoundResult>();
            }

            private IConcertDetailsService GetMockConcertDetailsService()
            {
                var concertDetailsService = Substitute.For<IConcertDetailsService>();
                concertDetailsService.GetConcert(1).ReturnsForAnyArgs(new Concert(1, "Test Concert", DateTime.MinValue, "Venue"));
                return concertDetailsService;
            }
        }

        [TestFixture]
        public class EditShouldOnPost
        {
            [Test]
            public void RedirectToTheListViewIfNoErrorsOccurred()
            {
                var controller = new ConcertsController(Substitute.For<IConcertDetailsService>());

                var result = controller.Edit(1, new ConcertViewModel());

                result.AssertActionRedirect().ToAction("Index");
            }

            [Test]
            public void RedisplayTheEditViewWhenValidationErrorsArePresent()
            {
                var controller = new ConcertsController(Substitute.For<IConcertDetailsService>());
                controller.ModelState.AddModelError("Date", "Not a date");

                var result = controller.Edit(1, new ConcertViewModel());

                result.AssertViewRendered().ForView("Edit");
            }

            [Test]
            public void RedisplayTheEditViewWithTheProvidedViewModelWhenValidationErrorsArePresent()
            {
                var controller = new ConcertsController(Substitute.For<IConcertDetailsService>());
                controller.ModelState.AddModelError("Date", "Not a date");

                var concertToSave1 = new ConcertViewModel();
                var result = controller.Edit(1, concertToSave1) as ViewResult;

                Assert.That(result.Model, Is.EqualTo(concertToSave1));
            }

            [Test]
            public void CallSaveOnTheConcertDetailsServiceIfNoErrorsOccurred()
            {
                var concertDetailsService = Substitute.For<IConcertDetailsService>();
                var controller = new ConcertsController(concertDetailsService);

                controller.Edit(1, new ConcertViewModel());

                concertDetailsService.ReceivedWithAnyArgs(1).SaveConcert(null);
            }
        }
    }
}
