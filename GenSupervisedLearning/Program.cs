using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using AForge.Genetic;

namespace GenSupervisedLearning
{
    class Program
    {

        static void readFile(String fileName, int[][] examples)
        {
            StreamReader s = new StreamReader(fileName);
            String line;
            String[] tokens;
            char[] delimiterChars = { ',' };
            int i = 0;
            while ((line = s.ReadLine()) != null)
            {
                tokens = line.Split(delimiterChars);
                examples[i] = new int[tokens.Length];

                for (int j = 0; j < tokens.Length; j++)
                {
                    examples[i][j] = int.Parse(tokens[j]);
                }
                i++;
            }
            s.Close();
        }

        static void Main(string[] args)
        {
            try
            {
                int numLines = 1473;
                int maxIter = 1500;

                int[][] examples = new int[numLines][];
                readFile("cmc.data", examples);
                
                IFitnessFunction f = new FitnessFunction(examples, true);

                int method = 0, best_method = method;
                ISelectionMethod[] methods = new ISelectionMethod[]{new RouletteWheelSelection(), new RankSelection(), new EliteSelection()};

                WAPChromosome c = new WAPChromosome(5 * 44);
                WAPChromosome legend = c;
                legend.Evaluate(f);

                double bestMR = 0, bestSP = 0, bestDC = 0, bestAA = 0;
                WAPPopulation pop = new WAPPopulation(100, c, f, methods[0]);
                pop.parallelism = true;

                Random rand = new Random();
                
                
                while (true)
                {
                    pop.RandomSelectionPortion = rand.NextDouble() * 0.7;
                    pop.MutationRate           = 0.1 + rand.NextDouble() * 0.3;
                    pop.SelectionMethod        = methods[method];
                    pop.DropConditionRate      = rand.NextDouble() * 0.5;
                    pop.AddAlternativeRate     = rand.NextDouble() * 0.5;

                    method = (method + 1) % methods.Length;

                    Console.WriteLine("Entrenando: Metodo:{0}, DC:{1}, AA:{2}, SP:{3}, MR:{4}", method, pop.DropConditionRate, pop.AddAlternativeRate, pop.RandomSelectionPortion, pop.MutationRate);
                    
                    
                    pop.Regenerate();
                    for (int i = 0; i < maxIter; i++)
                    {
                        pop.RunEpoch();
                        int prom = 0;
                        for (int j = 0; j < pop.Size; j++)
                            prom += ((WAPChromosome)pop[j]).numRules;
                        prom = prom / pop.Size;
                        //Console.WriteLine("Generacion {0} Mejor Fitness {1} prom: {2}, reglas:{3}", i, pop.BestChromosome.Fitness, pop.FitnessAvg, prom);
                        if (pop.BestChromosome.Fitness > legend.Fitness)
                        {
                            legend = (WAPChromosome)pop.BestChromosome;
                            best_method = method; bestAA = pop.AddAlternativeRate; bestDC = pop.DropConditionRate;
                            bestMR = pop.MutationRate; bestSP = pop.RandomSelectionPortion;
                        }
                    }

                    Console.WriteLine("\n\nMejor Cromosoma de la ultima generacion:");
                    Console.WriteLine("Fitness {0}", pop.BestChromosome.Fitness);
                    Console.WriteLine("Clasificados {0}", Math.Sqrt(pop.BestChromosome.Fitness) + (double)((WAPChromosome)pop.BestChromosome).numRules / (double)examples.Length / 2.0);
                    Console.WriteLine("Reglas {0}", ((WAPChromosome)pop.BestChromosome).numRules);

                    //Console.WriteLine(legend);
                    Console.WriteLine("\nMejor Cromosoma de todas las generaciones:");
                    Console.Write("Parm: Metodo:{0}, DC:{1}, AA:{2}, SP:{3}, MR:{4}", best_method, bestDC, bestAA, bestSP, bestMR);
                    Console.WriteLine("Fitness {0}", legend.Fitness);
                    Console.WriteLine("Clasificados {0}", Math.Sqrt(legend.Fitness) + (double)((WAPChromosome)legend).numRules / (double)examples.Length / 2.0);
                    Console.WriteLine("Reglas {0}", legend.numRules);
                    Console.WriteLine(legend);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
