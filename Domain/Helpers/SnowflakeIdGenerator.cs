using System.Net.NetworkInformation;
using System.Text;

namespace url_shortener_dotnet.Domain.Helpers;

public class SnowflakeIdGenerator
{
    private readonly long datacenterId;
    private readonly long machineId;
    private readonly long epoch;

    private const long DatacenterIdBits = 5L;
    private const long MachineIdBits = 5L;
    private const long SequenceBits = 12L;

    private const long MaxDatacenterId = -1L ^ (-1L << (int)DatacenterIdBits);
    private const long MaxMachineId = -1L ^ (-1L << (int)MachineIdBits);
    private const long SequenceMask = -1L ^ (-1L << (int)SequenceBits);

    private const int DatacenterIdShift = (int)SequenceBits + (int)MachineIdBits;
    private const int TimestampLeftShift = DatacenterIdShift + (int)DatacenterIdBits;
    private const int MachineIdShift = (int)SequenceBits;

    private long sequence = 0L;
    private long lastTimestamp = -1L;

    private static readonly object syncRoot = new object();

    //Epoch default is January 1, 2020
    public SnowflakeIdGenerator(long datacenterId, long machineId, long epoch = 1577836800000L)
    {
        if (datacenterId > MaxDatacenterId || datacenterId < 0)
        {
            throw new ArgumentException($"Datacenter Id can't be greater than {MaxDatacenterId} or less than 0");
        }

        if (machineId > MaxMachineId || machineId < 0)
        {
            throw new ArgumentException($"Machine Id can't be greater than {MaxMachineId} or less than 0");
        }

        this.datacenterId = datacenterId;
        this.machineId = machineId;
        this.epoch = epoch;
    }

    public long GenerateId()
    {
        lock (syncRoot)
        {
            var timestamp = CurrentTimeMillis();

            if (timestamp < lastTimestamp)
            {
                throw new InvalidOperationException("Clock moved backwards. Refusing to generate id");
            }

            if (lastTimestamp == timestamp)
            {
                sequence = (sequence + 1) & SequenceMask;
                if (sequence == 0)
                {
                    timestamp = WaitForNextMillis(lastTimestamp);
                }
            }
            else
            {
                sequence = 0L;
            }

            lastTimestamp = timestamp;

            return ((timestamp - epoch) << TimestampLeftShift) |
                   (datacenterId << DatacenterIdShift) |
                   (machineId << MachineIdShift) |
                   sequence;
        }
    }

    private static long WaitForNextMillis(long lastTimestamp)
    {
        var timestamp = CurrentTimeMillis();
        while (timestamp <= lastTimestamp)
        {
            timestamp = CurrentTimeMillis();
        }
        return timestamp;
    }

    private static long CurrentTimeMillis()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}