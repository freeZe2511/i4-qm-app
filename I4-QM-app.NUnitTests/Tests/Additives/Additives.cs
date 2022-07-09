using I4_QM_app.Models;
using I4_QM_app.NUnitTests.MockServices;
using I4_QM_app.Services;
using I4_QM_app.ViewModels;
using NUnit.Framework;
using System.Threading.Tasks;

namespace I4_QM_app.NUnitTests.Tests.Additives
{
    internal class Additives
    {
        private IDataService<Additive> additivesService;

        [SetUp]
        public void Setup()
        {
            additivesService = new MockAdditivesService();
        }

        [Test]
        public void AdditivesInit()
        {
            var additives = new AdditivesViewModel(additivesService);
            Assert.AreEqual(additives.Title, "Additives");
            Assert.IsTrue(additives.Descending);
            Assert.IsNotNull(additives.Additives);

            additives.OnAppearing();
            Assert.IsTrue(additives.IsBusy);

            Assert.IsNotNull(additives.DisableCommand);
        }

        [Test]
        public async Task AdditivesEmpty()
        {
            var additives = new AdditivesViewModel(additivesService);
            Assert.IsEmpty(await additivesService.GetItemsAsync());
            Assert.IsEmpty(additives.Additives);
        }

        [Test]
        public async Task AdditivesAddData()
        {
            var additives = new AdditivesViewModel(additivesService);
            Assert.IsEmpty(await additivesService.GetItemsAsync());
            Assert.IsEmpty(additives.Additives);

            await additivesService.AddItemAsync(new Additive
            {
                Id = "1",
                Name = "Test"
            });

            additives.LoadAdditivesCommand.Execute(null);
            Assert.AreEqual(1, additives.Additives.Count);
        }

        [Test]
        public async Task AdditivesSortDataById()
        {
            var additives = new AdditivesViewModel(additivesService);

            var a = new Additive
            {
                Id = "1",
                Name = "Test1"
            };

            var b = new Additive
            {
                Id = "2",
                Name = "Test2"
            };

            await additivesService.AddItemAsync(a);
            await additivesService.AddItemAsync(b);

            additives.LoadAdditivesCommand.Execute(null);
            Assert.AreEqual(2, additives.Additives.Count);

            Assert.AreEqual(a.Id, additives.Additives[0].Id);

            additives.Descending = false;
            additives.SortByCommand.Execute("Id");

            Assert.AreEqual(b.Id, additives.Additives[0].Id);
        }

        [Test]
        public async Task AdditivesSortDataByName()
        {
            var additives = new AdditivesViewModel(additivesService);

            var a = new Additive
            {
                Id = "1",
                Name = "Test1"
            };

            var b = new Additive
            {
                Id = "2",
                Name = "Test2"
            };

            await additivesService.AddItemAsync(a);
            await additivesService.AddItemAsync(b);

            additives.LoadAdditivesCommand.Execute(null);
            Assert.AreEqual(2, additives.Additives.Count);

            Assert.AreEqual(a.Name, additives.Additives[0].Name);

            additives.Descending = false;
            additives.SortByCommand.Execute("Name");

            Assert.AreEqual(b.Name, additives.Additives[0].Name);
        }

        [Test]
        public async Task AdditivesImageFound()
        {
            var additives = new AdditivesViewModel(additivesService);
            // TODO mock filestorage
        }
    }
}