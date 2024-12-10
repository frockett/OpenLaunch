using Radzen;

namespace OpenLaunch.Services;

public class DarkModeService
{
    public bool IsDarkMode { get; set; } = false;
    public Func<Task>? OnChange;
    public ThemeService ThemeService = new ThemeService();
    public async Task Toggle()
    {
        IsDarkMode = !IsDarkMode;
        ThemeService.SetTheme(IsDarkMode ? "material-dark" : "material");
        
        if (OnChange != null)
            await OnChange.Invoke();
    }
}