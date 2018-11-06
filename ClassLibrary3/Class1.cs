using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParserBAAK12
{
    public class ParseBinFileBAAK12
    {
        /// <summary>
        /// Парсер данных плат без хвоста, xLenght длительность данных 1024 или 2048 точек, для которого писать 1 или 2 соотвественно
        /// </summary>
        /// <param name="buf00"></param>
        /// <param name="xLenght"></param>
        /// <param name="data"></param>
        /// <param name="TimeData"></param>
        public static void ParseBinFileBAAK200(List<Byte> buf00, int xLenght, out Int32[,] data, out string TimeData)
        {
            data = new int[12, 1024 * xLenght];

            TimeData = null;


            Byte[] aTime = new Byte[8];
            for (int d = 0; d < 8; d++)
            {
                aTime[d] = buf00[12 + d];
            }

            TimeData =MyParseBinFileData(aTime);
            for (int i = 0; i < 12; i++)
            {


                for (int j = 0; j < (1024 * xLenght); j++)
                {
                   
                    data[i, j] = (((buf00[25 + (j * 2) + (i * 2048 * xLenght)] & 0xf) << 8) | buf00[24 + (j * 2) + (i * 2048 * xLenght)]);

                }


            }

        }
        /// <summary>
        /// Парсер данных платы с хвостом
        /// </summary>
        /// <param name="buf00"></param>
        /// <param name="data"></param>
        /// <param name="TimeData"></param>
        /// <param name="dataTail"></param>
        public static void ParseBinFileBAAK200H(Byte[] buf00,  out Int32[,] data, out string TimeData, out Int32[,] dataTail)
        {
            data = new int[12, 1024];
            dataTail = new int[12, 20000];
            TimeData = null;


            Byte[] aTime = new Byte[8];
            for (int d = 0; d < 8; d++)
            {
              aTime[d] = buf00[480000+8+24+ d];
            }
            TimeData = MyParseBinFileData(aTime);
            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < (1024); j++)
                {
                    data[i, j] = (((buf00[480000+24 + 41 + (j * 2) + (i * 2048)] & 0xf) << 8) | buf00[480000+24 + 40 + (j * 2) + (i * 2048)]);
                }
            }
            for (int j = 0; j < 20000; j++)
            {
                for (int i = 0; i < 12; i++)
                {
                    dataTail[i, j] = (((buf00[25+ (i * 2) + (j*24)] & 0xf) << 8) | buf00[24+(i * 2) + (j*24)]);
                }
            }

        }

        private static string MyParseBinFileData(Byte[] bytTime)
        {
            string ns = (((bytTime[0] & 0x7f))*10).ToString("000");//ok
            string mcs = (((bytTime[2] & 0x1) << 9) | (bytTime[1] << 1) | (bytTime[0] >> 7)).ToString("000");//ok
            string ms = (((bytTime[3] & 0x07) << 7) | ((bytTime[2] >> 1) & 0x7f)).ToString("000");//ok
            string sec = (((bytTime[4] & 0x01) << 5) | (bytTime[3]) >> 3).ToString("00");
            string min = ((bytTime[4] >> 1) & 0x3f).ToString("00");
            string hh = (((bytTime[5] & 0x0f) << 1) | (bytTime[4] >> 7)).ToString("00");//ok

            string dd = (((bytTime[6] & 0x03) << 4) | (bytTime[5] >> 4)).ToString("00");

          
            return dd + "." + hh + "." + min + "." + sec + "." + ms + "." + mcs + "." + ns;


        }
    }
}
