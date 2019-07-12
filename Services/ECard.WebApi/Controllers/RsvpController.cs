﻿using AutoMapper;
using ECard.Entities.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WebApi.Dtos;
using WebApi.Helpers;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class RsvpController : ControllerBase
    {
        private IRsvpService _rsvpService;
        private IMapper _mapper;

        public RsvpController(
            IRsvpService rsvpService,
            IMapper mapper)
        {
            _rsvpService = rsvpService;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("Create")]
        public IActionResult Create([FromBody]RsvpDto rsvpDto)
        {
            // map dto to entity
            var rsvp = _mapper.Map<Rsvp>(rsvpDto);

            try
            {
                // save 
                _rsvpService.Create(rsvp);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var rsvps = _rsvpService.GetAll();
            var rsvpDtos = _mapper.Map<IList<RsvpDto>>(rsvps);
            return Ok(rsvpDtos);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var rsvp = _rsvpService.GetById(id);
                var rsvpDto = _mapper.Map<RsvpDto>(rsvp);
                return Ok(rsvpDto);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [AllowAnonymous]
        [Route("GetByEmail/{email}")]
        public IActionResult GetByEmail(string email)
        {
            try
            {
                var rsvp = _rsvpService.GetByEmail(email);
                var rsvpDto = _mapper.Map<RsvpDto>(rsvp);
                return Ok(rsvpDto);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]RsvpDto rsvpDto)
        {
            // map dto to entity and set id
            var rsvp = _mapper.Map<Rsvp>(rsvpDto);
            rsvp.Id = id;

            try
            {
                // save 
                _rsvpService.Update(rsvp);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _rsvpService.Delete(id);
            return Ok();
        }
    }
}