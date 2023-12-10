using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using QaToDoApp.Models;
using QaToDoApp.Models.Dto;
using QaToDoApp.Repository;

namespace QaToDoApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoItemsController : ControllerBase
    {
        private readonly IToDoItemRepository _dbToDoItem;
        private readonly ApiResponse _response;
        private readonly IMapper _mapper;

        public ToDoItemsController(IToDoItemRepository dbToDoItem, IMapper mapper)
        {
            _dbToDoItem = dbToDoItem;
            _mapper = mapper;
            _response = new ApiResponse();
        }

        [HttpGet(Name = "GetAllToDoItems")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse>> GetTodoItems()
        {
            try
            {
                IEnumerable<ToDoItem> toDoList = await _dbToDoItem.GetAllAsync();
                _response.Result = _mapper.Map<List<ToDoItemDto>>(toDoList);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                    = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpGet("{id:int}", Name = "GetToDoItem")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse>> GetToDoItem(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                var toDoItem = await _dbToDoItem.GetAsync(u => u.Id == id);
                if (toDoItem == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                _response.Result = _mapper.Map<ToDoItemDto>(toDoItem);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                    = new List<string>() { ex.ToString() };
            }
            return _response; 
        }

        [HttpPost(Name = "CreateToDoItem")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> PostToDoItem(ToDoForCreateDto toDoForCreateDto)
        {
            try
            {
                if (await _dbToDoItem.GetAsync(u => u.Text.ToLower() == toDoForCreateDto.Text.ToLower()) != null)
                {
                    ModelState.AddModelError("ErrorMessages", "ToDoItem already Exists!");
                    return BadRequest(ModelState);
                }

                if (toDoForCreateDto == null)
                {
                    return BadRequest(null!);
                }
                
                var toDoItem = _mapper.Map<ToDoItem>(toDoForCreateDto);
                toDoItem.CreatedDate = DateTime.Now;

                await _dbToDoItem.CreateAsync(toDoItem);
                _response.Result = _mapper.Map<ToDoItemDto>(toDoItem);
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetToDoItem", new { id = toDoItem.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                    = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        
        [HttpPut("{id:int}", Name = "UpdateToDoItem")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> PutToDoItem(int id, ToDoForUpdateDto toDoForUpdateDto)
        {
            try
            {
                if (toDoForUpdateDto == null || id != toDoForUpdateDto.Id)
                {
                    return BadRequest();
                }

                var model = _mapper.Map<ToDoItem>(toDoForUpdateDto);
                model.UpdatedDate = DateTimeOffset.Now;

                await _dbToDoItem.UpdateAsync(model);
                _response.Result = _mapper.Map<ToDoItemDto>(model);
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                    = new List<string>() { ex.ToString() };
            } 
            return _response;
        }
        
        [HttpPatch("{id:int}", Name = "UpdatePartialToDoItem")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PatchToDoItem(int id, JsonPatchDocument<ToDoForUpdateDto> toDoForPatchDto)
        {
            if (toDoForPatchDto == null || id == 0)
            {
                return BadRequest();
            }
            var toDoItem = await _dbToDoItem.GetAsync(u => u.Id == id, false);
            var toDoItemDto = _mapper.Map<ToDoForUpdateDto>(toDoItem);

            if (toDoItem == null)
            {
                return BadRequest();
            }
            toDoForPatchDto.ApplyTo(toDoItemDto);
            var model = _mapper.Map<ToDoItem>(toDoItemDto);
            model.UpdatedDate = DateTimeOffset.Now;

            await _dbToDoItem.UpdateAsync(model);
            _response.Result = _mapper.Map<ToDoItemDto>(model);
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(_response);
        }

        [HttpDelete("{id:int}", Name = "DeleteToDoItem")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> DeleteToDoItem(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                var toDoItem = await _dbToDoItem.GetAsync(u => u.Id == id);
                if (toDoItem == null)
                {
                    return NotFound();
                }
                await _dbToDoItem.RemoveAsync(toDoItem);
                return NoContent();
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                    = new List<string>() { ex.ToString() };
            }
            return _response;
        }
    }
}
