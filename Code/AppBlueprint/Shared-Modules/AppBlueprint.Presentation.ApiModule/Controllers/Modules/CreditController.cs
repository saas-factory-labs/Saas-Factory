// using AppBlueprint.Infrastructure.DatabaseContexts.Modules.Credit;
// using AppBlueprint.Infrastructure.Repositories.Interfaces;
// using AppBlueprint.Infrastructure.UnitOfWork;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
//
// namespace AppBlueprint.Presentation.ApiModule.Controllers.Modules;
//
// [Authorize]
// [ApiController]
// [Route("api/credits")]
// [Produces("application/json")]
// public class CreditController : ControllerBase
// {
//     private readonly ICreditRepository _creditRepository;
//     private readonly IUnitOfWork _unitOfWork;
//
//     public CreditController(ICreditRepository creditRepository, IUnitOfWork unitOfWork)
//     {
//         _creditRepository = creditRepository ?? throw new ArgumentNullException(nameof(creditRepository));
//         _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
//     }
//
//     /// <summary>
//     /// Gets all credits.
//     /// </summary>
//     /// <returns>List of credits</returns>
//     [HttpGet]
//     [ProducesResponseType(typeof(IEnumerable<CreditResponseDto>), StatusCodes.Status200OK)]
//     [ProducesResponseType(StatusCodes.Status404NotFound)]
//     public async Task<ActionResult<IEnumerable<CreditResponseDto>>> Get(CancellationToken cancellationToken)
//     {
//         var credits = await _creditRepository.GetAllAsync(cancellationToken);
//         if (!credits.Any()) return NotFound(new { Message = "No credits found." });
//
//         var response = credits.Select(credit => new CreditResponseDto
//         {
//             Id = credit.Id,
//             CreditRemaining = credit.CreditRemaining
//         });
//
//         return Ok(response);
//     }
//
//     /// <summary>
//     /// Gets a credit by ID.
//     /// </summary>
//     /// <param name="id">Credit ID.</param>
//     /// <returns>Credit</returns>
//     [HttpGet("{id}")]
//     [ProducesResponseType(typeof(CreditResponseDto), StatusCodes.Status200OK)]
//     [ProducesResponseType(StatusCodes.Status404NotFound)]
//     public async Task<ActionResult<CreditResponseDto>> Get(int id, CancellationToken cancellationToken)
//     {
//         var credit = await _creditRepository.GetByIdAsync(id, cancellationToken);
//         if (credit is null) return NotFound(new { Message = $"Credit with ID {id} not found." });
//
//         var response = new CreditResponseDto
//         {
//             Id = credit.Id,
//             CreditRemaining = credit.CreditRemaining
//         };
//
//         return Ok(response);
//     }
//
//     /// <summary>
//     /// Creates a new credit.
//     /// </summary>
//     /// <param name="creditDto">Credit data transfer object.</param>
//     /// <returns>Created credit.</returns>
//     [HttpPost]
//     [ProducesResponseType(typeof(CreditResponseDto), StatusCodes.Status201Created)]
//     [ProducesResponseType(StatusCodes.Status400BadRequest)]
//     public async Task<ActionResult> Post([FromBody] CreditRequestDto creditDto, CancellationToken cancellationToken)
//     {
//         if (!ModelState.IsValid) return BadRequest(ModelState);
//
//         var newCredit = new CreditEntity
//         {
//             // CreditRemaining = creditDto.CreditRemaining
//         };
//
//         await _creditRepository.AddAsync(newCredit, cancellationToken);
//         await _unitOfWork.SaveChangesAsync();
//
//         return CreatedAtAction(nameof(Get), new { id = newCredit.Id }, newCredit);
//     }
//
//     /// <summary>
//     /// Updates an existing credit.
//     /// </summary>
//     /// <param name="id">Credit ID.</param>
//     /// <param name="creditDto">Credit data transfer object.</param>
//     /// <returns>No content.</returns>
//     [HttpPut("{id}")]
//     [ProducesResponseType(StatusCodes.Status204NoContent)]
//     [ProducesResponseType(StatusCodes.Status404NotFound)]
//     public async Task<ActionResult> Put(int id, [FromBody] CreditRequestDto creditDto, CancellationToken cancellationToken)
//     {
//         var existingCredit = await _creditRepository.GetByIdAsync(id, cancellationToken);
//         if (existingCredit is null) return NotFound(new { Message = $"Credit with ID {id} not found." });
//
//         // existingCredit.CreditRemaining = creditDto.CreditRemaining;
//         //
//         // _creditRepository.Update(existingCredit);
//         // await _unitOfWork.SaveChangesAsync(cancellationToken);
//
//         return NoContent();
//     }
//
//     /// <summary>
//     /// Deletes a credit by ID.
//     /// </summary>
//     /// <param name="id">Credit ID.</param>
//     /// <returns>No content.</returns>
//     [HttpDelete("{id}")]
//     [ProducesResponseType(StatusCodes.Status204NoContent)]
//     [ProducesResponseType(StatusCodes.Status404NotFound)]
//     public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
//     {
//         var existingCredit = await _creditRepository.GetByIdAsync(id, cancellationToken);
//         if (existingCredit is null) return NotFound(new { Message = $"Credit with ID {id} not found." });
//
//         _creditRepository.Delete(existingCredit);
//         await _unitOfWork.SaveChangesAsync();
//
//         return NoContent();
//     }
// }
//
//
//
// public class CreditRequestDto
// {
//     public string Name { get; set; }
// }
//
// public class CreditResponseDto
// {
//     public int Id { get; set; }
//     public decimal CreditRemaining { get; set; }
// }



