using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace CheckMK.Livestatus
{
    public class Livestatus : IDisposable
    {

        private Socket _socket;

        public string Host { get; private set; }
        public int Port { get; private set; }

        public LQLOutputFormat OutputFormat = LQLOutputFormat.Text;

        public bool EncodeSemicolon = false;

        public Livestatus(string host, int port)
        {
            Host = host;
            Port = port;
        }

        /// <summary>
        /// Sends a LQL (Livestatus Query Language) to a CheckMK server and receives the response.
        /// For more information see: <see href="https://docs.checkmk.com/latest/en/livestatus.html#lql" />
        /// </summary>
        /// <param name="query">A valid LQL query</param>
        public string QueryLQL(string query)
        {
            // Add the default output format if non was given in the query
            if (!query.Contains("OutputFormat: ") && OutputFormat != LQLOutputFormat.Text)
            {
                if (!query.EndsWith(";"))
                    query += ";";

                if (OutputFormat == LQLOutputFormat.Json)
                    query += "OutputFormat: json;";
                else if (OutputFormat == LQLOutputFormat.CSV)
                    query += "OutputFormat: CSV;";
            }

            // Validate the LQL query
            if (!ValidateLQL(query.Replace('\n', ';'), false))
                return "Invalid query!";

            // Create a socket connection to the CheckMK server
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(Host, Port);

            // Encode query into utf8 and send it to the server
            byte[] data = Encoding.UTF8.GetBytes(EncodeLQL(query));
            int bytesSent = 0;

            while (bytesSent < data.Length)
            {
                bytesSent += _socket.Send(data, bytesSent, data.Length - bytesSent, SocketFlags.None);
            }

            // We are done sending data
            _socket.Shutdown(SocketShutdown.Send);

            byte[] responseBytes = new byte[4096];
            char[] responseChars = new char[4096];
            string response = "";

            // Receive bytes til there isn't (shocker)
            while (true)
            {
                int bytesReceived = _socket.Receive(responseBytes);

                // Receiving 0 bytes means EOF has been reached
                if (bytesReceived == 0) break;

                // Convert byteCount bytes to ASCII characters using the 'responseChars' buffer as destination
                int charCount = Encoding.ASCII.GetChars(responseBytes, 0, bytesReceived, responseChars, 0);

                response += new string(responseChars, 0, charCount).Trim();
            }

            // Dispose the socket, so new LQL query can be send
            _socket.Dispose();
            _socket = null;
            return response;
        }

        public string ExecuteAdvancedLQL(string query)
        {
            string[] queries = query.Split('\n');

            foreach (var item in queries)
            {
                if (!ValidateLQL(item, true))
                    return "Invalid query!";
            }

            return "";
        }

        private string EncodeLQL(string query)
        {
            if (EncodeSemicolon)
                query = query.Replace(';', '\n');
            
            query += '\n';
            return query;
        }

        private bool ValidateLQL(string query, bool advanced)
        {
            /*if (query.Length == 0)
                throw new LivestatusInvalidQueryException(1, 0, "Query cannot be the length 0.");

            if (query.StartsWith(";") || query.StartsWith("\n"))
                throw new LivestatusInvalidQueryException(1, 0, "Query cannot start with an ending character.");

            if (query.EndsWith(";"))
                query = query.Remove(query.Length - 1, 1);

            string[] commands = query.Split(';');

            int column = 1;

            bool foundMainCommand = false;

            // Check for empty commands
            foreach (string command in commands)
            {
                if (command.Length == 0)
                    throw new LivestatusInvalidQueryException(1, column, "Query cannot have empty commands.");

                string beginning = command.Contains(" ") ? command.Split(' ')[0] : command;
                if (beginning.EndsWith(":"))
                {
                    beginning = beginning.Remove(beginning.Length - 1, 1);
                }

                if (!ALLOWED_BEGINNINGS.Contains(beginning))
                {
                    throw new LivestatusInvalidQueryException(1, column, $"Invalid command beginning: {beginning}");
                }

                if (beginning == "GET")
                {
                    if (foundMainCommand)
                        throw new LivestatusInvalidQueryException(1, column, "Multiple GET's aren't allowed.");
                    foundMainCommand = true;
                }

                column += command.Length + 1;
            }*/

            QueryValidator validator = new QueryValidator(advanced);

            if (!validator.Validate(query))
                return false;

            return true;
        }

        public void Dispose()
        {
            if (_socket != null)
            {
                _socket.Dispose();
                _socket = null;
            }
        }
    }
}
