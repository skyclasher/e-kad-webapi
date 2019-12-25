using AutoMapper;
using ECard.Business.XloRecords;
using ECard.Entities.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Framework.Constants;
using Project.Framework.Helper;
using System.Collections.Generic;
using WebApi.Dtos;
using WebApi.Helpers;

namespace WebApi.Controllers
{
	[Authorize]
	[ApiController]
	[Route("api/[controller]")]
	public class XloRecordController : ControllerBase
	{
		private IXloRecordComponent _xloRecordComponent;
		private IMapper _mapper;

		public XloRecordController(
			IXloRecordComponent xloRecordComponent,
			IMapper mapper)
		{
			_xloRecordComponent = xloRecordComponent;
			_mapper = mapper;
		}

		[AllowAnonymous]
		[HttpPost("Create")]
		public IActionResult Create([FromBody]XloRecordDto xloRecordDto)
		{
			// map dto to entity
			var xloRecord = _mapper.Map<XloRecord>(xloRecordDto);

			try
			{
				// save 
				_xloRecordComponent.Create(xloRecord);
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
			var xloRecords = _xloRecordComponent.GetAll();
			var xloRecordDtos = _mapper.Map<IList<XloRecordDto>>(xloRecords);
			return Ok(xloRecordDtos);
		}

		[HttpGet("{id}")]
		public IActionResult GetById(int id)
		{
			try
			{
				var xloRecord = _xloRecordComponent.GetById(id);
				var xloRecordDto = _mapper.Map<XloRecordDto>(xloRecord);
				return Ok(xloRecordDto);
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

		[Route("GetPagedXloRecordByCardDetailId/{cardDetailID}/{searchText}/{currentPage}")]
		public IActionResult GetPagedXloRecordByUserId(int cardDetailID, string searchText, int currentPage)
		{
			try
			{
				if (searchText == Constant.ReplaceText.EmptyString)
					searchText = string.Empty;

				PagingHelper<XloRecord> data = _xloRecordComponent.GetPagedXloRecordByCardDetailId(cardDetailID, currentPage, searchText);
				return Ok(data);
			}
			catch (AppException ex)
			{
				// return error message if there was an exception
				return BadRequest(new { message = ex.Message });
			}
		}

		[HttpPut("{id}")]
		public IActionResult Update(int id, [FromBody]XloRecordDto xloRecordDto)
		{
			// map dto to entity and set id
			var xloRecord = _mapper.Map<XloRecord>(xloRecordDto);
			xloRecord.Id = id;

			try
			{
				// save 
				_xloRecordComponent.Update(xloRecord);
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
			_xloRecordComponent.Delete(id);
			return Ok();
		}
	}
}
