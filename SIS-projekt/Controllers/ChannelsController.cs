using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIS_projekt;
using SIS_projekt.DTO;
using SIS_projekt.Models;
using Channel = SIS_projekt.Models.Channel;

namespace SIS_projekt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChannelsController : ControllerBase
    {
        private readonly AppDBContext _context;

        public ChannelsController(AppDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetChannels()
        {
            return Ok((await _context.Channels.ToListAsync()).Select(c => new
            {
                c.Id,
                c.Name
            }));
        }

        // GET: api/Channels/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Channel>> GetChannel(int id)
        {
            var jwtUserId = User.FindFirst("id")?.Value;
            if (jwtUserId == null)
            {
                return Unauthorized("Invalid token.");
            }
            var channel = await _context.Channels
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (channel == null)
            {
                return NotFound();
            }
            var isUserInChannel = await _context.UserChannels.AnyAsync(c => c.ChannelId == id && c.UserId == int.Parse(jwtUserId));
            if (!isUserInChannel)
            {
                return Unauthorized("You are not in this channel.");
                
            }

            return channel;
        }

        [HttpPost]
        public async Task<ActionResult<Channel>> PostChannel(Channel channel)
        {
            _context.Channels.Add(channel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetChannel", new { id = channel.Id }, channel);
        }

        [HttpPost("addUser")]
        public async Task<ActionResult> PostChannel(AddUserChannelDTO dto)
        {
            User? user = await _context.Users.FindAsync(dto.UserId);
            if (user is null)
            {
                return NotFound(new {userId =  dto.UserId} );
            }

            var channel = await _context.Channels.FindAsync(dto.ChannelId);
            if (channel is null)
            {
                return NotFound(new { channelId = dto.ChannelId });
            }

            var userChannel = await _context.UserChannels.FindAsync(dto.UserId, dto.ChannelId);
            if (userChannel is not null)
            {
                return BadRequest("User already in channel");
            }
            try
            {
                _context.UserChannels.Add(new UserChannel
                {
                    UserId = dto.UserId,
                    ChannelId = dto.ChannelId
                });
                await _context.SaveChangesAsync();
                return Created();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("removeUser")]
        public async Task<ActionResult> PostChannel(RemoveUserChannelDTO dto)
        {
            var uc = await _context.UserChannels.FindAsync(dto.ChannelId, dto.UserId);
            if (uc == null)
            {
                return NotFound();
            }

            _context.UserChannels.Remove(uc);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Channels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChannel(int id)
        {
            var channel = await _context.Channels.FindAsync(id);
            if (channel == null)
            {
                return NotFound();
            }

            _context.Channels.Remove(channel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ChannelExists(int id)
        {
            return _context.Channels.Any(e => e.Id == id);
        }
    }
}
