using Microsoft.JSInterop;

namespace HorizonChat.Services;

public class UsernameService
{
    private readonly IJSRuntime _jsRuntime;
    private string? _currentUsername;
    private const string StorageKey = "horizonChat_username";

    public event Action? OnUsernameChanged;

    public UsernameService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public string? CurrentUsername => _currentUsername;

    public async Task<string?> GetUsernameAsync()
    {
        if (_currentUsername != null)
            return _currentUsername;

        try
        {
            _currentUsername = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", StorageKey);
            return _currentUsername;
        }
        catch
        {
            return null;
        }
    }

    public async Task SetUsernameAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty", nameof(username));

        _currentUsername = username;
        
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, username);
            OnUsernameChanged?.Invoke();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save username to localStorage: {ex.Message}");
        }
    }

    public async Task ClearUsernameAsync()
    {
        _currentUsername = null;
        
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", StorageKey);
            OnUsernameChanged?.Invoke();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to remove username from localStorage: {ex.Message}");
        }
    }

    public bool HasUsername()
    {
        return !string.IsNullOrWhiteSpace(_currentUsername);
    }
}
