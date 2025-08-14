using System.Text;

namespace BinaryTypeDetector
{
    internal class Program
    {
        static void Main(string[] args)
        {
            {
                // Allow file path to be specified as command line argument
                string filePath;
                if (args.Length > 0)
                {
                    filePath = args[0];
                }
                else
                {
                    filePath = "C:\\Users\\usaha\\Downloads\\e37990dd-d748-4f84-9c07-a4c9f693d865.pdf"; // Default file path
                    Console.WriteLine($"No file path provided. Using default: {filePath}");
                    Console.WriteLine("Usage: dotnet run \"<file-path>\"");
                }

                // Check if file exists first
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"Error: File not found at '{filePath}'");
                    Console.WriteLine("Please verify the file path and try again.");
                    return;
                }

                // Dictionary of known file type signatures
                var magicNumbers = new Dictionary<string, byte[]>
                {
                    { "pdf", Encoding.ASCII.GetBytes("%PDF") },
                    { "png", new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } },
                    { "jpg", new byte[] { 0xFF, 0xD8, 0xFF } },
                    { "zip", new byte[] { 0x50, 0x4B, 0x03, 0x04 } }
                };

                // Read first 8 bytes from the file
                byte[] headerBytes = new byte[8];
                try
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        int bytesRead = fs.Read(headerBytes, 0, headerBytes.Length);
                        if (bytesRead == 0)
                        {
                            Console.WriteLine("Error: File is empty");
                            return;
                        }
                        
                        // Show the actual bytes read for debugging
                        Console.WriteLine($"Read {bytesRead} bytes: {BitConverter.ToString(headerBytes, 0, bytesRead)}");
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine("Error: Access denied to the file");
                    return;
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Error: IO Exception - {ex.Message}");
                    return;
                }

                // Check against known magic numbers
                bool matchFound = false;
                foreach (var kvp in magicNumbers)
                {
                    if (StartsWithBytes(headerBytes, kvp.Value))
                    {
                        Console.WriteLine($"It's a {kvp.Key.ToUpper()} file");
                        matchFound = true;
                        break;
                    }
                }

                if (!matchFound)
                {
                    Console.WriteLine("Unknown file type");
                }
            }

            // Helper method to compare byte sequences
            static bool StartsWithBytes(byte[] source, byte[] prefix)
            {
                if (prefix.Length > source.Length)
                    return false;

                for (int i = 0; i < prefix.Length; i++)
                {
                    if (source[i] != prefix[i])
                        return false;
                }
                return true;
            }
        }
    }
}
