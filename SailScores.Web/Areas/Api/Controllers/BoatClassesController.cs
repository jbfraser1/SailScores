using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SailScores.Api.Dtos;
using SailScores.Core.Model;
using IAuthorizationService = SailScores.Web.Services.Interfaces.IAuthorizationService;

namespace SailScores.Web.Areas.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class BoatClassesController : ControllerBase
    {
        private readonly CoreServices.IClubService _clubService;
        private readonly CoreServices.IBoatClassService _classService;
        private readonly IAuthorizationService _authService;
        private readonly IMapper _mapper;

        public BoatClassesController(
            CoreServices.IClubService clubService,
            CoreServices.IBoatClassService classService,
            IAuthorizationService authService,
            IMapper mapper)
        {
            _clubService = clubService;
            _classService = classService;
            _authService = authService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<BoatClassDto>> Get(Guid clubId)
        {
            var club = await _clubService.GetFullClubExceptScores(clubId);
            return _mapper.Map<List<BoatClassDto>>(club.BoatClasses);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Post([FromBody] BoatClassDto boatClass)
        {
            if (!await _authService.CanUserEdit(User, boatClass.ClubId))
            {
                return Unauthorized();
            }
            var classBizObj = _mapper.Map<BoatClass>(boatClass);
            await _classService.SaveNew(classBizObj);
            var savedClass =
                (await _clubService.GetFullClubExceptScores(boatClass.ClubId))
                .BoatClasses
                .First(c => c.Name == boatClass.Name);
            return Ok(savedClass.Id);
        }
    }
}
