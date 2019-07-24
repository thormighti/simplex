using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace TrySim

{
  

    class MainClass
    {
        static void Main(string[] args)
        {
            /*
            Minimise P = 4x - 8y + 5z
      Subject to:                               from cypher test project.
            2x + 3y + z≤70
x + 2y + 2z≤60
3x + 4y + z≤84
x + y + z≥33
x≥0*/
            // this doesnt involve the penalty cost
            double[] obj = { 4,-8,5 };
            double[,] A = { {2,3,1 }, { 1,2,2}, { 3,4,1}, { 1,1,1} };
                double[] b = { 70,60,84,33};
            var s = new Simplex(obj, A, b);

           

            var answer = s.Minimization();
            Console.WriteLine($"The Maximization/Minimization is = {answer.Item1}");
            Console.WriteLine(string.Join(", ", answer.Item2));
        
            Console.ReadKey();
        }
    }
}
