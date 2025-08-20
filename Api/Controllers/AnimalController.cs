using Api.Data;
using Api.DTOs;
using Api.Models;
using Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnimalController : ControllerBase
{
    private readonly IDBRepository<Animal> _animalRepository;

    public AnimalController(IDBRepository<Animal> animalRepository)
    {
        _animalRepository = animalRepository;
    }

    [HttpPost]
    public async Task<ActionResult<Animal>> CreateAnimal([FromBody] Animal animal)
    {
        if (animal == null)
        {
            return BadRequest("Animal cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(animal.Name))
        {
            return BadRequest("Animal name is required.");
        }

        animal.Id = Guid.NewGuid();

        await _animalRepository.AddAsync(animal);

        return CreatedAtAction(nameof(GetAnimal), new { id = animal.Id }, GetAnimalData(animal));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Animal>> GetAnimal(Guid id)
    {
        var animal = await _animalRepository.GetByIdAsync(id);
        if (animal == null)
        {
            return NotFound();
        }
        return Ok(GetAnimalData(animal));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAnimal(Guid id)
    {
        var foundAndDeleted = await _animalRepository.DeleteAsync(id);
        if (!foundAndDeleted)
        {
            return NotFound("Animal not found.");
        }
        return Ok();
    }

    private AnimalDTO GetAnimalData(Animal animal) 
    {
        return new AnimalDTO
        {
            Name = animal.Name,
            BirthDate = animal.BirthDate,
            CustomerId = animal.CustomerId,
        };
    }

}