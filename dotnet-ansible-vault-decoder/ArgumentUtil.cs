using System;
using System.Text;
using System.IO;

namespace dotnet_ansible_vault_decoder
{
    static class ArgumentUtil
    {
        public static Stream GetInputStream(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                return File.OpenRead(filePath);
            }
            else
            {
                return Console.OpenStandardInput();
            }
        }
        public static Stream GetOutputStream(string filePath, bool truncate)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                if (truncate)
                {
                    return File.Create(filePath);
                }
                else
                {
                    var ret = File.OpenWrite(filePath);
                    ret.Seek(0, SeekOrigin.End);
                    return ret;
                }
            }
            else
            {
                return Console.OpenStandardOutput();
            }
        }
        public static byte[] GetPassword(string password, string passfile, string passenv)
        {
            if (!string.IsNullOrEmpty(password))
            {
                return Encoding.UTF8.GetBytes(password);
            }
            else if(!string.IsNullOrEmpty(passenv))
            {
                return Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable(passenv));
            }
            else if(!string.IsNullOrEmpty(passfile))
            {
                using(var sr = File.OpenText(passfile))
                {
                    var l = sr.ReadLine();
                    return Encoding.UTF8.GetBytes(l.Trim());
                }
            }
            else
            {
                throw new ArgumentException("you must set either password, passfile, passenv");
            }
        }
        public static string EolOptionToEolString(string eol)
        {
            if (string.IsNullOrEmpty(eol))
            {
                return null;
            }
            if (eol.Equals("crlf", StringComparison.OrdinalIgnoreCase))
            {
                return "\r\n";
            }
            else if (eol.Equals("lf", StringComparison.OrdinalIgnoreCase))
            {
                return "\n";
            }
            else if (eol.Equals("cr", StringComparison.OrdinalIgnoreCase))
            {
                return "\r";
            }
            else
            {
                return null;
            }
        }
    }
}