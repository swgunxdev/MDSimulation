using System.Runtime.InteropServices;
using Earlz.BareMetal;

public class Marshaller
{
    public static int SizeOf<T>(bool useBareMetal=true)
    {
        return useBareMetal ? BareMetal.SizeOf<T>() : SizeOf<T>();
    }
}