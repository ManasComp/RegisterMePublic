#region

using Microsoft.AspNetCore.Mvc.Rendering;

#endregion

namespace WebGui.Areas.Visitor.Controllers.ViewModelServices;

public class SelectListItemComparer : IEqualityComparer<SelectListItem>
{
    public bool Equals(SelectListItem? x, SelectListItem? y)
    {
        if (x == null || y == null)
        {
            return x == y;
        }

        if (x.Group == null || y.Group == null)
        {
            return x.Group?.Name == y.Group?.Name;
        }

        return x.Text == y.Text && x.Value == y.Value && x.Group.Name == y.Group.Name;
    }

    public int GetHashCode(SelectListItem obj)
    {
        return HashCode.Combine(obj.Text, obj.Value, obj.Group?.Name);
    }
}
