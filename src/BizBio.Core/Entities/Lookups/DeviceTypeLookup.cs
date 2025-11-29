namespace BizBio.Core.Entities.Lookups;

public class DeviceTypeLookup : EnumLookup
{
    public ICollection<NFCScan> NFCScans { get; set; } = new List<NFCScan>();
}