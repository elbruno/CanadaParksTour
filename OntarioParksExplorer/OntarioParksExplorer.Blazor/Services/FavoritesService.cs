using Microsoft.JSInterop;

namespace OntarioParksExplorer.Blazor.Services;

public class FavoritesService
{
    private readonly IJSRuntime _jsRuntime;
    private const string StorageKey = "parkFavorites";

    public FavoritesService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<List<int>> GetFavoritesAsync()
    {
        try
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", StorageKey);
            if (string.IsNullOrEmpty(json))
                return new List<int>();
            
            return System.Text.Json.JsonSerializer.Deserialize<List<int>>(json) ?? new List<int>();
        }
        catch
        {
            return new List<int>();
        }
    }

    public async Task AddFavoriteAsync(int parkId)
    {
        var favorites = await GetFavoritesAsync();
        if (!favorites.Contains(parkId))
        {
            favorites.Add(parkId);
            await SaveFavoritesAsync(favorites);
        }
    }

    public async Task RemoveFavoriteAsync(int parkId)
    {
        var favorites = await GetFavoritesAsync();
        if (favorites.Contains(parkId))
        {
            favorites.Remove(parkId);
            await SaveFavoritesAsync(favorites);
        }
    }

    public async Task<bool> IsFavoriteAsync(int parkId)
    {
        var favorites = await GetFavoritesAsync();
        return favorites.Contains(parkId);
    }

    private async Task SaveFavoritesAsync(List<int> favorites)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(favorites);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, json);
    }
}
