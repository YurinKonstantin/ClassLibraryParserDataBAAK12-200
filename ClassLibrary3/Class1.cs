using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static void ParseBinFileBAAK200(Byte[] buf00, int xLenght, out Int32[,] data, out string TimeData)
        {

            data = new int[12, (int)(buf00.Length/12)];

            TimeData = String.Empty;


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
        /// Определяет максимальную амплитуда сигнала на канале, определяет евляется ли сигнал шумом
        /// </summary>

        /// <param name="data1">значение развертки для 12ти каналов</param>
        /// <param name="sig">сигма для каждого канала</param>
        /// <param name="Amp">Максимальная амплитуда</param>
        /// <param name="Nul">Определенная нулевая линия</param>
        /// <param name="bad">если tru то сигнал шумовой</param>
        /// <param name="obrNoise">Определяет необходимость проводить обработку на наличие шумов</param>
        /// <param name="KoefNoise">соотношение сигнала к просадке после, для определения шума</param>
        /// <param name="obrNoise">Амплитуда шума</param>
        /// <param name="obrNoise">Порого срабатывания канала</param>
        public static void MaxAmpAndNul(int[,] data1, ref double[] sig, ref int[] Amp, ref double[] Nul, ref bool bad, bool obrNoise, double KoefNoise, int AmpNoise)
        {
           // bad = false;
           // sig = new Double[12];
           // Amp = new int[12];
            double min;
            int xbad = 0;
            //bool obrNoise = ClassUserSetUp.ObrNoise;
            // Nul = new double[12];
            int Nsob = 150;//число точек от начала для поиска нулевой линнии
            int[] sumNul = new int[Nsob];
            
            for (int i = 0; i < 12; i++)
            {
               // точки нулевой линии для "a" - го канала
                                             // double sred = Enumerable.Range(0, 2).Select(x => f[x]).Sum();
              //  for (int a = 0; a < Nsob; a++)
              //  {
                    // Nul[i] = (Nul[i] + data1[i, a]);
                 //   sumNul[a] = data1[i, a];// точки нулевой линии для "a"-го канала
               // }
                int[] p = Enumerable.Range(0, 150).Select(x => data1[i, x]).ToArray();
                int nul = (int)p.Average();
                Nul[i] = nul;
                sig[i] = Math.Sqrt(Sum(p, nul) / Nsob);

                int aa = (Enumerable.Range(0, (data1.Length / 12)).Select(x => data1[i, x]).Max()) - nul;
                Amp[i] = aa;
                min = (Enumerable.Range(0, 1024).Select(x => data1[i, x]).Min()) - nul;

                if ((obrNoise && (Math.Abs(min / aa) > KoefNoise) && Math.Abs(aa) > AmpNoise) || Nul[i]<2000)
                {
                    xbad++;
                }

            }
      
            if (xbad > 0)
            {
                bad = true;
            }
        }
        static double  sum1D(double[] masD)
        {
            return Enumerable.Range(300, 700).Select(x => masD[x]).Sum();

        }
        public static void SumSig(double[,] data1, out double[] sumsin)
        {

            //int[] sumD = new int[1024];
            double[] mas1 = new Double[1024];
            sumsin = new double[12];
            for (int i = 0; i < 12; i++)
            {

                // double sred = Enumerable.Range(0, 2).Select(x => f[x]).Sum();
                for (int j = 0; j < 1014; j++)
                {
                    mas1[j] = data1[i, j];
                   
                }


                sumsin[i] = sum1D(mas1);
                mas1 = new double[1024];

            }
           
            // double sred = Enumerable.Range(1, 9).Select(x => mas[x]).Sum();
          

        }
        private static double Sum(int[] n, double x)
        {
            double res=0;
           for (int i=0; i<n.Length; i++)
            {
                res = res + Math.Pow((n[i] - x), 2);
            }
            return res;
        }
        /// <summary>
        /// Парсер данных платы с хвостом
        /// </summary>
        /// <param name="buf00"></param>
        /// <param name="data"></param>
        /// <param name="TimeData"></param>
        /// <param name="dataTail"></param>
        public static string ParseBinFileBAAK200H(byte[] buf00,  out int[,] data, out string TimeData, out int[,] dataTail)
        {
            data = new int[12, 1024];
            dataTail = new int[12, 20000];
            TimeData =String.Empty;
            try
            {
                byte[] aTime = new byte[8];
               // for (int d = 0; d < 8; d++)
               // {
               //     aTime[d] = buf00[480032+ d];
                //}
                aTime[0] = buf00[480032];
                aTime[1] = buf00[480033];
                aTime[2] = buf00[480034];
                aTime[3] = buf00[480035];
                aTime[4] = buf00[480036];
                aTime[5] = buf00[480037];
                aTime[6] = buf00[480038];
                aTime[7] = buf00[480039];
                TimeData = MyParseBinFileData(aTime);
                 for (int i = 0; i < 12; i++)
                 {
                     for (int j = 0; j < 20000; j++)
                     {

                         dataTail[i, j] = (((buf00[25 + (i * 2) + (j * 24)] & 0xf) << 8) | buf00[24 + (i * 2) + (j * 24)]);

                     }
                     for (int j = 0; j < (1024); j++)
                     {
                         data[i, j] = (((buf00[480065 + (j * 2) + (i * 2048)] & 0xf) << 8) | buf00[480064 + (j * 2) + (i * 2048)]);
                     }


                 }
             
                return "1";
            }
            catch(Exception ex)
            {
                return ex.ToString();
            }
            return "1";
        }

        private static string MyParseBinFileData(Byte[] bytTime)
        {
           

          //  string dd = (((bytTime[6] & 0x03) << 4) | (bytTime[5] >> 4)).ToString("00");

          
            return (((bytTime[6] & 0x03) << 4) | (bytTime[5] >> 4)).ToString("00") + "." + (((bytTime[5] & 0x0f) << 1) | (bytTime[4] >> 7)).ToString("00") + "." + ((bytTime[4] >> 1) & 0x3f).ToString("00") + "." + (((bytTime[4] & 0x01) << 5) | (bytTime[3]) >> 3).ToString("00")
                + "." + (((bytTime[3] & 0x07) << 7) | ((bytTime[2] >> 1) & 0x7f)).ToString("000")
                + "." + (((bytTime[2] & 0x1) << 9) | (bytTime[1] << 1) | (bytTime[0] >> 7)).ToString("000") + "." + (((bytTime[0] & 0x7f)) * 10).ToString("000"); ;


        }

        public static int[] TimeS(int[,] data1, int porogS, int[] Amp, double[] Nul)
        {
            int[] Time = new int[12];
            for(int i=0; i<12; i++)
            {
                if(porogS <= Amp[i])
                {
                    for(int j=0; j<1024; j++)
                    {
                        if(data1[i,j]>=porogS+ Nul[i])
                        {
                            Time[i] = j;
                            break;
                        }

                    }
                }
                else
                {
                    Time[i] = -1;
                }
            }


            return Time;
        }
    }
}
