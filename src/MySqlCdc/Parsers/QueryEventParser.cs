using MySqlCdc.Events;
using MySqlCdc.Protocol;

namespace MySqlCdc.Parsers
{
    /// <summary>
    /// Parses <see cref="QueryEvent"/> events.
    /// Supports all versions of MariaDB and MySQL.
    /// </summary>
    public class QueryEventParser : IEventParser
    {
        /// <summary>
        /// Parses <see cref="QueryEvent"/> from the buffer.
        /// </summary>
        public IBinlogEvent ParseEvent(EventHeader header, ref PacketReader reader)
        {
            long threadId = (uint)reader.ReadInt32LittleEndian();
            long duration = (uint)reader.ReadInt32LittleEndian();

            // DatabaseName length is null terminated
            reader.Advance(1);

            var errorCode = reader.ReadInt16LittleEndian();
            var statusVariableLength = reader.ReadInt16LittleEndian();
            var statusVariables = reader.ReadByteArraySlow(statusVariableLength);
            var databaseName = reader.ReadNullTerminatedString();
            var sqlStatement = reader.ReadStringToEndOfFile();

            return new QueryEvent(header, threadId, duration, errorCode, statusVariables, databaseName, sqlStatement);
        }
    }
}
