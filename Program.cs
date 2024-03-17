using NetTopologySuite.Geometries;
using NetTopologySuite.GeometriesGraph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointsCloudConverter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string pointCloudFilePath = "path_to_pointcloud_file.txt";
            string tempFilePath = "new_temp_file_path.txt";
            string regionPointFolderPath = "folder_path_with_linering_coordinates";

            FileStream tempStr = File.Create(tempFilePath);
            tempStr.Close();

            List<string> txtFiles = Directory.GetFiles(regionPointFolderPath, "*.txt").ToList();

            LinearRing genRing = CreateLinearRing(txtFiles.First(), ';');

            txtFiles.RemoveAt(0);

            List<LinearRing> holes = new List<LinearRing>();

            foreach (string file in txtFiles)
            {

                holes.Add(CreateLinearRing(file, ';'));

            }

            Polygon polygon = new Polygon(genRing, holes.ToArray());

            int count = 0;
            int newCount = 0;

            using (StreamReader sr = new StreamReader(pointCloudFilePath))
            using (StreamWriter writer = new StreamWriter(tempFilePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {

                    count++;

                    string[] parts = line.Split(' ');
                    double x = double.Parse(parts[1].Replace('.', ','));
                    double y = double.Parse(parts[2].Replace('.', ','));

                    Point pt = new Point(x, y);

                    if (polygon.Contains(pt))
                    {

                        newCount++;

                        writer.WriteLine(line);

                        Console.Write(count + " - " + newCount + "\r");
                    }

                }
            }

            Console.WriteLine("Готово!");
        }

        public static LinearRing CreateLinearRing(string txtFilePath, char delimiter)
        {
            LinearRing genRing;

            using (StreamReader sr = new StreamReader(txtFilePath))
            {
                string line;

                List<Coordinate> coordinates = new List<Coordinate>();

                while ((line = sr.ReadLine()) != null)
                {
                    string[] parts = line.Split(delimiter);
                    double x = double.Parse(parts[0].Replace('.', ','));
                    double y = double.Parse(parts[1].Replace('.', ','));

                    coordinates.Add(new Coordinate(x, y));
                }

                Coordinate[] points = coordinates.ToArray();

                genRing = new LinearRing(points);
            }
            return genRing;
        }
    }
}

