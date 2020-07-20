public struct VoxelID
{
    public uint Value;
    public VoxelID(uint value)
    {
        Value = value;
    }
    public static implicit operator VoxelID(uint value)
    {
        return new VoxelID(value);
    }
    public static implicit operator VoxelID(int value)
    {
        return new VoxelID((uint)value);
    }
    public static bool operator==(VoxelID a,VoxelID b)
    {
        return a.Value == b.Value;
    }
    public static bool operator !=(VoxelID a, VoxelID b)
    {
        return !(a==b);
    }

    public override bool Equals(object obj)
    {
        return obj.GetHashCode() == GetHashCode();
    }
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}
