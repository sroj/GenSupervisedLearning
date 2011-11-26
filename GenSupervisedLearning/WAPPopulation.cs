using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using AForge.Genetic;

namespace GenSupervisedLearning
{
    public class WAPPopulation : Population
    {
        public double DropConditionRate;
        public double AddAlternativeRate;
        protected Random rand;
        public bool parallelism;
        private int epoch;

        public WAPPopulation(int size, WAPChromosome ancestor, IFitnessFunction Func, ISelectionMethod sel)
            : base(size, ancestor, Func, sel)
        {
            AddAlternativeRate = 0.01;
            DropConditionRate = 0.6;
            rand = new Random();
            parallelism = false;
            epoch = 0;
        }

        public virtual void AddAlternative()
        {
            for (int i = 0; i < Size; i++)
                ((WAPChromosome)this[i]).AddAlternative();
        }

        public virtual void DropCondition()
        {
            for (int i = 0; i < Size; i++)
                ((WAPChromosome)this[i]).DropCondition();
        }

        private void ParallelAction(object param)
        {
            Thread.Sleep(0);
            KeyValuePair<int, int> lim = (KeyValuePair<int, int>)param;
            int begin = lim.Key, end = Math.Min(lim.Value, Size);
            bool eval;
            for (; begin < end; begin++)
            {
                eval = false;
                if (rand.NextDouble() <= AddAlternativeRate)
                {
                    ((WAPChromosome)this[begin]).AddAlternative();
                    eval = true;
                }
                if (rand.NextDouble() <= DropConditionRate)
                {
                    ((WAPChromosome)this[begin]).DropCondition();
                    eval = true;
                }
                if (eval) this[begin].Evaluate(FitnessFunction);
            }

            //Console.WriteLine("Thread {0} {1} of Epoch {2} Ended", lim.Key, lim.Value, epoch);
        }


        public override void Crossover()
        {
            base.Crossover();
        }

        public override void Selection()
        {
 	        base.Selection();
        }

        public override void Mutate()
        {
            base.Mutate();
        }


        public new void RunEpoch()
        {
            if (parallelism)
            {
                const int works = 250;
                Thread[] threads = new Thread[(int)Math.Ceiling((double)Size / (double)works)];
                
                for (int t = 0; t < threads.Length; t++)
                    threads[t] = new Thread(this.ParallelAction);

                for (int t = 0; t < threads.Length; t++)
                {
                    threads[t].Start(new KeyValuePair<int, int>(t * works, (t + 1) * works));
                    //Console.WriteLine("Thread {0} {1} of Epoch {2} Stared", t * works, (t + 1) * works, epoch);
                }

                for (int t = 0; t < threads.Length; t++)
                {
                    threads[t].Join();
                }
            }
            else
            {
                //Se aplica AddAlternative:
                AddAlternative();
                //Se aplica DropCondition:
                DropCondition();
            }


            base.RunEpoch();

            epoch++;
        }
    }
}
