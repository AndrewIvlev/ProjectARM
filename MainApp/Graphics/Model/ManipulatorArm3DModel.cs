using System.ComponentModel;
using System.Runtime.CompilerServices;
using ManipulationSystemLibrary.MathModel;

namespace MainApp.ViewModel
{
    using System.IO;

    using Newtonsoft.Json;

    public class ManipulatorArm3DModel : INotifyPropertyChanged
    {
        private Arm manipulator;
 
        public ManipulatorArm3DModel(Arm manip)
        {
            manipulator = manip;
        }
 
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public void ConvertFromJson(string jsonFilePath)
        {
            manipulator = JsonConvert.DeserializeObject<Arm>(File.ReadAllText(jsonFilePath));
        }

        // Начало планирования со следующей точки пути
        //public static List<double[]> MovingAlongTheTrajectory(Trajectory S, Arm model, List<Point3D> DeltaPoints, BackgroundWorker worker)
        //{
        //    var q = new List<double[]>();
            
        //    for (var i = 1; i < S.NumOfExtraPoints; i++)
        //    {
        //        var tmpQ = new double[model.n - 1];
        //        worker.ReportProgress((int)((float)i / S.NumOfExtraPoints * 100));
        //        for (var j = 0; j < model.n - 1; j++)
        //        {
        //            model.LagrangeMethodToThePoint(S.ExactExtra[i - 1]);
        //            tmpQ[j] = model.q[j];
        //        }
        //        q.Add(tmpQ);
        //        DeltaPoints.Add(new Point3D(i - 1, model.GetPointError(S.ExactExtra[i - 1]), 0));
        //    }

        //    return q;
        //}

        // Начало планирования с текущей точки пути
        /*public static double[][] MovingAlongTheTrajectory(Trajectory S, Arm model, List<Point3D> DeltaPoints, BackgroundWorker worker)
        {
            double[][] q = new double[S.NumOfExtraPoints][];
            for (int i = 0; i < S.NumOfExtraPoints; i++)
                q[i] = new double[model.n - 1];

            for (int i = 0; i < S.NumOfExtraPoints; i++)
            {
                worker.ReportProgress((int)((float)(i + 1) / S.NumOfExtraPoints * 100));
                for (int j = 0; j < model.n - 1; j++)
                    q[i][j] = model.LagrangeMethodToThePoint(S.ExactExtra[i])[j];

                DeltaPoints.Add(new Point3D(i, model.GetPointError(S.ExactExtra[i])));
            }

            return q;
        }*/
    }
}
