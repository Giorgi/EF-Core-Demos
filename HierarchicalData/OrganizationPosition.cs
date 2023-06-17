using Microsoft.EntityFrameworkCore;

namespace HierarchicalData;

public class OrganizationPosition
{
    public int Id { get; set; }
    public HierarchyId Path { get; set; }
    public string Name { get; set; }
}