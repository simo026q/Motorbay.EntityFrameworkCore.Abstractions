using System.Reflection;
using System.Runtime.InteropServices;

namespace Motorbay.EntityFrameworkCore.Abstractions.Repositories.Caching;

internal static class MemoryHelper
{
    public static int GetSize<T>() 
        => GetSize(typeof(T));

    public static int GetSize(Type type)
    {
        if (type.IsPrimitive)
        {
            return Marshal.SizeOf(type);
        }

        int size = 0;

        foreach (FieldInfo field in type.GetFields(BindingFlags.Instance))
        {
            size += GetSize(field.FieldType);
        }

        return size;
    }
}
