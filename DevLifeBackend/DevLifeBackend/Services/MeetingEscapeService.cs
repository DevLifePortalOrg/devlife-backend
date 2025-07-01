// DevLife.Api/Services/MeetingEscapeService.cs
using DevLife.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevLife.Api.Services
{
    public class MeetingEscapeService
    {
        private static readonly List<string> _excuses = new List<string>
        {
            "My compiler just threw a 418 I'm a teapot error, critical system failure imminent!",
            "I've encountered a critical 'infinite loop' in my real life, requiring immediate attention.",
            "My cat walked across the keyboard and deleted my entire project. Must save it!",
            "A wild bug appeared in production and I must chase it down to its lair.",
            "My coffee machine has crashed, I need to debug it ASAP for team productivity.",
            "Experiencing a 'stack overflow' of urgent tasks, cannot push another frame.",
            "I've just been pulled into a high-priority 'hotfix' situation on a legacy system.",
            "My VPN disconnected and I'm stuck in an 'offline' branch of reality.",
            "The build pipeline broke, and I'm the only one who can fix the 'broken pipe' error.",
            "My IDE just suggested a 'critical refactor' of my entire life schedule.",
            "Suddenly received an alert from 'prod' that I can't ignore.",
            "My pair programming partner just had a brilliant idea we need to implement immediately!"
        };

        private readonly Random _random = new Random();

        public async Task<Excuse> GenerateRandomExcuse()
        {
            int index = _random.Next(_excuses.Count);
            return await Task.FromResult(new Excuse { Text = _excuses[index] });
        }
    }
}