using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test_app2.Utilities
{
    internal class HEXConverter
    {
        private string[]? _inputString;
        private byte[]? _outputString;

        public string[] InputString
        { 
            get => _inputString;  
            set => _inputString = value;
        }

        public byte[] OutputSting
        {
            get => _outputString;
            set => _outputString = value;
        }

        public byte[] StringToByteArray(String hex)
        {
            
            InputString = hex.Split(' ');
            OutputSting = new byte[InputString.Length];
            for (var i = 0; i < InputString.Length; i++)
            {
                OutputSting[i] = Convert.ToByte(InputString[i], 16);
            }
            return OutputSting;
        }
    }
}
