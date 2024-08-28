using TT.Domain.Models;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Server.Features.Map;

public class MapCell(Location location, LocationInfo enchantment, (decimal minX, decimal maxY) boundary, (int x, int y) playerLocation)
{
    public bool HasRightBorder { get; } = location.Name_East == null;
    public bool HasTopBorder { get; } = location.Name_North == null;
    public bool HasLeftBorder { get; } = location.Name_West == null;
    public bool HasBottomBorder { get; } = location.Name_South == null;
    public int PosX { get; } = (int)((location.X - boundary.minX) * 100 + 10);
    public int PosY { get; } = (int)(-(location.Y - boundary.maxY) * 100 + 10);
    public string Background { get; } = playerLocation.x == location.X && playerLocation.y == location.Y 
        ? "lightpink" : location.Region == "streets" 
            ? "snow" : "gainsboro";
    public bool ShowEnchantment { get; } = enchantment != null;
    public string LocationName { get; } = location.Name;
    public CovenantNameFlag Covenant { get; } = enchantment?.CovenantId != null && CovenantDictionary.IdNameFlagLookup.TryGetValue(enchantment.CovenantId.Value, out var value) ? value : null;
    public float TakeOverAmount { get; } = location.TakeoverAmount;
}