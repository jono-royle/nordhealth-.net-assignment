using Api.Controllers;
using Api.DTOs;
using Api.Models;
using Api.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace VetAppUnitTests
{
    [TestClass]
    public sealed class AnimalControllerTests
    {
        private AnimalController _controller;
        private Mock<IDBRepository<Animal>> _dbRepository;
        private static Guid _animalId = Guid.NewGuid();
        private static Guid _customerId = Guid.NewGuid();

        private Animal _animal = new Animal
        {
            Name = "testAnimal",
            Id = _animalId,
            BirthDate = DateTime.Now.AddYears(-1),
            CustomerId = _customerId
        };

        [TestInitialize]
        public void Setup()
        {
            _dbRepository = new Mock<IDBRepository<Animal>>();
            _dbRepository.Setup(r => r.GetByIdAsync(_animalId)).ReturnsAsync(_animal);
            _dbRepository.Setup(r => r.ScanAsync(It.IsAny<Func<IQueryable<Animal>, IQueryable<Animal>>>(),
                               It.IsAny<CancellationToken>()))
        .ReturnsAsync(new List<Animal>());
            _controller = new AnimalController(_dbRepository.Object);
        }

        [TestMethod]
        public async Task GetAnimal_ReturnsOkAnimal()
        {
            var result = await _controller.GetAnimal(_animalId);
            var ok = result.Result as OkObjectResult;
            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            var returned = ok.Value as AnimalDTO;
            Assert.IsNotNull(returned);
            Assert.AreEqual(_animal.Name, returned.Name);
        }

        [TestMethod]
        public async Task GetNullAnimal_ReturnsNotFound()
        {
            var result = await _controller.GetAnimal(Guid.NewGuid());
            var notFound = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFound);
        }

        [TestMethod]
        public async Task DeleteNullAnimal_ReturnsNotFound()
        {
            var result = await _controller.DeleteAnimal(Guid.NewGuid());
            var notFound = result as NotFoundObjectResult;
            Assert.IsNotNull(notFound);
        }

        [TestMethod]
        public async Task CreateNullAnimal_ReturnsBadRequest()
        {
            var result = await _controller.CreateAnimal(null);
            var badRequest = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        [TestMethod]
        public async Task CreateNoNameAnimal_ReturnsBadRequest()
        {
            var result = await _controller.CreateAnimal(new Animal());
            var badRequest = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
        }

        [TestMethod]
        public async Task CreateAnimal_ReturnsCreatedAt()
        {
            var result = await _controller.CreateAnimal(_animal);
            var created = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(created);
            var returned = created.Value as AnimalDTO;
            Assert.IsNotNull(returned);
            Assert.AreEqual(_animal.Name, returned.Name);
        }
    }
}
