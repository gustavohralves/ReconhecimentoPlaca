using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Tesseract;

namespace ReconhecimentoPlacaOCR
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new StreamWriter(@"S:\ocr\ResultadoPlacasIdentificadas.txt", true, Encoding.ASCII);
            var resultado = new Dictionary<string, string>();
            var filtro = "";
            try
            {
                var cronometro = Stopwatch.StartNew();

                for (int i = 1; i < 4; i++)
                {
                    var imagem = $@"S:\ocr\placaTeste4.png";
                    //var imagem = $@"S:\ocr\placa{i}.png";
                    filtro = imagem.Contains("placan") ? "com filtro" : "sem filtro";

                    using var engine = new TesseractEngine(@"tessdata", "eng", EngineMode.Default);
                    using var img = Pix.LoadFromFile(imagem);
                    using var page = engine.Process(img);

                    var texto = page.GetText().Trim();
                    var regex = @"(?<p1>[a-zA-Z]+).+?(?<p2>\d+)";

                    var p1 = Regex.Match(texto, regex).Groups["p1"];
                    var p2 = Regex.Match(texto, regex).Groups["p2"];
                    texto =$"{p1}-{ p2}";

                    Console.WriteLine("Taxa de Precisao: {0}", page.GetMeanConfidence());
                    Console.WriteLine("Texto: \r\n{0}", texto);

                    if (texto.Length > 0)
                        resultado.Add(imagem, texto);
                    else
                        resultado.Add(imagem, "Não foi possível obter conteúdo da imagem");
                }
                cronometro.Stop();

                sw.WriteLine($"Placas Obtidas em: {DateTime.Now}\n");

                foreach (var item in resultado)
                    sw.WriteLine(item.ToString().Normalize().Trim());

                sw.WriteLine($"Tempo de execucao c/ imagens {filtro}: {cronometro.Elapsed}");
                sw.WriteLine("\n--------------------------------------");
                sw.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro: {0}", ex.Message);
            }
            finally
            {
                Console.ReadLine();
            }
        }
    }
}
