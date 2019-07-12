using AutoMapper;
using ECard.Entities.Entities;
using ECard.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using WebApi.Dtos;
using WebApi.Helpers;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ECardController : ControllerBase
    {
        private IECardDetailService _ecardDetailService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public ECardController(
            IECardDetailService ecardDetailService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _ecardDetailService = ecardDetailService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [HttpPost("Create")]
        public IActionResult Create([FromBody]ECardDetailDto ecardDetailDto)
        {
            // map dto to entity
            var ecardDetail = _mapper.Map<ECardDetail>(ecardDetailDto);

            try 
            {
                // save 
                _ecardDetailService.Create(ecardDetail);
                return Ok();
            } 
            catch(AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var ecardDetails =  _ecardDetailService.GetAll();
            var ecardDetailDtos = _mapper.Map<IList<ECardDetailDto>>(ecardDetails);
            return Ok(ecardDetailDtos);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var ecardDetail =  _ecardDetailService.GetById(id);
            var ecardDetailDto = _mapper.Map<ECardDetailDto>(ecardDetail);
            return Ok(ecardDetailDto);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]ECardDetailDto ecardDetailDto)
        {
            // map dto to entity and set id
            var ecardDetail = _mapper.Map<ECardDetail>(ecardDetailDto);
            ecardDetail.Id = id;

            try 
            {
                // save 
                _ecardDetailService.Update(ecardDetail);
                return Ok();
            } 
            catch(AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _ecardDetailService.Delete(id);
            return Ok();
        }
    }
}
