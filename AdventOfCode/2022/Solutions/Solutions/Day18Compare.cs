using System.Diagnostics;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class Day18Compare
{

    public static string InputPath = @"D:\GitHub\xexuxjy\AdventOfCode\2022\Solutions\Data\";

        Point3d[] moves = {
    new(-1,0,0),
    new(1,0,0),
    new(0,-1,0),
    new(0,1,0),
    new(0,0,-1),
    new(0,0,1),
};

    int[,,] space;

    public void DoTest()
    {
        var lines = File.ReadAllLines(InputPath+"puzzle-18-input.txt");
        var cubes = lines.Select(l => Point3d.Make(l)).ToArray();


        var (origin, upper) = FindBoundingBox(cubes);
        space = new int[upper.x, upper.y, upper.z];

        // origin is not (0,0,0) to keep a border for filling, so adjust cubes for it
        //
        cubes = cubes.Select(c => c - origin).ToArray();

        // draw the cubes
        //
        foreach (var c in cubes)
        {
            var (x, y, z) = c;
            space[x, y, z] = 256;
        }

        // all sides facing empty, both in and out
        //
        int all_sides = count_sides_byval(cubes, 0);

        fill(new(0, 0, 0), upper - new Point3d(1, 1, 1), 1);

        // all sides facing outside
        //
        int external_sides = count_sides_byval(cubes, 1);

        Console.WriteLine("Sides facing empty: {0}, facing outside: {1}.", all_sides, external_sides);
    }

    (Point3d lower, Point3d upper) FindBoundingBox(IEnumerable<Point3d> points)
    {
        var (xmin, xmax, ymin, ymax, zmin, zmax) = (1000, 0, 1000, 0, 1000, 0);

        foreach (var (x, y, z) in points)
        {
            xmin = Math.Min(xmin, x);
            ymin = Math.Min(ymin, y);
            zmin = Math.Min(zmin, z);

            xmax = Math.Max(xmax, x);
            ymax = Math.Max(ymax, y);
            zmax = Math.Max(zmax, z);
        }

        // leave a border around for easier filling
        //
        Point3d origin = new(xmin - 1, ymin - 1, zmin - 1);
        Point3d upper = new(xmax - (xmin - 1) + 2, ymax - (ymin - 1) + 2, zmax - (zmin - 1) + 2);

        return (origin, upper);
    }


    int count_sides_byval(IEnumerable<Point3d> anchors, int testval)
    {
        int sidecnt = 0;

        foreach (var (x, y, z) in anchors)
        {
            foreach (var sc in moves)
            {
                var (x1, y1, z1) = (x + sc.x, y + sc.y, z + sc.z);
                if (space[x1, y1, z1] == testval) sidecnt++;
            }
        }

        return sidecnt;
    }


    void fill(Point3d start, Point3d limit, int v)
    {
        Stack<Point3d> stack = new();
        stack.Push(start);

        while (stack.TryPop(out var pt))
        {
            var (x, y, z) = pt;
            space[x, y, z] = v;

            foreach (var sc in moves)
            {
                pt = new Point3d(x + sc.x, y + sc.y, z + sc.z);

                if (pt < start || pt > limit) continue;

                if (space[pt.x, pt.y, pt.z] == 0) stack.Push(pt);
            }
        }
    }
}

record struct Point3d(int x, int y, int z)
{
    public static Point3d Make(string text)
    {
        var sa = text.Split(',');
        return new(parse(0), parse(1), parse(2));

        int parse(int ix) => int.Parse(sa[ix]);
    }

    public static Point3d operator +(Point3d a, Point3d b) => new(a.x + b.x, a.y + b.y, a.z + b.z);
    public static Point3d operator -(Point3d a, Point3d b) => new(a.x - b.x, a.y - b.y, a.z - b.z);

    public static bool operator >(Point3d a, Point3d b) => a.x > b.x || a.y > b.y || a.z > b.z;
    public static bool operator <(Point3d a, Point3d b) => a.x < b.x || a.y < b.y || a.z < b.z;
}
