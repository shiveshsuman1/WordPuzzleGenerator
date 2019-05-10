using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataHandler
{
    public static class DataHandlerUtils
    {
        public static void SaveToFile(ISerializable objectToBeSaved)
        {

        }

        public static void CopyPuzzleGrid(Char[,] source, char[,] destination)
        {
            int iMax = source.GetLength(0);
            int jMax = source.GetLength(1);

            destination = new char[iMax,jMax];

            for(int i = 0; i < iMax; i++)
            {
                for(int j = 0; j < jMax; j++)
                {
                    destination[i,j] = source[i,j];
                }
            }
        }
    }
}
