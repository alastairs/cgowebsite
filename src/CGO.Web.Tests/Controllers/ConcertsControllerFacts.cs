﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CGO.Web.Controllers;
using CGO.Web.Models;
using CGO.Web.ViewModels;

using MvcContrib.TestHelper;
using NSubstitute;
using NUnit.Framework;

using Raven.Client;
using Raven.Client.Embedded;

namespace CGO.Web.Tests.Controllers
{
    public class ConcertsControllerFacts
    {
        [TestFixture]
        public class IndexShould
        {
            [Test]
            public void DisplayTheIndexView()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSession>());

                var result = controller.Index();

                result.AssertViewRendered().ForView("Index").WithViewData<IEnumerable<Concert>>();
            }

            [Test]
            public void DisplayTheConcertsInDescendingOrderByDate()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSession>());

                var result = controller.Index() as ViewResult;
                var concerts = result.Model as IEnumerable<Concert>;

                Assert.That(concerts.OrderByDescending(c => c.DateAndStartTime), Is.EqualTo(concerts));
            }
        }

        [TestFixture]
        public class DetailsShould : RavenTest
        {
            [Test]
            public void RenderTheDetailsView()
            {
                var controller = new ConcertsController(Session);

                var result = controller.Details(1);

                result.AssertViewRendered().ForView("Details").WithViewData<Concert>();
            }

            [Test]
            public void DisplayTheConcertRequested()
            {
                var controller = new ConcertsController(Session);

                var result = controller.Details(1) as ViewResult;
                var concert = result.Model as Concert;
                
                Assert.That(concert.Id, Is.EqualTo(1));
            }

            [SetUp]
            public void CreateSampleData()
            {
                using(var sampleDataSession = Store.OpenSession())
                {
                    sampleDataSession.Store(new Concert(1, "foo", DateTime.Now, "bar"));
                    sampleDataSession.SaveChanges();
                }
            }
        }

        [TestFixture]
        public class CreateShould : RavenTest
        {
            [Test]
            public void ShowTheCreateViewWhenCalledViaAGetRequest()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSession>());

                var result = controller.Create(); // The parameterless overload is called on GET.

                result.AssertViewRendered().ForView("Create");
            }

            [Test]
            public void ShowTheCreateViewWithTheSuppliedModelWhenThereAreValidationErrors()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSession>());
                controller.ViewData.ModelState.AddModelError("Title", "Please enter a title");

                var result = controller.Create(new ConcertViewModel()); // The overload with ConcertViewModel parameter is called on POST.

                result.AssertViewRendered().ForView("Create").WithViewData<ConcertViewModel>();
            }

            [Test]
            public void ReturnToTheListOfConcertsWhenThereAreNoValidationErrors()
            {
                var controller = new ConcertsController(Substitute.For<IDocumentSession>());

                var result = controller.Create(new ConcertViewModel());

                result.AssertActionRedirect().ToAction("List");
            }

            [Test]
            public void SaveTheConcertToTheDatabaseWhenThereAreNoValidationErrors()
            {
                var controller = new ConcertsController(Session);
                var concert = new Concert(1, "Foo", new DateTime(2012, 07, 29, 20, 00, 00), "Bar");

                controller.Create(new ConcertViewModel
                {
                    Date = concert.DateAndStartTime,
                    StartTime = concert.DateAndStartTime,
                    Location = concert.Location,
                    Title = concert.Title
                });

                Assert.That(Session.Load<Concert>(1), Is.EqualTo(concert).Using(new ConcertEqualityComparer()));
            }
        }
    }
}
