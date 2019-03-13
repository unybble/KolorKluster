using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace KolorKluster.Data
{
    public class Classifier
    {
        private List<RGBColor> _data;
        private List<RGBColor> _clusters;
        private int _numberOfClusters = 0;
        private Random _random;
        public Classifier(int numClusters)
        {
            _data = new List<RGBColor>();
            _clusters = new List<RGBColor>();
            _numberOfClusters = numClusters;
            _random = new Random();
            for (int i = 0; i < _numberOfClusters; i++)
            {
                _clusters.Add(new RGBColor(_random.Next(1,255), _random.Next(1, 255), _random.Next(1, 255))
                 { Cluster = i });
            }

            MakeCluster(_data, _numberOfClusters);
        }


        public string GetStringOutput()
        {
            StringBuilder sb = new StringBuilder();
            var group = _data.GroupBy(s => s.Cluster).OrderBy(s => s.Key);
            foreach (var g in group)
            {
                sb.AppendLine("Cluster # " + g.Key + ":");
                foreach (var value in g)
                {
                    sb.Append("("+value.R.ToString()+",");
                    sb.Append(value.G.ToString() + ",");
                    sb.Append(value.B.ToString()+")");

                    sb.AppendLine();
                }
                sb.AppendLine("------------------------------");
            }
            return sb.ToString();
        }

        public List<Cluster> GetClusters()
        {
            List<Cluster> clusters = new List<Cluster>();


            var group = _data.GroupBy(s => s.Cluster).OrderBy(s => s.Key);
            foreach (var g in group)
            {
                var c = new Cluster()
                {
                    ID = g.Key,
                    colors = new List<RGBColor>()
            };

                foreach (var value in g)
                {
                    var color = new RGBColor(value.R,value.G,value.B);
                    c.colors.Add(color);
                }
                clusters.Add(c);
            }
            return clusters;
        }


        public void Initialize(int numberDataPoints)
        {
             _data = DataMaker.GetColors(numberDataPoints);
             _random = new Random(_numberOfClusters);
            for (int i = 0; i < _numberOfClusters; ++i)
            {
                _data[i].Cluster = _data[i].Cluster = i;
            }
            for (int i = _numberOfClusters; i < _data.Count; i++)
            {
                _data[i].Cluster = _data[i].Cluster = _random.Next(0, _numberOfClusters);
            }
        }
        private bool UpdateColorMeans()
        {
            if (EmptyCluster(_data)) return false;

            var groupToComputeMeans = _data.GroupBy(s => s.Cluster).OrderBy(s => s.Key);
            int clusterIndex = 0;
            int r = 0;
            int g = 0;
            int b = 0;
            int br = 0;
            int gb = 0;
            int rg = 0;
            foreach (var item in groupToComputeMeans)
            {
                foreach (var value in item)
                {
                    r += value.R;
                    g += value.G;
                    b += value.B;
                    br += value.BR;
                    gb += value.GB;
                    rg += value.RG;
                }
                _clusters[clusterIndex].R = r / item.Count();
                _clusters[clusterIndex].G = g / item.Count();
                _clusters[clusterIndex].B = b / item.Count();
                _clusters[clusterIndex].BR = br / item.Count();
                _clusters[clusterIndex].GB = gb / item.Count();
                _clusters[clusterIndex].RG= rg / item.Count();
                clusterIndex++;

                r = 0;
                g = 0;
                b = 0;
                 br = 0;
                 gb = 0;
                 rg = 0;
            }
            return true;
        }
        private bool EmptyCluster(List<RGBColor> data)
        {
            var emptyCluster =
            data.GroupBy(s => s.Cluster).OrderBy(s => s.Key).Select(g => new { Cluster = g.Key, Count = g.Count() });

            foreach (var item in emptyCluster)
            {
                if (item.Count == 0)
                {
                    return true;
                }
            }
            return false;
        }
        private double EuclideanDistance(RGBColor dataPoint, RGBColor mean)
        {
            double _diffs = 0.0;
            _diffs = Math.Pow(dataPoint.R - mean.R, 2);
            _diffs += Math.Pow(dataPoint.G - mean.G, 2);
            _diffs += Math.Pow(dataPoint.B - mean.B, 2);
            _diffs = Math.Pow(dataPoint.BR - mean.BR, 2);
            _diffs += Math.Pow(dataPoint.GB- mean.GB, 2);
            _diffs += Math.Pow(dataPoint.RG - mean.RG, 2);

            return Math.Sqrt(_diffs);
        }
        private bool UpdateClusterMembership()
        {
            bool changed = false;

            double[] distances = new double[_numberOfClusters];

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _data.Count; ++i)
            {

                for (int k = 0; k < _numberOfClusters; ++k)
                    distances[k] = EuclideanDistance(_data[i], _clusters[k]);

                int newClusterId = MinIndex(distances);
                if (newClusterId != _data[i].Cluster)
                {
                    changed = true;
                    _data[i].Cluster = _data[i].Cluster = newClusterId;
                    sb.AppendLine("Data Point >> R: " + _data[i].R + ", G: " +
                    _data[i].G + ", B: " +
                    _data[i].B + " moved to Cluster # " + newClusterId);
                }
                else
                {
                    sb.AppendLine("No change.");
                }
                sb.AppendLine("------------------------------");
               
            }
            if (changed == false)
                return false;
            if (EmptyCluster(_data)) return false;
            return true;
        }
        private int MinIndex(double[] distances)
        {
            int _indexOfMin = 0;
            double _smallDist = distances[0];
            for (int k = 0; k < distances.Length; ++k)
            {
                if (distances[k] < _smallDist)
                {
                    _smallDist = distances[k];
                    _indexOfMin = k;
                }
            }
            return _indexOfMin;
        }
        public void MakeCluster(List<RGBColor> data, int numDataPoints)
        {
            bool _changed = true;
            bool _success = true;
            Initialize(numDataPoints);

            int maxIteration = data.Count * 100000;
            int _threshold = 0;
            while (_success == true && _changed == true && _threshold < maxIteration)
            {
                ++_threshold;
                _success = UpdateColorMeans();
                _changed = UpdateClusterMembership();
            }
        }


    }
}
