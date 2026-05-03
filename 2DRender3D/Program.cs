using Raylib_cs;
using System.Numerics;
using System.Runtime.Intrinsics.X86;

namespace _2Drender3D;

class Program
{
    const int windowWidth = 1000;
    const int windowHeight = 800;
    const string windowTitle = "2DRender3D";
    const double camera_scale = 200;
    const int FPS = 60;

    public static List<_3Dvector> cube_vecs = new List<_3Dvector>();
    public static void Main()
    {
        cube_vecs.Add(new _3Dvector(-100, -100, 100));
        cube_vecs.Add(new _3Dvector(-100, 100, 100));
        cube_vecs.Add(new _3Dvector(100, -100, 100));
        cube_vecs.Add(new _3Dvector(100, 100, 100));
        cube_vecs.Add(new _3Dvector(-100, -100, 300));
        cube_vecs.Add(new _3Dvector(-100, 100, 300));
        cube_vecs.Add(new _3Dvector(100, -100, 300));
        cube_vecs.Add(new _3Dvector(100, 100, 300));

        Raylib.InitWindow(windowWidth, windowHeight, windowTitle);
        Raylib.SetTargetFPS(FPS);
        Vector2 last_mouse_pos = new Vector2();
        Vector2 new_mouse_pos = new Vector2();

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();

            new_mouse_pos = Raylib.GetMousePosition();

            
            DrawLine(0, 1);
            DrawLine(0, 2);
            DrawLine(0, 4);
            DrawLine(1, 3);
            DrawLine(1, 5);
            DrawLine(2, 3);
            DrawLine(2, 6);
            DrawLine(3, 7);
            DrawLine(4, 5);
            DrawLine(4, 6);
            DrawLine(5, 7);
            DrawLine(6, 7);
            

            Vector2 delta_pos = last_mouse_pos - new_mouse_pos;
            double dtheta = -delta_pos.X / 100;
            double dphi = -delta_pos.Y / 100;

            double cx = 0, cy = 0, cz = 200;
            for (int i = 0; i < cube_vecs.Count; i++)
            {
                cube_vecs[i].x -= cx;
                cube_vecs[i].y -= cy;
                cube_vecs[i].z -= cz;

                double tmpx = cube_vecs[i].x, tmpz = cube_vecs[i].z;
                cube_vecs[i].x = tmpx * Math.Cos(dtheta) - tmpz * Math.Sin(dtheta);
                cube_vecs[i].z = tmpz * Math.Cos(dtheta) + tmpx * Math.Sin(dtheta);

                double tmpy = cube_vecs[i].y;
                tmpz = cube_vecs[i].z;
                cube_vecs[i].y = tmpy * Math.Cos(dphi) - tmpz * Math.Sin(dphi);
                cube_vecs[i].z = tmpz * Math.Cos(dphi) + tmpy * Math.Sin(dphi);

                cube_vecs[i].x += cx;
                cube_vecs[i].y += cy;
                cube_vecs[i].z += cz;
            }

            Raylib.ClearBackground(Color.White);
            last_mouse_pos = Raylib.GetMousePosition();
            Raylib.EndDrawing();
        }
    }

    public static _2Dvector Convert3DVTo2DV(_3Dvector v3)
    {
        _2Dvector resv = new _2Dvector();
        resv.x = v3.x / v3.z * camera_scale;
        resv.y = v3.y / v3.z * camera_scale;
        return resv;
    }

    public static int Convert2DVToPosx(_2Dvector v)
    {
        int x = Convert.ToInt32(v.x) + windowWidth / 2;
        return x;
    }

    public static int Convert2DVToPosy(_2Dvector v)
    {
        int y = Convert.ToInt32(v.y) + windowHeight / 2;
        return y;
    }

    public static void DrawLine(int i, int j)
    {
        if (cube_vecs[i].z < 0) return;
        if (cube_vecs[j].z < 0) return;
        _2Dvector vec1 = Convert3DVTo2DV(cube_vecs[i]);
        _2Dvector vec2 = Convert3DVTo2DV(cube_vecs[j]);
        int x1 = Convert2DVToPosx(vec1);
        int y1 = Convert2DVToPosy(vec1);
        int x2 = Convert2DVToPosx(vec2);
        int y2 = Convert2DVToPosy(vec2);
        Vector2 v1 = new Vector2(x1, y1);
        Vector2 v2 = new Vector2(x2, y2);
        Raylib.DrawLineEx(v1, v2, 5f, Color.Black);
    }
}

class _3Dvector
{
    public double x, y, z;
    public _3Dvector(double inx,  double iny, double inz)
    {
        x = inx;
        y = iny;
        z = inz;
    }
}

class _2Dvector
{
    public double x, y;
}