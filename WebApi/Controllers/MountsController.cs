using Core.Entities;
using Core.Interfaces.Services;
using Core.Models;
using Core.ViewModels;
using Core.ViewModels.MountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebApi.Mappers.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/mounts")]
    [ApiController]
    [Authorize]
    public class MountsController : ControllerBase
    {
        private readonly IItemService<Mount> _mountService;
        private readonly IMapper<PaginatedList<Mount>, PageViewModel<MountReadViewModel>> _paginatedMapper;
        private readonly IMapper<Mount, MountReadViewModel> _readMapper;
        private readonly IMapper<MountBaseViewModel, Mount> _createMapper;
        private readonly IUpdateMapper<MountBaseViewModel, Mount> _updateMapper;

        public MountsController(
            IItemService<Mount> mountService,
            IMapper<PaginatedList<Mount>, PageViewModel<MountReadViewModel>> paginatedMapper, 
            IMapper<Mount, MountReadViewModel> readMapper, 
            IMapper<MountBaseViewModel, Mount> createMapper,
            IUpdateMapper<MountBaseViewModel, Mount> updateMapper)
        {
            _mountService = mountService;
            _paginatedMapper = paginatedMapper;
            _readMapper = readMapper;
            _createMapper = createMapper;
            _updateMapper = updateMapper;
        }

        [HttpGet]
        public async Task<ActionResult<PageViewModel<MountReadViewModel>>> GetAsync(
            [FromQuery] PageParameters pageParams)
        {
            var mounts = await _mountService.GetAllAsync(pageParams.PageNumber, pageParams.PageSize);
            var pageModel = _paginatedMapper.Map(mounts);

            return Ok(pageModel);
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<ActionResult<MountReadViewModel>> GetAsync([FromRoute] int id)
        {
            var mount = await _mountService.GetByIdAsync(id);
            var readModel = _readMapper.Map(mount);

            return Ok(readModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MountReadViewModel>> CreateAsync([FromBody] MountBaseViewModel createModel)
        {
            var mount = _createMapper.Map(createModel);
            await _mountService.AddAsync(mount);

            var readModel = _readMapper.Map(mount);

            return CreatedAtAction(nameof(GetAsync), new { Id = readModel.Id }, readModel);
        }

        [HttpPut("{id:int:min(1)}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateAsync([FromRoute] int id, [FromBody] MountBaseViewModel updateModel)
        {
            var mount = await _mountService.GetByIdAsync(id);

            _updateMapper.Map(updateModel, mount);
            await _mountService.UpdateAsync(mount);

            return NoContent();
        }

        [HttpPatch("{id:int:min(1)}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateAsync(
            [FromRoute] int id,
            [FromBody] JsonPatchDocument<MountBaseViewModel> patchDocument)
        {
            var mount = await _mountService.GetByIdAsync(id);
            var updateModel = _updateMapper.Map(mount);

            patchDocument.ApplyTo(updateModel, ModelState);

            if (!TryValidateModel(updateModel))
            {
                return ValidationProblem(ModelState);
            }

            _updateMapper.Map(updateModel, mount);
            await _mountService.UpdateAsync(mount);

            return NoContent();
        }

        [HttpDelete("{id:int:min(1)}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteAsync([FromRoute] int id)
        {
            var mount = await _mountService.GetByIdAsync(id);
            await _mountService.DeleteAsync(mount);

            return NoContent();
        }
    }
}
