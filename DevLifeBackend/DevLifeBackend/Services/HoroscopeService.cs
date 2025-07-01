// DevLife.Api/Services/HoroscopeService.cs
using DevLife.Api.Utils;

namespace DevLife.Api.Services
{
    public class HoroscopeService
    {
        // In a real application, you might fetch daily horoscopes from an external API or a database.
        // For this example, we'll use a simple placeholder based on the zodiac sign.
        private readonly Dictionary<string, string> _horoscopeMessages = new Dictionary<string, string>
        {
            { "Aries", "Today is a good day to push new features. Expect some minor merge conflicts, but they're easily resolved." },
            { "Taurus", "Focus on refactoring today. Clean code brings inner peace and fewer bugs." },
            { "Gemini", "Collaborate with a colleague; two heads are better than one, especially for complex algorithms." },
            { "Cancer", "Take a moment to review your pull requests. Attention to detail will prevent future headaches." },
            { "Leo", "Your leadership skills will shine in a team meeting today. Guide your peers through tough technical decisions." },
            { "Virgo", "A new dependency might simplify your work, but choose wisely. Look for well-documented libraries." },
            { "Libra", "Balance your workload today. Don't let one task consume all your focus." },
            { "Scorpio", "Dive deep into that elusive bug. Your persistence will pay off with a satisfying fix." },
            { "Sagittarius", "Explore new technologies. Learning a new framework could open up exciting opportunities." },
            { "Capricorn", "Structure and planning will be key to your success today. Break down large tasks into smaller, manageable chunks." },
            { "Aquarius", "Think outside the box for a tricky problem. An unconventional solution might be the most elegant." },
            { "Pisces", "Connect with your creative side. Designing a beautiful UI or writing poetic code will be fulfilling." }
        };

        public async Task<string> GetDailyHoroscope(DateTime birthDate)
        {
            string zodiacSign = ZodiacCalculator.GetZodiacSign(birthDate);

            if (_horoscopeMessages.TryGetValue(zodiacSign, out string message))
            {
                return await Task.FromResult(message);
            }

            return await Task.FromResult("Your coding stars are aligned. Expect good things!");
        }
    }
}