using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrySim
{
  public  class Simplex
    {
        private double[] objectiveFunc; //objective fucntion
        private double[,] A;
        private double[] b;
        private List<int> NonBasicVar = new List<int>();   // storing the non basic variables
        private List<int> BasicVar = new List<int>();   // for storing the basic variable
        private double v = 0; // to store the optimal value
        
        public Simplex(double[] objectiveFunc, double[,] A, double[] b)   // constructr
        {
            int lengthOfObj = objectiveFunc.Length;
             int   lengthOfConstraint = b.Length;

            if (lengthOfObj != A.GetLength(1))
            {
                throw new Exception("Number of variables in c doesn't match number in A.");
            }
            
            if (lengthOfConstraint != A.GetLength(0))
            {
                throw new Exception("Number of constraints in A doesn't match number in b.");
            }

            // Extending the Obj coefficients vector with 0 padding
            this.objectiveFunc = new double[lengthOfObj + lengthOfConstraint];
            Array.Copy(objectiveFunc, this.objectiveFunc, lengthOfObj); // copying the element of c into the refrence, remaining length taken as zero. observes slack number == b.lenth
            
            // Extending the Variables abi A coefficient matrix with 0 padding
            this.A = new double[lengthOfObj + lengthOfConstraint, lengthOfObj + lengthOfConstraint];
            for (int i = 0; i < lengthOfConstraint; i++)
            {
                for (int j = 0; j < lengthOfObj; j++)
                {
                    this.A[i + lengthOfObj, j] = A[i, j];  // values of A stored in the exttension
                }
            }

            // Extending the constraint right-hand side vector with 0 padding
            this.b = new double[lengthOfObj + lengthOfConstraint];
            Array.Copy(b, 0, this.b, lengthOfObj, lengthOfConstraint); // extend from element index at 0 to index[obj length], with b.lenth as the length
            
            // Populating the non-basic and basic sets
            for (int i = 0; i < lengthOfObj; i++) // chek out why leobj
            {
                NonBasicVar.Add(i);
            }
            
            for (int i = 0; i < lengthOfConstraint; i++) // consraints holds the decision var.
            {
                BasicVar.Add(lengthOfObj + i);
            }
        }
        private void pivot(int e, int l)
        {
            //N.Remove(e);
            //B.Remove(l);

            b[e] = b[l] / A[l, e]; //  b of the pivot

            foreach (var j in NonBasicVar)
            {
                A[e, j] = A[l, j] / A[l, e]; // new var coff , dividing through by pivot elemet for nonbasic var
            }

            A[e, l] = 1 / A[l, e];  //  making the pivot element in unity , thats == 1;

            foreach (var i in BasicVar) // i is rows, including all the zeros
            {
                b[i] -= A[i, e] * b[e]; // storing the new be's I multiplied the element in the pivot colomn by the pivot b

                foreach (var j in NonBasicVar) 
                {                                                 // nested loop here
                    A[i, j] -=  A[i, e] * A[e, j];   //doing the calculations
                }

                A[i, l] = -1 * A[i, e] * A[e, l];             // storing in the pivot column
            }

            v  += objectiveFunc[e] * b[e];   //   sores the objective function

            foreach (var j in NonBasicVar)
            {
                objectiveFunc[j] = objectiveFunc[j] - objectiveFunc[e] * A[e, j];
            }

            objectiveFunc[l] = -1 * objectiveFunc[e] * A[e, l];

            NonBasicVar.Add(l);
            BasicVar.Add(e);
        }
        public Tuple<double, double[]> Minimization()
        {
            while (true)
            {
                // Find highest coefficient for entering var
                int e = -1; // standardizing the objective function
                double ce = 0;
                foreach (int j in NonBasicVar) // finding the coffecient of the nonbasic variables in objfunc. j is d item index
                {
                    if (objectiveFunc[j] < ce)  // finds the most negative
                    {
                        ce = objectiveFunc[j];
                        e = j;                           // e becomes the entering index
                    }
                }

                // If no coefficient < 0, there's no more mainimizing to do, and we're almost done
                if (e == -1) break;   // if e == -1break.no negative index

                // Find lowest check ratio
                double minRatio = double.MaxValue; // setting to a very large number. Oko help
                int l = 0;
                foreach (int i in BasicVar) // and here
                {
                    if (A[i, e] > 0)
                    {
                        double r = b[i] / A[i, e];
                        if (r < minRatio)
                        {
                            minRatio = r;
                            l = i;
                        }
                    }
                }

                //checking for  Unboundedness
                if (minRatio < 0)
                {
                    Console.WriteLine("the solution is infeasible and unbounded");
                }

                pivot(e, l);   // setting the pivot
            }

            // Extract amounts and slack for optimal solution
            double[] x = new double[b.Length];
            int n = b.Length;
            for (var i = 0; i < n; i++)
            {
                x[i] = BasicVar.Contains(i) ? b[i] : 0; // x checks for the calculated basic variables contained in BasicVAr
            }

            // Return max and variables
            return Tuple.Create(v, x);
        }

        
        public Tuple<double, double[]> Mazimization() // need to return two values
        {
            while (true)
            {
                // Find highest coefficient for entering var
                int e = -1; // standardizing the obj func
                double ce = 0;
                foreach (int j in NonBasicVar)
                {
                    if (objectiveFunc[j] > ce)   // finds the most positive
                    {
                        ce = objectiveFunc[j];
                        e = j;
                    }
                }

                // If no coefficient > 0, there's no more maximizing to do, and we're almost done
                if (e == -1) break;

                // Find lowest check ratio
                double minRatio = double.MaxValue;
                int l = 0;
                foreach (int i in BasicVar)
                {
                    if (A[i, e] > 0)
                    {
                        double r = b[i] / A[i, e];
                        if (r < minRatio)
                        {
                            minRatio = r;
                            l = i;
                        }
                    }
                }

                //checking for  Unboundedness
                if (minRatio < 0)
                {
                    Console.WriteLine("the solution is infeasible and unbounded");
                }

                pivot(e, l);
            }

            // Extract amounts and slack for optimal solution
            double[] x = new double[b.Length];
            int n = b.Length;
            for (var i = 0; i < n; i++)
            {
                x[i] = BasicVar.Contains(i) ? b[i] : 0; // extracting our slack and surplus
            }
           
            // Return max and variables
            return Tuple.Create(v, x); // returns the objective and and array of the answers
        }

    }
}
