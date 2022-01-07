﻿using Microsoft.AspNetCore.Mvc;
using RPGApi.Dtos;
using RPGApi.Repositories;
using AutoMapper;

namespace RPGApi.Controllers
{
    [ApiController]
    [Route("api/characters")]
    public class CharactersController : ControllerBase
    {
        private readonly IControllerRepository<Character> _characterRepository;
        private readonly IControllerRepository<Weapon> _weaponRepository;
        private readonly IControllerRepository<Spell> _spellRepository;
        private readonly IMapper _mapper;

        public CharactersController(IControllerRepository<Character> characterRepository,
            IControllerRepository<Weapon> weaponRepository,
            IControllerRepository<Spell> spellRepository, 
            IMapper mapper)
        {
            _characterRepository = characterRepository;
            _weaponRepository = weaponRepository;
            _spellRepository = spellRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CharacterReadDto>>> GetCharactersAsync()
        {
            IEnumerable<Character> characters = await _characterRepository.GetAllAsync();
            var readDtos = _mapper.Map<IEnumerable<CharacterReadDto>>(characters);

            return Ok(readDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CharacterReadDto>> GetCharacterAsync(Guid id)
        {
            Character? character = await _characterRepository.GetByIdAsync(id);

            if (character is null)
            {
                return NotFound();
            }

            var readDto = _mapper.Map<CharacterReadDto>(character);

            return Ok(readDto);
        }

        [HttpPost]
        public async Task<ActionResult<CharacterReadDto>> CreateCharacterAsync(CharacterCreateDto createDto)
        {
            Character character = _mapper.Map<Character>(createDto);

            _characterRepository.Add(character);
            await _characterRepository.SaveChangesAsync();

            var readDto = _mapper.Map<CharacterReadDto>(character);

            return CreatedAtAction(nameof(GetCharacterAsync), new { id = readDto.Id }, readDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCharacterAsync(Guid id, CharacterUpdateDto updateDto)
        {
            Character? character = await _characterRepository.GetByIdAsync(id);

            if (character is null)
            {
                return NotFound();
            }

            _mapper.Map(updateDto, character);
            _characterRepository.Update(character);
            await _characterRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCharacterAsync(Guid id)
        {
            Character? character = await _characterRepository.GetByIdAsync(id);

            if (character is null)
            {
                return NotFound();
            }

            _characterRepository.Delete(character);
            await _characterRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("add/weapon")]
        public async Task<ActionResult> AddWeaponAsync(CharacterAddRemoveItem addDto)
        {
            Character? character = await _characterRepository.GetByIdAsync(addDto.CharacterId);

            if (character is null)
            {
                return NotFound();
            }

            Weapon? weapon = await _weaponRepository.GetByIdAsync(addDto.ItemId);

            if (weapon is null)
            {
                return BadRequest();
            }

            character.Weapons.Add(weapon);
            _characterRepository.Update(character);
            await _characterRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("remove/weapon")]
        public async Task<ActionResult> RemoveWeaponAsync(CharacterAddRemoveItem weaponDto)
        {
            Character? character = await _characterRepository.GetByIdAsync(weaponDto.CharacterId);

            if (character is null)
            {
                return NotFound();
            }

            Weapon? weapon = await _weaponRepository.GetByIdAsync(weaponDto.ItemId);

            if (weapon is null)
            {
                return BadRequest();
            }

            character.Weapons.Remove(weapon);
            _characterRepository.Update(character);
            await _characterRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("add/spell")]
        public async Task<ActionResult> AddSpellAsync(CharacterAddRemoveItem spellDto)
        {
            Character? character = await _characterRepository.GetByIdAsync(spellDto.CharacterId);

            if (character is null)
            {
                return NotFound();
            }

            Spell? spell = await _spellRepository.GetByIdAsync(spellDto.ItemId);

            if (spell is null)
            {
                return BadRequest();
            }

            character.Spells.Add(spell);
            _characterRepository.Update(character);
            await _characterRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("remove/spell")]
        public async Task<ActionResult> RemoveSpellAsync(CharacterAddRemoveItem spellDto)
        {
            Character? character = await _characterRepository.GetByIdAsync(spellDto.CharacterId);

            if (character is null)
            {
                return NotFound();
            }

            Spell? spell = await _spellRepository.GetByIdAsync(spellDto.ItemId);

            if (spell is null)
            {
                return BadRequest();
            }

            character.Spells.Remove(spell);
            _characterRepository.Update(character);
            await _characterRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}
