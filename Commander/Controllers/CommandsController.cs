using System.Collections.Generic;
using AutoMapper;
using Commander.BLL.ModelsDTO;
using Commander.DAL.Models;
using Commander.DAL.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Commander.Controllers
{
    [ApiController]
    [Route("api/commands")]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepository _repository;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        
        // сначала было это, но его заменили на внедренную зависимость и проброс в конструкторе
        //private readonly CommandRepository _repository = new CommandRepository();
            
        //GET api/commands
        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDTO>> GetAllCommands()
        {
            var commandItems = _repository.GetAll();
            return Ok(_mapper.Map<IEnumerable<CommandReadDTO>>(commandItems));
        }

        //GET api/commands/{id}
        [HttpGet("{id}", Name="GetCommandById")]
        public ActionResult<CommandReadDTO> GetCommandById(int id)
        {
            var commandItem = _repository.GetById(id);
            if (commandItem != null)
            {
                return Ok(_mapper.Map<CommandReadDTO>(commandItem));
            }
            return NotFound();
        }

        //POST api/commands
        [HttpPost]
        public ActionResult<CommandReadDTO> CreateCommand(CommandCreateDTO commandCreateDTO)
        {
            var commandItem = _mapper.Map<Command>(commandCreateDTO);
            _repository.Create(commandItem);
            _repository.SaveChanges();

            // странно, но работает, хотя возвращаемый тип не соответствует
            /*
            return Ok(commandItem);
            */
            
            // но в итоге всё равно нужно смапить к ожидаемому типу
            var commandReadDTO = _mapper.Map<CommandReadDTO>(commandItem);
            // а можно усложнить вывод и воспользоваться уже существующим методом GetCommandById
            // но для этого нужно расширить атрибут, указав имя
            // таким образом с [HttpGet("{id}")]
            // он поменялся на [HttpGet("{id}", Name="GetCommandById")]
            /*
            return Ok(commandReadDTO);
            */
            // и теперь возврат результата будет таким
            return CreatedAtRoute(nameof(GetCommandById), new {Id = commandReadDTO.Id}, commandReadDTO);
            // всё это ради того, чтобы ответ был со статусом Status 201 Created,
            // тогда как ранее возвращался Status 200 Ok
        }
        
        // PUT api/commands/{id}
        // немного про PUT - он требует замены всей записи в базе. т.е. нельзя передать только измененные поля, оставив остальное как есть
        // ему нужно проапдейтить всё, поэтому в модели commandUpdateDTO присутствует всё, кроме Id, да ещё и с валидаторами
        [HttpPut("{id}")]
        public ActionResult UpdateCommand(int id, CommandUpdateDTO commandUpdateDTO)
        {
            var commandItem = _repository.GetById(id);
            if (commandItem == null)
            {
                return NotFound();
            }

            _mapper.Map(commandUpdateDTO, commandItem);
            
            // хоть наш метод Update в DAL-е пустой, является хорошей практикой вызывать его всё равно, поскольку там может со временем "завестись" какой-нить код
            _repository.Update(commandItem);
            _repository.SaveChanges();

            return NoContent();
        }
        
        // PATCH api/commands/{id}
        // универсальная команда. 
        // патч может: add/remove/replace/copy/move/test
        // принимает параметр в виде json последовательности команд, каждая из которых сожержит атрибут, точку применения и значение
        // [{"op": "replace", "path": "/howto", "value": "Some new value"}, {"op": "test", "path": "/line", "value": "dotnet new"}]
        // поэтому в проект нужно добавить NuGet Microsoft.AspNetCore.JsonPatch
        [HttpPatch("{id}")]
        public ActionResult PartialCommandUpdate(int id, JsonPatchDocument<CommandUpdateDTO> patchDoc)
        {
            var commandItem = _repository.GetById(id);
            if (commandItem == null)
            {
                return NotFound();
            }

            var commandToPatch = _mapper.Map<CommandUpdateDTO>(commandItem);
            patchDoc.ApplyTo(commandToPatch, ModelState);

            if (!TryValidateModel(commandToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(commandToPatch, commandItem);
            
            _repository.Update(commandItem);
            _repository.SaveChanges();
            
            return NoContent();
        }
        
        //DELETE api/commands/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteCommand(int id)
        {
            var commandItem = _repository.GetById(id);
            if (commandItem == null)
            {
                return NotFound();
            }
            
            _repository.Delete(commandItem);
            _repository.SaveChanges();

            return NoContent();
        }
    }
}