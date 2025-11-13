namespace HorizonChat.Services;

public class GuestNameGenerator
{
    private static readonly string[] Adjectives = new[]
    {
        "Happy", "Swift", "Brave", "Calm", "Clever", "Wise", "Noble", "Quick",
        "Bright", "Silent", "Bold", "Gentle", "Cool", "Cosmic", "Lucky", "Mystic",
        "Stellar", "Lunar", "Solar", "Azure", "Crimson", "Golden", "Silver", "Amber"
    };

    private static readonly string[] Nouns = new[]
    {
        "Tiger", "Eagle", "Dolphin", "Phoenix", "Dragon", "Wolf", "Hawk", "Lion",
        "Panda", "Falcon", "Raven", "Bear", "Fox", "Owl", "Lynx", "Jaguar",
        "Orca", "Koala", "Cheetah", "Panther", "Cobra", "Viper", "Sparrow", "Swan"
    };

    private readonly Random _random;
    private readonly HashSet<string> _usedNames;

    public GuestNameGenerator()
    {
        _random = new Random();
        _usedNames = new HashSet<string>();
    }

    /// <summary>
    /// Generates a unique guest name in the format "AdjectiveNoun####"
    /// Example: "BraveTiger1234"
    /// </summary>
    public string GenerateUniqueName()
    {
        string name;
        int attempts = 0;
        const int maxAttempts = 100;

        do
        {
            var adjective = Adjectives[_random.Next(Adjectives.Length)];
            var noun = Nouns[_random.Next(Nouns.Length)];
            var number = _random.Next(1000, 9999);
            
            name = $"{adjective}{noun}{number}";
            attempts++;

            if (attempts >= maxAttempts)
            {
                // Fallback to simple Guest#### format if too many collisions
                name = $"Guest{_random.Next(10000, 99999)}";
                break;
            }
        }
        while (_usedNames.Contains(name));

        _usedNames.Add(name);

        // Clean up old names if the set gets too large (keep last 1000)
        if (_usedNames.Count > 1000)
        {
            _usedNames.Clear();
        }

        return name;
    }

    /// <summary>
    /// Generates a simple guest name in the format "Guest####"
    /// Example: "Guest5678"
    /// </summary>
    public string GenerateSimpleName()
    {
        var number = _random.Next(1000, 9999);
        return $"Guest{number}";
    }

    /// <summary>
    /// Validates if a name follows the guest name pattern
    /// </summary>
    public bool IsGuestName(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return false;

        // Check if it's a simple Guest#### format
        if (System.Text.RegularExpressions.Regex.IsMatch(username, @"^Guest\d{4,5}$"))
            return true;

        // Check if it's an AdjectiveNoun#### format
        return System.Text.RegularExpressions.Regex.IsMatch(username, @"^[A-Z][a-z]+[A-Z][a-z]+\d{4}$");
    }
}
