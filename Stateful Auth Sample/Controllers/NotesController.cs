using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stateful_Auth_Sample.Authentication;
using Stateful_Auth_Sample.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Stateful_Auth_Sample.Controllers
{
    [Route("/api/[controller]"), Authorize(AuthenticationSchemes = CustomAuthenticationExtensions.CustomScheme)]
    public class NotesController : ControllerBase
    {
        private DataContext _data;

        public NotesController(DataContext data) => _data = data;

        [HttpGet("[action]/{id}"), Authorize(Policy = Policies.CanReadNotes)]
        public IActionResult Get(int id)
        {
            var note = _data.Notes.FirstOrDefault(s => s.Id == id);
            if (note == null)
                return BadRequest(new { Error = "Requested note doesn't exist" });

            return Ok(new { note.Title, note.Body });
        }

        [HttpGet("[action]"), Authorize(Policy = Policies.CanReadNotes)]
        public IActionResult List() => Ok(_data.Notes.Select(s => new { s.Id, s.Title }));

        // You'd likely create DTOs for this instead of requesting a database model straight from the user,
        // plus you'd also want to sanitize input before adding it to the database or updating data.

        [HttpPost("[action]"), Authorize(Policy = Policies.CanAddNotes)]
        public async Task<IActionResult> Add([FromBody] Note note)
        {
            _data.Add(note);
            await _data.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("[action]/{id}"), Authorize(Policy = Policies.CanDeleteNotes)]
        public async Task<IActionResult> Delete(int id)
        {
            var note = _data.Notes.FirstOrDefault(s => s.Id == id);
            if (note == null)
                return BadRequest(new { Error = "Requested note doesn't exist" });
            _data.Remove(note);
            await _data.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("[action]"), Authorize(Policy = Policies.CanUpdateNotes)]
        public async Task<IActionResult> Update([FromBody] Note note)
        {
            var dbNote = _data.Notes.FirstOrDefault(s => s.Id == note.Id);
            if (dbNote == null)
                return BadRequest(new { Error = "Requested note doesn't exist" });
            dbNote.Body = note.Body;
            dbNote.Title = note.Title;
            _data.Update(dbNote);
            await _data.SaveChangesAsync();
            return Ok();
        }
    }
}
